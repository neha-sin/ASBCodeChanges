
using Azure.Messaging.ServiceBus;
using System;
using System.Threading.Tasks;

class Program
{
    static string connectionString = "Endpoint=sb://bedegaming.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=6mhTpVNuZrUWZWFqHzoa4r/oeGGY6zA/s+ASbDKiZQk=";
    static string topicName = "mytopic";
    static string subscriptionName = "Sub1";

    static async Task Main(string[] args)
    {
        // Create a client
        await using var client = new ServiceBusClient(connectionString);

        // Create a receiver for the topic subscription
        ServiceBusReceiver receiver = client.CreateReceiver(topicName, subscriptionName);

        Console.WriteLine($"Listening to '{topicName}/{subscriptionName}'... Press Ctrl+C to stop.");

        while (true)
        {
            // Try to receive a message (with timeout)
            ServiceBusReceivedMessage message = await receiver.ReceiveMessageAsync(TimeSpan.FromSeconds(5));

            if (message != null)
            {
                string body = message.Body.ToString();
                Console.WriteLine($"Received: {body}");

                // Complete the message (i.e., remove from queue)
                await receiver.CompleteMessageAsync(message);
            }

            await Task.Delay(1000);
        }
    }
}