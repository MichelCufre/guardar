using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Exceptions
{
    public class EntityLockedException : ExpectedException
    {
        public EntityLockedException(string texto, string[] strArguments = null) : base(texto, strArguments)
        {
        }
    }
}
