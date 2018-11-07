using System;
using System.Linq;

namespace Communicator.Server
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("No arguments supplied - using settings.json instead.");
                new Server().Start();
            }

            if (int.TryParse(args[0], out int port))
            {
                var server = args.Contains("logging")
                                    ? new Server(port, true) 
                                    : new Server(port);
                Console.WriteLine($"Starting server on port {port}.");
                server.Start();                  
            }
            else
            {
                Console.WriteLine("First argument (port) must be a valid number.");
                return;
            }

        }
    }
}