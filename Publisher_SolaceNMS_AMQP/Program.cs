using Apache.NMS;
using Apache.NMS.AMQP;
using System;

class Program
{
    static void Main(string[] args)
    {
        string solaceHost = "amqps://demo.messaging.solace.cloud:5671";  // Or amqp:// for non-TLS
        string username = "bede";
        string password = "bede";
        string topicName = "my/sample/topic";

        IConnectionFactory factory = new NmsConnectionFactory(solaceHost);

        try
        {
            using (IConnection connection = factory.CreateConnection(username, password))
            {
                connection.Start();
                using (ISession session = connection.CreateSession(AcknowledgementMode.AutoAcknowledge))
                {
                    IDestination destination = session.GetTopic(topicName);

                    using (IMessageProducer producer = session.CreateProducer(destination))
                    {
                        ITextMessage message = session.CreateTextMessage("Hello Solace over AMQP!");
                        producer.Send(message);
                        Console.WriteLine($"Message sent to topic: {topicName}");
                    }
                }
                connection.Close();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending message: {ex.Message}");
        }
    }
}