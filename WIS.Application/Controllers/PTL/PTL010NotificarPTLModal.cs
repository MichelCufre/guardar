using Newtonsoft.Json;
using System;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules.Ptl;
using WIS.Domain.Automatismo;
using WIS.Domain.Automatismo.Constants;
using WIS.Domain.Automatismo.Dtos;
using WIS.Domain.Automatismo.Logic;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.Domain.General.Auxiliares;
using WIS.Domain.General.Enums;
using WIS.Domain.Services.Interfaces;
using WIS.Exceptions;
using WIS.Extension;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;
using WIS.Session;
using WIS.TrafficOfficer;

namespace WIS.Application.Controllers.PTL
{
    public class PTL010NotificarPTLModal : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly ISecurityService _security;
        protected readonly IIdentityService _identity;
        protected readonly ISessionAccessor _session;
        protected readonly ITrafficOfficerService _concurrencyControl;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IAutomatismoPtlClientService _automatismoPtlClientService;
        protected readonly IBarcodeService _barcodeService;
        protected readonly PtlLogic _logicPtl;

        public PTL010NotificarPTLModal(
          ISecurityService security,
          IUnitOfWorkFactory uowFactory,
          IIdentityService identity,
          IFormValidationService formValidationService,
          IAutomatismoPtlClientService automatismoPtlClientService,
          ITrafficOfficerService concurrencyControl,
          ISessionAccessor session,
          IBarcodeService barcodeService)
        {
            _security = security;
            _uowFactory = uowFactory;
            _identity = identity;
            _formValidationService = formValidationService;
            _automatismoPtlClientService = automatismoPtlClientService;
            _concurrencyControl = concurrencyControl;
            _session = session;
            _logicPtl = new PtlLogic(barcodeService, identity);
            _barcodeService = barcodeService;
        }


        #region Form functions
        public override Form FormInitialize(Form form, FormInitializeContext context)
        {

            return form;
        }
        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            switch (context.ButtonId)
            {
                case "SubmitContenedor": return SubmitContenedor(form, context);
                case "btnSubmitIniciar": return SubmitIniciar(form, context);
            }

