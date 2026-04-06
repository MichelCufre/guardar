using DocumentFormat.OpenXml.InkML;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries;
using WIS.Domain.Recorridos;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;

namespace WIS.Application.Controllers.REG
{
    public class REG700CrearRecorrido : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ILogger<REG700CrearRecorrido> _logger;
        protected readonly IFormValidationService _formValidationService;

        public REG700CrearRecorrido(
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            ILogger<REG700CrearRecorrido> logger,
            IFormValidationService formValidationService)
        {
            _identity = identity;
            _uowFactory = uowFactory;
            _logger = logger;
            _formValidationService = formValidationService;
        }

        public override Form FormInitialize(Form form, FormInitializeContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            form.GetField("isEnabled").SetChecked(true);

            this.InicializarSelects(form, uow);

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            uow.CreateTransactionNumber("FormSubmit", _identity.Application, _identity.UserId);

            try
            {
                var nombre = form.GetField("nombre").Value;
                var descripcion = form.GetField("descripcion").Value;
                var predio = form.GetField("predio").Value;
                var habilitado = form.GetField("isEnabled").IsChecked();

                var recorrido = new Recorrido
                {
                    Nombre = nombre,
                    Descripcion = descripcion,
                    Predio = predio,
                    EsDefault = false,
                    EsHabilitado = habilitado,
                    Transaccion = uow.GetTransactionNumber()
                };

                recorrido = uow.RecorridoRepository.AddRecorrido(recorrido);

                uow.SaveChanges();

                query.AddParameter("REG700_DETALLES_NU_RECORRIDO", recorrido.Id.ToString());

                var colParams = new Dictionary<string, string>
                {
                    [ParamManager.PARAM_PRED] = $"{ParamManager.PARAM_PRED}_{predio}"
                };
                
                query.AddParameter("REG700_DETALLE_IMPORT_HABILITADO", "S");
                query.AddSuccessNotification("REG700_Frm1_Success_Create");
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "FormSubmit");
                query.AddErrorNotification("General_Sec0_Error_Operacion");
            }

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new REG700RecorridoFormValidationModule(uow, this._identity), form, context);
        }

        #region Auxs

        public virtual void InicializarSelects(Form form, IUnitOfWork uow)
        {
            FormField selectPredio = form.GetField("predio");

            selectPredio.Options = [];

            var dbQuery = new GetPrediosUsuarioQuery();
           
            uow.HandleQuery(dbQuery);

            var predios = dbQuery.GetPrediosUsuario(_identity.UserId);

            foreach (var predio in predios)
            {
                selectPredio.Options.Add(new SelectOption(predio.Numero, $"{predio.Numero} - {predio.Descripcion}")); ;
            }

            if (predios.Count == 1) selectPredio.Value = predios.FirstOrDefault().Numero;

        }
      
        #endregion
    }
}
