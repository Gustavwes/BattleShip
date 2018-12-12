using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BattleShip.Player;

namespace BattleShip.Network
{
    class GameCommandHandler
    {

        public string CommandSorter(string command, string hostUserName, string clientUserName, bool myTurn)
        {
            var playerInput = new PlayerInput();
            var game = GameRunner.Instance();
            var commandStatusCode = command.Split(' ')[0];
            if (commandStatusCode == "221" || commandStatusCode == "222" && myTurn)
            {
                playerInput.SendMissile(game.player2);
                Console.WriteLine($"{game.player1.PlayerName} GameBoard");
                game.player1.GameBoard.PrintCurrentBoardState(game.player1);
               
                Console.WriteLine($"{game.player2.PlayerName} Gameboard");
             
                game.player2.GameBoard.PrintCurrentBoardState(game.player2);
            }
            if (command.ToLower() == "start")
            {
                return StartGame(clientUserName, hostUserName);
            }
            return "";
        }

        public string StartGame(string localPlayer, string remotePlayer)
        {
            var random = new Random();
            var randomResult = random.Next(1, 10);
            if (randomResult > 5)
                return $"221 You, {localPlayer} will start";
            else
            {
                return $"222 The other player, {remotePlayer} will start.";
            }
        }
        public void ReceiveHitMessage()
        {
            //Send back Hit/miss/sunk/win
        }

        public void SendHitMessage()
        {

        }

        public void FromHostReplySplitter(string recievedData)
        {
            var statusCode = recievedData.Take(3);
            var restOfString = recievedData.Skip(3);
        }
    }
}
