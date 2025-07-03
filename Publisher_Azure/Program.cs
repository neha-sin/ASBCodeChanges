using Azure.Messaging.ServiceBus;
using System;
using System.Threading.Tasks;

class Program
{
    // Replace with your Service Bus connection string
    const string connectionString = "Endpoint=sb://bedegaming.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=6mhTpVNuZrUWZWFqHzoa4r/oeGGY6zA/s+ASbDKiZQk=";

    // Replace with your queue name
    const string topicName = "mytopic";

    static async Task Main(string[] args)
    {
        // Create a ServiceBusClient
        await using var client = new ServiceBusClient(connectionString);

        // Create a sender for the queue
        ServiceBusSender sender = client.CreateSender(topicName);

        try
        {
            // Create a message to send
            ServiceBusMessage message = new ServiceBusMessage("Hello, Azure Service Bus!");

            // Send the message
            await sender.SendMessageAsync(message);
            Console.WriteLine("Message sent successfully.");
        }
        finally
        {
            // Dispose of the sender
            await sender.DisposeAsync();
        }
    }
}
