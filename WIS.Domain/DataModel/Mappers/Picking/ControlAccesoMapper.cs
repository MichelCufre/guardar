using WIS.Domain.General;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers.Picking
{
    public class ControlAccesoMapper : Mapper
    {
        public virtual ControlAcceso MapToObject(T_CONTROL_ACCESO controlAcceso)
        {
            if (controlAcceso == null)
                return null;

            return new ControlAcceso
            {
                Id = controlAcceso.CD_CONTROL_ACCESO,
                Descripcion = controlAcceso.DS_CONTROL_ACCESO,
            };
        }

        public virtual T_CONTROL_ACCESO MapToEntity(ControlAcceso controlAcceso)
        {
            return new T_CONTROL_ACCESO
            {
                CD_CONTROL_ACCESO = controlAcceso.Id,
                DS_CONTROL_ACCESO = controlAcceso.Descripcion,
            };
        }
    }
}
