﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BattleShip.Game_Board;
using BattleShip.Player;

namespace BattleShip
{
    class GameRunner
    {
       
        public void RunGame(string playerName)
        {
            var player1 = new Player.Player();
            var player2 = new Player.Player();
            //var playerInput = new PlayerInput();
            
            player1.PlayerName = playerName;
            player1.GameBoard = new GameBoard();
            player1.GameBoard.GenerateGameBoard();
            player1.ShipList = player1.GenerateShips();
            player1.GameBoard.PrintCurrentBoardState(player1);
            Console.WriteLine();
            PlaceShips(player1);
            Console.Clear();
            Console.WriteLine("Player 1 Gameboard");
            player1.GameBoard.PrintCurrentBoardState(player1);
            player2.GameBoard = new GameBoard();
            player2.ShipList = player2.GenerateShips();
            Console.WriteLine("Player 2 Gameboard");
            player2.GameBoard.GenerateGameBoard();
            player2.GameBoard.PrintCurrentBoardState(player2);

            //Tests
            //player2.GameBoard.GameSquares[0].HasShip = true;
            //player2.GameBoard.GameSquares[0].Hit=true;
            //player2.GameBoard.GameSquares[1].Hit = true;
            //Console.Clear();
            //playerInput.SendMissile(player2);
            //playerInput.ReceiveHit("a",5,player1);
            //playerInput.ReceiveHit("b", 5, player1);

            //player1.GameBoard.PrintCurrentBoardState(player1);
        }

        public void PlaceShips(Player.Player player)
        {
            var boardAligner = new List<(int, string)>() { (1, "a"), (2, "b"), (3, "c"), (4, "d"), (5, "e"), (6, "f"), (7, "g"), (8, "h"), (9, "i"), (10, "j") };

            Console.WriteLine("Place your ships:");
            foreach (var ship in player.ShipList)
            {
                var isOccupied = true; //
                var squareList = new List<Square>();
                while (isOccupied)
                {
                    Console.WriteLine($"Enter start position of {ship.ShipName} (length of {ship.ShipLength}) on the X-axis (A-J)");
                    var inputXAxis = Console.ReadLine().ToLower();
                    if (!boardAligner.Any(x => x.Item2 == inputXAxis))
                    {
                        Console.WriteLine("Bad input, try again");
                        continue;
                    }

                    Console.WriteLine($"Enter start position of {ship.ShipName} on the Y-axis (1-10)");
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
                    //NEED CHECK TO MAKE SURE THAT WE ONLY GET PROPER INPUT
                    Console.WriteLine("Do you want horizontal (H) or vertical (V) alignment?");
                    var alignment = Console.ReadLine().ToLower();
                    if (alignment != "v" && alignment != "h")
                    {
                        Console.WriteLine("Bad input, try again");
                        continue;
                    }
                    //var inputXAxis = XAxis;
                    //var inputYAxis = YAxis;
                    var currentXAxis = boardAligner.SingleOrDefault(x => x.Item2 == inputXAxis);

                    if ((inputYAxis + ship.ShipLength > 10 && alignment.ToLower() == "v") || (currentXAxis.Item1 + ship.ShipLength > 10 && alignment.ToLower() == "h"))
                    {
                        Console.WriteLine("Can't place ship! It would fall outside the gameboard");
                        continue;
                    }

                    squareList = GetConcernedSquaresForShipPlacement(player, alignment, inputXAxis, inputYAxis, ship.ShipLength);
                    if (squareList.Any(x => x.HasShip))
                    {
                        Console.WriteLine("Can't place ship! These squares are occupied:");
                        var clashSquareList = squareList.Where(x => x.HasShip == true).ToList();
                        foreach (var clashingSquare in clashSquareList)
                        {
                            Console.WriteLine($"{clashingSquare.XAxis}{clashingSquare.YAxis}");
                        }
                        continue;
                        //Returnera not valid eftersom rutan redan är upptagen
                    }

                    isOccupied = false;
                }

                foreach (var square in squareList)
                {
                    square.HasShip = true;
                    ship.OccupyingSquares.Add(square);
                }


                Console.Clear();
                player.GameBoard.PrintCurrentBoardState(player);
            }
        }

        private List<Square> GetConcernedSquaresForShipPlacement(Player.Player player, string alignment, string inputLetter, int inputNumber, int shipLength)
        {
            var returnListOfSquares = new List<Square>();
            var boardAligner = new List<(int, string)>() { (1, "a"), (2, "b"), (3, "c"), (4, "d"), (5, "e"), (6, "f"), (7, "g"), (8, "h"), (9, "i"), (10, "j") };
            var startSquareCharacter = boardAligner.SingleOrDefault(x => x.Item2 == inputLetter.ToLower());
            for (int i = 0; i < shipLength; i++)
            {
                if (alignment.ToLower() == "h")
                {
                    var nextSquare = boardAligner.SingleOrDefault(x => x.Item1 == startSquareCharacter.Item1 + i);
                    var currentSquare =
                        player.GameBoard.GameSquares.SingleOrDefault(x => int.Parse(x.YAxis.ToString()) == inputNumber && x.XAxis.ToLower() == nextSquare.Item2.ToLower());
                    returnListOfSquares.Add(currentSquare);
                }
                else
                {

                    var currentSquare =
                        player.GameBoard.GameSquares.SingleOrDefault(x => int.Parse(x.YAxis.ToString()) == inputNumber + i && x.XAxis.ToLower() == inputLetter.ToLower());
                    returnListOfSquares.Add(currentSquare);
                }
            }

            return returnListOfSquares;
        }
    }
}