using WIS.Domain.Porteria;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers.Porteria
{
    public class PorteriaCamionMapper
    {
        public PorteriaCamionMapper()
        {

        }

        public virtual PorteriaCamion MapToObject(T_PORTERIA_VEHICULO_CAMION Entity)
        {
            return new PorteriaCamion
            {
                nuPorteriaVehiculo = Entity.NU_PORTERIA_VEHICULO,
                nuPorteriaVehiculoCamion = Entity.NU_PORTERIA_VEHICULO_CAMION,
                cdCamion = Entity.CD_CAMION
            };
        }

        public virtual T_PORTERIA_VEHICULO_CAMION MapToEntity(PorteriaCamion Obj)
        {
            return new T_PORTERIA_VEHICULO_CAMION
            {
                NU_PORTERIA_VEHICULO = Obj.nuPorteriaVehiculo,
                NU_PORTERIA_VEHICULO_CAMION = Obj.nuPorteriaVehiculoCamion,
                CD_CAMION = Obj.cdCamion
            };
        }
    }
}
