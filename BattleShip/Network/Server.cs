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
            var game = GameRunner.Instance();
            var gameCommandHandler = new GameCommandHandler();
            StartListen(port);
            var myTurn = false;

            while (true)
            {
                Console.WriteLine("Waiting for player to connect...");

                using (var client = listener.AcceptTcpClient())
                using (var networkStream = client.GetStream())
                using (StreamReader reader = new StreamReader(networkStream, Encoding.UTF8))
                using (var writer = new StreamWriter(networkStream, Encoding.UTF8) { AutoFlush = true })
                {
                        var firstCommandFromClientIsHello = false;
                    var clientUserName = "";
                    while (client.Connected)
                    {
                        var firstCommand = "";
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
                                clientUserName = firstCommand.Split(' ')[1];
                                game.player2.PlayerName = clientUserName;
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
                        //Console.WriteLine($"Player has connected with ip: {client.Client.RemoteEndPoint}!");
                        var command = reader.ReadLine();
                        Console.WriteLine($"Recieved: {command}");
                        var responseToSend = gameCommandHandler.CommandSorter(command, hostUsername, clientUserName);
                        
                        if (string.Equals(responseToSend.Split(' ')[0], "222", StringComparison.InvariantCultureIgnoreCase))
                        {
                            myTurn = true;
                        }

                        if (myTurn)
                        {
                            Console.WriteLine("Your turn, enter command:");
                            command = Console.ReadLine();
                            responseToSend = gameCommandHandler.CommandSorter(command, hostUsername, clientUserName);
                        }
                        if (string.Equals(command, "EXIT", StringComparison.InvariantCultureIgnoreCase))
                        {
                            writer.WriteLine("BYE BYE");
                            break;
                        }

                        myTurn = false;
                        writer.WriteLine(responseToSend);

                    }
                }

            }

        }
    }
}

