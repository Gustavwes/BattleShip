using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace BattleShip.Game_Board
{
    public class GameBoard
    {
        public List<Square> GameSquares { get; set; }

        public GameBoard()
        {
            GameSquares = GenerateGameBoard();
        }
        public List<Square> GenerateGameBoard()
        {
            var gameSquares = new List<Square>();
            var XAxis = "ABCDEFGHIJ";
            for (int j = 0; j < 10; j++)
            {
                for (int i = 0; i < 10; i++)
                {
                    var gameSquare = new Square();
                    gameSquare.XAxis = XAxis[j].ToString();
                    gameSquare.YAxis = (i + 1);
                    gameSquares.Add(gameSquare);
                }
            }

            return gameSquares;
        }

        public void PrintCurrentBoardState(Player.Player player)
        {
            var XAxis = "ABCDEFGHIJ";
            Console.Write("   ");
            for (int i = 0; i < 10; i++)
            {
                Console.Write(XAxis[i]);

                for (int j = 0; j < 1; j++)
                {
                    Console.Write("  ");
                }
            }
            Console.WriteLine();

            for (int i = 0; i <= 10; i++)
            {
                var lineOf10 = new List<Square>();
                lineOf10 = GameSquares.Where(x => x.YAxis == i % 10).ToList();
                if (i == 10)
                    lineOf10 = GameSquares.Where(x => x.YAxis % 10 == 0).ToList();
                if (lineOf10.Count > 1)
                    DrawXAxis(lineOf10, player, false);
            }

        }

        private void DrawXAxis(List<Square> listOfSquares, Player.Player player, bool hasSunk)
        {

            Console.Write($"{listOfSquares[0].YAxis}");
            if (listOfSquares[0].YAxis != 10)
                Console.Write(" ");

            foreach (var square in listOfSquares)
            {
                var shipOnSquare = square.GetShipOnSquare(player); //jobbar med att kunna printa ut om något har sjunkit
                if (square.HasShip && square.Hit == false)
                {
                    Console.Write($"|O|");
                    continue;
                }

                if (square.HasShip && square.Hit)
                {
                    if (shipOnSquare.IsSunk || hasSunk)
                        Console.Write("|S|");
                    else
                    {
                        Console.Write($"|X|");
                    }
                    continue;
                }
                if (square.HasShip == false && square.Hit)
                    Console.Write($"|M|");
                else
                {
                    Console.Write("| |");
                }
            }
            Console.WriteLine();
        }
        public Square GetSquare(string inputXAxis, int inputYAxis, Player.Player player)
        {
            return player.GameBoard.GameSquares.SingleOrDefault(x =>
                x.XAxis.ToLower() == inputXAxis && x.YAxis == inputYAxis);
        }
    }
}
