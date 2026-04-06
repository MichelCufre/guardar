using System.Collections.Generic;
using WIS.Domain.Automatismo;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers.Automatismo
{
    public class AutomatismoPuestoMapper : Mapper
    {
        public virtual AutomatismoPuesto Map(T_AUTOMATISMO_PUESTO entity)
        {
            if (entity == null) return null;

            return new AutomatismoPuesto()
            {
                FechaRegistro = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UDPROW,
                Puesto = entity.ID_PUESTO,
                IdAutomatismo = entity.NU_AUTOMATISMO,
                Id = entity.NU_AUTOMATISMO_PUESTO,
                Transaccion = entity.NU_TRANSACCION,
                Impresora = entity.CD_IMPRESORA
            };
        }
        public virtual List<AutomatismoPuesto> Map(List<T_AUTOMATISMO_PUESTO> colEntity)
        {
            var colAutomatismoPuesto = new List<AutomatismoPuesto>();
            foreach (T_AUTOMATISMO_PUESTO item in colEntity)
            {
                colAutomatismoPuesto.Add(Map(item));
            }
            return colAutomatismoPuesto;
        }
        public virtual T_AUTOMATISMO_PUESTO MapToEntity(AutomatismoPuesto obj)
        {
            if (obj == null) return null;

            return new T_AUTOMATISMO_PUESTO()
            {
                NU_TRANSACCION = obj.Transaccion,
                NU_AUTOMATISMO_PUESTO = obj.Id,
                NU_AUTOMATISMO = obj.IdAutomatismo,
                DT_ADDROW = obj.FechaRegistro,
                DT_UDPROW = obj.FechaModificacion,
                ID_PUESTO = obj.Puesto,
                CD_IMPRESORA = obj.Impresora
            };
        }
    }
}
