using System;
using System.Collections.Generic;
using System.Linq;
using WIS.AutomationManager.Models.Mappers.Interfaces;
using WIS.Domain.Automatismo.Dtos;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General.API.Dtos.Entrada;

namespace WIS.AutomationManager.Models.Mappers
{
    public class ConfirmacionSalidaAutomatismoMapper : IConfirmacionSalidaAutomatismoMapper
    {
        public PickingRequest Map(ConfirmacionSalidaStockRequest request)
        {
            var picking = new PickingRequest()
            {
                Empresa = request.Empresa,
                EstadoSalida = request.EstadoSalida,
                Usuario = request.Usuario,
                IdRequest = request.IdSalida
            };

            foreach (var contenedor in request.Contenedores)
            {
                foreach (var det in contenedor.Productos.GroupBy(w => new { w.Producto, w.Identificador }))
                {
                    picking.Detalles.Add(new DetallePickingRequest()
                    {
                        CodigoProducto = det.Key.Producto,
                        Identificador = det.Key.Identificador,
                        Cantidad = det.Sum(s => s.Cantidad),
                        IdExternoContenedor = contenedor.IdMatricula,
                        TipoContenedor = BarcodeDb.TIPO_CONTENEDOR_W,
                        FechaVencimiento = det.Min(m => m.FechaVencimiento)
                    });
                }
            }

            if (request.Detalles != null && request.Detalles.Count > 0)
            {
                picking.DetallesFinalizados = new List<DetallePickingFinalizadoRequest>();

                foreach (var det in request.Detalles)
                {
                    picking.DetallesFinalizados.Add(new DetallePickingFinalizadoRequest()
                    {
                        CodigoProducto = det.Producto,
                        CantidadPreparada = det.CantidadPreparada,
                        CantidadSolicitada = det.CantidadSolicitada
                    });
                }
            }

            return picking;
        }
    }
}
