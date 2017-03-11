using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;

namespace ABC.Leaves.Api
{
    public class Program
    {
        public static string[] CommandLineArgs { get; private set;}

        public static void Main(string[] args)
        {
            CommandLineArgs = args;

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();
            host.Run();
        }
    }
}
