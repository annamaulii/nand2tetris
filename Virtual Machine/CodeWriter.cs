using System;

namespace VirtualMachine1
{
    class CodeWriter
    {
        private static int count;
        private static int ret;
        private string dateiname;
        private string currentFunctionName;

        public void setFileName(string fileName)
        {
            dateiname = fileName;
        }
        public string writeArithmetic(string command)
        {
            switch (command)
            {
                case "add":
                    return "@SP \n" +
                            "M=M-1\n" +
                            "A=M \n" +
                            "D=M \n" +
                            "A=A-1 \n" +
                            "M=D+M \n";
                case "sub":
                    return "@SP \n" +
                            "M=M-1\n" +
                            "A=M \n" +
                            "D=M \n" +
                            "A=A-1 \n" +
                            "M=M-D \n";

                case "neg":
                    return "@SP \n" +
                            "A=M-1\n" +
                            "D=M \n" +
                            "M=-M \n";
                case "eq":
                    count += 1;
                    return "@SP \n" +
                            "M=M-1\n" +
                            "A=M \n" +
                            "D=M \n" +
                            "A=A-1 \n" +
                            "D=D-M \n" +
                            "@eq" + count + " \n" +
                            "D;JEQ\n" +
                            "@SP \n" +
                            "A=M-1 \n" +
                            "M=0 \n" +
                            "@endEq" + count + " \n" +
                            "0;JMP \n" +
                            "(eq" + count + ")\n" +
                            "@SP \n" +
                            "A=M-1 \n" +
                            "M=-1 \n" +
                            "(endEq" + count + ")\n";
                case "gt":
                    count += 1;
                    return "@SP \n" +
                            "M=M-1\n" +
                            "A=M \n" +
                            "D=M \n" +
                            "A=A-1 \n" +
                            "D=M-D \n" +
                            "@gt" + count + " \n" +
                            "D;JGT \n" +
                            "@SP \n" +
                            "A=M-1 \n" +
                            "M=0 \n" +
                            "@endGt" + count + " \n" +
                            "0;JMP \n" +
                            "(gt" + count + ")\n" +
                            "@SP \n" +
                            "A=M-1 \n" +
                            "M=-1 \n" +
                            "(endGt" + count + ")\n";
                case "lt":
                    count += 1;
                    return "@SP \n" +
                            "M=M-1\n" +
                            "A=M \n" +
                            "D=M \n" +
                            "A=A-1 \n" +
                            "D=M-D \n" +
                            "@lt" + count + " \n" +
                            "D;JLT\n" +
                            "@SP \n" +
                            "A=M-1 \n" +
                            "M=0 \n" +
                            "@endLt" + count + " \n" +
                            "0;JMP \n" +
                            "(lt" + count + ")\n" +
                            "@SP \n" +
                            "A=M-1 \n" +
                            "M=-1 \n" +
                            "(endLt" + count + ")\n";
                case "and":
                    return "@SP \n" +
                            "M=M-1\n" +
                            "A=M \n" +
                            "D=M \n" +
                            "A=A-1 \n" +
                            "M=D&M \n";
                case "or":
                    return "@SP \n" +
                            "M=M-1\n" +
                            "A=M \n" +
                            "D=M \n" +
                            "A=A-1 \n" +
                            "M=M|D \n";
                case "not":
                    return "@SP \n" +
                            "A=M-1 \n" +
                            "D=M \n" +
                            "M=!M\n";
                default: return "@0";

            }
        }
        public string WritePushPop(string command, string segment, int index)
        {
            if (command.Equals("push"))
            {
                switch (segment)
                {
                    case "argument":
                        return "@ARG\n" +
                             "D=M \n" +
                             "@" + index + "\n" +
                             "A=D+A \n" +
                             "D=M \n" +
                             "@SP \n" +
                             "A=M\n" +
                             "M=D\n" +
                             "@SP\n" +
                             "M=M+1\n";
                    case "local":
                        return "@LCL\n" +
                            "D=M \n" +
                            "@" + index + "\n" +
                            "A=D+A \n" +
                            "D=M \n" +
                            "@SP \n" +
                            "A=M\n" +
                            "M=D\n" +
                            "@SP\n" +
                            "M=M+1\n";
                    case "static":
                        return "@" + dateiname + "." + index + "\n" +
                            "D=M \n" +
                            "@SP \n" +
                            "A=M \n" +
                            "M=D \n" +
                            "@SP \n" +
                            "M=M+1 \n";
                    case "constant":
                        return "@" + index + "\n" +
                               "D=A\n" +
                               "@SP\n" +
                               "A=M\n" +
                               "M=D\n" +
                               "@SP\n" +
                               "M=M+1\n";
                    case "this":
                        return "@THIS\n" +
                            "D=M \n" +
                            "@" + index + "\n" +
                            "A=D+A \n" +
                            "D=M \n" +
                            "@SP \n" +
                            "A=M\n" +
                            "M=D\n" +
                            "@SP\n" +
                            "M=M+1\n";
                    case "that":
                        return "@THAT\n" +
                            "D=M \n" +
                            "@" + index + "\n" +
                            "A=D+A \n" +
                            "D=M \n" +
                            "@SP \n" +
                            "A=M\n" +
                            "M=D\n" +
                            "@SP\n" +
                            "M=M+1\n";
                    case "pointer":
                        return "@3\n" +
                            "D=A \n" +
                            "@" + index + "\n" +
                            "A=D+A \n" +
                            "D=M \n" +
                            "@SP \n" +
                            "A=M\n" +
                            "M=D\n" +
                            "@SP\n" +
                            "M=M+1\n";
                    case "temp":
                        return "@5\n" +
                            "D=A \n" +
                            "@" + index + "\n" +
                            "A=D+A \n" +
                            "D=M \n" +
                            "@SP \n" +
                            "A=M\n" +
                            "M=D\n" +
                            "@SP\n" +
                            "M=M+1\n";
                    default: return "@0";
                }
            }
            else if (command.Equals("pop"))
            {
                switch (segment)
                {
                    case "argument":
                        return "@ARG\n" +
                               "D=M\n" +
                               "@" + index + "\n" +
                               "D=A+D\n" +
                               "@R13\n" +
                               "M=D\n " +
                               "@SP\n" +
                               "M=M-1\n" +
                               "A=M\n" +
                               "D=M\n" +
                               "@R13\n" +
                               "A=M\n" +
                               "M=D\n";

                    case "local":
                        return "@LCL\n" +
                               "D=M\n" +
                               "@" + index + "\n" +
                               "D=A+D\n" +
                               "@R13\n" +
                               "M=D\n " +
                               "@SP\n" +
                               "M=M-1\n" +
                               "A=M\n" +
                               "D=M\n" +
                               "@R13\n" +
                               "A=M\n" +
                               "M=D\n";

                    case "static":
                        return
                            "@SP\n" +
                               "M=M-1\n" +
                               "A=M\n" +
                               "D=M\n" +
                               "@" + dateiname + "." + index + "\n" +
                               "M=D \n";
                    case "this":
                        return "@THIS\n" +
                               "D=M\n" +
                               "@" + index + "\n" +
                               "D=A+D\n" +
                               "@R13\n" +
                               "M=D\n " +
                               "@SP\n" +
                               "M=M-1\n" +
                               "A=M\n" +
                               "D=M\n" +
                               "@R13\n" +
                               "A=M\n" +
                               "M=D\n";
                    case "that":
                        return "@THAT\n" +
                               "D=M\n" +
                               "@" + index + "\n" +
                               "D=A+D\n" +
                               "@R13\n" +
                               "M=D\n " +
                               "@SP\n" +
                               "M=M-1\n" +
                               "A=M\n" +
                               "D=M\n" +
                               "@R13\n" +
                               "A=M\n" +
                               "M=D\n";
                    case "pointer":
                        return "@3\n" +
                               "D=A\n" +
                               "@" + index + "\n" +
                               "D=A+D\n" +
                               "@R13\n" +
                               "M=D\n " +
                               "@SP\n" +
                               "M=M-1\n" +
                               "A=M\n" +
                               "D=M\n" +
                               "@R13\n" +
                               "A=M\n" +
                               "M=D\n";
                    case "temp":
                        return "@5\n" +
                               "D=A\n" +
                               "@" + index + "\n" +
                               "D=A+D\n" +
                               "@R13\n" +
                               "M=D\n " +
                               "@SP\n" +
                               "M=M-1\n" +
                               "A=M\n" +
                               "D=M\n" +
                               "@R13\n" +
                               "A=M\n" +
                               "M=D\n";
                    default: return "@0";
                }

            }
            else
            {
                throw new NotImplementedException();
            }
        }
        public string writeLabel(string label)
        {
            return "(" + currentFunctionName + "$" + label + ") \n";
        }
        public string writeGoto(string label)
        {
            return "@" + currentFunctionName + "$" + label + "\n" + "0;JMP\n";
        }
        public string writeIf(string label)
        {
            return "@SP\n" +
                    "M=M-1\n" +
                    "A=M\n" +
                    "D=M\n" +
                    "@" + currentFunctionName + "$" +label + "\n" +
                    "D;JNE\n";  // wenn nicht 0
        }
        public string writeCall(string functionName, int numArgs)
        {
            ret += 1;
            return "@return-address" + ret + "\n" +
                    "D=A\n" +
                    "@SP\n" +
                    "A=M\n" +
                    "M=D\n" +
                    "@SP\n" +
                    "M=M+1\n" +             // push return-address

                    "@LCL\n" +
                    "D=M \n" +
                    "@SP \n" +
                    "A=M\n" +
                    "M=D\n" +
                    "@SP\n" +
                    "M=M+1\n" +             // push LCL

                    "@ARG\n" +
                    "D=M \n" +
                    "@SP \n" +
                    "A=M\n" +
                    "M=D\n" +
                    "@SP\n" +
                    "M=M+1\n" +             // push ARG

                    "@THIS\n" +
                    "D=M \n" +
                    "@SP \n" +
                    "A=M\n" +
                    "M=D\n" +
                    "@SP\n" +
                    "M=M+1\n" +             // push THIS

                    "@THAT\n" +
                    "D=M \n" +
                    "@SP \n" +
                    "A=M\n" +
                    "M=D\n" +
                    "@SP\n" +
                    "M=M+1\n" +             // push THAT

                    "@SP\n" +
                    "D=M\n" +
                    "@" + numArgs + "\n" +
                    "D=D-A\n" +
                    "@5\n" +
                    "D=D-A\n" +
                    "@ARG\n" +
                    "M=D\n" +               // ARG = SP-n-5

                    "@SP\n" +
                    "D=M\n" +
                    "@LCL\n" +
                    "M=D\n" +               // LCL = SP


                    "@" + functionName + "\n" +
                    "0;JMP\n"+

                    "(return-address" + ret + ")\n";      // (return-address)
        }
        public string writeFunction(string functionName, int numLocals)
        {
            string write = "";
            for (int i = 0; i < numLocals; i++)
            {
                write += WritePushPop("push", "constant", 0);
            }

            currentFunctionName = functionName;
            return  "(" + currentFunctionName + ")\n" +
                    write ;
        }
        public string writeReturn()
        {
            return "@LCL\n" +
                    "D=M\n" +
                    "@R14\n" +
                    "M=D\n" +       // FRAME = LCL

                    "@R14\n" +
                    "D=M\n" +
                    "@5 \n" +
                    "A=D-A\n" +
                    "D=M\n" +
                    "@R15\n" +
                    "M=D\n" +       // RET = *(FRAME - 5)
                                    // R14 = FRAME
                                    // R15 = RET

                    "@SP\n" +
                    "M=M-1\n" +
                    "A=M\n" +
                    "D=M\n" +
                    "@ARG\n" +
                    "A=M\n" +
                    "M=D\n" +        // *ARG = pop()

                    "@ARG\n" +
                    "D=M+1\n" +
                    "@SP\n" +
                    "M=D\n" +       // SP = ARG + 1

                    "@R14\n" +
                    "D=M\n" +
                    "@4 \n" +
                    "A=D-A\n" +
                    "A=M\n" +
                    "D=A\n" +
                    "@LCL\n" +
                    "M=D\n" +       // LCL = *(FRAME - 4)

                    "@R14\n" +
                    "D=M\n" +
                    "@3 \n" +
                    "A=D-A\n" +
                    "A=M\n" +
                    "D=A\n" +
                    "@ARG\n" +
                    "M=D\n" +       // ARG = *(FRAME - 3)

                    "@R14\n" +
                    "D=M\n" +
                    "@2 \n" +
                    "A=D-A\n" +
                    "A=M\n" +
                    "D=A\n" +
                    "@THIS\n" +
                    "M=D\n" +       // THIS = *(FRAME - 2)

                    "@R14\n" +
                    "D=M\n" +
                    "@1 \n" +
                    "A=D-A\n" +
                    "A=M\n" +
                    "D=A\n" +
                    "@THAT\n" +
                    "M=D\n" +      //THAT = *(FRAME - 1)



                    "@R15\n" +
                    "A=M\n" +
                    "0;JMP\n";      // goto RET
        }
    }
}