using System;
using Kontur.ImageTransformer.Server;

namespace Kontur.ImageTransformer
{
    public class EntryPoint
    {
        public static void Main(string[] args)
        {
            using (var server = new AsyncHttpServer())
            {
                server.StartServer("http://localhost:8080/");

                Console.ReadKey();
            }
        }
    }
}
