using System;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using MultiAssembly.Handlers;
using System.IO;
using System.Net;

namespace MultiAssembly
{
    internal static class Network
    {
        public const int NetworkHertz = 60;

        public static string host = "zserver";
        public static int tcpPort = 33333;
        public static int udpPort = 33334;

        private static TcpClient tcp = new TcpClient();
        private static UdpClient udp = new UdpClient();

        private static bool Initialized = false;
        private static Thread? loopThread;
        private static Thread? tcpThread;
        private static Thread? udpThread;
        private static bool shutdownThreads = false;

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

            if (loopThread != null) loopThread.Join();
            if (tcpThread != null) tcpThread.Join();
            if (udpThread != null) udpThread.Join();

            loopThread = new Thread(new ThreadStart(loop));
            loopThread.Start();

            tcpThread = new Thread(new ThreadStart(tcpLoop));
            tcpThread.Start();

            udpThread = new Thread(new ThreadStart(udpLoop));
            udpThread.Start();

            Initialized = true;
        }
        public static bool IsInitialized()
        {
            return Initialized;
        }

        public static void Disconnect()
        {
            SendTCP("UREG", UUID.LocalKP.Public);

            shutdownThreads = true;
            if (loopThread != null) loopThread.Join();
            if (tcpThread != null) tcpThread.Join();
            if (udpThread != null) udpThread.Join();
            shutdownThreads = false;

            loopThread = null;
            tcpThread = null;
            udpThread = null;

            tcp.Close();
            udp.Close();

            while (Player.Players.Count > 0)
            {
                Player.Players[0].Destroy();
            }

            Initialized = false;
        }

        private static void loop()
        {
            while (true)
            {
                if (shutdownThreads) return;
                if (GameObjects.Player != null)
                {
                    Plugin.PlayerLoop();
                }
                Thread.Sleep(1000 / NetworkHertz);
            }
        }
        private static void tcpLoop()
        {
            byte[] buf = new byte[1024];
            while (true)
            {
                if (shutdownThreads) return;
                try
                {
                    if (tcp.Available <= 0) goto End;

                    int n = tcp.GetStream().Read(buf, 0, buf.Length);

                    //Console.WriteLine("TCP Message: " + BitConverter.ToString(new ArraySegment<byte>(buf, 0, n).ToArray()) + "(length " + n + ")");

                    MemoryStream stream = new MemoryStream((byte[])buf.Clone(), 0, n, false, true);

                    while (stream.Position < stream.Length)
                    {
                        ushort packetLength = Bit.ReadUShort(stream);

                        MemoryStream tmpStream = new MemoryStream(stream.GetBuffer(), (int)stream.Position, packetLength, false);

                        stream.Position += packetLength;

                        TCP.Run(Utility.ReadFCFI(tmpStream), tmpStream);
                    }
                } catch (Exception e)
                {
                    Console.WriteLine("TCP Exception occured: " + e.ToString());
                }
            End:
                Thread.Sleep(1000 / NetworkHertz);
            }
        }
        private static void udpLoop()
        {
            while (true)
            {
                if (shutdownThreads) return;
                try
                {
                    while (udp.Available > 0)
                    {

                        IPEndPoint ep = new IPEndPoint(IPAddress.Any, 0);

                        byte[] buf = udp.Receive(ref ep);

                        MemoryStream stream = new MemoryStream((byte[])buf.Clone());

                        UDP.Run(Utility.ReadFCFI(stream), stream);
                    }
                } catch (Exception e)
                {
                    Console.WriteLine("UDP Exception occured: " + e.ToString());
                }
                Thread.Sleep(1000 / NetworkHertz);
            }
        }
    }
}
