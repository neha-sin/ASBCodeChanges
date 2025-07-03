
using Apache.NMS;
using Apache.NMS.AMQP;
using System;

class SolaceQueueConsumer
{
    static void Main(string[] args)
    {
        string solaceHost = "amqps://demo.messaging.solace.cloud:5671";  // For TLS; use amqp:// for non-TLS
        string username = "bede";
        string password = "bede";
        string queueName = "amqp_queue";  // Solace Queue name (must exist on broker)

        IConnectionFactory factory = new NmsConnectionFactory(solaceHost);

        try
        {
            using (IConnection connection = factory.CreateConnection(username, password))
            {
                connection.Start();
                using (ISession session = connection.CreateSession(AcknowledgementMode.AutoAcknowledge))
                {
                    IDestination destination = session.GetQueue(queueName);

                    using (IMessageConsumer consumer = session.CreateConsumer(destination))
                    {
                        Console.WriteLine($"Waiting for messages on queue: {queueName}...");

                        while (true)
                        {
                            IMessage receivedMessage = consumer.Receive(TimeSpan.FromSeconds(10));

                            if (receivedMessage is ITextMessage textMessage)
                            {
                                Console.WriteLine($"Received TextMessage: {textMessage.Text}");
                            }
                            else if (receivedMessage is IBytesMessage bytesMessage)
                            {
                                byte[] messageBytes = new byte[bytesMessage.BodyLength];
                                bytesMessage.ReadBytes(messageBytes);

                                string messageText = System.Text.Encoding.UTF8.GetString(messageBytes);
                                Console.WriteLine($"Received BytesMessage as text: {messageText}");
                            }
                            else
                            {
                                Console.WriteLine($"Received message of type: {receivedMessage.GetType()}");
                            }

                        }
                    }
                }
                connection.Close();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error receiving message: {ex.Message}");
        }
    }
}