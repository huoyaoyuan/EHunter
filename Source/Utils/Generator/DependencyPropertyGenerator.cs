﻿using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace EHunter.SourceGenerator
{
    [Generator]
    internal class DependencyPropertyGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var classTargets = context.SyntaxProvider
                .CreateSyntaxProvider(
                    (s, _) => s is ClassDeclarationSyntax { AttributeLists.Count: > 0 },
                    (c, cancellationToken) =>
                    {
                        var classDeclaration = (ClassDeclarationSyntax)c.Node;

                        foreach (var attrList in classDeclaration.AttributeLists)
                            foreach (var attr in attrList.Attributes)
                                if (c.SemanticModel.GetSymbolInfo(attr, cancellationToken).Symbol is IMethodSymbol methodSymbol &&
                                    methodSymbol.ContainingType.ToDisplayString() == "EHunter.DependencyPropertyAttribute")
                                {
                                    return c.SemanticModel.GetDeclaredSymbol(classDeclaration, cancellationToken: cancellationToken);
                                }

                        return null;
                    })
                .Where(x => x is not null);

            context.RegisterSourceOutput(classTargets, (context, classDeclaration) => Execute(context, classDeclaration!));
        }

        public static void Execute(SourceProductionContext context, INamedTypeSymbol @class)
        {
            var members = new List<MemberDeclarationSyntax>();
            var namespaces = new HashSet<string>
                {
                    "Microsoft.UI.Xaml"
                };
            var usedTypes = new HashSet<ITypeSymbol>(SymbolEqualityComparer.Default);

            foreach (var attribute in @class.GetAttributes())
            {
                if (attribute.AttributeClass?.ToDisplayString() != "EHunter.DependencyPropertyAttribute")
                    continue;

                if (attribute.ConstructorArguments[0].Value is not string propertyName)
                    continue;

                if (attribute.ConstructorArguments[1].Value is not INamedTypeSymbol type)
                    continue;

                bool isSetterPublic = true;
                string? defaultValue = null;
                bool isNullable = false;
                bool instanceChangedCallback = false;

                foreach (var namedArgument in attribute.NamedArguments)
                {
                    switch (namedArgument)
                    {
                        case { Key: "IsSetterPublic", Value.Value: bool value }:
                            isSetterPublic = value;
                            break;
                        case { Key: "DefaultValue", Value.Value: string value }:
                            defaultValue = value;
                            break;
                        case { Key: "IsNullable", Value.Value: bool value }:
                            isNullable = value;
                            break;
                        case { Key: "InstanceChangedCallback", Value.Value: bool value }:
                            instanceChangedCallback = value;
                            break;
                    }
                }

                string fieldName = propertyName + "Property";

                namespaces.UseNamespace(usedTypes, @class, type);

                var defaultValueExpression
                    = defaultValue is null
                    ? LiteralExpression(SyntaxKind.NullLiteralExpression)
                    : ParseExpression(defaultValue);

                var metadataCreation = ObjectCreationExpression(
                        IdentifierName("PropertyMetadata")
                    )
                    .AddArgumentListArguments(
                        Argument(defaultValueExpression)
                    );

                if (instanceChangedCallback)
                {
                    string partialMethodName = $"On{propertyName}Changed";

                    var oldValueExpression = CastExpression(
                        type.GetTypeSyntax(isNullable),
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName("e"),
                            IdentifierName("OldValue")
                            )
                        );
                    var newValueExpression = CastExpression(
                        type.GetTypeSyntax(isNullable),
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName("e"),
                            IdentifierName("NewValue")
                            )
                        );
                    var lambdaBody = InvocationExpression(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            ParenthesizedExpression(CastExpression(
                                @class.GetTypeSyntax(false),
                                IdentifierName("d")
                                )),
                            IdentifierName(partialMethodName)
                            )
                        )
                        .AddArgumentListArguments(
                            Argument(oldValueExpression),
                            Argument(newValueExpression)
                        );
                    metadataCreation = metadataCreation
                        .AddArgumentListArguments(
                            Argument(ParenthesizedLambdaExpression()
                                .AddParameterListParameters(
                                    Parameter(Identifier("d")),
                                    Parameter(Identifier("e"))
                                )
                                .WithExpressionBody(lambdaBody)
                                )
                        );

                    var partialMethod = MethodDeclaration(
                        PredefinedType(Token(SyntaxKind.VoidKeyword)),
                        partialMethodName
                        )
                        .AddParameterListParameters(
                            Parameter(Identifier("oldValue"))
                                .WithType(type.GetTypeSyntax(isNullable)),
                            Parameter(Identifier("newValue"))
                                .WithType(type.GetTypeSyntax(isNullable))
                        )
                        .AddModifiers(
                            Token(SyntaxKind.PartialKeyword)
                        )
                        .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
                    members.Add(partialMethod);
                }

                var registration = InvocationExpression(
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        IdentifierName("DependencyProperty"),
                        IdentifierName("Register"))
                    )
                    .AddArgumentListArguments(
                        Argument(LiteralExpression(
                            SyntaxKind.StringLiteralExpression,
                            Literal(propertyName)
                            )),
                        Argument(TypeOfExpression(type.GetTypeSyntax(false))),
                        Argument(TypeOfExpression(@class.GetTypeSyntax(false))),
                        Argument(metadataCreation)
                    );

                var staticFieldDeclarator = VariableDeclarator(fieldName)
                    .WithInitializer(EqualsValueClause(
                        registration
                        ));

                var staticFieldDeclaration = FieldDeclaration(VariableDeclaration(IdentifierName("DependencyProperty")))
                    .AddModifiers(
                        Token(SyntaxKind.PublicKeyword),
                        Token(SyntaxKind.StaticKeyword),
                        Token(SyntaxKind.ReadOnlyKeyword)
                    )
                    .AddDeclarationVariables(staticFieldDeclarator);

                ExpressionSyntax getProperty = InvocationExpression(IdentifierName("GetValue"))
                    .AddArgumentListArguments(
                        Argument(IdentifierName(fieldName))
                    );
                getProperty = CastExpression(
                    type.GetTypeSyntax(isNullable),
                    getProperty
                    );
                var getter = AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                    .WithExpressionBody(ArrowExpressionClause(
                        getProperty
                        ))
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

                ExpressionSyntax setProperty = InvocationExpression(IdentifierName("SetValue"))
                    .AddArgumentListArguments(
                        Argument(IdentifierName(fieldName)),
                        Argument(IdentifierName("value"))
                    );
                var setter = AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                    .WithExpressionBody(ArrowExpressionClause(
                        setProperty
                        ))
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
                if (!isSetterPublic)
                    setter = setter.AddModifiers(Token(SyntaxKind.PrivateKeyword));

                var propertyDeclaration = PropertyDeclaration(type.GetTypeSyntax(isNullable), propertyName)
                    .AddModifiers(Token(SyntaxKind.PublicKeyword))
                    .AddAccessorListAccessors(getter, setter);

                members.Add(staticFieldDeclaration);
                members.Add(propertyDeclaration);
            }

            if (members.Count > 0)
            {
                var generatedClass = ClassDeclaration(@class.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat))
                    .AddModifiers(Token(SyntaxKind.PartialKeyword))
                    .AddMembers(members.ToArray());
                var generatedNamespace = NamespaceDeclaration(ParseName(
                        @class.ContainingNamespace.ToDisplayString()
                    ))
                    .AddMembers(generatedClass)
                    .WithNamespaceKeyword(
                        Token(SyntaxKind.NamespaceKeyword)
                            .WithLeadingTrivia(
                                Trivia(NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword), true))
                            )
                    );
                var compilationUnit = CompilationUnit()
                    .AddMembers(generatedNamespace)
                    .AddUsings(namespaces.Select(ns => UsingDirective(ParseName(ns))).ToArray())
                    .NormalizeWhitespace();

                string fileName = @class.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat
                    .WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted)) + ".g.cs";
                context.AddSource(fileName, SyntaxTree(compilationUnit, encoding: Encoding.UTF8).GetText());
            }
        }
    }
}
