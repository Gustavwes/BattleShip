using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

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
            StartListen(portNumber);

            while (true)
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

                    Console.WriteLine("Started A Loop on Client");
                    var firstReplyIsCorrect = false;
                    while (client.Connected)
                    {
                        while (!firstReplyIsCorrect)
                        {
                            var firstReply = "";
                            Console.WriteLine($"Connected to host at ip: {client.Client.RemoteEndPoint}");
                            Console.WriteLine(reader.ReadLine());
                            Console.WriteLine("Start the game by writing Hello");
                            var firstCommand = Console.ReadLine() + " " + userName;
                            writer.WriteLine(firstCommand);
                            firstReply = reader.ReadLine();
                            if (firstReply.Split(' ')[0] == "220")
                            {
                                Console.WriteLine($"{firstReply}");
                                game.player2.PlayerName = firstReply.Split(' ')[1];
                                firstReplyIsCorrect = true;
                                Console.WriteLine("Enter Start to start: ");
                                var command = Console.ReadLine();

                                if (command == "QUIT") break;

                                // Skicka text
                                writer.WriteLine(command);
                            }
                        if (!client.Connected) break;

                        responseFromServer = reader.ReadLine();
                        Console.WriteLine($"Svar: {responseFromServer}");
                            if (string.Equals(responseFromServer.Split(' ')[0], "221", StringComparison.InvariantCultureIgnoreCase))
                            {
                                myTurn = true;
                            }

                            Console.WriteLine(responseFromServer);
                        }


                        // Läs minst en rad test
                        do
                        {
                            if (myTurn)
                            {
                                Console.WriteLine("Your turn, enter command:");
                                myCommand = Console.ReadLine();
                                writer.WriteLine(myCommand);
                                responseFromServer = reader.ReadLine();
                                myCommand = gameCommandHandler.ResponseSorter(responseFromServer, myCommand);
                                writer.WriteLine(myCommand); //need checks to see if turn is over or need to wait for next server turn (e.g. faulty input)
                                myTurn = false;
                            }
                            else
                            {
                                Console.WriteLine("Waiting for opponent move...");
                                responseFromServer = reader.ReadLine();
                                myResponse = gameCommandHandler.CommandSorter(responseFromServer);
                                writer.WriteLine(myResponse); //need checks to see if turn is over or need to wait for next server turn (e.g. faulty input)
                                myTurn = true;

                            }
                            game.PrintBothGameBoards();
                            
                        } while (networkStream.DataAvailable);

                    }

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