            return form;
        }
        public override Form FormButtonAction(Form form, FormButtonActionContext context)
        {
            if (context.ButtonId == "UserConfirmation")
            {
                string userIdConfirmado = context.GetParameter("userId");

                if (string.IsNullOrEmpty(userIdConfirmado))
                {
                    context.AddErrorNotification("PTL010_form1_Error_ParametroUserIdVacio");
                    return form;
                }

                string numeroAutomatismo = context.GetParameter("numeroAutomatismo");

                if (string.IsNullOrEmpty(numeroAutomatismo))
                {
                    context.AddErrorNotification("PTL010_form1_Error_ParametroNumeroAutomatismoVacio");
                    return form;
                }

                using var uow = this._uowFactory.GetUnitOfWork();

                var automatismo = uow.AutomatismoRepository.GetAutomatismoById(numeroAutomatismo.ToNumber<int>());

                (ValidationsResult validationResult, PtlColorResponse response) = _automatismoPtlClientService.GetPtlColor(int.Parse(userIdConfirmado), automatismo.ZonaUbicacion);

                if (validationResult != null && validationResult.Errors.Count() == 0)
                {
                    context.AddOrUpdateParameter("PTL010_USER_COLOR", response.Css);
                    context.AddOrUpdateParameter("PTL010_USER_COLOR_CODE", response.Code);
                }
                else
                    validationResult.Errors.ForEach(x => { context.AddErrorNotification(string.Join('.', x.Messages)); });
            }
            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new PTL010NotificarPTLModalValidationModule(uow, _identity, _session, _concurrencyControl, _barcodeService), form, context);
        }
        #endregion

        #region Metodos Auxiliares
        public virtual Form SubmitContenedor(Form form, FormSubmitContext context)
        {
            context.AddOrUpdateParameter("SOLICITAR_CREDENCIALES_USUARIO", "S");

            return form;
        }

        public virtual Form SubmitIniciar(Form form, FormSubmitContext context)
        {
            TrafficOfficerTransaction transaction = null;

            try
            {
                string vlComparteContenedorRow = context.GetParameter("GRID_VL_COMP_CONT_PICK_PROD_ROW_SELECTED");
                string subClaseRow = context.GetParameter("GRID_SUBCLASE_PROD_ROW_SELECTED");
                string userIdConfirmado = context.GetParameter("userId");

                if (string.IsNullOrEmpty(vlComparteContenedorRow))
                {
                    context.AddErrorNotification("PTL010_form1_Error_ParametroVlComparteContenedorPickingSelected");
                    return form;
                }

                if (string.IsNullOrEmpty(subClaseRow))
                {
                    context.AddErrorNotification("PTL010_form1_Error_ParametroSubClaseSelected");
                    return form;
                }

                if (string.IsNullOrEmpty(userIdConfirmado))
                {
                    context.AddErrorNotification("PTL010_form1_Error_ParametroUserIdVacio");
                    return form;
                }

                using var uow = this._uowFactory.GetUnitOfWork();

                var vlComparteContenedor = context.GetParameter("GRID_VL_COMP_CONT_PICK_PROD_ROW_SELECTED").Split('$')[3];
                var subClase = context.GetParameter("GRID_SUBCLASE_PROD_ROW_SELECTED").Split('$')[3];
                var codigoBarras = form.GetField("contenedor").Value;
                int preparacion = context.GetParameter("preparacion").ToNumber<int>();
                int empresa = context.GetParameter("empresa").ToNumber<int>();
                string codigoCliente = context.GetParameter("cliente");

                //Las lineas de picking pendientes de la agrupacion ninguna puede tener lote auto 
                if (_logicPtl.AnyPickingPendienteConLoteAuto(uow, preparacion, vlComparteContenedor, subClase))
                {
                    context.AddInfoNotification("PTL010_form1_Error_ExistenLineasConLoteAUTO");
                    return form;
                }

                uow.CreateTransactionNumber("IniciarPickToLight");
                uow.BeginTransaction();

                _barcodeService.ValidarEtiquetaContenedor(codigoBarras, _identity.UserId, out AuxContenedor datosContenedor, out int cantidadEmpresa);

                bool creoContenedor = datosContenedor.NuPreparacion == -1 && (datosContenedor.Estado != EstadoContenedor.EnCamion || datosContenedor.Estado != EstadoContenedor.EnPreparacion);

                transaction = _concurrencyControl.CreateTransaccion();

                (var result, string response) = IniciarPickToLight(uow, context, codigoBarras, preparacion, codigoCliente, subClase, vlComparteContenedor, empresa, creoContenedor, userIdConfirmado.ToNumber<int>(), transaction);

                uow.SaveChanges();
                uow.Commit();

                if (result.HasError())
                {
                    result.Errors.ForEach(x => { context.AddErrorNotification(string.Join('.', x.Messages)); });
                }
                else
                    context.AddSuccessNotification("PTL010_form1_Info_Success");

            }
            catch (ValidationFailedException ex)
            {
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());

            }
            catch (Exception ex)
            {

                context.AddErrorNotification(ex.Message);
            }
            finally
            {
                if (transaction != null)
                {
                    _concurrencyControl.DeleteTransaccion(transaction);
                }
            }

            return form;
        }

        public virtual (ValidationsResult, string) IniciarPickToLight(IUnitOfWork uow, FormSubmitContext context, string codigoBarras, int preparacion, string codigoCliente, string subClase, string vlComparteContenedorPicking, int empresa, bool crearContenedor, int userIdConfirmado, TrafficOfficerTransaction transaction)
        {
            Contenedor contenedor = null;

            _barcodeService.ValidarEtiquetaContenedor(codigoBarras, _identity.UserId, out AuxContenedor auxContenedor, out int cantidadEmpresa);

            var numeroContenedor = auxContenedor.NuContenedor;
            var tipoContenedor = auxContenedor.TipoContenedor;

            if (crearContenedor)
            {
                string ubicacion = uow.EquipoRepository.GetEquipo(uow.FuncionarioRepository.GetFuncionario(userIdConfirmado).Equipo ?? 0).CodigoUbicacion;
                contenedor = _logicPtl.CrearContenedor(uow, numeroContenedor, preparacion, ubicacion, subClase, tipoContenedor ?? "W");
                uow.SaveChanges();
            }
            else
            {
                contenedor = uow.ContenedorRepository.GetContenedor(preparacion, numeroContenedor);
                var cliente = context.GetParameter("cliente");
                if (!_logicPtl.EsCompatibleContenedorConAgrupaciones(uow, preparacion, contenedor.Numero, vlComparteContenedorPicking, subClase, cliente))
                    throw new ValidationFailedException("PTL010_form1_Error_ContenedorNoCompatibleAgrupaciones");


                if (_logicPtl.TieneMasDeUnConjuntoDeAgrupacionContenedor(uow, preparacion, contenedor.Numero))
                    throw new ValidationFailedException("PTL010_form1_Error_ContenedorTieneMasAgrupacion");

                if (subClase != (contenedor.CodigoSubClase ?? ""))
                    throw new ValidationFailedException("PTL010_form1_Error_ContenedorLeidoMismaSubClase");

            }
            string numeroAutomatismo = context.GetParameter("numeroAutomatismo");
            string ptlColorCode = context.GetParameter("PTL010_USER_COLOR_CODE");



            if (string.IsNullOrEmpty(numeroAutomatismo))
                throw new ValidationFailedException("PTL010_form1_Error_ParametroNumeroAutomatismoVacio");

            if (string.IsNullOrEmpty(ptlColorCode))
                throw new ValidationFailedException("PTL010_form1_Error_ParametroPtlColorVacio");

            var automatismo = uow.AutomatismoRepository.GetAutomatismoById(numeroAutomatismo.ToNumber<int>());

            (string entityToLock, string keySinUsuario) = _logicPtl.CrearKeyEntidadBloqueo(PtlDb.APLICACION_BLOQUEO, userIdConfirmado, subClase, codigoCliente, preparacion, vlComparteContenedorPicking);


            (var result1, bool response1) = _automatismoPtlClientService.ValidarOperacion(automatismo.ZonaUbicacion, ptlColorCode, empresa, keySinUsuario);

            if (result1.HasError())
            {
                return (result1, "ERROR");
            }

            var detail = new PtlDetailPicking
            {
                Cliente = codigoCliente,
                Contenedor = contenedor.Numero,
                Preparacion = contenedor.NumeroPreparacion,
                SubClase = contenedor.CodigoSubClase,
                UbicacionEquipo = contenedor.Ubicacion,
                ComparteContenedorPicking = vlComparteContenedorPicking,
                TipoContenedor = contenedor.TipoContenedor
            };

            if (_concurrencyControl.IsLocked(entityToLock, keySinUsuario))
                throw new ValidationFailedException("PTL010_Sec0_Error_AgrupacionBloqueada", new string[] { keySinUsuario });

            _concurrencyControl.AddLock(entityToLock, keySinUsuario, transaction);


            string referencia = _logicPtl.CreatePtlReferencia(preparacion, codigoCliente, empresa);

            (var result, string response) = _automatismoPtlClientService.PrenderLuces(userIdConfirmado, automatismo.ZonaUbicacion, ptlColorCode, empresa, JsonConvert.SerializeObject(detail), referencia, keySinUsuario);


            return (result, response);
        }

        #endregion

    }
}

