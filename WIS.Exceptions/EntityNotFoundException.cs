using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Exceptions
{
    public class EntityNotFoundException : ExpectedException
    {
        public EntityNotFoundException(string texto, string[] strArguments = null) : base(texto, strArguments)
        {
        }
    }
}
