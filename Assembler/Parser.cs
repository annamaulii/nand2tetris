
using System.IO;
using System.Linq;


namespace ConsoleApp1
{
    enum CommandType
    {
        A, C, L
    }
    class Parser
    {
        public string[] zeilen;
        int position = 0;

        // Datei einlesen und Leerzeilen und Kommentare raus
        public Parser(string filePath)
        {
            zeilen = File.ReadAllLines(filePath);
            zeilen = zeilen.Select(l => l.Replace(" ", "")).ToArray();
            zeilen = zeilen.Select(l => l.Split("//").First()).ToArray();
            zeilen = zeilen.Where(l => !l.Contains("//")).ToArray();
            zeilen = zeilen.Where(l => !l.Equals("")).ToArray();
        }

        // Hat die Datei noch mehr Zeilen/Befehle oder ist es zu Ende?
        public bool hasMoreCommands()
        {
            return zeilen.Length != position;
        }

        // Welcher CommandType (A- oder C-Instruction)
        public CommandType commandType()
        {
            if (symbol().StartsWith("@"))
            {
                return CommandType.A;
            }

            if (symbol().StartsWith("("))
            {
                return CommandType.L;
            }
            else
            {
                return CommandType.C;
            }
        }

        // Zeile holen 
        public string symbol()
        {
            return zeilen[position];
        }

        // Eine Zeile weiter
        public void advance()
        {
            position = position + 1;
        }

        // Destination-Part der 16-Bit Zahl
        public string dest()
        {
            if (symbol().Contains("="))
            {
                return new string(symbol().TakeWhile(istgleich => istgleich != '=').ToArray());
            }
            else
            {
                return "";
            }
        }

        // Jump-Part der 16-Bit Zahl
        public string jump()
        {
            if (symbol().Contains(";"))
            {
                return symbol().Split(';').Last();
            }
            else
            {
                return "";
            }
        }

        // Comp-Part der 16-Bit Zahl
        public string comp()
        {
            if (symbol().Contains("="))
            {
                return symbol().Split('=').Last();
            }
            else if (symbol().Contains(";"))
            {
                return symbol().Split(';').First();
            }
            else
            {
                return "";
            }
        }
    }
}
