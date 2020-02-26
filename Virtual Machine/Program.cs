using System;
using System.Collections.Generic;
using System.IO;

namespace VirtualMachine1
{
    class Program
    {
        static string vmPath = @"C:\Users\maula\Desktop\nand2tetris\nand2tetris\projects\11\Square";
        
        class MyArrayList
        {
            int[] array;
            private int length;

            public MyArrayList()
            {
                array = new int[1];                             // admin wird erstellt
                length = 0;                                     // variable zur Länge zugewiesen
            }
            public void Add(int v)
            {
                if (length >= array.Length)                     // wenn array länge erreicht wurde, dann ...
                {
                    var array1 = new int[length * 2];           // verdopple Array Länge 
                    for (int i = 0; i < array.Length; i++)      // Daten des alten Arrays in neues 
                    {
                        array1[i] = array[i];
                    }

                    array = array1;                             // Altes Array ist Neues Array
                }
                array[length] = v;                              // Add
                length = length + 1;                            // eins weiter 
            }
            public int At(int index)
            {
                if (index >= 0)                                 // wenn Array nicht leer ist
                {   
                    return array[index];                        // gib Wert an Stelle Index zurück 
                }
                else
                {
                    throw new Exception();                      // sonst Fehler
                }
            }
        }

        class MyLinkedList
        {
            private item first;

            class item                                  // neuer Datentyp (für Kästchen)
            {                                          
                public int i;                           // Zahl
                public item next;                       // Pfeil
                public override string ToString()       // ToString Methode überschrieben
                {
                    return i.ToString();
                }
            }
            public MyLinkedList()
            {
                first = null;                           // erstes Kästchen leer
            }
            public void Add(int v)
            {
                if (first == null)                      // wenn erstes Kästchen leer, neues item anlegen
                {
                    first = new item();
                    first.i = v;
                }
                else
                {
                    item last = first;                  // wenn erstes Kästchen nicht leer, gehe zum letzten
                    while (last.next != null)           // Letzte Stelle suchen (Einkaufliste ganz unten)
                        last = last.next;               // letzter Pfeil
                    last.next = new item();
                    last.next.i = v;                    
                }
            }
            public int At(int index)
            {
                item stelle = first;                    // neues item mit erster Stelle des Kästchens
                for (int i = 0; i < index; i++)         // Suche, bis stelle gefunden wurde
                {
                    stelle = stelle.next;
                }
                return stelle.i;
            }
        }
        static void Main(string[] args)
        {
            var dateien = Directory.EnumerateFiles(vmPath, "*.vm");
            foreach (var datei in dateien)
            {

                var p = new Parser(Path.Combine(vmPath, datei));
                var c = new CodeWriter();
                c.setFileName(Path.GetFileNameWithoutExtension(datei));
                do
                {
                    var text = "";

                    if (p.commandType().Equals(CommandType.C_ARITHMETIC))
                        text = c.writeArithmetic(p.arg1());
                    else if (p.commandType().Equals(CommandType.C_PUSH))
                        text = c.WritePushPop(p.arg1(), p.arg2(), p.arg3());
                    else if (p.commandType().Equals(CommandType.C_POP))
                        text = c.WritePushPop(p.arg1(), p.arg2(), p.arg3());
                    else if (p.commandType().Equals(CommandType.C_LABEL))
                        text = c.writeLabel(p.arg2());
                    else if (p.commandType().Equals(CommandType.C_GOTO))
                        text = c.writeGoto(p.arg2());
                    else if (p.commandType().Equals(CommandType.C_IF))
                        text = c.writeIf(p.arg2());
                    else if (p.commandType().Equals(CommandType.C_FUNCTION))
                        text = c.writeFunction(p.arg2(), p.arg3());
                    else if (p.commandType().Equals(CommandType.C_RETURN))
                        text = c.writeReturn();
                    else if (p.commandType().Equals(CommandType.C_CALL))
                        text = c.writeCall(p.arg2(), p.arg3());

                    p.advance();
                    Output(text);
                } while (p.hasMoreCommands());
            }
            var o = Path.Combine(vmPath, new DirectoryInfo(vmPath).Name + ".asm");
            File.WriteAllLines(o, Liste);
            Console.ReadKey();
        }
        private static void Output(string text)
        {
            Console.WriteLine(text);
            Liste.Add(text);
        }
        private static List<string> Liste = new List<string>()
        {
            "@261",
            "D=A",
            "@SP",
            "M=D",
            "@Sys.init",
            "0;JMP"
        };
    }
}
