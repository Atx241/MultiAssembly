using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
internal static class Plugin
{
    public static TcpClient tcp = new TcpClient();
    public static UdpClient udp = new UdpClient();
    public static void Main()
    {
        Console.WriteLine("Mock Player");
        Console.WriteLine("This software is packaged with the MultiAssembly software chain, and is therefore governed under the same license as the software chain itself");
        Console.WriteLine("---------");
        Console.WriteLine("Enter a remote endpoint:");

        string ep;
        while (true)
        {
            ep = Console.ReadLine();
            tcp = new TcpClient();
            var connected = tcp.ConnectAsync(ep, 33333).Wait(5000);
            if (!connected)
            {
                Console.WriteLine("TCP connection timed out, please try again or enter a different endpoint");
                continue;
            }
            break;
        }

        Console.WriteLine("Enter a username:");
        string username = Console.ReadLine();

        udp.Connect(ep, 33334);
        Console.WriteLine("Successfully connected to the remote endpoint");

        var vehicle = File.ReadAllBytes("vehicle.mpv");

        Network.SendTCP("REG_", UUID.LocalKP.Public, (byte)username.Length, username, vehicle.ToArray());

        var rot = 0.0;
        var circleSize = 10;

        while (true)
        {
            Network.SendUDP("PTUP", Math.Sin(rot) * circleSize, 10.0, Math.Cos(rot) * circleSize);
            rot += 0.1;
            Thread.Sleep(16);
        }
    }
}