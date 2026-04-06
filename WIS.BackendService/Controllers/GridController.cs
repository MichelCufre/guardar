using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WIS.Application.GridConfiguration;
using WIS.Application.Setup;
using WIS.Exceptions;
using WIS.GridComponent.Execution.Serialization;
using WIS.Security;

namespace WIS.BackendService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GridController : ControllerBase
    {
        private readonly ILogger<GridController> _logger;
        private readonly IGridConfigService _configService;
        private readonly ISecurityService _securityService;
        private readonly IApplicationSetupService _applicationSetupService;

        public GridController(ILogger<GridController> logger, IGridConfigService configService, ISecurityService securityService, IApplicationSetupService applicationSetupService)
        {
            this._logger = logger;
            this._configService = configService;
            _securityService = securityService;
            _applicationSetupService = applicationSetupService;
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult UpdateConfig(GridWrapper data)
        {
            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", data.User))
            using (MappedDiagnosticsLogicalContext.SetScoped("Application", data.Application))
            {
                IGridWrapper response = new GridWrapper();

                using (MappedDiagnosticsLogicalContext.SetScoped("UserId", data.User))
                {
                    try
                    {

                        this._applicationSetupService.SetupServices(new ApplicationSetupInfo
                        {
                            Application = data.Application,
                            Predio = data.Predio,
                            User = data.User,
                            Token = data.PageToken,
                            Session = data.GetSessionData()
                        });

                        this._configService.UpdateGridConfig(data);
                    }
                    catch (Exception ex)
                    {
                        this._logger.LogError(ex, "Grid - UpdateConfig");
                        // response.SetError(ex.Message);
                        response.SetError("General_Sec0_Error_ErrorConfigGrilla");
                    }
                }
                return Ok(response);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult SaveFilter(GridWrapper data)
        {
            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", data.User))
            using (MappedDiagnosticsLogicalContext.SetScoped("Application", data.Application))
            {

                IGridWrapper response = new GridWrapper();

                try
                {
                    this._applicationSetupService.SetupServices(new ApplicationSetupInfo
                    {
                        Application = data.Application,
                        Predio = data.Predio,
                        User = data.User,
                        Token = data.PageToken,
                        Session = data.GetSessionData()
                    });

                    this._configService.SaveFilter(data);
                }
                catch (Exception ex)
                {
                    this._logger.LogError(ex, "Grid - SaveFilter");
                    response.SetError(ex.Message);
                }

                return Ok(response);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult RemoveFilter(GridWrapper data)
        {
            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", data.User))
            using (MappedDiagnosticsLogicalContext.SetScoped("Application", data.Application))
            {
                IGridWrapper response = new GridWrapper();

                try
                {
                    this._applicationSetupService.SetupServices(new ApplicationSetupInfo
                    {
                        Application = data.Application,
                        Predio = data.Predio,
                        User = data.User,
                        Token = data.PageToken,
                        Session = data.GetSessionData()
                    });
                    this._configService.RemoveFilter(data);
                }
                catch (ExpectedException ex)
                {
                    response.SetError(ex.Message, ex.StrArguments);
                }
                catch (Exception ex)
                {
                    this._logger.LogError(ex, "Grid - RemoveFilter");
                    response.SetError("General_Sec0_Error_ErrorEliminarFiltro");
                }

                return Ok(response);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult GetFilterList(GridWrapper data)
        {
            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", data.User))
            using (MappedDiagnosticsLogicalContext.SetScoped("Application", data.Application))
            {
                IGridWrapper response = new GridWrapper();

                try
                {
                    this._applicationSetupService.SetupServices(new ApplicationSetupInfo
                    {
                        Application = data.Application,
                        Predio = data.Predio,
                        User = data.User,
                        Token = data.PageToken,
                        Session = data.GetSessionData()
                    });
                    response.SetData(this._configService.GetFilterList(data));
                }
                catch (Exception ex)
                {
                    this._logger.LogError(ex, "Grid - GetFilterList");
                    response.SetError("General_Sec0_Error_ErrorListarFiltro");
                }

                return Ok(response);
            }
        }
    }
}
