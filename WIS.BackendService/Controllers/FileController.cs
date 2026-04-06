using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NLog;
using System;
using System.Threading.Tasks;
using WIS.Application.File;
using WIS.Application.Setup;
using WIS.Exceptions;
using WIS.File.Execution;
using WIS.File.Execution.Serialization;
using WIS.Security;

namespace WIS.BackendService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;
        private readonly ILogger<FileController> _logger;
        private readonly ISecurityService _securityService;
        private readonly IApplicationSetupService _applicationSetupService;

        public FileController(IFileService fileService, ILogger<FileController> logger, ISecurityService securityService, IApplicationSetupService applicationSetupService)
        {
            this._fileService = fileService;
            this._logger = logger;
            this._securityService = securityService;
            this._applicationSetupService = applicationSetupService;

        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetFile(FileWrapper wrapper)
        {
            var response = new FileWrapper();

            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", wrapper.User))
            using (MappedDiagnosticsLogicalContext.SetScoped("Application", wrapper.Application))
            {
                try
                {
                    var request = wrapper.GetData<File.Execution.FileDownloadRequest>();

                    this._applicationSetupService.SetupServices(new ApplicationSetupInfo
                    {
                        Application = wrapper.Application,
                        Predio = wrapper.Predio,
                        User = wrapper.User,
                        Token = wrapper.PageToken,
                        Session = wrapper.GetSessionData()
                    });

                    if (!this._securityService.CanUserAccessApplication())
                        throw new UserNotAllowedException("General_Sec0_Error_AccessNotAllowed");

                    var res = await this._fileService.GetFile(wrapper.User, request);
                    response.SetData(res);
                }
                catch (InvalidUserException ex)
                {
                    this._logger.LogError(ex, "File - GetFile - Unauthenticated");
                    return Unauthorized();
                }
                catch (UserNotAllowedException ex)
                {
                    this._logger.LogError(ex, "File - GetFile - Forbidden");
                    return Unauthorized();
                }
                catch (Exception ex)
                {
                    this._logger.LogError(ex, "File - GetFile");
                    response.SetError(ex.Message);
                }

                return Ok(response);
            }
        }

        //Modal de archivos 
        [HttpPost("[action]")]
        public IActionResult AddFile(FileWrapper wrapper)
        {
            var response = new FileWrapper();

            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", wrapper.User))
            {
                try
                {
                    var request = wrapper.GetData<FileUploadRequest>();
                    var res = this._fileService.AddFile(wrapper.User, request);
                    response.SetData(res);
                }
                catch (Exception ex)
                {
                    this._logger.LogError(ex, "File - GetFile");
                    response.SetError(ex.Message);
                }

                return Ok(response);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult DeleteFile(FileWrapper wrapper)
        {
            var response = new FileWrapper();

            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", wrapper.User))
            using (MappedDiagnosticsLogicalContext.SetScoped("Application", wrapper.Application))
            {
                try
                {
                    var request = wrapper.GetData<FileDeleteRequest>();
                    var res = this._fileService.DeleteFile(wrapper.User, request);

                    response.SetData(res);
                }
                catch (InvalidUserException ex)
                {
                    this._logger.LogError(ex, "File - Delete - Unauthenticated");
                    return Unauthorized();
                }
                catch (UserNotAllowedException ex)
                {
                    this._logger.LogError(ex, "File - Delete - Forbidden");
                    return Unauthorized();
                }
                catch (Exception ex)
                {
                    this._logger.LogError(ex, "File - Delete");
                    response.SetError(ex.Message);
                }

                return Ok(response);
            }
        }
    }
}
