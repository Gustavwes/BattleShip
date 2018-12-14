using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BattleShip.Game_Board;

namespace BattleShip.Player
{
    public class Player
    {
        public string PlayerName { get; set; }
        public List<Ship> ShipList { get; set; }
        public GameBoard GameBoard { get; set; }

        public List<Ship> GenerateShipsForPlayer()
        {
            var carrier = new Ship("Carrier", 5, "X");
            var battleShip = new Ship("BattleShip", 4, "X");
            var destroyer = new Ship("Destroyer", 3, "X");
            var submarine = new Ship("Submarine", 3, "X");
            var patrolBoat = new Ship("Patrol Boat", 2, "X");

            var shipList = new List<Ship>();
            shipList.Add(carrier);
            shipList.Add(battleShip);
            shipList.Add(destroyer);
            shipList.Add(submarine);
            shipList.Add(patrolBoat);
            return shipList;

        }

        public bool CheckIfAllShipsSunk()
        {
            return !ShipList.Any(x => x.IsSunk == false);
        }
    }

}
