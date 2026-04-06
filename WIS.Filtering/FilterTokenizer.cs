using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WIS.Filtering.Tokens;

namespace WIS.Filtering
{
    public class FilterTokenizer : IFilterTokenizer
    {
        public List<IFilterToken> Tokens { get; set; }

        protected readonly Dictionary<FilterTokenType, int> OperatorPrecedence = new Dictionary<FilterTokenType, int>
        {
            { FilterTokenType.PARENTHESIS_OPEN, 1 },
            { FilterTokenType.BINARY_OR, 2 },
            { FilterTokenType.BINARY_AND, 3 },
            { FilterTokenType.BINARY_EQUAL, 4 },
            { FilterTokenType.BINARY_GREATER_THAN, 4 },
            { FilterTokenType.BINARY_GREATER_THAN_OR_EQUAL, 4 },
            { FilterTokenType.BINARY_LESS_THAN, 4 },
            { FilterTokenType.BINARY_LESS_THAN_OR_EQUAL, 4 },
            { FilterTokenType.BINARY_OPERATION_BETWEEN, 4 },
            { FilterTokenType.BINARY_OPERATION_IN, 4 },
            { FilterTokenType.BINARY_OPERATION_NOT_IN, 4 },
            { FilterTokenType.UNARY_NOT, 5 },
        };

        public FilterTokenizer()
        {
            this.Tokens = new List<IFilterToken>();
        }

        public virtual void Tokenize(string text)
        {
            this.Tokens.Clear();

            using (var reader = new StringReader(text))
            {
                while (reader.Peek() != -1)
                {
                    while (char.IsWhiteSpace((char)reader.Peek()))
                    {
                        reader.Read();
                    }

                    if (reader.Peek() == -1)
                        break;

                    var c = (char)reader.Peek();

                    switch (c)
                    {
                        case '!':
                            this.Tokens.Add(new FilterTokenNot());
                            reader.Read();
                            break;
                        case '(':
                            this.Tokens.Add(new FilterTokenParenthesisOpen());
                            reader.Read();
                            break;
                        case ')':
                            this.Tokens.Add(new FilterTokenParenthesisClose());
                            reader.Read();
                            break;
                        case '<':
                            reader.Read();
                            if (reader.Peek() == '=')
                            {
                                this.Tokens.Add(new FilterTokenLessThanOrEqual());
                                reader.Read();
                            }
                            else
                            {
                                this.Tokens.Add(new FilterTokenLessThan());
                            }
                            break;
                        case '>':
                            reader.Read();
                            if (reader.Peek() == '=')
                            {
                                this.Tokens.Add(new FilterTokenGreaterThanOrEqual());
                                reader.Read();
                            }
                            else
                            {
                                this.Tokens.Add(new FilterTokenGreaterThan());
                            }
                            break;
                        case '=':
                            this.Tokens.Add(new FilterTokenEqual());
                            reader.Read();
                            break;
                        case '$':
                            var functionToken = this.ParseFunction(reader);
                            this.Tokens.Add(functionToken);
                            this.Tokens.Add(this.ParseFunctionLiteral(reader, functionToken)); //Ver de cambiar, no toda funcion recibe multiples valores, dividir segun caso
                            reader.Read();
                            break;
                        case '\'':
                            reader.Read();
                            this.Tokens.Add(this.ParseLiteral(reader));
                            break;
                        default:
                            this.Tokens.Add(this.ParseKeyword(reader));
                            break;
                    }
                }
            }
        }

        public virtual IFilterToken ParseLiteral(StringReader reader)
        {
            var text = new StringBuilder();

            if ((char)reader.Peek() != '\'')
            {
                while ((char)reader.Peek() != '\'')
                {
                    text.Append((char)reader.Read());
                }
            }
            else
            {
                text.Append((char)reader.Read());
            }

            reader.Read();

            string value = text.ToString().ToLower();

            if (value == "null")
                return new FilterTokenLiteralNull();

            return new FilterTokenLiteral(value);
        }

