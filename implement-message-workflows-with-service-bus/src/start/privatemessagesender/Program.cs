using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;

namespace privatemessagesender
{
    class Program
    {
        public static IConfigurationRoot Configuration { get; set; }
        const string QueueName = "salesmessages";
        static IQueueClient queueClient;

        static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            // tell the builder to look for the appsettings.json file
            builder
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddUserSecrets<Program>();

            Configuration = builder.Build();

            Console.WriteLine("Sending a message to the Sales Messages queue...");

            await SendSalesMessageAsync();

            Console.WriteLine("Message was sent successfully.");
        }

        static async Task SendSalesMessageAsync()
        {

            queueClient = new QueueClient(Configuration["ConnectionString"], QueueName);

            // Send messages.
            try
            {
                string messageBody = $"Local $10,000 order for bicycle parts from retailer Adventure Works.";
                var message = new Message(Encoding.UTF8.GetBytes(messageBody));
                Console.WriteLine($"Sending message: {messageBody}");
                await queueClient.SendAsync(message);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{DateTime.Now} :: Exception: {exception.Message}");
            }

            // Close the connection to the queue here
            await queueClient.CloseAsync();
        }
    }
}
