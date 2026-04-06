using WIS.Domain.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class InterfazMapper : Mapper
    {
        public virtual InterfazEjecucion MapToObject(T_INTERFAZ_EJECUCION entity)
        {
            if (entity == null) return null;

            return new InterfazEjecucion
            {
                Id = entity.NU_INTERFAZ_EJECUCION,
                CdInterfazExterna = entity.CD_INTERFAZ_EXTERNA,
                Archivo = entity.NM_ARCHIVO,
                Situacion = entity.CD_SITUACAO,
                Comienzo = entity.DT_COMIENZO,
                FechaSituacion = entity.DT_SITUACAO,
                ErrorCarga = entity.FL_ERROR_CARGA,
                ErrorProcedimiento = entity.FL_ERROR_PROCEDIMIENTO,
                FuncionarioAceptacion = entity.CD_FUNCIONARIO_ACEPTACION,
                Referencia = entity.DS_REFERENCIA,
                NdSituacion = entity.ND_SITUACION,
                Empresa = entity.CD_EMPRESA,
                GrupoConsulta = entity.CD_GRUPO_CONSULTA,
                UserId = entity.USERID,
                Procesado = entity.ID_PROCESADO,
                InterfazExterna = MapToObject(entity.T_INTERFAZ_EXTERNA)
            };
        }

        public virtual InterfazExterna MapToObject(T_INTERFAZ_EXTERNA entity)
        {
            if (entity == null) return null;

            return new InterfazExterna()
            {
                CodigoInterfaz = entity.CD_INTERFAZ,
                CodigoInterfazExterna = entity.CD_INTERFAZ_EXTERNA,
                Descripcion = entity.DS_INTERFAZ_EXTERNA,
                FechaALta = entity.DT_ADDROW,
                ReProcesable = entity.FL_RE_PROCESABLE,
                IdSecuencia = entity.ID_SECUENCIA,
                ComienzoProceso = entity.LN_COMIENZO_PROCESO,
                NombreProcedimiento = entity.NM_PROCEDIMIENTO,
                Interfaces = null,
                NuReconoOrden = entity.NU_RECONO_ORDEN,
                TipoArchivo = entity.TP_ARCHIVO,
                Delimitador = entity.VL_DELIMITADOR,
                DelimitadorSegmento = entity.VL_DELIMITADOR_SEGMENTO,
                ProcExtraeSecuencia = entity.VL_PROC_EXTRAE_SECUENCIA,
                ReconoContenido = entity.VL_RECONO_CONTENIDO,
                ReconoExtension = entity.VL_RECONO_EXTENSION,
                ReconoPostfijo = entity.VL_RECONO_POSTFIJO,
                ReconoPrefijo = entity.VL_RECONO_PREFIJO,
                Endpoint = entity.VL_ENDPOINT,
                EndpointReprocess = entity.VL_ENDPOINT_REPROCESS,
                ParametroDeHabilitacion = entity.VL_PARAMETRO_HABILITACION,
                Interfaz = MapToObject(entity.T_INTERFAZ)
            };
        }

        public virtual Interfaz MapToObject(T_INTERFAZ entity)
        {
            if (entity == null) return null;

            return new Interfaz
            {
                Id = entity.CD_INTERFAZ,
                DescripcionInterfaz = entity.DS_INTERFAZ,
                NombreInterfaz = entity.NM_INTERFAZ,
                NombreProcedimiento = entity.NM_PROCEDIMIENTO,
                DescripcionObjeto = entity.DS_OBJETO,
                EsperarAprobacion = this.MapStringToBoolean(entity.FL_ESPERAR_APROBACION),
                IgnorarErrorCarga = this.MapStringToBoolean(entity.FL_IGNORAR_ERROR_CARGA),
                FechaAlta = entity.DT_ADDROW,
                IdEntradaSalida = entity.ID_ENTRADA_SALIDA,
                ObjetoConsulta = entity.VL_OBJETO_CONSULTA,
                TipoObjetoDb = entity.TP_OBJETO_BD
            };
        }

        public virtual InterfazError MapToObject(T_INTERFAZ_EJECUCION_ERROR entity)
        {
            if (entity == null) return null;

            return new InterfazError
            {
                Id = entity.NU_INTERFAZ_EJECUCION,
                NroError = entity.NU_ERROR,
                Registro = entity.NU_REGISTRO,
                Referencia = entity.DS_REFERENCIA,
                Parametro = entity.CD_PARAMETRO,
                CodigoError = entity.DS_REFERENCIA,
                Descripcion = entity.DS_ERROR
            };
        }

        public virtual InterfazData MapToObject(T_INTERFAZ_EJECUCION_DATA entity)
        {
            if (entity == null)
                return null;

            return new InterfazData
            {
                Id = entity.NU_INTERFAZ_EJECUCION,
                Alta = entity.DT_ADDROW,
                Data = entity.DATA
            };
        }

        public virtual T_INTERFAZ_EJECUCION MapToEntity(InterfazEjecucion obj)
        {
            if (obj == null)
                return null;

            return new T_INTERFAZ_EJECUCION
            {
                NU_INTERFAZ_EJECUCION = obj.Id,
                CD_INTERFAZ_EXTERNA = obj.CdInterfazExterna,
                NM_ARCHIVO = obj.Archivo,
                CD_SITUACAO = obj.Situacion,
                DT_COMIENZO = obj.Comienzo,
                DT_SITUACAO = obj.FechaSituacion,
                FL_ERROR_CARGA = obj.ErrorCarga,
                FL_ERROR_PROCEDIMIENTO = obj.ErrorProcedimiento,
                CD_FUNCIONARIO_ACEPTACION = obj.FuncionarioAceptacion,
                DS_REFERENCIA = obj.Referencia,
                ND_SITUACION = obj.NdSituacion,
                CD_EMPRESA = obj.Empresa,
                CD_GRUPO_CONSULTA = obj.GrupoConsulta,
                USERID = obj.UserId,
                ID_PROCESADO = obj.Procesado
            };
        }

        public virtual T_INTERFAZ_EJECUCION_ERROR MapToEntity(InterfazError obj)
        {
            if (obj == null)
                return null;

            return new T_INTERFAZ_EJECUCION_ERROR
            {
                NU_INTERFAZ_EJECUCION = obj.Id,
                NU_ERROR = obj.NroError,
                NU_REGISTRO = obj.Registro,
                DS_REFERENCIA = obj.Referencia,
                CD_PARAMETRO = obj.Parametro,
                CD_ERROR = obj.CodigoError,
                DS_ERROR = obj.Descripcion
            };
        }

    }
}
