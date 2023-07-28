using System;
using System.Collections.Generic;
using System.Text;

namespace NativeBuffering.Generator
{
    internal class BufferedMessageClassGenerator
    {
        public void Generate(BufferedObjectMetadata metadata, CodeGenerationContext context)
        {
            context.WriteLines($"public unsafe readonly struct {metadata.BufferedMessageClassName} : IReadOnlyBufferedObject<{metadata.BufferedMessageClassName}>");
            using (context.CodeBlock())
            {
                context.WriteLines("public NativeBuffer Buffer { get; }");
                context.WriteLines($"public {metadata.BufferedMessageClassName}(NativeBuffer buffer) => Buffer = buffer;");
                context.WriteLines($"public static {metadata.BufferedMessageClassName} Parse(NativeBuffer buffer) => new {metadata.BufferedMessageClassName}(buffer);");

                var properties = metadata.Properties;   
                for(var index = 0; index< properties.Length; index++) 
                {
                    var property = properties[index];
                    var propertyType = property.PropertySymbol.Type;
                    var propertyName = property.PropertySymbol.Name;
                    string? bufferedMessageTypeName;

                    #region Dictionary
                    if (propertyType.IsDictionary(out var keyTypeSymbol, out var valueTypeSymbol))
                    { 
                        var keyFullTypeName = keyTypeSymbol!.GetFullName();
                        var valueFullTypeName = valueTypeSymbol!.GetFullName();
                        if (valueTypeSymbol!.IsString())
                        {
                            valueFullTypeName = "BufferedString";
                        }
                        if (valueTypeSymbol!.IsBinary())
                        {
                            valueFullTypeName = "BufferedBinary";
                        }
                        if(valueTypeSymbol!.IsBufferedMessageSource(out  bufferedMessageTypeName))
                        {
                            valueFullTypeName = bufferedMessageTypeName;
                        }

                        if (keyTypeSymbol!.IsUnmanagedType)
                        {
                            if (valueTypeSymbol!.IsUnmanagedType)
                            {
                                context.WriteLines($"public ReadOnlyUnmanagedUnmanagedDictionary<{keyFullTypeName}, {valueFullTypeName}> {propertyName} => Buffer.ReadUnmanagedUnmanagedDictionaryField<{keyFullTypeName}, {valueFullTypeName}>({index});");
                                continue;
                            }

                            context.WriteLines($"public ReadOnlyUnmanagedBufferedObjectDictionary<{keyFullTypeName}, {valueFullTypeName}> {propertyName} => Buffer.ReadUnmanagedBufferedObjectDictionaryField<{keyFullTypeName}, {valueFullTypeName}>({index});");
                            continue;
                        }

                        if (valueTypeSymbol!.IsUnmanagedType)
                        {
                            context.WriteLines($"public ReadOnlyStringUnmanagedDictionary<{valueFullTypeName}> {propertyName} => Buffer.ReadStringUnmanagedDictionaryField<{valueFullTypeName}>({index});");
                            continue;
                        }

                        context.WriteLines($"public ReadOnlyStringBufferedObjectDictionary<{valueFullTypeName}> {propertyName} => Buffer.ReadStringBufferedObjectDictionaryField<{valueFullTypeName}>({index});");
                        continue;
                    }
                    #endregion

                    #region Collection
                    if (propertyType.IsCollection(out var elementTypeSymbol))
                    {
                        var elementFullTypeName = elementTypeSymbol!.GetFullName();
                        if (elementTypeSymbol!.IsString())
                        {
                            elementFullTypeName = "BufferedString";
                        }
                        if (elementTypeSymbol!.IsBinary())
                        {
                            elementFullTypeName = "BufferedBinary";
                        }

                        if (elementTypeSymbol!.IsUnmanagedType)
                        {
                            context.WriteLines($"public ReadOnlyFixedLengthTypedList<{elementFullTypeName}> {propertyName} => Buffer.ReadUnmanagedCollectionField<{elementFullTypeName}>({index});");
                            continue;
                        }

                        if(elementTypeSymbol!.IsBufferedMessageSource(out bufferedMessageTypeName))
                        {
                            context.WriteLines($"public ReadOnlyVariableLengthTypeList<{bufferedMessageTypeName}> {propertyName} => Buffer.ReadBufferedObjectCollectionField<{bufferedMessageTypeName}>({index});");
                            continue;
                        }

                        context.WriteLines($"public ReadOnlyVariableLengthTypeList<{elementFullTypeName}> {propertyName} => Buffer.ReadBufferedObjectCollectionField<{elementFullTypeName}>({index});");
                        continue;
                    }
                    #endregion

                    #region Scalar
                    if (propertyType.IsPrimitive())
                    { 
                        context.WriteLines($"public {propertyType.GetFullName()} {propertyName} => Buffer.ReadUnmanagedField<{propertyType.GetFullName()}>({index});");
                        continue;
                    }

                    if (propertyType.IsUnmanagedType)
                    { 
                        context.WriteLines($"public ref readonly {propertyType.GetFullName()} {propertyName} => ref Buffer.ReadUnmanagedFieldAsRef<{propertyType.GetFullName()}>({index});");
                        continue;
                    }

                    if(propertyType.IsString())
                    {
                        context.WriteLines($"public BufferedString {propertyName} => Buffer.ReadBufferedObjectField<BufferedString>({index});");
                        continue;
                    }

                    if(propertyType.IsBinary())
                    {
                        context.WriteLines($"public BufferedBinary {propertyName} => Buffer.ReadBufferedObjectField<BufferedBinary>({index});");
                        continue;
                    }

                    if(propertyType.IsBufferedMessageSource(out bufferedMessageTypeName))
                    {
                        context.WriteLines($"public {bufferedMessageTypeName} {propertyName} => Buffer.ReadBufferedObjectField<{bufferedMessageTypeName}>({index});");
                        continue;
                    }
                    #endregion
                }
            }
        }
    }
}
