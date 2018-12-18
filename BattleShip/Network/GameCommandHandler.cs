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
        public (string, bool) CommandSorter(string command)
        {

            var playerInput = new PlayerInput();
            var game = GameRunner.Instance();
            var commandStatusCode = command.Split(' ')[0];
            if (command == "help")
            {
                var returnString = "Available commands : 1. Fire <coordinates> <message> 2. Quit";
                return (returnString, false);
            }
            if (command == "start")
            {
                var returnString = StartGame(game.player1.PlayerName, game.player2.PlayerName);
                return (returnString, true);
            }

            if (command.Split(' ')[0] == "fire")
            {
                try
                {
                    var coordinates = command.Split(' ')[1];
                    var hitMessage = playerInput.ReceiveHit(coordinates.Substring(0, 1), int.Parse(coordinates.Substring(1, 1)),
                        game.player1, false, false);

                    if (game.player1.CheckIfAllShipsSunk())
                    {
                        hitMessage.Item2 = $"260 {game.player2.PlayerName} wins!";
                    }

                    return (hitMessage.Item2, hitMessage.Item1);

                }
                catch (Exception e)
                {
                    return ("500 Syntax error", false);
                }

            }
            return ("500 Syntax error", false);
        }

        public (string, bool) ResponseSorter(string response, string coordinates)
        {
            var game = GameRunner.Instance();
            var playerInput = new PlayerInput();
            var commandStatusCode = response.Split(' ')[0];
            if (coordinates.Split(' ').Length > 1)
            {
                coordinates = coordinates.Split(' ')[1];
            }
            var gameStatus = ($"Your turn {game.player2.PlayerName}", true);
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

                gameStatus.Item2 = false;
                if (statusCode == 260)
                {
                    gameStatus.Item2 = true;
                }

                if (statusCode == 500)
                    gameStatus.Item2 = true;
            }

            return gameStatus;

        }


        public string StartGame(string myUserName, string otherPlayerUsername)
        {
            var random = new Random();
            var randomResult = random.Next(1, 10);
            return randomResult > 5 ? $"221 You, {otherPlayerUsername} will start" : $"222 The other player, {myUserName} will start.";
        }

    }
}
