using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Exceptions
{
    public class InvalidUserException : ExpectedException
    {
        public InvalidUserException(string texto, string[] strArguments = null) : base(texto, strArguments)
        {
        }
    }
}
