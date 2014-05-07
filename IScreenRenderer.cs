using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateOfMindTest
{
    public interface IScreenRenderer
    {
        void Write(string text);
        void Clear();
        void DisplayChoices(string choice1, string choice2);
        void DisplayChoices(string choice1, string choice2, string choice3, string choice4);
    }

    public class ConsoleRenderer : IScreenRenderer
    {
        public void Write(string text)
        {
            Console.WriteLine(text);
        }

        public void Clear()
        {
            Console.Clear();
        }

        public void DisplayChoices(string choice1, string choice2)
        {
            throw new NotImplementedException();
        }

        public void DisplayChoices(string choice1, string choice2, string choice3, string choice4)
        {
            throw new NotImplementedException();
        }
    }

}
