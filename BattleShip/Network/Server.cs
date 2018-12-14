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
            var game = GameRunner.Instance();
            var gameCommandHandler = new GameCommandHandler();
            StartListen(port);
            var myTurn = false;
            var responseFromClient = "";
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
                    var firstCommand = "";
                    while (client.Connected)
                    {
                        Console.WriteLine($"Player has connected with ip: {client.Client.RemoteEndPoint}!");
                        Console.WriteLine("Started A Loop on Server");
                        while (!firstCommandFromClientIsHello)
                        {
                            writer.WriteLine("210 Welcome to BattleShip/1.0");
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
                                writer.WriteLine("220 You have connect to player: " + hostUsername);
                            }

                            if (firstCommand.ToLower() == "quit")
                            {
                                writer.WriteLine("270 Hasta la vista");
                                break;
                            }
                            var command = reader.ReadLine();
                            Console.WriteLine($"Recieved: {command}");
                            var responseToSend = gameCommandHandler.CommandSorter(command);

                            if (string.Equals(responseToSend.Split(' ')[0], "222", StringComparison.InvariantCultureIgnoreCase))
                            {
                                myTurn = true;
                            }

                        writer.WriteLine(responseToSend);
                        }


                        var myCommand = "";
                        if (myTurn)
                        {
                            Console.WriteLine("Your turn, enter command:");
                            myCommand = Console.ReadLine();
                            writer.WriteLine(myCommand);
                            responseFromClient = reader.ReadLine(); // Get if its a hit or miss or need to write again
                            Console.WriteLine(responseFromClient);
                            gameCommandHandler.ResponseSorter(responseFromClient, myCommand);
                            //gameCommandHandler.CommandSorter(responseFromClient);
                            myTurn = false;
                        }
                        else
                        {
                            Console.WriteLine("Waiting for opponent move...");
                            responseFromClient = reader.ReadLine();

                            var responseToSend = gameCommandHandler.CommandSorter(responseFromClient);

                            writer.WriteLine(responseToSend); //need a bool to check if turn is over (e.g. invalid command received from client
                            myTurn = true;
                        }
                        if (string.Equals(myCommand, "EXIT", StringComparison.InvariantCultureIgnoreCase))
                        {
                            writer.WriteLine("BYE BYE");
                            break;
                        }

                        game.PrintBothGameBoards();

                    }
                }

            }

        }
    }
}

