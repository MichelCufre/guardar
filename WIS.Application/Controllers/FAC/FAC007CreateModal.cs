using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Facturacion;
using WIS.Exceptions;
using WIS.Filtering;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;
using WIS.Session;

namespace WIS.Application.Controllers.FAC
{
    public class FAC007CreateModal : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly ISessionAccessor _session;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly IFormValidationService _formValidationService;
        protected readonly ILogger<FAC007CreateModal> _logger;

        public FAC007CreateModal(
            IUnitOfWorkFactory uowFactory,
            ISessionAccessor session,
            IIdentityService identity,
            ISecurityService security,
            IFilterInterpreter filterInterpreter,
            IFormValidationService formValidationService,
            ILogger<FAC007CreateModal> logger)
        {
            this._uowFactory = uowFactory;
            this._session = session;
            this._identity = identity;
            this._security = security;
            _filterInterpreter = filterInterpreter;
            this._formValidationService = formValidationService;
            this._logger = logger;
        }

        public override Form FormInitialize(Form form, FormInitializeContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            form.GetField("empresa").Options = this.SelectEmpresa(uow, query);
            form.GetField("facturacion").Options = this.SelectFacturacion(uow);
            form.GetField("nuComponente").Options = this.SelectComponente(uow);

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            uow.CreateTransactionNumber(this._identity.Application);

            FacturacionResultado facturacionResultado = this.CrearFacturacionResultado(uow, form, context);

            List<string> facturacionProcesos = uow.FacturacionRepository.GetFacturacionProceso(facturacionResultado.NumeroEjecucion, facturacionResultado.CodigoEmpresa, facturacionResultado.CodigoFacturacion);

            if (facturacionProcesos.Count > 1)
                throw new ValidationFailedException("FAC007_Sec0_error_CantFacturacProceso");

            if (facturacionProcesos.Count == 1)
                facturacionResultado.CodigoProceso = facturacionProcesos.FirstOrDefault();

            RegistroModificacionIngresoResultadosManuales registroModificacionIRM = new RegistroModificacionIngresoResultadosManuales(uow, this._identity.UserId, this._identity.Application);

            registroModificacionIRM.RegistrarFacturacionResultado(facturacionResultado);

            uow.SaveChanges();
            context.AddSuccessNotification("FAC007CreateModal_Sec0_Success_ResultadoAgregado");

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            string numeroEjecucion = context.GetParameter("nuEjecucion");

            return this._formValidationService.Validate(new MantenimientoIngresoResultadosManualesFormValidationModule(uow, this._identity.GetFormatProvider(), numeroEjecucion), form, context);
        }

        public virtual FacturacionResultado CrearFacturacionResultado(IUnitOfWork uow, Form form, FormSubmitContext context)
        {
            FacturacionResultado nuevaFacturacionResultado = new FacturacionResultado();

            string nuEjecucion = context.GetParameter("nuEjecucion");
            string nuComponente = form.GetField("nuComponente").Value;
            string cdFacturacion = form.GetField("facturacion").Value;
            string dsAdicional = form.GetField("dsAdicional").Value;

            nuevaFacturacionResultado.CodigoEmpresa = int.Parse(form.GetField("empresa").Value);
            nuevaFacturacionResultado.NumeroEjecucion = int.Parse(nuEjecucion);
            nuevaFacturacionResultado.DescripcionAdicional = dsAdicional;
            nuevaFacturacionResultado.CodigoSituacion = SituacionDb.CALCULO_EJECUTADO;
            nuevaFacturacionResultado.CodigoFuncAprobacionRechazo = this._identity.UserId;
            nuevaFacturacionResultado.FechaIngresado = DateTime.Now;
            nuevaFacturacionResultado.FechaActualizacion = DateTime.Now;
            nuevaFacturacionResultado.CodigoFacturacion = cdFacturacion;
            nuevaFacturacionResultado.NumeroComponente = nuComponente;
            nuevaFacturacionResultado.PrecioMinimo = 0;
            nuevaFacturacionResultado.PrecioUnitario = 0;
            nuevaFacturacionResultado.NumeroTransaccion = uow.GetTransactionNumber();

            FacturacionCodigoComponente facturacionCodigoComponente = uow.FacturacionRepository.GetFacturacionCodigoComponente(nuComponente, cdFacturacion);
            nuevaFacturacionResultado.NumeroCuentaContable = facturacionCodigoComponente.NumeroCuentaContable;

            string resultado = form.GetField("resultado").Value;
            if (!string.IsNullOrEmpty(resultado))
                nuevaFacturacionResultado.CantidadResultado = decimal.Parse(resultado, this._identity.GetFormatProvider());

            return nuevaFacturacionResultado;
        }

        public virtual List<SelectOption> SelectProceso(IUnitOfWork uow, FormInitializeContext query)
        {
            string numeroEjecucion = query.GetParameter("nuEjecucion");

            return uow.FacturacionRepository.GetFacturacionEjecucionEmpresa(int.Parse(numeroEjecucion))
                .GroupBy(g => g.CodigoProceso).Select(w => new SelectOption(w.Key, w.Key)).ToList();
        }

        public virtual List<SelectOption> SelectEmpresa(IUnitOfWork uow, FormInitializeContext query)
        {
            int nuEjecucion = int.Parse(query.GetParameter("nuEjecucion"));
            return uow.EmpresaRepository.GetEmpresasFacturacionEjecucionParaUsuario(this._identity.UserId, nuEjecucion)
                        .Select(w => new SelectOption(w.Id.ToString(), w.Id.ToString() + " - " + w.Nombre))
                        .ToList();
        }

        public virtual List<SelectOption> SelectFacturacion(IUnitOfWork uow)
        {
            return null;
        }

        public virtual List<SelectOption> SelectComponente(IUnitOfWork uow)
        {
            return null;
        }
    }
}
