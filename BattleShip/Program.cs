using System;
using System.Net;
using System.Net.Sockets;
using BattleShip.Game_Board;
using BattleShip.Network;

namespace BattleShip
{
    class Program
    {
        static TcpListener listener;

        static void Main(string[] args)
        {

            var game = new GameRunner();
            game.RunGame();
            var portNumber = 0;
            Console.WriteLine("Welcome to Battleship! Enter a port-number to start");
            portNumber = int.Parse(Console.ReadLine());
            Console.WriteLine("Type in the host-ip if you wish to join a session. If you are hosting just press enter");
            var hostAddress = Console.ReadLine();

            if (hostAddress.Length > 1)
            {
                var networkServer = new Server();
                networkServer.StartServer();
            }
            else
            {
                var networkClient = Client.Instance();
                networkClient.StartClient(portNumber, hostAddress);

            }

        }

    }
}
