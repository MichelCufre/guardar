using WIS.Domain.General;
using WIS.Domain.Recepcion;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class EstrategiaMapper : Mapper
    {

        public virtual T_ALM_ESTRATEGIA MapToEntity(Estrategia estrategia)
        {
            return new T_ALM_ESTRATEGIA
            {
                NU_ALM_ESTRATEGIA = estrategia.NumeroEstrategia,
                DS_ALM_ESTRATEGIA = estrategia.Descripcion,
                DT_ADDROW = estrategia.FechaAdicion,
                DT_UPDROW = estrategia.FechaModificacion,
            };
        }
        public virtual V_REC275_LOGICAS MapToEntity(Logica logica)
        {
            return new V_REC275_LOGICAS
            {
                NU_ALM_LOGICA = logica.NumeroLogica,
                DS_ALM_LOGICA = logica.Descripcion,
            };
        }
        public virtual T_ALM_LOGICA_INSTANCIA MapToEntity(InstanciaLogica logicaEstrategia)
        {
            return new T_ALM_LOGICA_INSTANCIA
            {
                DS_ALM_LOGICA_INSTANCIA = logicaEstrategia.Descripcion,
                DT_ADDROW = logicaEstrategia.FechaRegistro,
                DT_UPDROW = logicaEstrategia.FechaModificacion,
                NU_ALM_ESTRATEGIA = logicaEstrategia.Estrategia,
                NU_ALM_LOGICA = logicaEstrategia.Logica,
                NU_ALM_LOGICA_INSTANCIA = logicaEstrategia.Id,
                NU_ORDEN = logicaEstrategia.Orden,
                VL_AUDITORIA = logicaEstrategia.DatoAuditoria,
                FL_ORDEN_ASC = logicaEstrategia.OrdenarAscendente,
            };
        }
        public virtual T_ALM_LOGICA_INSTANCIA_PARAM MapToEntity(InstanciaLogicaParametro instanciaParametro)
        {
            return new T_ALM_LOGICA_INSTANCIA_PARAM
            {
                NU_ALM_LOGICA_INSTANCIA_PARAM = instanciaParametro.Id,
                NU_ALM_LOGICA_INSTANCIA = instanciaParametro.Instancia,
                DT_ADDROW = instanciaParametro.FechaRegistro,
                DT_UPDROW = instanciaParametro.FechaModificacion,
                NU_ALM_PARAMETRO = instanciaParametro.Parametro.Id,
                NU_ALM_LOGICA = instanciaParametro.Logica,
                VL_ALM_PARAMETRO = instanciaParametro.Valor,
                VL_AUDITORIA = instanciaParametro.DatoAuditoria,
            };
        }
        public virtual T_ALM_ASOCIACION MapToEntity(AsociacionEstrategia asociacionEstrategia)
        {
            return new T_ALM_ASOCIACION
            {
                CD_ALM_OPERATIVA_ASOCIABLE = asociacionEstrategia.Operativa.Codigo,
                CD_CLASSE = asociacionEstrategia.Clase,
                CD_EMPRESA = asociacionEstrategia.Empresa,
                CD_GRUPO = asociacionEstrategia.Grupo,
                CD_PRODUTO = asociacionEstrategia.Producto,
                DT_ADDROW = asociacionEstrategia.FechaRegistro,
                DT_UPDROW = asociacionEstrategia.FechaModificacion,
                NU_ALM_ASOCIACION = asociacionEstrategia.Id,
                NU_ALM_ESTRATEGIA = asociacionEstrategia.Estrategia,
                NU_PREDIO = asociacionEstrategia.Predio,
                TP_ALM_ASOCIACION = asociacionEstrategia.Tipo,
                TP_ALM_OPERATIVA_ASOCIABLE = asociacionEstrategia.Operativa.Tipo,
                VL_AUDITORIA = asociacionEstrategia.DatoAuditoria,
            };
        }
        public virtual AsociacionEstrategia MapToObject(T_ALM_ASOCIACION estrategiaAsociacion)
        {
            return new AsociacionEstrategia
            {
                Clase = estrategiaAsociacion.CD_CLASSE,
                Empresa = estrategiaAsociacion.CD_EMPRESA,
                Grupo = estrategiaAsociacion.CD_GRUPO,
                Producto = estrategiaAsociacion.CD_PRODUTO,
                FechaRegistro = estrategiaAsociacion.DT_ADDROW,
                FechaModificacion = estrategiaAsociacion.DT_UPDROW,
                Id = estrategiaAsociacion.NU_ALM_ASOCIACION,
                Estrategia = estrategiaAsociacion.NU_ALM_ESTRATEGIA,
                Predio = estrategiaAsociacion.NU_PREDIO,
                Tipo = estrategiaAsociacion.TP_ALM_ASOCIACION,
                DatoAuditoria = estrategiaAsociacion.VL_AUDITORIA,
                Operativa = new OperativaAlmacenaje
                {
                    Codigo = estrategiaAsociacion.CD_ALM_OPERATIVA_ASOCIABLE,
                    Tipo = estrategiaAsociacion.TP_ALM_OPERATIVA_ASOCIABLE,
                }
            };
        }
        public virtual InstanciaLogicaParametro MapToObject(T_ALM_LOGICA_INSTANCIA_PARAM instanciaParametro)
        {
            return new InstanciaLogicaParametro
            {
                Id = instanciaParametro.NU_ALM_LOGICA_INSTANCIA_PARAM,
                Instancia = instanciaParametro.NU_ALM_LOGICA_INSTANCIA,
                FechaRegistro = instanciaParametro.DT_ADDROW,
                FechaModificacion = instanciaParametro.DT_UPDROW,
                Parametro = new LogicaParametro
                {
                    Id = instanciaParametro.NU_ALM_PARAMETRO
                },
                Logica = instanciaParametro.NU_ALM_LOGICA,
                Valor = instanciaParametro.VL_ALM_PARAMETRO,
                DatoAuditoria = instanciaParametro.VL_AUDITORIA,
            };
        }

        public virtual Estrategia MapToObject(T_ALM_ESTRATEGIA estrategia)
        {
            return new Estrategia
            {
                NumeroEstrategia = estrategia.NU_ALM_ESTRATEGIA,
                Descripcion = estrategia.DS_ALM_ESTRATEGIA,
                FechaAdicion = estrategia.DT_ADDROW,
                FechaModificacion = estrategia.DT_UPDROW,
            };
        }
        public virtual Logica MapToObject(V_REC275_LOGICAS logica)
        {
            return new Logica
            {
                NumeroLogica = logica.NU_ALM_LOGICA,
                Descripcion = logica.DS_ALM_LOGICA,
            };
        }

        public virtual ParametroDefault MapToObject(V_REC275_PARAMETROS parametros)
        {
            return new ParametroDefault
            {
                NumeroParametro = parametros.NU_ALM_PARAMETRO,
                NumeroLogica = parametros.NU_ALM_LOGICA,
                DescripcionParametro = parametros.DS_ALM_PARAMETRO,
                Parametro = parametros.NM_ALM_PARAMETRO,
                Valor = parametros.VL_ALM_PARAMETRO_DEFAULT,
            };
        }
        public virtual InstanciaLogica MapToObject(T_ALM_LOGICA_INSTANCIA logicaEstrategia)
        {
            return new InstanciaLogica
            {
                Descripcion = logicaEstrategia.DS_ALM_LOGICA_INSTANCIA,
                FechaRegistro = logicaEstrategia.DT_ADDROW,
                FechaModificacion = logicaEstrategia.DT_UPDROW,
                Estrategia = logicaEstrategia.NU_ALM_ESTRATEGIA,
                Logica = logicaEstrategia.NU_ALM_LOGICA,
                Id = logicaEstrategia.NU_ALM_LOGICA_INSTANCIA,
                Orden = logicaEstrategia.NU_ORDEN,
                DatoAuditoria = logicaEstrategia.VL_AUDITORIA,
                OrdenarAscendente = logicaEstrategia.FL_ORDEN_ASC,
            };
        }

        public virtual EstrategiaSugerencia MapToObject(T_ALM_SUGERENCIA sugerencia)
        {
            return new EstrategiaSugerencia
            {
                NumeroEstrategia = sugerencia.NU_ALM_ESTRATEGIA,
                CodigoOperativaAsociable = sugerencia.CD_ALM_OPERATIVA_ASOCIABLE,
                NumeroPredio = sugerencia.NU_PREDIO,
                CodigoClase = sugerencia.CD_CLASSE,
                CodigoGrupo = sugerencia.CD_GRUPO,
                CodigoEmpresa = sugerencia.CD_EMPRESA,
                CodigoProducto = sugerencia.CD_PRODUTO,
                Referencia = sugerencia.CD_REFERENCIA,
                Agrupador = sugerencia.CD_AGRUPADOR,
                CodigoInstanciaLogica = sugerencia.NU_ALM_LOGICA_INSTANCIA,
                UbicacionSugerida = sugerencia.CD_ENDERECO_SUGERIDO,
                Calculo = sugerencia.VL_TIEMPO_CALCULO,
                Estado = sugerencia.CD_ESTADO,
                MotivoRechazo = sugerencia.CD_MOTVO_RECHAZO,
                FechaAdicion = sugerencia.DT_ADDROW,
                FechaModificacion = sugerencia.DT_UPDROW,
                Funcionario = sugerencia.CD_FUNCIONARIO,
                NuTransaccion = sugerencia.NU_TRANSACCION,
                TipoOperativa = sugerencia.TP_ALM_OPERATIVA_ASOCIABLE,
                NuAlmSugerencia = sugerencia.NU_ALM_SUGERENCIA,
            };
        }

        public virtual T_ALM_SUGERENCIA MapToEntity(EstrategiaSugerencia sugerencia)
        {
            return new T_ALM_SUGERENCIA
            {
                NU_ALM_ESTRATEGIA = sugerencia.NumeroEstrategia,
                CD_ALM_OPERATIVA_ASOCIABLE = sugerencia.CodigoOperativaAsociable,
                NU_PREDIO = sugerencia.NumeroPredio,
                CD_CLASSE = sugerencia.CodigoClase,
                CD_GRUPO = sugerencia.CodigoGrupo,
                CD_EMPRESA = sugerencia.CodigoEmpresa,
                CD_PRODUTO = sugerencia.CodigoProducto,
                CD_REFERENCIA = sugerencia.Referencia,
                CD_AGRUPADOR = sugerencia.Agrupador,
                NU_ALM_LOGICA_INSTANCIA = sugerencia.CodigoInstanciaLogica,
                CD_ENDERECO_SUGERIDO = sugerencia.UbicacionSugerida,
                VL_TIEMPO_CALCULO = sugerencia.Calculo,
                CD_ESTADO = sugerencia.Estado,
                CD_MOTVO_RECHAZO = sugerencia.MotivoRechazo,
                DT_ADDROW = sugerencia.FechaAdicion,
                DT_UPDROW = sugerencia.FechaModificacion,
                CD_FUNCIONARIO = sugerencia.Funcionario,
                NU_TRANSACCION = sugerencia.NuTransaccion,
                TP_ALM_OPERATIVA_ASOCIABLE = sugerencia.TipoOperativa,
                NU_ALM_SUGERENCIA= sugerencia.NuAlmSugerencia,
            };
        }
    }
}
