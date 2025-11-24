using MultiAssembly;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace GlobiAssembly
{
    internal static class Network
    {
        public const int NetworkHertz = 60;

        public static string host = "localhost";
        public static int tcpPort = 33333;
        public static int udpPort = 33334;

        private static TcpClient tcp = new TcpClient();
        private static UdpClient udp = new UdpClient();

        private static bool Initialized = false;
        private static Thread? loopThread;
        private static bool shutdownLoopThread = false;

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
        public static void SendTCP(string fcfi, params object[] objs)
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(Bytes(UUID.LocalKP.Private, fcfi));
            bytes.AddRange(BytesFromArray(objs));
            tcp.GetStream().Write(bytes.ToArray(), 0, bytes.Count);
        }

        public static void SendUDP(string fcfi, params object[] objs)
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(Bytes(UUID.LocalKP.Private, fcfi));
            bytes.AddRange(BytesFromArray(objs));
            udp.Send(bytes.ToArray(), bytes.Count);
        }

        public static void Initialize()
        {
            tcp = new TcpClient { NoDelay = true };
            udp = new UdpClient();
            tcp.Connect(host, tcpPort);
            udp.Connect(host, udpPort);

            SendTCP("REG_", UUID.LocalKP.Public, Plugin.Username);

            GameObjects.Player = GameObject.FindFirstObjectByType<PlaneContainer>();

            foreach (Transform t in GameObjects.Player.transform)
            {
                Console.WriteLine("Player part child: " + t.gameObject.name);
            }

            if (loopThread != null)
            {
                loopThread.Join();
            }

            loopThread = new Thread(new ThreadStart(loop));
            loopThread.Start();

            Initialized = true;
        }
        public static bool IsInitialized()
        {
            return Initialized;
        }

        public static void Disconnect()
        {
            SendTCP("UREG", UUID.LocalKP.Public);

            tcp.Close();
            udp.Close();

            if (loopThread != null)
            {
                shutdownLoopThread = true;
                loopThread.Join();
                shutdownLoopThread = false;
            }

            loopThread = null;

            Initialized = false;
        }

        private static void loop()
        {
            while (true)
            {
                if (shutdownLoopThread)
                {
                    return;
                }
                if (GameObjects.Player != null)
                {
                    Plugin.PlayerLoop();
                }
                Thread.Sleep(1000 / NetworkHertz);
            }
        }
    }
}
