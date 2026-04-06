using WIS.Domain.General;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class UnidadMedidaMapper : Mapper
    {

        public virtual UnidadMedida MapToObject(T_UNIDADE_MEDIDA entity)
        {
            if (entity == null)
                return null;    

            return new UnidadMedida
            {
                Id = entity.CD_UNIDADE_MEDIDA,
                IdExterno = entity.CD_UNIDAD_MEDIDA_EXTERNA,
                Descripcion = entity.DS_UNIDADE_MEDIDA,
                FechaInsercion = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UPDROW,
                AceptaDecimal = MapStringToBoolean(entity.FG_ACEITA_DECIMAL)
            };
        }

        public virtual T_UNIDADE_MEDIDA MapToEntity(UnidadMedida obj)
        {
            return new T_UNIDADE_MEDIDA
            {
                CD_UNIDADE_MEDIDA = obj.Id,
                CD_UNIDAD_MEDIDA_EXTERNA = obj.IdExterno,
                DT_ADDROW = obj.FechaInsercion,
                DT_UPDROW = obj.FechaModificacion,
                DS_UNIDADE_MEDIDA = obj.Descripcion,
                FG_ACEITA_DECIMAL = MapBooleanToString(obj.AceptaDecimal)
            };
        }
    }
}
