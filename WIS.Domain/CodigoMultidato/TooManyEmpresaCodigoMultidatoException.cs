using System;

namespace WIS.Domain.CodigoMultidato
{
    public class TooManyEmpresaCodigoMultidatoException : Exception
    {
        public string Codigo { get; set; }

        public TooManyEmpresaCodigoMultidatoException(string codigo)
        {
            Codigo = codigo;
        }
    }
}
