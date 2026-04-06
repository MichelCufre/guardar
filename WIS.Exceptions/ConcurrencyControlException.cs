using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Exceptions
{
    public class ConcurrencyControlException : ExpectedException
    {
        public ConcurrencyControlException(string texto, string[] strArguments = null) : base(texto, strArguments)
        {
        }
    }
}
