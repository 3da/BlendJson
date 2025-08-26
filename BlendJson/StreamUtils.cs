using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace BlendJson
{
    public static class StreamUtils
    {
        public static async Task<byte[][]> LoadLargeBytesFromStreamAsync(Stream stream, CancellationToken token = default)
        {
            var result = new List<byte[]>();

            while (true)
            {
                var buffer = new byte[0X7FFFFFC7];
                int sizeLeft = 0X7FFFFFC7;
                int offset = 0;

                int count;

                while (true)
                {
                    count = await stream.ReadAsync(buffer, offset, sizeLeft, token);
                    sizeLeft -= count;
                    offset += count;
                    if (sizeLeft == 0 || count == 0)
                    {
                        if (sizeLeft > 0)
                            Array.Resize(ref buffer, buffer.Length - sizeLeft);

                        result.Add(buffer);
                        break;
                    }
                }
                if (count == 0)
                    break;


            }

            return result.ToArray();
        }

        public static async Task WriteLargeBytesToStreamAsync(Stream stream, byte[][] bytes, CancellationToken token = default)
        {
            foreach (var b in bytes)
            {
                await stream.WriteAsync(b, token);
            }
        }
    }
}
