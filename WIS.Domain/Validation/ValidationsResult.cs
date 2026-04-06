using System.Collections.Generic;
using System.Linq;

namespace WIS.Domain.General
{
    public class ValidationsResult
    {
        public string SuccessMessage { get; set; }

        public List<ValidationsError> Errors { get; set; }

        public virtual void AddError(string message, int item = 0)
        {
            this.Errors.Add(new ValidationsError(item, false, new List<string> { message }));
        }
        public virtual void AddError(List<string> messages, int item = 0)
        {
            this.Errors.Add(new ValidationsError(item, false, messages));
        }
        public virtual bool IsValid()
        {
            return this.Errors.Count == 0;
        }

        public virtual bool HasError()
        {
            return this.Errors.Count > 0;
        }

        public virtual bool HasProceduralError()
        {
            return this.Errors.Exists(e => e.IsProcedural);
        }

        public ValidationsResult()
        {
            this.Errors = new List<ValidationsError>();
        }

        public virtual List<string> GetErrors()
        {
            return this.Errors?.SelectMany(s => s.Messages).ToList();
        }
    }
}
