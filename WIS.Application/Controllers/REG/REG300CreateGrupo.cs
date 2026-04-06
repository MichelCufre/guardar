using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Domain.DataModel;
using WIS.Components.Common.Select;
using WIS.Domain.General;
using WIS.Exceptions;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;
using System.Collections.Generic;

namespace WIS.Application.Controllers.REG
{
	public class REG300CreateGrupo : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IFormValidationService _formValidationService;
        protected readonly ILogger<REG300CreateGrupo> _logger;

        public REG300CreateGrupo(IUnitOfWorkFactory uowFactory, IIdentityService identity, IFormValidationService formValidationService, ILogger<REG300CreateGrupo> logger)
        {
            _uowFactory = uowFactory;
            _identity = identity;
            _formValidationService = formValidationService;
            _logger = logger;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            form.GetField("codigoGrupo").Value = string.Empty;
            form.GetField("descripcion").Value = string.Empty;
            form.GetField("clase").Options = new List<SelectOption>();

            List<Clase> clases = uow.ClaseRepository.GetClases();
            foreach (var clase in clases)
            {
                form.GetField("clase").Options.Add(new SelectOption(clase.Id.ToString(), $"{clase.Id} - { clase.Descripcion}")); ;
            }

            return form;
        }
        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            try
            {
                var grupo = new Grupo
                {
                    Id = form.GetField("codigoGrupo").Value.ToUpper(),
                    Descripcion = form.GetField("descripcion").Value,
                    FechaInsercion = DateTime.Now,
                    CodigoClase = form.GetField("clase").Value,
                    Default = false,
                };
                uow.GrupoRepository.AddGrupo(grupo);
                uow.SaveChanges();
                context.AddSuccessNotification("General_Sec0_Error_Er021_SaveSuccess");
            }
            catch (ValidationFailedException ex)
            {
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
                this._logger.LogError(ex, ex.Message, ex.StrArguments);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, ex.Message);
                throw ex;
            }
            return form;
        }
        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new CreateGrupoFormValidationModule(uow), form, context);
        }
    }
}
