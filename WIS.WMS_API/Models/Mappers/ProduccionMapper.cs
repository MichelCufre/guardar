using System;
using System.Collections.Generic;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Domain.Interfaces;
using WIS.Domain.Produccion;
using WIS.Domain.Produccion.Constants;
using WIS.Domain.Produccion.Models;
using WIS.WMS_API.Models.Mappers.Interfaces;

namespace WIS.WMS_API.Models.Mappers
{
    public class ProduccionMapper : Mapper, IProduccionMapper
    {
        public ProduccionMapper()
        {
        }

        #region Create

        public virtual List<IngresoProduccion> Map(ProduccionesRequest request, InterfazEjecucion ejecucion)
        {
            var models = new List<IngresoProduccion>();

            foreach (var i in request.Ingresos)
            {
                var model = new IngresoBlackBox
                {
                    IdProduccionExterno = i.IdProduccionExterno?.Trim(),
                    Empresa = request.Empresa,
                    Predio = i.Predio,
                    IdManual = "N",
                    Tipo = i.Tipo,
                    TipoDeFlujo = i.TipoDeFlujo,
                    Anexo1 = i.Anexo1,
                    Anexo2 = i.Anexo2,
                    Anexo3 = i.Anexo3,
                    Anexo4 = i.Anexo4,
                    Anexo5 = i.Anexo5,
                    GeneraPedido = i.GenerarPedido ?? false,
                    LiberarPedido = i.LiberarPedido ?? false,
                    FechaAlta = DateTime.Now,
                    IdFormula = i.CodigoFormula,
                    EjecucionEntrada = ejecucion.Id,
                    IdModalidadLote = i.IdModalidadLote,
                    Funcionario = ejecucion.UserId ?? 0,
                    IdEspacioProducion = i.EspacioProduccion,
                    Situacion = SituacionDb.PRODUCCION_CREADA,
                    CantidadIteracionesFormula = i.CantidadFormula ?? 0,
                    Detalles = MapDetalles(i, request.Empresa)
                };

                models.Add(model);
            }

            return models;
        }

        public virtual List<IngresoProduccionDetalleTeorico> MapDetalles(ProduccionRequest request, int empresa)
        {
            var models = new List<IngresoProduccionDetalleTeorico>();

            foreach (var i in request.Insumos)
            {
                models.Add(MapDetalle(i, empresa));
            }

            foreach (var p in request.Productos)
            {
                models.Add(MapDetalle(p, empresa));
            }
            return models;
        }

        public virtual IngresoProduccionDetalleTeorico MapDetalle(ProduccionInsumoRequest request, int empresa)
        {
            return new IngresoProduccionDetalleTeorico
            {
                Empresa = empresa,
                Tipo = CIngresoProduccionDetalleTeorico.TipoDetalleEntrada,
                Producto = request.CodigoProducto,
                Faixa = 1,
                CantidadTeorica = request.CantidadTeorica,
                Identificador = request.Identificador?.Trim()?.ToUpper(),
                CantidadPedidoGenerada = 0,
                CantidadAbastecido = 0,
                CantidadConsumida = 0,
                FechaAlta = DateTime.Now,
                Anexo1 = request.Anexo1,
                Anexo2 = request.Anexo2,
                Anexo3 = request.Anexo3,
                Anexo4 = request.Anexo4
            };
        }

        public virtual IngresoProduccionDetalleTeorico MapDetalle(ProduccionProductoFinalesRequest request, int empresa)
        {
            return new IngresoProduccionDetalleTeorico
            {
                Empresa = empresa,
                Tipo = CIngresoProduccionDetalleTeorico.TipoDetalleSalida,
                Producto = request.CodigoProducto,
                Faixa = 1,
                CantidadTeorica = request.CantidadTeorica,
                CantidadPedidoGenerada = 0,
                CantidadAbastecido = 0,
                CantidadConsumida = 0,
                FechaAlta = DateTime.Now,
                Anexo1 = request.Anexo1,
                Anexo2 = request.Anexo2,
                Anexo3 = request.Anexo3,
                Anexo4 = request.Anexo4
            };
        }

        #endregion

        #region GetProduccion

