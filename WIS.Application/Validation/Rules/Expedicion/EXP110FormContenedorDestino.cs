using System;
using System.Collections.Generic;
using System.Security.Principal;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Expedicion.EXP110EmpaquetadoPicking.Dto;
using WIS.Domain.General;
using WIS.Domain.General.Auxiliares;
using WIS.Domain.General.Enums;
using WIS.Domain.Services.Interfaces;
using WIS.Exceptions;
using WIS.Security;
using WIS.TrafficOfficer;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Expedicion
{
    public class EXP110FormContenedorDestino : IValidationRule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly string _codigoBarras;
        protected readonly ConfiguracionInicial _confInicial;
        protected readonly ITrafficOfficerService _concurrencyControl;
        protected readonly IParameterService _parameterService;
        protected readonly IBarcodeService _barcodeService;
        protected readonly IIdentityService _identity;

        public EXP110FormContenedorDestino(IUnitOfWork uow, string codigoBarras, IIdentityService identity, ConfiguracionInicial confInicial, ITrafficOfficerService concurrencyControl, IParameterService parameterService, IBarcodeService barcodeService)
        {
            this._uow = uow;
            this._codigoBarras = codigoBarras;
            this._confInicial = confInicial;
            this._concurrencyControl = concurrencyControl;
            this._parameterService = parameterService;
            _barcodeService = barcodeService;
            _identity = identity;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();
            try
            {
                if (_confInicial == null)
                    throw new ValidationFailedException("EXP110_form1_Error_FaltaConfiguracionInicial");

                if (string.IsNullOrEmpty(_codigoBarras) || _codigoBarras == "(NUEVO)")
                    return errors;

                _barcodeService.ValidarEtiquetaContenedor(this._codigoBarras, _identity.UserId, out AuxContenedor datosContenedor, out int cantidadEmpresa);

                Contenedor contenedor = this._uow.ContenedorRepository.GetContenedor(datosContenedor.NuPreparacion, datosContenedor.NuContenedor);

                if (contenedor == null)
                    throw new ValidationFailedException("EXP110_form1_Error_ContenedorNoExiste");

                if (contenedor.IsFacturado())
                    throw new ValidationFailedException("EXP110_form1_Error_ContenedorFacturado");

                if ((contenedor.IdContenedorEmpaque ?? "N") == "N")
                    throw new ValidationFailedException("EXP110_form1_Error_ContenedorNoEsDeEmpaque");

                if (contenedor.Estado != EstadoContenedor.EnPreparacion || contenedor.NumeroPreparacion == -1)
                    throw new ValidationFailedException("EXP110_form1_Error_ContenedorNoReEmpaqueOrNoExiste");

                if (contenedor.Ubicacion != _confInicial.Ubicacion)
                    throw new ValidationFailedException("EXP110_form1_Error_ContenedorOtraEstacion");

                if (contenedor.NroLpn != null)
                    throw new ValidationFailedException("EXP110_form1_Error_ContenedorAsociadoLpn");

                var preparacion = _uow.PreparacionRepository.GetPreparacionPorNumero(contenedor.NumeroPreparacion);

                if(!_uow.FuncionarioRepository.AnyFuncionarioPermisionByEmpresa(preparacion.Empresa.Value, _identity.UserId))
                    throw new ValidationFailedException("General_Sec0_Error_UsuarioSinPermisosParaEmpresa",new  string[] { preparacion.Empresa.Value.ToString()});

                if (preparacion.Predio != _identity.Predio)
                    throw new ValidationFailedException("EXP110_form1_Error_ContenedorNoPerteneceAlPredioLogueado");

                var manejaDocumental = (this._parameterService.GetValueByEmpresa(ParamManager.MANEJO_DOCUMENTAL, (int)preparacion.Empresa) ?? "N") == "S";

                if (manejaDocumental)
                    throw new ValidationFailedException("EXP110_form1_Error_MesaEmpaqueEmpresaDocumental");

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
