using System.Collections.Generic;
using WIS.Domain.Automatismo;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers.Automatismo
{
    public class AutomatismoCaracteristicaMapper : Mapper
    {
        public virtual AutomatismoCaracteristica Map(T_AUTOMATISMO_CARACTERISTICA entity)
        {
            if (entity == null) return null;

            return new AutomatismoCaracteristica()
            {
                Id = entity.NU_AUTOMATISMO_CARACTERISTICA,
                IdAutomatismo = entity.NU_AUTOMATISMO,
                Codigo = entity.CD_AUTOMATISMO_CARACTERISTICA,
                Descripcion = entity.DS_AUTOMATISMO_CARACTERISTICA,
                Valor = entity.VL_AUTOMATISMO_CARACTERISTICA,
                ValorAuxiliar = entity.VL_AUX1,
                NumeroAuxiliar = entity.NU_AUX1,
                CantidadAuxiliar = entity.QT_AUX1,
                FlagAuxiliar = base.MapStringToBoolean(entity.FL_AUX1),
                FechaAlta = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UDPROW,
                Transaccion = entity.NU_TRANSACCION
            };
        }

        public virtual List<AutomatismoCaracteristica> Map(List<T_AUTOMATISMO_CARACTERISTICA> colEntity)
        {
            var colObj = new List<AutomatismoCaracteristica>();

            if (colEntity == null) return colObj;

            foreach (var entity in colEntity)
            {
                colObj.Add(Map(entity));
            }
            return colObj;
        }

        public virtual T_AUTOMATISMO_CARACTERISTICA MapToEntity(AutomatismoCaracteristica obj)
        {
            if (obj == null) return null;

            return new T_AUTOMATISMO_CARACTERISTICA()
            {
                NU_AUTOMATISMO_CARACTERISTICA = obj.Id,
                NU_AUTOMATISMO = obj.IdAutomatismo,
                CD_AUTOMATISMO_CARACTERISTICA = obj.Codigo,
                DS_AUTOMATISMO_CARACTERISTICA = obj.Descripcion,
                VL_AUTOMATISMO_CARACTERISTICA = obj.Valor,
                NU_AUX1 = obj.NumeroAuxiliar,
                VL_AUX1 = obj.ValorAuxiliar,
                QT_AUX1 = obj.CantidadAuxiliar,
                FL_AUX1 = base.MapBooleanToString(obj.FlagAuxiliar),
                DT_ADDROW = obj.FechaAlta,
                DT_UDPROW = obj.FechaModificacion,
                NU_TRANSACCION = obj.Transaccion,
            };
        }

        public virtual List<T_AUTOMATISMO_CARACTERISTICA> MapToEntity(List<AutomatismoCaracteristica> colObj)
        {
            var colEntity = new List<T_AUTOMATISMO_CARACTERISTICA>();

            if (colObj == null) return colEntity;

            foreach (var obj in colObj)
            {
                colEntity.Add(MapToEntity(obj));
            }
            return colEntity;
        }


        public virtual PtlColor Map(AutomatismoCaracteristica obj)
        {
            if (obj == null) return null;

            return new PtlColor
            {
                Code = obj.Valor,
                Css = obj.ValorAuxiliar,
                Description = obj.Descripcion,
                IsEnabled = obj.FlagAuxiliar
            };
        }

        public virtual List<PtlColor> Map(List<AutomatismoCaracteristica> objs)
        {
            var result = new List<PtlColor>();

            if (objs != null)
            {
                foreach (var obj in objs)
                {
                    result.Add(this.Map(obj));
                }
            }

            return result;
        }
    }
}
