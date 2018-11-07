using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Communicator.Common.Security.Encoder;
using Newtonsoft.Json;

namespace Communicator.Common
{
    /// <summary>
    ///     Enables connection and message sending/receiving, does not include security logic.
    /// </summary>
    public abstract class ConnectionBase
    {
        protected readonly TcpClient TcpClient;
        protected readonly NetworkStream Stream;
        protected readonly ICanHandleMessageOutput MessageOutput;
        protected BigInteger SharedSecret;
        protected IEncoder Encoder;
        protected int BufferSize;
        public bool IsConnectionOpen => TcpClient.Connected;

        protected ConnectionBase(TcpClient tcpClient,
                                 ICanHandleMessageOutput messageOutput,
                                 int bufferSize = 4096)
        {
            TcpClient = tcpClient;
            Stream = tcpClient.GetStream();
            MessageOutput = messageOutput;
            BufferSize = bufferSize;
        }

        /// <summary>
        ///     Starts the connection - first initializes security (Diffie-Hellman key exchange),
        ///     then handles all the incoming messages in a loop.
        /// </summary>
        public void Start()
        {
            var task = new Task(() =>
            {
                InitializeSecurity();
                StartListening();
            }, TaskCreationOptions.LongRunning);
            task.Start();
        }

        /// <summary>
        ///     Encodes (encrypts as well, if necessary) and sends the message in JSON format.
        /// </summary>
        /// <param name="message"></param>
        public void Send(Dictionary<string, string> message)
        {
            if (TcpClient.Connected)
            {
                var bytes = Encoding.UTF8.GetBytes(Encode(message));
                if (bytes.Length > BufferSize)
                {
                    MessageOutput.Log("Message is too large:\n" +
                                      $"\tSize: {bytes.Length}\n" +
                                      $"\tMax size: {BufferSize}");
                    return;
                }
                Stream.Write(bytes, 0, bytes.Length);
            }
            else
            {
                MessageOutput.Log("TcpClient is not connected.");
            }
        }

        protected abstract void InitializeSecurity();

        protected abstract void HandleMessage(Dictionary<string, string> message);

        protected void SetEncryption(string encryptionName)
        {
            switch (encryptionName.ToLower())
            {
                case "caesar":
                {
                    Encoder = new CaesarEncoder(SharedSecret);
                    break;
                }
                case "xor":
                {
                    Encoder = new XorEncoder(SharedSecret);
                    break;
                }
                default:
                {
                    Encoder = new NoneEncoder();
                    break;
                }
            }
        }

        protected Dictionary<string, string> WaitForJson()
        {
            byte[] bytes = new byte[BufferSize];
            int i;
            try
            {
                while ((i = Stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    var json = Encoding.UTF8.GetString(bytes, 0, i);
                    var message = Decode(json);
                    return message;
                }
            }
            catch (Exception e)
            {
                MessageOutput.Log(e);
                return null;
            }

            return null;
        }

        // Waits for json input and returns it only if it contains the required key.
        protected Dictionary<string, string> WaitForKey(string key)
        {
            var message = WaitForJson();
            if (message.ContainsKey(key))
            {
                return message;
            }
            else
            {
                MessageOutput.Log("Unexpected keys", message);
                return null;
            }
        }

        protected string Encode(Dictionary<string, string> message)
        {
            if (Encoder != null
             && message.ContainsKey("msg")
             && message.ContainsKey("from"))
            {
                message["msg"] = Encoder.Encode(message["msg"]);
                MessageOutput.Log($"Encoded message: {message["msg"]}");
            }

            var json = JsonConvert.SerializeObject(message);
            MessageOutput.Log($"Sending json: {json}");
            return json;
        }

        protected Dictionary<string, string> Decode(string json)
        {
            MessageOutput.Log($"Received json: {json}");
            var message = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            if (Encoder != null
             && message.ContainsKey("msg")
             && message.ContainsKey("from"))
            {
                message["msg"] = Encoder.Decode(message["msg"]);
                MessageOutput.Log($"Decoded message: {message["msg"]}");
            }

            return message;
        }

        private void StartListening()
        {
            while (true)
            {
                var message = WaitForJson();
                if (message == null)
                {
                    TcpClient.Close();
                    return;
                }
                else
                {
                    HandleMessage(message);
                }
            }
        }
    }
}