using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Exceptions
{
    public class OperationNotAllowedException : ExpectedException
    {
        public OperationNotAllowedException(string texto, string[] strArguments = null) : base(texto, strArguments)
        {
        }
    }
}
