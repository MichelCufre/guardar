using System;
using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Domain.Expedicion.EXP110EmpaquetadoPicking.Dto;
using WIS.Domain.General;
using WIS.Exceptions;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Expedicion
{
    public class EXP110FormCodigoBarrasProducto : IValidationRule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly string _codigoBarras;
        protected readonly int _cdEmpresa;
        protected readonly int _nuContenedor;
        protected readonly int _nuPreparacion;
        protected readonly ContenedorDestinoData _contenedorDestinoData;


        public EXP110FormCodigoBarrasProducto(IUnitOfWork uow, string codigoBarras, int cdEmpresa, ContenedorDestinoData contenedorDestinoData, int nuContenedor, int nuPreparacion)
        {
            _uow = uow;
            _codigoBarras = codigoBarras.ToUpper();
            _cdEmpresa = cdEmpresa;
            _nuContenedor = nuContenedor;
            _nuPreparacion = nuPreparacion;
            _contenedorDestinoData = contenedorDestinoData;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();
            try
            {
                if (!_uow.ProductoCodigoBarraRepository.ExisteCodigoBarra(_codigoBarras, _cdEmpresa))
                    throw new ValidationFailedException("EXP110_form1_Error_CodigoBarrasProductoNoExiste");

                var productoBarras = _uow.ProductoCodigoBarraRepository.GetProductoCodigoBarra(_codigoBarras, _cdEmpresa);

                Producto prod = _uow.ProductoRepository.GetProducto(_cdEmpresa, productoBarras.IdProducto);

                var stockContenedor = _uow.EmpaquetadoPickingRepository.TieneStockProductoContenedor(_nuContenedor, _nuPreparacion, prod.Codigo);

                if (!stockContenedor)
                    throw new ValidationFailedException("EXP110_form1_Error_ProductoSinStock");

            }
            catch (ValidationFailedException ex)
            {
                throw ex;
                //errors.Add(new ValidationError(ex.Message, ex.StrArguments?.ToList()));
            }
            catch (Exception ex)
            {
                errors.Add(new ValidationError(ex.Message));
            }

            return errors;
        }
    }
}
