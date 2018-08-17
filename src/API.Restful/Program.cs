using Infrastructure.SQL.EntityFramework;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace API.Restful
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = BuildWebHost(args);
            host.Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)      
                .UseStartup<Startup>()
                .Build();
    }
}
