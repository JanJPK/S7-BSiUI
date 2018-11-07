using System;
using System.Collections.Generic;
using Communicator.Common;

namespace Communicator.Client
{
    /// <summary>
    ///     Writes messages to console, ignores messages sent by current client.
    /// </summary>
    public class ClientMessageOutput : MessageOutputBase
    {
        private readonly string username;

        public ClientMessageOutput(string username, bool logging) : base(logging)
        {
            this.username = username;
        }

        public override void HandleMessage(Dictionary<string, string> message)
        {
            var from = message["from"];
            if (!from.Equals(username))
            {
                var msg = message["msg"];
                Console.WriteLine($"{from}: {msg}");
            }
            else
            {
                Log("Ignoring message; from is same as username.");
            }
        }
    }
}