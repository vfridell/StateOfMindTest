using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateOfMindTest
{
    public class StateOfMind
    {
        private List<Routine> _routines = new List<Routine>();

        public StateOfMind()
        {
            //_routines.Add(new Dream());
            //_routines.Add(new PersonID());
            _routines.Add(new PersonIDRun());
        }

        public void RunRoutine()
        {
            Memory.GetInstance().AddToMemory(new Interaction() { resultValue = 5, displayText = "Where am I?" });

             Memory.GetInstance().AddToMemory(new Interaction() { resultValue = 5, displayText = "What's the secret number?" });
             Memory.GetInstance().AddToMemory(new Interaction() { resultValue = 5, displayText = "What's the air speed velocity of a laden swallow?" });
             Memory.GetInstance().AddToMemory(new Interaction() { resultValue = 5, displayText = "What time do we let the dogs out?" });

            foreach (Routine r in _routines)
            {
                r.Init();
                r.Run();
            }
        }
        
    }
}
