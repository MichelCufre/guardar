using WIS.Domain.General;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class ProductoRotatividadMapper : Mapper
    {
        public ProductoRotatividadMapper()
        {
        }

        public virtual ProductoRotatividad MapToObject(T_ROTATIVIDADE entity)
        {
            if (entity == null)
                return null;

            return new ProductoRotatividad
            {
                Id = entity.CD_ROTATIVIDADE,
                Descripcion = entity.DS_ROTATIVIDADE,
                CantidadMaximaDeDiasEnAlmacen = entity.QT_MAX_DIAS_ESTOCAGEM,
                FechaInsercion = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UPDROW
            };
        }

        public virtual T_ROTATIVIDADE MapToEntity(ProductoRotatividad obj)
        {
            return new T_ROTATIVIDADE
            {
                CD_ROTATIVIDADE = obj.Id,
                DS_ROTATIVIDADE = obj.Descripcion,
                QT_MAX_DIAS_ESTOCAGEM = obj.CantidadMaximaDeDiasEnAlmacen,
                DT_ADDROW = obj.FechaInsercion,
                DT_UPDROW = obj.FechaModificacion

            };
        }
    }
}
