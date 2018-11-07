using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using Communicator.Common;
using Communicator.Common.Security;

namespace Communicator.Server
{
    /// <summary>
    ///     Enables communication with client, includes server-specific security initialization.    
    /// </summary>
    public class ConnectionToClient : ConnectionBase
    {
        private readonly DiffieHellmanJsonReader primesReader;

        public ConnectionToClient(TcpClient tcpClient,
                                  ICanHandleMessageOutput messageOutput,
                                  int bufferSize,
                                  DiffieHellmanJsonReader primesReader)
            : base(tcpClient, messageOutput, bufferSize)
        {
            this.primesReader = primesReader;
        }

        protected override void InitializeSecurity()
        {
            Dictionary<string, string> message;
            var dhg = new DiffieHellmanGenerator();

            // Stage 1: Await key request
            message = WaitForKey("request");
            if (message == null)
            {
                MessageOutput.Log("Invalid request from client; expected keys");
                return;
            }

            // Stage 2: Send keys
            var publicKeys = primesReader.GetRandomKeystore();
            var secretB = dhg.GenerateSecret();
            var transportB = dhg.GenerateTransport(secretB, publicKeys);
            Send(publicKeys.GetJson());

            // Stage 3: Send b, await a
            message = WaitForKey("a");
            Send(new Dictionary<string, string>
            {
                {"b", transportB.ToString()}
            });
            if (message == null)
            {
                MessageOutput.Log("Invalid response from client; expected a");
                return;
            }

            // Stage 4: Calculate shared secret
            var transportA = BigInteger.Parse(message["a"]);
            SharedSecret = dhg.GenerateSharedSecret(secretB, transportA, publicKeys);

            // Stage 5: Await encryption
            message = WaitForJson();
            if (message == null)
            {
                return;
            }

            if (message.ContainsKey("encryption"))
            {
                SetEncryption(message["encryption"]);
                MessageOutput.Log("Connection summary:\n" +
                                  $"\tp: {publicKeys.P}\n" +
                                  $"\tg: {publicKeys.G}\n" +
                                  $"\tsecret: {secretB}\n" +
                                  $"\ttransport: {transportB}\n" +
                                  $"\treceived transport: {transportA}\n" +
                                  $"\tshared secret: {SharedSecret}\n" +
                                  $"\tencryption: {message["encryption"]}");
            }
            else
            {
                // Client skipped encryption step; assume none and handle the message.
                SetEncryption("none");
                MessageOutput.Log("Connection summary:\n" +
                                  $"\tp: {publicKeys.P}\n" +
                                  $"\tg: {publicKeys.G}\n" +
                                  $"\tsecret: {secretB}\n" +
                                  $"\ttransport: {transportB}\n" +
                                  $"\treceived transport: {transportA}\n" +
                                  $"\tshared secret: {SharedSecret}\n" +
                                  $"\tencryption: none");
                message["msg"] = Encoding.UTF8.GetString(Convert.FromBase64String(message["msg"]));
                MessageOutput.HandleMessage(message);
            }
        }

        protected override void HandleMessage(Dictionary<string, string> message)
        {
            if (message.ContainsKey("encryption"))
            {
                MessageOutput.Log("Client requested encryption change:\n" +
                                  $"\tencryption: {message["encryption"]}");
                SetEncryption(message["encryption"]);
            }
            else
            {
                MessageOutput.HandleMessage(message);
            }
        }
    }
};