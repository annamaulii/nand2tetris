using System;
using System.Collections.Generic;
using System.Linq;

namespace Assembler
{
    class SymbolTable
    {
        static int address;
        static string symbol;

        public static void addEntry()
        {

        }

        public bool contains(string symbol)
        {
            return true;
        }

        public static string GetAddress(Dictionary<string, int> addresse)
        {
            var address = addresse.Skip(1).First();
            var value = int.Parse(address.ToString());
            var binary = Convert.ToString(value, 2);
            return binary.PadLeft(16, '0');
        }

    }
}
