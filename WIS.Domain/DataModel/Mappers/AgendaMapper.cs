using System.Collections.Generic;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.Recepcion;
using WIS.Domain.Recepcion.Enums;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class AgendaMapper : Mapper
    {
        protected RecepcionTipoMapper _recepcionTipoMapper;

        public AgendaMapper()
        {
        }

        #region Agenda

        public virtual Agenda MapAgendaToObject(T_AGENDA agenda, T_RECEPCION_TIPO tpRecepcion)
        {
            if (agenda == null) return null;

            var model = new Agenda
            {
                Id = agenda.NU_AGENDA,
                CodigoInternoCliente = agenda.CD_CLIENTE,
                IdEmpresa = agenda.CD_EMPRESA ?? -1,
                FechaInsercion = agenda.DT_ADDROW,
                FechaModificacion = agenda.DT_UPDROW,
                NumeroDocumento = agenda.NU_DOCUMENTO,
                Predio = agenda.NU_PREDIO,
                FuncionarioResponsable = agenda.CD_FUN_RESP,
                FechaFuncionarioResponsable = agenda.DT_FUN_RESP,
                Estado = this.MapEstado((short)agenda.CD_SITUACAO),
                TipoDocumento = agenda.CD_TIPO_DOCUMENTO,
                CodigoOperacion = agenda.CD_OPERACAO,
                CodigoPuerta = agenda.CD_PORTA,
                FechaInicio = agenda.DT_INICIO,
                FechaFin = agenda.DT_FIN,
                PlacaVehiculo = agenda.DS_PLACA,
                DUA = agenda.NU_DUA,
                Anexo1 = agenda.DS_ANEXO1,
                Anexo2 = agenda.DS_ANEXO2,
                Anexo3 = agenda.DS_ANEXO3,
                Anexo4 = agenda.DS_ANEXO4,
                EnviaDocumentacion = MapStringToBoolean(agenda.ID_ENVIO_DOCUMENTACION),
                IdUsuarioEnvioDocumentacion = agenda.CD_FUNC_ENVIO_DOCU,
                Averiado = MapStringToBoolean(agenda.ID_AVERIA),
                FechaCierre = agenda.DT_CIERRE,
                FechaEntrega = agenda.DT_ENTREGA,
                TipoRecepcionInterno = agenda.TP_RECEPCION,
                IdFuncionarioAsignado = agenda.CD_FUNCIONARIO_ASIGNADO,
                NumeroInterfazEjecucion = agenda.NU_INTERFAZ_EJECUCION,
                CargaDetalleAutomatica = MapStringToBoolean(agenda.FL_CARGA_AUTO_DETALLE),
                NroOrdenTarea = agenda.NU_ORT_ORDEN,
                SincronizacionRealizada = MapStringToBoolean(agenda.FL_SYNC_REALIZADA),
                NumeroTransaccion = agenda.NU_TRANSACCION,
                EsFacturaValidada = MapStringToBoolean(agenda.FL_FACTURA_VALIDA)
            };

            foreach (var detalle in agenda.T_DET_AGENDA)
            {
                model.Detalles.Add(this.MapAgendaDetalleToObject(detalle));
            }

            if (tpRecepcion != null)
            {
                if (_recepcionTipoMapper == null)
                    _recepcionTipoMapper = new RecepcionTipoMapper();

                model.TipoRecepcion = _recepcionTipoMapper.MapToObject(tpRecepcion);
            }

            return model;
        }

        public virtual T_AGENDA MapAgendaToEntity(Agenda agenda)
        {
            var entity = this.MapAgendaSinDependenciaToEntity(agenda);

            var detalles = new List<T_DET_AGENDA>();

            foreach (var linea in agenda.Detalles)
            {
                detalles.Add(this.MapAgendaDetalleToEntity(linea));
            }

            entity.T_DET_AGENDA = detalles;

            return entity;

        }

        public virtual T_AGENDA MapAgendaSinDependenciaToEntity(Agenda agenda)
        {
            return new T_AGENDA
            {
                NU_AGENDA = agenda.Id,
                CD_CLIENTE = agenda.CodigoInternoCliente,
                CD_EMPRESA = agenda.IdEmpresa,
                DT_ADDROW = agenda.FechaInsercion,
                DT_UPDROW = agenda.FechaModificacion,
                NU_PREDIO = agenda.Predio,
                CD_FUN_RESP = agenda.FuncionarioResponsable,
                DT_FUN_RESP = agenda.FechaFuncionarioResponsable,
                CD_SITUACAO = MapEstado(agenda.Estado),
                CD_TIPO_DOCUMENTO = NullIfEmpty(agenda.TipoDocumento),
                CD_OPERACAO = agenda.CodigoOperacion,
                CD_PORTA = agenda.CodigoPuerta,
                DT_INICIO = agenda.FechaInicio,
                DT_FIN = agenda.FechaFin,
                DS_PLACA = agenda.PlacaVehiculo,
                NU_DUA = agenda.DUA,
                DS_ANEXO1 = agenda.Anexo1,
                DS_ANEXO2 = agenda.Anexo2,
                DS_ANEXO3 = agenda.Anexo3,
                DS_ANEXO4 = agenda.Anexo4,
                ID_ENVIO_DOCUMENTACION = MapBooleanToString(agenda.EnviaDocumentacion),
                CD_FUNC_ENVIO_DOCU = agenda.IdUsuarioEnvioDocumentacion,
                ID_AVERIA = MapBooleanToString(agenda.Averiado),
                DT_CIERRE = agenda.FechaCierre,
                DT_ENTREGA = agenda.FechaEntrega,
                TP_RECEPCION = agenda.TipoRecepcionInterno,
                CD_FUNCIONARIO_ASIGNADO = agenda.IdFuncionarioAsignado,
                NU_INTERFAZ_EJECUCION = agenda.NumeroInterfazEjecucion,
                FL_CARGA_AUTO_DETALLE = MapBooleanToString(agenda.CargaDetalleAutomatica),
                NU_DOCUMENTO = NullIfEmpty(agenda.NumeroDocumento),
                NU_ORT_ORDEN = agenda.NroOrdenTarea,
                FL_SYNC_REALIZADA = MapBooleanToString(agenda.SincronizacionRealizada),
                NU_TRANSACCION = agenda.NumeroTransaccion,
                FL_FACTURA_VALIDA = MapBooleanToString(agenda.EsFacturaValidada)
            };
        }

        public virtual EstadoAgenda MapEstado(short estado)
        {
            switch (estado)
            {
                case EstadoAgendaDb.Abierta: return EstadoAgenda.Abierta;
                case EstadoAgendaDb.Cerrada: return EstadoAgenda.Cerrada;
                case EstadoAgendaDb.Cancelada: return EstadoAgenda.Cancelada;
                case EstadoAgendaDb.IngresandoFactura: return EstadoAgenda.IngresandoFactura;
                case EstadoAgendaDb.ConferidaConDiferencias: return EstadoAgenda.ConferidaConDiferencias;
                case EstadoAgendaDb.ConferidaSinDiferencias: return EstadoAgenda.ConferidaSinDiferencias;
                case EstadoAgendaDb.AguardandoDesembarque: return EstadoAgenda.AguardandoDesembarque;
                case EstadoAgendaDb.DocumentoAsociado: return EstadoAgenda.DocumentoAsociado;
            }

            return EstadoAgenda.Unknown;
        }

        public virtual short MapEstado(EstadoAgenda estado)
        {
            switch (estado)
            {
                case EstadoAgenda.Abierta: return EstadoAgendaDb.Abierta;
                case EstadoAgenda.Cerrada: return EstadoAgendaDb.Cerrada;
                case EstadoAgenda.Cancelada: return EstadoAgendaDb.Cancelada;
                case EstadoAgenda.IngresandoFactura: return EstadoAgendaDb.IngresandoFactura;
                case EstadoAgenda.ConferidaConDiferencias: return EstadoAgendaDb.ConferidaConDiferencias;
                case EstadoAgenda.ConferidaSinDiferencias: return EstadoAgendaDb.ConferidaSinDiferencias;
                case EstadoAgenda.AguardandoDesembarque: return EstadoAgendaDb.AguardandoDesembarque;
                case EstadoAgenda.DocumentoAsociado: return EstadoAgendaDb.DocumentoAsociado;
            }

            return -1;
        }

        #endregion

        #region Detalle agenda

        public virtual AgendaDetalle MapAgendaDetalleToObject(T_DET_AGENDA detalle)
        {
            return new AgendaDetalle
            {
                IdAgenda = detalle.NU_AGENDA,
                IdEmpresa = detalle.CD_EMPRESA,
                CodigoProducto = detalle.CD_PRODUTO,
                Faixa = detalle.CD_FAIXA,
                Identificador = detalle.NU_IDENTIFICADOR?.Trim()?.ToUpper(),
                Estado = MapEstadoDetalle((short)detalle.CD_SITUACAO),
                CantidadAgendada = detalle.QT_AGENDADO ?? 0,
                CantidadRecibida = detalle.QT_RECIBIDA ?? 0,
                CantidadAceptada = detalle.QT_ACEPTADA ?? 0,
                CantidadAgendadaOriginal = detalle.QT_AGENDADO_ORIGINAL ?? 0,
                CantidadRecibidaFicticia = detalle.QT_RECIBIDA_FICTICIA ?? 0,
                CantidadCrossDocking = detalle.QT_CROSS_DOCKING ?? 0,
                Vencimiento = detalle.DT_FABRICACAO,
                FechaAlta = detalle.DT_ADDROW,
                FechaModificacion = detalle.DT_UPDROW,
                FechaAceptacionRecepcion = detalle.DT_ACEPTADA_RECEPCION,
                IdUsuarioAceptacionRecepcion = detalle.CD_FUNC_ACEPTO_RECEPCION,
                CIF = detalle.VL_CIF,
                Precio = detalle.VL_PRECIO,
                NumeroTransaccion = detalle.NU_TRANSACCION,
                EstadoId = (short)detalle.CD_SITUACAO
            };
        }

        public virtual T_DET_AGENDA MapAgendaDetalleToEntity(AgendaDetalle linea)
        {
            return new T_DET_AGENDA
            {
                NU_AGENDA = linea.IdAgenda,
                CD_EMPRESA = linea.IdEmpresa,
                CD_PRODUTO = linea.CodigoProducto,
                CD_FAIXA = linea.Faixa,
                NU_IDENTIFICADOR = linea.Identificador?.Trim()?.ToUpper(),
                CD_SITUACAO = MapEstadoDetalle(linea.Estado),
                QT_AGENDADO = linea.CantidadAgendada,
                QT_RECIBIDA = linea.CantidadRecibida,
                QT_ACEPTADA = linea.CantidadAceptada,
                QT_AGENDADO_ORIGINAL = linea.CantidadAgendadaOriginal,
                QT_RECIBIDA_FICTICIA = linea.CantidadRecibidaFicticia,
                QT_CROSS_DOCKING = linea.CantidadCrossDocking,
                DT_FABRICACAO = linea.Vencimiento,
                DT_ADDROW = linea.FechaAlta,
                DT_UPDROW = linea.FechaModificacion,
                DT_ACEPTADA_RECEPCION = linea.FechaAceptacionRecepcion,
                CD_FUNC_ACEPTO_RECEPCION = linea.IdUsuarioAceptacionRecepcion,
                VL_CIF = linea.CIF,
                VL_PRECIO = linea.Precio,
                NU_TRANSACCION = linea.NumeroTransaccion,
            };
        }

        public virtual EstadoAgendaDetalle MapEstadoDetalle(short estado)
        {
            switch (estado)
            {
                case EstadoAgendaDetalleDb.Abierta: return EstadoAgendaDetalle.Abierta;
                case EstadoAgendaDetalleDb.ConferidaConDiferencias: return EstadoAgendaDetalle.ConferidaConDiferencias;
                case EstadoAgendaDetalleDb.ConferidaSinDiferencias: return EstadoAgendaDetalle.ConferidaSinDiferencias;
                case EstadoAgendaDetalleDb.EnProgreso: return EstadoAgendaDetalle.EnProgreso;
            }

            return EstadoAgendaDetalle.Unknown;
        }

        public virtual short MapEstadoDetalle(EstadoAgendaDetalle estado)
        {
            switch (estado)
            {
                case EstadoAgendaDetalle.Abierta: return EstadoAgendaDetalleDb.Abierta;
                case EstadoAgendaDetalle.ConferidaConDiferencias: return EstadoAgendaDetalleDb.ConferidaConDiferencias;
                case EstadoAgendaDetalle.ConferidaSinDiferencias: return EstadoAgendaDetalleDb.ConferidaSinDiferencias;
                case EstadoAgendaDetalle.EnProgreso: return EstadoAgendaDetalleDb.EnProgreso;
            }

            return -1;
        }

        #endregion 

        #region Problema detalle agenda

        public virtual AgendaDetalleProblema MapAgendaDetalleProblemaEntityToObject(T_RECEPCION_AGENDA_PROBLEMA problema)
        {
            if (problema == null)
                return null;

            return new AgendaDetalleProblema
            {
                Id = problema.NU_RECEPCION_AGENDA_PROBLEMA,
                NumeroAgenda = problema.NU_AGENDA,
                CodigoProducto = problema.CD_PRODUTO,
                Identificador = problema.NU_IDENTIFICADOR?.Trim()?.ToUpper(),
                TipoProblema = this.MapTipoProblemaDetalle(problema.ND_TIPO),
                Problema = this.MapProblemaDetalle(problema.ND_PROBLEMA),
                Diferencia = problema.VL_DIFERENCIA,
                Aceptado = this.MapStringToBoolean(problema.FL_ACEPTADO),
                Funcionario = problema.CD_FUNCIONARIO,
                FechaAlta = problema.DT_ADDROR,
                FechaModificacion = problema.DT_UPDROW,
                Embalaje = problema.CD_FAIXA,
                FuncionarioAceptaProblema = problema.CD_FUNCIONARIO_ACEPTA_PROBLEMA,
                FechaAceptacionProblema = problema.DT_ACEPTADO,
                Lpn = problema.NU_LPN

            };
        }

        public virtual T_RECEPCION_AGENDA_PROBLEMA MapAgendaDetalleProblemaToEntity(AgendaDetalleProblema agendaProb)
        {
            return new T_RECEPCION_AGENDA_PROBLEMA
            {
                NU_RECEPCION_AGENDA_PROBLEMA = agendaProb.Id,
                NU_AGENDA = agendaProb.NumeroAgenda,
                CD_PRODUTO = agendaProb.CodigoProducto,
                NU_IDENTIFICADOR = agendaProb.Identificador?.Trim()?.ToUpper(),
                ND_TIPO = this.MapTipoProblemaDetalle(agendaProb.TipoProblema),
                ND_PROBLEMA = this.MapProblemaDetalle(agendaProb.Problema),
                VL_DIFERENCIA = agendaProb.Diferencia,
                FL_ACEPTADO = this.MapBooleanToString(agendaProb.Aceptado),
                CD_FUNCIONARIO = agendaProb.Funcionario,
                DT_ADDROR = agendaProb.FechaAlta,
                DT_UPDROW = agendaProb.FechaModificacion,
                CD_FAIXA = agendaProb.Embalaje,
                CD_FUNCIONARIO_ACEPTA_PROBLEMA = agendaProb.FuncionarioAceptaProblema,
                DT_ACEPTADO = agendaProb.FechaAceptacionProblema,
                NU_LPN = agendaProb.Lpn

            };
        }

        public virtual ProblemaAgendaDetalle MapProblemaDetalle(string problema)
        {
            switch (problema)
            {
                case ProblemaAgendaDb.RecibidoExcedeAgendado: return ProblemaAgendaDetalle.RecibidoExcedeAgendado;
                case ProblemaAgendaDb.RecibidoExcedeSaldoReferenciaRecepcion: return ProblemaAgendaDetalle.RecibidoExcedeSaldoReferenciaRecepcion;
                case ProblemaAgendaDb.RecibidoProductoNoEsperado: return ProblemaAgendaDetalle.RecibidoProductoNoEsperado;
                case ProblemaAgendaDb.RecibidoLoteNoEsperado: return ProblemaAgendaDetalle.RecibidoLoteNoEsperado;
                case ProblemaAgendaDb.RecibidoMenorAgendado: return ProblemaAgendaDetalle.RecibidoMenorAgendado;
                case ProblemaAgendaDb.RecibidoMenorFacturado: return ProblemaAgendaDetalle.RecibidoMenorFacturado;
                case ProblemaAgendaDb.RecibidoMenorSaldoReferencias: return ProblemaAgendaDetalle.RecibidoMenorSaldoReferencias;
                case ProblemaAgendaDb.FacturadoExcedeAgendado: return ProblemaAgendaDetalle.FacturadoExcedeAgendado;
                case ProblemaAgendaDb.FacturadoExcedeSaldoReferenciaRecepcion: return ProblemaAgendaDetalle.FacturadoExcedeSaldoReferenciaRecepcion;

                case ProblemaAgendaDb.RecibidoExcedeLpnDeclarado: return ProblemaAgendaDetalle.RecibidoExcedeLpnDeclarado;
                case ProblemaAgendaDb.RecibidoProductoNoEsperadoLpn: return ProblemaAgendaDetalle.RecibidoProductoNoEsperadoLpn;
                case ProblemaAgendaDb.RecibidoMenorLpnDeclarado: return ProblemaAgendaDetalle.RecibidoMenorLpnDeclarado;

            }

            return ProblemaAgendaDetalle.Unknown;
        }

        public virtual string MapProblemaDetalle(ProblemaAgendaDetalle problema)
        {
            switch (problema)
            {

                case ProblemaAgendaDetalle.RecibidoExcedeAgendado: return ProblemaAgendaDb.RecibidoExcedeAgendado;
                case ProblemaAgendaDetalle.RecibidoExcedeSaldoReferenciaRecepcion: return ProblemaAgendaDb.RecibidoExcedeSaldoReferenciaRecepcion;
                case ProblemaAgendaDetalle.RecibidoProductoNoEsperado: return ProblemaAgendaDb.RecibidoProductoNoEsperado;
                case ProblemaAgendaDetalle.RecibidoLoteNoEsperado: return ProblemaAgendaDb.RecibidoLoteNoEsperado;
                case ProblemaAgendaDetalle.RecibidoMenorAgendado: return ProblemaAgendaDb.RecibidoMenorAgendado;
                case ProblemaAgendaDetalle.RecibidoMenorFacturado: return ProblemaAgendaDb.RecibidoMenorFacturado;
                case ProblemaAgendaDetalle.RecibidoMenorSaldoReferencias: return ProblemaAgendaDb.RecibidoMenorSaldoReferencias;
                case ProblemaAgendaDetalle.FacturadoExcedeAgendado: return ProblemaAgendaDb.FacturadoExcedeAgendado;
                case ProblemaAgendaDetalle.FacturadoExcedeSaldoReferenciaRecepcion: return ProblemaAgendaDb.FacturadoExcedeSaldoReferenciaRecepcion;

                case ProblemaAgendaDetalle.RecibidoExcedeLpnDeclarado: return ProblemaAgendaDb.RecibidoExcedeLpnDeclarado;
                case ProblemaAgendaDetalle.RecibidoProductoNoEsperadoLpn: return ProblemaAgendaDb.RecibidoProductoNoEsperadoLpn;
                case ProblemaAgendaDetalle.RecibidoMenorLpnDeclarado: return ProblemaAgendaDb.RecibidoMenorLpnDeclarado;

            }

            return null;
        }

        public virtual TipoProblemaAgendaDetalle MapTipoProblemaDetalle(string problema)
        {
            switch (problema)
            {
                case ProblemaAgendaDb.TipoProblema: return TipoProblemaAgendaDetalle.Problema;
                case ProblemaAgendaDb.TipoNotificacion: return TipoProblemaAgendaDetalle.Notificacion;
            }

            return TipoProblemaAgendaDetalle.Unknown;
        }

        public virtual string MapTipoProblemaDetalle(TipoProblemaAgendaDetalle problema)
        {
            switch (problema)
            {
                case TipoProblemaAgendaDetalle.Problema: return ProblemaAgendaDb.TipoProblema;
                case TipoProblemaAgendaDetalle.Notificacion: return ProblemaAgendaDb.TipoNotificacion;
            }

            return null;
        }

        #endregion 

        public virtual AgendaLpnPlanificacion MapToObject(T_AGENDA_LPN_PLANIFICACION entity)
        {
            if (entity == null) return null;

            return new AgendaLpnPlanificacion()
            {
                NroAgenda = entity.NU_AGENDA,
                NroLPN = entity.NU_LPN,
                Planificado = entity.FL_PLANIFICADO,
                Recibido = entity.FL_RECIBIDO,
                Funcionario = entity.CD_FUNCIONARIO,
                FuncionarioRecepcion = entity.CD_FUNCIONARIO_RECEPCION,
                FechaRecepcion = entity.DT_RECEPCION,
                FechaInsercion = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UPDROW
            };
        }

        public virtual T_AGENDA_LPN_PLANIFICACION MapToEntity(AgendaLpnPlanificacion obj)
        {
            if (obj == null) return null;

            return new T_AGENDA_LPN_PLANIFICACION()
            {
                NU_AGENDA = obj.NroAgenda,
                NU_LPN = obj.NroLPN,
                FL_PLANIFICADO = obj.Planificado,
                FL_RECIBIDO = obj.Recibido,
                CD_FUNCIONARIO = obj.Funcionario,
                CD_FUNCIONARIO_RECEPCION = obj.FuncionarioRecepcion,
                DT_RECEPCION = obj.FechaRecepcion,
                DT_ADDROW = obj.FechaInsercion,
                DT_UPDROW = obj.FechaModificacion,
            };
        }

        public virtual List<SaldoAgendaExportExcel> MapAgendaFacEntityToObject(List<V_EVENTO_SALDO_FAC> agendaRef)
        {
            List<SaldoAgendaExportExcel> lista = new List<SaldoAgendaExportExcel>();
            foreach (var det in agendaRef)
            {
                SaldoAgendaExportExcel newDet = new SaldoAgendaExportExcel();
                newDet.Agenda = det.NU_AGENDA;
                newDet.Producto = det.CD_PRODUTO;
                newDet.ProductoExterno = det.CD_EXTERNO;
                newDet.Proveedor = det.CD_PROVEEDOR;
                newDet.TipoFactura = det.TP_FACTURA;
                newDet.Serie = det.NU_SERIE;
                newDet.Factura = det.NU_FACTURA;
                newDet.Empresa = det.CD_EMPRESA ?? 0;
                newDet.TipoProveeor = det.TP_PROVEEDOR;
                newDet.CantRecibida = det.QT_RECIBIDA ?? 0;
                newDet.CantAgendada = det.QT_AGENDADA ?? 0;
                newDet.CantRestante = det.QT_RESTANTE ?? 0;

                lista.Add(newDet);
            }


            return lista;
        }
        public virtual List<SaldoReferenciaExpedidosExcel> MapAgendaRefEntityToObject(List<V_EVENTO_SALDO_REF> agendaref)
        {
            List<SaldoReferenciaExpedidosExcel> list = new List<SaldoReferenciaExpedidosExcel>();
            foreach (var det in agendaref)
            {
                SaldoReferenciaExpedidosExcel newDet = new SaldoReferenciaExpedidosExcel();
                newDet.Agenda = det.NU_AGENDA;
                newDet.Empresa = det.CD_EMPRESA ?? 0;
                newDet.Producto = det.CD_PRODUTO;
                newDet.ProductoExterno = det.CD_EXTERNO;
                newDet.Proveedor = det.CD_PROVEEDOR;
                newDet.Referencia = det.NU_REFERENCIA;
                newDet.TipoProveeor = det.TP_PROVEEDOR;
                newDet.CantRecibida = det.QT_RECIBIDA ?? 0;
                newDet.CantReferencia = det.QT_REFERENCIA ?? 0;
                newDet.CantRestante = det.QT_RESTANTE ?? 0;
                list.Add(newDet);
            }

            return list;
        }
    }
}
