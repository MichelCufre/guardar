using WIS.Domain.Recepcion;
using WIS.Domain.Reportes;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class RecepcionTipoMapper : Mapper
    {
        public RecepcionTipoMapper()
        {
        }

        public virtual RecepcionTipo MapToObject(T_RECEPCION_TIPO entity)
        {
            if (entity == null)
                return null;

            var tipo = new RecepcionTipo
            {
                Tipo = entity.TP_RECEPCION,
                Descripcion = entity.DS_TIPO_RECEPCION,
                TipoAgente = entity.TP_AGENTE,
                TipoReferencia = entity.TP_REFERENCIA,
                TipoSeleccionReferencia = entity.TP_SELECCION_REFERENCIA,
                TipoManejoNumeroDocumento = entity.TP_MANEJO_NU_DOCUMENTO,
                OrdenCompraSinSaldoAgenda = entity.FL_OC_SIN_SALDO_AGENDA,
                EspecificarPredio = entity.FL_ESPECIFICAR_PREDIO,
                CierreAgenda = entity.VL_CIERRE_AGENDA,
                VlCrossDocking = entity.VL_CROSS_DOCKING,
                AdmiteProductosActivos = entity.FL_PRODUCTOS_ACTIVOS,
                PermiteRecibirLotesNoEsperados = entity.FL_RECIBIR_LOTES_NO_ESPERADOS,
                ControlarVencimiento = entity.FL_CONTROLAR_VENCIMIENTO,
                VlEtiquetaRecepcion = entity.VL_ETIQUETAS_RECEPCION,
                VlEspecificaLotes = entity.VL_ESPECIFICAR_LOTES,
                RequiereAgendarHorarioPuerta = entity.FL_AGENDAR_HORARIO_PUERTA,
                VlEstadoEtiqueta = entity.VL_ESTADO_ETIQUETA,
                PermiteAlmacenarEnAveria = entity.FL_PERMITIR_ALMACENAR_AVERIA,
                VlInterfazEnCierreAgenda = entity.VL_INTERFAZ_EN_CIERRE_AGENDA,
                OrdenCompraSinSaldoRecepcion = entity.FL_OC_SIN_SALDO_RECEPCION,
                AceptaCantidadMayorAgendado = entity.FL_ACEPTA_QT_MAYOR_A_AGENDADO,
                ManipuleoTarea = entity.FL_MANIPULEO_TAREA,
                ControlPeso = entity.FL_CTRL_PESO,
                ControlVencimiento = entity.FL_CTRL_VENCIMIENTO,
                ControlVolumen = entity.FL_CTRL_VOLUMEN,
                AdmiteMonoReferencia = entity.FL_MONO_REFERENCIA,
                MotivoRequerido = entity.FL_MOTIVO_REQUERIDO,
                PermitePlanificarLpn = entity.FL_PERMITE_PLANIFICAR_LPN,
                PermiteRecibirLpn = entity.FL_PERMITE_RECIBIR_LPN,
                PermiteLpnNoEsperado = entity.FL_PERMITE_LPN_NO_ESPERADO,

                AceptaProductosNoEsperados = this.MapStringToBoolean(entity.FL_PRODUCTOS_NO_ESPERADOS),
                IngresaFactura = this.MapStringToBoolean(entity.FL_INGRESO_FACTURA),
                PermiteDigitacion = this.MapStringToBoolean(entity.FL_DIGITACION_HABILITADA),
                PermiteRecepcionAutomatica = this.MapStringToBoolean(entity.FL_PERMITE_AUTO_RECEPCION),
                HabilitaAsociacionAlCrearEmpresa = this.MapStringToBoolean(entity.FL_HABILITAR_EMPRESA_DEFAULT),
                CancelarSaldosReferenciasAlCierreDeAgenda = this.MapStringToBoolean(entity.FL_CANCELAR_SALDO_AL_CIERRE),
                CargaDetalleAutomaticamente = this.MapStringToBoolean(entity.FL_CARGA_AUTO_DETALLE),
            };

            foreach (var reporte in entity.T_RECEPCION_TIPO_REPORTE_DEF)
            {
                tipo.Reportes.Add(new ReporteDefinicion()
                {
                    Id = reporte.CD_REPORTE,
                    Descripcion = reporte.T_REPORTE_DEFINICION.DS_REPORTE,
                    Tipo = reporte.T_REPORTE_DEFINICION.TP_REPORTE,
                    DescRecursoTexto = reporte.T_REPORTE_DEFINICION.DS_RECURSO_TEXTO,
                });
            }

            return tipo;
        }

        public virtual EmpresaRecepcionTipo MapToObject(T_RECEPCION_REL_EMPRESA_TIPO entity)
        {
            if (entity == null) return null;

            var tipo = new EmpresaRecepcionTipo
            {
                Id = entity.NU_RECEPCION_REL_EMPRESA_TIPO,
                IdEmpresa = entity.CD_EMPRESA,
                TipoExterno = entity.TP_RECEPCION_EXTERNO,
                ManejoDeInterfaz = entity.FL_MANEJO_INTERFAZ,
                DescripcionExterna = entity.DS_RECEPCION_EXTERNO,
                InterfazExterna = entity.CD_INTERFAZ_EXTERNA,
                Habilitado = this.MapStringToBoolean(entity.FL_HABILITADO)
            };

            tipo.RecepcionTipoInterno = this.MapToObject(entity.T_RECEPCION_TIPO);

            return tipo;
        }

        public virtual T_RECEPCION_REL_EMPRESA_TIPO MapToEntity(EmpresaRecepcionTipo empresaTipo)
        {
            if (empresaTipo == null) return null;

            return new T_RECEPCION_REL_EMPRESA_TIPO
            {
                NU_RECEPCION_REL_EMPRESA_TIPO = empresaTipo.Id,
                CD_EMPRESA = empresaTipo.IdEmpresa,
                TP_RECEPCION = NullIfEmpty(empresaTipo.RecepcionTipoInterno?.Tipo),
                TP_RECEPCION_EXTERNO = empresaTipo.TipoExterno,
                CD_INTERFAZ_EXTERNA = empresaTipo.InterfazExterna,
                FL_MANEJO_INTERFAZ = empresaTipo.ManejoDeInterfaz,
                DS_RECEPCION_EXTERNO = empresaTipo.DescripcionExterna,
                FL_HABILITADO = this.MapBooleanToString(empresaTipo.Habilitado)
            };
        }

        public virtual T_RECEPCION_EMP_TIPO_REPORTE MapToEntity(EmpresaRecepcionTipoReporte obj)
        {
            if (obj == null) return null;

            return new T_RECEPCION_EMP_TIPO_REPORTE
            {
                NU_REC_EMP_TIPO_REP = obj.Id,
                CD_EMPRESA = obj.IdEmpresa,
                CD_REPORTE = obj.CodigoReporte,
                TP_RECEPCION_EXTERNO = obj.TipoRecepcion,
            };
        }
    }
}
