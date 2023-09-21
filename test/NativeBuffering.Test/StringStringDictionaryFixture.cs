using NativeBuffering.Dictionaries;

namespace NativeBuffering.Test
{
    public partial class StringStringDictionaryFixture
    {
        [Fact]
        public void GetValues()
        {
            var random = new Random();
            var numbers = Enumerable.Range(0, random.Next(10, 30)).Select(_ => random.Next(10, 5000)).Distinct().ToArray();

            var source = new Source(numbers.ToDictionary(it => it.ToString(), it => it.ToString()));
            using var pooledMessage = source.AsBufferedMessage<SourceBufferedMessage>();
            var message = pooledMessage.BufferedMessage;

            Assert.Equal(numbers.Length, message.Value.Count);
            foreach (var number in numbers)
            {
                Assert.Equal(number.ToString(), message.Value[number.ToString()]);
            }

            var keys = message.Value.Keys;
            Assert.Equal(numbers.Length, keys.Count());
            foreach (var number in numbers)
            {
                Assert.Contains(number.ToString(), keys);
            }

            var values = message.Value.Values.Select(it=>(string)it);
            Assert.Equal(numbers.Length, keys.Count());
            foreach (var number in numbers)
            {
                Assert.Contains(number.ToString(), values);
            }

            var keySet = new HashSet<string>(message.Value.Keys);
            var valueSet = new HashSet<string>(message.Value.Values.Select(it => (string)it));
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
            public Source(Dictionary<string, string> value)
            {
                Value = value;
            }

            public Dictionary<string, string> Value { get; }
        }
    }
}
