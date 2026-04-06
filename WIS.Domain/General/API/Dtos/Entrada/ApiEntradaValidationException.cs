using System.Collections.Generic;

namespace WIS.Domain.General.API.Dtos.Entrada
{
    public class ApiEntradaValidationException : ApiException
    {
        public ApiEntradaValidationException(string texto, List<ValidationsError> errors) : base(texto)
        {
            Errors = errors;
        }

        public List<ValidationsError> Errors { get; set; }
    }
}
