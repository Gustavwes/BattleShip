using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using BattleShip.Game_Board;

namespace BattleShip.Player
{
    class PlayerInput
    {
        public void SendMissile(Player playerToHit)
        {
            var boardAligner = new List<(int, string)>() { (1, "a"), (2, "b"), (3, "c"), (4, "d"), (5, "e"), (6, "f"), (7, "g"), (8, "h"), (9, "i"), (10, "j") };
            var inputInComplete = true;
            while (inputInComplete)
            {
                Console.WriteLine("Enter horizontal coordinate (A-J):");
                var inputXAxis = Console.ReadLine().ToLower();
                if (!boardAligner.Any(x => x.Item2 == inputXAxis))
                {
                    Console.WriteLine("Bad input, try again");
                    continue;
                }
                Console.WriteLine($"Enter vertical coordinate (1-10)");
                if (!int.TryParse(Console.ReadLine(), out int inputYAxis))
                {
                    Console.WriteLine("Bad input, try again");
                    continue;
                }
                if (inputYAxis > 10)
                {
                    Console.WriteLine("Outside board parameters, try again");
                    continue;
                }

                var selectedSquare = playerToHit.GameBoard.GetSquare(inputXAxis, inputYAxis, playerToHit);
                selectedSquare.Hit = true;
                inputInComplete = false;
            }

        }
        public (bool, string) ReceiveHit(string xAxis, int yAxis, Player player)
        {
            var hitShip = (false, "miss");
            var selectedSquare = player.GameBoard.GetSquare(xAxis, yAxis, player);
            var shipOnSquare = selectedSquare.GetShipOnSquare(player);


            if (selectedSquare.HasShip && shipOnSquare != null)
            {
                selectedSquare.Hit = true;
                shipOnSquare.OccupyingSquares
                        .SingleOrDefault(x => x.XAxis == selectedSquare.XAxis && x.YAxis == selectedSquare.YAxis).Hit =
                    true;
                if (CheckIfBoatIsSunk(shipOnSquare))
                {
                    hitShip = (true, $"{shipOnSquare.ShipName} is sunk");
                    return (hitShip);
                }
                hitShip = (true, "hit");
            }

            return hitShip;
        }

        public bool CheckIfBoatIsSunk(Ship ship)
        {
            var allHit = true;
            foreach (var square in ship.OccupyingSquares)
            {
                if (square.Hit == false)
                    allHit = false;
                else
                {
                    ship.IsSunk = true;
                    return allHit;
                }
            }

            ship.IsSunk = false;
            return allHit;
        }
    }
}
