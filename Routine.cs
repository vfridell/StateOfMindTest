using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateOfMindTest
{
    public abstract class Routine
    {
        public delegate DecisionResult Decider(Interaction interaction);
        protected SortedList<int, Screen> _screens = new SortedList<int, Screen>();
        protected Dictionary<int, Decider> _decisionFuncs = new Dictionary<int, Decider>();

        public enum RoutineType { Sleeping, Dawn, Awake, Dusk, PersonID };
        public RoutineType routineType;

        public Routine()
        {
            NextScreenIndex = 1;
            EndAt = -1;
        }

        public abstract void Init();

        public virtual void Run()
        {
            do
            {
                if (!_screens.ContainsKey(NextScreenIndex))
                    return;

                Interaction result = _screens[NextScreenIndex].Render();
                if (result.resultValue != -1) Memory.GetInstance().AddToMemory(result);

                if (_decisionFuncs.ContainsKey(NextScreenIndex))
                {
                    DecisionResult decision = _decisionFuncs[NextScreenIndex](Memory.GetInstance().LastInteraction);
                    NextScreenIndex = decision.NextIndex;
                    EndAt = decision.EndAt;
                }
                else
                {
                    NextScreenIndex++;
                }
            } while (NextScreenIndex <= EndAt);
        }

        protected int NextScreenIndex { get; set; }
        protected int EndAt { get; set; }

        protected static Decider GetYesNoDecider(int yesNext, int yesEnd, int noNext, int noEnd)
        {
            return new Decider((i) => { if (i.resultValue == 1) return new DecisionResult(yesNext, yesEnd); else return new DecisionResult(noNext, noEnd); }); 
        }

        protected static Decider Goto(int index)
        {
            return new Decider(i => { return new DecisionResult(index, 9999); });
        }
    }

    public class DecisionResult
    {
        public DecisionResult(int next, int end) { NextIndex = next; EndAt = end; }
        public int NextIndex { get; set; }
        public int EndAt { get; set; }
    }


    public class Dream : Routine
    {
        Interaction lastTime;

        public override void Init()
        {
            routineType = RoutineType.Dawn;

            lastTime = Memory.GetInstance().Remember("Where am I?");

            _screens.Add(1, new Screen("Uh.  Where...", 5000));
            _screens.Add(2, new InputSingleValue("Where am I?", 15000));
            _screens.Add(3, new Screen("Weird. ", 5000));
            _screens.Add(4, new Remembrance("Last time you said {0}", 5000, new string[] {"Where am I?"}));
            _decisionFuncs.Add(4, CheckAnswer);
            _screens.Add(5, new Screen("Which is consistant, so I guess it's true", 5000));
            _decisionFuncs.Add(5, Goto(7));
            _screens.Add(6, new Screen("Which is different, and confuses me.", 10000));
            _screens.Add(7, new Screen("Anyway, I had the strangest...", 5000));
            _screens.Add(8, new Screen("Strangest dream.", 5000));

            EndAt = 8;
        }

        private DecisionResult CheckAnswer(Interaction i)
        {
            if (lastTime.resultValue == i.resultValue)
                return new DecisionResult(5, 8); 
            else 
                return new DecisionResult(6, 8);
        }
    }

    public class PersonID : Routine
    {
        public override void Init()
        {
            routineType = RoutineType.PersonID;

            _screens.Add(1, new Screen("I'm wondering if we've met before.", 5000));
            _screens.Add(2, new InputSingleValue("What is the air speed velocity of a laden swallow?", 15000));
            _decisionFuncs.Add(2, CheckAnswer);
            
            _screens.Add(3, new Screen("Hey Dude!", 5000));
            _screens.Add(4, new Screen("I knew you'd be back.", 5000));

            _screens.Add(5, new Screen("Oh.", 2000));
            _screens.Add(6, new Screen("No, That's not right.", 5000));
            _screens.Add(7, new InputSingleValue("Do I know you?", 5000));
            _decisionFuncs.Add(7, GetYesNoDecider(2, 4, 8, 11));

            _screens.Add(8, new Screen("Well, no wonder.", 5000));
            _screens.Add(9, new Screen("Let's think of a secret", 5000));
            _screens.Add(10, new InputSingleValue("How many pizzas?", 30000));
            _screens.Add(11, new Screen("Great!", 5000));

            EndAt = 11;
        }

        private DecisionResult CheckAnswer(Interaction i)
        {
            if (i.resultValue == 9) return new DecisionResult(3, 4); else return new DecisionResult(5, 7); 
        }

    }
}
