using System.Collections.Generic;
using System.Linq;
using WIS.Exceptions;

namespace WIS.Domain.Validation
{
    public class Error
    {
        public string Mensaje { get; set; }
        public object[] Argumentos { get; set; }

        public Error(string mensaje, params object[] argumentos)
        {
            Mensaje = mensaje;
            Argumentos = argumentos ?? new object[0];
        }

        public Error(ValidationFailedException ex)
        {
            Mensaje = ex.Message;
            Argumentos = ex.StrArguments ?? new object[0];
        }

        public virtual List<string> GetArgumentos()
        {
            return Argumentos.Select(a => a?.ToString()).ToList();
        }
    }
}
