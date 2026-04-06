using WIS.Domain.General;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class DominioMapper : Mapper
    {
        public virtual Dominio MapToObject(T_DOMINIO entity)
        {
            if (entity == null)
                return null;

            var dominio = new Dominio
            {
                Codigo = entity.CD_DOMINIO,
                Descripcion = entity.DS_DOMINIO,
                FlInterno = entity.FL_INTERNO_WIS
            };

            if (entity.T_DET_DOMINIO != null && entity.T_DET_DOMINIO.Count > 0)
            {
                foreach (var det in entity.T_DET_DOMINIO)
                {
                    dominio.Detalles.Add(MapToObject(det));
                }
            }

            return dominio;
        }

        public virtual T_DOMINIO MapToEntity(Dominio obj)
        {
            if (obj == null)
                return null;

            return new T_DOMINIO
            {
                CD_DOMINIO = obj.Codigo,
                DS_DOMINIO = obj.Descripcion,
                FL_INTERNO_WIS = obj.FlInterno
            };
        }

        public virtual DominioDetalle MapToObject(T_DET_DOMINIO entity)
        {
            if (entity == null)
                return null;

            return new DominioDetalle
            {
                Id = entity.NU_DOMINIO,
                Codigo = entity.CD_DOMINIO,
                Valor = entity.CD_DOMINIO_VALOR,
                Descripcion = entity.DS_DOMINIO_VALOR
            };
        }

        public virtual T_DET_DOMINIO MapToEntity(DominioDetalle obj)
        {
            if (obj == null)
                return null;

            return new T_DET_DOMINIO
            {
                NU_DOMINIO = obj.Id,
                CD_DOMINIO = obj.Codigo,
                CD_DOMINIO_VALOR = obj.Valor,
                DS_DOMINIO_VALOR = obj.Descripcion
            };
        }
    }
}
