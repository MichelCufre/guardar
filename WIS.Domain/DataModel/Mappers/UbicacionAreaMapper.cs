using WIS.Domain.General;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class UbicacionAreaMapper : Mapper
    {

        public UbicacionAreaMapper()
        {
        }

        public virtual UbicacionArea MapToObject(T_TIPO_AREA area)
        {
            if (area == null)
                return null;

            return new UbicacionArea
            {
                Id = area.CD_AREA_ARMAZ,
                Descripcion = area.DS_AREA_ARMAZ,
                DisponibilizaStock = this.MapStringToBoolean(area.ID_DISP_ESTOQUE),
                EsAreaAveria = this.MapStringToBoolean(area.ID_AREA_AVARIA),
                EsAreaEmbarque = this.MapStringToBoolean(area.ID_AREA_EMBARQUE),
                EsAreaEspera = this.MapStringToBoolean(area.ID_AREA_ESPERA),
                EsAreaInventariable = this.MapStringToBoolean(area.FL_INVENTARIABLE),
                EsAreaPicking = this.MapStringToBoolean(area.ID_AREA_PICKING),
                EsAreaProblema = this.MapStringToBoolean(area.ID_AREA_PROBLEMA),
                EsAreaStockGeneral = this.MapStringToBoolean(area.ID_ESTOQUE_GERAL),
                PermiteVehiculo = this.MapStringToBoolean(area.ID_VEICULO),
                EsAreaMantenible = this.MapStringToBoolean(area.ID_PERMITE_MANTENIMIENTO)
            };
        }

        public virtual T_TIPO_AREA MapToEntity(UbicacionArea area)
        {
            return new T_TIPO_AREA
            {
                CD_AREA_ARMAZ = area.Id,
                DS_AREA_ARMAZ = area.Descripcion,
                ID_DISP_ESTOQUE = this.MapBooleanToString(area.DisponibilizaStock),
                ID_AREA_AVARIA = this.MapBooleanToString(area.EsAreaAveria),
                ID_AREA_EMBARQUE = this.MapBooleanToString(area.EsAreaEmbarque),
                ID_AREA_ESPERA = this.MapBooleanToString(area.EsAreaEspera),
                FL_INVENTARIABLE = this.MapBooleanToString(area.EsAreaInventariable),
                ID_AREA_PICKING = this.MapBooleanToString(area.EsAreaPicking),
                ID_AREA_PROBLEMA = this.MapBooleanToString(area.EsAreaProblema),
                ID_ESTOQUE_GERAL = this.MapBooleanToString(area.EsAreaStockGeneral),
                ID_VEICULO = this.MapBooleanToString(area.PermiteVehiculo),
            };
        }
    }
}
