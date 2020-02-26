using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp4
{
    class VMWriter
    {
        public Action<string> Output { get; }

        public VMWriter(Action<string> output)
        {
            Output = output;
        }
        public void writePush(string segment, int index)
        {
            Output("push" + " " + segment +  " " +index );
        }
        public void writePop(string segment, int index)
        {
            Output("pop" + " " + segment + " " + index);
        }
        public void WriteArithmetic(string command)
        {
            Output(command);
        }
        public void WriteLabel(string label)
        {
            Output("label " + label);
        }
        public void WriteGoto(string label)
        {
            Output("goto " + label);
        }
        public void WriteIf(string label)
        {
            Output("if-goto " + label);
        }
        public void writeCall(string name, int nArgs)
        {
            Output("call " + name + " " + nArgs);
        }
        public void writeFunction(string name, int nLocals)
        {
            Output("function " + name + " " + nLocals);
        }
        public void writeReturn()
        {
            Output("return");
        }
    }
}
