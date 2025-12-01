using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace MultiAssembly
{
    internal static class Bit
    {
        public static byte[] TCPReadExactly(NetworkStream stream, int length)
        {
            var prevNetTimeout = stream.ReadTimeout;
            stream.ReadTimeout = Network.NetworkTimeout;
            byte[] ret = new byte[length];
            int n = 0;

            while (n < length)
            {
                var read = stream.Read(ret, n, length - n);
                if (read == 0)
                {
                    throw new SocketException();
                }
                n += read;
            }

            stream.ReadTimeout = prevNetTimeout;

            return ret;
        }
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

        public static float ReadFloat(MemoryStream stream)
        {
            byte[] buf = new byte[4];
            int n = stream.Read(buf, 0, 4);
            if (n < 4)
            {
                throw new InvalidOperationException("Not enough data in memory stream to read double (read " + n + " want 4)");
            }
            return BitConverter.ToSingle(buf, 0);
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
        public static ushort ReadUShort(MemoryStream stream)
        {
            byte[] buf = new byte[2];
            int n = stream.Read(buf, 0, 2);

            if (!BitConverter.IsLittleEndian)
            {
                byte[] tmpbuf = buf;
                buf[1] = tmpbuf[0];
                buf[0] = tmpbuf[1];
            }

            if (n < 2)
            {
                throw new InvalidOperationException("Not enough data in memory stream to read double (read " + n + " want 2)");
            }
            return BitConverter.ToUInt16(buf);
        }
    }
}
