using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public static class HttpClientExtensions
    {
        public static async Task<HttpResponseMessage> PatchAsync(this HttpClient client, string requestUri, HttpContent iContent)
        {
            var method = new HttpMethod("PATCH");
            var request = new HttpRequestMessage(method, requestUri)
            {
                Content = iContent
            };

            var response = new HttpResponseMessage();
            try
            {
                response = await client.SendAsync(request);
            }
            catch (TaskCanceledException e)
            {
                Debug.WriteLine("ERROR: " + e.ToString());
            }

            return response;
        }

        public static async Task<HttpResponseMessage> OptionsAsync(this HttpClient client, string requestUri, HttpContent iContent)
        {
            var method = new HttpMethod("OPTIONS");
            var request = new HttpRequestMessage(method, requestUri);

            var response = new HttpResponseMessage();
            try
            {
                response = await client.SendAsync(request);
            }
            catch (TaskCanceledException e)
            {
                Debug.WriteLine("ERROR: " + e.ToString());
            }

            return response;
        }


        public static async Task<HttpResponseMessage> HeadAsync(this HttpClient client, string requestUri, HttpContent iContent)
        {
            var method = new HttpMethod("HEAD");
            var request = new HttpRequestMessage(method, requestUri);

            var response = new HttpResponseMessage();
            try
            {
                response = await client.SendAsync(request);
            }
            catch (TaskCanceledException e)
            {
                Debug.WriteLine("ERROR: " + e.ToString());
            }

            return response;
        }
    }
}
