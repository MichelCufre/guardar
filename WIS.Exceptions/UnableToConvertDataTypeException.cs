using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Exceptions
{
    public class UnableToConvertDataTypeException : ExpectedException
    {
        public UnableToConvertDataTypeException(string texto, string[] strArguments = null) : base(texto, strArguments)
        {
        }
    }
}
