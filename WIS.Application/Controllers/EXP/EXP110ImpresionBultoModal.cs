using Newtonsoft.Json;
using NLog;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules.Expedicion;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Expedicion;
using WIS.Domain.Expedicion.EXP110EmpaquetadoPicking;
using WIS.Domain.Expedicion.EXP110EmpaquetadoPicking.Dto;
using WIS.Domain.General.Auxiliares;
using WIS.Domain.Services.Interfaces;
using WIS.Extension;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;

namespace WIS.Application.Controllers.EXP
{
    public class EXP110ImpresionBultoModal : AppController
    {
        protected readonly Logger Logger = LogManager.GetCurrentClassLogger();

        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IFormValidationService _formValidationService;
        protected readonly ITrackingService _trackingService;
        protected readonly IBarcodeService _barcodeService;

        protected readonly EmpaquetadoPickingLogic _logic;

        public EXP110ImpresionBultoModal(
            ISecurityService security,
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IFormValidationService formValidationService,
            IPrintingService printingService,
            ITrackingService trackingService,
            IBarcodeService barcodeService)
        {
            _security = security;
            _uowFactory = uowFactory;
            _identity = identity;
            _formValidationService = formValidationService;
            _trackingService = trackingService;
            _barcodeService = barcodeService;

            _logic = new EmpaquetadoPickingLogic(printingService, trackingService, barcodeService, identity, Logger);
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new EtiquetasEmpaquetadoPickingQuery();
            uow.HandleQuery(dbQuery);

            var estilo = dbQuery.GetEtiquetasEstilo().FirstOrDefault(x => x.Id == "WIS-CONH");

            form.GetField("nuContenedorBulto").Value = "";
            form.GetField("cdEstiloBulto").Value = string.Format("{0} - {1}", estilo.Id, estilo.Descripcion);
            form.GetField("cdEstiloBulto").ReadOnly = true;
            form.GetField("ImpimirPrimerBulto").Value = "S";

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            switch (context.ButtonId)
            {
                case "btnSubmitGuardar":
                    return SubmitBtnGuardar(form, context);
                case "btnSubmitImprimir":
                    return SubmitBtnSubmitImprimir(form, context);
                case "nuContenedorBulto":
                    return SubmitContenedorBulto(form, context);
                default: return form;
            }
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new EXP110ImpresionBultosValidationModule(uow, _identity, _barcodeService), form, context);
        }

        #region Metodos Auxiliares

        public virtual Form SubmitBtnSubmitImprimir(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.CreateTransactionNumber("EXP110ImpresionBulto SubmitBtnSubmitImprimir");
            uow.BeginTransaction();

            var confInicial = context.GetParameter("CONF_INICIAL");

            if (string.IsNullOrEmpty(form.GetField("nuContenedorBulto").Value) ||
                string.IsNullOrEmpty(confInicial))
            {
                context.AddWarningNotification("EXP110ImpBultoModal_frm1_Msg_NoEsPosibleImprimir");
                return form;
            }

            var qtBultos = form.GetField("cantidadBulto").Value;
            if (string.IsNullOrEmpty(qtBultos) || qtBultos.ToNumber<int>() <= 0)
            {
                context.AddErrorNotification("EXP110ImpBultoModal_frm1_Msg_CantidadBultoMayorCero");
                return form;
            }

            var contenedorBultoJSON = context.GetParameter("AUX_DATOS_CONT_BULTO");

            if (string.IsNullOrEmpty(contenedorBultoJSON))
            {
                context.AddErrorNotification("EXP110ImpresionBulto_form1_Error_ContenedorDataNoExiste");
                return form;
            }

            var datosContenedorBulto = JsonConvert.DeserializeObject<DatosContenedorBulto>(contenedorBultoJSON);

            var barrasContenedor = form.GetField("nuContenedorBulto").Value;
            var memo = form.GetField("comentarios").Value;
            var cantidadBulto = form.GetField("cantidadBulto").Value;
            var imprimePrimerBulto = form.GetField("ImpimirPrimerBulto").IsChecked();

            var contenedor = uow.ContenedorRepository.GetContenedor(datosContenedorBulto.NumeroPreparacion, datosContenedorBulto.NumeroContenedor);

            if (contenedor == null)
            {
                context.AddErrorNotification("EXP110ImpresionBulto_form1_Error_ContenedorNoExiste");
                return form;
            }

            contenedor.CantidadBulto = cantidadBulto.ToNumber<int>();
            contenedor.NumeroTransaccion = uow.GetTransactionNumber();

            uow.ContenedorRepository.UpdateContenedor(contenedor);
            uow.SaveChanges();

            var cdCliente = datosContenedorBulto.CodigoCliente;
            var empresa = datosContenedorBulto.Empresa;
            var nuPedido = datosContenedorBulto.NumeroPedido;

            _logic.ImprimirEtiquetaBulto(uow, _identity.UserId, confInicial, contenedor, cdCliente, empresa, nuPedido, imprimePrimerBulto, memo);

            if (uow.ParametroRepository.GetParameter("MODAL_IMPRESION_EMPAQUETADO") == "S")
                context.AddOrUpdateParameter("AUX_IMP_RESUMEN", "S");

            context.AddSuccessNotification("EXP110ImpBultoModal_frm1_Msg_SeEnvioImprimir");
            uow.SaveChanges();

            return form;
        }

