using System;
using System.IO;
using System.Linq;

namespace VirtualMachine1
{
    enum CommandType
    {
        C_ARITHMETIC, C_PUSH, C_POP, C_LABEL, C_GOTO, C_IF, C_FUNCTION, C_RETURN, C_CALL
    }
    class Parser
    {
        string[] zeilen;
        int position = 0;
        public Parser(string filePath)
        {
            zeilen = File.ReadAllLines(filePath);
            zeilen = zeilen.Select(l => l.Split("//").First()).ToArray();
            zeilen = zeilen.Where(l => !l.Contains("//")).ToArray();
            zeilen = zeilen.Where(l => !l.Equals("")).ToArray();
        }
        public bool hasMoreCommands()
        {
            return zeilen.Length != position;
        }
        public void advance()
        {
            position = position + 1;
        }
        public CommandType commandType()
        {
            switch (arg1())
            {
                case "call": return CommandType.C_CALL;
                case "push": return CommandType.C_PUSH;
                case "pop": return CommandType.C_POP;
                case "label": return CommandType.C_LABEL;
                case "goto": return CommandType.C_GOTO;
                case "function": return CommandType.C_FUNCTION;
                case "return": return CommandType.C_RETURN;
                case "if-goto": return CommandType.C_IF;
                default: return CommandType.C_ARITHMETIC;
            }
        }
        public string arg1()
        {
            return zeilen[position].ToString().Split(" ")[0];
        }
        public string arg2()
        {
            return zeilen[position].ToString().Split(" ")[1];
        }
        public int arg3()
        {
            return Convert.ToInt32(zeilen[position].ToString().Split(" ")[2]);
        }
    }
}
