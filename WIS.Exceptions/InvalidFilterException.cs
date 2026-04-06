using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Exceptions
{
    public class InvalidFilterException : ExpectedException
    {
        public InvalidFilterException(string texto, string[] strArguments = null) : base(texto, strArguments)
        {
        }
    }
}
