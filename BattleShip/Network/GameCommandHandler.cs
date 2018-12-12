using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using BattleShip.Player;

namespace BattleShip.Network
{
    class GameCommandHandler
    {
        
        //bool not removed
        public string CommandSorter(string command)
        {
            
            var playerInput = new PlayerInput();
            var game = GameRunner.Instance();
            var commandStatusCode = command.Split(' ')[0];
   
            if (command.ToLower() == "start")
            {
                return StartGame(game.player1.PlayerName, game.player2.PlayerName);
            }

            if (command.Split(' ')[0].ToLower() == "fire")
            {
               // Regex rx = new Regex("^FIRE [A-H]([1-9]|10)([ ]|$)");
                var coordinates = command.Split(' ')[1];
                var hitMessage = playerInput.ReceiveHit(coordinates.Substring(0, 1), int.Parse(coordinates.Substring(1, 1)),
                    game.player1, false);
                
                return hitMessage.Item2;
                
            }
            return "";
        }

        public string ResponseSorter(string response, string coordinates)
        {
            var game = GameRunner.Instance();
            var playerInput = new PlayerInput();
            var commandStatusCode = response.Split(' ')[0];
            coordinates = coordinates.Split(' ')[1];
            // TODO: to implement updating of opponent board
            if (int.TryParse(commandStatusCode, out int statusCode))
            {
                if (statusCode > 240 && statusCode < 256)
                {
                    playerInput.ReceiveHit(coordinates.Substring(0, 1), int.Parse(coordinates.Substring(1, 1)),
                        game.player2, true);
                }
            }

            return "Your turn " + game.player2.PlayerName;
            
        }

       
        public string StartGame(string myUserName, string otherPlayerUsername)
        {
            var random = new Random();
            var randomResult = random.Next(1, 10);
            if (randomResult > 5)
                return $"221 You, {otherPlayerUsername} will start";
            else
            {
                return $"222 The other player, {myUserName} will start.";
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
