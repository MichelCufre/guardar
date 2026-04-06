using System;
using System.Collections.Generic;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Domain.Produccion;
using WIS.Domain.Produccion.Models;
using WIS.WMS_API.Models.Mappers.Interfaces;

namespace WIS.WMS_API.Models.Mappers
{
    public class ProducirProduccionMapper : IProducirProduccionMapper
    {
        public ProducirProduccionMapper()
        {
        }

        public virtual ProducirProduccion Map(ProducirProduccionRequest request)
        {
            var productos = new List<SalidaProduccionDetalle>();

            foreach (var detalle in request.Productos)
            {
                productos.Add(new SalidaProduccionDetalle()
                {
                    Producto = detalle.Producto,
                    Identificador = detalle.Identificador?.Trim()?.ToUpper(),
                    Ubicacion = detalle.Ubicacion,
                    Vencimiento = detalle.Vencimiento,
                    Cantidad = detalle.Cantidad,
                    Motivo = detalle.Motivo,
                    Faixa = 1,
                    FechaAlta = DateTime.Now,
                    Empresa = request.Empresa,
                });
            }

            var produccion = new ProducirProduccion()
            {
                Empresa = request.Empresa,
                IdProduccionExterno = request.IdProduccionExterno,
                IdEspacio = request.IdEspacio,
                ConfirmarMovimiento = request.ConfirmarMovimiento,
                FinalizarProduccion = request.FinalizarProduccion,
                Productos = productos
            };

            return produccion;
        }
    }
}
