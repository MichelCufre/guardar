using System;
using System.Collections.Generic;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.WMS_API.Models.Mappers.Interfaces;

namespace WIS.WMS_API.Models.Mappers
{
    public class PickingProductoMapper : Mapper, IPickingProductoMapper
    {
        public PickingProductoMapper()
        {
        }

        public virtual List<UbicacionPickingProducto> Map(PickingProductosRequest request)
        {
            List<UbicacionPickingProducto> ubicaciones = new List<UbicacionPickingProducto>();

            foreach (var ubicacionPicking in request.PickingProductos)
            {
                UbicacionPickingProducto ubicacion = new UbicacionPickingProducto(ubicacionPicking.TipoOperacion);
                ubicacion.Empresa = request.Empresa;
                ubicacion.CodigoProducto = ubicacionPicking.CodigoProducto;
                ubicacion.Padron = ubicacionPicking.Padron;
                ubicacion.StockMinimo = ubicacionPicking.StockMinimo;
                ubicacion.StockMaximo = ubicacionPicking.StockMaximo;
                ubicacion.CantidadDesborde = ubicacionPicking.CantidadDesborde;
                ubicacion.UbicacionSeparacion = ubicacionPicking.Ubicacion;
                ubicacion.FechaInsercion = DateTime.Now;
                ubicacion.CodigoUnidadCajaAutomatismo = ubicacionPicking.codigoUnidadCajaAutomatismo;
                ubicacion.CantidadUnidadCajaAutomatismo = ubicacionPicking.cantidadUnidadCajaAutomatismo;
                ubicacion.FlagConfirmarCodBarrasAutomatismo = ubicacionPicking.flagConfirmarCodBarrasAutomatismo;
                ubicacion.Prioridad = ubicacionPicking.prioridad;

                ubicaciones.Add(ubicacion);
            }

            return ubicaciones;
        }

        public virtual PickingProductoResponse MapToResponse(UbicacionPickingProducto ubicacionPicking)
        {
            return new PickingProductoResponse()
            {
                Id = ubicacionPicking.Id,
                Empresa = ubicacionPicking.Empresa,
                CodigoProducto = ubicacionPicking.CodigoProducto,
                Faixa = ubicacionPicking.Faixa,
                Padron = ubicacionPicking.Padron,
                UbicacionSeparacion = ubicacionPicking.UbicacionSeparacion,
                StockMinimo = ubicacionPicking.StockMinimo,
                StockMaximo = ubicacionPicking.StockMaximo,
                CantidadDesborde = ubicacionPicking.CantidadDesborde,
                CantidadPadronDesborde = ubicacionPicking.CantidadPadronDesborde,
                TipoPicking = ubicacionPicking.TipoPicking,
                FechaAlta = ubicacionPicking.FechaInsercion?.ToString(CDateFormats.DATE_ONLY),
                FechaModificacion = ubicacionPicking.FechaModificacion?.ToString(CDateFormats.DATE_ONLY),
                Predio = ubicacionPicking.Predio,
                NuTransaccion = ubicacionPicking.NuTransaccion,
                NuTransaccionDelete = ubicacionPicking.NuTransaccionDelete,
                CodigoUnidadCajaAutomatismo = ubicacionPicking.CodigoUnidadCajaAutomatismo,
                CantidadUnidadCajaAutomatismo = ubicacionPicking.CantidadUnidadCajaAutomatismo,
                FlagConfirmarCodBarrasAutomatismo = ubicacionPicking.FlagConfirmarCodBarrasAutomatismo,
                Prioridad = ubicacionPicking.Prioridad
            };
        }
    }
}
