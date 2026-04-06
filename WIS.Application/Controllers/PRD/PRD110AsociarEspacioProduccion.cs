using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules.Produccion;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.Produccion.Interfaces;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;
using WIS.TrafficOfficer;

namespace WIS.Application.Controllers.PRD
{
    public class PRD110AsociarEspacioProduccion : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly ITrafficOfficerService _concurrencyControl;
        protected readonly IFormValidationService _formValidationService;
        protected readonly ILogicaProduccionFactory _logicaProduccionFactory;
        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public PRD110AsociarEspacioProduccion(IIdentityService identity, IUnitOfWorkFactory uowFactory, ITrafficOfficerService concurrencyControl, IFormValidationService formValidationService, ILogicaProduccionFactory logicaProduccionFactory, ISecurityService security)
        {
            _identity = identity;
            _uowFactory = uowFactory;
            _formValidationService = formValidationService;
            _logicaProduccionFactory = logicaProduccionFactory;
            _security = security;
            _concurrencyControl = concurrencyControl;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = _uowFactory.GetUnitOfWork();

            var permissions = _security.CheckPermissions(new List<string> { "PRD110_Section_Access_Editar" });

            var idIngreso = form.GetField("codigo").Value;
            var ingreso = uow.IngresoProduccionRepository.GetIngresoByIdConDetalles(idIngreso);

            form.GetField("idExterno").Value = ingreso.IdProduccionExterno;

            if (ingreso.EspacioProduccion != null)
            {
                form.GetField("espacio").Value = ingreso.EspacioProduccion.Id;
                form.GetField("espacio").Options = new List<SelectOption> { new SelectOption(ingreso.EspacioProduccion.Id, ingreso.EspacioProduccion.Descripcion) };
            }

            form.Buttons.ForEach(b => b.Disabled = !permissions["PRD110_Section_Access_Editar"]);

            context.AddParameter("hasPermission", permissions["PRD110_Section_Access_Editar"] ? "S" : "N");

            this.InicializarSelects(form);

            return form;
        }

        protected virtual void InicializarSelects(Form form)
        {
            var selectEspacio = form.GetField("espacio");
            var nuIngreso = form.GetField("codigo").Value;

            selectEspacio.Options = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            var ingreso = uow.ProduccionRepository.GetIngreso(nuIngreso);
            var espacios = uow.EspacioProduccionRepository.GetAllEspaciosProduccionByPredio(ingreso.Predio);

            if (espacios != null)
            {
                foreach (var espacio in espacios.OrderBy(e => e.Descripcion))
                {
                    selectEspacio.Options.Add(new SelectOption(espacio.Id, $"{espacio.Id} - {espacio.Descripcion}"));
                }
            }

        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = _uowFactory.GetUnitOfWork();

            return _formValidationService.Validate(new PRD110AsociarEspacioFormValidationModule(uow, _identity), form, context);
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = _uowFactory.GetUnitOfWork();

            var idIngreso = form.GetField("codigo").Value;
            var idEspacio = form.GetField("espacio").Value;

            if (_concurrencyControl.IsLocked("T_PRDC_INGRESO", idIngreso, true))
            {
                context.AddErrorNotification("General_msg_Error_ProduccionBloqueada");
                return form;
            }

            if (uow.IngresoProduccionRepository.AnyInsumosReales(idIngreso))
            {
                context.AddErrorNotification("PRD110_Sec0_Error_ProduccionConReservas");
                return form;
            }

            if (uow.EspacioProduccionRepository.AnyContenedorAsociado(idIngreso))
            {
                context.AddErrorNotification("PRD110_Sec0_Error_EspacioAsociado");
                return form;
            }

            var transaction = _concurrencyControl.CreateTransaccion();

            try
            {
                _concurrencyControl.AddLock("T_PRDC_INGRESO", idIngreso, transaction, true);

                var logicaProduccion = _logicaProduccionFactory.GetLogicaProduccion(uow, idIngreso);

                uow.BeginTransaction();
                uow.CreateTransactionNumber("Asociar Linea a Producción");

                logicaProduccion.AsociarEspacioProduccion(idEspacio);
                context.AddSuccessNotification("PRD100_form1_Msg_EspacioAsociado");

                uow.SaveChanges();
                uow.Commit();
            }
            catch (Exception ex)
            {
                _concurrencyControl.DeleteTransaccion(transaction);
                _logger.Error(ex, ex.Message);
                throw;
            }
            finally
            {
                _concurrencyControl.DeleteTransaccion(transaction);
            }

            return form;
        }

    }
}
