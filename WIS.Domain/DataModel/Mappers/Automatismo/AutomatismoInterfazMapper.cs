using System.Collections.Generic;
using WIS.Domain.Automatismo;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Mappers.Integracion;
using WIS.Domain.Integracion;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers.Automatismo
{
    public class AutomatismoInterfazMapper : Mapper
    {
        protected IntegracionServicioMapper _integracionServicioMapper;

        public AutomatismoInterfazMapper()
        {
            this._integracionServicioMapper = new IntegracionServicioMapper();
        }

        public virtual AutomatismoInterfaz Map(T_AUTOMATISMO_INTERFAZ entity)
        {
            if (entity == null) return null;

            return new AutomatismoInterfaz()
            {
                Id = entity.NU_AUTOMATISMO_INTERFAZ,
                IdAutomatismo = entity.NU_AUTOMATISMO,
                InterfazExterna = entity.CD_INTERFAZ_EXTERNA,
                Method = entity.VL_METHOD,
                IdIntegracionServicio = entity.NU_INTEGRACION,
                Interfaz = entity.CD_INTERFAZ,
                ProtocoloComunicacion = GetProtocolo(entity.ND_PROTOCOLO_COMUNICACION),
                FechaRegistro = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UDPROW,
                Transaccion = entity.NU_TRANSACCION,

                IntegracionServicio = _integracionServicioMapper.Map(entity.T_INTEGRACION_SERVICIO),
            };
        }
        public virtual T_AUTOMATISMO_INTERFAZ Map(AutomatismoInterfaz obj)
        {
            if (obj == null) return null;

            return new T_AUTOMATISMO_INTERFAZ()
            {
                NU_AUTOMATISMO_INTERFAZ = obj.Id,
                NU_AUTOMATISMO = obj.IdAutomatismo,
                CD_INTERFAZ_EXTERNA = obj.InterfazExterna,
                VL_METHOD = obj.Method,
                NU_INTEGRACION = obj.IdIntegracionServicio,
                CD_INTERFAZ = obj.Interfaz,
                ND_PROTOCOLO_COMUNICACION = GetProtocoloDb(obj.ProtocoloComunicacion),
                DT_ADDROW = obj.FechaRegistro,
                DT_UDPROW = obj.FechaModificacion,
                NU_TRANSACCION = obj.Transaccion,
            };
        }

        public virtual List<AutomatismoInterfaz> Map(List<T_AUTOMATISMO_INTERFAZ> colEntity)
        {
            var colAutomatismoInterfaz = new List<AutomatismoInterfaz>();
            foreach (var item in colEntity)
            {
                colAutomatismoInterfaz.Add(Map(item));
            }
            return colAutomatismoInterfaz;
        }
        public virtual ServiceHttpProtocol GetProtocolo(string tipo)
        {
            switch (tipo)
            {
                case MetodoComunicacionDb.ND_PROTOCOLO_POST: return ServiceHttpProtocol.POST;
                case MetodoComunicacionDb.ND_PROTOCOLO_GET: return ServiceHttpProtocol.GET;
                case MetodoComunicacionDb.ND_PROTOCOLO_PUT: return ServiceHttpProtocol.PUT;
                case MetodoComunicacionDb.ND_PROTOCOLO_DELETE: return ServiceHttpProtocol.DELETE;
            }
            return ServiceHttpProtocol.UNKNOWN;
        }

        public virtual string GetProtocoloDb(ServiceHttpProtocol tipo)
        {
            switch (tipo)
            {
                case ServiceHttpProtocol.POST: return MetodoComunicacionDb.ND_PROTOCOLO_POST;
                case ServiceHttpProtocol.GET: return MetodoComunicacionDb.ND_PROTOCOLO_GET;
                case ServiceHttpProtocol.PUT: return MetodoComunicacionDb.ND_PROTOCOLO_PUT;
                case ServiceHttpProtocol.DELETE: return MetodoComunicacionDb.ND_PROTOCOLO_DELETE;
            }
            return null;
        }
    }
}
