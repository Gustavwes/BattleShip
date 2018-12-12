using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BattleShip.Player;

namespace BattleShip.Network
{
    class GameCommandHandler
    {

        public string CommandSorter(string command, string hostUserName, string clientUserName, bool playerTurn)
        {
            var playerInput = new PlayerInput();
            var game = GameRunner.Instance();
            var commandStatusCode = command.Split(' ')[0];
            if (commandStatusCode == "221" || commandStatusCode == "222" && playerTurn)
            {
                playerInput.SendMissile(game.player2);
                Console.WriteLine($"{game.player1.PlayerName} GameBoard");
                game.player1.GameBoard.PrintCurrentBoardState(game.player1);
               
                Console.WriteLine($"{game.player2.PlayerName} Gameboard");
             
                game.player2.GameBoard.PrintCurrentBoardState(game.player2);
            }
            if (command.ToLower() == "start")
            {
                var random = new Random();
                var randomResult = random.Next(1, 10);
                if (randomResult > 5)
                    return $"221 You, {clientUserName} will start";
                else
                {
                    return $"222 Host, {hostUserName} will start.";
                }
            }
            return "";
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
