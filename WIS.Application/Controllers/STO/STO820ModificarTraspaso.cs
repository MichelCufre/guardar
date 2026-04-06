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
using WIS.Domain.StockEntities.Constants;
using WIS.Exceptions;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Persistence.Database;
using WIS.Security;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace WIS.Application.Controllers.STO
{
    public class STO820ModificarTraspaso : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IFormValidationService _formValidationService;

        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public STO820ModificarTraspaso(IIdentityService identity,
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
            if (!long.TryParse(context.GetParameter("idTraspaso"), out long idTraspaso))
                throw new ValidationFailedException("STO820_Sec0_Error_TraspasoNoValido");

            using var uow = this._uowFactory.GetUnitOfWork();

            var traspaso = uow.TraspasoEmpresasRepository.GetTraspaso(idTraspaso);
            var empresa = uow.EmpresaRepository.GetEmpresa(traspaso.EmpresaOrigen);
            var flFinalizarConPreparacion = form.GetField("flFinalizarConPreparacion");


            form.GetField("idExterno").Value = traspaso.IdExterno.ToString();
            form.GetField("descripcion").Value = traspaso.Descripcion;
            form.GetField("nuDocumentoIngreso").Value = traspaso.DocumentoIngreso;
            form.GetField("nuDocumentoIngreso").ReadOnly = true;
            form.GetField("nuDocumentoEgreso").Value = traspaso.DocumentoEgreso;
            form.GetField("nuDocumentoEgreso").ReadOnly = true;
            form.GetField("tpDocumentoIngreso").Value = traspaso.TipoDocumentoIngreso;
            form.GetField("tpDocumentoIngreso").ReadOnly = true;
            form.GetField("tpDocumentoEgreso").Value = traspaso.TipoDocumentoEgreso;
            form.GetField("tpDocumentoEgreso").ReadOnly = true;
            flFinalizarConPreparacion.Value = traspaso.FinalizarConPreparacion.ToString();
            form.GetField("flPropagarLPN").Value = traspaso.PropagarLPN.ToString();
            form.GetField("flReplicaProductos").Value = traspaso.ReplicaProductos.ToString();
            form.GetField("flReplicaProductos").Disabled = true;
            form.GetField("flReplicaCB").Value = traspaso.ReplicaCodBarras.ToString();
            form.GetField("flReplicaCB").Disabled = true;
            form.GetField("flCtrlCaractIguales").Value = traspaso.ControlaCaractIguales.ToString();
            form.GetField("flCtrlCaractIguales").Disabled = true;
            form.GetField("flReplicaAgentes").Value = traspaso.ReplicaAgentes.ToString();
            form.GetField("flReplicaAgentes").Disabled = true;

            if (traspaso.Estado != TraspasoEmpresasDb.ESTADO_TRASPASO_EN_EDICION)
            {
                form.GetField("cdEmpresaOrigen").ReadOnly = true;
                form.GetField("cdEmpresaDestino").ReadOnly = true;
                form.GetField("tpTraspaso").ReadOnly = true;
                form.GetField("idExterno").ReadOnly = true;
                form.GetField("descripcion").ReadOnly = true;
                form.GetField("flFinalizarConPreparacion").Disabled = true;
                form.GetField("flPropagarLPN").Disabled = true;
            }

            this.InicializarSelects(uow, form, context, traspaso);

            var selectTipoTraspaso = form.GetField("tpTraspaso");
            if (selectTipoTraspaso.Value != TipoTraspasoDb.TraspasoPda)
            {
                flFinalizarConPreparacion.Disabled = true;
                flFinalizarConPreparacion.Value = "false";
            }
            else
            {
                flFinalizarConPreparacion.Disabled = false;
            }

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

            uow.CreateTransactionNumber("STO820 Modificación Traspaso Empresas");
            uow.BeginTransaction();

            try
            {
                if (!long.TryParse(context.GetParameter("idTraspaso"), out long idTraspaso))
                    throw new ValidationFailedException("STO820_Sec0_Error_TraspasoNoValido");
                else if (!uow.TraspasoEmpresasRepository.AnyTraspaso(idTraspaso))
                    throw new ValidationFailedException("STO820_Sec0_Error_TraspasoNoValido");

                var traspaso = uow.TraspasoEmpresasRepository.GetTraspaso(idTraspaso);

                if (context.ButtonId == "btnSubmitUpdateTraspaso")
                {
                    if (traspaso.Estado != TraspasoEmpresasDb.ESTADO_TRASPASO_EN_EDICION)
                        throw new ValidationFailedException("STO820_Sec0_Error_EstadoTraspasoNoValido");

                    traspaso.Descripcion = form.GetField("descripcion").Value;
                    traspaso.IdExterno = form.GetField("idExterno").Value;
                    traspaso.EmpresaOrigen = int.Parse(form.GetField("cdEmpresaOrigen").Value);
                    traspaso.EmpresaDestino = int.Parse(form.GetField("cdEmpresaDestino").Value);
                    traspaso.TipoTraspaso = form.GetField("tpTraspaso").Value;
                    traspaso.FinalizarConPreparacion = bool.Parse(form.GetField("flFinalizarConPreparacion").Value);
                    traspaso.PropagarLPN = bool.Parse(form.GetField("flPropagarLPN").Value);

                    traspaso.FechaModificacion = DateTime.Now;

                    uow.TraspasoEmpresasRepository.UpdateTraspaso(traspaso);

                    uow.SaveChanges();
                    uow.Commit();

                    context.AddSuccessNotification("General_Sec0_Error_Er021_SaveSuccess");
                }

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

        public virtual void InicializarSelects(IUnitOfWork uow, Form form, FormInitializeContext context, TraspasoEmpresas traspaso)
        {
            var empresaOrigen = uow.EmpresaRepository.GetEmpresa(traspaso.EmpresaOrigen);
            var empresaDestino = uow.EmpresaRepository.GetEmpresa(traspaso.EmpresaDestino);

            var selectEmpresaOrigen = form.GetField("cdEmpresaOrigen");
            selectEmpresaOrigen.Options = SearchEmpresa(form, new FormSelectSearchContext()
            {
                SearchValue = empresaOrigen.Id.ToString()
            });
            selectEmpresaOrigen.Value = empresaOrigen.Id.ToString();

            var selectEmpresaDestino = form.GetField("cdEmpresaDestino");
            selectEmpresaDestino.Options = SearchEmpresa(form, new FormSelectSearchContext()
            {
                SearchValue = empresaDestino.Id.ToString()
            });
            selectEmpresaDestino.Value = empresaDestino.Id.ToString();


            var selectTipoTraspaso = form.GetField("tpTraspaso");
            selectTipoTraspaso.Options = new List<SelectOption>();

            var config = uow.TraspasoEmpresasRepository.GetConfiguracionTraspasoByEmpresa(empresaOrigen.Id);

            if (config == null || (config != null && config.TodoTipoTraspaso))
            {
                uow.TraspasoEmpresasRepository.GetTiposTraspaso().ForEach(w =>
                {
                    selectTipoTraspaso.Options.Add(new SelectOption(w.Key, w.Value));
                });
            }
            else
            {
                uow.TraspasoEmpresasRepository.GetTiposTraspaso(config.Id).ForEach(w =>
                {
                    selectTipoTraspaso.Options.Add(new SelectOption(w.Key, w.Value));
                });
            }

            if (config == null)
            {
                selectEmpresaDestino.Disabled = true;
                selectEmpresaDestino.ReadOnly = true;
                selectTipoTraspaso.Disabled = true;
                context.AddInfoNotification("STO820_Sec0_Error_EmpresaNoConfigurada");
            }
            selectTipoTraspaso.Value = traspaso.TipoTraspaso;

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
            return opciones;
        }

        #endregion
    }
}
