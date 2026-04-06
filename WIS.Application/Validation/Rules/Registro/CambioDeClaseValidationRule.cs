using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.General.Enums;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class CambioDeClaseValidationRule : IValidationRule
    {
        protected readonly string _producto;
        protected readonly string _empresa;
        protected readonly string _field;
        protected readonly IUnitOfWork _uow;
        protected readonly UbicacionMapper _mapper;

        public CambioDeClaseValidationRule(IUnitOfWork uow, string field, string producto, string empresa)
        {
            this._field = field;
            this._producto = producto;
            this._empresa = empresa;
            this._uow = uow;
            this._mapper = new UbicacionMapper();
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (this._uow.ProductoRepository.ExisteProducto(int.Parse(this._empresa), this._producto))
            {
                var producto = this._uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(int.Parse(this._empresa), this._producto);

                var ubicacionesPermitidas = new List<short>()
                {
                    AreaUbicacionDb.PuertaEmbarque,
                    AreaUbicacionDb.Transferencia
                };

                if (producto.CodigoClase != this._field)
                {
                    if (_uow.StockRepository.AnyStockUbicacion(producto.Codigo, producto.CodigoEmpresa))
                    {
                        List<string> ubicacionesConStock = _uow.StockRepository.GetUbicacionesConOSinStock(producto.Codigo, producto.CodigoEmpresa);

                        foreach (var ubicacion in ubicacionesConStock)
                        {
                            if (!ubicacionesPermitidas.Contains(_uow.UbicacionRepository.GetUbicacion(ubicacion).IdUbicacionArea))
                            {
                                errors.Add(new ValidationError("REG009_Sec0_Error_Er007_NoSePuedeCambiarPorStock"));

                                return errors;
                            }

                        }
                    }
                }
            }

            return errors;
        }
    }
}
