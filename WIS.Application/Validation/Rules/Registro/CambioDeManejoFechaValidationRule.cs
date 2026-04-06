using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General.Enums;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class CambioDeManejoFechaValidationRule : IValidationRule
    {
        protected readonly string _producto;
        protected readonly string _empresa;
        protected readonly string _field;
        protected readonly IUnitOfWork _uow;
        protected readonly ProductoMapper _mapper;

        public CambioDeManejoFechaValidationRule(IUnitOfWork uow, string field, string producto, string empresa)
        {
            this._field = field;
            this._producto = producto;
            this._empresa = empresa;
            this._uow = uow;
            this._mapper = new ProductoMapper();
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (this._uow.ProductoRepository.ExisteProducto(int.Parse(this._empresa), this._producto))
            {
                var producto = this._uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(int.Parse(this._empresa), this._producto);

                if (this._mapper.MapManejoFecha(producto.TipoManejoFecha) != this._mapper.MapManejoFecha(this._field))
                {
                    var permiteModificarSinValidar = (_uow.ParametroRepository.GetParameter(ParamManager.PERMITIR_MOD_DATOS_LOGISTICOS) ?? "N") == "S";

                    if (!permiteModificarSinValidar && _uow.StockRepository.AnyStockUbicacion(producto.Codigo, producto.CodigoEmpresa))
                    {
                        if ((this._mapper.MapManejoFecha(producto.TipoManejoFecha) == ManejoFechaProducto.Duradero && this._mapper.MapManejoFecha(this._field) == ManejoFechaProducto.Expirable)
                            ||
                           (this._mapper.MapManejoFecha(producto.TipoManejoFecha) == ManejoFechaProducto.Fifo && this._mapper.MapManejoFecha(this._field) == ManejoFechaProducto.Expirable))
                        {
                            errors.Add(new ValidationError("REG009_Sec0_Error_Er007_NoSePuedeCambiarPorStock"));
                        }
                    }
                }
            }

            return errors;
        }
    }
}
