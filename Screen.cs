using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StateOfMindTest
{
    public class Screen
    {
        protected int _msTimeout;
        protected string _displayText;

        public Screen(string text, int millisecondTimeout)
        {
            _msTimeout = millisecondTimeout;
            _displayText = text;
        }

        public virtual Interaction Render()
        {
            Interaction result = new Interaction();
            Console.Clear();
            Console.WriteLine(_displayText);
            Console.WriteLine(Memory.GetInstance().LastInteraction.resultValue.ToString());
            result.displayText = _displayText;
            Thread.Sleep(_msTimeout);
            return result;
        }
    }

    public class InputSingleValue : Screen
    {
        public InputSingleValue(string text, int millisecondTimeout)
            : base(text, millisecondTimeout)
        {
        }

        public override Interaction Render()
        {
            Interaction result = new Interaction();
            Console.Clear();
            Console.WriteLine(_displayText);
            Console.WriteLine(Memory.GetInstance().LastInteraction.resultValue.ToString());
            result.displayText = _displayText;
            Task task = Task.Run(() =>
            {
                string val = Console.ReadLine();
                int intResult;
                if (int.TryParse(val, out intResult))
                    result.resultValue = intResult;
            });
            task.Wait(_msTimeout);

            return result;
        }
    }

    public class Remembrance : Screen
    {
        private string[] _memoryKeys;

        public Remembrance(string textTemplate, int millisecondTimeout, params string[] memoryKeys)
            : base(textTemplate, millisecondTimeout)
        {
            _memoryKeys = memoryKeys;
        }

        public override Interaction Render()
        {
            List<object> rememberedValues = new List<object>();
            foreach (string s in _memoryKeys)
            {
                rememberedValues.Add(Memory.GetInstance().Remember(s).resultValue);
            }

            Interaction result = new Interaction();
            Console.Clear();
            Console.WriteLine(string.Format(_displayText, rememberedValues.ToArray()));
            Console.WriteLine(Memory.GetInstance().LastInteraction.resultValue.ToString());
            result.displayText = _displayText;
            Thread.Sleep(_msTimeout);
            return result;
        }
    }

}
