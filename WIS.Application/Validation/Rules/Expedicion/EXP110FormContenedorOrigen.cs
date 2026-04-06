using NLog;
using System;
using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Expedicion.EXP110EmpaquetadoPicking;
using WIS.Domain.Expedicion.EXP110EmpaquetadoPicking.Dto;
using WIS.Domain.General;
using WIS.Domain.General.Auxiliares;
using WIS.Domain.General.Enums;
using WIS.Domain.Services.Interfaces;
using WIS.Exceptions;
using WIS.FormComponent;
using WIS.Security;
using WIS.TrafficOfficer;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Expedicion
{
    public class EXP110FormContenedorOrigen : IValidationRule
    {
        protected readonly Logger Logger = LogManager.GetCurrentClassLogger();

        protected readonly string _codigoBarras;
        protected readonly ContenedorDestinoData _contenedorDestinoData;
        protected readonly FormField _field;
        protected readonly IUnitOfWork _uow;
        protected readonly IIdentityService _identity;
        protected readonly ITrafficOfficerService _concurrencyControl;
        protected readonly IParameterService _parameterService;
        protected readonly ITrackingService _trackingService;
        protected readonly IBarcodeService _barcodeService;

        protected readonly EmpaquetadoPickingLogic _logic;

        public EXP110FormContenedorOrigen(IUnitOfWork uow,
            string codigoBarras,
            IIdentityService identity,
            ContenedorDestinoData contenedorDestinoData,
            FormField field,
            ITrafficOfficerService concurrencyControl,
            IParameterService parameterService,
            IPrintingService printingService,
            ITrackingService trackingService,
            IBarcodeService barcodeService)
        {
            this._uow = uow;
            this._codigoBarras = codigoBarras;
            this._identity = identity;
            this._contenedorDestinoData = contenedorDestinoData;
            this._field = field;
            this._concurrencyControl = concurrencyControl;
            this._parameterService = parameterService;
            this._trackingService = trackingService;
            this._barcodeService = barcodeService;

            this._logic = new EmpaquetadoPickingLogic(printingService, trackingService, barcodeService, identity, Logger);
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();
            try
            {
                if (string.IsNullOrEmpty(_codigoBarras))
                    return errors;

                _barcodeService.ValidarEtiquetaContenedor(this._codigoBarras, _identity.UserId, out AuxContenedor datosContenedor, out int cantidadEmpresa);

                Contenedor contenedor = this._uow.ContenedorRepository.GetContenedor(datosContenedor.NuPreparacion, datosContenedor.NuContenedor);

                if (contenedor == null)
                    throw new ValidationFailedException("EXP110_form1_Error_ContenedorNoExiste");

                if (contenedor.NroLpn != null)
                    throw new ValidationFailedException("EXP110_form1_Error_ContenedorAsociadoLpn");

                var preparacion = _uow.PreparacionRepository.GetPreparacionPorNumero(contenedor.NumeroPreparacion);

                if (!_uow.FuncionarioRepository.AnyFuncionarioPermisionByEmpresa(preparacion.Empresa.Value, _identity.UserId))
                    throw new ValidationFailedException("General_Sec0_Error_UsuarioSinPermisosParaEmpresa", new string[] { preparacion.Empresa.Value.ToString() });

                if (preparacion.Predio != _identity.Predio)
                    throw new ValidationFailedException("EXP110_form1_Error_ContenedorNoPerteneceAlPredioLogueado");

                var manejaDocumental = (this._parameterService.GetValueByEmpresa(ParamManager.MANEJO_DOCUMENTAL, (int)preparacion.Empresa) ?? "N") == "S";
                if (manejaDocumental)
                    throw new ValidationFailedException("EXP110_form1_Error_MesaEmpaqueEmpresaDocumental");

                if (contenedor.Estado != EstadoContenedor.EnPreparacion || contenedor.NumeroPreparacion == -1)
                    throw new ValidationFailedException("EXP110_form1_Error_ContenedorNoReEmpaqueOrNoExiste");

                if (!_uow.EmpaquetadoPickingRepository.ContenedorPuedeUsarseEnEmpaque(contenedor.Numero, contenedor.NumeroPreparacion))
                    throw new ValidationFailedException("EXP110_form1_Error_ContenedorNoTienePedidoParaEmpaquetar");

                var camionAsociado = _uow.ContenedorRepository.GetCamionAsignado(contenedor.Numero, contenedor.NumeroPreparacion);
                if (camionAsociado != null && _uow.PedidoRepository.AnyPedidoFacturaEnEmpaque(contenedor.NumeroPreparacion, contenedor.Numero))
                    throw new ValidationFailedException("EXP110_form1_Error_ContenedorFacturaEnEmpaqueAsociadoEgreso", new string[] { camionAsociado.Id.ToString() });

                if (_contenedorDestinoData != null)
                {
                    var menssageResult = string.Empty;
                    var compatible = _logic.ExisteCompatibilidadContenedor(_uow, contenedor.Numero, contenedor.NumeroPreparacion, _contenedorDestinoData.SubClase, _contenedorDestinoData.CompartContenedorEntrega, out menssageResult);

                    if (!compatible)
                        throw new ValidationFailedException(menssageResult);
                }

                if (contenedor.IsFacturado())
                    throw new ValidationFailedException("EXP110_form1_Error_ContenedorFacturado");

                if (contenedor.IdContenedorEmpaque == "S")
                    throw new ValidationFailedException("EXP110_form1_Error_ContenedorYaEmpaquetado");

                if (!string.IsNullOrEmpty(contenedor.ValorControl))
                {
                    if (contenedor.ValorControl == EstadoControlContenedor.Iniciado)
                        throw new ValidationFailedException("General_msg_Error_ContenedorConControlEnProceso");
                    else
                        throw new ValidationFailedException("General_msg_Error_ContenedorControlado");
                }

                var idLock = $"{contenedor.NumeroPreparacion}#{contenedor.Numero}#{contenedor.TipoContenedor}#{contenedor.IdExterno}";
                if (this._concurrencyControl.IsLocked("T_CONTENEDOR", idLock))
                    throw new ValidationFailedException("General_msg_Error_ContenedorBloqueado", new string[] { contenedor.TipoContenedor, contenedor.IdExterno });

                this._concurrencyControl.AddLock("T_CONTENEDOR", idLock);

            }
            catch (ValidationFailedException ex)
            {
                if (ex.Message == "EXP110_form1_Error_ClienteOrigenDistintoClienteDestino")
                    this._field.Value = string.Empty;

                throw ex;
            }
            catch (Exception ex)
            {
                errors.Add(new ValidationError(ex.Message));
            }

            return errors;
        }
    }
}
