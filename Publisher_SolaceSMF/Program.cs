using System;
using System.Text;
using SolaceSystems.Solclient.Messaging;
using System.Threading;

public class Program {

    static void Main(string[] args) {

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
        var contextFactoryProps = new ContextFactoryProperties() {
            SolClientLogLevel = SolLogLevel.Warning  // Set log level.
        };
        contextFactoryProps.LogToConsoleError();  // Log errors to console.
        ContextFactory.Instance.Init(contextFactoryProps);  // Must init the API before using any of its artifacts.

        // instantiate a context factory, create a session, then connect
        var contextProps = new ContextProperties();
        var context = ContextFactory.Instance.CreateContext(contextProps, null);
        var session = context.CreateSession(sessionProps, null, OnSessionEvent);
        session.Connect();

        var message = ContextFactory.Instance.CreateMessage();
        message.Destination     = ContextFactory.Instance.CreateTopic("tutorial/topic");
        //message.DeliveryMode    = settings.DeliveryMode;
    
        message.BinaryAttachment = Encoding.ASCII.GetBytes("Sample Message");            
        session.Send(message);

        Console.WriteLine("Message Sent");
    }

    private static void OnSessionEvent(object sender, SessionEventArgs e) {
        if (e.Event == SessionEvent.UpNotice) {
            Console.WriteLine($"Connected to {e.Info.Split(',')[0]}");
        }

        // TODO: add support for other event types?
    }

}