        public virtual ProduccionResponse MapToResponse(IngresoProduccion ingreso)
        {
            var response = new ProduccionResponse()
            {
                NroIngresoProduccion = ingreso.Id,
                IdProduccionExterno = ingreso.IdProduccionExterno,
                Empresa = ingreso.Empresa.Value,
                Predio = ingreso.Predio,
                Tipo = ingreso.Id,
                EspacioProducion = ingreso.IdEspacioProducion,
                CodigoFormula = ingreso.IdFormula,
                CantidadFormula = ingreso.CantidadIteracionesFormula,
                IdModalidadLote = ingreso.IdModalidadLote,
                GeneraPedido = MapBooleanToString(ingreso.GeneraPedido),
                Funcionario = ingreso.Funcionario,
                Situacion = ingreso.Situacion,
                FechaAlta = ingreso.FechaAlta?.ToString(CDateFormats.DATE_ONLY),
                FechaModificacion = ingreso.FechaActualizacion?.ToString(CDateFormats.DATE_ONLY),
                NroIngresoProduccionOriginal = ingreso.NumeroProduccionOriginal,
                Anexo1 = ingreso.Anexo1,
                Anexo2 = ingreso.Anexo2,
                Anexo3 = ingreso.Anexo3,
                Anexo4 = ingreso.Anexo4,
                Anexo5 = ingreso.Anexo5,
                FechaInicioProduccion = ingreso.FechaInicioProduccion?.ToString(CDateFormats.DATE_ONLY),
                FechaFinProduccion = ingreso.FechaFinProduccion?.ToString(CDateFormats.DATE_ONLY),
                IdManual = ingreso.IdManual,
                NroInterfazEjecucionEntrada = ingreso.EjecucionEntrada,
                PosicionEnCola = ingreso.PosicionEnCola,
                PermiteAutoAsignarLinea = ingreso.PermitirAutoasignarLinea,
                NroUltInterfazEjecucion = ingreso.NroUltInterfazEjecucion
            };

            foreach (var detalle in ingreso.Detalles)
            {
                response.DetallesTeoricos.Add(MapDetalleToResponse(detalle));
            }

            foreach (var detalle in ingreso.Consumidos)
            {
                response.Insumos.Add(MapDetalleToResponse(detalle));
            }

            foreach (var detalle in ingreso.Producidos)
            {
                response.ProductosProducidos.Add(MapDetalleToResponse(detalle));
            }

            return response;
        }

        public virtual ProduccionDetalleTeoricoResponse MapDetalleToResponse(IngresoProduccionDetalleTeorico detalle)
        {
            if (detalle == null) return null;

            return new ProduccionDetalleTeoricoResponse()
            {
                Tipo = detalle.Tipo,
                Producto = detalle.Producto,
                Identificador = detalle.Identificador?.Trim()?.ToUpper(),
                CantidadTeorica = detalle.CantidadTeorica,
                CantidadPedidoGenerada = detalle.CantidadConsumida,
                CantidadAbastecido = detalle.CantidadAbastecido,
                CantidadConsumida = detalle.CantidadConsumida,
                Anexo1 = detalle.Anexo1,
                Anexo2 = detalle.Anexo2,
                Anexo3 = detalle.Anexo3,
                Anexo4 = detalle.Anexo4,
            };
        }

        public virtual ProduccionInsumoResponse MapDetalleToResponse(IngresoProduccionDetalleReal detalle)
        {
            if (detalle == null) return null;

            return new ProduccionInsumoResponse()
            {
                Producto = detalle.Producto,
                Identificador = detalle.Identificador?.Trim()?.ToUpper(),
                CantidadReal = detalle.QtReal,
                CantidadRealOriginal = detalle.QtRealOriginal,
                CantidadDesafectada = detalle.QtDesafectado,
                CantidadNotificada = detalle.QtNotificado,
                CantidadMerma = detalle.QtMerma,
                NuOrden = detalle.NuOrden,
                Anexo1 = detalle.DsAnexo1,
                Anexo2 = detalle.DsAnexo2,
                Anexo3 = detalle.DsAnexo3,
                Anexo4 = detalle.DsAnexo4,
                Referencia = detalle.Referencia,
            };
        }

        public virtual ProduccionProducidosResponse MapDetalleToResponse(IngresoProduccionDetalleSalida detalle)
        {
            if (detalle == null) return null;

            return new ProduccionProducidosResponse()
            {
                Producto = detalle.Producto,
                Identificador = detalle.Identificador?.Trim()?.ToUpper(),
                Vencimiento = detalle.DtVencimiento?.ToString(CDateFormats.DATE_ONLY),
                CantidadTeorica = detalle.CantidadTeorica,
                CantidadProducida = detalle.QtProducido,
                CantidadNotificada = detalle.QtNotificado,
                Motivo = detalle.NdMotivo,
                DescMotivo = detalle.DsMotivo,
                NuOrden = detalle.NuOrden,
                Anexo1 = detalle.DsAnexo1,
                Anexo2 = detalle.DsAnexo2,
                Anexo3 = detalle.DsAnexo3,
                Anexo4 = detalle.DsAnexo4,
                NuPrdcIngresoTeorico = detalle.NuPrdcIngresoTeorico,
            };
        }

        #endregion
    }
}
