using WIS.Exceptions;

namespace WIS.Domain.General.API.Dtos
{
    public class ApiException : ExpectedException
    {
        public ApiException(string texto, string[] strArguments = null) : base(texto, strArguments)
        {
        }
    }
}
