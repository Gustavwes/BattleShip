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
            if (command == "quit")
            {
                var returnString = "270 Bye Bye";
                return (returnString, false);
            }
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

            if (command.Split(' ')[0].ToLower() == "fire")
            {
                var fireCoordinates = GetPositionFromFireCommand(command);
                try
                {
                    var coordinates = command.Split(' ')[1].ToLower();
                    var hitMessage = playerInput.ReceiveHit(fireCoordinates.character, fireCoordinates.number,
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
            if (commandStatusCode == "270")
                return ("270", false);
            if (commandStatusCode == "260")
                return ("270", false);

            if (coordinates.Split(' ').Length > 1)
            {
                coordinates = coordinates.Split(' ')[1];
            }
            var gameStatus = ($"Your turn {game.player2.PlayerName}", true);
            // TODO: to implement updating of opponent board
            if (!String.IsNullOrWhiteSpace(coordinates))

                if (int.TryParse(commandStatusCode, out int statusCode))
                {
                    var fireCoordinates = GetPositionFromFireCommand("fire " + coordinates);
                    if (statusCode == 230)
                    {
                        playerInput.ReceiveHit(fireCoordinates.character, fireCoordinates.number,
                            game.player2, false, false);
                    }
                    if (statusCode > 240 && statusCode < 250)
                    {
                        playerInput.ReceiveHit(fireCoordinates.character, fireCoordinates.number,
                             game.player2, true, false);
                    }
                    if (statusCode > 250 && statusCode < 260)
                    {
                        playerInput.ReceiveHit(fireCoordinates.character, fireCoordinates.number,
                            game.player2, true, true);
                    }

                    gameStatus.Item2 = false;
                    if (statusCode == 260)
                    {
                        gameStatus.Item2 = true;
                    }

                    if (statusCode == 500 || statusCode == 501)
                        gameStatus.Item2 = true;
                }

            return gameStatus;

        }

        public (string character, int number) GetPositionFromFireCommand(string command)
        {
            var parts = command.Split(' ');
            var character = parts[1][0].ToString();
            var number = int.Parse(parts[1].Substring(1));

            return (character, number);
        }

        public string StartGame(string myUserName, string otherPlayerUsername)
        {
            var random = new Random();
            var randomResult = random.Next(1, 10);
            return randomResult > 5 ? $"221 You, {otherPlayerUsername} will start" : $"222 The other player, {myUserName} will start.";
        }

    }
}
