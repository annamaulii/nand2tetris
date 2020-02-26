using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApp4
{
    class CompilationEngine
    {
        private JackTokenizer currentToken;
        private static readonly Keyword[] FunctionKeywords = new Keyword[] { Keyword.CONSTRUCTOR, Keyword.FUNCTION, Keyword.METHOD };
        private static readonly Keyword[] classVarDecType = new Keyword[] { Keyword.INT, Keyword.CHAR, Keyword.BOOLEAN };
        private static readonly Keyword[] Statements = new Keyword[] { Keyword.LET, Keyword.IF, Keyword.ELSE, Keyword.WHILE, Keyword.DO, Keyword.RETURN };
        private static readonly Keyword[] KeywordConstant = new Keyword[] { Keyword.TRUE, Keyword.FALSE, Keyword.NULL, Keyword.THIS };
        private static readonly char[] op = new char[] { '+', '-', '*', '/', '&', '|', '<', '>', '=' };
        private static readonly char[] notOrMinus = new char[] { '-', '~' };
        public Dictionary<string, variable> dictVariables = new Dictionary<string, variable>();
        public int countStatic = 0;
        private int countField = 0;
        public int countLocals = 0;
        public int countArg = 0;
        private string className;
        private int parameter;
        private int zahlIf = 0;
        private int zahlWhile = 0;

        VMWriter VMWriter { get; }

        public CompilationEngine(string file, Action<string> output)
        {
            currentToken = new JackTokenizer(file);
            VMWriter = new VMWriter(output);
            CompileClass();
        }
        public void CompileClass()
        {
            exceptKeyword(Keyword.CLASS, "class");
            currentToken.advance();
            className = currentToken.Identifier();
            currentToken.advance();
            exceptSymbol('{', "class");
            currentToken.advance();
            while (currentToken.tokenType() == TokenType.KEYWORD &&
                (currentToken.keyword() == Keyword.STATIC || currentToken.keyword() == Keyword.FIELD))
            {
                CompileClassVarDec();
            }
            while (currentToken.tokenType() == TokenType.KEYWORD && FunctionKeywords.Contains(currentToken.keyword()))
            {
                CompileSubroutine();
            }
            exceptSymbol('}', "class");
            currentToken.advance();
        }
        public void CompileClassVarDec()
        {
            var kind = currentToken.Identifier();
            if (kind == "field")
            {
                kind = "this";
            }
            currentToken.advance();
            var varType = currentToken.Identifier();
            currentToken.advance();
            var index = (kind == "this") ? countField++ : countStatic++;
            dictVariables.Add(currentToken.Identifier(), new variable { index = index, kind = kind, type = varType });
            currentToken.advance();
            while (currentToken.tokenType() == TokenType.SYMBOL && currentToken.Symbol() == ',')
            {
                currentToken.advance();
                index = (kind == "this") ? countField++ : countStatic++;
                dictVariables.Add(currentToken.Identifier(), new variable { index = index, kind = kind, type = varType });
                currentToken.advance();
            }
            exceptSymbol(';', "class Variablen Deklaration");
            currentToken.advance();
        }
        public void CompileSubroutine()
        {
            var isConstructor = currentToken.tokenType() == TokenType.KEYWORD && currentToken.keyword() == Keyword.CONSTRUCTOR;
            var isMethod = currentToken.tokenType() == TokenType.KEYWORD && currentToken.keyword() == Keyword.METHOD;
            currentToken.advance();
            currentToken.advance();
            var name = currentToken.Identifier();
            currentToken.advance();
            exceptSymbol('(', "subroutine");
            currentToken.advance();

            compileParameterList(isMethod);
            exceptSymbol(')', "subroutine");
            currentToken.advance();
            exceptSymbol('{', "subroutineBody");
            currentToken.advance();
            while (currentToken.tokenType() == TokenType.KEYWORD && currentToken.keyword() == Keyword.VAR)
            {
                compileVarDec();
            }
            VMWriter.writeFunction(className + "." + name, countLocals);
            if (isConstructor)
            {
                int arg = dictVariables.Count(dic => dic.Value.kind == "this");
                VMWriter.writePush("constant", arg);
                VMWriter.writeCall("Memory.alloc", 1);
                VMWriter.writePop("pointer", 0);
            }
            if(isMethod)
            {
                VMWriter.writePush("argument", 0);
                VMWriter.writePop("pointer", 0);
            }
            while (currentToken.tokenType() == TokenType.KEYWORD && (Statements.Contains(currentToken.keyword())))
            {
                compileStatement();
            }
            exceptSymbol('}', "subroutineBody");
            currentToken.advance();
            countLocals = 0;
            countArg = 0;
            zahlIf = 0;
            zahlWhile = 0;
            dictVariables = dictVariables.Where
                (v => !(v.Value.kind == "local" || v.Value.kind == "argument")).ToDictionary(k => k.Key, k => k.Value);

        }
        public int compileParameterList(bool isMethod)
        {
            if(isMethod)
            {
                countArg++;
            }
            if (currentToken.tokenType() == TokenType.SYMBOL && currentToken.Symbol() == ')')
            {
                // Parameter-Liste ist leer 
            }
            else
            {
                var type = currentToken.Identifier();
                currentToken.advance();
                dictVariables.Add(currentToken.Identifier(), new variable { index = countArg++, kind = "argument", type = type });
                currentToken.advance();
                while (currentToken.tokenType() == TokenType.SYMBOL && currentToken.Symbol() == ',')
                {
                    parameter++;
                    currentToken.advance();
                    type = currentToken.Identifier();
                    currentToken.advance();
                    dictVariables.Add(currentToken.Identifier(), new variable { index = countArg++, kind = "argument", type = type });
                    currentToken.advance();
                }
                exceptSymbol(')', "ParameterList");
            }
            return parameter;
        }
        public void compileVarDec()
        {
            exceptKeyword(Keyword.VAR, "varDec");
            currentToken.advance();
            var varType = currentToken.Identifier();
            currentToken.advance();
            dictVariables.Add(currentToken.Identifier(), new variable { index = countLocals++, kind = "local", type = varType });
            currentToken.advance();
            while (currentToken.tokenType() == TokenType.SYMBOL && currentToken.Symbol() == ',')
            {
                currentToken.advance();
                dictVariables.Add(currentToken.Identifier(), new variable { index = countLocals++, kind = "local", type = varType });
                currentToken.advance();
            }
            exceptSymbol(';', "var Variablen Deklaration");
            currentToken.advance();
        }
        public void compileStatement()
        {
            if (currentToken.keyword() == Keyword.LET)
            {
                compileLet();
            }
            else if (currentToken.keyword() == Keyword.IF)
            {
                compileIf();
            }
            else if (currentToken.keyword() == Keyword.WHILE)
            {
                compileWhile();
            }
            else if (currentToken.keyword() == Keyword.DO)
            {
                compileDo();
            }
            else if (currentToken.keyword() == Keyword.RETURN)
            {
                compileReturn();
            }
        }
        public void compileDo()
        {
            currentToken.advance();
            compileSubroutineCall();
            VMWriter.writePop("temp", 0);
            exceptSymbol(';', "Do-Statement");
            currentToken.advance();
        }
        public void compileLet()
        {
            currentToken.advance();
            var name = currentToken.Identifier();
            currentToken.advance();
            if (currentToken.tokenType() == TokenType.SYMBOL && currentToken.Symbol() == '[')
            {
                currentToken.advance();
                CompileExpression();
                VMWriter.writePush(dictVariables[name].kind, dictVariables[name].index);
                VMWriter.WriteArithmetic("add");
                exceptSymbol(']', "Let-Statement");
                currentToken.advance();
                exceptSymbol('=', "Let-Statement");
                currentToken.advance();
                CompileExpression();
                VMWriter.writePop("temp", 0);
                VMWriter.writePop("pointer", 1);
                VMWriter.writePush("temp", 0);
                VMWriter.writePop("that", 0);
            }
            else
            {
                exceptSymbol('=', "Let-Statement");
                currentToken.advance();
                CompileExpression();
                VMWriter.writePop(dictVariables[name].kind, dictVariables[name].index);
            }
            exceptSymbol(';', "Let-Statement");
            currentToken.advance();
        }
        public void compileWhile()
        {
            int zahlWhile= this.zahlWhile++;
            currentToken.advance();
            exceptSymbol('(', "While-Statement");
            currentToken.advance();
            VMWriter.WriteLabel("WHILE_EXP" + zahlWhile);
            CompileExpression();
            VMWriter.WriteArithmetic("not");
            exceptSymbol(')', "While-Statement");
            currentToken.advance();
            VMWriter.WriteIf("WHILE_END" + zahlWhile);
            exceptSymbol('{', "While-Statement");
            currentToken.advance();
            while (currentToken.tokenType() == TokenType.KEYWORD && (Statements.Contains(currentToken.keyword())))
            {
                compileStatement();
            }
            exceptSymbol('}', "While-Statement");
            VMWriter.WriteGoto("WHILE_EXP" + zahlWhile);
            VMWriter.WriteLabel("WHILE_END" + zahlWhile);
            currentToken.advance();
        }
        public void compileReturn()
        {
            currentToken.advance();
            if (!(currentToken.tokenType() == TokenType.SYMBOL && currentToken.Symbol() == ';'))
            {
                CompileExpression();
            }
            else
            {
                VMWriter.writePush("constant", 0);
            }
            VMWriter.writeReturn();
            exceptSymbol(';', "return-Statement");
            currentToken.advance();
        }
        public void compileIf()
        {
            int zahlIf = this.zahlIf++;
            currentToken.advance();
            exceptSymbol('(', "If-Statement");
            currentToken.advance();
            CompileExpression();
            VMWriter.WriteIf("IF_TRUE" + zahlIf);
            VMWriter.WriteGoto("IF_FALSE" + zahlIf);
            VMWriter.WriteLabel("IF_TRUE" + zahlIf);
            exceptSymbol(')', "If-Statement");
            currentToken.advance();
            exceptSymbol('{', "If-Statement");
            currentToken.advance();
            while (currentToken.tokenType() == TokenType.KEYWORD && (Statements.Contains(currentToken.keyword())))
            {
                compileStatement();
            }
            exceptSymbol('}', "If-Statement");
            currentToken.advance();
            if (currentToken.tokenType() == TokenType.KEYWORD && currentToken.keyword() == Keyword.ELSE)
                VMWriter.WriteGoto("IF_END" + zahlIf);
            VMWriter.WriteLabel("IF_FALSE" + zahlIf);
            if (currentToken.tokenType() == TokenType.KEYWORD && currentToken.keyword() == Keyword.ELSE)
            {
                currentToken.advance();
                exceptSymbol('{', "Else-Statement");
                currentToken.advance();
                while (currentToken.tokenType() == TokenType.KEYWORD && (Statements.Contains(currentToken.keyword())))
                {
                    compileStatement();
                }
                exceptSymbol('}', "Else-Statement");
                currentToken.advance();
                VMWriter.WriteLabel("IF_END" + zahlIf);
            }
        }
        public void CompileExpression()
        {
            CompileTerm();
            while (op.Contains(currentToken.Symbol()))
            {
                var operation = currentToken.Symbol();
                currentToken.advance();
                CompileTerm();
                switch (operation)
                {
                    case '+': VMWriter.WriteArithmetic("add"); break;
                    case '-': VMWriter.WriteArithmetic("sub"); break;
                    case '*': VMWriter.writeCall("Math.multiply", 2); break;
                    case '/': VMWriter.writeCall("Math.divide", 2); break;
                    case '&': VMWriter.WriteArithmetic("and"); break;
                    case '|': VMWriter.WriteArithmetic("or"); break;
                    case '<': VMWriter.WriteArithmetic("lt"); break;
                    case '>': VMWriter.WriteArithmetic("gt"); break;
                    case '=': VMWriter.WriteArithmetic("eq"); break;
                    default: throw new Exception();
                }
            }
        }
        public void CompileTerm()
        {
            if (currentToken.tokenType() == TokenType.INT_CONST)
            {
                int nbr = currentToken.IntVal();
                currentToken.advance();
                VMWriter.writePush("constant", nbr);
            }
            else if (currentToken.tokenType() == TokenType.STRING_CONST)
            {
                string str = currentToken.StringVal();
                currentToken.advance();
                VMWriter.writePush("constant", str.Length);
                VMWriter.writeCall("String.new", 1);
                var ascii = Encoding.ASCII.GetBytes(str);
                for (int i = 0; i < ascii.Length; i++)
                {
                    VMWriter.writePush("constant", ascii[i]);
                    VMWriter.writeCall("String.appendChar", 2);
                }
            }
            else if (currentToken.tokenType() == TokenType.KEYWORD && KeywordConstant.Contains(currentToken.keyword()))
            {
                var keywordConstant = currentToken.keyword();
                currentToken.advance();
                switch (keywordConstant)
                {
                    case Keyword.TRUE: VMWriter.writePush("constant", 0);
                                       VMWriter.WriteArithmetic("not"); break;
                    case Keyword.FALSE:
                    case Keyword.NULL: VMWriter.writePush("constant", 0); break;
                    case Keyword.THIS: VMWriter.writePush("pointer", 0); break;
                    default: throw new Exception();
                }
            }
            else if (currentToken.tokenType() == TokenType.IDENTIFIER)
            {
                if (currentToken.Next.Type == TokenType.SYMBOL && currentToken.Next.Value[0] == '[')
                {
                    var name = currentToken.Identifier();
                    currentToken.advance();
                    currentToken.advance();
                    CompileExpression();
                    VMWriter.writePush(dictVariables[name].kind, dictVariables[name].index);
                    VMWriter.WriteArithmetic("add");
                    VMWriter.writePop("pointer", 1);
                    VMWriter.writePush("that", 0);
                    exceptSymbol(']', "Term Array");
                    currentToken.advance();
                }
                else if (currentToken.Next.Type == TokenType.SYMBOL && currentToken.Next.Value[0] == '(')
                {
                    compileSubroutineCall();
                }
                else if (currentToken.Next.Type == TokenType.SYMBOL && currentToken.Next.Value[0] == '.')
                {
                    compileSubroutineCall();
                }
                else
                {
                    var name = currentToken.Identifier();
                    currentToken.advance();
                    VMWriter.writePush(dictVariables[name].kind, dictVariables[name].index);
                }
            }
            else if (currentToken.tokenType() == TokenType.SYMBOL)
            {
                if (notOrMinus.Contains(currentToken.Symbol()))
                {
                    var unary = currentToken.Symbol();
                    currentToken.advance();
                    CompileExpression();
                    switch (unary)
                    {
                        case '-': VMWriter.WriteArithmetic("neg"); break;
                        case '~': VMWriter.WriteArithmetic("not"); break;
                        default: throw new Exception();
                    }
                }
                else if (currentToken.tokenType() == TokenType.SYMBOL && currentToken.Symbol() == '(')
                {
                    currentToken.advance();
                    CompileExpression();
                    exceptSymbol(')', "Expression");
                    currentToken.advance();
                }
                else
                {
                    throw new Exception("Operation ist weder unär noch '(' sondern " + currentToken.Identifier());
                }
            }
            else
            {
                throw new Exception();
            }
        }
        private void compileSubroutineCall()
        {
            var name = currentToken.Identifier();
            currentToken.advance();
            if (currentToken.tokenType() == TokenType.SYMBOL && currentToken.Symbol() == '(')
            {
                VMWriter.writePush("pointer", 0);
                currentToken.advance();
                int nArgs = CompileExpressionList();
                exceptSymbol(')', "subroutineCall");
                currentToken.advance();
                VMWriter.writeCall(className + "." + name, nArgs+1);
            }
            else if (currentToken.tokenType() == TokenType.SYMBOL && currentToken.Symbol() == '.')
            {
                currentToken.advance();
                var subroutineName = currentToken.Identifier();
                currentToken.advance();
                exceptSymbol('(', "subroutineCall");
                if (dictVariables.ContainsKey(name))
                {
                    VMWriter.writePush(dictVariables[name].kind, dictVariables[name].index);
                }
                    currentToken.advance();
                int nArgs = CompileExpressionList();
                exceptSymbol(')', "subroutineCall");
                currentToken.advance();
                if (dictVariables.ContainsKey(name))
                {
                    VMWriter.writeCall(dictVariables[name].type + "." + subroutineName, nArgs+1);
                }
                else
                {
                    VMWriter.writeCall(name + "." + subroutineName, nArgs);
                }
            }
            else
            {
                throw new Exception();
            }
        }
        public int CompileExpressionList()
        {
            int zahl = 0;
            if (!(currentToken.tokenType() == TokenType.SYMBOL && (currentToken.Symbol() == ')')))
            {
                zahl = 1;
                CompileExpression();
                while (currentToken.tokenType() == TokenType.SYMBOL && (currentToken.Symbol() == ','))
                {
                    zahl++;
                    currentToken.advance();
                    CompileExpression();
                }
            }
            return zahl;
        }
        private void exceptKeyword(Keyword symbol, string context)
        {
            if (!(currentToken.tokenType() == TokenType.KEYWORD && currentToken.keyword() == symbol))
            {
                throw new Exception("Keyword: " + symbol + " für " + context + " fehlt");
            }
        }
        private void exceptSymbol(char symbol, string context)
        {
            if (!(currentToken.tokenType() == TokenType.SYMBOL && currentToken.Symbol() == symbol))
            {
                throw new Exception(symbol + " für " + context + " fehlt stattdessen: " + currentToken.Identifier());
            }
        }
        private void exceptStatement(string symbol, string context)
        {
            if (!(currentToken.tokenType() == TokenType.KEYWORD && Statements.Contains(currentToken.keyword())))
            {
                throw new Exception(symbol + " für " + context + " fehlt.");
            }
        }
    }
}
