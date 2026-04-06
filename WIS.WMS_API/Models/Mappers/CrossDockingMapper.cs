using System.Collections.Generic;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Domain.Picking.Dtos;
using WIS.WMS_API.Models.Mappers.Interfaces;

namespace WIS.WMS_API.Models.Mappers
{
    public class CrossDockingMapper : ICrossDockingMapper
    {
        public virtual List<CrossDockingUnaFase> Map(CrossDockingUnaFaseRequest request)
        {
            List<CrossDockingUnaFase> listCrossdocking = new List<CrossDockingUnaFase>();

            foreach (var crossDocking in request.CrossDocking)
            {
                foreach (var detalle in crossDocking.Detalles)
                {
                    listCrossdocking.Add(new CrossDockingUnaFase()
                    {
                        Empresa = request.Empresa,
                        Agenda = crossDocking.Agenda,
                        CodigoAgente = detalle.CodigoAgente,
                        TipoAgente = detalle.TipoAgente,
                        Preparacion = crossDocking.Preparacion,
                        Ubicacion = crossDocking.Ubicacion,
                        Producto = detalle.Producto,
                        Identificador = detalle.Identificador?.Trim()?.ToUpper(),
                        IdExternoContenedor = detalle.IdExternoContenedor,
                        Cantidad = detalle.Cantidad,
                        TipoContenedor = detalle.TipoContenedor,
                        SituacionDestino = detalle.SituacionDestino,
                        Faixa = 1
                    });
                }

            }
            return listCrossdocking;
        }
    }
}
