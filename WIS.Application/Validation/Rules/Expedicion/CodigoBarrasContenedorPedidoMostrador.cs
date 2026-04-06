using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General.Auxiliares;
using WIS.Domain.General.Enums;
using WIS.Domain.Services.Interfaces;
using WIS.Exceptions;
using WIS.Security;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class CodigoBarrasContenedorPedidoMostrador : IValidationRule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly string _codigoBarras;
        protected readonly string _cdCliente;
        protected readonly int _cdEmpresa;
        protected readonly string _nuPedido;
        protected readonly IIdentityService _identity;
        protected readonly IBarcodeService _barcodeService;

        public CodigoBarrasContenedorPedidoMostrador(IUnitOfWork uow, string codigoBarras, string cdCliente, int cdEmpresa, string nuPedido, IIdentityService identity, IBarcodeService barcodeService)
        {
            this._uow = uow;
            this._codigoBarras = codigoBarras;
            this._cdCliente = cdCliente;
            this._cdEmpresa = cdEmpresa;
            this._nuPedido = nuPedido;
            this._identity = identity;
            this._barcodeService = barcodeService;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();
            try
            {
                _barcodeService.ValidarEtiquetaContenedor(this._codigoBarras, _identity.UserId, out AuxContenedor datosContenedor, out int cantidadEmpresa);

                if (datosContenedor.Estado == EstadoContenedor.Enviado)
                    throw new ValidationFailedException("EXP330_form1_Error_ContenedorYaExpedido");

                if (datosContenedor.Estado == EstadoContenedor.Vacio)
                    throw new ValidationFailedException("General_msg_Error_ContenedorReservadoCrossDocking");

                if (datosContenedor.NuPreparacion == -1)
                    throw new ValidationFailedException("EXP330_form1_Error_ContenedorNoExiste");

                if (this._uow.PreparacionRepository.EsPickingManualNoFinalizado(datosContenedor.NuPreparacion) &&
                    this._uow.ParametroRepository.GetParameter("FL_PERMITE_PREP_MAN_NO_FIN") == "N")
                    throw new ValidationFailedException("EXP330_form1_Error_ContenedorPreparacionManualNoFinalizada");

                var detPreparacion = this._uow.PreparacionRepository.GetDetallePreparacion(datosContenedor.NuPreparacion, datosContenedor.NuContenedor);

                if (!_uow.FuncionarioRepository.AnyFuncionarioPermisionByEmpresa(detPreparacion.Empresa, _identity.UserId))
                    throw new ValidationFailedException("General_Sec0_Error_UsuarioSinPermisosParaEmpresa", new string[] { detPreparacion.Empresa.ToString() });

                if (this._uow.PreparacionRepository.ExistenAnulacionesPendientes(detPreparacion.NumeroPreparacion, datosContenedor.NuContenedor, detPreparacion.Pedido, detPreparacion.Empresa, detPreparacion.Cliente, out string producto))
                    throw new ValidationFailedException("EXP330_form1_Error_ContConAnulacionesPendientes", new string[] { datosContenedor.TipoContenedor, datosContenedor.IdExternoContenedor, detPreparacion.NumeroPreparacion.ToString(), detPreparacion.Pedido, producto });

                var camion = this._uow.CamionRepository.GetCamionAsignado((long)detPreparacion.Carga);

                if (camion != null && camion.TipoArmadoEgreso != TipoArmadoEgreso.Retira)
                    throw new ValidationFailedException("EXP330_form1_Error_ContenedorYaAsignadoCamionEsgreso");

                if (!string.IsNullOrEmpty(this._nuPedido) && (detPreparacion.Pedido != this._nuPedido || detPreparacion.Cliente != this._cdCliente || detPreparacion.Empresa != _cdEmpresa))
                    throw new ValidationFailedException("EXP330_form1_Error_DistintoContenedorPedidoSeleccionado");

                if (!_uow.PreparacionRepository.PreparacionPertenecePredio(detPreparacion.NumeroPreparacion, _identity.Predio))
                    throw new ValidationFailedException("EXP330_form1_Error_ContenedorNoPertebecePredio");

                var detalle = this._uow.PreparacionRepository.GetDetallePreparacion(datosContenedor.NuPreparacion, datosContenedor.NuContenedor);

                if (detalle != null && detalle.Agrupacion != Agrupacion.Pedido)
                    throw new ValidationFailedException("EXP330_form1_Error_ContenedorNoTipoExpedicion");

            }
            catch (ValidationFailedException ex)
            {
                errors.Add(new ValidationError(ex.Message, ex.StrArguments?.ToList()));
            }
            catch (Exception ex)
            {
                errors.Add(new ValidationError(ex.Message));
            }

            return errors;
        }
    }
}
