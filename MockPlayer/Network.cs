using System.Text;
internal static class Network
{
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
            switch (obj)
            {
                case byte b:
                    ret.Add(b);
                    break;
                case byte[] ba:
                    ret.AddRange(ba);
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
        var tmp = new List<byte>(Bytes((ushort)bytes.Count));
        tmp.AddRange(bytes);
        bytes = tmp;
        Plugin.tcp.GetStream().Write(bytes.ToArray(), 0, bytes.Count);
    }

    public static void SendUDP(string fcfi, params object[] objs)
    {
        List<byte> bytes = new List<byte>();
        bytes.AddRange(Bytes(UUID.LocalKP.Private, fcfi));
        bytes.AddRange(BytesFromArray(objs));
        Plugin.udp.Send(bytes.ToArray(), bytes.Count);
    }
}