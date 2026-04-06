using System.Collections.Generic;
using WIS.Domain.General;
using WIS.Domain.Validation;

namespace WIS.Domain.Tracking.Validation
{
    public class TrackingValidationResult
    {
        public string Code { get; set; }
        public List<Error> Errors { get; set; }
        public string Message { get; set; }

        public TrackingValidationResult()
        {
            this.Errors = new List<Error>();
        }

        public virtual bool HasError()
        {
            return this.Errors.Count > 0;
        }

        public virtual void AddError(string message)
        {
            this.Errors.Add(new Error(message));
        }
        public virtual void AddGenericError()
        {
            AddError("Se encontraron errores al validar");
        }
    }
}
