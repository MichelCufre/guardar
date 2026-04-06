using WIS.Domain.Expedicion;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class TipoVehiculoMapper : Mapper
    {
        public virtual VehiculoEspecificacion MapToObject(T_TIPO_VEICULO entity)
        {
            return new VehiculoEspecificacion
            {
                Id = entity.CD_TIPO_VEICULO,
                Tipo = entity.DS_TIPO_VEICULO,
                CapacidadVolumen = entity.VL_CAPAC_VOLUMEN,
                CapacidadPeso = entity.VL_CAPAC_PESO,
                CapacidadPallet = entity.VL_CAPAC_PALLET,
                TieneRefrigeracion = this.MapStringToBoolean(entity.ID_FRIGORIFICADO),
                AdmiteCargaLateral = this.MapStringToBoolean(entity.ID_CARGA_LATERAL),
                AdmiteZorra = this.MapStringToBoolean(entity.ID_ADMITE_ZORRA),
                TieneSoloCabina = this.MapStringToBoolean(entity.ID_SOLO_CABINA),
                PorcentajeOcupacionPeso = entity.VL_PORCENTAJE_OCUPACION_PESO,
                PorcentajeOcupacionVolumen = entity.VL_PORCENTAJE_OCUPACION_VOLU,
                PorcentajeOcupacionPallet = entity.VL_PORCENTAJE_OCUPACION_PALLET,
                SincronizacionRealizada = this.MapStringToBoolean(entity.FL_SYNC_REALIZADA),
            };
        }

        public virtual T_TIPO_VEICULO MapToEntity(VehiculoEspecificacion especificacion)
        {
            return new T_TIPO_VEICULO
            {
                CD_TIPO_VEICULO = especificacion.Id,
                DS_TIPO_VEICULO = especificacion.Tipo,
                VL_CAPAC_VOLUMEN = especificacion.CapacidadVolumen,
                VL_CAPAC_PESO = especificacion.CapacidadPeso,
                VL_CAPAC_PALLET = especificacion.CapacidadPallet,
                ID_FRIGORIFICADO = this.MapBooleanToString(especificacion.TieneRefrigeracion),
                ID_CARGA_LATERAL = this.MapBooleanToString(especificacion.AdmiteCargaLateral),
                ID_ADMITE_ZORRA = this.MapBooleanToString(especificacion.AdmiteZorra),
                ID_SOLO_CABINA = this.MapBooleanToString(especificacion.TieneSoloCabina),
                VL_PORCENTAJE_OCUPACION_PESO = especificacion.PorcentajeOcupacionPeso,
                VL_PORCENTAJE_OCUPACION_VOLU = especificacion.PorcentajeOcupacionVolumen,
                VL_PORCENTAJE_OCUPACION_PALLET = especificacion.PorcentajeOcupacionPallet,
                FL_SYNC_REALIZADA = this.MapBooleanToString(especificacion.SincronizacionRealizada)
            };
        }
    }
}
