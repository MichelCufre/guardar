using Microsoft.AspNetCore.Mvc;

namespace WIS.XmlProcessorEntrada.Models
{
    public class ApiResponseException: Exception
    {
        public ProblemDetails ProblemDetails { get; set; }

        public ApiResponseException(ProblemDetails problemDetails)
        {
            ProblemDetails = problemDetails;
        }
    }
}
