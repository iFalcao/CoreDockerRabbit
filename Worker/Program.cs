using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Worker
{
    class Program
    {
        private static HttpClient _httpClient = new HttpClient(new HttpClientHandler()
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
            {
                return true;
            }
        });

        public static async Task PostMessage(string postData)
        {
            var json = JsonConvert.SerializeObject(postData);
            var content = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var result = await _httpClient.PostAsync("http://publisher_api:80/api/values", content);
            string resultContent = await result.Content.ReadAsStringAsync();
            Console.WriteLine("Server returned: " + resultContent);
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Posting a message!");
            PostMessage("Hello World!").Wait();
        }
    }
}
