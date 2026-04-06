using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules.Stock;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.Eventos;
using WIS.Domain.General;
using WIS.Domain.StockEntities;
using WIS.Domain.StockEntities.Constants;
using WIS.Exceptions;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;

namespace WIS.Application.Controllers.STO
{
    public class STO820CrearTraspaso : AppController
    {
        protected readonly IIdentityService _identity;
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IFormValidationService _formValidationService;
        protected readonly ISecurityService _security;

        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public STO820CrearTraspaso(IIdentityService identity,
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
            form.GetField("idExterno").Value = string.Empty;
            form.GetField("descripcion").Value = string.Empty;
            form.GetField("cdEmpresaOrigen").Value = string.Empty;
            form.GetField("cdEmpresaDestino").Value = string.Empty;
            form.GetField("nuDocumentoIngreso").Value = string.Empty;
            form.GetField("nuDocumentoIngreso").ReadOnly = true;
            form.GetField("nuDocumentoEgreso").Value = string.Empty;
            form.GetField("nuDocumentoEgreso").ReadOnly = true;
            form.GetField("tpDocumentoIngreso").Value = string.Empty;
            form.GetField("tpDocumentoIngreso").ReadOnly = true;
            form.GetField("tpDocumentoEgreso").Value = string.Empty;
            form.GetField("tpDocumentoEgreso").ReadOnly = true;
            form.GetField("tpTraspaso").Value = string.Empty;
            form.GetField("flFinalizarConPreparacion").Value = "false";
            form.GetField("flPropagarLPN").Value = "false";
            form.GetField("flReplicaProductos").Value = "false";
            form.GetField("flReplicaProductos").Disabled = true;
            form.GetField("flReplicaCB").Value = "false";
            form.GetField("flReplicaCB").Disabled = true;
            form.GetField("flCtrlCaractIguales").Value = "false";
            form.GetField("flCtrlCaractIguales").Disabled = true;
            form.GetField("flReplicaAgentes").Value = "false";
            form.GetField("flReplicaAgentes").Disabled = true;

            this.InicializarSelects(form);

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new TraspasoEmpresasValidationModule(uow, this._identity, this._security), form, context);
        }

        public override List<SelectOption> FormSelectSearch(Form form, FormSelectSearchContext context)
        {
            switch (context.FieldId)
            {
                case "cdEmpresaOrigen":
                    return this.SearchEmpresa(form, context);
                case "cdEmpresaDestino":
                    return this.SearchEmpresaDestino(form, context);
                default:
                    return new List<SelectOption>();
            }
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            uow.CreateTransactionNumber("STO820 Alta Traspaso Empresas");
            uow.BeginTransaction();

            try
            {
                var traspaso = new TraspasoEmpresas
                {
                    Descripcion = form.GetField("descripcion").Value,
                    IdExterno = form.GetField("idExterno").Value,
                    EmpresaOrigen = int.Parse(form.GetField("cdEmpresaOrigen").Value),
                    EmpresaDestino = int.Parse(form.GetField("cdEmpresaDestino").Value),
                    TipoTraspaso = form.GetField("tpTraspaso").Value,
                    Estado = TraspasoEmpresasDb.ESTADO_TRASPASO_EN_EDICION,
                    FinalizarConPreparacion = bool.Parse(form.GetField("flFinalizarConPreparacion").Value),
                    PropagarLPN = bool.Parse(form.GetField("flPropagarLPN").Value),
                    ReplicaProductos = bool.Parse(form.GetField("flReplicaProductos").Value),
                    ReplicaCodBarras = bool.Parse(form.GetField("flReplicaCB").Value),
                    ControlaCaractIguales = bool.Parse(form.GetField("flCtrlCaractIguales").Value),
                    ReplicaAgentes = bool.Parse(form.GetField("flReplicaAgentes").Value),
                    FechaAlta = DateTime.Now,
                };

                uow.TraspasoEmpresasRepository.AddTraspaso(traspaso);

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

            var selectEmpresaOrigen = form.GetField("cdEmpresaOrigen");
            var selectEmpresaDestino = form.GetField("cdEmpresaDestino");
            var empresa = uow.EmpresaRepository.GetEmpresaUnicaParaUsuario(_identity.UserId);

            if (empresa != null)
            {
                selectEmpresaOrigen.ReadOnly = true;
                selectEmpresaOrigen.Value = empresa.Id.ToString();
                selectEmpresaOrigen.Options = new List<SelectOption> { new SelectOption(selectEmpresaOrigen.Value, empresa.Nombre) };

                var configuracionEmpresa = uow.TraspasoEmpresasRepository.GetConfiguracionTraspasoByEmpresa(empresa.Id);
                if (configuracionEmpresa.TodaEmpresa)
                {
                    selectEmpresaDestino.Value = empresa.Id.ToString();
                    selectEmpresaDestino.Options = new List<SelectOption> { new SelectOption(selectEmpresaDestino.Value, empresa.Nombre) };
                }
                else
                {
                    selectEmpresaDestino.Options = SearchEmpresaDestino(form, new FormSelectSearchContext()
                    {
                        SearchValue = empresa.Id.ToString()
                    });
                    selectEmpresaDestino.Value = empresa.Id.ToString();
                }

            }
            else
            {
                selectEmpresaOrigen.Value = string.Empty;
                selectEmpresaOrigen.Options = new List<SelectOption>();

                selectEmpresaDestino.Value = string.Empty;
                selectEmpresaDestino.Options = new List<SelectOption>();
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

        public virtual List<SelectOption> SearchEmpresaDestino(Form form, FormSelectSearchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            List<SelectOption> opciones = new List<SelectOption>();
            var empresaOrigen = form.GetField("cdEmpresaOrigen").Value;

            if (int.TryParse(empresaOrigen, out int codEmpresa))
            {
                var configuracionEmpresa = uow.TraspasoEmpresasRepository.GetConfiguracionTraspasoByEmpresa(codEmpresa);
                if (configuracionEmpresa != null)
                {
                    if (configuracionEmpresa.TodaEmpresa)
                    {
                        List<Empresa> empresas = uow.EmpresaRepository.GetByNombreOrCodePartial(context.SearchValue);

                        foreach (Empresa empresa in empresas)
                        {
                            opciones.Add(new SelectOption(empresa.Id.ToString(), $"{empresa.Id} - {empresa.Nombre}"));
                        }
                    }
                    else
                    {
                        List<Empresa> empresas = uow.TraspasoEmpresasRepository.GetEmpresasByConfiguracionEmpresa(configuracionEmpresa.Id, context.SearchValue);

                        foreach (Empresa empresa in empresas)
                        {
                            opciones.Add(new SelectOption(empresa.Id.ToString(), $"{empresa.Id} - {empresa.Nombre}"));
                        }
                    }
                }
            }
            return opciones;
        }

        #endregion
    }
}
