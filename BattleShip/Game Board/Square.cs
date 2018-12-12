using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BattleShip.Game_Board
{
    public class Square
    {
        public bool Hit { get; set; }
        public bool HasShip { get; set; }
        public string XAxis { get; set; }
        public int YAxis { get; set; }

        public Ship GetShipOnSquare(Player.Player player)
        {
            var shipOnSquare = player.ShipList.Where(x => x.OccupyingSquares.Any(k => k.XAxis == XAxis && k.YAxis == YAxis)).ToList();
            if (shipOnSquare.Count > 0)
                return shipOnSquare[0];

            return null;
        }

    }
}
