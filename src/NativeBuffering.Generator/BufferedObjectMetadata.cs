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
            var fullName = typeSymbol.GetFullName();
            if (fullName == typeSymbol.Name)
            {
                BufferedMessageClassFullName = bufferedMessageClassName;
            }
            else
            {
                BufferedMessageClassFullName = $"{fullName.Substring(0, fullName.Length - typeSymbol.Name.Length)}{bufferedMessageClassName}";
            }
        }

        public ITypeSymbol TypeSymbol { get; }
        public string BufferedMessageClassName { get; }
        public BufferedObjectProperty[] Properties { get; }
        public string BufferedMessageClassFullName { get; }
    }
}
