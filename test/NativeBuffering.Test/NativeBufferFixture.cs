using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NativeBuffering.Test
{
    public class NativeBufferFixture
    {
        [Fact]
        public void ToByteArray()
        { 
            var bytes = new byte[byte.MaxValue + 1];
            for (int i = byte.MinValue; i <= byte.MaxValue; i++)
            {
                bytes[i] = (byte)i;
            }
            var buffer = new NativeBuffer(bytes,10);
            var array = buffer.ToByteArray();
            Assert.Equal(246, array.Length);
            for (int index = 0; index < array.Length; index++)
            {
                Assert.Equal(index + 10, array[index]);
            }
        }
    }
}