        public virtual IFilterToken ParseKeyword(StringReader reader)
        {
            var text = new StringBuilder();

            if (this.IsValidContentChar((char)reader.Peek()))
            {
                while (this.IsValidContentChar((char)reader.Peek()))
                {
                    text.Append((char)reader.Read());
                }
            }
            else
            {
                text.Append((char)reader.Read());
            }

            var potentialKeyword = text.ToString().ToLower();

            switch (potentialKeyword)
            {
                case "and":
                    return new FilterTokenAnd();
                case "or":
                    return new FilterTokenOr();
                default:
                    return new FilterTokenLiteral(potentialKeyword);
            }
        }

        public virtual IFunctionFilterToken ParseFunction(StringReader reader)
        {
            var text = new StringBuilder();

            reader.Read(); //omite $

            do
            {
                text.Append((char)reader.Read());
            }
            while ((char)reader.Peek() != '(');

            var functionName = text.ToString().ToLower();

            switch (functionName)
            {
                case "in":
                    return new FilterTokenIn();
                case "notin":
                    return new FilterTokenNotIn();
                case "between":
                    return new FilterTokenBetween();
            }

            return null;
        }

        public virtual IFilterToken ParseFunctionLiteral(StringReader reader, IFunctionFilterToken functionToken)
        {
            reader.Read(); //omite (

            var values = new StringBuilder();

            do
            {
                values.Append((char)reader.Read());
            }
            while ((char)reader.Peek() != ')');

            string processedValues = values.ToString();

            List<string> separateValues = processedValues.Split(';').Select(d => d.Trim()).ToList();

            if (functionToken.IsIterable)
                return new FilterTokenLiteralGroupIterable(separateValues);

            return new FilterTokenLiteralGroup(separateValues);
        }

        public virtual List<IFilterToken> GetPolishNotation()
        {
            Queue<IFilterToken> outputQueue = new Queue<IFilterToken>();
            Stack<IFilterToken> stack = new Stack<IFilterToken>();

            int index = 0;

            var tokens = this.GetPivotedList();

            tokens.Reverse();

            while (index < tokens.Count)
            {
                IFilterToken token = tokens[index];
                IFilterToken topToken;

                switch (token.TokenType)
                {
                    case FilterTokenType.LITERAL:
                        outputQueue.Enqueue(token);
                        break;
                    case FilterTokenType.PARENTHESIS_OPEN:
                        stack.Push(token);
                        break;
                    case FilterTokenType.PARENTHESIS_CLOSE:
                        topToken = stack.Pop();
                        while (topToken.TokenType != FilterTokenType.PARENTHESIS_OPEN)
                        {
                            outputQueue.Enqueue(topToken);
                            topToken = stack.Pop();
                        }
                        break;
                    default:
                        while (stack.Count > 0 && this.OperatorPrecedence[stack.Peek().TokenType] >= this.OperatorPrecedence[token.TokenType])
                        {
                            outputQueue.Enqueue(stack.Pop());
                        }

                        stack.Push(token);
                        break;
                }

                index++;
            }

            while (stack.Count > 0)
            {
                outputQueue.Enqueue(stack.Pop());
            }

            return outputQueue.Reverse().ToList();
        }

        public virtual List<IFilterToken> GetPivotedList()
        {
            var pivotedList = new List<IFilterToken>();

            foreach (var token in this.Tokens)
            {
                if (token.TokenType == FilterTokenType.PARENTHESIS_OPEN)
                    pivotedList.Add(new FilterTokenParenthesisClose());
                else if (token.TokenType == FilterTokenType.PARENTHESIS_CLOSE)
                    pivotedList.Add(new FilterTokenParenthesisOpen());
                else
                    pivotedList.Add(token);
            }

            return pivotedList;
        }

        public virtual bool IsValid()
        {
            int parenthesisOpenCount = this.Tokens.Where(d => d.TokenType == FilterTokenType.PARENTHESIS_OPEN).Count();
            int parenthesisCloseCount = this.Tokens.Where(d => d.TokenType == FilterTokenType.PARENTHESIS_CLOSE).Count();
            int literalCount = this.Tokens.Where(d => d.TokenType == FilterTokenType.LITERAL).Count();

            return parenthesisCloseCount == parenthesisOpenCount && literalCount > 1;
        }

        public virtual bool IsValidContentChar(char c)
        {
            return char.IsLetterOrDigit(c) || c == '/' || c == ':' || c == ';'
                || c == '\'' || c == '.' || c == ',' || c == '_' || c == '-'
                || c == '%' || c == '*';
        }
    }
}
