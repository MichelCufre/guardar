using System.Collections.Generic;
using WIS.Domain.Recepcion;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class FacturaMapper : Mapper
    {
        public virtual T_RECEPCION_FACTURA MapToEntity(Factura obj)
        {
            if (obj == null) return null;

            var entity = this.MapFacturaSinDetallesToEntity(obj);
            var detalles = new List<T_RECEPCION_FACTURA_DET>();

            foreach (var linea in obj.Detalles)
            {
                detalles.Add(this.MapToEntity(linea));
            }

            entity.T_RECEPCION_FACTURA_DET = detalles;
            return entity;
        }

        public virtual T_RECEPCION_FACTURA MapFacturaSinDetallesToEntity(Factura obj)
        {
            if (obj == null) return null;

            return new T_RECEPCION_FACTURA
            {
                NU_RECEPCION_FACTURA = obj.Id,
                NU_SERIE = obj.Serie,
                NU_FACTURA = obj.NumeroFactura,
                TP_FACTURA = obj.TipoFactura,
                CD_EMPRESA = obj.IdEmpresa,
                DT_EMISION = obj.FechaEmision,
                IM_TOTAL_DIGITADO = obj.TotalDigitado,
                CD_CLIENTE = obj.CodigoInternoCliente,
                DT_VENCIMIENTO = obj.FechaVencimiento,
                CD_MONEDA = obj.CodigoMoneda,
                NU_PREDIO = obj.Predio,
                DS_ANEXO1 = obj.Anexo1,
                DS_ANEXO2 = obj.Anexo2,
                DS_ANEXO3 = obj.Anexo3,
                DS_OBSERVACION = obj.Observacion,
                DT_UPDROW = obj.FechaCreacion,
                DT_ADDROW = obj.FechaModificacion,
                CD_SITUACAO = obj.Situacion,
                ND_ESTADO = obj.Estado,
                ID_ORIGEN = obj.IdOrigen,
                NU_TRANSACCION = obj.NumeroTransaccion,
                NU_TRANSACCION_DELETE = obj.NumeroTransaccionDelete,
                IM_IVA_BASE = obj.IvaBase,
                IM_IVA_MINIMO = obj.IvaMinimo,
                NU_AGENDA = obj.Agenda,
                NU_REFERENCIA = obj.Referencia,
                NU_REMITO = obj.Remito,
            };
        }

        public virtual T_RECEPCION_FACTURA_DET MapToEntity(FacturaDetalle obj)
        {
            if (obj == null) return null;

            return new T_RECEPCION_FACTURA_DET
            {
                NU_RECEPCION_FACTURA_DET = obj.Id,
                NU_RECEPCION_FACTURA = obj.IdFactura,
                CD_EMPRESA = obj.IdEmpresa,
                CD_PRODUTO = obj.Producto,
                CD_FAIXA = obj.Faixa,
                NU_IDENTIFICADOR = obj.Identificador,
                QT_VALIDADA = obj.CantidadValidada,
                QT_RECIBIDA = obj.CantidadRecibida,
                IM_UNITARIO_DIGITADO = obj.ImporteUnitario,
                DT_VENCIMIENTO = obj.FechaVencimiento,
                DT_ADDROW = obj.FechaCreacion,
                DT_UPDROW = obj.FechaModificacion,
                NU_TRANSACCION = obj.NumeroTransaccion,
                NU_TRANSACCION_DELETE = obj.NumeroTransaccionDelete,
                QT_FACTURADA = obj.CantidadFacturada,
                DS_ANEXO1 = obj.Anexo1,
                DS_ANEXO2 = obj.Anexo2,
                DS_ANEXO3 = obj.Anexo3,
                DS_ANEXO4 = obj.Anexo4
            };
        }

        public virtual Factura MapToObject(T_RECEPCION_FACTURA entity, ICollection<T_RECEPCION_FACTURA_DET> detalles)
        {
            if (entity == null) return null;

            var lineas = new List<FacturaDetalle>();

            if (detalles != null)
            {
                foreach (var detalle in detalles)
                {
                    lineas.Add(this.MapToObject(detalle));
                }
            }

            return new Factura
            {
                Id = entity.NU_RECEPCION_FACTURA,
                Serie = entity.NU_SERIE,
                NumeroFactura = entity.NU_FACTURA,
                TipoFactura = entity.TP_FACTURA,
                IdEmpresa = entity.CD_EMPRESA,
                FechaEmision = entity.DT_EMISION,
                TotalDigitado = entity.IM_TOTAL_DIGITADO,
                CodigoInternoCliente = entity.CD_CLIENTE,
                FechaVencimiento = entity.DT_VENCIMIENTO,
                CodigoMoneda = entity.CD_MONEDA,
                Predio = entity.NU_PREDIO,
                Anexo1 = entity.DS_ANEXO1,
                Anexo2 = entity.DS_ANEXO2,
                Anexo3 = entity.DS_ANEXO3,
                Observacion = entity.DS_OBSERVACION,
                FechaModificacion = entity.DT_UPDROW,
                FechaCreacion = entity.DT_ADDROW,
                Situacion = entity.CD_SITUACAO,
                Estado = entity.ND_ESTADO,
                IdOrigen = entity.ID_ORIGEN,
                NumeroTransaccion = entity.NU_TRANSACCION,
                NumeroTransaccionDelete = entity.NU_TRANSACCION_DELETE,
                Agenda = entity.NU_AGENDA,
                IvaBase = entity.IM_IVA_BASE,
                IvaMinimo = entity.IM_IVA_MINIMO,
                Referencia = entity.NU_REFERENCIA,
                Remito = entity.NU_REMITO,
                Detalles = lineas,
            };
        }

        public virtual FacturaDetalle MapToObject(T_RECEPCION_FACTURA_DET entity)
        {
            if (entity == null) return null;

            return new FacturaDetalle
            {
                Id = entity.NU_RECEPCION_FACTURA_DET,
                IdFactura = entity.NU_RECEPCION_FACTURA,
                IdEmpresa = entity.CD_EMPRESA,
                Producto = entity.CD_PRODUTO,
                Faixa = entity.CD_FAIXA,
                Identificador = entity.NU_IDENTIFICADOR.Trim(),
                CantidadValidada = entity.QT_VALIDADA,
                CantidadRecibida = entity.QT_RECIBIDA,
                ImporteUnitario = entity.IM_UNITARIO_DIGITADO,
                FechaVencimiento = entity.DT_VENCIMIENTO,
                FechaCreacion = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UPDROW,
                NumeroTransaccion = entity.NU_TRANSACCION,
                NumeroTransaccionDelete = entity.NU_TRANSACCION_DELETE,
                CantidadFacturada = entity.QT_FACTURADA,
                Anexo1 = entity.DS_ANEXO1,
                Anexo2 = entity.DS_ANEXO2,
                Anexo3 = entity.DS_ANEXO3,
                Anexo4 = entity.DS_ANEXO4
            };
        }

        public virtual Factura MapFacturaToObject(T_RECEPCION_FACTURA entity)
        {
            if (entity == null) return null;

            var lineas = new List<FacturaDetalle>();

            foreach (var detalle in entity.T_RECEPCION_FACTURA_DET)
            {
                lineas.Add(this.MapToObject(detalle));
            }

            return new Factura
            {
                Id = entity.NU_RECEPCION_FACTURA,
                Serie = entity.NU_SERIE,
                NumeroFactura = entity.NU_FACTURA,
                TipoFactura = entity.TP_FACTURA,
                IdEmpresa = (int)entity.CD_EMPRESA,
                FechaEmision = entity.DT_EMISION,
                TotalDigitado = entity.IM_TOTAL_DIGITADO,
                CodigoInternoCliente = entity.CD_CLIENTE,
                FechaVencimiento = entity.DT_VENCIMIENTO,
                CodigoMoneda = entity.CD_MONEDA,
                Predio = entity.NU_PREDIO,
                Anexo1 = entity.DS_ANEXO1,
                Anexo2 = entity.DS_ANEXO2,
                Anexo3 = entity.DS_ANEXO3,
                Observacion = entity.DS_OBSERVACION,
                FechaModificacion = entity.DT_UPDROW,
                FechaCreacion = entity.DT_ADDROW,
                Situacion = entity.CD_SITUACAO,
                Estado = entity.ND_ESTADO,
                IdOrigen = entity.ID_ORIGEN,
                NumeroTransaccion = entity.NU_TRANSACCION,
                NumeroTransaccionDelete = entity.NU_TRANSACCION_DELETE,
                Agenda = entity.NU_AGENDA,
                IvaBase = entity.IM_IVA_BASE,
                IvaMinimo = entity.IM_IVA_MINIMO,
                Detalles = lineas
            };
        }
    }
}
