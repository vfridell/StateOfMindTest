using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateOfMindTest
{
    public abstract class Routine
    {
        public enum RoutineType { Sleeping, Dawn, Awake, Dusk, PersonID };
        public RoutineType routineType;

        public Routine() { }

        public abstract void Init();
        public abstract Interaction Run();
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

    public class PersonIDRun : Routine
    {
        public override void Init()
        {
            routineType = RoutineType.PersonID;
        }

        public override Interaction Run()
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
                    Interaction player = Memory.GetInstance().Remember(answer.displayText, true);
                    if (answer.resultValue == player.resultValue)
                    {
                        face.Talk(string.Format("Hey, {0}!", player.playerName));
                        face.Talk("I knew you'd be back.");
                        return player;
                    }
                    face.Talk("Oh.", 2000);
                    face.TalkInCircles(5000, "No, That's not right.", "Nope");
                    Interaction knowYou = Memory.GetInstance().Remember("Do I know you?");
                    if(null == knowYou) knowYou = face.YesNo("Do I know you?");
                    if (knowYou.playerAnswer == Interaction.Answer.No)
                    {
                        face.Talk("Well, no wonder.");
                        return new Interaction() { success = false };
                    }
                    face.Talk("Well then, let's maybe try another");
                }
                face.Talk("Actually, I don't think we've met.");
            }
            else
            {
                face.Talk("But I guess that's not possible");
            }
            return new Interaction() { success = false };
        }
    }

    public class CreatePlayer : Routine
    {
        public override void Init()
        {
            routineType = RoutineType.PersonID; 
        }

        public override Interaction Run()
        {
            var renderer = new ConsoleRenderer();
            var input = new ConsoleInput();
            var face = new Face(renderer, input);

            face.Talk("Let's think of a secret");
            string question = Memory.GetInstance().RandomQuestionWithNoAnswer;
            if (string.IsNullOrEmpty(question))
            {
                face.Talk("Sorry, I'm full on friends");
                return new Interaction() { success = false };
            }

            Interaction newPlayer = face.RememberSingleValue(question, true);
            face.Talk("Great!");
            return newPlayer;
        }
    }

    public class ChitChat : Routine
    {
        public override void Init()
        {
            routineType = RoutineType.Awake;
        }

        public override Interaction Run()
        {
            var renderer = new ConsoleRenderer();
            var input = new ConsoleInput();
            var face = new Face(renderer, input);

            face.Talk("So happy to be here!");
            face.RememberSingleValue("What's new?");
            face.Talk("Uh huh.");
            return new Interaction();
        }
    }
}
