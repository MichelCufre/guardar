using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Documento.Integracion.Egreso;
using WIS.Domain.Expedicion;
using WIS.Exceptions;
using WIS.Filtering;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.GridComponent.Build;
using WIS.GridComponent.Excel;
using WIS.Security;
using WIS.Session;

namespace WIS.Application.Controllers.EXP
{
    public class EXP052 : AppController
    {
        protected readonly ISessionAccessor _session;
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IFactoryService _factoryService;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<EXP052> _logger;

        protected List<string> GridKeys { get; }

        public EXP052(
            ISessionAccessor session,
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IFormValidationService formValidationService,
            IFactoryService factoryService,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter,
            ILogger<EXP052> logger)
        {
            this._session = session;
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._formValidationService = formValidationService;
            this._factoryService = factoryService;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
            this._logger = logger;

            this.GridKeys = new List<string>
            {
                "CD_CAMION",
                "CD_PRODUTO",
                "CD_EMPRESA",
                "CD_CLIENTE",
                "NU_IDENTIFICADOR",
                "CD_FAIXA"
            };
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            int idCamion = GetIdCamion(context);

            if (uow.CamionRepository.AnyCamion(idCamion))
            {
                var camionDescripcion = uow.CamionRepository.GetCamionDescripcion(idCamion);

                context.AddParameter("EXP052_RESPETA_ORDEN", GetRespetaOrden(context));

                context.AddParameter("EXP052_CD_CAMION", idCamion.ToString());
                context.AddParameter("EXP052_PLACA", camionDescripcion.Matricula);
                context.AddParameter("EXP052_SITUACION", $"{(short)camionDescripcion.Estado} - {camionDescripcion.DescSituacion}");
                context.AddParameter("EXP052_DT_INGRESO", camionDescripcion.FechaCreacion?.ToString(CDateFormats.DATE_ONLY));

                context.AddParameter("EXP052_RUTA", camionDescripcion.Ruta == null ? string.Empty : $"{camionDescripcion.Ruta} - {camionDescripcion.DescRuta}");
                context.AddParameter("EXP052_PUERTA", camionDescripcion.Puerta == null ? string.Empty : $"{camionDescripcion.Puerta} - {camionDescripcion.DescPuerta}");
                context.AddParameter("EXP052_EMPRESA", camionDescripcion.Empresa == null ? string.Empty : $"{camionDescripcion.Empresa} - {camionDescripcion.DescEmpresa}");

                var documentoEgreso = uow.DocumentoRepository.GetEgresoPorCamion(idCamion);

                if (documentoEgreso != null)
                {
                    form.GetField("tpEgreso").Value = documentoEgreso.Tipo;
                    form.GetField("nroDoc").Value = documentoEgreso.Numero;
                    form.GetField("nroDocNoGenerado").Value = documentoEgreso.Numero;

                    form.GetField("tpEgreso").ReadOnly = true;
                    form.GetField("nroDocNoGenerado").ReadOnly = true;
                }
            }
            else
            {
                throw new MissingParameterException("EXP052_Sec0_error_NoExisteCamion", new string[] { idCamion.ToString() });
            }

            this.InicializarSelects(form);

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            int idCamion = GetIdCamion(context);
            var tpEgreso = form.GetField("tpEgreso").Value;
            var nroDocNoGenerado = form.GetField("nroDocNoGenerado").Value;
            var egreso = new EgresoDocumental(this._factoryService);

            try
            {
                using (var uow = this._uowFactory.GetUnitOfWork())
                {
                    uow.CreateTransactionNumber(this._identity.Application);
                    uow.BeginTransaction();

                    var documento = egreso.GenerarEgresoDocumental(uow, this._identity.UserId, idCamion, tpEgreso, nroDocNoGenerado);

                    uow.SaveChanges();
                    uow.Commit();

                    context.Redirect("/expedicion/EXP040", new List<ComponentParameter>());
                    context.AddSuccessNotification("EXP052_Sec0_Success_EgresoGenerado", new List<string> { documento.Numero, documento.Tipo });
                }
            }
            catch (ValidationFailedException ex)
            {
                this._logger.LogError(ex, ex.Message);
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, ex.Message);
                context.AddErrorNotification("General_Sec0_Error_Operacion");
            }

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var editMode = uow.DocumentoRepository.GetEgresoPorCamion(GetIdCamion(context)) != null;

                this._formValidationService.Validate(new EXP052FormValidationModule(editMode, uow, this._session, this._identity), form, context);

                return form;
            }
        }

        #region Metodos Auxiliares

        public virtual void InicializarSelects(Form form)
        {
            //Inicializar selects
            FormField selectEgreso = form.GetField("tpEgreso");

            selectEgreso.Options = new List<SelectOption>();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var documentosEgreso = uow.DocumentoTipoRepository.GetDocumentosEgresoHabilitados();
                foreach (var documentoHabilitado in documentosEgreso)
                {
                    selectEgreso.Options.Add(new SelectOption(documentoHabilitado.TipoDocumento, documentoHabilitado.DescripcionTipoDocumento));
                }
            }
        }

        public virtual int GetIdCamion(ComponentContext context)
        {
            if (!int.TryParse(GetParamValue(context, "camion"), out int idCamion))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            return idCamion;
        }

        public virtual string GetRespetaOrden(ComponentContext context)
        {
            return GetParamValue(context, "respetaOrden")?.Trim()?.ToUpper();
        }

        public virtual string GetParamValue(ComponentContext context, string paramId)
        {
            if (context.Parameters.Count == 0)
                throw new MissingParameterException("General_Sec0_Error_Sin_ParametrosURI");

            return context.Parameters.FirstOrDefault(x => x.Id == paramId)?.Value;
        }

        #endregion
    }
}
