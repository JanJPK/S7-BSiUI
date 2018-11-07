using System;
using System.Linq;
using System.Net;

namespace Communicator.Client
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Client client;
            if (args.Length == 0)
            {
                Console.WriteLine("No arguments supplied - using settings.json instead.");
                client = new Client();
            }
            else if (args.Length < 2)
            {
                Console.WriteLine("Please supply correct arguments: {username} {address} {port}.\n" +
                                  "Example: user 127.0.0.1 8001");
                return;
            }
            else
            {
                string encryption = "none";
                bool logging = false;
                if (args.Length > 2)
                {
                    if (args.Contains("xor"))
                    {
                        encryption = "xor";
                    }
                    else if (args.Contains("caesar"))
                    {
                        encryption = "caesar";
                    }

                    if (args.Contains("logging"))
                    {
                        logging = true;
                    }
                }

                if (int.TryParse(args[2], out int port))
                {
                    if (IPAddress.TryParse(args[1], out IPAddress ip))
                    {
                        string name = args[0];
                        client = new Client(name, ip, port, encryption, logging);
                        Console.WriteLine($"Connecting to: {ip}:{port}...");
                    }
                    else
                    {
                        Console.WriteLine("Second argument (ip) must be a valid address.");
                        return;
                    }
                }
                else
                {
                    Console.WriteLine("Third argument (port) must be a valid number.");
                    return;
                }
            }


            client.Start();

            while (true)
            {
                client.SendMessage(Console.ReadLine());
            }
        }
    }
}