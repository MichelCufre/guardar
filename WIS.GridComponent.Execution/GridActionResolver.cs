using Microsoft.Extensions.Logging;
using NLog;
using System;
using WIS.Exceptions;
using WIS.GridComponent.Execution.Serialization;

namespace WIS.GridComponent.Execution
{
    public class GridActionResolver : IGridActionResolver
    {
        private readonly ILogger<GridActionResolver> _logger;
        private readonly IGridCoordinator _coordinator;

        public GridActionResolver(IGridCoordinator coordinator, ILogger<GridActionResolver> logger)
        {
            this._coordinator = coordinator;
            this._logger = logger;
        }

        public IGridWrapper InvokeAction(IGridWrapper wrapper, IGridController controller)
        {
            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", wrapper.User))
            using (MappedDiagnosticsLogicalContext.SetScoped("Application", wrapper.Application))
            {

                IGridWrapper response = new GridWrapper();

                try
                {
                    if (!this._coordinator.IsActionAvailable(wrapper.Action))
                        throw new NotSupportedException("Accion no soportada");

                    response = this._coordinator.Actions[wrapper.Action](wrapper, controller);
                }
                catch (ExpectedException ex)
                {
                    this._logger.LogWarning(ex, "GridActionResolver - InvokeAction");
                    response.SetError(ex.Message, ex.StrArguments);
                }
                catch (Exception ex)
                {
                    this._logger.LogError(ex, "GridActionResolver - InvokeAction");
                    response.SetError("General_Sec0_Error_ErrorOperacionGrilla");
                }

                return response;
            }
        }
    }
}
