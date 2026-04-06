using System.Collections.Generic;
using System.Text;
using WIS.Domain.Automatismo;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers.Automatismo
{
    public class AutomatismoDataMapper : Mapper
    {
        public virtual AutomatismoData Map(T_AUTOMATISMO_DATA entity)
        {
            if (entity == null) return null;

            return new AutomatismoData()
            {
                Id = entity.NU_AUTOMATISMO_DATA,
                IdAutomatismoEjecucion = entity.NU_AUTOMATISMO_EJECUCION,
                FechaRegistro = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UDPROW,
                Transaccion = entity.NU_TRANSACCION,
                RequestData = Encoding.UTF8.GetString(entity.VL_DATA_REQUEST),
                ResponseData = Encoding.UTF8.GetString(entity.VL_DATA_RESPONSE)
            };
        }

        public virtual List<AutomatismoData> Map(List<T_AUTOMATISMO_DATA> colEntity)
        {
            List<AutomatismoData> colObj = new List<AutomatismoData>();

            foreach (var entity in colEntity)
                colObj.Add(Map(entity));

            return colObj;
        }

        public virtual T_AUTOMATISMO_DATA MapToEntity(AutomatismoData obj)
        {
            if (obj == null) return null;

            return new T_AUTOMATISMO_DATA()
            {
                NU_AUTOMATISMO_DATA = obj.Id,
                NU_AUTOMATISMO_EJECUCION = obj.IdAutomatismoEjecucion,
                DT_ADDROW = obj.FechaRegistro,
                DT_UDPROW = obj.FechaModificacion,
                NU_TRANSACCION = obj.Transaccion,
                VL_DATA_REQUEST = Encoding.UTF8.GetBytes(obj.RequestData ?? ""),
                VL_DATA_RESPONSE = Encoding.UTF8.GetBytes(obj.ResponseData ?? "")
            };
        }
    }
}
