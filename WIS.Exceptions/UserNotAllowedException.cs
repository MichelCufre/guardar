using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Exceptions
{
    public class UserNotAllowedException : ExpectedException
    {
        public UserNotAllowedException(string texto, string[] strArguments = null) : base(texto, strArguments)
        {
        }
    }
}
