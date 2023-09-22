using Microsoft.CodeAnalysis;
using System.IO.Pipes;

namespace NativeBuffering.Generator
{
    public class BufferedObjectProperty
    {
        public BufferedObjectProperty(IPropertySymbol propertySymbol, BufferedMessageMemberKind kind)
        {
            PropertySymbol = propertySymbol;
            Kind = kind;
            IsNullable = propertySymbol.Type.IsNullable(out var underlyingTypeSymbol);
            NullableUnderlyingType = underlyingTypeSymbol;
        }

        public IPropertySymbol PropertySymbol { get; }
        public BufferedMessageMemberKind Kind { get; }

        public ITypeSymbol? ElementType { get; set; }
        public ITypeSymbol? KeyType { get; set; }
        public ITypeSymbol? ValueType { get; set; }

        public string? BufferedMessageTypeName { get; set; }
        public bool IsNullable { get; }
        public ITypeSymbol? NullableUnderlyingType { get;  }
    }
}
