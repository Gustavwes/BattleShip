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
        public (string,bool) CommandSorter(string command)
        {

            var playerInput = new PlayerInput();
            var game = GameRunner.Instance();
            var commandStatusCode = command.Split(' ')[0];

            if (command == "start")
            {
                return (StartGame(game.player1.PlayerName, game.player2.PlayerName), true);
            }

            if (command.Split(' ')[0] == "fire")
            {
                // Regex rx = new Regex("^FIRE [A-H]([1-9]|10)([ ]|$)");
                var coordinates = command.Split(' ')[1];
                var hitMessage = playerInput.ReceiveHit(coordinates.Substring(0, 1), int.Parse(coordinates.Substring(1, 1)),
                    game.player1, false, false);

                if (game.player1.CheckIfAllShipsSunk())
                {
                    hitMessage.Item2 = "260 You Win!";
                }

                return (hitMessage.Item2, hitMessage.Item1);

            }
            return ("500 Syntax error",false);
        }

        public (string, bool) ResponseSorter(string response, string coordinates)
        {
            var game = GameRunner.Instance();
            var playerInput = new PlayerInput();
            var commandStatusCode = response.Split(' ')[0];
            coordinates = coordinates.Split(' ')[1];
            var gameStatus = ($"Your turn {game.player2.PlayerName}", false);
            // TODO: to implement updating of opponent board
            if (int.TryParse(commandStatusCode, out int statusCode))
            {

                if (statusCode == 230)
                {
                    playerInput.ReceiveHit(coordinates.Substring(0, 1), int.Parse(coordinates.Substring(1, 1)),
                        game.player2, false, false);
                }
                if (statusCode > 240 && statusCode < 250)
                {
                    playerInput.ReceiveHit(coordinates.Substring(0, 1), int.Parse(coordinates.Substring(1, 1)),
                         game.player2, true, false);
                }
                if (statusCode > 250 && statusCode < 260)
                {
                    playerInput.ReceiveHit(coordinates.Substring(0, 1), int.Parse(coordinates.Substring(1, 1)),
                        game.player2, true, true);
                }

                if (statusCode == 260)
                {
                    gameStatus.Item2 = true;
                }

                if (statusCode == 500)
                    gameStatus.Item2 = false;
            }
            else
            {
                
            }


            return gameStatus;

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
