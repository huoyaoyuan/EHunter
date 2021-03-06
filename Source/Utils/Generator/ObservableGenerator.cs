﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace EHunter.SourceGenerator
{
    [Generator]
    internal class ObservableGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
            => context.RegisterForSyntaxNotifications(() => new AttributeReceiver());

        public void Execute(GeneratorExecutionContext context)
        {
            var receiver = (AttributeReceiver)context.SyntaxContextReceiver!;

            var attributeType = context.Compilation.GetTypeByMetadataName("EHunter.ObservablePropertyAttribute");
            if (attributeType is null)
            {
                return;
            }

            static TypeSyntax GetTypeSyntax(ITypeSymbol typeSymbol, bool isNullable)
            {
                var typeName = ParseTypeName(typeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));
                return isNullable ? NullableType(typeName) : typeName;
            }

            foreach (var classDeclaration in receiver.CandidateClasses)
            {
                var semanticModel = context.Compilation.GetSemanticModel(classDeclaration.SyntaxTree);
                var @class = semanticModel.GetDeclaredSymbol(classDeclaration);

                if (@class is null)
                    continue;

                var members = new List<MemberDeclarationSyntax>();
                var namespaces = new HashSet<string>();
                var usedTypes = new HashSet<ITypeSymbol>(SymbolEqualityComparer.Default);

                void UseNamespace(ITypeSymbol symbol)
                {
                    if (usedTypes.Contains(symbol))
                        return;

                    usedTypes.Add(symbol);

                    var ns = symbol.ContainingNamespace;
                    if (!SymbolEqualityComparer.Default.Equals(ns, @class!.ContainingNamespace))
                        namespaces!.Add(ns.ToDisplayString());

                    if (symbol is INamedTypeSymbol { IsGenericType: true } genericSymbol)
                    {
                        foreach (var a in genericSymbol.TypeArguments)
                            UseNamespace(a);
                    }
                }

                foreach (var attribute in @class.GetAttributes())
                {
                    if (!SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, attributeType))
                        continue;

                    if (attribute.ConstructorArguments[0].Value is not string propertyName)
                        continue;

                    if (attribute.ConstructorArguments[1].Value is not INamedTypeSymbol type)
                        continue;

                    bool isSetterPublic = true;
                    string? initializer = null;
                    bool isNullable = false;
                    string? changedAction = null;

                    foreach (var namedArgument in attribute.NamedArguments)
                    {
                        switch (namedArgument)
                        {
                            case { Key: "IsSetterPublic", Value: { Value: bool value } }:
                                isSetterPublic = value;
                                break;
                            case { Key: "Initializer", Value: { Value: string value } }:
                                initializer = value;
                                break;
                            case { Key: "IsNullable", Value: { Value: bool value } }:
                                isNullable = value;
                                break;
                            case { Key: "ChangedAction", Value: { Value: string value } }:
                                changedAction = value;
                                break;
                        }
                    }

                    var sb = new StringBuilder(propertyName.Length + 1)
                        .Append('_')
                        .Append(propertyName);
                    sb[1] = char.ToLowerInvariant(sb[1]);
                    string fieldName = sb.ToString();

                    UseNamespace(type);

                    var variableDeclaration = VariableDeclarator(fieldName);
                    if (initializer != null)
                        variableDeclaration = variableDeclaration
                            .WithInitializer(EqualsValueClause(
                                ParseExpression(initializer)
                                ));
                    var fieldDeclaration = FieldDeclaration(VariableDeclaration(GetTypeSyntax(type, isNullable)))
                        .AddModifiers(Token(SyntaxKind.PrivateKeyword))
                        .AddDeclarationVariables(variableDeclaration);

                    var getter = AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                        .WithExpressionBody(ArrowExpressionClause(
                            IdentifierName(fieldName)
                            ))
                        .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
                    var setProperty = InvocationExpression(IdentifierName("SetProperty"))
                        .AddArgumentListArguments(
                            Argument(IdentifierName(fieldName))
                                .WithRefKindKeyword(Token(SyntaxKind.RefKeyword)),
                            Argument(IdentifierName("value"))
                        );
                    var setter = changedAction is null
                        ? AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                            .WithExpressionBody(ArrowExpressionClause(setProperty))
                            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                        : AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                            .WithBody(Block(
                                IfStatement(setProperty, ParseStatement(changedAction))
                            ));
                    if (!isSetterPublic)
                        setter = setter.AddModifiers(Token(SyntaxKind.PrivateKeyword));
                    var propertyDeclaration = PropertyDeclaration(GetTypeSyntax(type, isNullable), propertyName)
                        .AddModifiers(Token(SyntaxKind.PublicKeyword))
                        .AddAccessorListAccessors(getter, setter);

                    members.Add(fieldDeclaration);
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

                    string fileName = @class.Name + ".g.cs";
                    context.AddSource(fileName, SyntaxTree(compilationUnit, encoding: Encoding.UTF8).GetText());
                }
            }
        }
    }
}
