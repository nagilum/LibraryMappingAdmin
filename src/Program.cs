using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DotNetLibraryAdmin
{
    public class Program
    {
        /// <summary>
        /// Init all the things..
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            // Load config from disk.
            Config.Load();

            // Init the host.
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(l =>
                {
                    l.ClearProviders();
                    l.AddConsole();
                })
                .ConfigureWebHostDefaults(b =>
                {
                    b.UseStartup<Startup>();
                })
                .Build()
                .Run();
        }
    }
}