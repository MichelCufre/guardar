using WIS.Domain.General;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class UbicacionTipoMapper : Mapper
    {

        public UbicacionTipoMapper()
        {
        }

        public virtual UbicacionTipo MapToObject(T_TIPO_ENDERECO entity)
        {
            if (entity == null)
                return null;

            return new UbicacionTipo
            {
                Id = entity.CD_TIPO_ENDERECO,
                Descripcion = entity.DS_TIPO_ENDERECO,
                Altura = entity.VL_ALTURA,
                Ancho = entity.VL_COMPRIMENTO,
                Largo = entity.VL_LARGURA,
                PesoMaximo = entity.VL_PESO_MAXIMO,
                CapacidadPallets = entity.QT_CAPAC_PALETES,
                IdTipoEstatura = entity.CD_TIPO_ESTRUTURA,
                PermiteVariosLotes = this.MapStringToBoolean(entity.ID_VARIOS_LOTES),
                PermiteVariosProductos = this.MapStringToBoolean(entity.ID_VARIOS_PRODUTOS),
                VolumenUnidadFacturacion = entity.QT_VOLUMEN_UNIDAD_FACTURACION,
                FechaInsercion = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UPDROW,
                IdUbicacionArea = entity.CD_AREA_ARMAZ,
                RespetaClase = this.MapStringToBoolean(entity.FL_RESPETA_CLASE),
            };
        }

        public virtual T_TIPO_ENDERECO MapToEntity(UbicacionTipo area)
        {
            return new T_TIPO_ENDERECO
            {
                CD_TIPO_ENDERECO = area.Id,
                DS_TIPO_ENDERECO = area.Descripcion,
                VL_ALTURA = area.Altura,
                VL_LARGURA = area.Largo,
                VL_COMPRIMENTO = area.Ancho,
                VL_PESO_MAXIMO = area.PesoMaximo,
                QT_CAPAC_PALETES = area.CapacidadPallets,
                CD_TIPO_ESTRUTURA = area.IdTipoEstatura,
                ID_VARIOS_LOTES = this.MapBooleanToString(area.PermiteVariosLotes),
                ID_VARIOS_PRODUTOS = this.MapBooleanToString(area.PermiteVariosProductos),
                QT_VOLUMEN_UNIDAD_FACTURACION = area.VolumenUnidadFacturacion,
                DT_ADDROW = area.FechaInsercion,
                DT_UPDROW = area.FechaModificacion,
                CD_AREA_ARMAZ = area.IdUbicacionArea,
                FL_RESPETA_CLASE = this.MapBooleanToString(area.RespetaClase),
            };
        }

        public virtual TipoDeEstructura MapToObject(V_PAR050_TIPO_ESTRUTURA entity)
        {
            if (entity == null) return null;

            return new TipoDeEstructura
            {
                Codigo = entity.CD_TP_ESTR,
                Tipo = entity.DS_TP_ESTR,
                idBloqueado = entity.ID_BLOCADO,
                idPortaPallet = entity.ID_GAIOLA,
                idPrateleria = entity.ID_PORTA_PALETE,
                idGailoa = entity.ID_PRATELEIRA
            };
        }
    }
}
