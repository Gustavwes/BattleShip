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

        public void StartClient(int portNumber, string hostAddress, string userName) //kanske ska vara static
        {


            StartListen(portNumber);

            while (true)
            {
                Console.WriteLine("Waiting to connect");

                using (var client = new TcpClient(hostAddress, portNumber))
                using (var networkStream = client.GetStream())
                using (StreamReader reader = new StreamReader(networkStream, Encoding.UTF8))
                using (var writer = new StreamWriter(networkStream, Encoding.UTF8) { AutoFlush = true })
                {
                    Console.WriteLine($"Connected to host at ip: {client.Client.RemoteEndPoint}");
                    Console.WriteLine("Start the game by writing Hello");
                    var firstReplyIsCorrect = false;
                    var firstReply = "";
                    while (!firstReplyIsCorrect)
                    {
                        var firstCommand = Console.ReadLine() + " " + userName;
                        writer.WriteLine(firstCommand);
                        firstReply = reader.ReadLine();
                        if (firstReply.Take(3).ToString() == "220")
                            firstReplyIsCorrect = true;
                    }


                    while (client.Connected)
                    {
                        Console.WriteLine($"{firstReply}");
                        Console.WriteLine("Enter command to send: (write QUIT to quit)");
                        var text = Console.ReadLine();

                        if (text == "QUIT") break;

                        // Skicka text
                        writer.WriteLine(text);

                        if (!client.Connected) break;

                        // Läs minst en rad
                        do
                        {
                            var line = reader.ReadLine();
                            Console.WriteLine($"Svar: {line}");

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
