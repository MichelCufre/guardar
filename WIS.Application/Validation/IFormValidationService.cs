using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.FormComponent.Validation;

namespace WIS.Application.Validation
{
    public interface IFormValidationService
    {
        public Form Validate(IFormValidationModule module, Form form, FormValidationContext context);
    }
}