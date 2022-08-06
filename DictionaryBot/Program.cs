namespace DictionaryBot
{
    public static class Program
    {
        public static bool IsDebugConfiguration()
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }

        public static void Main(string[] args) => new Core().RunAsync().GetAwaiter().GetResult();
    }
}