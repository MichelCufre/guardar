using WIS.Domain.General;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class ProductoRamoMapper : Mapper
    {

        public ProductoRamoMapper()
        {
        }

        public virtual ProductoRamo MapToObject(T_RAMO_PRODUTO ramo)
        {
            if (ramo == null)
                return null;

            return new ProductoRamo
            {
                Id = ramo.CD_RAMO_PRODUTO,
                Descripcion = ramo.DS_RAMO_PRODUTO,
                Alta = ramo.DT_ADDROW,
                Modificacion = ramo.DT_UPDROW
            };
        }

        public virtual T_RAMO_PRODUTO MapToEntity(ProductoRamo ramo)
        {
            return new T_RAMO_PRODUTO
            {
                CD_RAMO_PRODUTO = ramo.Id,
                DS_RAMO_PRODUTO = ramo.Descripcion,
                DT_ADDROW = ramo.Alta,
                DT_UPDROW = ramo.Modificacion
            };
        }
    }
}
