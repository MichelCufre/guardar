using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules.Documento;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Documento;
using WIS.Domain.Documento.Integracion.Egreso;
using WIS.Domain.Documento.Integracion.Recepcion;
using WIS.Domain.General;
using WIS.Domain.Services.Interfaces;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;

namespace WIS.Application.Controllers.DOC
{
    public class DOC100Create : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IFactoryService _factoryService;
        protected readonly IParameterService _parameterService;

        public DOC100Create(IUnitOfWorkFactory uowFactory, IIdentityService identity, ISecurityService security, IFormValidationService formValidationService, IFactoryService factoryService, IParameterService parameterService)
        {
            _uowFactory = uowFactory;
            _identity = identity;
            _security = security;
            _formValidationService = formValidationService;
            _factoryService = factoryService;
            _parameterService = parameterService;
        }

        public override Form FormInitialize(Form form, FormInitializeContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            form.GetField("tpOperativa").Value = string.Empty;
            form.GetField("empresaEgreso").Value = string.Empty;
            form.GetField("empresaIngreso").Value = string.Empty;
            form.GetField("preparacion").Value = string.Empty;

            form.GetField("autoDocIngreso").Value = "true";
            form.GetField("autoDocEgreso").Value = "true";

            var docIngreso = form.GetField("docIngreso");
            docIngreso.Disabled = true;
            docIngreso.ReadOnly = true;

            var docEgreso = form.GetField("docEgreso");
            docEgreso.Disabled = true;
            docEgreso.ReadOnly = true;

            this.InicializarSelectTipoOperativa(uow, form);

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            uow.CreateTransactionNumber(this._identity.Application);
            uow.BeginTransaction();

            try
            {
                CrearAsociacion(uow, form);

                uow.SaveChanges();
                uow.Commit();

                context.AddSuccessNotification("General_Sec0_Error_Er021_SaveSuccess");
            }
            catch (Exception ex)
            {
                uow.Rollback();
                throw ex;
            }
            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new DOC100FormValidationModule(uow, this._identity, this._security, this._parameterService, false), form, context);
        }

        public override List<SelectOption> FormSelectSearch(Form form, FormSelectSearchContext query)
        {
            var tpOperativa = form.GetField("tpOperativa").Value;
            var empresaEgreso = form.GetField("empresaEgreso").Value;
            var empresaIngreso = form.GetField("empresaIngreso").Value;

            if (tpOperativa == TipoOperativaDocumental.Produccion)
                empresaIngreso = empresaEgreso;

            switch (query.FieldId)
            {
                case "empresaEgreso": return this.SearchEmpresa(form, query);
                case "empresaIngreso": return this.SearchEmpresa(form, query);
                case "preparacion": return this.SearchPreparacion(form, query);
                case "docIngreso": return this.SearchDocumentos(empresaIngreso, form, query, TipoOperativaDocumental.GetParamTpDocIngreso(tpOperativa));
                case "docEgreso": return this.SearchDocumentos(empresaEgreso, form, query, TipoOperativaDocumental.GetParamTpDocEgreso(tpOperativa));
            }

            return new List<SelectOption>();
        }

        #region Auxs

        public virtual List<SelectOption> SearchEmpresa(Form form, FormSelectSearchContext context)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                List<Empresa> empresas = uow.EmpresaRepository.GetEmpresasUsuarioDocumentalesByNombreOrCodePartial(context.SearchValue, this._identity.UserId);

                foreach (var empresa in empresas)
                {
                    opciones.Add(new SelectOption(empresa.Id.ToString(), $"{empresa.Id} - {empresa.Nombre}"));
                }
            }

            return opciones;
        }

        public virtual List<SelectOption> SearchPreparacion(Form form, FormSelectSearchContext context)
        {
            var opciones = new List<SelectOption>();
            var empresa = form.GetField("empresaEgreso").Value;
            var tpOperativa = form.GetField("tpOperativa").Value;

            if (string.IsNullOrEmpty(tpOperativa) || string.IsNullOrEmpty(empresa) || !int.TryParse(empresa, out int cdEmpresa))
                return opciones;

            using var uow = this._uowFactory.GetUnitOfWork();

            opciones = uow.PreparacionRepository.GetPreparacionByNumeroODescripcion(context.SearchValue, tpOperativa, cdEmpresa)
                       .Select(w => new SelectOption(w.Id.ToString(), w.Id.ToString() + " - " + w.Descripcion)).ToList();

            return opciones;
        }

        public virtual List<SelectOption> SearchDocumentos(string empresa, Form form, FormSelectSearchContext context, string paramTpDoc)
        {
            var opciones = new List<SelectOption>();

            if (string.IsNullOrEmpty(paramTpDoc) || string.IsNullOrEmpty(empresa) || !int.TryParse(empresa, out int cdEmpresa))
                return opciones;

            using var uow = this._uowFactory.GetUnitOfWork();

            var tpDoc = _parameterService.GetValueByEmpresa(paramTpDoc, cdEmpresa);

            return uow.DocumentoRepository.GetDocumentoById(context.SearchValue, cdEmpresa, tpDoc).Select(w => new SelectOption($"{w[0]}_{w[1]}", $"{w[1]} - {w[2]}")).ToList();
        }

        public virtual void InicializarSelectTipoOperativa(IUnitOfWork uow, Form form)
        {
            var selectTpOperativa = form.GetField("tpOperativa");

            selectTpOperativa.Disabled = false;
            selectTpOperativa.ReadOnly = false;

            List<SelectOption> opciones = uow.DominioRepository.GetDominios("TPOP").Select(w => new SelectOption(w.Valor, w.Valor + " - " + w.Descripcion)).ToList();
            selectTpOperativa.Options = opciones;

            if (opciones != null && opciones.Count == 1)
            {
                selectTpOperativa.Value = opciones.FirstOrDefault().Value;
                selectTpOperativa.Disabled = true;
                selectTpOperativa.ReadOnly = true;
            }
        }

        public virtual void CrearAsociacion(IUnitOfWork uow, Form form)
        {
            var tpOperativa = form.GetField("tpOperativa").Value;
            var empresaEgreso = int.Parse(form.GetField("empresaEgreso").Value);
            var empresaIngreso = -1;

            if (tpOperativa == TipoOperativaDocumental.Produccion)
                empresaIngreso = empresaEgreso;
            else
                empresaIngreso = int.Parse(form.GetField("empresaIngreso").Value);

            var docIngreso = ProcesarIngreso(uow, form, empresaIngreso);
            var docEgreso = ProcesarEgreso(uow, form, empresaEgreso);

            var documentoPreparacion = new DocumentoPreparacion()
            {
                Preparacion = int.Parse(form.GetField("preparacion").Value),
                EmpresaIngreso = empresaIngreso,
                EmpresaEgreso = empresaEgreso,
                TpOperativa = tpOperativa,
                NroDocumentoIngreso = docIngreso.Numero,
                TpDocumentoIngreso = docIngreso.Tipo,
                NroDocumentoEgreso = docEgreso.Numero,
                TpDocumentoEgreso = docEgreso.Tipo,
                Activa = true,
                FechaAlta = DateTime.Now,
                Funcionario = _identity.UserId
            };

            uow.DocumentoRepository.AddDocumentoPreparacion(documentoPreparacion);
        }

        public virtual IDocumentoEgreso ProcesarEgreso(IUnitOfWork uow, Form form, int empresa)
        {
            IDocumentoEgreso docEgreso;
            var nuTransaccion = uow.GetTransactionNumber();
            var autoDocEgreso = bool.Parse(form.GetField("autoDocEgreso").Value);
            var tpOperativa = form.GetField("tpOperativa").Value;
            var paramTpDocEgreso = TipoOperativaDocumental.GetParamTpDocEgreso(tpOperativa);
            var tpDocEgreso = _parameterService.GetValueByEmpresa(paramTpDocEgreso, empresa);

            if (autoDocEgreso)
            {
                var egreso = new EgresoDocumental(this._factoryService);
                docEgreso = egreso.CrearCabezalEgreso(uow, _identity.UserId, empresa, null, tpDocEgreso, null);
                uow.DocumentoRepository.AddEgreso(docEgreso, nuTransaccion);
            }
            else
            {
                var tpNuDocEgreso = form.GetField("docEgreso").Value;
                var nuDocEgreso = tpNuDocEgreso.Substring(tpNuDocEgreso.IndexOf("_") + 1);
                docEgreso = uow.DocumentoRepository.GetEgreso(nuDocEgreso, tpDocEgreso);
                docEgreso.FechaModificacion = DateTime.Now;
                uow.DocumentoRepository.UpdateEgreso(docEgreso, nuTransaccion);
            }

            return docEgreso;
        }

        public virtual IDocumentoIngreso ProcesarIngreso(IUnitOfWork uow, Form form, int empresa)
        {
            IDocumentoIngreso docIngreso;
            var nuTransaccion = uow.GetTransactionNumber();
            var autoDocIngreso = bool.Parse(form.GetField("autoDocIngreso").Value);
            var tpOperativa = form.GetField("tpOperativa").Value;
            var paramTpDocIngreso = TipoOperativaDocumental.GetParamTpDocIngreso(tpOperativa);
            var tpDocIngreso = _parameterService.GetValueByEmpresa(paramTpDocIngreso, empresa);

            if (autoDocIngreso)
            {
                var ingreso = new IngresoDocumental(this._factoryService, this._parameterService, this._identity);
                docIngreso = ingreso.CrearCabezalIngreso(empresa, tpDocIngreso, uow, _identity.UserId);
                uow.DocumentoRepository.AddIngreso(docIngreso, nuTransaccion);
            }
            else
            {
                var tpNuDocIngreso = form.GetField("docIngreso").Value;
                var nuDocIngreso = tpNuDocIngreso.Substring(tpNuDocIngreso.IndexOf("_") + 1);
                docIngreso = uow.DocumentoRepository.GetIngreso(nuDocIngreso, tpDocIngreso);
                docIngreso.FechaModificacion = DateTime.Now;
                uow.DocumentoRepository.UpdateIngreso(docIngreso, nuTransaccion);
            }

            return docIngreso;
        }

        #endregion
    }
}
