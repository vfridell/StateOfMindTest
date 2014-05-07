using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StateOfMindTest
{
    public class Interaction
    {
        public DateTime timestamp;
        public int resultValue = -1;
        public string displayText = "";
        public bool success = true;
        public string playerName = "";

        public enum Answer { Yes = 1, No = 2, Maybe = 3 };
        public Answer playerAnswer
        {
            get
            {
                switch (resultValue)
                {
                    case 1:
                        return Answer.Yes;
                    case 2:
                        return Answer.No;
                    default:
                        return Answer.Maybe;
                }
            }
        }

    }
}
