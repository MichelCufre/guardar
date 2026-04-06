using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Persistence.Database;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class UbicacionAreaPickingValidationRule : IValidationRule
    {
        protected readonly string _idUbicacion;
        protected readonly int? _empresa;
        protected readonly string _codigoProducto;
        protected readonly bool _isNew;
        protected readonly IUnitOfWork _uow;

        public UbicacionAreaPickingValidationRule(IUnitOfWork uow, string idUbicacion, int? empresa, string codigoProducto, bool isNew)
        {
            this._idUbicacion = idUbicacion.ToUpper();
            this._empresa = empresa;
            this._codigoProducto = codigoProducto;
            this._uow = uow;
            this._isNew = isNew;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!string.IsNullOrEmpty(_idUbicacion))
            {
                var area = _uow.UbicacionAreaRepository.GetTipoAreaByUbicacion(_idUbicacion);

                if (area == null || !area.EsAreaPicking || !area.DisponibilizaStock)
                    errors.Add(new ValidationError("General_Sec0_Error_UbicacionNoEsAreaPicking"));

                var tipoUbicacion = _uow.UbicacionTipoRepository.GetTipoByUbicacion(_idUbicacion);

                if (_empresa.HasValue && !string.IsNullOrEmpty(_codigoProducto))
                {
                    if (!tipoUbicacion.PermiteVariosProductos)
                    {
                        if (_uow.UbicacionPickingProductoRepository.AnyUbicacionPickingOtroProducto(_idUbicacion, _empresa.Value, _codigoProducto))
                        {
                            errors.Add(new ValidationError("General_Sec0_Error_UbicacionMonoProductoYaAsignada"));
                        }
                        else if (_uow.StockRepository.AnyStockOtroProducto(_empresa.Value, _codigoProducto, _idUbicacion))
                        {
                            errors.Add(new ValidationError("General_Sec0_Error_UbicacionMonoProductoOtroStock"));
                        }
                    }

                    if (_isNew && _uow.UbicacionPickingProductoRepository.AnyUbicacionPickingProducto(_idUbicacion, _empresa.Value, _codigoProducto, 1))
                        errors.Add(new ValidationError("General_Sec0_Error_ProductoUbicacionExistente"));

                    if (tipoUbicacion.RespetaClase)
                    {
                        var producto = _uow.ProductoRepository.GetProducto(_empresa.Value, _codigoProducto);
                        var ubicacion = _uow.UbicacionRepository.GetUbicacion(_idUbicacion);

                        if (ubicacion.CodigoClase != producto.CodigoClase)
                            errors.Add(new ValidationError("REG050_msg_Error_ProductoDistintaClaseUbicacion"));
                    }
                }
            }

            return errors;
        }
    }
}
