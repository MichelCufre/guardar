using System;
using System.Collections.Generic;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Domain.Produccion;
using WIS.Domain.Produccion.Models;
using WIS.WMS_API.Models.Mappers.Interfaces;

namespace WIS.WMS_API.Models.Mappers
{
    public class ConsumirProduccionMapper : IConsumirProduccionMapper
    {
        public ConsumirProduccionMapper()
        {
        }

        public virtual ConsumirProduccion Map(ConsumirProduccionRequest request)
        {
            var insumos = new List<IngresoProduccionDetalle>();

            if (request.Insumos != null && request.Insumos.Count > 0)
            {
                foreach (var detalle in request.Insumos)
                {
                    insumos.Add(new IngresoProduccionDetalle()
                    {
                        Producto = detalle.Producto,
                        Identificador = detalle.Identificador?.Trim()?.ToUpper(),
                        Ubicacion = detalle.Ubicacion,
                        Referencia = detalle.Referencia,
                        Cantidad = detalle.Cantidad,
                        Motivo = detalle.Motivo,
                        UsarSoloReserva = detalle.UsarSoloReserva,
                        Empresa = request.Empresa,
                        Faixa = 1,
                        FechaAlta = DateTime.Now,
                    });
                }
            }

            var consumo = new ConsumirProduccion()
            {
                Empresa = request.Empresa,
                IdProduccionExterno = request.IdProduccionExterno,
                IdEspacio = request.IdEspacio,
                ConfirmarMovimiento = request.ConfirmarMovimiento,
                FinalizarProduccion = request.FinalizarProduccion,
                Insumos = insumos,
                IniciarProduccion = request.IniciarProduccion,
            };

            return consumo;
        }
    }
}
