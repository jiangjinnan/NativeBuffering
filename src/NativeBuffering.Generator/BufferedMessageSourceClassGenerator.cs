using Microsoft.CodeAnalysis;

namespace NativeBuffering.Generator
{
    internal class BufferedMessageSourceClassGenerator
    {
        public void Generate(BufferedObjectMetadata metadata, CodeGenerationContext context)
        {
            context.WriteLines($"public partial class {metadata.TypeSymbol.Name} : IBufferedObjectSource");
            using (context.CodeBlock())
            {
                GenerateCalculateSizeMethod(metadata, context);
                GenerateWriteMethod(metadata, context);
            }
        }

        private void GenerateCalculateSizeMethod(BufferedObjectMetadata metadata, CodeGenerationContext context)
        {
            var properties = metadata.Properties;
            context.WriteLines("public int CalculateSize()");
            using (context.CodeBlock())
            {
                context.WriteLines(" var size = 0;");
                for (int index = 0; index < properties.Length; index++)
                {
                    var property = properties[index];
                    var propertyName = property.PropertySymbol.Name;
                    var propertyType = property.PropertySymbol.Type;
                    if (propertyType.IsUnmanagedType)
                    {
                        context.WriteLines($"size += NativeBuffering.Utilities.CalculateUnmanagedFieldSize({propertyName});");
                        continue;
                    }

                    if (propertyType.IsString())
                    {
                        context.WriteLines($"size += NativeBuffering.Utilities.CalculateStringFieldSize({propertyName});");
                        continue;
                    }

                    if (propertyType.IsBinary())
                    {
                        context.WriteLines($"size += NativeBuffering.Utilities.CalculateBinaryFieldSize({propertyName});");
                        continue;
                    }

                    if (propertyType.IsBufferedMessageSource(out _))
                    {                        
                         context.WriteLines($"size += NativeBuffering.Utilities.CalculateBufferedObjectFieldSize({propertyName});");
                        continue;
                    }

                    if (propertyType.IsDictionary(out _, out _))
                    {
                        context.WriteLines($"size += NativeBuffering.Utilities.CalculateDictionaryFieldSize({propertyName});");
                        continue;
                    }

                    context.WriteLines($"size += NativeBuffering.Utilities.CalculateCollectionFieldSize({propertyName});");
                }
                context.WriteLines("return size;");
            }
        }

        private void GenerateWriteMethod(BufferedObjectMetadata metadata, CodeGenerationContext context)
        {
            var properties = metadata.Properties;
            context.WriteLines("public void Write(BufferedObjectWriteContext context)");
            using (context.CodeBlock())
            {
                context.WriteLines("using var scope = new BufferedObjectWriteContextScope(context);");
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

                        BufferedMessageMemberKind.UnmanagedUnmanagedDictionary => context.WriteLines($"scope.WriteUnmanagedUnmanagedDictionaryField({propertyName});"),
                        BufferedMessageMemberKind.UnmanagedMessageDictionary => context.WriteLines($"scope.WriteUnmanagedBufferedObjectDictionaryField({propertyName});"),
                        BufferedMessageMemberKind.UnmanagedStringDictionary => context.WriteLines($"scope.WriteUnmanagedStringDictionaryField({propertyName});"),
                        BufferedMessageMemberKind.UnmanagedBinaryDictionary => context.WriteLines($"scope.WriteUnmanagedBinaryDictionaryField({propertyName});"),

                        BufferedMessageMemberKind.StringUnmanagedDictionary => context.WriteLines($"scope.WriteStringUnmanagedDictionaryField({propertyName});"),
                        BufferedMessageMemberKind.StringStringDictionary => context.WriteLines($"scope.WriteStringStringDictionaryField({propertyName});"),
                        BufferedMessageMemberKind.StringMessageDictionary => context.WriteLines($"scope.WriteStringBufferedObjectDictionaryField({propertyName});"),
                        BufferedMessageMemberKind.StringBinaryDictionary => context.WriteLines($"scope.WriteStringBinaryDictionaryField({propertyName});"),

                        BufferedMessageMemberKind.UnmanagedCollection => context.WriteLines($"scope.WriteUnmanagedCollectionField({propertyName});"),
                        BufferedMessageMemberKind.StringCollection => context.WriteLines($"scope.WriteStringCollectionField({propertyName});"),
                        BufferedMessageMemberKind.MessageCollection => context.WriteLines($"scope.WriteBufferedObjectCollectionField({propertyName});"),
                        BufferedMessageMemberKind.BinaryCollection => context.WriteLines($"scope.WriteBinaryCollectionField({propertyName});"),

                        _ => throw new NotSupportedException("Will never hit here!")
                    };
                }
            }
        }
    }
}
