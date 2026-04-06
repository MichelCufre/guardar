using WIS.Domain.General;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class ProductoFamiliaMapper : Mapper
    {
        public ProductoFamiliaMapper()
        {
        }

        public virtual ProductoFamilia MapToObject(T_FAMILIA_PRODUTO entity)
        {
            if (entity == null)
                return null;

            return new ProductoFamilia
            {
                Id = entity.CD_FAMILIA_PRODUTO,
                Descripcion = entity.DS_FAMILIA_PRODUTO,
                Alta = entity.DT_ADDROW,
                Modificacion = entity.DT_UPDROW,
            };
        }

        public virtual T_FAMILIA_PRODUTO MapToEntity(ProductoFamilia obj)
        {
            return new T_FAMILIA_PRODUTO
            {
                CD_FAMILIA_PRODUTO = obj.Id,
                DS_FAMILIA_PRODUTO = obj.Descripcion,
                DT_ADDROW = obj.Alta,
                DT_UPDROW = obj.Modificacion
            };
        }
    }
}
