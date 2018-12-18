using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace BattleShip.Network
{
    public class Client
    {
        static TcpListener listener;
        private static Client _instance;

        public static Client Instance()
        {
            return _instance ?? (_instance = new Client());
        }

        public void StartClient(int portNumber, string hostAddress, string userName, Player.Player hostPlayer, Player.Player clientPlayer) //kanske ska vara static
        {

            var gameCommandHandler = new GameCommandHandler();
            var game = GameRunner.Instance();
            var gameStatus = ("", true);
            StartListen(portNumber);
            var gameFlowHelper = new GameFlowHelper();
            var gameOver = false;
            while (!gameOver)
            {
                Console.WriteLine("Waiting to connect");
                var myTurn = false;
                var hostUsername = "";
                var responseToServer = "";
                var myCommand = "";
                var myResponse = "";
                var responseFromServer = "";

                using (var client = new TcpClient(hostAddress, portNumber))
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
                    Console.WriteLine("Started A Loop on Client");
                    var firstReplyIsCorrect = false;
                    while (client.Connected)
                    {
                        while (!firstReplyIsCorrect)
                        {
                            var firstReply = "";
                            Console.WriteLine($"Connected to host at ip: {client.Client.RemoteEndPoint}");
                            Console.WriteLine(reader.ReadLine());
                            Console.WriteLine("Start the game by writing HELLO");
                            var firstCommand = Console.ReadLine() + " " + userName;
                            writer.WriteLine(firstCommand);
                            firstReply = reader.ReadLine();
                            if (firstReply.Split(' ')[0] == "220")
                            {
                                Console.WriteLine($"{firstReply}");
                                game.player2.PlayerName = firstReply.Split(' ')[1];
                                firstReplyIsCorrect = true;
                                Console.WriteLine("Enter START to start: ");
                                var startCommand = Console.ReadLine();

                                if (startCommand.ToUpper() == "QUIT")
                                {
                                    startCommand = "270 Connection closed";
                                    gameOver = true;
                                    writer.WriteLine(startCommand);
                                    break;
                                }

                                // Skicka text
                                writer.WriteLine(startCommand);
                            }
                            
                            if (!client.Connected) break;

                            responseFromServer = reader.ReadLine();
                            Console.WriteLine($"Svar: {responseFromServer}");
                            if (string.Equals(responseFromServer.Split(' ')[0], "221", StringComparison.InvariantCultureIgnoreCase))
                            {
                                gameFlowHelper.StillMyTurn = true;
                            }

                            Console.WriteLine(responseFromServer);
                        }

                        
                        // Läs minst en rad test
                        do
                        {
                            if (gameFlowHelper.StillMyTurn)
                            {
                                Console.WriteLine("Your turn, enter command:");
                                myCommand = Console.ReadLine();
                                writer.WriteLine(myCommand);
                                responseFromServer = reader.ReadLine();
                                gameFlowHelper.Last3Responses.Add(responseFromServer); //add last response
                                if (gameFlowHelper.CheckForRepeatedErrors())
                                {
                                    writer.WriteLine("QUIT");
                                    client.Close();
                                    gameOver = true;
                                    break;
                                }
                                gameFlowHelper.ResponsesAndCommands.Add(myCommand);
                                gameFlowHelper.ResponsesAndCommands.Add(responseFromServer);
                                //gameFlowHelper.Last3Responses.Add(responseFromServer);
                                gameStatus = gameCommandHandler.ResponseSorter(responseFromServer, myCommand);
                                if (gameStatus.Item1 == "260")
                                {
                                    Console.WriteLine(responseFromServer);
                                    writer.WriteLine("QUIT");
                                    gameOver = true;
                                    client.Close();
                                    break;
                                }
                                if (gameStatus.Item1 == "270")
                                {
                                    writer.WriteLine("QUIT");
                                    gameOver = true;
                                    client.Close();
                                    break;
                                }
                                //writer.WriteLine(myResponse); //need checks to see if turn is over or need to wait for next server turn (e.g. faulty input)
                                if (!gameStatus.Item2)
                                    gameFlowHelper.StillMyTurn = false;
                            }
                            else
                            {
                                Console.WriteLine("Waiting for opponent move...");
                                responseFromServer = reader.ReadLine();
                                gameStatus = gameCommandHandler.CommandSorter(responseFromServer.ToLower());
                                //here we need gamestatus to know if their turn is over or if we need to continue our turn (loop around this)
                                writer.WriteLine(gameStatus.Item1); //need checks to see if their turn is over or need to wait for next server turn (e.g. faulty input)
                                gameFlowHelper.ResponsesAndCommands.Add(responseFromServer);
                                gameFlowHelper.ResponsesAndCommands.Add(gameStatus.Item1);
                                gameFlowHelper.Last3Responses.Add(gameStatus.Item1);
                                if (gameStatus.Item1.Contains("260"))
                                {
                                    writer.WriteLine("QUIT");
                                    gameOver = true;
                                    client.Close();
                                    break;
                                }
                                if (gameFlowHelper.CheckForRepeatedErrors())
                                {
                                    writer.WriteLine("QUIT");
                                    gameOver = true;
                                    client.Close();
                                    break;
                                }
                                if (gameStatus.Item2)
                                    gameFlowHelper.StillMyTurn = true;

                            }
                            game.PrintBothGameBoards();
                            gameFlowHelper.PrintLast3Responses();

                        } while (networkStream.DataAvailable);
                        //gameOver = true;
                        //client.Dispose();
                    }
                    gameOver = true;
                    break;

                }

            }
        }

        

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
                Console.WriteLine("Misslyckades att öppna socket. Troligtvis upptagen.");
                Environment.Exit(1);
            }
        }
    }
}
