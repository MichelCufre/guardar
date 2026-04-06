using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Exceptions
{
    public class ValidationFailedException : ExpectedException
    {
        public ValidationFailedException(string texto, string[] strArguments = null) : base(texto, strArguments)
        {
        }

        //Fix temporal para sacar errores de grilla en formulario
        public bool setData { get; }
        public ValidationFailedException(string texto, bool setData, string[] strArguments = null) : base(texto, strArguments)
        {
            this.setData = setData;
        }
        //Fin Fix
    }
}
