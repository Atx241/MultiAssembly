using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace GlobiAssembly
{
    internal static class Network
    {
        public static string host = "localhost";
        public static int tcpPort = 33333;
        public static int udpPort = 33334;
        public static TcpClient tcp = new TcpClient();
        public static UdpClient udp = new UdpClient();
        private static bool Initialized = false;

        public static byte[] Bytes(params object[] objs)
        {
            return BytesFromArray(objs);
        }
        public static byte[] BytesFromArray(object[] objs)
        {
            var ret = new List<byte>();

            foreach (var obj in objs)
            {
                byte[] next = { };
                switch (obj) {
                    case byte b:
                        ret.Add(b);
                        break;
                    case short s:
                        next = BitConverter.GetBytes(s);
                        break;
                    case int i:
                        next = BitConverter.GetBytes(i);
                        break;
                    case long l:
                        next = BitConverter.GetBytes(l);
                        break;
                    case ushort us:
                        next = BitConverter.GetBytes(us);
                        break;
                    case uint ui:
                        next = BitConverter.GetBytes(ui);
                        break;
                    case ulong ul:
                        next = BitConverter.GetBytes(ul);
                        break;
                    case float f:
                        next = BitConverter.GetBytes(f);
                        break;
                    case double d:
                        next = BitConverter.GetBytes(d);
                        break;
                    case char c:
                        next = BitConverter.GetBytes(c);
                        break;
                    case string str:
                        next = Encoding.UTF8.GetBytes(str);
                        break;
                    default:
                        throw new Exception("Bytes() encountered an invalid type (" + obj.GetType().ToString() + ")");
                }
                if (!BitConverter.IsLittleEndian)
                {
                    Array.Reverse(next);
                }
                ret.AddRange(next);
            }

            return ret.ToArray();
        }
        public static void SendTCPMessage(string fcfi, params object[] objs)
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(Bytes(UUID.LocalKP.Private, fcfi));
            bytes.AddRange(BytesFromArray(objs));
            tcp.GetStream().Write(bytes.ToArray(), 0, bytes.Count);
        }

        public static void Initialize()
        {
            tcp = new TcpClient { NoDelay = true };
            udp = new UdpClient();
            tcp.Connect(host, tcpPort);
            udp.Connect(host, udpPort);

            SendTCPMessage("REG_", UUID.LocalKP.Public, Plugin.Username);

            Initialized = true;
        }
        public static bool IsInitialized()
        {
            return Initialized;
        }

        public static void Disconnect()
        {
            SendTCPMessage("UREG", UUID.LocalKP.Public);

            tcp.Close();
            udp.Close();

            Initialized = false;
        }
    }
}
