using System;
using System.Collections.Generic;
using System.Text;

namespace BattleShip.Game_Board
{
    class Ship
    {
        public string ShipName { get; set; }
        public int ShipLength { get; set; }
        public string ShipOrientation { get; set; }
        public List<Square> OccupyingSquares = new List<Square>();
        public bool IsSunk = false;

        public Ship(string shipName, int shipLength, string shipOrientation)
        {
            ShipName = shipName;
            ShipLength = shipLength;
            ShipOrientation = shipOrientation;
        }
    }
}
