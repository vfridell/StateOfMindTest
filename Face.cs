using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StateOfMindTest
{
    public class Face
    {
        IScreenRenderer _renderer;
        IGetInput _input;
        public static readonly string AllQuestionsAsked = "I've asked all my questions.";
        public static readonly string NoOneKnown = "Uh, never mind";

        public Face(IScreenRenderer renderer, IGetInput input)
        {
            _renderer = renderer;
            _input = input;
        }

        public Interaction Talk(string text, int millisecondTimeout = 5000)
        {
            Interaction result = new Interaction();
            _renderer.Clear();
            _renderer.Write(text);
            result.displayText = text;
            Thread.Sleep(millisecondTimeout);
            return result;
        }

        public Interaction RememberSingleValue(string text, bool longTerm = false, int millisecondTimeout = 5000)
        {
            Interaction result = new Interaction();
            _renderer.Clear();
            _renderer.Write(text);
            result.displayText = text;
            Task task = Task.Run(() =>
            {
                int intResult;
                if (_input.TryGetInteger(out intResult))
                {
                    result.resultValue = intResult;
                }
            });
            task.Wait(millisecondTimeout);
            
            if(result.resultValue != -1)
                Memory.GetInstance().AddToMemory(result, longTerm);
            
            return result;
        }

        public Interaction GetSingleValue(string text, int millisecondTimeout = 5000)
        {
            Interaction result = new Interaction();
            _renderer.Clear();
            _renderer.Write(text);
            result.displayText = text;
            Task task = Task.Run(() =>
            {
                int intResult;
                if (_input.TryGetInteger(out intResult))
                {
                    result.resultValue = intResult;
                }
            });
            task.Wait(millisecondTimeout);

            return result;
        }

        public Interaction YesNo(string text, bool longTerm = false, int millisecondTimeout = 10000)
        {
            Interaction result = new Interaction();
            _renderer.Clear();
            _renderer.Write(text);
            result.displayText = text;
            Task task = Task.Run(() =>
            {
                int intResult;
                if (_input.TryGetInteger(out intResult))
                {
                    result.resultValue = intResult;
                }
            });
            task.Wait(millisecondTimeout);

            if (result.resultValue != -1)
                Memory.GetInstance().AddToMemory(result, longTerm);

            return result;
        }

        public Interaction Remembrance(string textTemplate, bool longTerm = true, int millisecondTimeout = 5000, params string[] memoryKeys)
        {
            List<object> rememberedValues = new List<object>();
            foreach (string s in memoryKeys)
            {
                Interaction i = Memory.GetInstance().Remember(s, longTerm);
                if(null != i)
                    rememberedValues.Add(i.resultValue);
            }

            Interaction result = new Interaction();
            result.displayText = string.Format(textTemplate, rememberedValues.ToArray());
            _renderer.Clear();
            _renderer.Write(result.displayText);
            Thread.Sleep(millisecondTimeout);
            return result;
        }
    }
}
