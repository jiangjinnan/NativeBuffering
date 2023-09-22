using NativeBuffering.Dictionaries;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NativeBuffering.Test
{
    public partial class UnmanagedStringDictionaryFixture
    {
        [Fact]
        public void GetValues()
        {
            var random = new Random();
            var numbers = Enumerable.Range(0, random.Next(10, 30)).Select(_ => random.Next(10, 5000)).Distinct().ToArray();

            var source = new Source(numbers.ToDictionary(it => (long)it, it => (string?)it.ToString()));
            source.Value.Add(1, null);
            source.Value.Add(2, "");
            using var pooledMessage = source.AsBufferedMessage<SourceBufferedMessage>();
            var message = pooledMessage.BufferedMessage;

            Assert.Equal(numbers.Length + 2, message.Value.Count);
            foreach (var number in numbers)
            {
                Assert.Equal(number.ToString(), message.Value[number]);
            }
            Assert.Equal(string.Empty, message.Value[1]);
            Assert.Equal(string.Empty, message.Value[2]);

            var keys = message.Value.Keys;
            Assert.Equal(numbers.Length + 2, keys.Count());
            foreach (var number in numbers)
            {
                Assert.Contains(number, keys);
            }

            var values = message.Value.Values.Select(it => (string)it);
            Assert.Equal(numbers.Length + 2, keys.Count());
            foreach (var number in numbers)
            {
                Assert.Contains(number.ToString(), values);
            }

            var keySet = new HashSet<long>(message.Value.Keys);
            var valueSet = new HashSet<string>(message.Value.Values.Select(it => (string)it));
            foreach (var kv in message.Value)
            {
                keySet.Remove(kv.Key);
                valueSet.Remove(kv.Value);
            }

            keySet.Remove(1);
            keySet.Remove(2);
            valueSet.Remove("");
            Assert.Empty(keySet);
            Assert.Empty(valueSet);
        }

        [BufferedMessageSource]
        public partial class Source 
        {
            public Source(Dictionary<long, string?> value)
            {
                Value = value;
            }

            public Dictionary<long, string?> Value { get; }
        }
    }
}
