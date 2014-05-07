using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateOfMindTest
{
    public class StateOfMind
    {
        public StateOfMind()
        {
        }

        public void RunRoutine()
        {
            Routine idRoutine = new PersonIDRun();
            Routine secondOne;
            idRoutine.Init();
            Interaction result = idRoutine.Run();
            if (string.IsNullOrEmpty(result.playerName))
                secondOne = new CreatePlayer();
            else
                secondOne = new ChitChat();

            secondOne.Init();
            secondOne.Run();

        }
        
    }
}
