using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Domain.General;
using WIS.WMSTrackingAPI.Models.Mappers.Interfaces;
using WIS.Domain.Tracking.Models;
using WIS.Extension;
using WIS.Domain.DataModel.Queries.Registro;
using System;

namespace WIS.WMSTrackingAPI.Models.Mappers
{
    public class RutaMapper : Mapper, IRutaMapper
    {
        public RutaMapper()
        {
        }

        public virtual Ruta Map(RutaZonaRequest request)
        {
            return new Ruta()
            {
                Descripcion = request.Descripcion,
                Zona = request.Zona,
                EstadoId = SituacionDb.Activo,
                FechaAlta = DateTime.Now,
                FechaSituacion = DateTime.Now,
                FechaModificacion = DateTime.Now,
            };
        }

        public virtual RutaResponse MapToResponse(Ruta ruta)
        {
            return new RutaResponse()
            {
                Id = ruta.Id,
                Descripcion = ruta.Descripcion,
                Situacion = ruta.EstadoId,
                CodigoPuerta = ruta.PuertaEmbarqueId,
                CodigoOnda = ruta.OndaId,
                ControlaOrdenDeCarga = ruta.ControlaOrdenDeCargaId,
                CodigoTransportista = ruta.Transportista,
                Zona = ruta.Zona,
                FechaAlta = ruta.FechaSituacion.ToString(CDateFormats.DATE_ONLY),
                FechaSituacion = ruta.FechaSituacion.ToString(CDateFormats.DATE_ONLY),
                FechaModificacion = ruta.FechaSituacion.ToString(CDateFormats.DATE_ONLY),
            };
        }

    }
}
