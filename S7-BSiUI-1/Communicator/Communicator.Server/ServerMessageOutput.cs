using System;
using System.Collections.Generic;
using Communicator.Common;

namespace Communicator.Server
{
    /// <summary>
    ///     Writes messages to console and broadcasts them to other clients.
    /// </summary>
    public class ServerMessageOutput : MessageOutputBase
    {
        private readonly Server server;

        public ServerMessageOutput(Server server, bool logging) : base(logging)
        {
            this.server = server;
        }

        public override void HandleMessage(Dictionary<string, string> message)
        {
            if (message.ContainsKey("from") && message.ContainsKey("msg"))
            {
                var from = message["from"];
                var msg = message["msg"];
                Console.WriteLine($"{from}: {msg}");
                server.BroadcastMessage(message);
            }
        }
    }
}