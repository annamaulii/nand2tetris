
using System.Collections.Generic;


namespace Assembler
{
    public class Code
    {
        // Dictionarys der verschiedenen Parts

        public static Dictionary<string, int> dictDest = new Dictionary<string, int>()
            {
                { "", 0},
                {"M", 1},
                {"D", 2},
                {"MD", 3},
                {"A", 4},
                {"AM", 5},
                {"AD", 6},
                {"AMD", 7}
            };
        public static Dictionary<string, int> dictJump = new Dictionary<string, int>()
            {
                { "", 0},
                {"JGT", 1},
                {"JEQ", 2},
                {"JGE", 3},
                {"JLT", 4},
                {"JNE", 5},
                {"JLE", 6},
                {"JMP", 7}
            };

        public static Dictionary<string, int> dictComp = new Dictionary<string, int>()
            {
                { "", 0},
                {"0", 42 },
                {"1", 63 },
                {"-1", 58 },
                {"D", 12 },
                {"A",48 },
                {"!D", 13 },
                {"!A",49 },
                {"-D", 15 },
                {"-A", 51 },
                {"D+1", 31 },
                {"A+1", 55 },
                {"D-1", 14 },
                {"A-1", 50 },
                {"D+A", 2 },
            {"A+D",2 },
                {"D-A", 19 },
                {"A-D", 7 },
                {"D&A", 0 },
                {"D|A", 21 },
                {"A|D", 21 }
            };

        
        // Dest-Part as dem Dest-Dictionary rausholen (int Zahl)
        public int Dest(string dest)
        {
            return dictDest[dest];
        }
        // Jump-Part as dem Jump-Dictionary rausholen (int Zahl
        public int Jump(string jump)
        {
            return dictJump[jump];
        }
        // Comp-Part as dem Comp-Dictionary rausholen (int Zahl)
        // a=0 oder a=1
        public int Comp(string comp)
        {
            if (comp.Contains("M"))
                return dictComp[comp.Replace("M", "A")] + 64;

            else
                return dictComp[comp];
            
        }
    }
}