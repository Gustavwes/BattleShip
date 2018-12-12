using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BattleShip.Player;

namespace BattleShip.Network
{
    class GameCommandHandler
    {
        //bool not removed
        public string CommandSorter(string command, string myUserName, string otherPlayerUsername)
        {
            var playerInput = new PlayerInput();
            var game = GameRunner.Instance();
            var commandStatusCode = command.Split(' ')[0];
            //if (commandStatusCode == "221" || commandStatusCode == "222")
            //{
            //    playerInput.SendMissile(game.player2);
            //    Console.WriteLine($"{game.player1.PlayerName} GameBoard");
            //    game.player1.GameBoard.PrintCurrentBoardState(game.player1);
               
            //    Console.WriteLine($"{game.player2.PlayerName} Gameboard");
             
            //    game.player2.GameBoard.PrintCurrentBoardState(game.player2);
            //}
            if (command.ToLower() == "start")
            {
                return StartGame(myUserName, otherPlayerUsername);
            }
            return "";
        }

        public string StartGame(string myUserName, string otherPlayerUsername)
        {
            var random = new Random();
            var randomResult = random.Next(1, 10);
            if (randomResult > 5)
                return $"221 You, {myUserName} will start";
            else
            {
                return $"222 The other player, {otherPlayerUsername} will start.";
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
