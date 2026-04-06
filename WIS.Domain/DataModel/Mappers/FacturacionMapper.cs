using WIS.Domain.Facturacion;
using WIS.Domain.General;
using WIS.Domain.OrdenTarea;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class FacturacionMapper : Mapper
    {
        public FacturacionMapper()
        {

        }

        #region MapToObject 
        public virtual FacturacionCodigoComponente MapToObject(T_FACTURACION_CODIGO_COMPONEN entity)
        {
            return new FacturacionCodigoComponente
            {
                Id = entity.CD_FACTURACION,
                Descripcion = entity.DS_SIGNIFICADO,
                NumeroComponente = entity.NU_COMPONENTE,
                NumeroCuentaContable = entity.NU_CUENTA_CONTABLE,
                FechaIngresado = entity.DT_ADDROW,
                FechaActualizado = entity.DT_UPDROW,
                NumeroTransaccion = entity.NU_TRANSACCION,
            };
        }

        public virtual OrdenTareaObjeto MapToObject(T_ORT_ORDEN_TAREA entity)
        {
            return new OrdenTareaObjeto
            {
                NuTarea = entity.NU_ORDEN_TAREA,
                NuOrden = entity.NU_ORT_ORDEN,
                CdTarea = entity.CD_TAREA,
                Empresa = entity.CD_EMPRESA,
                CdFuncionarioAddrow = entity.CD_FUNCIONARIO_ADDROW,
                DtAddrow = entity.DT_ADDROW,
                DtUpdrow = entity.DT_UPDROW,
                Resuelta = entity.FL_RESUELTA
            };
        }

        public virtual Componente MapToObject(V_ORT_FUNC_COMP_COR18 entity)
        {
            return new Componente
            {
                NU_COMPONENTE = entity.NU_COMPONENTE,
                DS_SIGNIFICADO = entity.DS_SIGNIFICADO,
            };
        }

        public virtual Pallet MapToObject(V_REG605_PALLETS entity)
        {
            return new Pallet
            {
                Descripcion = entity.DS_PALLET,
                Id = entity.CD_PALLET,
                FechaInsercion = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UPDROW,
            };
        }

        public virtual FacturacionProceso MapToObject(T_FACTURACION_PROCESO entity)
        {
            return new FacturacionProceso
            {
                CodigoProceso = entity.CD_PROCESO,
                CodigoSituacionError = entity.CD_SITUACAO_ERROR,
                CodigoFacturacion = entity.CD_FACTURACION,
                NombreProcedimiento = entity.NM_PROCEDIMIENTO,
                DescripcionProceso = entity.DS_PROCESO,
                NumeroComponente = entity.NU_COMPONENTE,
                NumeroCuentaContable = entity.NU_CUENTA_CONTABLE,
                TipoProceso = entity.TP_PROCESO,
                EjecucionPorHora = entity.FL_EJEC_POR_HORA
            };
        }

        public virtual FacturacionEjecucion MapToObject(T_FACTURACION_EJECUCION entity)
        {
            return new FacturacionEjecucion
            {
                NumeroEjecucion = entity.NU_EJECUCION,
                Nombre = entity.NM_EJECUCION,
                EjecucionPorHora = entity.FL_EJEC_POR_HORA,
                FechaDesde = entity.DT_DESDE,
                FechaHasta = entity.DT_HASTA,
                CorteQuincena = entity.DT_CORTE_QUINCENA,
                FechaIngresado = entity.DT_ADDROW,
                FechaProgramacion = entity.DT_PROGRAMACION,
                FechaEjecucion = entity.DT_EJECUCION,
                FechaAprobacion = entity.DT_APROBACION,
                FechaEnviada = entity.DT_ENVIADA,
                FechaAnulacion = entity.DT_ANULACION,
                CodigoFuncEjecucion = entity.CD_FUNC_EJECUCION,
                CodigoFuncProgramacion = entity.CD_FUNC_PROGRAMACION,
                CodigoFuncAprobacion = entity.CD_FUNC_APROBACION,
                CodigoFuncAnulacion = entity.CD_FUNC_ANULACION,
                CodigoFuncEnviada = entity.CD_FUNC_ENVIADA,
                CodigoSituacion = entity.CD_SITUACAO,
                FechaModificacion = entity.DT_UPDROW,
            };
        }

        public virtual CuentaContable MapToObject(T_FACTURACION_CUENTA_CONTABLE entity)
        {
            return new CuentaContable
            {
                Id = entity.NU_CUENTA_CONTABLE,
                Descripcion = entity.DS_CUENTA_CONTABLE
            };
        }

        public virtual FacturacionEjecucionEmpresa MapToObject(T_FACTURACION_EJEC_EMPRESA entity)
        {
            if (entity == null)
                return null;

            return new FacturacionEjecucionEmpresa
            {
                NumeroEjecucion = entity.NU_EJECUCION,
                CodigoEmpresa = entity.CD_EMPRESA,
                CodigoProceso = entity.CD_PROCESO,
                CodigoSituacion = entity.CD_SITUACAO,
                Estado = entity.ID_ESTADO,
                FechaDesde = entity.DT_DESDE,
                FechaHasta = entity.DT_HASTA,
                FechaModificacion = entity.DT_UPDROW
            };
        }

        public virtual FacturacionResultado MapToObject(T_FACTURACION_RESULTADO entity)
        {
            if (entity == null)
                return null;

            return new FacturacionResultado
            {
                NumeroEjecucion = entity.NU_EJECUCION,
                CodigoEmpresa = entity.CD_EMPRESA,
                CodigoUnidadMedida = entity.CD_UNIDADE_MEDIDA,
                CodigoFacturacion = entity.CD_FACTURACION,
                CodigoFuncAprobacionRechazo = entity.CD_FUNC_APROBACION_RECHAZO,
                CodigoSituacion = entity.CD_SITUACAO,
                CantidadResultado = entity.QT_RESULTADO,
                CodigoProceso = entity.CD_PROCESO,
                DescripcionAdicional = entity.DS_ADICIONAL,
                FechaIngresado = entity.DT_ADDROW,
                FechaActualizacion = entity.DT_UPDROW,
                FechaAprobacionRechazo = entity.DT_APROBACION_RECHAZO,
                NumeroComponente = entity.NU_COMPONENTE,
                NumeroCuentaContable = entity.NU_CUENTA_CONTABLE,
                NumeroFactura = entity.NU_FACTURA,
                NumeroTicketInterfazFacturacion = entity.NU_TICKET_INTERFAZ_FACTURACION,
                PrecioUnitario = entity.VL_PRECIO_UNITARIO,
                PrecioMinimo = entity.VL_PRECIO_MINIMO,
                Moneda = entity.CD_MONEDA,
                NumeroTransaccion = entity.NU_TRANSACCION,
                NumeroTransaccionDelete = entity.NU_TRANSACCION_DELETE,
            };
        }

        public virtual FacturacionPalletDet MapToObject(T_FACTURACION_PALLET_DET entity)
        {
            if (entity == null)
                return null;

            return new FacturacionPalletDet
            {
                NumeroPalletDet = entity.NU_PALLET_DET,
                NumeroComponente = entity.NU_COMPONENTE,
                NumeroBultosFacturados = entity.NU_BULTOS_FACTURADOS,
                NumeroPallet = entity.NU_PALLET,
                NumeroEjecucionFacturacion = entity.NU_EJECUCION_FACTURACION,
                AplicoMinimo = entity.FL_APLICO_MINIMO,
                CantidadResultado = entity.QT_RESULTADO,
                CodigoEmpresa = entity.CD_EMPRESA,
                CodigoFacturacion = entity.CD_FACTURACION,
                Estado = entity.ID_ESTADO,
                FechaDesde = entity.DT_DESDE,
                FechaHasta = entity.DT_HASTA
            };
        }

        public virtual FacturacionCodigo MapToObject(T_FACTURACION_CODIGO entity)
        {
            return new FacturacionCodigo
            {
                CodigoFacturacion = entity.CD_FACTURACION,
                DescripcionFacturacion = entity.DS_FACTURACION,
                TipoCalculo = entity.TP_CALCULO,
                FechaIngresado = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UPDROW,
            };
        }

        public virtual FacturacionEmpresaProceso MapToObject(T_FACTURACION_EMPRESA_PROCESO entity)
        {
            if (entity == null)
                return null;

            return new FacturacionEmpresaProceso
            {
                CodigoEmpresa = entity.CD_EMPRESA,
                CodigoProceso = entity.CD_PROCESO,
                Resultado = entity.QT_RESULTADO,
                SituacionError = entity.CD_SITUACAO_ERROR,
                TipoUltimoProceso = entity.TP_ULTIMO_PROCESO,
                UltimoProceso = entity.HR_ULTIMO_PROCESO,
                FechaModificacion = entity.DT_UPDROW,
                NumeroTransaccion = entity.NU_TRANSACCION,
                NumeroTransaccionDelete = entity.NU_TRANSACCION_DELETE,
            };
        }

        public virtual FacturacionUnidadMedida MapToObject(T_FACTURACION_UNIDAD_MEDIDA entity)
        {
            return new FacturacionUnidadMedida
            {
                UnidadMedida = entity.CD_UNIDADE_MEDIDA,
                NumeroComponente = entity.NU_COMPONENTE,
                Uso = entity.ID_USO,
                NumeroTransaccion = entity.NU_TRANSACCION,
            };
        }

        public virtual FacturacionUnidadMedidaEmpresa MapToObject(T_FACTURACION_UND_MEDIDA_EMP entity)
        {
            return new FacturacionUnidadMedidaEmpresa
            {
                UnidadMedida = entity.CD_UNIDADE_MEDIDA,
                Empresa = entity.CD_EMPRESA,
                Funcionario = entity.CD_FUNCIONARIO,
                Fecha = entity.DT_ADDROW
            };
        }
        #endregion

        #region MapToEntity
        public virtual T_FACTURACION_CODIGO_COMPONEN MapToEntity(FacturacionCodigoComponente resultadoEjecucion)
        {
            return new T_FACTURACION_CODIGO_COMPONEN
            {
                CD_FACTURACION = resultadoEjecucion.Id,
                DS_SIGNIFICADO = resultadoEjecucion.Descripcion,
                NU_COMPONENTE = resultadoEjecucion.NumeroComponente,
                NU_CUENTA_CONTABLE = NullIfEmpty(resultadoEjecucion.NumeroCuentaContable),
                DT_ADDROW = resultadoEjecucion.FechaIngresado,
                DT_UPDROW = resultadoEjecucion.FechaActualizado,
                NU_TRANSACCION = resultadoEjecucion.NumeroTransaccion,
            };
        }

        public virtual T_FACTURACION_PROCESO MapToEntity(FacturacionProceso facturacionProceso)
        {
            return new T_FACTURACION_PROCESO
            {
                CD_PROCESO = facturacionProceso.CodigoProceso,
                CD_FACTURACION = facturacionProceso.CodigoFacturacion,
                NM_PROCEDIMIENTO = facturacionProceso.NombreProcedimiento,
                DS_PROCESO = facturacionProceso.DescripcionProceso,
                NU_COMPONENTE = facturacionProceso.NumeroComponente,
                NU_CUENTA_CONTABLE = facturacionProceso.NumeroCuentaContable,
                TP_PROCESO = facturacionProceso.TipoProceso,
                FL_EJEC_POR_HORA = facturacionProceso.EjecucionPorHora,
                CD_SITUACAO_ERROR = facturacionProceso.CodigoSituacionError
            };
        }

        public virtual T_FACTURACION_EJECUCION MapToEntity(FacturacionEjecucion facturacionEjecucion)
        {
            return new T_FACTURACION_EJECUCION
            {
                NU_EJECUCION = facturacionEjecucion.NumeroEjecucion,
                NM_EJECUCION = facturacionEjecucion.Nombre,
                FL_EJEC_POR_HORA = facturacionEjecucion.EjecucionPorHora,
                DT_DESDE = facturacionEjecucion.FechaDesde,
                DT_HASTA = facturacionEjecucion.FechaHasta,
                DT_CORTE_QUINCENA = facturacionEjecucion.CorteQuincena,
                DT_ADDROW = facturacionEjecucion.FechaIngresado,
                DT_PROGRAMACION = facturacionEjecucion.FechaProgramacion,
                DT_EJECUCION = facturacionEjecucion.FechaEjecucion,
                DT_APROBACION = facturacionEjecucion.FechaAprobacion,
                DT_ENVIADA = facturacionEjecucion.FechaEnviada,
                DT_ANULACION = facturacionEjecucion.FechaAnulacion,
                CD_FUNC_EJECUCION = facturacionEjecucion.CodigoFuncEjecucion,
                CD_FUNC_PROGRAMACION = facturacionEjecucion.CodigoFuncProgramacion,
                CD_FUNC_APROBACION = facturacionEjecucion.CodigoFuncAprobacion,
                CD_FUNC_ANULACION = facturacionEjecucion.CodigoFuncAnulacion,
                CD_FUNC_ENVIADA = facturacionEjecucion.CodigoFuncEnviada,
                CD_SITUACAO = facturacionEjecucion.CodigoSituacion,
                DT_UPDROW = facturacionEjecucion.FechaModificacion,
            };
        }

        public virtual T_FACTURACION_EJEC_EMPRESA MapToEntity(FacturacionEjecucionEmpresa facturacionEjecucionEmpresa)
        {
            return new T_FACTURACION_EJEC_EMPRESA
            {
                NU_EJECUCION = facturacionEjecucionEmpresa.NumeroEjecucion,
                CD_EMPRESA = facturacionEjecucionEmpresa.CodigoEmpresa,
                CD_PROCESO = facturacionEjecucionEmpresa.CodigoProceso,
                CD_SITUACAO = facturacionEjecucionEmpresa.CodigoSituacion,
                ID_ESTADO = facturacionEjecucionEmpresa.Estado,
                DT_DESDE = facturacionEjecucionEmpresa.FechaDesde,
                DT_HASTA = facturacionEjecucionEmpresa.FechaHasta,
                DT_UPDROW = facturacionEjecucionEmpresa.FechaModificacion
            };
        }

        public virtual T_FACTURACION_CUENTA_CONTABLE MapToEntity(CuentaContable cuentaContable)
        {
            return new T_FACTURACION_CUENTA_CONTABLE
            {
                NU_CUENTA_CONTABLE = cuentaContable.Id,
                DS_CUENTA_CONTABLE = cuentaContable.Descripcion
            };
        }

        public virtual T_FACTURACION_RESULTADO MapToEntity(FacturacionResultado facturacionResultado)
        {
            return new T_FACTURACION_RESULTADO
            {
                NU_EJECUCION = facturacionResultado.NumeroEjecucion,
                CD_EMPRESA = facturacionResultado.CodigoEmpresa,
                CD_UNIDADE_MEDIDA = NullIfEmpty(facturacionResultado.CodigoUnidadMedida),
                CD_FACTURACION = facturacionResultado.CodigoFacturacion,
                CD_FUNC_APROBACION_RECHAZO = facturacionResultado.CodigoFuncAprobacionRechazo,
                CD_SITUACAO = facturacionResultado.CodigoSituacion,
                QT_RESULTADO = facturacionResultado.CantidadResultado,
                CD_PROCESO = facturacionResultado.CodigoProceso,
                DS_ADICIONAL = facturacionResultado.DescripcionAdicional,
                DT_ADDROW = facturacionResultado.FechaIngresado,
                DT_UPDROW = facturacionResultado.FechaActualizacion,
                DT_APROBACION_RECHAZO = facturacionResultado.FechaAprobacionRechazo,
                NU_COMPONENTE = facturacionResultado.NumeroComponente,
                NU_CUENTA_CONTABLE = NullIfEmpty(facturacionResultado.NumeroCuentaContable),
                NU_FACTURA = facturacionResultado.NumeroFactura,
                NU_TICKET_INTERFAZ_FACTURACION = facturacionResultado.NumeroTicketInterfazFacturacion,
                VL_PRECIO_UNITARIO = facturacionResultado.PrecioUnitario,
                VL_PRECIO_MINIMO = facturacionResultado.PrecioMinimo,
                CD_MONEDA = facturacionResultado.Moneda,
                NU_TRANSACCION = facturacionResultado.NumeroTransaccion,
                NU_TRANSACCION_DELETE = facturacionResultado.NumeroTransaccionDelete,
            };
        }

        public virtual T_FACTURACION_PALLET_DET MapToEntity(FacturacionPalletDet facturacionPalletDet)
        {
            return new T_FACTURACION_PALLET_DET
            {
                NU_PALLET_DET = facturacionPalletDet.NumeroPalletDet,
                NU_COMPONENTE = facturacionPalletDet.NumeroComponente,
                NU_BULTOS_FACTURADOS = facturacionPalletDet.NumeroBultosFacturados,
                NU_PALLET = facturacionPalletDet.NumeroPallet,
                NU_EJECUCION_FACTURACION = facturacionPalletDet.NumeroEjecucionFacturacion,
                FL_APLICO_MINIMO = facturacionPalletDet.AplicoMinimo,
                QT_RESULTADO = facturacionPalletDet.CantidadResultado,
                CD_EMPRESA = facturacionPalletDet.CodigoEmpresa,
                CD_FACTURACION = facturacionPalletDet.CodigoFacturacion,
                ID_ESTADO = facturacionPalletDet.Estado,
                DT_DESDE = facturacionPalletDet.FechaDesde,
                DT_HASTA = facturacionPalletDet.FechaHasta
            };
        }

        public virtual T_FACTURACION_CODIGO MapToEntity(FacturacionCodigo facturacionCodigo)
        {
            return new T_FACTURACION_CODIGO
            {
                CD_FACTURACION = facturacionCodigo.CodigoFacturacion,
                DS_FACTURACION = facturacionCodigo.DescripcionFacturacion,
                TP_CALCULO = facturacionCodigo.TipoCalculo,
                DT_ADDROW = facturacionCodigo.FechaIngresado,
                DT_UPDROW = facturacionCodigo.FechaModificacion
            };
        }

        public virtual T_FACTURACION_EMPRESA_PROCESO MapToEntity(FacturacionEmpresaProceso facturacionCodigo)
        {
            return new T_FACTURACION_EMPRESA_PROCESO
            {
                CD_EMPRESA = facturacionCodigo.CodigoEmpresa,
                CD_PROCESO = facturacionCodigo.CodigoProceso,
                QT_RESULTADO = facturacionCodigo.Resultado,
                CD_SITUACAO_ERROR = facturacionCodigo.SituacionError,
                TP_ULTIMO_PROCESO = facturacionCodigo.TipoUltimoProceso,
                HR_ULTIMO_PROCESO = facturacionCodigo.UltimoProceso,
                DT_UPDROW = facturacionCodigo.FechaModificacion,
                NU_TRANSACCION = facturacionCodigo.NumeroTransaccion,
                NU_TRANSACCION_DELETE = facturacionCodigo.NumeroTransaccionDelete,
            };
        }

        public virtual T_FACTURACION_UNIDAD_MEDIDA MapToEntity(FacturacionUnidadMedida facturacionUnidadMedida)
        {
            return new T_FACTURACION_UNIDAD_MEDIDA
            {
                CD_UNIDADE_MEDIDA = facturacionUnidadMedida.UnidadMedida,
                NU_COMPONENTE = facturacionUnidadMedida.NumeroComponente,
                ID_USO = facturacionUnidadMedida.Uso,
                NU_TRANSACCION = facturacionUnidadMedida.NumeroTransaccion,
            };
        }

        public virtual T_FACTURACION_UND_MEDIDA_EMP MapToEntity(FacturacionUnidadMedidaEmpresa facturacionUnidadMedidaEmpresa)
        {
            return new T_FACTURACION_UND_MEDIDA_EMP
            {
                CD_UNIDADE_MEDIDA = facturacionUnidadMedidaEmpresa.UnidadMedida,
                CD_EMPRESA = facturacionUnidadMedidaEmpresa.Empresa,
                CD_FUNCIONARIO = facturacionUnidadMedidaEmpresa.Funcionario,
                DT_ADDROW = facturacionUnidadMedidaEmpresa.Fecha
            };
        }
        #endregion
    }
}
