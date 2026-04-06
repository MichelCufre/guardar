using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Exceptions
{
    public class UserNotAuthenticatedException : ExpectedException
    {
        public UserNotAuthenticatedException(string texto, string[] strArguments = null) : base(texto, strArguments)
        {
        }
    }
}
