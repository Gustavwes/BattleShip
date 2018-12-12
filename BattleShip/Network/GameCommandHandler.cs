using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BattleShip.Network
{
    class GameCommandHandler
    {

        public string CommandSorter(string command, string hostUserName, string clientUserName)
        {
            if (command.ToLower() == "start")
            {
                var random = new Random();
                var randomResult = random.Next(1, 10);
                if (randomResult > 5)
                    return $"221 You, {clientUserName} will start";
                else
                {
                    return $"222 Host, {hostUserName} will start.";
                }
            }
            return "";
        }
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