        public virtual Form SubmitContenedorBulto(Form form, FormSubmitContext context)
        {
            var confInicial = context.GetParameter("CONF_INICIAL");
            using var uow = this._uowFactory.GetUnitOfWork();

            var nuContBulto = form.GetField("nuContenedorBulto").Value;
            if (string.IsNullOrEmpty(nuContBulto) || string.IsNullOrEmpty(confInicial))
            {
                context.AddWarningNotification("EXP110ImpBultoModal_frm1_Msg_NoEsPosibleImprimir");
                return form;
            }

            form.GetField("nuContenedorBulto").ReadOnly = true;

            _barcodeService.ValidarEtiquetaContenedor(nuContBulto, _identity.UserId, out AuxContenedor datosContenedor, out int cantidadEmpresa);

            var datosContenedorBulto = uow.EmpaquetadoPickingRepository.GetDatosContenedorBulto(datosContenedor.NuPreparacion, datosContenedor.NuContenedor);

            form.GetField("nuPreparacionBulto").Value = datosContenedor.NuPreparacion.ToString();
            form.GetField("cantidadBulto").Value = datosContenedorBulto.CantidadBultos.ToString();
            form.GetField("comentarios").Value = datosContenedorBulto?.DescripcionMemo?.ToString();

            context.AddOrUpdateParameter("AUX_DATOS_CONT_BULTO", JsonConvert.SerializeObject(datosContenedorBulto));

            return form;
        }

        public virtual Form SubmitBtnGuardar(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var barrasContenedor = form.GetField("nuContenedorBulto").Value;
            var cantidadBulto = form.GetField("cantidadBulto").Value;

            uow.CreateTransactionNumber("EXP110ImpresionBulto SubmitBtnGuardar");
            uow.BeginTransaction();

            if (!string.IsNullOrEmpty(barrasContenedor) && !string.IsNullOrEmpty(cantidadBulto))
            {
                _barcodeService.ValidarEtiquetaContenedor(barrasContenedor, _identity.UserId, out AuxContenedor datosContenedor, out int cantidadEmpresa);

                var contenedor = uow.ContenedorRepository.GetContenedor(datosContenedor.NuPreparacion, datosContenedor.NuContenedor);

                contenedor.CantidadBulto = cantidadBulto.ToNumber<int>();
                contenedor.NumeroTransaccion = uow.GetTransactionNumber();

                uow.ContenedorRepository.UpdateContenedor(contenedor);

                uow.SaveChanges();

                context.AddSuccessNotification("EXP110ImpBultoModal_frm1_Msg_CambiosGuardados");
            }
            else
            {
                context.AddInfoNotification("EXP110ImpBultoModal_frm1_Msg_NoHayCambios");
            }

            uow.Commit();
            return form;
        }

        #endregion
    }
}
