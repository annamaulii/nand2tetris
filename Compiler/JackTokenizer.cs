using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ConsoleApp4
{
    enum TokenType
    {
        KEYWORD, SYMBOL, IDENTIFIER, INT_CONST, STRING_CONST
    }
    enum Keyword
    {
        CLASS, METHOD, FUNCTION, CONSTRUCTOR, INT, BOOLEAN, CHAR, VOID, VAR, STATIC,
        FIELD, LET, DO, IF, ELSE, WHILE, RETURN, TRUE, FALSE, NULL, THIS
    }

    class JackTokenizer
    {
        int position = 0;
        
        List<Token> Tokens= new List<Token>();
        public JackTokenizer(string file)
        {
            string token="";
            string text = File.ReadAllText(file);
            for (int i = 0; i < text.Length; i++)
            {
                var ca = text.Substring(i);
                if (text[i] == '/' && text[i + 1] == '/')
                {
                    var pos = text.IndexOf('\n', i);
                    i = pos;
                }
                else if (text[i] == '/' && text[i + 1] == '*')
                {
                    var pos = text.IndexOf("*/", i);
                    i = pos + 1;
                }
                else if (text[i] == '\r' || text[i] == '\n' || text[i] == ' ' || text[i] == '\t')
                {
                    if (token.Length != 0)
                    {
                        Tokens.Add(new Token(token));
                        token = "";
                    }
                }
                else if (Token.symbols.Contains(text[i].ToString()))
                {
                    if (token.Length != 0)
                    {
                        Tokens.Add(new Token(token));
                        token = "";
                    }
                    Tokens.Add(new Token(text[i].ToString()));
                }
                else if (text[i] == '"')
                {

                    if (token.Length != 0)
                    {
                        Tokens.Add(new Token(token));
                        token = "";
                    }

                    var pos = text.IndexOf("\"", i+1);
                    Tokens.Add(new Token(text.Substring(i, pos - i+1)));

                    i = pos;
                }
                else
                {
                    token += text[i];
                }

            }
        }
        public bool hasMoreTokens()
        {
            return Tokens.Count != position;
        }
        public void advance()
        {
            position = position + 1;
        }
        public class Token
        {
            static HashSet<string> keywords = new HashSet<string>(new string[]{ "class", "constructor", "function", "method", "field", "static", "var", "int", "char", "boolean", "void", "true", "false", "null", "this", "let", "do", "if", "else", "while", "return" });
            public static string[] symbols = { "{", "}", "(", ")", "[", "]", ".", ",", ";", "+", "-", "*", "/", "&", "|", "<", ">", "=", "~" };
            public string Value { get; }
            public TokenType Type { get; }
            public Token(string token)
            {
                Value = token;
                if (keywords.Contains(token))
                    Type = TokenType.KEYWORD;
                else if (symbols.Contains(token))
                    Type = TokenType.SYMBOL;
                else if (int.TryParse(token, out int a))
                    Type =  TokenType.INT_CONST;
                else if (token.StartsWith("\"") && token.EndsWith("\""))
                    Type = TokenType.STRING_CONST;
                else 
                    Type = TokenType.IDENTIFIER;
            }
            public override string ToString() => Value;
        }

        Token Current => Tokens[position];
        public Token Next => Tokens[position + 1];

        public TokenType tokenType() => Current.Type;

        public Keyword keyword()
        {
            switch (Current.Value)
            {
                case "class": return Keyword.CLASS; ;
                case "method": return Keyword.METHOD;
                case "function": return Keyword.FUNCTION;
                case "constructor": return Keyword.CONSTRUCTOR;
                case "int": return Keyword.INT;
                case "boolean": return Keyword.BOOLEAN;
                case "char": return Keyword.CHAR;
                case "void": return Keyword.VOID;
                case "var": return Keyword.VAR;
                case "static": return Keyword.STATIC;
                case "field": return Keyword.FIELD;
                case "let": return Keyword.LET;
                case "do": return Keyword.DO;
                case "if": return Keyword.IF;
                case "else": return Keyword.ELSE;
                case "while": return Keyword.WHILE;
                case "return": return Keyword.RETURN;
                case "true": return Keyword.TRUE;
                case "false": return Keyword.FALSE;
                case "null": return Keyword.NULL;
                case "this": return Keyword.THIS;
                default: throw new Exception();
            }
        }
        public char Symbol()
        {
            return Current.Value[0];
        }
        public string Identifier()
        {
            return Current.Value;
        }
        public int IntVal()
        {
            return int.Parse(Current.Value);
        }
        public string StringVal()
        {
            return Current.Value.Substring(1, Current.Value.Length - 2);                
        }
    }
}
