using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using WIS.Application.Localization;
using WIS.Application.Localization.Serialization;
using WIS.Translation;

namespace WIS.BackendService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TranslationController : ControllerBase
    {
        private readonly ILogger<TranslationController> _logger;
        private readonly ITranslationUpdateService _translationService;
        private readonly ITranslator _translator;

        public TranslationController(ITranslationUpdateService translationService,
            ITranslator translator,
            ILogger<TranslationController> logger) : base()
        {
            this._logger = logger;
            this._translationService = translationService;
            this._translator = translator;
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult GetAllTranslationVersions(TranslationWrapper data)
        {
            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", data.User))
            using (MappedDiagnosticsLogicalContext.SetScoped("Application", data.Application))
            {
                try
                {
                    List<TranslationVersion> versions = this._translationService.GetAllVersions(data.Application, data.User);

                    data.SetData(versions);
                }
                catch (Exception ex)
                {
                    this._logger.LogError(ex, "Localization - GetAllTranslationVersions");

                    data.SetError(ex.Message);
                }

                return Ok(data);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult GetResources(TranslationWrapper data)
        {
            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", data.User))
            using (MappedDiagnosticsLogicalContext.SetScoped("Application", data.Application))
            {
                try
                {
                    string language = data.GetData<string>();

                    List<TranslatedValue> resources = this._translationService.GetResources(data.Application, data.User, language);

                    data.SetData(resources);
                }
                catch (Exception ex)
                {
                    this._logger.LogError(ex, "Localization - GetResources");

                    data.SetError(ex.Message);
                }

                return Ok(data);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult UpdateDatabaseResources(TranslationWrapper data)
        {
            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", data.User))
            using (MappedDiagnosticsLogicalContext.SetScoped("Application", data.Application))
            {
                try
                {
                    this._translationService.UpdateDatabaseResources(data.Application, data.User);
                }
                catch (Exception ex)
                {
                    this._logger.LogError(ex, "Localization - UpdateDatabaseResources");

                    data.SetError(ex.Message);
                }

                return Ok(data);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult Translate(List<string> keys)
        {
            var t = _translator.Translate(keys);
            return Content(JsonConvert.SerializeObject(t), "application/json");
        }
    }
}
