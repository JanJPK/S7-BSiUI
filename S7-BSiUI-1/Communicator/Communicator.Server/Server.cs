using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Communicator.Common;
using Communicator.Common.Security;

namespace Communicator.Server
{
    /// <summary>
    ///     Handles creation of communication channel to multiple clients.
    /// </summary>
    public class Server
    {
        private readonly List<ConnectionToClient> clients = new List<ConnectionToClient>();
        private readonly TcpListener listener;
        private readonly ServerMessageOutput messageOutput;
        private readonly int bufferSize;

        public Server(int port, bool logging = false)
        {
            listener = new TcpListener(IPAddress.Any, port);
            messageOutput = new ServerMessageOutput(this, logging);            
        }

        public Server(string configPath = @".\settings.json")
        {
            var settings = new SettingsJsonReader(configPath);
            bufferSize = Convert.ToInt32(settings.GetValue("buffersize"));
            listener = new TcpListener(IPAddress.Any, Convert.ToInt32(settings.GetValue("port")));
            messageOutput = new ServerMessageOutput(this, Convert.ToBoolean(settings.GetValue("logging")));
        }

        public void Start()
        {
            listener.Start();
            messageOutput.Log("Server started.");
            while (true)
            {
                var client = new ConnectionToClient(listener.AcceptTcpClient(),
                                                    messageOutput,
                                                    bufferSize,
                                                    new DiffieHellmanJsonReader(@".\primes.json"));
                messageOutput.Log("New client is connecting...");
                client.Start();
                clients.Add(client);
                messageOutput.Log("Client connected.");
            }
        }

        public void BroadcastMessage(Dictionary<string, string> message)
        {
            foreach (var client in clients)
            {
                client.Send(new Dictionary<string, string>(message));
            }
            clients.RemoveAll(c => c.IsConnectionOpen == false);            
        }
    }
}