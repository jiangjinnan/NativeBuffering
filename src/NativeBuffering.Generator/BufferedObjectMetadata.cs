using Microsoft.CodeAnalysis;

namespace NativeBuffering.Generator
{
    public class BufferedObjectMetadata
    {
        public BufferedObjectMetadata(ITypeSymbol typeSymbol, string bufferedMessageClassName, BufferedObjectProperty[] properties)
        {
            TypeSymbol = typeSymbol;
            BufferedMessageClassName = bufferedMessageClassName;
            Properties = properties;
        }

        public ITypeSymbol TypeSymbol { get; }
        public string BufferedMessageClassName { get; }
        public BufferedObjectProperty[] Properties { get; }
    }
}
