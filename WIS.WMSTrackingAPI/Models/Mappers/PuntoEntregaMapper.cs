using System.Collections.Generic;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Domain.Picking;
using WIS.Domain.Tracking.Models;
using WIS.WMSTrackingAPI.Models.Mappers.Interfaces;

namespace WIS.WMSTrackingAPI.Models.Mappers
{
    public class PuntoEntregaMapper : Mapper, IPuntoEntregaMapper
    {
        public PuntoEntregaMapper()
        {
        }

        public virtual PuntoEntregaAgentes Map(PuntoEntregaAgentesRequest request)
        {
            var model = new PuntoEntregaAgentes()
            {
                CodigoPuntoEntrega = request.CodigoPuntoEntrega,
                Zona = request.Zona
            };

            foreach (var a in request.Agentes)
            {
                model.Agentes.Add(new PuntoEntregaAgente()
                {
                    Codigo = a.Codigo,
                    Tipo = a.Tipo,
                    CodigoEmpresa = a.CodigoEmpresa,
                });
            }

            return model;
        }
    }
}
