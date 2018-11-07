using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Communicator.Common;

namespace Communicator.Client
{
    /// <summary>
    ///     Handles creation of communication channel to server and parsing input from user.
    /// </summary>
    public class Client
    {
        private readonly ConnectionToServer server;
        private readonly ClientMessageOutput messageOutput;
        private readonly string username;

        public Client(string username, IPAddress serverAddress, int serverPort,
                      string encryption, bool logging)
        {
            messageOutput = new ClientMessageOutput(username, logging);
            var tcpClient = new TcpClient();
            tcpClient.Connect(serverAddress, serverPort);
            server = new ConnectionToServer(tcpClient,
                                            messageOutput,
                                            encryption);
            this.username = username;
        }

        public Client(string configPath = @".\settings.json")
        {
            var settings = new SettingsJsonReader(configPath);
            username = settings.GetValue("username");
            messageOutput = new ClientMessageOutput(username,
                                                    Convert.ToBoolean(settings.GetValue("logging")));
            var tcpClient = new TcpClient();
            tcpClient.Connect(settings.GetValue("address"),
                              Convert.ToInt32(settings.GetValue("port")));
            server = new ConnectionToServer(tcpClient,
                                            messageOutput,
                                            settings.GetValue("encryption"));
        }

        public void Start()
        {
            messageOutput.Log("Connecting to server...");
            server.Start();
            messageOutput.Log("Connected.");
        }

        public void SendMessage(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            if (text.Equals("!none"))
            {
                server.ChangeEncryption("none");
            } 
            else if (text.Equals("!xor"))
            {
                server.ChangeEncryption("xor");
            }
            else if (text.Equals("!caesar"))
            {
                server.ChangeEncryption("caesar");
            }
            else
            {
                Dictionary<string, string> message = new Dictionary<string, string>
                {
                    {"from", username},
                    {"msg", text}
                };
                server.Send(message);
            }            
        }
    }
}