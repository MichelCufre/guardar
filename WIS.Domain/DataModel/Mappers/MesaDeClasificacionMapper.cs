using WIS.Domain.Recepcion;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
	public class MesaDeClasificacionMapper : Mapper
    {
        #region MapToObject 
        public virtual EstacionDeClasificacion MapToObject(T_ESTACION_CLASIFICACION estacion)
        {
            if (estacion == null)
                return null;

            return new EstacionDeClasificacion
            {
                Codigo = estacion.CD_ESTACION,
                Descripcion = estacion.DS_ESTACION,
                FechaAdicion = estacion.DT_ADDROW,
                FechaModificacion = estacion.DT_UPDROW,
                Predio = estacion.NU_PREDIO,
                Ubicacion = estacion.CD_ENDERECO,
            };
        }

        public virtual SugerenciaDeClasificacion MapToObject(V_REC410_SUGERENCIAS sugerencia)
        {
            return new SugerenciaDeClasificacion
            {
                Estacion = sugerencia.CD_ESTACION,
                Posicion = sugerencia.NU_POSICION,
                Destino = sugerencia.CD_ENDERECO_DESTINO,
                Zona = sugerencia.CD_ZONA,
                Equipo = sugerencia.CD_EQUIPO,
            };
        }

        #endregion

        #region MapToEntity

        public virtual T_ESTACION_CLASIFICACION MapToEntity(EstacionDeClasificacion estacion)
        {
            return new T_ESTACION_CLASIFICACION
            {
                CD_ESTACION = estacion.Codigo,
                DS_ESTACION = estacion.Descripcion,
                DT_ADDROW = estacion.FechaAdicion,
                DT_UPDROW = estacion.FechaModificacion,
                NU_PREDIO = estacion.Predio,
                CD_ENDERECO = estacion.Ubicacion,
            };
        }

        public virtual DetalleEtiquetaSinClasificar MapToEntity(V_DET_ETIQUETA_SIN_CLASIFICAR det)
        {
            if (det == null)
                return null;
            DetalleEtiquetaSinClasificar obj = new DetalleEtiquetaSinClasificar();
            obj.EtiquetaLote = det.NU_ETIQUETA_LOTE;
            obj.Agenda = det.NU_AGENDA;
            obj.EtiquetaExterna = det.NU_EXTERNO_ETIQUETA;
            obj.TipoEtiqueta = det.TP_ETIQUETA;
            obj.CodigoProducto = det.CD_PRODUTO;
            obj.DescripcionProducto = det.DS_PRODUTO;
            obj.Lote = det.NU_IDENTIFICADOR?.Trim()?.ToUpper();
            obj.Faixa = det.CD_FAIXA;
            obj.CodigoEmpresa = det.CD_EMPRESA;
            obj.DescEmpresa = det.NM_EMPRESA;
            obj.Cantidad = det.QT_PRODUTO;
            obj.Ubicacion = det.CD_ENDERECO;

            return obj;
        }

        #endregion
    }
}
