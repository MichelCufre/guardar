using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules.Stock;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.StockEntities;
using WIS.Exceptions;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;

namespace WIS.Application.Controllers.STO
{
    public class STO800ModificarConfiguracion : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IFormValidationService _formValidationService;

        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public STO800ModificarConfiguracion(IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IFormValidationService formValidationService,
            ISecurityService security)
        {
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._formValidationService = formValidationService;
            this._security = security;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            if (!long.TryParse(context.GetParameter("idConfig"), out long idConfig))
                throw new ValidationFailedException("STO800_Sec0_Error_ConfigNoValida");

            using var uow = this._uowFactory.GetUnitOfWork();

            var config = uow.TraspasoEmpresasRepository.GetConfiguracionTraspaso(idConfig);
            var empresa = uow.EmpresaRepository.GetEmpresa(config.EmpresaOrigen);

            form.GetField("cdEmpresaOrigen").Value = $"{empresa.Id} - {empresa.Nombre}";
            form.GetField("cdEmpresaOrigen").ReadOnly = true;
            form.GetField("flTodaEmpresa").Value = config.TodaEmpresa.ToString();
            form.GetField("flTodoTipoTraspaso").Value = config.TodoTipoTraspaso.ToString();
            form.GetField("flCabezalAuto").Value = config.GeneraCabezalAuto.ToString();
            form.GetField("flReplicaProductos").Value = config.ReplicaProductos.ToString();
            form.GetField("flReplicaCB").Value = config.ReplicaCodBarras.ToString();
            form.GetField("flCtrlCaractIguales").Value = config.ControlaCaractIguales.ToString();
            form.GetField("flReplicaAgentes").Value = config.ReplicaAgentes.ToString();
            form.GetField("cdTipoDocuIngreso").Value = string.Empty;
            form.GetField("cdTipoDocuEgreso").Value = string.Empty;

            this.InicializarSelects(form, config);

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new ConfiguracionTraspasoEmpresasValidationModule(uow, this._identity, this._security), form, context);
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            uow.CreateTransactionNumber("STO800 Modificación Configuración Traspaso Empresas");
            uow.BeginTransaction();

            try
            {
                if (!long.TryParse(context.GetParameter("idConfig"), out long idConfig))
                    throw new ValidationFailedException("STO800_Sec0_Error_ConfigNoValida");
                else if (!uow.TraspasoEmpresasRepository.AnyConfiguracionTraspaso(idConfig))
                    throw new ValidationFailedException("STO800_Sec0_Error_EmpresaSinConfig");

                var config = uow.TraspasoEmpresasRepository.GetConfiguracionTraspaso(idConfig);

                config.GeneraCabezalAuto = bool.Parse(form.GetField("flCabezalAuto").Value);
                config.ReplicaProductos = bool.Parse(form.GetField("flReplicaProductos").Value);
                config.ReplicaCodBarras = bool.Parse(form.GetField("flReplicaCB").Value);
                config.ControlaCaractIguales = bool.Parse(form.GetField("flCtrlCaractIguales").Value);
                config.ReplicaAgentes = bool.Parse(form.GetField("flReplicaAgentes").Value);
                config.TipoDocumentoIngreso = form.GetField("cdTipoDocuIngreso").Value;
                config.TipoDocumentoEgreso = form.GetField("cdTipoDocuEgreso").Value;

                config.TodaEmpresa = bool.Parse(form.GetField("flTodaEmpresa").Value);
                if (config.TodaEmpresa)
                    uow.TraspasoEmpresasRepository.RemoverEmpresasDestino(idConfig);

                config.TodoTipoTraspaso = bool.Parse(form.GetField("flTodoTipoTraspaso").Value);
                if (config.TodoTipoTraspaso)
                    uow.TraspasoEmpresasRepository.RemoverTiposTraspaso(idConfig);

                config.FechaModificacion = DateTime.Now;

                uow.TraspasoEmpresasRepository.UpdateConfiguracionTraspaso(config);

                uow.SaveChanges();
                uow.Commit();

                context.AddSuccessNotification("General_Sec0_Error_Er021_SaveSuccess");
            }
            catch (ValidationFailedException ex)
            {
                _logger.Error($"Error {ex.Message} - {ex}");
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
                uow.Rollback();
            }
            catch (Exception ex)
            {
                _logger.Error($"Error {ex.Message} - {ex}");
                context.AddErrorNotification(ex.Message);
                uow.Rollback();
            }

            return form;
        }

        #region Metodos Auxiliares 

        public virtual void InicializarSelects(Form form, TraspasoEmpresasConfiguracion config)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var selectTipoDocumentoIngreso = form.GetField("cdTipoDocuIngreso");
            selectTipoDocumentoIngreso.Options = new List<SelectOption>();

            uow.DocumentoTipoRepository.GetTiposDocumentoIngresoPermitenTraspaso()?.ForEach(w =>
            {
                selectTipoDocumentoIngreso.Options.Add(new SelectOption(w.TipoDocumento.ToString(), w.DescripcionTipoDocumento));
            });

            selectTipoDocumentoIngreso.Value = config.TipoDocumentoIngreso;

            var selectTipoDocumentoEgreso = form.GetField("cdTipoDocuEgreso");
            selectTipoDocumentoEgreso.Options = new List<SelectOption>();

            uow.DocumentoTipoRepository.GetTiposDocumentoEgresoPermitenTraspaso()?.ForEach(w =>
            {
                selectTipoDocumentoEgreso.Options.Add(new SelectOption(w.TipoDocumento, w.DescripcionTipoDocumento));
            });

            selectTipoDocumentoEgreso.Value = config.TipoDocumentoEgreso;
        }

        #endregion
    }
}
