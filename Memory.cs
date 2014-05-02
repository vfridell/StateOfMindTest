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
            _interactions.Add(interaction);
            _textMemory[interaction.displayText] = interaction;
        }

        public Interaction Remember(string text)
        {
            return _textMemory[text];
        }
    }
}
