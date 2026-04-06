using WIS.Domain.StockEntities;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class UnidadTransporteMapper : Mapper
    {
        public UnidadTransporteMapper()
        {

        }

        public virtual UnidadTransporte MapToObject(T_UNIDAD_TRANSPORTE entity)
        {
            if (entity == null) return null;

            return new UnidadTransporte
            {
                CodigoUnidadBulto = entity.CD_UNIDAD_BULTO,
                CodigoBarras = entity.CD_BARRAS,
                CodigoGrupo = entity.CD_GRUPO,
                Situacion = entity.CD_SITUACAO,
                FechaInsercion = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UPDROW,
                NumeroExternoUnidad = entity.NU_EXTERNO_UNIDAD,
                NumeroUnidadTransporte = entity.NU_UNIDAD_TRANSPORTE,
                Peso = entity.PS_REAL,
                CantidadBultos = entity.QT_BULTO,
                Profundidad = entity.VL_PROFUNDIDADE,
                CantidadContenedores = entity.QT_CONTENEDOR,
                Ubicacion = entity.CD_ENDERECO,
                UbicacionDestino = entity.CD_ENDERECO_SUGERIDO,
                TipoUnidadTransporte = entity.TP_UNIDAD_TRANSPORTE,
                Altura = entity.VL_ALTURA,
                Ancho = entity.VL_LARGURA
            };
        }

        public virtual T_UNIDAD_TRANSPORTE MapToEntity(UnidadTransporte obj)
        {
            return new T_UNIDAD_TRANSPORTE
            {
                NU_UNIDAD_TRANSPORTE = obj.NumeroUnidadTransporte,
                NU_EXTERNO_UNIDAD = obj.NumeroExternoUnidad,
                TP_UNIDAD_TRANSPORTE = obj.TipoUnidadTransporte,
                CD_ENDERECO = obj.Ubicacion,
                CD_SITUACAO = obj.Situacion,
                CD_BARRAS = obj.CodigoBarras,
                DT_ADDROW = obj.FechaInsercion,
                DT_UPDROW = obj.FechaModificacion,
                CD_GRUPO = obj.CodigoGrupo,
                CD_ENDERECO_SUGERIDO = obj.UbicacionDestino,
                PS_REAL = obj.Peso,
                VL_ALTURA = obj.Altura,
                VL_LARGURA = obj.Ancho,
                VL_PROFUNDIDADE = obj.Profundidad,
                CD_UNIDAD_BULTO = obj.CodigoUnidadBulto,
                QT_BULTO = obj.CantidadBultos,
                QT_CONTENEDOR = obj.CantidadContenedores
            };
        }
    }
}
