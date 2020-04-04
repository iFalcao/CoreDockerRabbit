using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

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
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var result = await _httpClient.PostAsync("http://publisher_api:80/api/values", content);
            string resultContent = await result.Content.ReadAsStringAsync();
            Console.WriteLine("Server returned: " + resultContent);
        }

        static void Main(string[] args)
        {
            string[] testStrings = new string[] { "one", "two", "three", "four", "five" };

            Console.WriteLine("Sleeping to wait for Rabbit");
            Task.Delay(10000).Wait();
            Console.WriteLine("Posting messages to webApi");
            for (int i = 0; i < 5; i++)
                PostMessage(testStrings[i]).Wait();

            Task.Delay(1000).Wait();
            Console.WriteLine("Consuming Queue Now");

            var factory = new ConnectionFactory() { HostName = "rabbitmq", Port = 5672 };
            factory.UserName = "guest";
            factory.Password = "guest";
            var conn = factory.CreateConnection();
            var channel = conn.CreateModel();
            channel.QueueDeclare(queue: "hello",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(" [x] Received from Rabbit: {0}", message);
            };
            channel.BasicConsume(queue: "hello",
                                 autoAck: true,
                                 consumer: consumer);
        }
    }
}
