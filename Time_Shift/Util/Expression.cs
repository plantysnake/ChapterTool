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

        private bool EvalAble { get; set; } = true;

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

        private static bool IsAlpha(char c) => (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_';

        public class Token
        {
            public string Value { get; set; } = string.Empty;
            public Symbol TokenType { get; set; } = Symbol.Blank;
            public decimal Number { get; set; }

            public static Token End => new Token("", Symbol.Blank);
            public static Token Zero => new Token("0", Symbol.Number);


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
                Blank, Number, Variable, Operator, Bracket, Function
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

        private static readonly HashSet<string> FunctionTokens = new HashSet<string>
        {
            "abs",
            "acos", "asin", "atan",
            "cos", "sin", "tan",
            "cosh", "sinh", "tanh",
            "exp", "log", "log10", "sqrt",
            "ceil", "floor"
        };

        private static readonly Dictionary<string, decimal> MathDefines = new Dictionary<string, decimal>
        {
            ["M_E"]        = 2.71828182845904523536M, // e
            ["M_LOG2E"]    = 1.44269504088896340736M, // log2(e)
            ["M_LOG10E"]   = 0.43429448190325182765M, // log10(e)
            ["M_LN2"]      = 0.69314718055994530942M, // ln(2)
            ["M_LN10"]     = 2.30258509299404568402M, // ln(10)
            ["M_PI"]       = 3.14159265358979323846M, // pi
            ["M_PI_2"]     = 1.57079632679489661923M, // pi/2
            ["M_PI_4"]     = 0.78539816339744830962M, // pi/4
            ["M_1_PI"]     = 0.31830988618379067154M, // 1/pi
            ["M_2_PI"]     = 0.63661977236758134308M, // 2/pi
            ["M_2_SQRTPI"] = 1.12837916709551257390M, // 2/sqrt(pi)
            ["M_SQRT2"]    = 1.41421356237309504880M, // sqrt(2)
            ["M_SQRT1_2"]  = 0.70710678118654752440M  // 1/sqrt(2)
        };

        private static Token EvalCMath(Token func, Token value)
        {
            if (!FunctionTokens.Contains(func.Value))
                throw new Exception($"There is no function named {func.Value}");
            var ret = new Token {TokenType = Token.Symbol.Number};
            switch (func.Value)
            {
            case "abs"  : ret.Number = Math.Abs(value.Number); break;
            case "acos" : ret.Number = (decimal)Math.Acos((double)value.Number); break;
            case "asin" : ret.Number = (decimal)Math.Asin((double)value.Number); break;
            case "atan" : ret.Number = (decimal)Math.Atan((double)value.Number); break;
            case "cos"  : ret.Number = (decimal)Math.Cos((double)value.Number); break;
            case "sin"  : ret.Number = (decimal)Math.Sin((double)value.Number); break;
            case "tan"  : ret.Number = (decimal)Math.Tan((double)value.Number); break;
            case "cosh" : ret.Number = (decimal)Math.Cosh((double)value.Number); break;
            case "sinh" : ret.Number = (decimal)Math.Sinh((double)value.Number); break;
            case "tanh" : ret.Number = (decimal)Math.Tanh((double)value.Number); break;
            case "exp"  : ret.Number = (decimal)Math.Exp((double)value.Number); break;
            case "log"  : ret.Number = (decimal)Math.Log((double)value.Number); break;
            case "log10": ret.Number = (decimal)Math.Log10((double)value.Number); break;
            case "sqrt" : ret.Number = (decimal)Math.Sqrt((double)value.Number); break;
            case "ceil" : ret.Number = Math.Ceiling(value.Number); break;
            case "floor": ret.Number = Math.Floor(value.Number); break;
            }
            return ret;
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
            {
                decimal number;
                if (!decimal.TryParse(numRet, out number))
                    throw new Exception($"Invalid number token [{numRet}]");
                return new Token(numRet, Token.Symbol.Number) { Number = number };
            }
            if (FunctionTokens.Contains(varRet))
                return new Token(varRet, Token.Symbol.Function);
            if (MathDefines.ContainsKey(varRet))
                return new Token(varRet, Token.Symbol.Number) {Number = MathDefines[varRet]};
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
            if (!precedence.ContainsKey(token.Value))
                throw new Exception($"Invalid operator [{token.Value}]");
            return precedence[token.Value];
        }

        public static IEnumerable<Token> BuildPostExpressionStack(string expr)
        {
            var retStack = new Stack<Token>();
            var stack = new Stack<Token>();
            var funcStack = new Stack<Token>();
            stack.Push(Token.End);
            int pos = 0;
            var preToken = Token.End;
            bool comment = false;
            while (pos < expr.Length && !comment)
            {
                var token = GetToken(expr, ref pos);
                switch (token.TokenType)
                {
                case Token.Symbol.Function:
                    funcStack.Push(token);
                    break;
                case Token.Symbol.Bracket:
                    switch (token.Value)
                    {
                    case "(": stack.Push(token); break;
                    case ")":
                        while (stack.Peek().Value != "(")
                        {
                            retStack.Push(stack.Peek());
                            stack.Pop();
                        }
                        if (stack.Peek().Value == "(") stack.Pop();
                        if (funcStack.Count != 0)
                        {
                            retStack.Push(funcStack.Peek());
                            funcStack.Pop();
                        }
                        break;
                        default:
                        throw new ArgumentOutOfRangeException($"Invalid bracket token {token.Value}");
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
                            retStack.Push(Token.Zero);
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
                            retStack.Push(Token.Zero);
                        }
                        else while (lastToken.TokenType != Token.Symbol.Bracket &&
                                GetPriority(lastToken) >= GetPriority(token))
                        {
                            retStack.Push(lastToken);
                            stack.Pop();
                            lastToken = stack.Peek();
                        }
                        stack.Push(token);
                        break;
                    default:
                        throw new Exception($"Unexcept token type: {token.Value} => {token.TokenType}");
                    }
                    preToken = token;
                    break;
                default:
                    preToken = token;
                    retStack.Push(token);
                    break;
                }
            }
            while (stack.Peek().Value != string.Empty)
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
                case Token.Symbol.Function:
                    var para = stack.Peek();
                    stack.Pop();
                    stack.Push(EvalCMath(token, para));
                    break;
                }
            }
            return stack.Peek().Number;
        }

        public decimal Eval(Dictionary<string, decimal> values) => Eval(PostExpression, values);

        public decimal Eval(double time)
        {
            if (!EvalAble) return (decimal)time;
            try
            {
                return Eval(new Dictionary<string, decimal> { ["t"] = (decimal)time });
            }
            catch (Exception exception)
            {
                EvalAble = false;
                Console.WriteLine($"Eval Failed: {exception.Message}");
                return (decimal)time;
            }
        }

        public decimal Eval()
        {
            if (!EvalAble) return 0;
            try
            {
                return Eval(new Dictionary<string, decimal>());
            }
            catch (Exception exception)
            {
                EvalAble = false;
                Console.WriteLine($"Eval Failed: {exception.Message}");
                return 0;
            }
        }


    }
}