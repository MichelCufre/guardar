using WIS.Domain.Produccion;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers.Produccion
{
    public class IdentificadorConsumirMapper : Mapper
    {
        public virtual IdentificadorConsumir MapEntityToObject(T_PRDC_CONSUMO_IDENTIFICADOR entity)
        {
            return new IdentificadorConsumir
            {
                Empresa = entity.CD_EMPRESA,
                Faixa = entity.CD_FAIXA,
                Identificador = entity.NU_IDENTIFICADOR?.Trim()?.ToUpper(),
                Orden = entity.NU_ORDEN,
                Producto = entity.CD_PRODUTO,
                Stock = entity.QT_STOCK ?? 0,
                Ubicacion = entity.CD_ENDERECO
            };
        }

        public virtual T_PRDC_CONSUMO_IDENTIFICADOR MapObjectToEntity(IdentificadorConsumir identificador)
        {
            return new T_PRDC_CONSUMO_IDENTIFICADOR
            {
                CD_EMPRESA = identificador.Empresa,
                CD_FAIXA = identificador.Faixa,
                NU_IDENTIFICADOR = identificador.Identificador?.Trim()?.ToUpper(),
                NU_ORDEN = identificador.Orden,
                CD_PRODUTO = identificador.Producto,
                QT_STOCK = identificador.Stock,
                CD_ENDERECO = identificador.Ubicacion
            };
        }
    }
}
