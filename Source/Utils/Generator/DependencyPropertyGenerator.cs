using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace EHunter.SourceGenerator
{
    [Generator]
    internal class DependencyPropertyGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
            => context.RegisterForSyntaxNotifications(() => new AttributeReceiver("EHunter.DependencyPropertyAttribute"));

        public void Execute(GeneratorExecutionContext context)
        {
            var receiver = (AttributeReceiver)context.SyntaxContextReceiver!;

            var attributeType = context.Compilation.GetTypeByMetadataName("EHunter.DependencyPropertyAttribute");
            var @object = context.Compilation.GetSpecialType(SpecialType.System_Object);
            if (attributeType is null)
            {
                return;
            }

            foreach (var classDeclaration in receiver.CandidateClasses)
            {
                var semanticModel = context.Compilation.GetSemanticModel(classDeclaration.SyntaxTree);
                var @class = semanticModel.GetDeclaredSymbol(classDeclaration);

                if (@class is null)
                    continue;

                var members = new List<MemberDeclarationSyntax>();
                var namespaces = new HashSet<string>
                {
                    "Microsoft.UI.Xaml"
                };
                var usedTypes = new HashSet<ITypeSymbol>(SymbolEqualityComparer.Default);

                foreach (var attribute in @class.GetAttributes())
                {
                    if (!SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, attributeType))
                        continue;

                    if (attribute.ConstructorArguments[0].Value is not string propertyName)
                        continue;

                    if (attribute.ConstructorArguments[1].Value is not INamedTypeSymbol type)
                        continue;

                    bool isSetterPublic = true;
                    string? defaultValue = null;
                    bool isNullable = false;
                    string? changedMethod = null;

                    foreach (var namedArgument in attribute.NamedArguments)
                    {
                        switch (namedArgument)
                        {
                            case { Key: "IsSetterPublic", Value: { Value: bool value } }:
                                isSetterPublic = value;
                                break;
                            case { Key: "DefaultValue", Value: { Value: string value } }:
                                defaultValue = value;
                                break;
                            case { Key: "IsNullable", Value: { Value: bool value } }:
                                isNullable = value;
                                break;
                            case { Key: "ChangedMethod", Value: { Value: string value } }:
                                changedMethod = value;
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

                    if (changedMethod is not null)
                        metadataCreation = metadataCreation
                            .AddArgumentListArguments(
                                Argument(IdentifierName(changedMethod))
                            );

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
                    if (!SymbolEqualityComparer.Default.Equals(type, @object))
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

                    string fileName = @class.Name + ".g.cs";
                    context.AddSource(fileName, SyntaxTree(compilationUnit, encoding: Encoding.UTF8).GetText());
                }
            }
        }
    }
}
