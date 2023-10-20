using Microsoft.CodeAnalysis;

namespace NativeBuffering.Generator
{
    internal class BufferedMessageSourceClassGenerator
    {
        public void Generate(BufferedObjectMetadata metadata, CodeGenerationContext context)
        {
            if(metadata.TypeSymbol.IsValueType)
            {
                context.WriteLines($"public partial struct {metadata.TypeSymbol.Name} : IBufferedObjectSource");
            }
            else
            {
                context.WriteLines($"public partial class {metadata.TypeSymbol.Name} : IBufferedObjectSource");
            }
            using (context.CodeBlock())
            {
                GenerateCalculateSizeMethod(metadata, context);
                GenerateWriteMethod(metadata, context);
                GenerateConstructor(metadata, context);
            }
        }

        private void GenerateCalculateSizeMethod(BufferedObjectMetadata metadata, CodeGenerationContext context)
        {
            context.WriteLines("public int CalculateSize()");
            using (context.CodeBlock())
            {
                //context.WriteLines("var context = BufferedObjectWriteContext.CreateForSizeCalculation();");
                context.WriteLines("var context = BufferedObjectWriteContext.AcquireForSizeCalculation();");
                context.WriteLines("Write(context);");
                context.WriteLines("var result = context.Position;");
                context.WriteLines("BufferedObjectWriteContext.Release(context);");
                context.WriteLines("return result;");
            }
        }

        private void GenerateWriteMethod(BufferedObjectMetadata metadata, CodeGenerationContext context)
        {
            var properties = metadata.Properties;
            context.WriteLines("public void Write(BufferedObjectWriteContext context)");
            using (context.CodeBlock())
            {
                //context.WriteLines("using var scope = new BufferedObjectWriteContextScope(context);");
                //context.WriteLines($"var scope = new BufferedObjectWriteContextScope(context, {metadata.Properties.Length});");
                context.WriteLines($"var scope = context.GetWriteContextScope({metadata.Properties.Length});");
                for (int index = 0; index < properties.Length; index++)
                {
                    var property = properties[index];
                    var propertyName = property.PropertySymbol.Name;
                    var propertyType = property.PropertySymbol.Type;

                    _ = property.Kind switch
                    {
                        BufferedMessageMemberKind.Primitive => context.WriteLines($"scope.WriteUnmanagedField({propertyName});"),
                        BufferedMessageMemberKind.Unmanaged => context.WriteLines($"scope.WriteUnmanagedField({propertyName});"),
                        BufferedMessageMemberKind.String => context.WriteLines($"scope.WriteStringField({propertyName});"),
                        BufferedMessageMemberKind.Binary => context.WriteLines($"scope.WriteBinaryField({propertyName});"),
                        BufferedMessageMemberKind.Message => context.WriteLines($"scope.WriteBufferedObjectField({propertyName});"),

                        BufferedMessageMemberKind.UnmanagedUnmanagedDictionary => property.ValueType!.IsNullable(out _)
                        ? context.WriteLines($"scope.WriteUnmanagedNullableUnmanagedDictionaryField({propertyName});")
                        : context.WriteLines($"scope.WriteUnmanagedNonNullableUnmanagedDictionaryField({propertyName});"),
                        BufferedMessageMemberKind.UnmanagedMessageDictionary => context.WriteLines($"scope.WriteUnmanagedBufferedObjectDictionaryField({propertyName});"),
                        BufferedMessageMemberKind.UnmanagedStringDictionary => context.WriteLines($"scope.WriteUnmanagedStringDictionaryField({propertyName});"),
                        BufferedMessageMemberKind.UnmanagedBinaryDictionary => context.WriteLines($"scope.WriteUnmanagedBinaryDictionaryField({propertyName});"),

                        BufferedMessageMemberKind.StringUnmanagedDictionary => property.ValueType!.IsNullable(out _)
                        ? context.WriteLines($"scope.WriteStringNullableUnmanagedDictionaryField({propertyName});")
                        : context.WriteLines($"scope.WriteStringNonNullableUnmanagedDictionaryField({propertyName});"),
                        BufferedMessageMemberKind.StringStringDictionary => context.WriteLines($"scope.WriteStringStringDictionaryField({propertyName});"),
                        BufferedMessageMemberKind.StringMessageDictionary => context.WriteLines($"scope.WriteStringBufferedObjectDictionaryField({propertyName});"),
                        BufferedMessageMemberKind.StringBinaryDictionary => context.WriteLines($"scope.WriteStringBinaryDictionaryField({propertyName});"),

                        BufferedMessageMemberKind.UnmanagedCollection => context.WriteLines($"scope.WriteUnmanagedCollectionField({propertyName});"),
                        BufferedMessageMemberKind.StringCollection => context.WriteLines($"scope.WriteStringCollectionField({propertyName});"),
                        BufferedMessageMemberKind.MessageCollection => context.WriteLines($"scope.WriteBufferedObjectCollectionField({propertyName});"),
                        BufferedMessageMemberKind.BinaryCollection => context.WriteLines($"scope.WriteBinaryCollectionField({propertyName});"),

                        _ => throw new NotSupportedException("Will never hit here!")
                    }; ;
                }
                context.WriteLines("context.ReleaseWriteContextScope(scope);");
            }
        }

        private void GenerateConstructor(BufferedObjectMetadata metadata, CodeGenerationContext context)
        {
            if (metadata.TypeSymbol is INamedTypeSymbol namedTypeSymbol)
            { 
                if (!namedTypeSymbol.Constructors.Any(it=>it.Parameters.Length ==0 && !it.IsStatic && it.DeclaredAccessibility == Accessibility.Public))
                {
                    context.WriteLines($"public {metadata.TypeSymbol.Name}() {{ }}");                              
                }
            }
        }
    }
}
