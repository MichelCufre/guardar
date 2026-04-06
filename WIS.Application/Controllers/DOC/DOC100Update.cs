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
using WIS.Exceptions;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;

namespace WIS.Application.Controllers.DOC
{
    public class DOC100Update : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IFactoryService _factoryService;
        protected readonly IParameterService _parameterService;

        public DOC100Update(IUnitOfWorkFactory uowFactory, IIdentityService identity, ISecurityService security, IFormValidationService formValidationService, IFactoryService factoryService, IParameterService parameterService)
        {
            _uowFactory = uowFactory;
            _identity = identity;
            _security = security;
            _formValidationService = formValidationService;
            _factoryService = factoryService;
            _parameterService = parameterService;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            try
            {
                string idDocPrep = context.GetParameter("nroDocPrep");
                if (!int.TryParse(idDocPrep, out int nroDocPrep))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                var docPrep = uow.DocumentoRepository.GetDocumentoPreparacion(nroDocPrep);
                if (docPrep == null)
                    throw new ValidationFailedException("DOC100_Sec0_Error_NoExisteAsociacion", new string[] { idDocPrep });

                InicializarSelectTipoOperativa(uow, form, docPrep.TpOperativa);

                InicializarSelectEmpresa("empresaEgreso", form, docPrep.EmpresaIngreso.ToString());
                InicializarSelectEmpresa("empresaIngreso", form, docPrep.EmpresaIngreso.ToString());
                InicializarSelectPreparacion(uow, form, docPrep.Preparacion);

                InicializarSelectDocumentos(form, docPrep.NroDocumentoIngreso, true);
                InicializarSelectDocumentos(form, docPrep.NroDocumentoEgreso, false);

                form.GetField("autoDocIngreso").Value = "false";
                form.GetField("autoDocEgreso").Value = "false";

                context.AddParameter("nroDocPrep", docPrep.NroDocumentoPreparacion.ToString());
            }
            catch (ValidationFailedException ex)
            {
                if (!string.IsNullOrEmpty(ex.Message))
                    context.AddErrorNotification(ex.Message, new List<string>(ex.StrArguments ?? new string[0]));
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(ex.Message))
                    context.AddErrorNotification(ex.Message);
            }

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            uow.CreateTransactionNumber(this._identity.Application);
            uow.BeginTransaction();

            try
            {
                ModificarAsociacion(uow, form, context);

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
            return this._formValidationService.Validate(new DOC100FormValidationModule(uow, this._identity, this._security, this._parameterService, true), form, context);
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

        public virtual void ModificarAsociacion(IUnitOfWork uow, Form form, FormSubmitContext context)
        {
            var docPrep = uow.DocumentoRepository.GetDocumentoPreparacion(int.Parse(context.GetParameter("nroDocPrep")));
            var docIngreso = ProcesarIngreso(uow, form, docPrep.EmpresaIngreso);
            var docEgreso = ProcesarEgreso(uow, form, docPrep.EmpresaEgreso);

            docPrep.TpOperativa = form.GetField("tpOperativa").Value;
            docPrep.NroDocumentoIngreso = docIngreso.Numero;
            docPrep.TpDocumentoIngreso = docIngreso.Tipo;
            docPrep.NroDocumentoEgreso = docEgreso.Numero;
            docPrep.TpDocumentoEgreso = docEgreso.Tipo;
            docPrep.FechaModificacion = DateTime.Now;

            uow.DocumentoRepository.UpdateDocumentoPreparacion(docPrep);
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

            var tpDoc = _parameterService.GetValueByEmpresa(paramTpDoc, cdEmpresa);

            using var uow = this._uowFactory.GetUnitOfWork();

            return uow.DocumentoRepository.GetDocumentoById(context.SearchValue, cdEmpresa, tpDoc).Select(w => new SelectOption($"{w[0]}_{w[1]}", $"{w[1]} - {w[2]}")).ToList();
        }

        public virtual void InicializarSelectTipoOperativa(IUnitOfWork uow, Form form, string value)
        {
            var selectTpOperativa = form.GetField("tpOperativa");
            selectTpOperativa.Options = uow.DominioRepository.GetDominios("TPOP").Select(w => new SelectOption(w.Valor, w.Valor + " - " + w.Descripcion)).ToList();
            selectTpOperativa.Value = value;
            selectTpOperativa.Disabled = true;
            selectTpOperativa.ReadOnly = true;
        }

        public virtual void InicializarSelectEmpresa(string fieldId, Form form, string value)
        {
            var fieldEmpresa = form.GetField(fieldId);

            fieldEmpresa.Options = this.SearchEmpresa(form, new FormSelectSearchContext()
            {
                SearchValue = value
            });

            fieldEmpresa.Value = value;
            fieldEmpresa.Disabled = true;
            fieldEmpresa.ReadOnly = true;
        }

        public virtual void InicializarSelectPreparacion(IUnitOfWork uow, Form form, int nuPrep)
        {
            var fieldPreparacion = form.GetField("preparacion");
            var prep = uow.PreparacionRepository.GetPreparacionPorNumero(nuPrep);

            fieldPreparacion.Options = new List<SelectOption>() { new SelectOption(prep.Id.ToString(), $"{prep.Id} - {prep.Descripcion}") };
            fieldPreparacion.Value = prep.Id.ToString();

            fieldPreparacion.Disabled = true;
            fieldPreparacion.ReadOnly = true;
        }

        public virtual void InicializarSelectDocumentos(Form form, string value, bool ingreso)
        {
            var tpOperativa = form.GetField("tpOperativa").Value;
            var fieldId = "docIngreso";
            var empresa = form.GetField("empresaIngreso").Value;
            var paramTpDoc = TipoOperativaDocumental.GetParamTpDocIngreso(tpOperativa);

            if (!ingreso)
            {
                fieldId = "docEgreso";
                empresa = form.GetField("empresaEgreso").Value;
                paramTpDoc = TipoOperativaDocumental.GetParamTpDocEgreso(tpOperativa);
            }

            var field = form.GetField(fieldId);

            field.Options = this.SearchDocumentos(empresa, form, new FormSelectSearchContext()
            {
                SearchValue = value
            }, paramTpDoc);

            field.Value = value;
        }

        #endregion
    }
}
