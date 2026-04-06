using System;
using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Domain.General.Auxiliares;
using WIS.Domain.Picking;
using WIS.Domain.Services.Interfaces;
using WIS.Exceptions;
using WIS.Security;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Produccion
{
    public class CodigoBarrasContenedorEnsambladoFormulaValidationRule : IValidationRule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IIdentityService _identity;
        protected readonly string _CodigoBarras;
        protected readonly IBarcodeService _barcodeService;

        public CodigoBarrasContenedorEnsambladoFormulaValidationRule(IUnitOfWork uow, string codigoBarras, IIdentityService identity, IBarcodeService barcodeService)
        {
            this._uow = uow;
            this._identity = identity;
            this._CodigoBarras = codigoBarras;
            this._barcodeService = barcodeService;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();
            try
            {
                _barcodeService.ValidarEtiquetaContenedor(_CodigoBarras, _identity.UserId, out AuxContenedor auxContenedor, out int cantidadEmpresa);

                if (auxContenedor.Estado == WIS.Domain.General.Enums.EstadoContenedor.Enviado)
                    throw new Exception("EXP330_form1_Error_ContenedorYaExpedido");

                if (auxContenedor.NuPreparacion == -1)
                    throw new Exception("EXP330_form1_Error_ContenedorNoExiste");

                if (!_uow.PreparacionRepository.PreparacionPertenecePredio(auxContenedor.NuPreparacion, _identity.Predio))
                    throw new Exception("EXP330_form1_Error_ContenedorNoPertebecePredio");

                var detPreparacion = this._uow.PreparacionRepository.GetPedidoDetallePreparacion(auxContenedor.NuPreparacion, auxContenedor.NuContenedor);

                if (this._uow.PreparacionRepository.ExistenAnulacionesPendientes(detPreparacion.NumeroPreparacion, auxContenedor.NuContenedor, detPreparacion.Pedido, detPreparacion.Empresa, detPreparacion.Cliente, out string producto))
                    throw new ValidationFailedException("EXP330_form1_Error_ContConAnulacionesPendientes", new string[] { auxContenedor.TipoContenedor, auxContenedor.IdExternoContenedor, detPreparacion.NumeroPreparacion.ToString(), detPreparacion.Pedido, producto });


                if (!_uow.PreparacionRepository.ContenedorConPedidoEnsamblado(auxContenedor.NuPreparacion, auxContenedor.NuContenedor))
                    throw new Exception("PRD400_Op_Error_ContenedorNoEnsamblado");

            }
            catch (ValidationFailedException ex)
            {
                errors.Add(new ValidationError(ex.Message, new List<string>(ex.StrArguments ?? new string[0])));
            }
            catch (Exception ex)
            {
                errors.Add(new ValidationError(ex.Message));
            }

            return errors;
        }
    }
}
