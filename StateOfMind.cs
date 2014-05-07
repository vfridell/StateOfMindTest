﻿using System;
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
            foreach (Routine r in _routines)
            {
                r.Init();
                r.Run();
            }
        }
        
    }
}
