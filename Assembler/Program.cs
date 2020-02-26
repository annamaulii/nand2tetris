using Assembler;
using System;
using System.Collections.Generic;
using System.IO;

namespace ConsoleApp1
{
    class Program
    {
        const string name = @"Pong\Pong";
        static string hackPath = @"C:\Users\maula\Desktop\nand2tetris\nand2tetris\projects\11\" + name + ".hack";
        static string asmPath = @"C:\Users\maula\Desktop\nand2tetris\nand2tetris\projects\11\" + name + ".asm";
        public static void Main(string[] args)
        {
            // Objekte erstellt für Parser und Code
            var p = new Parser(asmPath);
            var c = new Code();
            int zeilennummer = 0;

            // Zeilennummer (Markierungen werden nicht mitgezählt)
            foreach (var zeile in p.zeilen)
            {
                if (!zeile.StartsWith("("))
                {
                    zeilennummer += 1;
                }
                else
                {
                    symbolTable.Add(zeile.Replace("(", "").Replace(")", ""), zeilennummer);
                }
            }
            int ramaddress = 16;
            foreach (var zeile in p.zeilen)
            {
                if (zeile.StartsWith("@") && !int.TryParse(zeile.Substring(1), out int abcd) && !symbolTable.ContainsKey(zeile.Substring(1)))
                {
                        symbolTable.Add(zeile.Replace("@", ""), ramaddress);
                        ramaddress += 1;
                }
            }
            // CommandType A oder CommandType B
            do
            {
                if (p.commandType().Equals(CommandType.A))
                {
                    string text = p.symbol().Substring(1);
                    if (!int.TryParse(text, out int zahl))
                    {
                        zahl = symbolTable[text];
                    }
                    Output(Convert.ToString(zahl, 2).PadLeft(16, '0'));
                }
                else if (p.commandType().Equals(CommandType.C))
                {
                    var p1 = Convert.ToString(c.Comp(p.comp()), 2).PadLeft(7, '0');
                    var p3 = Convert.ToString(c.Jump(p.jump()), 2).PadLeft(3, '0');
                    var p2 = Convert.ToString(c.Dest(p.dest()), 2).PadLeft(3, '0');
                    Output("111" + p1 + p2 + p3);
                }
                p.advance();
            } while (p.hasMoreCommands());
            File.WriteAllLines(hackPath, Liste);
            Console.WriteLine("Fertig");
            Console.ReadKey();
        }

        // Klasse für Ausgabe auf Konsole
        private static void Output(string text)
        {
            //Console.WriteLine(text);
            //File.AppendAllText(hackPath, text + "\n");
            Liste.Add(text);
        }

        // Symbol-Tabelle Dictionary
        public static Dictionary<string, int> symbolTable = new Dictionary<string, int>()
        {
            {"R0", 0 },
            {"R1", 1 },
            {"R2", 2 },
            {"R3", 3 },
            {"R4", 4 },
            {"R5", 5 },
            {"R6", 6 },
            {"R7", 7 },
            {"R8", 8 },
            {"R9", 9 },
            {"R10", 10 },
            {"R11", 11 },
            {"R12", 12 },
            {"R13", 13 },
            {"R14", 14 },
            {"R15", 15 },

            {"SP", 0 },
            {"LCL", 1 },
            {"Arg", 2 },
            {"THIS", 3 },
            {"THAT", 4 },
            {"SCREEN", 16384 },
            {"KBD", 24576 }
        };
        private static List<string> Liste = new List<string>();
        
    }
}
