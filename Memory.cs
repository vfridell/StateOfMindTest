using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateOfMindTest
{
    public class Memory
    {
        private static Memory _instance;
        protected Memory() { }
        public static Memory GetInstance()
        {
            if (null == _instance)
            {
                _instance = new Memory();
            }
            return _instance;
        }

        private List<Interaction> _interactions = new List<Interaction>();
        private Dictionary<string, Interaction> _textMemory = new Dictionary<string, Interaction>();
        public Interaction LastInteraction
        { 
            get 
            {
                if (_interactions.Count < 1)
                    return new Interaction();
                else
                    return _interactions[_interactions.Count - 1];
            }
        }

        public void AddToMemory(Interaction interaction)
        {
            interaction.timestamp = DateTime.Now;
            _interactions.Add(interaction);
            _textMemory[interaction.displayText] = interaction;
        }

        public Interaction Remember(string text)
        {
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
                return Questions.Where<string>(s => Remember(s) != null).ToList<string>();
            }
        }
    }
}
