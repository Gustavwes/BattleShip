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
            var gameFlowHelper = new GameFlowHelper();
            var gameStatus = ("", true);
            var gameOver = false;
            while (!gameOver)
            {
                Console.WriteLine("Waiting for player to connect...");

                using (var client = listener.AcceptTcpClient())
                using (var networkStream = client.GetStream())
                using (StreamReader reader = new StreamReader(networkStream, Encoding.UTF8))
                using (var writer = new StreamWriter(networkStream, Encoding.UTF8) { AutoFlush = true })
                {
                    if (gameStatus.Item1 == "270")
                    {
                        gameOver = true;
                        client.GetStream().Close();
                        networkStream.Close();
                        break;
                    }
                    var firstCommandFromClientIsHello = false;
                    var clientUserName = "";
                    var firstCommand = "";
                    while (client.Connected)
                    {
                        //Console.WriteLine($"Player has connected with ip: {client.Client.RemoteEndPoint}!");
                        //Console.WriteLine("Started A Loop on Server");
                        while (!firstCommandFromClientIsHello)
                        {
                            writer.WriteLine("210 BATTLESHIP/1.0");
                            firstCommand = reader.ReadLine();
                            if (firstCommand.ToUpper() == "QUIT")
                            {
                                firstCommand = "270 Connection closed";
                                writer.WriteLine(firstCommand);
                                gameOver = true;
                                break;
                            }
                            if (!firstCommand.ToLower().Contains("hello") && !firstCommand.ToLower().Contains("helo"))
                            {
                                writer.WriteLine("500 Syntax Error");
                            }

                            if (firstCommand.ToLower().Contains("helo") || firstCommand.ToLower().Contains("hello"))
                            {
                                clientUserName = firstCommand.Split(' ')[1];
                                game.player2.PlayerName = clientUserName;
                                firstCommandFromClientIsHello = true;
                                writer.WriteLine("220 " + hostUsername);
                            }

                            var command = reader.ReadLine().ToLower();
                            Console.WriteLine($"Recieved: {command}");
                            var responseToSend = gameCommandHandler.CommandSorter(command.ToLower()).Item1;

                            if (string.Equals(responseToSend.Split(' ')[0], "222", StringComparison.InvariantCultureIgnoreCase))
                            {
                                gameFlowHelper.StillMyTurn = true;
                            }

                            writer.WriteLine(responseToSend);
                        }


                        var myCommand = "";
                        if (gameFlowHelper.StillMyTurn)
                        {
                           Console.WriteLine("Your turn, enter command:");
                            myCommand = Console.ReadLine();
                            writer.WriteLine(myCommand);
                            responseFromClient = reader.ReadLine();
                            gameFlowHelper.Last3Responses.Add(responseFromClient); //add last response
                            gameFlowHelper.CheckForRepeatedErrors();
                            gameFlowHelper.ResponsesAndCommands.Add(myCommand);
                            gameFlowHelper.ResponsesAndCommands.Add(responseFromClient);
                            if (gameFlowHelper.CheckForRepeatedErrors())
                            {
                                writer.WriteLine("270 Connection closed");
                                gameOver = true;
                                break;
                            }
                            gameStatus = gameCommandHandler.ResponseSorter(responseFromClient, myCommand);
                            if (gameStatus.Item1.Contains("260"))
                            {
                                writer.WriteLine("270 BYE BYE");
                                client.Dispose();
                                gameOver = true;
                                break;
                            }
                            if (gameStatus.Item1 == "270")
                            {
                                break;
                            }
                            if (!gameStatus.Item2)
                                gameFlowHelper.StillMyTurn = false;
                        }
                        else
                        {
                            Console.WriteLine("Waiting for opponent move...");
                            responseFromClient = reader.ReadLine();
                            gameStatus = gameCommandHandler.CommandSorter(responseFromClient.ToLower());
                            writer.WriteLine(gameStatus.Item1); 
                            gameFlowHelper.Last3Responses.Add(responseFromClient);
                            gameFlowHelper.ResponsesAndCommands.Add(responseFromClient);
                            gameFlowHelper.ResponsesAndCommands.Add(gameStatus.Item1);
                            if (gameStatus.Item1.Contains("270"))
                            {
                                writer.WriteLine("270 BYE BYE");
                                client.Dispose();
                                gameOver = true;
                                break;
                            }
                            if (gameStatus.Item1.Contains("260"))
                            {
                                writer.WriteLine("270 BYE BYE");
                                client.Dispose();
                                gameOver = true;
                                break;
                            }
                            if (gameFlowHelper.CheckForRepeatedErrors())
                            {
                                writer.WriteLine("270 Connection closed");
                                gameOver = true;
                                networkStream.Close();
                                break;
                            }
                            if (gameStatus.Item2)
                                gameFlowHelper.StillMyTurn = true;
                        }

                        game.PrintBothGameBoards();
                        gameFlowHelper.PrintLast3Responses();

                    }
                    gameOver = true;
                    client.Dispose();
                    break;
                }
                gameOver = true;
                break;
            }

        }
    }
}

