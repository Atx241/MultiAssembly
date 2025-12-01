
internal static class UUID
{
    public const int UUIDLength = 16;
    public const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_";
    public static string Generate()
    {
        string ret = "";
        for (int i = 0; i < UUIDLength; i++)
        {
            ret += validChars[Random.Shared.Next() % validChars.Length];
        }
        return ret;
    }

    public struct KeyPair
    {
        public string Public;
        public string Private;

        public KeyPair(string Public, string Private)
        {
            this.Public = Public;
            this.Private = Private;
        }
        public static KeyPair Generate()
        {
            return new KeyPair(UUID.Generate(), UUID.Generate());
        }
    }

    public static KeyPair LocalKP = KeyPair.Generate();
}
