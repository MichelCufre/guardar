using System.Collections.Generic;
using WIS.Domain.Recepcion;
using WIS.Domain.Recepcion.RecepcionAgendamiento;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class ReferenciaRecepcionMapper : Mapper
    {
        public virtual ReferenciaRecepcion MapToObject(T_RECEPCION_REFERENCIA entity)
        {
            if (entity == null)
                return null;

            var referencia = new ReferenciaRecepcion
            {
                Id = entity.NU_RECEPCION_REFERENCIA,
                Numero = entity.NU_REFERENCIA,
                TipoReferencia = entity.TP_REFERENCIA,
                IdEmpresa = entity.CD_EMPRESA,
                FechaEntrega = entity.DT_ENTREGA,
                FechaVencimientoOrden = entity.DT_VENCIMIENTO_ORDEN,
                FechaEmitida = entity.DT_EMITIDA,
                Memo = entity.DS_MEMO,
                Serializado = entity.VL_SERIALIZADO,
                Situacion = entity.CD_SITUACAO,
                FechaInsercion = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UPDROW,
                NumeroInterfazEjecucion = entity.NU_INTERFAZ_EJECUCION,
                Anexo1 = entity.DS_ANEXO1,
                Anexo2 = entity.DS_ANEXO2,
                Anexo3 = entity.DS_ANEXO3,
                IdPredio = entity.NU_PREDIO,
                CodigoCliente = entity.CD_CLIENTE,
                Estado = entity.ND_ESTADO_REFERENCIA,
                Moneda = entity.CD_MONEDA,
                NumeroTransaccion = entity.NU_TRANSACCION,
                NumeroTransaccionDelete = entity.NU_TRANSACCION_DELETE,
            };

            return referencia;
        }
        public virtual ReferenciaRecepcion MapToObjectWithDetail(T_RECEPCION_REFERENCIA entity)
        {
            if (entity == null)
                return null;

            var referencia = this.MapToObject(entity);

            foreach (var detalle in entity.T_RECEPCION_REFERENCIA_DET)
            {
                var detalleReferencia = this.MapDetalleToObject(detalle);
                detalleReferencia.ReferenciaRecepcion = referencia;

                referencia.Detalles.Add(detalleReferencia);
            }

            return referencia;
        }

        public virtual T_RECEPCION_REFERENCIA MapToEntity(ReferenciaRecepcion referencia)
        {
            return new T_RECEPCION_REFERENCIA
            {
                NU_RECEPCION_REFERENCIA = referencia.Id,
                NU_REFERENCIA = referencia.Numero,
                TP_REFERENCIA = referencia.TipoReferencia,
                CD_EMPRESA = referencia.IdEmpresa,
                DT_ENTREGA = referencia.FechaEntrega,
                DT_VENCIMIENTO_ORDEN = referencia.FechaVencimientoOrden,
                DT_EMITIDA = referencia.FechaEmitida,
                DS_MEMO = referencia.Memo,
                VL_SERIALIZADO = referencia.Serializado,
                CD_SITUACAO = referencia.Situacion,
                DT_ADDROW = referencia.FechaInsercion,
                DT_UPDROW = referencia.FechaModificacion,
                NU_INTERFAZ_EJECUCION = referencia.NumeroInterfazEjecucion,
                DS_ANEXO1 = referencia.Anexo1,
                DS_ANEXO2 = referencia.Anexo2,
                DS_ANEXO3 = referencia.Anexo3,
                NU_PREDIO = referencia.IdPredio,
                CD_CLIENTE = referencia.CodigoCliente,
                ND_ESTADO_REFERENCIA = referencia.Estado,
                CD_MONEDA = referencia.Moneda,
                NU_TRANSACCION = referencia.NumeroTransaccion,
                NU_TRANSACCION_DELETE = referencia.NumeroTransaccionDelete,
            };
        }

        public virtual T_RECEPCION_AGENDA_REFERENCIA MapToEntity(DetalleAgendaReferenciaAsociada detalle)
        {
            return new T_RECEPCION_AGENDA_REFERENCIA
            {
                CD_EMPRESA = detalle.DetalleAgenda.IdEmpresa,
                CD_FAIXA = detalle.DetalleAgenda.Faixa,
                CD_PRODUTO = detalle.DetalleAgenda.CodigoProducto,
                DT_ADDROW = detalle.FechaInsercion,
                NU_AGENDA = detalle.DetalleAgenda.IdAgenda,
                NU_IDENTIFICADOR = detalle.DetalleAgenda.Identificador?.Trim()?.ToUpper(),
                NU_INTERFAZ_EJECUCION = detalle.NumeroInterfazEjecucion,
                NU_RECEPCION_REFERENCIA_DET = detalle.DetalleReferencia.Id,
                NU_TRANSACCION = detalle.NumeroTransaccion,
                NU_TRANSACCION_DELETE = detalle.NumeroTransaccionDelete,
                QT_AGENDADA = detalle.CantidadAgendada,
                QT_RECIBIDA = detalle.CantidadRecibida,
            };
        }

        public virtual List<ReferenciaRecepcionDetalle> MapDetalleToObject(List<T_RECEPCION_REFERENCIA_DET> entity)
        {
            List<ReferenciaRecepcionDetalle> list = new List<ReferenciaRecepcionDetalle>();
            if (entity == null)
                return null;
            foreach (var det in entity)
            {
                list.Add(MapDetalleToObject(det));
            }
            return list;
        }
        public virtual ReferenciaRecepcionDetalle MapDetalleToObject(T_RECEPCION_REFERENCIA_DET entity)
        {
            if (entity == null)
                return null;

            var detalle = new ReferenciaRecepcionDetalle
            {
                Id = entity.NU_RECEPCION_REFERENCIA_DET,
                IdReferencia = entity.NU_RECEPCION_REFERENCIA,
                IdLineaSistemaExterno = entity.ID_LINEA_SISTEMA_EXTERNO,
                IdEmpresa = entity.CD_EMPRESA,
                CodigoProducto = entity.CD_PRODUTO,
                Faixa = entity.CD_FAIXA,
                Identificador = entity.NU_IDENTIFICADOR?.Trim()?.ToUpper(),
                CantidadReferencia = entity.QT_REFERENCIA ?? 0,
                CantidadAnulada = entity.QT_ANULADA ?? 0,
                CantidadAgendada = entity.QT_AGENDADA ?? 0,
                CantidadRecibida = entity.QT_RECIBIDA ?? 0,
                CantidadConfirmadaInterfaz = entity.QT_CONFIRMADA_INTERFAZ ?? 0,
                ImporteUnitario = entity.IM_UNITARIO,
                FechaVencimiento = entity.DT_VENCIMIENTO,
                Anexo1 = entity.DS_ANEXO1,
                FechaInsercion = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UPDROW,
                NumeroTransaccion = entity.NU_TRANSACCION,
                NumeroTransaccionDelete = entity.NU_TRANSACCION_DELETE,
            };

            return detalle;
        }

        public virtual T_RECEPCION_REFERENCIA_DET MapDetalleToEntity(ReferenciaRecepcionDetalle detalle)
        {
            return new T_RECEPCION_REFERENCIA_DET
            {
                NU_RECEPCION_REFERENCIA_DET = detalle.Id,
                NU_RECEPCION_REFERENCIA = detalle.IdReferencia,
                ID_LINEA_SISTEMA_EXTERNO = detalle.IdLineaSistemaExterno,
                CD_EMPRESA = detalle.IdEmpresa,
                CD_PRODUTO = detalle.CodigoProducto,
                CD_FAIXA = detalle.Faixa,
                NU_IDENTIFICADOR = detalle.Identificador?.Trim()?.ToUpper(),
                QT_REFERENCIA = detalle.CantidadReferencia,
                QT_ANULADA = detalle.CantidadAnulada,
                QT_AGENDADA = detalle.CantidadAgendada,
                QT_RECIBIDA = detalle.CantidadRecibida,
                QT_CONFIRMADA_INTERFAZ = detalle.CantidadConfirmadaInterfaz,
                IM_UNITARIO = detalle.ImporteUnitario,
                DT_VENCIMIENTO = detalle.FechaVencimiento,
                DS_ANEXO1 = detalle.Anexo1,
                DT_ADDROW = detalle.FechaInsercion,
                DT_UPDROW = detalle.FechaModificacion,
                NU_TRANSACCION = detalle.NumeroTransaccion,
                NU_TRANSACCION_DELETE = detalle.NumeroTransaccionDelete,
            };
        }

        public virtual ReferenciaRecepcionTipo MapToObject(T_RECEPCION_REFERENCIA_TIPO entity)
        {
            if (entity == null)
                return null;

            return new ReferenciaRecepcionTipo
            {
                Tipo = entity.TP_REFERENCIA,
                Descripcion = entity.DS_REFERENCIA,
            };
        }
    }
}
