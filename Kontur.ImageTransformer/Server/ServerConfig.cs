namespace Kontur.ImageTransformer.Server
{
    internal class ServerConfig
    {
        public ServerConfig(uint maxThreads = 20, uint countIo = 25, uint timeout = 900)
        {
            MaxThreads = (int) maxThreads;
            CountIO = (int) countIo;
            Timeout = (int) timeout;
        }

        public int MaxThreads { get; }
        public int CountIO { get; }
        public int Timeout { get; }
    }
}