namespace NativeBuffering
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class BufferedMessageSourceAttribute : Attribute
    {
        public string? BufferedMessageClassName { get; set; }
        public string[] ExcludedProperties { get; }
        public BufferedMessageSourceAttribute(params string[] excludedProperties)
        {
            ExcludedProperties = excludedProperties;
        }
    }
}
