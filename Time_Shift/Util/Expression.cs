// ****************************************************************************
//
// Copyright (C) 2014-2016 TautCony (TautCony@vcb-s.com)
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// ****************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChapterTool.Util
{
    public class Expression
    {
        private IEnumerable<Token> PostExpression { get; set; }

        public static Expression Empty
        {
            get
            {
                var ret = new Expression
                {
                    PostExpression = new List<Token> {new Token {TokenType = Token.Symbol.Variable, Value = "t"}}
                };
                return ret;
            }
        }

        private Expression()
        {
        }

        public Expression(string expr)
        {
            PostExpression = BuildPostExpressionStack(expr);
        }

        public override string ToString()
        {
            return PostExpression.Reverse().Aggregate("", (word, token) => word + token.Value + " ");
        }

        private static bool IsDigit(char c) => c >= '0' && c <= '9' || c == '.';

        private static bool IsAlpha(char c) => (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');

        public class Token
        {
            public string Value { get; set; } = string.Empty;
            public Symbol TokenType { get; set; } = Symbol.Blank;
            public decimal Number { get; set; }

            public static Lazy<Token> Empty => new Lazy<Token>(() => new Token("", Symbol.Blank));

            public Token()
            {
            }

            public Token(string value, Symbol type)
            {
                Value = value;
                TokenType = type;
            }

            public enum Symbol
            {
                Blank, Number, Variable, Operator, Bracket
            }

            public override string ToString()
            {
                if (TokenType == Symbol.Number)
                    return $"{TokenType} [{Number}]";
                return $"{TokenType} [{Value}]";
            }

            public static Token operator +(Token lhs, Token rhs)
            {
                var ret = lhs.Number + rhs.Number;
                return new Token { TokenType = Symbol.Number, Number = ret };
            }

            public static Token operator -(Token lhs, Token rhs)
            {
                var ret = lhs.Number - rhs.Number;
                return new Token { TokenType = Symbol.Number, Number = ret };
            }

            public static Token operator *(Token lhs, Token rhs)
            {
                var ret = lhs.Number * rhs.Number;
                return new Token { TokenType = Symbol.Number, Number = ret };
            }

            public static Token operator /(Token lhs, Token rhs)
            {
                var ret = lhs.Number / rhs.Number;
                return new Token { TokenType = Symbol.Number, Number = ret };
            }

            public static Token operator %(Token lhs, Token rhs)
            {
                var ret = lhs.Number % rhs.Number;
                return new Token { TokenType = Symbol.Number, Number = ret };
            }

            public static Token operator ^(Token lhs, Token rhs)
            {
                var ret = (decimal)Math.Pow((double)lhs.Number, (double)rhs.Number);
                return new Token { TokenType = Symbol.Number, Number = ret };
            }
        }

        private static Token GetToken(string expr, ref int pos)
        {
            const string tokens = "()+-*/%^";
            string numRet = string.Empty;
            string varRet = string.Empty;
            int i = pos;
            while (expr[i] == ' ' || expr[i] == '\t') ++i;
            for (; i < expr.Length; i++)
            {
                if (IsDigit(expr[i]))
                {
                    if (varRet.Length == 0)
                        numRet += expr[i];
                    else
                        varRet += expr[i];
                    continue;
                }
                if (IsAlpha(expr[i]))
                {
                    if (numRet.Length != 0)
                        break;
                    varRet += expr[i];
                    continue;
                }
                if (tokens.Contains(expr[i]))
                {
                    if (numRet.Length == 0 && varRet.Length == 0)
                    {
                        pos = i + 1;
                        if (expr[pos - 1] == '(' || expr[pos - 1] == ')')
                            return new Token($"{expr[pos - 1]}", Token.Symbol.Bracket);
                        return new Token($"{expr[pos - 1]}", Token.Symbol.Operator);
                    }
                    break;
                }
            }
            pos = i;
            if (varRet.Length == 0)
                return new Token(numRet, Token.Symbol.Number) { Number = decimal.Parse(numRet) };
            return new Token(varRet, Token.Symbol.Variable);
        }

        private static int GetPriority(Token token)
        {
            var precedence = new Dictionary<string, int>
            {
                ["+"] = 0,
                ["-"] = 0,
                ["*"] = 1,
                ["/"] = 1,
                ["%"] = 1,
                ["^"] = 2
            };
            if (string.IsNullOrEmpty(token.Value) || token.TokenType == Token.Symbol.Blank) return -2;
            if (token.TokenType == Token.Symbol.Variable ||
                token.TokenType == Token.Symbol.Number) return -1;
            return precedence[token.Value];
        }

        public static IEnumerable<Token> BuildPostExpressionStack(string expr)
        {
            var retStack = new Stack<Token>();
            var stack = new Stack<Token>();
            stack.Push(Token.Empty.Value);
            int pos = 0;
            var preToken = Token.Empty.Value;
            bool comment = false;
            while (pos < expr.Length && !comment)
            {
                var token = GetToken(expr, ref pos);
                switch (token.TokenType)
                {
                case Token.Symbol.Bracket:
                    switch (token.Value)
                    {
                    case "(":
                        stack.Push(token);
                        break;

                    case ")":
                        while (stack.Peek().Value != "(")
                        {
                            retStack.Push(stack.Peek());
                            stack.Pop();
                        }
                        if (stack.Peek().Value == "(") stack.Pop();
                        break;
                    }
                    preToken = token;
                    break;

                case Token.Symbol.Operator:
                    var lastToken = stack.Peek();
                    switch (lastToken.TokenType)
                    {
                    case Token.Symbol.Blank:
                    case Token.Symbol.Bracket:
                        if (preToken.Value == "(" && token.Value == "-")
                            retStack.Push(new Token { TokenType = Token.Symbol.Number, Value = "0", Number = 0M });
                        stack.Push(token);
                        break;

                    case Token.Symbol.Operator:
                        if (token.Value == "/" && preToken.Value == "/")
                        {
                            stack.Pop();
                            comment = true;
                            break;
                        }
                        if (token.Value == "-" && preToken.TokenType == Token.Symbol.Operator)
                        {
                            retStack.Push(new Token { TokenType = Token.Symbol.Number, Value = "0", Number = 0M });
                        }
                        else if (GetPriority(lastToken) >= GetPriority(token))
                        {
                            retStack.Push(lastToken);
                            stack.Pop();
                        }
                        stack.Push(token);
                        break;

                    default:
                        throw new Exception($"Invalid operator stack item {token}");
                    }
                    preToken = token;
                    break;

                default:
                    preToken = token;
                    retStack.Push(token);
                    break;
                }
            }
            while (stack.Peek().Value != Token.Empty.Value.Value)
            {
                retStack.Push(stack.Peek());
                stack.Pop();
            }
            return retStack;
        }

        public static decimal Eval(IEnumerable<Token> posfix, Dictionary<string, decimal> values)
        {
            var stack = new Stack<Token>();
            foreach (var token in posfix.Reverse())
            {
                switch (token.TokenType)
                {
                case Token.Symbol.Number:
                    stack.Push(token);
                    break;

                case Token.Symbol.Variable:
                    stack.Push(new Token { TokenType = Token.Symbol.Number, Number = values[token.Value] });
                    break;

                case Token.Symbol.Operator:
                    var rhs = stack.Peek();
                    stack.Pop();
                    var lhs = stack.Peek();
                    stack.Pop();
                    switch (token.Value)
                    {
                    case "+": stack.Push(lhs + rhs); break;
                    case "-": stack.Push(lhs - rhs); break;
                    case "*": stack.Push(lhs * rhs); break;
                    case "/": stack.Push(lhs / rhs); break;
                    case "%": stack.Push(lhs % rhs); break;
                    case "^": stack.Push(lhs ^ rhs); break;
                    }
                    break;
                }
            }
            return stack.Peek().Number;
        }

        public decimal Eval(Dictionary<string, decimal> values) => Eval(PostExpression, values);

        public decimal Eval(double time)
        {
            try
            {
                return Eval(new Dictionary<string, decimal> { ["t"] = (decimal)time });
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return (decimal)time;
            }
        }

        public decimal Eval() => Eval(new Dictionary<string, decimal>());


    }
}