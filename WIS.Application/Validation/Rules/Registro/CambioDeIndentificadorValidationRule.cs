using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General.Enums;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class CambioDeIndentificadorValidationRule : IValidationRule
    {
        protected readonly string _producto;
        protected readonly string _empresa;
        protected readonly string _field;
        protected readonly IUnitOfWork _uow;
        protected readonly ProductoMapper _mapper;

        public CambioDeIndentificadorValidationRule(IUnitOfWork uow, string field, string producto, string empresa)
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
                string tipo = this._mapper.MapManejoIdentificador(producto.ManejoIdentificador);

                if (!string.IsNullOrEmpty(tipo) && tipo != this._field)
                {
                    if (_uow.StockRepository.AnyStockUbicacion(producto.Codigo, producto.CodigoEmpresa))
                    {
                        if (this._field == ManejoIdentificadorDb.Lote)
                            return errors;

                        if (this._field == ManejoIdentificadorDb.Producto)
                            return errors;

                        if (this._field == ManejoIdentificadorDb.Serie)
                        {
                            if (_uow.StockRepository.AnyStockConSerieDuplicada(producto.Codigo, producto.CodigoEmpresa, 1))
                                errors.Add(new ValidationError("REG009_Sec0_Error_Er008_NoSePuedeCambiarSerie"));
                        }
                    }
                }
            }
            return errors;
        }
    }
}
