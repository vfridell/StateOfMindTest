using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateOfMindTest
{
    public class Memory
    {
        private Random _rand = new Random(4548);
        private static Memory _instance;
        protected Memory() { }
        public static Memory GetInstance()
        {
            if (null == _instance)
            {
                _instance = new Memory();
                _instance.AddToMemory(new Interaction() { resultValue = 5, playerName = "Digit", displayText = "What's the secret number?" }, true);
                _instance.AddToMemory(new Interaction() { resultValue = 5, playerName = "Foom", displayText = "What's the air speed velocity of a laden swallow?" }, true);
                _instance.AddToMemory(new Interaction() { resultValue = 5, playerName = "Pillfred", displayText = "What time do we let the dogs out?" }, true);
            }
            return _instance;
        }

        private List<Interaction> _shortTerm = new List<Interaction>();
        private List<Interaction> _longTerm = new List<Interaction>();
        private Dictionary<string, Interaction> _textMemory = new Dictionary<string, Interaction>();
        public Interaction LastInteraction
        { 
            get 
            {
                if (_shortTerm.Count > 1)
                    return _shortTerm[_shortTerm.Count - 1];
                else if (_longTerm.Count > 1)
                    return _longTerm[_longTerm.Count - 1];
                else
                    return new Interaction();
            }
        }

        public void AddToMemory(Interaction interaction, bool longTerm = false)
        {
            interaction.timestamp = DateTime.Now;
            _shortTerm.Add(interaction);
            if(longTerm) _longTerm.Add(interaction);
            _textMemory[interaction.displayText] = interaction;
        }

        public Interaction Remember(string text, bool longTerm = false)
        {
            Interaction remembered;
            if (longTerm)
            {
                remembered = _longTerm.FirstOrDefault(i => i.displayText == text);
                if (null != remembered) return remembered;
            }

            if(!_textMemory.ContainsKey(text)) return null;
            return _textMemory[text];
        }

        public List<string> Questions = new List<string>() 
        { 
            "What's the secret number?", 
            "What's the air speed velocity of a laden swallow?", 
            "How many stairs must a man fall down?",
            "What time do we let the dogs out?",
            "What is your quest?"
        };

        public List<string> QuestionsWithAnswers
        {
            get
            {
                return Questions.Where<string>(s => Remember(s, true) != null).ToList<string>();
            }
        }

        public string RandomQuestionWithNoAnswer
        {
            get
            {
                if (QuestionsWithAnswers.Count == Questions.Count)
                    return "";

                string question;
                do
                {
                    question = Questions.FirstOrDefault<string>(s => Remember(s, true) == null && _rand.Next(0, 10) > 5);
                } while (string.IsNullOrEmpty(question));
                return question;
            }
        }

        public List<string> PlayerNames = new List<string>()
        {
            "Sparky",
            "Mr Pibb",
            "The Swiper",
            "Father Time",
            "West Wind",
            "Doc Nasty"
        };

        //public string UsedPlayerNames
        //{
        //    get
        //    {
        //        return Questions.Where<string>((s) => { Remember(s) != null; });
        //    }
        //}
    }
}
