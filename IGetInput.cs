using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateOfMindTest
{
    public interface IGetInput
    {
        int GetInteger();
        bool TryGetInteger(out int value);
    }

    public class ConsoleInput : IGetInput
    {
        public int GetInteger()
        {
            string val = Console.ReadLine();
            int intResult;
            if (int.TryParse(val, out intResult))
                return intResult;

            throw new Exception("Could not parse integer from console input");
        }

        public bool TryGetInteger(out int value)
        {
            string val = Console.ReadLine();
            return int.TryParse(val, out value);
        }
    }
}
