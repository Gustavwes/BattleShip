using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BattleShip.Network
{
    class GameCommandHandler
    {
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
