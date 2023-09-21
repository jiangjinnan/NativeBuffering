using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace NativeBuffering.Generator
{
    [Generator]
    public class Generator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxReceiver is not SyntaxReceiver receiver)
            {
                return;
            }

            foreach (var node in receiver.Candidates)
            {
                var model = context.Compilation.GetSemanticModel(node.SyntaxTree);
                if (model.GetDeclaredSymbol(node, context.CancellationToken) is not ITypeSymbol typeSymbol)
                {
                    continue;
                }
                if (!typeSymbol.GetAttributes().Any(it => it.AttributeClass?.Name == "BufferedMessageSourceAttribute"))
                {
                    continue;
                }

                var metadata = BufferedObjectMetadataResolver.Resolve(typeSymbol);
                var generationContext = new CodeGenerationContext();
                Generate(typeSymbol, generationContext, (_, ctx) => new BufferedMessageSourceClassGenerator().Generate(metadata, ctx));
                context.AddSource($"{typeSymbol.GetFullName()}.g.cs", SourceText.From(generationContext.SourceCode!, Encoding.UTF8));

                generationContext = new CodeGenerationContext();
                Generate(typeSymbol, generationContext, (_, ctx) => new BufferedMessageClassGenerator().Generate(metadata, ctx));
                context.AddSource($"{metadata.BufferedMessageClassFullName}.g.cs", SourceText.From(generationContext.SourceCode!, Encoding.UTF8));
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            //Debugger.Launch();
            //Debugger.Break();
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        public void Generate(ITypeSymbol typeSymbol, CodeGenerationContext context, Action<ITypeSymbol, CodeGenerationContext> generator)
        {
            context.WriteLines("#nullable enable");
            context.WriteLines("using System;");
            context.WriteLines("using System.Runtime.CompilerServices;");
            context.WriteLines("using System.Collections.Generic;");
            context.WriteLines("using NativeBuffering;", "using NativeBuffering.Dictionaries;", "using NativeBuffering.Collections;");

            var stack = new Stack<ISymbol>();
            var namespaceOrTypes = typeSymbol.GetContainingNamespaceAndTypes();
            foreach (var item in namespaceOrTypes)
            {
                stack.Push(item);
                if (item is INamespaceSymbol)
                {
                    break;
                }
            }
            WriteClass();
            context.WriteLines("#nullable disable");

            void WriteClass()
            {
                while (stack.Any())
                {
                    var symbol = stack.Pop();
                    if (symbol is INamespaceSymbol @namespace && !string.IsNullOrWhiteSpace(@namespace.Name))
                    {
                        context.WriteLines($"namespace {@namespace}");
                        using (context.CodeBlock())
                        {
                            WriteClass();
                        }
                    }

                    if (symbol is INamedTypeSymbol typeSymbol)
                    {
                        if (stack.Any()) // outer class
                        {
                            context.WriteLines($"{typeSymbol.DeclaredAccessibility.ToString().ToLower()} partial class {typeSymbol.Name}");
                            using (context.CodeBlock())
                            {
                                WriteClass();
                            }
                        }
                        else
                        {
                            generator(typeSymbol, context);
                        }
                    }
                }
            }
        }

        private class SyntaxReceiver : ISyntaxReceiver
        {
            public List<SyntaxNode> Candidates { get; } = new List<SyntaxNode>();

            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if (syntaxNode is ClassDeclarationSyntax || syntaxNode is RecordDeclarationSyntax || syntaxNode is StructDeclarationSyntax)
                {
                    Candidates.Add(syntaxNode);
                }
            }
        }
    }
}
