using System;
using System.Collections.Generic;
using System.IO;

namespace ConsoleApp4
{
    class Program
    {
        static void Main(string[] args)
        {
            var dateien = Directory.EnumerateFiles(args[0], "*.jack");

            foreach (var datei in dateien)
            {
                new CompilationEngine(datei, Output);

            var o = Path.Combine(args[0], Path.GetFileNameWithoutExtension(datei) + ".vm");
            File.WriteAllLines(o, Liste);
                Liste.Clear();

            }
            Console.ReadKey();
        }
        static void Output(string text)
        {
            Console.WriteLine(text);
            Liste.Add(text);
        }
        static List<string> Liste = new List<string>();
    }
}
