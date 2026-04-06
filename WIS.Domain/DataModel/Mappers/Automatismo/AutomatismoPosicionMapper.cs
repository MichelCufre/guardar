using System.Collections.Generic;
using WIS.Domain.Automatismo;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers.Automatismo
{
    public class AutomatismoPosicionMapper : Mapper
    {
        public virtual AutomatismoPosicion Map(T_AUTOMATISMO_POSICION entity)
        {
            if (entity == null) return null;

            return new AutomatismoPosicion()
            {
                IdUbicacion = entity.CD_ENDERECO,
                FechaRegistro = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UDPROW,
                IdAutomatismo = entity.NU_AUTOMATISMO,
                Id = entity.NU_AUTOMATISMO_POSICION,
                Transaccion = entity.NU_TRANSACCION,
                TipoUbicacion = entity.ND_TIPO_ENDERECO,
                PosicionExterna = entity.VL_POSICION_EXTERNA,
                ComparteAgrupacion = entity.VL_COMPARTE_AGRUPACION,
                Orden = entity.NU_ORDEN,
                TipoAgrupacion = entity.TP_AGRUPACION_UBIC,
            };
        }

        public virtual List<AutomatismoPosicion> Map(List<T_AUTOMATISMO_POSICION> colEntity)
        {
            List<AutomatismoPosicion> colAutomatismoPosicion = null;
            if (colEntity != null)
            {
                colAutomatismoPosicion = new List<AutomatismoPosicion>();
                colEntity.ForEach(ent =>
                {
                    colAutomatismoPosicion.Add(this.Map(ent));
                });
            }
            return colAutomatismoPosicion;
        }

        public virtual T_AUTOMATISMO_POSICION MapToEntity(AutomatismoPosicion obj)
        {
            if (obj == null) return null;

            return new T_AUTOMATISMO_POSICION()
            {
                CD_ENDERECO = obj.IdUbicacion,
                DT_ADDROW = obj.FechaRegistro,
                DT_UDPROW = obj.FechaModificacion,
                NU_AUTOMATISMO = obj.IdAutomatismo,
                NU_AUTOMATISMO_POSICION = obj.Id,
                NU_TRANSACCION = obj.Transaccion,
                ND_TIPO_ENDERECO = obj.TipoUbicacion,
                VL_POSICION_EXTERNA = obj.PosicionExterna,
                NU_ORDEN = obj.Orden,
                TP_AGRUPACION_UBIC = obj.TipoAgrupacion,
                VL_COMPARTE_AGRUPACION = obj.ComparteAgrupacion,
            };
        }
    }
}
