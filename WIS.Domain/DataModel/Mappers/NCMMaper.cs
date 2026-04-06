using WIS.Domain.General;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class NCMMaper : Mapper
    {
        public NCMMaper()
        {
        }

        public virtual CodigoNomenclaturaComunMercosur MapToObject(T_NAM entity)
        {
            return new CodigoNomenclaturaComunMercosur
            {
                Id = entity.CD_NAM,
                Descripcion = entity.DS_NAM
            };
        }

        public virtual T_NAM MapToEntity(CodigoNomenclaturaComunMercosur obj)
        {
            return new T_NAM
            {
                CD_NAM = obj.Id,
                DS_NAM = obj.Descripcion
            };
        }

        public virtual MotivoAjuste MapToMotivoAjuste(T_MOTIVO_AJUSTE entity)
        {
            return new MotivoAjuste
            {
                Codigo = entity.CD_MOTIVO_AJUSTE,
                Descripcion = entity.DS_MOTIVO_AJUSTE,
                FechaCreacion = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UPDROW
            };
        }

        public virtual T_MOTIVO_AJUSTE MapFromMotivoAjuste(MotivoAjuste obj)
        {
            return new T_MOTIVO_AJUSTE
            {
                CD_MOTIVO_AJUSTE = obj.Codigo,
                DS_MOTIVO_AJUSTE = obj.Descripcion,
                DT_ADDROW = obj.FechaCreacion,
                DT_UPDROW = obj.FechaModificacion
            };
        }

    }
}
