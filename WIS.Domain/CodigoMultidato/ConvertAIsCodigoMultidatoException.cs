using System;

namespace WIS.Domain.CodigoMultidato
{
    public class ConvertAIsCodigoMultidatoException : Exception
    {
        public ConvertAIsCodigoMultidatoException(Exception inner)
            : base("General_Sec0_Error_ConvertAIsCodigoMultidato", inner)
        {

        }
    }
}
