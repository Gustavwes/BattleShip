using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using BattleShip.Game_Board;

namespace BattleShip.Player
{
    public class PlayerInput
    {
        public void SendMissile(Player playerToHit)
        {
            //The advantage to using this when we send a message is that could make sure we never send faulty messages. However
            //It does not help us to take care of incoming messages.

            var letterAligner = new List<(int, string)>() { (1, "a"), (2, "b"), (3, "c"), (4, "d"), (5, "e"), (6, "f"), (7, "g"), (8, "h"), (9, "i"), (10, "j") };
            var inputCorrect = false;
            while (!inputCorrect)
            {
                Console.WriteLine("Enter horizontal coordinate (A-J):");
                var inputXAxis = Console.ReadLine().ToLower();
                if (letterAligner.All(x => x.Item2 != inputXAxis))
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
                inputCorrect = true;
            }

        }
        public (bool, string) ReceiveHit(string xAxis, int yAxis, Player player, bool responseHit, bool isSunk)
        {
            var hitShip = (true, "miss");
            var selectedSquare = player.GameBoard.GetSquare(xAxis, yAxis, player);
            var shipOnSquare = selectedSquare.GetShipOnSquare(player);

            if (responseHit)
            {
                //These fakeships could possibly be named after the response we get back from the server/client.... we do get the name, and 
                //we could name the fakeships by those names, check if enough squares with that name have been sunk
                //e.g. 5 fakeships with the name carrier, and then change the status of those squares to sunk
                selectedSquare.HasShip = true;
                selectedSquare.Hit = true;
                var fakeShip = new Ship($"fakeShip{xAxis}{yAxis}", 1, "v");
                fakeShip.OccupyingSquares.Add(selectedSquare);
                shipOnSquare = fakeShip; //Does this need to be shown?
                player.ShipList.Add(fakeShip);
                if (isSunk)
                    fakeShip.IsSunk = true;
                hitShip = (true, "Hit was made"); //if we do the above solution then maybe this should be modified slightly to allow the workflow to continue past this
            }

            if (!responseHit && !isSunk)
            {
                selectedSquare.Hit = true;
                hitShip = (true, "230 Miss!");

            }

            if (selectedSquare.HasShip && shipOnSquare != null && !responseHit)
            {
                selectedSquare.Hit = true;
                shipOnSquare.OccupyingSquares.SingleOrDefault(x => x.XAxis == selectedSquare.XAxis && x.YAxis == selectedSquare.YAxis).Hit = true;

                
                if (CheckIfBoatIsSunk(shipOnSquare))
                {
                    switch (shipOnSquare.ShipName)
                    {
                        case "Carrier":
                            hitShip = (true, "251 You sunk my Carrier");
                            break;
                        case "Battleship":
                            hitShip = (true, "252 You sunk my Battleship");
                            break;
                        case "Destroyer":
                            hitShip = (true, "253 You sunk my Destroyer");
                            break;
                        case "Submarine":
                            hitShip = (true, "254 You sunk my Submarine");
                            break;
                        case "Patrol Boat":
                            hitShip = (true, "255 You sunk my Patrol Boat");
                            break;
                        default:
                            hitShip = (true, "Something went wrong, couldn't find boat");
                            break;
                    }
                    return (hitShip);
                }

                switch (shipOnSquare.ShipName)
                {
                    case "Carrier":
                        hitShip = (true, "241 You hit my Carrier");
                        break;
                    case "Battleship":
                        hitShip = (true, "242 You hit my Battleship");
                        break;
                    case "Destroyer":
                        hitShip = (true, "243 You hit my Destroyer");
                        break;
                    case "Submarine":
                        hitShip = (true, "244 You hit my Submarine");
                        break;
                    case "Patrol Boat":
                        hitShip = (true, "245 You hit my Patrol Boat");
                        break;
                    default:
                        hitShip = (true, "Something went wrong, couldn't find boat");
                        break;
                }

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
            }

            ship.IsSunk = allHit;
            return allHit;
        }

        public bool CheckIfGameOver(Player player)
        {
            return player.CheckIfAllShipsSunk() || false;
        }
    }
}
