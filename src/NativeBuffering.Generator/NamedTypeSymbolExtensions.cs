using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;


namespace NativeBuffering.Generator
{
    internal static class NamedTypeSymbolExtensions
    {
        private static readonly HashSet<SpecialType> _primitiveTypes = new()
        {
            SpecialType.System_SByte,
            SpecialType.System_Byte,
            SpecialType.System_Int16,
            SpecialType.System_Int32,
            SpecialType.System_Int64,
            SpecialType.System_UInt16,
            SpecialType.System_UInt32,
            SpecialType.System_UInt64,
            SpecialType.System_IntPtr,
            SpecialType.System_UIntPtr,
            SpecialType.System_Single,
            SpecialType.System_Double,
            SpecialType.System_Char,
            SpecialType.System_Decimal,
            SpecialType.System_Boolean
        };
        public static bool IsPrimitive(this ITypeSymbol typeSymbol)=> _primitiveTypes.Contains(typeSymbol.SpecialType);
        public static bool IsString(this ITypeSymbol typeSymbol) => typeSymbol.SpecialType == SpecialType.System_String;
        public static bool IsBufferedMessageSource(this ITypeSymbol typeSymbol, out string? bufferedMessageTypeName)
        {          
            var attribute = typeSymbol.GetAttributes().FirstOrDefault(it => it.AttributeClass?.Name == "BufferedMessageSourceAttribute");
            if (attribute is null)
            {
                bufferedMessageTypeName = null;
                return false;
            }
            var className = attribute.NamedArguments.SingleOrDefault(it => it.Key == "ClassName").Value.Value as string;
            bufferedMessageTypeName = string.IsNullOrWhiteSpace(className) ? $"{typeSymbol.Name}BufferedMessage" : className!;
            return true;
        }
        public static string GetFullName(this ITypeSymbol typeSymbol)
        {
            var nullable = typeSymbol.IsNullable(out var underlying);
            if (nullable)
            {
                typeSymbol = underlying!;
            }
            var parts = new List<string>();
            Collect(typeSymbol);
            var builder = new StringBuilder();
            for (int index = parts.Count - 1; index >= 0; index--)
            {
                builder.Append(parts[index]);
                if (index != 0)
                {
                    builder.Append(".");
                }
            }
            var name = builder.ToString();
            return nullable ? $"{name}?" : name;

            void Collect(INamespaceOrTypeSymbol symbol)
            {
                if (!string.IsNullOrEmpty(symbol.Name))
                {
                    parts.Add(symbol.Name);
                }
                else
                {
                    return;
                }


                var type = symbol.ContainingType;
                if (type is not null)
                {
                    Collect(type);
                }
                else
                {
                    var ns = symbol.ContainingNamespace;
                    if (ns is not null)
                    {
                        Collect(ns);
                    }
                }
            }
        }
        public static bool IsNullable(this ITypeSymbol typeSymbol, out ITypeSymbol? underlyingTypeSymbol)
        {
            if (typeSymbol is INamedTypeSymbol namedTypeSymbol && typeSymbol.Name == "Nullable")
            {
                underlyingTypeSymbol = namedTypeSymbol.TypeArguments[0];
                return true;
            }
            underlyingTypeSymbol = null;
            return false;
        }
        public static bool IsDictionary(this ITypeSymbol typeSymbol,  out ITypeSymbol? keyTypeSymbol, out ITypeSymbol? valueTypeSymbol)
        {
            if (typeSymbol is not INamedTypeSymbol namedTypeSymbol)
            {
                keyTypeSymbol = default;
                valueTypeSymbol = default;
                return false;
            }
            if ((typeSymbol.MetadataName == "IDictionary`2" && typeSymbol.ContainingNamespace?.ToString() == "System.Collections.Generic"))
            {
                if (namedTypeSymbol.TypeArguments[0] is INamedTypeSymbol key && namedTypeSymbol.TypeArguments[1] is INamedTypeSymbol value)
                {
                    keyTypeSymbol = key;
                    valueTypeSymbol = value;
                    return true;
                }
            }

            var dictionaryType = typeSymbol.AllInterfaces.SingleOrDefault(it => it.MetadataName == "IDictionary`2" && it.ContainingNamespace.ToString() == "System.Collections.Generic");
            if (dictionaryType != null)
            {
                keyTypeSymbol = namedTypeSymbol.TypeArguments[0];
                valueTypeSymbol = namedTypeSymbol.TypeArguments[1];
                return true;
            }
            keyTypeSymbol = default;
            valueTypeSymbol = default;
            return false;
        }
        public static bool IsCollection(this ITypeSymbol typeSymbol,  out ITypeSymbol? elementTypeSymbol)
        {
            if(typeSymbol.IsString() || typeSymbol.IsBinary() || typeSymbol.IsDictionary(out _, out _))
            {
                elementTypeSymbol = null;
                return false;
            }

            if (typeSymbol is IArrayTypeSymbol array)
            {
                elementTypeSymbol = array.ElementType;
                return true;
            }

            var enumerableType = typeSymbol.AllInterfaces.SingleOrDefault(it => it.MetadataName == "IEnumerable`1" && it.ContainingNamespace.ToString() == "System.Collections.Generic");
            if (enumerableType is not null && enumerableType.TypeArguments.Single() is INamedTypeSymbol element2)
            {
                elementTypeSymbol = element2;
                return true;
            }

            if (typeSymbol is not INamedTypeSymbol namedTypeSymbol)
            {
                elementTypeSymbol = null;
                return false;
            }
            if (namedTypeSymbol.MetadataName == "IEnumerable`1" && typeSymbol.ContainingNamespace?.ToString() == "System.Collections.Generic" && namedTypeSymbol.TypeArguments.Single() is INamedTypeSymbol element1)
            {
                elementTypeSymbol = element1;
                return true;
            }

           

            elementTypeSymbol = null;
            return false;
        }
        public static bool IsBinary(this ITypeSymbol typeSymbol)
        {
            if (typeSymbol is IArrayTypeSymbol array && array.ElementType.SpecialType == SpecialType.System_Byte)
            {
                return true;
            }

            if (typeSymbol is not INamedTypeSymbol namedTypeSymbol)
            {
                return false;
            }         

            if (namedTypeSymbol.MetadataName == "Memory`1" && namedTypeSymbol.TypeArguments.Count() == 0 && namedTypeSymbol.TypeArguments.Single().SpecialType == SpecialType.System_Byte)
            {
                return true;
            }

            return false;
        }
        public static IEnumerable<ISymbol> GetContainingNamespaceAndTypes(this ISymbol symbol)
        {
            if (symbol is INamespaceOrTypeSymbol self)
            {
                yield return self;
            }
            while (true)
            {
                symbol = symbol.ContainingSymbol;
                if (symbol is null)
                {
                    yield break;
                }
                yield return symbol;
            }
        }
    }
}
