using System.Collections.Generic;
using WIS.Domain.Automatismo;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers.Automatismo
{
    public class AutomatismoCaracteristicaConfiguracionMapper : Mapper
    {
        public virtual AutomatismoCaracteristicaConfiguracion Map(T_AUTOMATISMO_CARACTERISTICA_CONFIG entity)
        {
            if (entity == null) return null;

            return new AutomatismoCaracteristicaConfiguracion
            {
                TipoAutomatismo = entity.TP_AUTOMATISMO,
                Codigo = entity.CD_AUTOMATISMO_CARACTERISTICA,
                Descripcion = entity.DS_AUTOMATISMO_CARACTERISTICA,
                Valor = entity.VL_AUTOMATISMO_CARACTERISTICA,
                CantidadAuxiliar = entity.QT_AUX1,
                FlagAuxiliar = MapStringToBoolean(entity.FL_AUX1),
                NumeroAuxiliar = entity.NU_AUX1,
                Opciones = entity.VL_OPCIONES,
                ValorAuxiliar = entity.VL_AUX1
            };
        }

        public virtual List<AutomatismoCaracteristicaConfiguracion> Map(List<T_AUTOMATISMO_CARACTERISTICA_CONFIG> colEntity)
        {
            List<AutomatismoCaracteristicaConfiguracion> objs = new List<AutomatismoCaracteristicaConfiguracion>();

            foreach (var entity in colEntity) objs.Add(Map(entity));

            return objs;
        }
    }
}
