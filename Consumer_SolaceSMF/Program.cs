using System;
using System.Text;
using SolaceSystems.Solclient.Messaging;
using System.Threading;

public class Program
{
    static IFlow Flow;
    static EventWaitHandle WaitEventWaitHandle = new AutoResetEvent(false);

    static void Main(string[] args)
    {
        var sessionProps = new SessionProperties
        {
            ClientName = "Demo CS Publisher",
            Host = "tcps://mr-connection-5ezl00b3y25.messaging.solace.cloud:55443",
            VPNName = "demo",
            UserName = "solace-cloud-client",
            Password = "mciv17ctbf32pkru3t5ukujh4d",
            SSLValidateCertificate = false
        };

        // initialize context for Solace -- the context is a singleton
        var contextFactoryProps = new ContextFactoryProperties()
        {
            SolClientLogLevel = SolLogLevel.Warning  // Set log level.
        };
        contextFactoryProps.LogToConsoleError();  // Log errors to console.
        ContextFactory.Instance.Init(contextFactoryProps);  // Must init the API before using any of its artifacts.

        // instantiate a context factory, create a session, then connect
        var contextProps = new ContextProperties();
        var context = ContextFactory.Instance.CreateContext(contextProps, null);
        ISession Session = context.CreateSession(sessionProps, null, null);
        Session.Connect();

        string queueName = "demoQ";

        // Create the queue
        IQueue Queue = ContextFactory.Instance.CreateQueue(queueName);

        Flow = Session.CreateFlow(new FlowProperties()
        {
            AckMode = MessageAckMode.ClientAck
        },
        Queue, null, HandleMessageEvent, HandleFlowEvent);

        Flow.Start();
        Console.WriteLine("Waiting for a message in the queue '{0}'...", queueName);

        WaitEventWaitHandle.WaitOne();

    }
    
     private static void HandleMessageEvent(object source, MessageEventArgs args)
    {
        // Received a message
        Console.WriteLine("Received message.");
        using (IMessage message = args.Message)
        {
            // Expecting the message content as a binary attachment
            Console.WriteLine("Message content: {0}", Encoding.ASCII.GetString(message.BinaryAttachment));
            // ACK the message
            Flow.Ack(message.ADMessageId);
            // finish the program
            //WaitEventWaitHandle.Set();
        }
    }

    public static void HandleFlowEvent(object sender, FlowEventArgs args)
    {
        // Received a flow event
        Console.WriteLine("Received Flow Event '{0}' Type: '{1}' Text: '{2}'",
            args.Event,
            args.ResponseCode.ToString(),
            args.Info);
    }

}