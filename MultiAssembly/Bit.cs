using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace MultiAssembly
{
    internal static class Bit
    {
        public static double ReadDouble(MemoryStream stream)
        {
            byte[] buf = new byte[8];
            int n = stream.Read(buf, 0, 8);
            if (n < 8)
            {
                throw new InvalidOperationException("Not enough data in memory stream to read double (read " + n + " want 8)");
            }
            return BitConverter.ToDouble(buf, 0);
        }

        //NOTE: Passing -1 to size reads the remainder of the stream into a string
        public static string ReadString(MemoryStream stream, int size)
        {
            int _size = size;
            if (size == -1) {
                _size = (int)(stream.Length - stream.Position);
            }
            byte[] buf = new byte[_size];
            int n = stream.Read(buf, 0, _size);
            if (size != -1 && n < size)
            {
                throw new InvalidOperationException("Not enough data in memory stream to read string (read " + n + " want " + size + ")");
            }
            return Encoding.UTF8.GetString(buf);
        }
    }
}
