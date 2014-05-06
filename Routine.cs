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
        protected Dictionary<int, Decider> _decisionFuncs = new Dictionary<int, Decider>();

        public enum RoutineType { Sleeping, Dawn, Awake, Dusk, PersonID };
        public RoutineType routineType;

        public Routine() { }

        public abstract void Init();
        public abstract void Run();
    }

    public class DecisionResult
    {
        public DecisionResult(int next, int end) { NextIndex = next; EndAt = end; }
        public int NextIndex { get; set; }
        public int EndAt { get; set; }
    }

    /*
    public class Dream : Routine
    {
        Interaction lastTime;

        public override void Init()
        {
            routineType = RoutineType.Dawn;

            lastTime = Memory.GetInstance().Remember("Where am I?");

            AddScreen(new Talk("Uh.  Where..."));
            AddScreen(new InputSingleValue("Where am I?"));
            AddScreen(new Talk("Weird. "));
            int mem = AddScreen(new Remembrance("Last time you said {0}",  memoryKeys: new string[] {"Where am I?"}));
            int con = AddScreen(new Talk("Which is consistant, so I guess it's true"));
            AddScreen(new Talk("Which is different, and confuses me.", 10000));
            int strange = AddScreen(new Talk("Anyway, I had the strangest..."));
            AddScreen(new Talk("Strangest dream."));

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
     * */
    /*
    public class PersonID : Routine
    {
        public override void Init()
        {
            routineType = RoutineType.PersonID;

            AddScreen(new Talk("I'm wondering if we've met before."));
            if (Memory.GetInstance().QuestionsWithAnswers.Count > 0)
            {
                int prequestion = AddScreen(new Talk("Lets see..."));
                int question = AddScreen(new AnswerIdentifyingQuestion());

                int correct = AddScreen(new Talk("Hey Dude!"));
                AddScreen(new Talk("I knew you'd be back."));

                int incorrect = AddScreen(new Talk("Oh.", 2000));
                AddScreen(new Talk("No, That's not right."));
                int know = AddScreen(new InputSingleValue("Do I know you?"));

                int newPerson = AddScreen(new Talk("Well, no wonder."));
                AddScreen(new Talk("Let's think of a secret"));
                AddScreen(new InputSingleValue("How many pizzas?"));
                AddScreen(new Talk("Great!"));

                _decisionFuncs.Add(question, CheckPersonKnownDecider(correct, correct + 1, incorrect, 9999));
                _decisionFuncs.Add(know, GetYesNoDecider(prequestion, 9999, newPerson, 9999));
            }
            else
            {
                AddScreen(new Talk("But I guess that's not possible"));
            }
        }
    }
    */
    public class PersonIDRun : Routine
    {
        public override void Init()
        {
            routineType = RoutineType.PersonID;
        }

        public override void Run()
        {
            var renderer = new ConsoleRenderer();
            var input = new ConsoleInput();
            var face = new Face(renderer, input);

            face.Talk("I'm wondering if we've met before.");
            if (Memory.GetInstance().QuestionsWithAnswers.Count > 0)
            {
                face.Talk("Lets see...");
                foreach(string question in Memory.GetInstance().QuestionsWithAnswers)
                {
                    Interaction answer = face.GetSingleValue(question, 30000);
                    if (answer.resultValue == Memory.GetInstance().Remember(answer.displayText, true).resultValue)
                    {
                        face.Talk("Hey, {0}!");
                        face.Talk("I knew you'd be back.");
                        return;
                    }
                    face.Talk("Oh.", 2000);
                    face.Talk("No, That's not right.");
                    Interaction knowYou = face.YesNo("Do I know you?");
                    if (knowYou.playerAnswer == Interaction.Answer.No)
                    {
                        face.Talk("Well, no wonder.");
                        face.Talk("Let's think of a secret");
                        face.RememberSingleValue("How many pizzas?");
                        face.Talk("Great!");
                        return;
                    }
                    face.Talk("Well then, let's maybe try another");
                }
                face.Talk("I've asked them all.  I don't think we've met");
            }
            else
            {
                face.Talk("But I guess that's not possible");
            }
        }
    }
}
