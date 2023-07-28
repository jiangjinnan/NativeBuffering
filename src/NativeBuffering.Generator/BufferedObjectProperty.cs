using Microsoft.CodeAnalysis;

namespace NativeBuffering.Generator
{
    public class BufferedObjectProperty
    {
        public BufferedObjectProperty(IPropertySymbol propertySymbol, BufferedMessageMemberKind kind)
        {
            PropertySymbol = propertySymbol;
            Kind = kind;
        }

        public IPropertySymbol PropertySymbol { get; }
        public BufferedMessageMemberKind Kind { get; }

        public ITypeSymbol? ElementType { get; set; }
        public ITypeSymbol? KeyType { get; set; }
        public ITypeSymbol? ValueType { get; set; }

        public string? BufferedMessageTypeName { get; set; }
    }
}
