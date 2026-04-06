using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Exceptions
{
    public class MissingParameterException : ExpectedException
    {
        public MissingParameterException(string texto, string[] strArguments = null) : base(texto, strArguments)
        {
        }
    }
}
