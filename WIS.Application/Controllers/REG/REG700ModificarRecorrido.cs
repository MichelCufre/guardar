using Microsoft.Extensions.Logging;
using System;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Components.Common.Select;
using WIS.Domain.Automatismo;
using WIS.Domain.DataModel;
using WIS.Domain.Recorridos;
using WIS.Extension;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;

namespace WIS.Application.Controllers.REG
{
    public class REG700ModificarRecorrido : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ILogger<REG700CrearRecorrido> _logger;
        protected readonly IFormValidationService _formValidationService;

        public REG700ModificarRecorrido(
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

            var nuRecorrido = query.GetParameter("REG700_DETALLES_NU_RECORRIDO").ToNumber<int>();

            var recorrido = uow.RecorridoRepository.GetRecorridoById(nuRecorrido);

            form.GetField("idRecorrido").Value = recorrido.Id.ToString();
            form.GetField("idRecorrido").ReadOnly = true;
            form.GetField("nombre").Value = recorrido.Nombre;
            form.GetField("descripcion").Value = recorrido.Descripcion;
            form.GetField("isEnabled").SetChecked(recorrido.EsHabilitado);

            if (recorrido.EsDefault || uow.RecorridoRepository.AnyPredeterminadoRecorridoAplicacion(nuRecorrido) || uow.RecorridoRepository.AnyPredeterminadoRecorridoAplicacionUsuario(nuRecorrido))
            {
                form.GetField("isEnabled").Disabled = true;
            }

            if (recorrido.EsDefault)
            {
                form.GetField("nombre").Disabled = true;
            }

            this.InicializarSelects(form, uow, recorrido);

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext query)
        {
            using var uow = _uowFactory.GetUnitOfWork();

            uow.CreateTransactionNumber("FormSubmit", _identity.Application, _identity.UserId);

            try
            {
                var nuRecorrido = query.GetParameter("REG700_DETALLES_NU_RECORRIDO").ToNumber<int>();

                var nombre = form.GetField("nombre").Value;
                var descripcion = form.GetField("descripcion").Value;

                var recorrido = uow.RecorridoRepository.GetRecorridoById(nuRecorrido);

                recorrido.Nombre = nombre;
                recorrido.Descripcion = descripcion;
                recorrido.EsHabilitado = form.GetField("isEnabled").IsChecked();
                recorrido.Transaccion = uow.GetTransactionNumber();

                uow.RecorridoRepository.UpdateRecorrido(recorrido);

                uow.SaveChanges();

                query.AddSuccessNotification("REG700_Frm1_Success_Update");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "FormSubmit");
                query.AddErrorNotification("General_Sec0_Error_Operacion");
            }

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            var nuRecorrido = context.GetParameter("REG700_DETALLES_NU_RECORRIDO").ToNumber<int>();

            return this._formValidationService.Validate(new REG700RecorridoFormValidationModule(uow, this._identity, nuRecorrido, isUpdate: true), form, context);
        }

        #region Auxs

        public virtual void InicializarSelects(Form form, IUnitOfWork uow, Recorrido recorrido)
        {
            FormField selectPredio = form.GetField("predio");

            var predio = uow.PredioRepository.GetPredio(recorrido.Predio);

            selectPredio.Options =
            [
                new SelectOption(predio.Numero, $"{predio.Numero} - {predio.Descripcion}")
            ];

            selectPredio.Value = predio.Numero;
            selectPredio.ReadOnly = true;
        }

        #endregion
    }
}
