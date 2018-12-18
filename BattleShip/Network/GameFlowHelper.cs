using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BattleShip.Network
{
    public class GameFlowHelper
    {
        public List<string> Last3Responses { get; set; }
        public bool StillMyTurn { get; set; }
        public List<string> ResponsesAndCommands { get; set; }

        public GameFlowHelper()
        {
            Last3Responses = new List<string>();
            ResponsesAndCommands = new List<string>();
        }

        public void PrintLast3Responses()
        {
            if (ResponsesAndCommands.Count > 3)
            {
                ResponsesAndCommands = ResponsesAndCommands.Skip(Math.Max(0, ResponsesAndCommands.Count() - 3)).ToList();
            }

            foreach (var line in ResponsesAndCommands)
            {
                Console.WriteLine(line);

            }
        }

        public bool CheckForRepeatedErrors()
        {
            var errorCount = 0;
            if (Last3Responses.Count >= 3)
            {
                Last3Responses = Last3Responses.Skip(Math.Max(0, Last3Responses.Count() - 3)).ToList();

                foreach (var response in Last3Responses)
                {
                    var commandStatusCode = response.Split(' ')[0];
                    if (int.TryParse(commandStatusCode, out int statusCode))
                    {
                        if (statusCode == 500)
                        {
                            errorCount++;
                        }
                    }
                }

                if (errorCount >= 2 && Last3Responses.Any(x=>x.Contains("quit") || x.Contains("QUIT")))
                    return true;


            }
            return false;
        }
    }
}
