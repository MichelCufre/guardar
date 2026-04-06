using WIS.Domain.Expedicion;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class CargaCamionMapper : Mapper
    {
        public CargaCamionMapper()
        {

        }
        public virtual CargaCamion MapToObject(T_CLIENTE_CAMION CargaCamionEntity)
        {
            if (CargaCamionEntity == null)
                return null;

            return new CargaCamion
            {

                Camion = CargaCamionEntity.CD_CAMION,
                Carga = CargaCamionEntity.NU_CARGA,
                Cliente = CargaCamionEntity.CD_CLIENTE,
                Empresa = CargaCamionEntity.CD_EMPRESA,
                FechaModificacion = CargaCamionEntity.DT_UPDROW,
                FechaAlta = CargaCamionEntity.DT_ADDROW,
                IdCargar = CargaCamionEntity.ID_CARGAR,
                TipoModalidad = CargaCamionEntity.TP_MODALIDAD,
                SincronizacionRealizada = MapStringToBoolean(CargaCamionEntity.FL_SYNC_REALIZADA),
            };
        }

        public virtual T_CLIENTE_CAMION MapToEntity(CargaCamion CargaCamion)
        {
            return new T_CLIENTE_CAMION
            {
                CD_CAMION = CargaCamion.Camion,
                NU_CARGA = CargaCamion.Carga,
                CD_CLIENTE = CargaCamion.Cliente,
                CD_EMPRESA = CargaCamion.Empresa,
                DT_UPDROW = CargaCamion.FechaModificacion,
                DT_ADDROW = CargaCamion.FechaAlta,
                ID_CARGAR = CargaCamion.IdCargar,
                TP_MODALIDAD = CargaCamion.TipoModalidad,
                FL_SYNC_REALIZADA = MapBooleanToString(CargaCamion.SincronizacionRealizada),
            };
        }
    }
}
