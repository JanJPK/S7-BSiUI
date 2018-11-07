using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Numerics;
using Communicator.Common;
using Communicator.Common.Security;

namespace Communicator.Client
{
    /// <summary>
    ///     Enables communication with server, includes client-specific security initialization
    ///     and encryption change request.
    /// </summary>
    public class ConnectionToServer : ConnectionBase
    {
        private string encryption;

        public ConnectionToServer(TcpClient tcpClient,
                                  ICanHandleMessageOutput messageOutput,
                                  string encryption)
            : base(tcpClient, messageOutput)
        {
            this.encryption = encryption;
        }

        /// <summary>
        ///     Changes encryption used in communication with server.
        /// </summary>
        /// <param name="newEncryption">New encryption name.</param>
        public void ChangeEncryption(string newEncryption)
        {
            encryption = newEncryption;
            Send(new Dictionary<string, string>
            {
                {"encryption", encryption}
            });
            MessageOutput.Log("Client requested encryption change:\n" +
                              $"\tencryption: {encryption}");
            SetEncryption(encryption);
        }

        protected override void InitializeSecurity()
        {
            Dictionary<string, string> message;
            var dhg = new DiffieHellmanGenerator();

            // Stage 1: Send key request                
            Send(new Dictionary<string, string>
            {
                {"request", "keys"}
            });

            // Stage 2: Process response
            message = WaitForKey("p");
            if (message == null)
            {
                MessageOutput.Log("Invalid response from server; expected p");
                return;
            }

            //int p = Convert.ToInt32(message["p"]);
            //int g = Convert.ToInt32(message["g"]);
            //var publicKeys = new DiffieHellmanPublicKeystore(p, g);
            var publicKeys = new DiffieHellmanPublicKeystore(message);
            var secretA = dhg.GenerateSecret();
            var transportA = dhg.GenerateTransport(secretA, publicKeys);

            // Stage 3: Send a, await b
            Send(new Dictionary<string, string>
            {
                {"a", transportA.ToString()}
            });
            message = WaitForKey("b");
            if (message == null)
            {
                MessageOutput.Log("Invalid response from server; expected b");
                return;
            }

            // Stage 4: Calculate shared secret
            var transportB = BigInteger.Parse(message["b"]);
            SharedSecret = dhg.GenerateSharedSecret(secretA, transportB, publicKeys);

            // Stage 5: Send encryption type
            Send(new Dictionary<string, string>
            {
                {"encryption", encryption}
            });
            SetEncryption(encryption);

            MessageOutput.Log("Connection summary:\n " +
                              $"\tp: {publicKeys.P}\n" +
                              $"\tg: {publicKeys.G}\n" +
                              $"\tsecret: {secretA}\n" +
                              $"\ttransport: {transportA}\n" +
                              $"\treceived transport: {transportB}\n" +
                              $"\tshared secret: {SharedSecret}\n" +
                              $"\tencryption: {encryption}");
        }

        protected override void HandleMessage(Dictionary<string, string> message)
        {
            MessageOutput.HandleMessage(message);
        }
    }
}