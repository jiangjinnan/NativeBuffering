namespace NativeBuffering.Generator
{

    [Serializable]
	public class BufferedObjectMetadataException : Exception
	{
		public BufferedObjectMetadataException() { }
		public BufferedObjectMetadataException(string message) : base(message) { }
		public BufferedObjectMetadataException(string message, Exception inner) : base(message, inner) { }
		protected BufferedObjectMetadataException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
