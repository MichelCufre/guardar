using Microsoft.Extensions.Logging;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using WIS.Exceptions;
using WIS.FormComponent.Execution.Serialization;

namespace WIS.FormComponent.Execution
{
    public class FormActionResolver : IFormActionResolver
    {
        private readonly ILogger<FormActionResolver> _logger;
        private readonly IFormCoordinator _coordinator;

        public FormActionResolver(IFormCoordinator coordinator, ILogger<FormActionResolver> logger)
        {
            this._coordinator = coordinator;
            this._logger = logger;
        }

        public IFormWrapper InvokeAction(IFormWrapper wrapper, IFormController controller)
        {
            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", wrapper.User))
            using (MappedDiagnosticsLogicalContext.SetScoped("Application", wrapper.Application))
            {
                IFormWrapper response = new FormWrapper();

                try
                {
                    if (!this._coordinator.IsActionAvailable(wrapper.Action))
                        throw new NotSupportedException("Accion no soportada");

                    response = this._coordinator.Actions[wrapper.Action](wrapper, controller);
                }
                catch (ExpectedException ex)
                {
                    this._logger.LogWarning(ex, ex.GetMessage());
                    response.SetError(ex.Message, ex.StrArguments);
                }
                catch (Exception ex)
                {
                    this._logger.LogError(ex, "Form - CallController");
                    response.SetError(ex.Message);
                }

                return response;
            }
        }
    }
}
