using WIS.Domain.Impresiones;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class ImpresoraMapper : Mapper
    {
        public ImpresoraMapper()
        {

        }

        public virtual Impresora MapToObject(T_IMPRESORA entity)
        {
            if (entity == null) return null;

            return new Impresora
            {
                Id = entity.CD_IMPRESORA,
                Descripcion = entity.DS_IMPRESORA,
                Predio = entity.NU_PREDIO,
                CodigoLenguajeImpresion = entity.CD_LENGUAJE_IMPRESION,
                Servidor=entity.CD_SERVIDOR
            };
        }

        public virtual T_IMPRESORA MapToEntity(Impresora obj)
        {
            if (obj == null) return null;

            return new T_IMPRESORA
            {
                CD_IMPRESORA = obj.Id,
                DS_IMPRESORA = obj.Descripcion,
                NU_PREDIO = obj.Predio,
                CD_LENGUAJE_IMPRESION = obj.CodigoLenguajeImpresion,
                CD_SERVIDOR = obj.Servidor
            };
        }
    }
}
