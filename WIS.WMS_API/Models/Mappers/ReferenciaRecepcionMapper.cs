using System.Collections.Generic;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Domain.Recepcion;
using WIS.Extension;
using WIS.WMS_API.Models.Mappers.Interfaces;

namespace WIS.WMS_API.Models.Mappers
{
    public class ReferenciaRecepcionMapper : Mapper, IReferenciaRecepcionMapper
    {
        public ReferenciaRecepcionMapper()
        {
        }

        public virtual List<ReferenciaRecepcion> Map(ReferenciasRecepcionRequest request)
        {
            List<ReferenciaRecepcion> referencias = new List<ReferenciaRecepcion>();

            foreach (var r in request.Referencias)
            {
                ReferenciaRecepcion referencia = new ReferenciaRecepcion(r.CodigoAgente, r.TipoAgente);
                referencia.Numero = r.Referencia;
                referencia.IdEmpresa = request.Empresa;
                referencia.TipoReferencia = r.TipoReferencia;
                referencia.Memo = r.Memo;
                referencia.Anexo1 = r.Anexo1;
                referencia.Anexo2 = r.Anexo2;
                referencia.Anexo3 = r.Anexo3;
                referencia.IdPredio = r.Predio;
                referencia.Moneda = r.Moneda;
                referencia.Serializado = r.Serializado;
                referencia.FechaEmitida = r.FechaEmitida;
                referencia.FechaEntrega = r.FechaEntrega;
                referencia.FechaVencimientoOrden = r.FechaVencimientoOrden;

                foreach (var detalleRequest in r.Detalles)
                {
                    ReferenciaRecepcionDetalle detalle = new ReferenciaRecepcionDetalle();
                    detalle.IdLineaSistemaExterno = detalleRequest.IdLineaSistemaExterno;
                    detalle.IdEmpresa = request.Empresa;
                    detalle.CodigoProducto = detalleRequest.CodigoProducto;
                    detalle.Anexo1 = detalleRequest.Anexo1;
                    detalle.Identificador = detalleRequest.Identificador?.Trim()?.ToUpper();
                    detalle.CantidadReferencia = detalleRequest.CantidadReferencia;
                    detalle.ImporteUnitario = detalleRequest.ImporteUnitario;
                    detalle.Faixa = 1;
                    detalle.FechaVencimiento = detalleRequest.FechaVencimiento;

                    referencia.Detalles.Add(detalle);
                }
                referencias.Add(referencia);
            }
            return referencias;
        }

        public virtual ReferenciaRecepcionResponse MapToResponse(ReferenciaRecepcion referencia, string tipoAgente, string codigoAgente)
        {
            var refResponde = new ReferenciaRecepcionResponse()
            {
                IdReferencia = referencia.Id,
                Referencia = referencia.Numero,
                TipoReferencia = referencia.TipoReferencia,
                TipoAgente = tipoAgente,
                CodigoAgente = codigoAgente,
                Empresa = referencia.IdEmpresa,
                Moneda = referencia.Moneda,
                Situacion = referencia.Situacion,
                Anexo1 = referencia.Anexo1,
                Anexo2 = referencia.Anexo2,
                Anexo3 = referencia.Anexo3,
                Memo = referencia.Memo,
                Predio = referencia.IdPredio,
                Serializado = referencia.Serializado,
                EstadoReferencia = referencia.Estado,
                NumeroInterfazEjecucion = referencia.NumeroInterfazEjecucion,
                FechaEmitida = referencia.FechaEmitida.ToString(CDateFormats.DATE_ONLY),
                FechaEntrega = referencia.FechaEntrega.ToString(CDateFormats.DATE_ONLY),
                FechaInsercion = referencia.FechaInsercion.ToString(CDateFormats.DATE_ONLY),
                FechaModificacion = referencia.FechaModificacion.ToString(CDateFormats.DATE_ONLY),
                FechaVencimientoOrden = referencia.FechaVencimientoOrden.ToString(CDateFormats.DATE_ONLY),
            };

            foreach (var detalle in referencia.Detalles)
            {
                refResponde.Detalles.Add(MapDetalleToResponse(detalle));
            }

            return refResponde;
        }

        public virtual ReferenciaRecepcionDetalleResponse MapDetalleToResponse(ReferenciaRecepcionDetalle det)
        {
            if (det == null)
                return null;

            var detalle = new ReferenciaRecepcionDetalleResponse
            {
                IdReferenciaDetalle = det.Id,
                IdReferencia = det.IdReferencia,
                IdLineaSistemaExterno = det.IdLineaSistemaExterno,
                Empresa = det.IdEmpresa,
                CodigoProducto = det.CodigoProducto,
                Faixa = det.Faixa,
                Identificador = det.Identificador?.Trim()?.ToUpper(),
                CantidadReferencia = det.CantidadReferencia,
                CantidadAnulada = det.CantidadAnulada,
                CantidadAgendada = det.CantidadAgendada,
                CantidadRecibida = det.CantidadRecibida,
                CantidadConfirmadaInterfaz = det.CantidadConfirmadaInterfaz,
                Anexo1 = det.Anexo1,
                ImporteUnitario = det.ImporteUnitario,
                FechaInsercion = det.FechaInsercion.ToString(CDateFormats.DATE_ONLY),
                FechaVencimiento = det.FechaVencimiento.ToString(CDateFormats.DATE_ONLY),
                FechaModificacion = det.FechaModificacion.ToString(CDateFormats.DATE_ONLY)
            };

            return detalle;
        }

        public virtual List<ReferenciaRecepcion> Map(ModificacionDetalleReferenciaRequest request)
        {
            List<ReferenciaRecepcion> models = new List<ReferenciaRecepcion>();

            foreach (var r in request.Referencias)
            {
                ReferenciaRecepcion model = new ReferenciaRecepcion(r.CodigoAgente, r.TipoAgente);
                model.Numero = r.Referencia;
                model.IdEmpresa = request.Empresa;
                model.TipoReferencia = r.TipoReferencia;

                foreach (var detalleRequest in r.Detalles)
                {
                    ReferenciaRecepcionDetalle detalle = new ReferenciaRecepcionDetalle(detalleRequest.TipoOperacion);
                    detalle.IdLineaSistemaExterno = detalleRequest.IdLineaSistemaExterno;
                    detalle.IdEmpresa = request.Empresa;
                    detalle.CodigoProducto = detalleRequest.CodigoProducto;
                    detalle.Identificador = detalleRequest.Identificador?.Trim()?.ToUpper();
                    detalle.CantidadReferencia = detalleRequest.CantidadOperacion;
                    detalle.Faixa = 1;
                    model.Detalles.Add(detalle);
                }
                models.Add(model);
            }
            return models;
        }

        public virtual List<ReferenciaRecepcion> Map(AnularReferenciasRequest request)
        {
            List<ReferenciaRecepcion> models = new List<ReferenciaRecepcion>();

            foreach (var r in request.Referencias)
            {
                ReferenciaRecepcion model = new ReferenciaRecepcion(r.CodigoAgente, r.TipoAgente);
                model.Numero = r.Referencia;
                model.IdEmpresa = request.Empresa;
                model.TipoReferencia = r.TipoReferencia;
                models.Add(model);
            }
            return models;
        }
    }
}