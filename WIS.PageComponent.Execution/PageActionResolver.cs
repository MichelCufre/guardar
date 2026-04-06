using Microsoft.Extensions.Logging;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using WIS.Exceptions;
using WIS.PageComponent.Execution.Serialization;

namespace WIS.PageComponent.Execution
{
    public class PageActionResolver : IPageActionResolver
    {
        private readonly ILogger<PageActionResolver> _logger;
        private readonly IPageCoordinator _coordinator;


        public PageActionResolver(IPageCoordinator coordinator, ILogger<PageActionResolver> logger)
        {
            this._coordinator = coordinator;
            this._logger = logger;
        }

        public IPageWrapper InvokeAction(IPageWrapper wrapper, IPageController controller)
        {
            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", wrapper.User))
            using (MappedDiagnosticsLogicalContext.SetScoped("Application", wrapper.Application))
            {
                IPageWrapper response = new PageWrapper();

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
                    this._logger.LogError(ex, "Page - CallController");

                    response.SetError(ex.Message);
                }

                return response;
            }
        }
    }
}
