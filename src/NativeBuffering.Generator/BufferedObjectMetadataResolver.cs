using Microsoft.CodeAnalysis;

namespace NativeBuffering.Generator
{
    internal static class BufferedObjectMetadataResolver
    {
        public static BufferedObjectMetadata Resolve(ITypeSymbol typeSymbol)
        {
            var attribute = typeSymbol.GetAttributes().FirstOrDefault(it => it.AttributeClass?.Name == "BufferedMessageSourceAttribute");
            if (attribute is null)
            {
                throw new BufferedObjectMetadataException($"{typeSymbol.Name} is not annotated with BufferedMessageSourceAttribute.");
            }
            var bufferedMessageClassName = attribute.NamedArguments.SingleOrDefault(it => it.Key == "BufferedMessageClassName").Value.Value as string;

            HashSet<string> excludedPropertyNames = new();
            if (attribute!.ConstructorArguments.Any())
            {
                excludedPropertyNames = new HashSet<string>(attribute.ConstructorArguments[0].Values.Select(it => it.Value!.ToString()));
            }

            var properties = new List<BufferedObjectProperty>();
            foreach (var property in typeSymbol.GetMembers().OfType<IPropertySymbol>())
            {
                if (property.IsStatic)
                {
                    continue;
                }

                if(excludedPropertyNames.Contains(property.Name))
                {
                    continue;
                }

                BufferedObjectProperty bufferedObjectProperty;
                var propertyType = property.Type;

                #region Scalar

                if (propertyType.IsBufferedMessageSource(out var bufferedMessageTypeName))
                {
                    bufferedObjectProperty = new BufferedObjectProperty(property, BufferedMessageMemberKind.Message);
                    properties.Add(bufferedObjectProperty);
                    continue;
                }

                if (propertyType.IsPrimitive())
                {
                    bufferedObjectProperty = new BufferedObjectProperty(property, BufferedMessageMemberKind.Primitive);
                    properties.Add(bufferedObjectProperty);
                    continue;
                }

                if (propertyType.IsUnmanagedType)
                {
                    bufferedObjectProperty = new BufferedObjectProperty(property, BufferedMessageMemberKind.Unmanaged);
                    properties.Add(bufferedObjectProperty);
                    continue;
                }               

                if (propertyType.IsString())
                {
                    bufferedObjectProperty = new BufferedObjectProperty(property, BufferedMessageMemberKind.String);
                    properties.Add(bufferedObjectProperty);
                    continue;
                }

                if (propertyType.IsBinary())
                {
                    bufferedObjectProperty = new BufferedObjectProperty(property, BufferedMessageMemberKind.Binary);
                    properties.Add(bufferedObjectProperty);
                    continue;
                }
                #endregion

                #region Dictionary
                if (propertyType.IsDictionary(out var keyTypeSymbol, out var valueTypeSymbol))
                {
                    #region Unmanaged Dictionary
                    if (keyTypeSymbol!.IsUnmanagedType)
                    {
                        if (valueTypeSymbol!.IsBufferedMessageSource(out bufferedMessageTypeName))
                        {
                            bufferedObjectProperty = new BufferedObjectProperty(property, BufferedMessageMemberKind.UnmanagedMessageDictionary);
                            bufferedObjectProperty.KeyType = keyTypeSymbol;
                            bufferedObjectProperty.ValueType = valueTypeSymbol;
                            bufferedObjectProperty.BufferedMessageTypeName = bufferedMessageTypeName;
                            properties.Add(bufferedObjectProperty);
                            continue;
                        }

                        if (valueTypeSymbol!.IsUnmanagedType)
                        {
                            bufferedObjectProperty = new BufferedObjectProperty(property, BufferedMessageMemberKind.UnmanagedUnmanagedDictionary);
                            bufferedObjectProperty.KeyType = keyTypeSymbol;
                            bufferedObjectProperty.ValueType = valueTypeSymbol;
                            properties.Add(bufferedObjectProperty);
                            continue;
                        }

                        if (valueTypeSymbol.IsString())
                        {
                            bufferedObjectProperty = new BufferedObjectProperty(property, BufferedMessageMemberKind.UnmanagedStringDictionary);
                            bufferedObjectProperty.KeyType = keyTypeSymbol;
                            bufferedObjectProperty.ValueType = valueTypeSymbol;
                            properties.Add(bufferedObjectProperty);
                            continue;
                        }                     

                        if (valueTypeSymbol.IsBinary())
                        {
                            bufferedObjectProperty = new BufferedObjectProperty(property, BufferedMessageMemberKind.UnmanagedBinaryDictionary);
                            bufferedObjectProperty.KeyType = keyTypeSymbol;
                            bufferedObjectProperty.ValueType = valueTypeSymbol;
                            properties.Add(bufferedObjectProperty);
                            continue;
                        }

                         throw new BufferedObjectMetadataException($"Unsupported dictionary value type {valueTypeSymbol.Name} for property {property.Name}.");
                    }
                    #endregion

                    #region String Dictionary
                    if (keyTypeSymbol.IsString())
                    {
                        if (valueTypeSymbol!.IsBufferedMessageSource(out bufferedMessageTypeName))
                        {
                            bufferedObjectProperty = new BufferedObjectProperty(property, BufferedMessageMemberKind.StringMessageDictionary);
                            bufferedObjectProperty.KeyType = keyTypeSymbol;
                            bufferedObjectProperty.ValueType = valueTypeSymbol;
                            bufferedObjectProperty.BufferedMessageTypeName = bufferedMessageTypeName;
                            properties.Add(bufferedObjectProperty);
                            continue;
                        }

                        if (valueTypeSymbol!.IsUnmanagedType)
                        {
                            bufferedObjectProperty = new BufferedObjectProperty(property, BufferedMessageMemberKind.StringUnmanagedDictionary);
                            bufferedObjectProperty.KeyType = keyTypeSymbol;
                            bufferedObjectProperty.ValueType = valueTypeSymbol;
                            properties.Add(bufferedObjectProperty);
                            continue;
                        }

                        if (valueTypeSymbol.IsString())
                        {
                            bufferedObjectProperty = new BufferedObjectProperty(property, BufferedMessageMemberKind.StringStringDictionary);
                            bufferedObjectProperty.KeyType = keyTypeSymbol;
                            bufferedObjectProperty.ValueType = valueTypeSymbol;
                            properties.Add(bufferedObjectProperty);
                            continue;
                        }                        

                        if (valueTypeSymbol.IsBinary())
                        {
                            bufferedObjectProperty = new BufferedObjectProperty(property, BufferedMessageMemberKind.StringBinaryDictionary);
                            bufferedObjectProperty.KeyType = keyTypeSymbol;
                            bufferedObjectProperty.ValueType = valueTypeSymbol;
                            properties.Add(bufferedObjectProperty);
                            continue;
                        }

                        throw new BufferedObjectMetadataException($"Unsupported dictionary value type {valueTypeSymbol.Name} for property {property.Name}.");
                    }
                    #endregion

                    throw new BufferedObjectMetadataException($"Unsupported dictionary key type {keyTypeSymbol.Name} for property {property.Name}.");
                }
                #endregion

                #region Collection
                if(propertyType.IsCollection(out var elementTypeSymbol))
                {
                    if (elementTypeSymbol.IsBufferedMessageSource(out bufferedMessageTypeName))
                    {
                        bufferedObjectProperty = new BufferedObjectProperty(property, BufferedMessageMemberKind.MessageCollection);
                        bufferedObjectProperty.ElementType = elementTypeSymbol;
                        bufferedObjectProperty.BufferedMessageTypeName = bufferedMessageTypeName;
                        properties.Add(bufferedObjectProperty);
                        continue;
                    }

                    if (elementTypeSymbol!.IsUnmanagedType)
                    {
                        bufferedObjectProperty = new BufferedObjectProperty(property, BufferedMessageMemberKind.UnmanagedCollection);
                        bufferedObjectProperty.ElementType = elementTypeSymbol;
                        properties.Add(bufferedObjectProperty);
                        continue;
                    }

                    if (elementTypeSymbol.IsString())
                    {
                        bufferedObjectProperty = new BufferedObjectProperty(property, BufferedMessageMemberKind.StringCollection);
                        bufferedObjectProperty.ElementType = elementTypeSymbol;
                        properties.Add(bufferedObjectProperty);
                        continue;
                    }                  

                    if (elementTypeSymbol.IsBinary())
                    {
                        bufferedObjectProperty = new BufferedObjectProperty(property, BufferedMessageMemberKind.BinaryCollection);
                        bufferedObjectProperty.ElementType = elementTypeSymbol;
                        properties.Add(bufferedObjectProperty);
                        continue;
                    }

                    throw new BufferedObjectMetadataException($"Unsupported collection element type {elementTypeSymbol.Name} for property {property.Name}.");
                }
                #endregion          

                throw new BufferedObjectMetadataException($"Unsupported property type {propertyType.Name} for property {property.Name}.");
            }
            return new BufferedObjectMetadata(typeSymbol, bufferedMessageClassName ?? $"{typeSymbol.Name}BufferedMessage", properties.ToArray());
        }
    }
}
