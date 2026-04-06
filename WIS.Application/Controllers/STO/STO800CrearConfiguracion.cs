using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules.Stock;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.Domain.StockEntities;
using WIS.Exceptions;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;

namespace WIS.Application.Controllers.STO
{
    public class STO800CrearConfiguracion : AppController
    {
        protected readonly IIdentityService _identity;
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IFormValidationService _formValidationService;
        protected readonly ISecurityService _security;

        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public STO800CrearConfiguracion(IIdentityService identity,
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
            form.GetField("cdEmpresaOrigen").Value = string.Empty;
            form.GetField("flTodaEmpresa").Value = "false";
            form.GetField("flTodoTipoTraspaso").Value = "false";
            form.GetField("flCabezalAuto").Value = "false";
            form.GetField("flReplicaProductos").Value = "false";
            form.GetField("flReplicaCB").Value = "false";
            form.GetField("flCtrlCaractIguales").Value = "false";
            form.GetField("flReplicaAgentes").Value = "false";
            form.GetField("cdTipoDocuIngreso").Value = string.Empty;
            form.GetField("cdTipoDocuEgreso").Value = string.Empty;

            this.InicializarSelects(form);

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new ConfiguracionTraspasoEmpresasValidationModule(uow, this._identity, this._security), form, context);
        }

        public override List<SelectOption> FormSelectSearch(Form form, FormSelectSearchContext context)
        {
            switch (context.FieldId)
            {
                case "cdEmpresaOrigen":
                    return this.SearchEmpresa(form, context);
                default:
                    return new List<SelectOption>();
            }
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            uow.CreateTransactionNumber("STO800 Alta Configuración Traspaso Empresas");
            uow.BeginTransaction();

            try
            {
                var config = new TraspasoEmpresasConfiguracion
                {
                    EmpresaOrigen = int.Parse(form.GetField("cdEmpresaOrigen").Value),
                    TodaEmpresa = bool.Parse(form.GetField("flTodaEmpresa").Value),
                    TodoTipoTraspaso = bool.Parse(form.GetField("flTodoTipoTraspaso").Value),
                    GeneraCabezalAuto = bool.Parse(form.GetField("flCabezalAuto").Value),
                    ReplicaProductos = bool.Parse(form.GetField("flReplicaProductos").Value),
                    ReplicaCodBarras = bool.Parse(form.GetField("flReplicaCB").Value),
                    ControlaCaractIguales = bool.Parse(form.GetField("flCtrlCaractIguales").Value),
                    ReplicaAgentes = bool.Parse(form.GetField("flReplicaAgentes").Value),
                    TipoDocumentoIngreso = form.GetField("cdTipoDocuIngreso").Value,
                    TipoDocumentoEgreso = form.GetField("cdTipoDocuEgreso").Value,
                    FechaAlta = DateTime.Now,
                };

                uow.TraspasoEmpresasRepository.AddConfiguracionTraspaso(config);

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

        public virtual void InicializarSelects(Form form)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var selectTipoDocumentoIngreso = form.GetField("cdTipoDocuIngreso");
            selectTipoDocumentoIngreso.Options = new List<SelectOption>();

            uow.DocumentoTipoRepository.GetTiposDocumentoIngresoPermitenTraspaso()?.ForEach(w =>
            {
                selectTipoDocumentoIngreso.Options.Add(new SelectOption(w.TipoDocumento.ToString(), w.DescripcionTipoDocumento));
            });

            var selectTipoDocumentoEgreso = form.GetField("cdTipoDocuEgreso");
            selectTipoDocumentoEgreso.Options = new List<SelectOption>();

            uow.DocumentoTipoRepository.GetTiposDocumentoEgresoPermitenTraspaso()?.ForEach(w =>
            {
                selectTipoDocumentoEgreso.Options.Add(new SelectOption(w.TipoDocumento, w.DescripcionTipoDocumento));
            });

            var selectEmpresaOrigen = form.GetField("cdEmpresaOrigen");
            var empresa = uow.EmpresaRepository.GetEmpresaUnicaParaUsuario(_identity.UserId);

            if (empresa != null)
            {
                selectEmpresaOrigen.ReadOnly = true;
                selectEmpresaOrigen.Value = empresa.Id.ToString();
                selectEmpresaOrigen.Options = new List<SelectOption> { new SelectOption(selectEmpresaOrigen.Value, empresa.Nombre) };
            }
            else
            {
                selectEmpresaOrigen.Value = string.Empty;
                selectEmpresaOrigen.Options = new List<SelectOption>();
            }
        }

        public virtual List<SelectOption> SearchEmpresa(Form form, FormSelectSearchContext context)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            List<Empresa> empresas = uow.EmpresaRepository.GetByNombreOrCodePartial(context.SearchValue);

            foreach (Empresa empresa in empresas)
            {
                opciones.Add(new SelectOption(empresa.Id.ToString(), $"{empresa.Id} - {empresa.Nombre}"));
            }

            return opciones;
        }

        #endregion
    }
}
