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
            EndAt = 9999;
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

        protected int _addIndex = 0;
        protected int AddScreen(Screen screen)
        {
            _addIndex++;
            _screens.Add(_addIndex, screen);
            return _addIndex;
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

        protected static Decider CheckAnswerDecider(int correctNext, int correctEnd, int wrongNext, int wrongEnd)
        {
            return new Decider((i) => 
            {
                Interaction correctMem = Memory.GetInstance().Remember(i.displayText);
                if (null != correctMem && i.resultValue == Memory.GetInstance().Remember(i.displayText).resultValue) 
                    return new DecisionResult(correctNext, correctEnd); 
                else 
                    return new DecisionResult(wrongNext, wrongEnd); 
            });
        }

        protected static Decider CheckPersonKnownDecider(int yesNext, int yesEnd, int noNext, int noEnd)
        {
            return new Decider((i) =>
            {
                if (i.displayText == AnswerIdentifyingQuestion.NoOneKnown ||
                    i.displayText == AnswerIdentifyingQuestion.AllQuestionsAsked)
                {
                    return new DecisionResult(noNext, noEnd);
                }

                Interaction correctMem = Memory.GetInstance().Remember(i.displayText);
                if (null != correctMem && i.resultValue == Memory.GetInstance().Remember(i.displayText).resultValue)
                    return new DecisionResult(yesNext, yesEnd);
                else
                    return new DecisionResult(noNext, noEnd);
            });
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

            AddScreen(new Screen("Uh.  Where..."));
            AddScreen(new InputSingleValue("Where am I?"));
            AddScreen(new Screen("Weird. "));
            int mem = AddScreen(new Remembrance("Last time you said {0}",  memoryKeys: new string[] {"Where am I?"}));
            int con = AddScreen(new Screen("Which is consistant, so I guess it's true"));
            AddScreen(new Screen("Which is different, and confuses me.", 10000));
            int strange = AddScreen(new Screen("Anyway, I had the strangest..."));
            AddScreen(new Screen("Strangest dream."));

            _decisionFuncs.Add(mem, CheckAnswer);
            _decisionFuncs.Add(con, Goto(strange));
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

            AddScreen(new Screen("I'm wondering if we've met before."));
            if (Memory.GetInstance().QuestionsWithAnswers.Count > 0)
            {
                int prequestion = AddScreen(new Screen("Lets see..."));
                int question = AddScreen(new AnswerIdentifyingQuestion());

                int correct = AddScreen(new Screen("Hey Dude!"));
                AddScreen(new Screen("I knew you'd be back."));

                int incorrect = AddScreen(new Screen("Oh.", 2000));
                AddScreen(new Screen("No, That's not right."));
                int know = AddScreen(new InputSingleValue("Do I know you?"));

                int newPerson = AddScreen(new Screen("Well, no wonder."));
                AddScreen(new Screen("Let's think of a secret"));
                AddScreen(new InputSingleValue("How many pizzas?"));
                AddScreen(new Screen("Great!"));

                _decisionFuncs.Add(question, CheckPersonKnownDecider(correct, correct + 1, incorrect, 9999));
                _decisionFuncs.Add(know, GetYesNoDecider(prequestion, 9999, newPerson, 9999));
            }
            else
            {
                AddScreen(new Screen("But I guess that's not possible"));
            }
        }
    }

    public class PersonIDRun : Routine
    {
        public override void Init()
        {
            routineType = RoutineType.PersonID;
        }

        public override void Run()
        {

            new Screen("I'm wondering if we've met before.").Render();
            if (Memory.GetInstance().QuestionsWithAnswers.Count > 0)
            {
                new Screen("Lets see...").Render();
                Interaction answer;
                do
                {
                    answer = new AnswerIdentifyingQuestion().Render();
                    if (answer.resultValue == Memory.GetInstance().Remember(answer.displayText).resultValue)
                    {
                        new Screen("Hey Dude!").Render();
                        new Screen("I knew you'd be back.").Render();
                        return;
                    }
                    new Screen("Oh.", 2000).Render();
                    new Screen("No, That's not right.").Render();
                    Interaction knowYou = new InputSingleValue("Do I know you?").Render();
                    if (knowYou.resultValue == 2)
                    {
                        new Screen("Well, no wonder.").Render();
                        new Screen("Let's think of a secret").Render();
                        new InputSingleValue("How many pizzas?").Render();
                        new Screen("Great!").Render();
                        return;
                    }
                    new Screen("Well then, let's maybe try another").Render();
                } while (answer.displayText != AnswerIdentifyingQuestion.AllQuestionsAsked);
                new Screen("I've asked them all.  I don't think we've met").Render();
            }
            else
            {
                new Screen("But I guess that's not possible").Render();
            }
        }
    }
}
