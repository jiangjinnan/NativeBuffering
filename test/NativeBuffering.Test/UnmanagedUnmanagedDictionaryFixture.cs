using NativeBuffering.Dictionaries;

namespace NativeBuffering.Test
{
    public partial class UnmanagedUnmanagedDictionaryFixture
    {
        [Fact]
        public void GetValues()
        { 
            var random = new Random();
            var numbers = Enumerable.Range(0, random.Next(10,30)).Select(_=>random.Next(10,5000)).Distinct().ToArray();

            var source = new Source(numbers.ToDictionary(it=>(long)it, it=>(long)it));
            using var pooledMessage = source.AsBufferedMessage<SourceBufferedMessage>();
            var message = pooledMessage.BufferedMessage;

            Assert.Equal(numbers.Length, message.Value.Count);
            foreach(var number in numbers)
            {
                Assert.Equal(number, message.Value[number]);
            }

            var keys = message.Value.Keys;
            Assert.Equal(numbers.Length, keys.Count());
            foreach (var number in numbers)
            {
                Assert.Contains(number, keys);
            }

            var values = message.Value.Values;
            Assert.Equal(numbers.Length, keys.Count());
            foreach (var number in numbers)
            {
                Assert.Contains(number, values);
            }

            var keySet = new HashSet<long>( message.Value.Keys);
            var valueSet = new HashSet<long>( message.Value.Values);
            foreach (var kv in message.Value)
            {
                keySet.Remove(kv.Key);
                valueSet.Remove(kv.Value);
            }
            Assert.Empty(keySet);
            Assert.Empty(valueSet);
        }

        [BufferedMessageSource]
        public partial class Source 
        {
            public Source(Dictionary<long, long> value)
            {
                Value = value;
            }

            public Dictionary<long, long> Value { get; }
        }
    }
}
