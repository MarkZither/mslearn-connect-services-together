using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;

namespace performancemessagesender
{
    class Program
    {
        public static IConfigurationRoot Configuration { get; set; }
        const string TopicName = "salesperformancemessages";
        static ITopicClient topicClient;

        static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            // tell the builder to look for the appsettings.json file
            builder
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddUserSecrets<Program>();

            Configuration = builder.Build();

            Console.WriteLine("Sending a message to the Sales Performance topic...");

            await SendPerformanceMessageAsync();

            Console.WriteLine("Message was sent successfully.");

        }

        static async Task SendPerformanceMessageAsync()
        {
            topicClient = new TopicClient(Configuration["ConnectionString"], TopicName);

            // Send messages.
            try
            {
                string messageBody = $"Local Total sales for Brazil in August: $13m.";
                var message = new Message(Encoding.UTF8.GetBytes(messageBody));
                Console.WriteLine($"Sending message: {messageBody}");
                await topicClient.SendAsync(message);
                await topicClient.CloseAsync();
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{DateTime.Now} :: Exception: {exception.Message}");
            }

            // Close the connection to the topic here
            await topicClient.CloseAsync();
        }
    }
}
