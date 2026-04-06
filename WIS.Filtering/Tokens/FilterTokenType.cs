using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Filtering.Tokens
{
    public enum FilterTokenType
    {
        PARENTHESIS_OPEN,
        PARENTHESIS_CLOSE,
        BINARY_AND,
        BINARY_OR,
        BINARY_EQUAL,
        BINARY_GREATER_THAN,
        BINARY_GREATER_THAN_OR_EQUAL,
        BINARY_LESS_THAN,
        BINARY_LESS_THAN_OR_EQUAL,
        UNARY_NOT,
        BINARY_OPERATION_BETWEEN,
        BINARY_OPERATION_IN,
        BINARY_OPERATION_NOT_IN,
        LITERAL,
        BINARY_MULTIPLY,
        BINARY_ADD,
        BINARY_SUBTRACT,
        BINARY_DIVIDE,
        UNARY_NEGATE
    }
}
