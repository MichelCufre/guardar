using WIS.Domain.General;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class UbicacionMapper : Mapper
    {
        public UbicacionMapper()
        {

        }
        public virtual Ubicacion MapToObject(T_ENDERECO_ESTOQUE entity)
        {
            if (entity == null) return null;

            return new Ubicacion
            {
                Id = entity.CD_ENDERECO,
                IdEmpresa = entity.CD_EMPRESA,
                IdUbicacionTipo = entity.CD_TIPO_ENDERECO,
                IdProductoRotatividad = entity.CD_ROTATIVIDADE,
                IdProductoFamilia = entity.CD_FAMILIA_PRINCIPAL,
                CodigoClase = entity.CD_CLASSE,
                CodigoSituacion = entity.CD_SITUACAO,
                EsUbicacionBaja = this.MapStringToBoolean(entity.ID_ENDERECO_BAIXO),
                EsUbicacionSeparacion = this.MapStringToBoolean(entity.ID_ENDERECO_SEP),
                NecesitaReabastecer = this.MapStringToBoolean(entity.ID_NECESSIDADE_RESUPRIR),
                FechaModificacion = entity.DT_UPDROW,
                FechaInsercion = entity.DT_ADDROW,
                CodigoControl = entity.CD_CONTROL,
                IdUbicacionArea = entity.CD_AREA_ARMAZ,
                FacturacionComponente = entity.NU_COMPONENTE,
                IdUbicacionZona = entity.CD_ZONA_UBICACION,
                NumeroPredio = entity.NU_PREDIO,
                Bloque = entity.ID_BLOQUE,
                Calle = entity.ID_CALLE,
                Columna = entity.NU_COLUMNA,
                Altura = entity.NU_ALTURA,
                DominioSector = entity.ND_SECTOR,
                IdControlAcceso = entity.CD_CONTROL_ACCESO,
                Profundidad= entity.NU_PROFUNDIDAD,
                CodigoBarras = entity.CD_BARRAS,
                OrdenDefecto = entity.NU_ORDEN_DEFAULT
            };
        }

        public virtual T_ENDERECO_ESTOQUE MapToEntity(Ubicacion ubicacion)
        {
            if (ubicacion == null) return null;

            return new T_ENDERECO_ESTOQUE
            {
                CD_ENDERECO = ubicacion.Id,
                CD_EMPRESA = ubicacion.IdEmpresa,
                CD_TIPO_ENDERECO = ubicacion.IdUbicacionTipo,
                CD_ROTATIVIDADE = ubicacion.IdProductoRotatividad,
                CD_FAMILIA_PRINCIPAL = ubicacion.IdProductoFamilia,
                CD_CLASSE = ubicacion.CodigoClase,
                CD_SITUACAO = ubicacion.CodigoSituacion,
                ID_ENDERECO_BAIXO = this.MapBooleanToString(ubicacion.EsUbicacionBaja),
                ID_ENDERECO_SEP = this.MapBooleanToString(ubicacion.EsUbicacionSeparacion),
                ID_NECESSIDADE_RESUPRIR = this.MapBooleanToString(ubicacion.NecesitaReabastecer),
                DT_UPDROW = ubicacion.FechaModificacion,
                DT_ADDROW = ubicacion.FechaInsercion,
                CD_CONTROL = ubicacion.CodigoControl,
                CD_AREA_ARMAZ = ubicacion.IdUbicacionArea,
                NU_COMPONENTE = ubicacion.FacturacionComponente,
                CD_ZONA_UBICACION = NullIfEmpty(ubicacion.IdUbicacionZona),
                NU_PREDIO = ubicacion.NumeroPredio,
                ID_BLOQUE = ubicacion.Bloque,
                ID_CALLE = ubicacion.Calle,
                NU_COLUMNA = ubicacion.Columna,
                NU_ALTURA = ubicacion.Altura,
                ND_SECTOR = ubicacion.DominioSector,
                CD_CONTROL_ACCESO = NullIfEmpty(ubicacion.IdControlAcceso),
                NU_PROFUNDIDAD = ubicacion.Profundidad,
                CD_BARRAS = ubicacion.CodigoBarras,
                NU_ORDEN_DEFAULT = ubicacion.OrdenDefecto
            };
        }
    }
}