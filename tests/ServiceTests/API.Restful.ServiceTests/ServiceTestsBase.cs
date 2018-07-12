using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

using Microsoft.Extensions.Configuration;

namespace API.Restful.ServiceTests
{
    public class ServiceTestsBase
    {
        public HttpClient HttpClient;

        public static IConfiguration Configuration { get; set; }
        public ServiceTestsBase()
        {
            var builder = new ConfigurationBuilder()
                          .SetBasePath(Directory.GetCurrentDirectory()) 
                          .AddJsonFile("appsettings.json");

            Configuration = builder.Build();

            HttpClient = new HttpClient
            {
                BaseAddress = new Uri(Configuration.GetConnectionString("HttpConnectionStringRestful")),
            };

            HttpClient.DefaultRequestHeaders.Accept.Clear();
            HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
        }
    }
}
