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

        public Screen(string text, int millisecondTimeout = 5000)
        {
            _msTimeout = millisecondTimeout;
            _displayText = text;
        }

        public virtual Interaction Render()
        {
            Interaction result = new Interaction();
            Console.Clear();
            Console.WriteLine(_displayText);
            result.displayText = _displayText;
            Thread.Sleep(_msTimeout);
            return result;
        }
    }

    public class InputSingleValue : Screen
    {
        public InputSingleValue(string text, int millisecondTimeout = 30000)
            : base(text, millisecondTimeout)
        {
        }

        public override Interaction Render()
        {
            Interaction result = new Interaction();
            Console.Clear();
            Console.WriteLine(_displayText);
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

    public class AnswerIdentifyingQuestion : Screen, IDisposable
    {
        private IEnumerator<string> _questionEnumerator;

        public AnswerIdentifyingQuestion(int millisecondTimeout = 30000)
            : base("", millisecondTimeout)
        {
            _questionEnumerator = Memory.GetInstance().QuestionsWithAnswers.GetEnumerator();
        }

        public override Interaction Render()
        {
            Interaction result = new Interaction() { resultValue = -1 };
            Console.Clear();

            if (Memory.GetInstance().QuestionsWithAnswers.Count == 0)
            {
                _displayText = "Uh, never mind";
                result.displayText = _displayText;
                Console.WriteLine(_displayText);
                Thread.Sleep(5000);
                return result;
            }

            if (!_questionEnumerator.MoveNext())
            {
                _displayText = "I've asked all my questions.";
                result.displayText = _displayText;
                Console.WriteLine(_displayText);
                Thread.Sleep(5000);
                return result;
            }
            else
            {
                result.displayText = _displayText;
                _displayText = _questionEnumerator.Current;
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

        public void Dispose()
        {
            _questionEnumerator.Dispose();
        }
    }

    public class Remembrance : Screen
    {
        private string[] _memoryKeys;

        public Remembrance(string textTemplate, int millisecondTimeout = 5000, params string[] memoryKeys)
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
            result.displayText = _displayText;
            Thread.Sleep(_msTimeout);
            return result;
        }
    }

}
