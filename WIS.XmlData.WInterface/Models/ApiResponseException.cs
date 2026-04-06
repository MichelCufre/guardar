using Microsoft.AspNetCore.Mvc;

namespace WIS.XmlData.WInterface.Models
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
