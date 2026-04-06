using System.Collections.Generic;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Domain.Parametrizacion;
using WIS.Domain.StockEntities;
using WIS.Extension;
using WIS.WMS_API.Controllers.Entrada;
using WIS.WMS_API.Models.Mappers.Interfaces;

namespace WIS.WMS_API.Models.Mappers
{
    public class LpnMapper : Mapper, ILpnMapper
    {
        public virtual List<Lpn> Map(LpnsRequest request)
        {
            List<Lpn> models = new List<Lpn>();

            foreach (var lpn in request.Lpns)
            {
                models.Add(new Lpn()
                {
                    Tipo = lpn.Tipo,
                    IdExterno = lpn.IdExterno?.Trim(),
                    IdPacking = lpn.IdPacking?.Trim(),
                    Empresa = request.Empresa,
                    Estado = EstadosLPN.Importado,
                    BarrasSinDefinir = MapBarras(lpn.Barras),
                    AtributosSinDefinir = MapAtributos(lpn.Atributos),
                    Detalles = MapDetalles(lpn.Detalles, request.Empresa),
                });
            }

            return models;
        }

        public virtual List<LpnDetalle> MapDetalles(List<LpnDetalleRequest> detalles, int empresa)
        {
            var models = new List<LpnDetalle>();

            foreach (var d in detalles)
            {
                models.Add(MapDetalle(d, empresa));
            }

            return models;
        }

        public virtual LpnDetalle MapDetalle(LpnDetalleRequest detalle, int empresa)
        {
            return new LpnDetalle()
            {
                IdLineaSistemaExterno = detalle.IdLineaSistemaExterno,
                Empresa = empresa,
                CodigoProducto = detalle.CodigoProducto,
                Lote = detalle.Identificador?.Trim()?.ToUpper(),
                Vencimiento = detalle.FechaVencimiento,
                CantidadDeclarada = detalle.CantidadDeclarada,
                Faixa = 1,
                Cantidad = 0,
                CantidadReserva = 0,
                CantidadRecibida = 0,
                CantidadExpedida = 0,
                AtributosSinDefinir = MapAtributos(detalle.Atributos),
                IdAveria = "N",
                IdCtrlCalidad = EstadoControlCalidad.Controlado,
                IdInventario = "R",
            };
        }

        public virtual List<AtributoValor> MapAtributos(List<AtributoRequest> atributos)
        {
            var models = new List<AtributoValor>();

            if (atributos != null)
            {
                foreach (var d in atributos)
                {
                    models.Add(MapAtributo(d));
                }
            }

            return models;
        }

        public virtual AtributoValor MapAtributo(AtributoRequest atributo)
        {
            return new AtributoValor()
            {
                Nombre = atributo.Nombre?.ToUpper(),
                Valor = atributo.Valor
            };
        }

        public virtual List<LpnBarras> MapBarras(List<BarrasRequest> barras)
        {
            var models = new List<LpnBarras>();

            if (barras != null)
            {
                foreach (var d in barras)
                {
                    models.Add(MapBarra(d));
                }
            }

            return models;
        }

        public virtual LpnBarras MapBarra(BarrasRequest barra)
        {
            return new LpnBarras()
            {
                CodigoBarras = barra.CodigoBarras?.Trim()?.ToUpper(),
                Tipo = barra.Tipo
            };
        }

        public virtual LpnResponse MapToResponse(Lpn lpn)
        {
            var response = new LpnResponse()
            {
                Numero = lpn.NumeroLPN,
                IdExterno = lpn.IdExterno,
                Empresa = lpn.Empresa,
                Tipo = lpn.Tipo,
                Estado = lpn.Estado,
                Ubicacion = lpn.Ubicacion,
                IdPacking = lpn.IdPacking,
                FechaInsercion = lpn.FechaAdicion.ToString(CDateFormats.DATE_ONLY),
                FechaModificacion = lpn.FechaModificacion.ToString(CDateFormats.DATE_ONLY),
                FechaActivacion = lpn.FechaActivacion?.ToString(CDateFormats.DATE_ONLY),
                FechaFin = lpn.FechaFin?.ToString(CDateFormats.DATE_ONLY),
                NroAgenda = lpn.NroAgenda,
            };

            foreach (var detalle in lpn.Detalles)
            {
                response.Detalles.Add(MapDetalleToResponse(detalle));
            }

            return response;
        }

        public virtual LpnDetalleResponse MapDetalleToResponse(LpnDetalle det)
        {
            if (det == null)
                return null;

            var detalle = new LpnDetalleResponse
            {
                Id = det.Id,
                Numero = det.NumeroLPN,
                IdLineaSistemaExterno = det.IdLineaSistemaExterno,
                Empresa = det.Empresa,
                CodigoProducto = det.CodigoProducto,
                Faixa = det.Faixa,
                Lote = det.Lote?.Trim()?.ToUpper(),
                Cantidad = det.Cantidad,
                Vencimiento = det.Vencimiento?.ToString(CDateFormats.DATE_ONLY),
                CantidadDeclarada = det.CantidadDeclarada,
                CantidadRecibida = det.CantidadRecibida,
                CantidadReserva = det.CantidadReserva,
                CantidadExpedida = det.CantidadExpedida,
                Averiado = this.MapStringToBoolean(det.IdAveria),
                MotivoAveria = det.DescripcionMotivoAveria,
                ControlDeCalidadPendiente = det.IdCtrlCalidad == EstadoControlCalidad.Pendiente,
            };

            return detalle;
        }
    }
}
