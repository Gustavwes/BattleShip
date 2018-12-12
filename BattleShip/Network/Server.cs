using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace BattleShip.Network
{
    class Server
    {
        static TcpListener listener;

        static void StartListen(int port)
        {
            try
            {
                listener = new TcpListener(IPAddress.Any, port);
                listener.Start();
                Console.WriteLine($"Starts listening on port: {port}");
            }
            catch (SocketException ex)
            {
                Console.WriteLine("Couldn't host on port.");
                Environment.Exit(1);
            }
        }

        public void StartServer(int port, string hostUsername)
        {
            //Console.WriteLine("Välkommen till servern");
            //Console.WriteLine("Ange port att lyssna på:");
            //var port = int.Parse(Console.ReadLine());

            StartListen(port);

            while (true)
            {
                Console.WriteLine("Waiting for player to connect...");

                using (var client = listener.AcceptTcpClient())
                using (var networkStream = client.GetStream())
                using (StreamReader reader = new StreamReader(networkStream, Encoding.UTF8))
                using (var writer = new StreamWriter(networkStream, Encoding.UTF8) { AutoFlush = true })
                {
                    while (client.Connected)
                    {
                        var firstCommandFromClientIsHello = false;
                        var firstCommand = "";
                        var connectedUserUsername = "";
                        writer.WriteLine("210 BattleShip/1.0");
                        while (!firstCommandFromClientIsHello)
                        {
                            firstCommand = reader.ReadLine();
                            if (firstCommand.Length < 6 && firstCommand.ToLower() != "quit")
                            {
                                writer.WriteLine("500 Syntax Error");
                            }

                            if (firstCommand.Split(' ')[0].ToLower() == "helo" ||
                                firstCommand.Split(' ')[0].ToLower() == "hello")
                            {
                                connectedUserUsername = firstCommand.Split(' ')[1];
                                firstCommandFromClientIsHello = true;
                                writer.WriteLine("220 " + hostUsername);
                                continue;
                            }

                            if (firstCommand.ToLower() == "quit")
                            {
                                writer.WriteLine("270 Hasta la vista");
                                break;
                            }
                            else
                            {
                                writer.WriteLine("500 Syntax Error");
                            }

                        }
                        Console.WriteLine($"Player has connected with ip: {client.Client.RemoteEndPoint}!");
                        var command = reader.ReadLine();
                        Console.WriteLine($"Recieved: {command}");

                        if (string.Equals(command, "EXIT", StringComparison.InvariantCultureIgnoreCase))
                        {
                            writer.WriteLine("BYE BYE");
                            break;
                        }


                        if (string.Equals(command, "DATE", StringComparison.InvariantCultureIgnoreCase))
                        {
                            writer.WriteLine(DateTime.UtcNow.ToString("o"));
                            break;
                        }

                        writer.WriteLine($"UNKNOWN COMMAND: {command}");
                    }
                }

            }

        }
    }
}

