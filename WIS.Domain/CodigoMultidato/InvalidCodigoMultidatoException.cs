using System;
using WIS.Exceptions;

namespace WIS.Domain.CodigoMultidato
{
    public class InvalidCodigoMultidatoException : ExpectedException
    {
        public InvalidCodigoMultidatoException(string texto, string[] strArguments = null)
            : base(texto, strArguments) { }
    }
}
