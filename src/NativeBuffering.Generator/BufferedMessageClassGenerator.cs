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
                context.WriteLines($"public static {metadata.BufferedMessageClassName} DefaultValue => throw new NotImplementedException();");

                context.WriteLines("public NativeBuffer Buffer { get; }");
                context.WriteLines($"public {metadata.BufferedMessageClassName}(NativeBuffer buffer) => Buffer = buffer;");
                context.WriteLines($"public static {metadata.BufferedMessageClassName} Parse(NativeBuffer buffer) => new {metadata.BufferedMessageClassName}(buffer);");

                var properties = metadata.Properties;
                for (var index = 0; index < properties.Length; index++)
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
                        //if (valueTypeSymbol!.IsBufferedMessageSource(out bufferedMessageTypeName))
                        //{
                        //    valueFullTypeName = bufferedMessageTypeName;
                        //}

                        // Key = Unmannaged
                        if (keyTypeSymbol!.IsUnmanagedType)
                        {
                            if(valueTypeSymbol!.IsBufferedMessageSource(out bufferedMessageTypeName))
                            {
                                if (valueTypeSymbol!.IsNullable(out _))
                                {
                                    context.WriteLines($"public ReadOnlyUnmanagedNullableBufferedObjectDictionary<{keyTypeSymbol.GetFullName()}, {bufferedMessageTypeName}> {propertyName} => Buffer.ReadUnmanagedNullableBufferedObjectDictionaryField<{keyTypeSymbol.GetFullName()}, {bufferedMessageTypeName}>({index});");
                                }
                                else
                                {
                                    context.WriteLines($"public ReadOnlyUnmanagedNonNullableBufferedObjectDictionary<{keyTypeSymbol.GetFullName()}, {bufferedMessageTypeName}> {propertyName} => Buffer.ReadUnmanagedNonNullableBufferedObjectDictionaryField<{keyTypeSymbol.GetFullName()}, {bufferedMessageTypeName}>({index});");
                                }
                                continue;
                            }

                            if (valueTypeSymbol!.IsString() || valueTypeSymbol!.IsBinary())
                            {
                                context.WriteLines($"public ReadOnlyUnmanagedNonNullableBufferedObjectDictionary<{keyFullTypeName}, {valueFullTypeName}> {propertyName} => Buffer.ReadUnmanagedNonNullableBufferedObjectDictionaryField<{keyFullTypeName}, {valueFullTypeName}>({index});");
                                continue;
                            }

                            if (valueTypeSymbol!.IsNullable(out _))
                            {
                                context.WriteLines($"public ReadOnlyUnmanagedNullableUnmanagedDictionary<{keyFullTypeName}, {valueFullTypeName}> {propertyName} => Buffer.ReadUnmanagedNullableUnmanagedDictionaryField<{keyFullTypeName}, {valueFullTypeName}>({index});");
                                continue;
                            }

                            context.WriteLines($"public ReadOnlyUnmanagedNonNullableUnmanagedDictionary<{keyFullTypeName}, {valueFullTypeName}> {propertyName} => Buffer.ReadUnmanagedNonNullableUnmanagedDictionaryField<{keyFullTypeName}, {valueFullTypeName}>({index});");

                        }
                        // Key = String
                        else
                        {
                            if (valueTypeSymbol!.IsBufferedMessageSource(out bufferedMessageTypeName))
                            {
                                if (valueTypeSymbol!.IsNullable(out _))
                                {
                                    context.WriteLines($"public ReadOnlyStringNullableBufferedObjectDictionary< {bufferedMessageTypeName}> {propertyName} => Buffer.ReadStringNullableBufferedObjectDictionaryField<{bufferedMessageTypeName}>({index});");
                                }
                                else
                                {
                                    context.WriteLines($"public ReadOnlyStringNonNullableBufferedObjectDictionary< {bufferedMessageTypeName}> {propertyName} => Buffer.ReadStringNonNullableBufferedObjectDictionaryField<{bufferedMessageTypeName}>({index});");
                                }
                                continue;
                            }

                            if (valueTypeSymbol!.IsString() || valueTypeSymbol!.IsBinary())
                            {
                                context.WriteLines($"public ReadOnlyStringNonNullableBufferedObjectDictionary< {valueFullTypeName}> {propertyName} => Buffer.ReadStringNonNullableBufferedObjectDictionaryField<{valueFullTypeName}>({index});");
                                continue;
                            }

                            if (valueTypeSymbol!.IsNullable(out _))
                            {
                                context.WriteLines($"public ReadOnlyStringNullableUnmanagedDictionary<{valueFullTypeName}> {propertyName} => Buffer.ReadStringNullableUnmanagedDictionaryField<{valueFullTypeName}>({index});");
                                continue;
                            }

                            context.WriteLines($"public ReadOnlyStringNonNullableUnmanagedDictionary<{valueFullTypeName}> {propertyName} => Buffer.ReadStringNonNullableUnmanagedDictionaryField<{valueFullTypeName}>({index});");
                        }
                        continue;
                    }
                    #endregion

                    #region Collection
                    if (propertyType.IsCollection(out var elementTypeSymbol))
                    {
                        if (elementTypeSymbol!.IsString())
                        {
                            context.WriteLines($"public ReadOnlyNonNullableBufferedObjectList<BufferedString> {propertyName} => Buffer.ReadNonNullableBufferedObjectCollectionField<BufferedString>({index});");
                            continue;
                        }
                        if (elementTypeSymbol!.IsBinary())
                        {
                            context.WriteLines($"public ReadOnlyNonNullableBufferedObjectList<BufferedBinary> {propertyName} => Buffer.ReadNonNullableBufferedObjectCollectionField<BufferedBinary>({index});");
                            continue;
                        }

                        var elementFullTypeName = elementTypeSymbol!.GetFullName();

                        if (elementTypeSymbol!.IsBufferedMessageSource(out bufferedMessageTypeName))
                        {
                            if (elementTypeSymbol!.IsNullable(out _) || !elementTypeSymbol.IsValueType)
                            {
                                context.WriteLines($"public ReadOnlyNullableBufferedObjectList<{bufferedMessageTypeName}> {propertyName} => Buffer.ReadNullableBufferedObjectCollectionField<{bufferedMessageTypeName}>({index});");
                            }
                            else
                            {
                                context.WriteLines($"public ReadOnlyNonNullableBufferedObjectList<{bufferedMessageTypeName}> {propertyName} => Buffer.ReadNonNullableBufferedObjectCollectionField<{bufferedMessageTypeName}>({index});");
                            }
                            continue;
                        }

                        if (elementTypeSymbol!.IsUnmanagedType)
                        {
                            if (elementTypeSymbol.IsNullable(out var underlyingTypeSymbol))
                            {
                                context.WriteLines($"public ReadOnlyNullableUnmanagedList<{underlyingTypeSymbol!.GetFullName()}> {propertyName} => Buffer.ReadNullableUnmanagedCollectionField<{underlyingTypeSymbol!.GetFullName()}>({index});");
                            }
                            else
                            {
                                context.WriteLines($"public ReadOnlyNonNullableUnmanagedList<{elementFullTypeName}> {propertyName} => Buffer.ReadNonNullableUnmanagedCollectionField<{elementFullTypeName}>({index});");
                            }
                            continue;
                        }

                        context.WriteLines($"public ReadOnlyNullableBufferedObjectList<{elementFullTypeName}> {propertyName} => Buffer.ReadNullableBufferedObjectCollectionField<{elementFullTypeName}>({index});");
                        continue;
                    }
                    #endregion

                    #region Scalar
                    if (propertyType.IsPrimitive())
                    {
                        if (property.IsNullable)
                        {
                            context.WriteLines($"public {property.NullableUnderlyingType!.GetFullName()}? {propertyName} => Buffer.ReadNullableUnmanagedField<{property.NullableUnderlyingType!.GetFullName()}>({index});");
                            continue;
                        }
                        else
                        {
                            context.WriteLines($"public {propertyType.GetFullName()} {propertyName} => Buffer.ReadUnmanagedField<{propertyType.GetFullName()}>({index});");
                            continue;
                        }
                    }

                    if (propertyType.IsUnmanagedType)
                    {
                        if (property.IsNullable)
                        {
                            context.WriteLines($"public {property.NullableUnderlyingType!.GetFullName()}? {propertyName} => Buffer.ReadNullableUnmanagedField<{property.NullableUnderlyingType!.GetFullName()}>({index});");
                            continue;
                        }
                        else
                        {
                            context.WriteLines($"public ref readonly {propertyType.GetFullName()} {propertyName} => ref Buffer.ReadUnmanagedFieldAsRef<{propertyType.GetFullName()}>({index});");
                            continue;
                        }
                    }

                    if (propertyType.IsString())
                    {
                        context.WriteLines($"public BufferedString {propertyName} => Buffer.ReadNonNullableBufferedObjectField<BufferedString>({index});");
                        continue;
                    }

                    if (propertyType.IsBinary())
                    {
                        context.WriteLines($"public BufferedBinary {propertyName} => Buffer.ReadNonNullableBufferedObjectField<BufferedBinary>({index});");
                        continue;
                    }

                    if (propertyType.IsBufferedMessageSource(out bufferedMessageTypeName))
                    {
                        if (property.IsNullable || !propertyType.IsValueType)
                        {
                            context.WriteLines($"public {bufferedMessageTypeName}? {propertyName} => Buffer.ReadNullableBufferedObjectField<{bufferedMessageTypeName}>({index});");
                        }
                        else
                        {
                            context.WriteLines($"public {bufferedMessageTypeName} {propertyName} => Buffer.ReadNonNullableBufferedObjectField<{bufferedMessageTypeName}>({index});");
                        }
                        continue;
                    }
                    #endregion
                }
            }


        }
    }
}
