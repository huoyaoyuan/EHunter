using System.Collections.Generic;
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

            static TypeSyntax GetTypeSyntax(ITypeSymbol typeSymbol)
                 => ParseTypeName(typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));

            foreach (var classDeclaration in receiver.CandidateClasses)
            {
                var semanticModel = context.Compilation.GetSemanticModel(classDeclaration.SyntaxTree);
                var @class = semanticModel.GetDeclaredSymbol(classDeclaration);

                if (@class is null)
                    continue;

                var members = new List<MemberDeclarationSyntax>();

                foreach (var attribute in @class.GetAttributes())
                {
                    if (!SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, attributeType))
                        continue;

                    if (attribute.ConstructorArguments[0].Value is not string propertyName)
                        continue;

                    if (attribute.ConstructorArguments[1].Value is not INamedTypeSymbol type)
                        continue;

                    bool isSetterPublic = true;

                    foreach (var namedArgument in attribute.NamedArguments)
                    {
                        switch (namedArgument)
                        {
                            case { Key: "IsSetterPublic", Value: { Value: bool isSetterPublicValue } }:
                                isSetterPublic = isSetterPublicValue;
                                break;
                        }
                    }

                    var sb = new StringBuilder(propertyName.Length + 1)
                        .Append('_')
                        .Append(propertyName);
                    sb[1] = char.ToLowerInvariant(sb[1]);
                    string fieldName = sb.ToString();

                    var fieldDeclaration = FieldDeclaration(VariableDeclaration(GetTypeSyntax(type)))
                        .AddModifiers(Token(SyntaxKind.PrivateKeyword))
                        .AddDeclarationVariables(VariableDeclarator(fieldName));

                    var getter = AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                        .WithExpressionBody(ArrowExpressionClause(
                            IdentifierName(fieldName)
                            ))
                        .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
                    var setter = AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                        .WithExpressionBody(ArrowExpressionClause(
                            InvocationExpression(IdentifierName("SetProperty"))
                                .AddArgumentListArguments(
                                    Argument(IdentifierName(fieldName))
                                        .WithRefKindKeyword(Token(SyntaxKind.RefKeyword)),
                                    Argument(IdentifierName("value"))
                                )
                            ))
                        .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
                    if (!isSetterPublic)
                        setter = setter.AddModifiers(Token(SyntaxKind.PrivateKeyword));
                    var propertyDeclaration = PropertyDeclaration(GetTypeSyntax(type), propertyName)
                        .AddModifiers(Token(SyntaxKind.PublicKeyword))
                        .AddAccessorListAccessors(getter, setter);

                    members.Add(fieldDeclaration);
                    members.Add(propertyDeclaration);
                }

                if (members.Count > 0)
                {
                    var generatedClass = ClassDeclaration(@class.Name)
                        .AddModifiers(Token(SyntaxKind.PartialKeyword))
                        .AddMembers(members.ToArray());
                    var generatedNamespace = NamespaceDeclaration(ParseName(
                        @class.ContainingNamespace.ToDisplayString(
                            SymbolDisplayFormat.FullyQualifiedFormat
                                .WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted)
                            )
                        ))
                        .AddMembers(generatedClass);
                    var compilationUnit = CompilationUnit()
                        .AddMembers(generatedNamespace)
                        .NormalizeWhitespace();

                    string fileName = @class.Name + ".g.cs";
                    context.AddSource(fileName, SyntaxTree(compilationUnit, encoding: Encoding.UTF8).GetText());
                }
            }
        }
    }
}
