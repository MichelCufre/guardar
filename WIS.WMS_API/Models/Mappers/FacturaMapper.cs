using System;
using System.Collections.Generic;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Domain.General.API.Dtos.Salida;
using WIS.Domain.Recepcion;
using WIS.Extension;
using WIS.WMS_API.Models.Mappers.Interfaces;

namespace WIS.WMS_API.Models.Mappers
{
    public class FacturaMapper : Mapper, IFacturaMapper
    {
        public virtual List<Factura> Map(FacturasRequest request)
        {
            List<Factura> facturas = new List<Factura>();

            foreach (var f in request.Facturas)
            {
                Factura factura = new Factura();
                factura.IdEmpresa = request.Empresa;
                factura.CodigoInternoCliente = f.CodigoAgente;
                factura.CodigoAgente = f.CodigoAgente;
                factura.CodigoMoneda = f.CodigoMoneda;
                factura.NumeroFactura = f.Factura;
                factura.Predio = f.Predio;
                factura.Serie = f.Serie;
                factura.Observacion = f.Observacion;
                factura.TipoFactura = f.TipoFactura?.ToUpper();
                factura.TotalDigitado = f.TotalDigitado;
                factura.FechaVencimiento = f.FechaVencimiento;
                factura.FechaEmision = f.FechaEmision;
                factura.Anexo1 = f.Anexo1;
                factura.Anexo2 = f.Anexo2;
                factura.Anexo3 = f.Anexo3;
                factura.FechaCreacion = DateTime.Now;
                factura.FechaModificacion = DateTime.Now;
                factura.IvaBase = f.IvaBase;
                factura.IvaMinimo = f.IvaMinimo;

                foreach (var detalleRequest in f.Detalles)
                {
                    FacturaDetalle detalle = new FacturaDetalle();
                    detalle.Producto = detalleRequest.Producto;
                    detalle.Identificador = detalleRequest.Identificador;
                    detalle.CantidadFacturada = detalleRequest.CantidadFacturada;
                    detalle.ImporteUnitario = detalleRequest.UnitarioDigitado;
                    detalle.FechaVencimiento = detalleRequest.FechaVencimiento;
                    detalle.IdFactura = factura.Id;
                    detalle.IdEmpresa = factura.IdEmpresa;
                    detalle.Faixa = 1;
                    detalle.FechaCreacion = DateTime.Now;
                    detalle.FechaModificacion = DateTime.Now;
                    detalle.Anexo1 = detalleRequest.Anexo1;
                    detalle.Anexo2 = detalleRequest.Anexo2;
                    detalle.Anexo3 = detalleRequest.Anexo3;
                    detalle.Anexo4 = detalleRequest.Anexo4;

                    factura.Detalles.Add(detalle);
                }
                facturas.Add(factura);
            }

            return facturas;
        }

        public virtual FacturaResponse MapToResponse(Factura factura)
        {
            var facturaResponse = new FacturaResponse()
            {
                CodigoEmpresa = factura.IdEmpresa.ToString(),
                CodigoCliente = factura.CodigoInternoCliente,
                Serie = factura.Serie,
                Predio = factura.Predio,
                Factura = factura.NumeroFactura,
                TipoFactura = factura.TipoFactura,
                Anexo1 = factura.Anexo1,
                Anexo2 = factura.Anexo2,
                Anexo3 = factura.Anexo3,
                Observacion = factura.Observacion,
                TotalDigitado = factura.TotalDigitado,
                CodigoMoneda = factura.CodigoMoneda,
                Origen = factura.IdOrigen,
                FechaCreacion = factura.FechaCreacion.ToString(CDateFormats.DATE_ONLY),
                CodigoSituacion = factura.Situacion.ToString(),
                Estado = factura.Estado,
                ImporteIvaBase = factura.IvaBase,
                ImporteIvaMinimo = factura.IvaMinimo,
                Agenda = factura.Agenda.ToString(),
                NumeroOrdenCompra = factura.Referencia
            };

            foreach (var detalle in factura.Detalles)
            {
                facturaResponse.Detalles.Add(MapDetalleToResponse(detalle, facturaResponse));
            }

            return facturaResponse;
        }

        public virtual FacturaDetalleResponse MapDetalleToResponse(FacturaDetalle det, FacturaResponse facturaResponse)
        {
            if (det == null)
                return null;

            var detalle = new FacturaDetalleResponse
            {
                Producto = det.Producto,
                Identificador = det.Identificador,
                CantidadFacturada = det.CantidadFacturada ?? 0,
                CantidadValidada = det.CantidadValidada ?? 0,
                CantidadRecibida = det.CantidadRecibida ?? 0,
                ImporteUnitario = det.ImporteUnitario.ToString(),
                FechaVencimiento = det.FechaVencimiento.ToString(CDateFormats.DATE_ONLY),
                FechaCreacion = det.FechaCreacion.ToString(CDateFormats.DATE_ONLY),
                Anexo1 = det.Anexo1,
                Anexo2 = det.Anexo2,
                Anexo3 = det.Anexo3,
                Anexo4 = det.Anexo4,
            };

            return detalle;
        }
    }
}
