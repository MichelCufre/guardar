using System.Collections.Generic;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Domain.Recepcion;
using WIS.Domain.Recepcion.Enums;
using WIS.Extension;
using WIS.WMS_API.Models.Mappers.Interfaces;

namespace WIS.WMS_API.Models.Mappers
{
    public class AgendaMapper : Mapper, IAgendaMapper
    {
        public AgendaMapper()
        {
        }

        public virtual List<Agenda> Map(AgendasRequest request)
        {
            List<Agenda> models = new List<Agenda>();

            foreach (var a in request.Agendas)
            {
                Agenda model = new Agenda(a.CodigoAgente, a.TipoAgente, a.TipoReferencia)
                {
                    IdEmpresa = request.Empresa,
                    TipoRecepcionInterno = a.TipoRecepcion,
                    NumeroDocumento = a.Referencia,
                    Predio = a.Predio,
                    LiberarAgenda = a.LiberarAgenda ?? false,
                    CodigoPuerta = a.PuertaDesembarco,
                    Anexo1 = a.Anexo1,
                    Anexo2 = a.Anexo2,
                    Anexo3 = a.Anexo3,
                    Anexo4 = a.Anexo4,
                    FechaEntrega = a.FechaEntrega,
                    PlacaVehiculo = a.PlacaVehiculo
                };

                models.Add(model);
            }

            return models;
        }

        public virtual AgendaResponse MapToResponse(Agenda agenda, string cdAgente, string tpAgente)
        {
            var agendaResponse = new AgendaResponse()
            {
                NroAgenda = agenda.Id,
                Empresa = agenda.IdEmpresa,
                CodigoAgente = cdAgente,
                TipoAgente = tpAgente,
                NumeroDocumento = agenda.NumeroDocumento,
                FechaInsercion = agenda.FechaInsercion.ToString(CDateFormats.DATE_ONLY),
                FechaModificacion = agenda.FechaModificacion.ToString(CDateFormats.DATE_ONLY),
                Predio = agenda.Predio,
                Estado = MapEstado(agenda.Estado),
                TipoDocumento = agenda.TipoDocumento,
                NroPuerta = agenda.CodigoPuerta,
                FechaInicio = agenda.FechaInicio.ToString(CDateFormats.DATE_ONLY),
                FechaFin = agenda.FechaFin.ToString(CDateFormats.DATE_ONLY),
                PlacaVehiculo = agenda.PlacaVehiculo,
                DUA = agenda.DUA,
                Anexo1 = agenda.Anexo1,
                Anexo2 = agenda.Anexo2,
                Anexo3 = agenda.Anexo3,
                Anexo4 = agenda.Anexo4,
                Averiado = this.MapBooleanToString(agenda.Averiado),
                FechaCierre = agenda.FechaCierre.ToString(CDateFormats.DATE_ONLY),
                FechaEntrega = agenda.FechaEntrega.ToString(CDateFormats.DATE_ONLY),
                TipoRecepcion = agenda.TipoRecepcionInterno,
                NumeroInterfazEjecucion = agenda.NumeroInterfazEjecucion
            };

            foreach (var detalle in agenda.Detalles)
            {
                agendaResponse.Detalles.Add(MapDetalleToResponse(detalle));
            }


            return agendaResponse;
        }
        public virtual AgendaDetalleResponse MapDetalleToResponse(AgendaDetalle det)
        {
            if (det == null)
                return null;

            var detalle = new AgendaDetalleResponse
            {
                CodigoProducto = det.CodigoProducto,
                //Faixa = det.Faixa,
                Identificador = det.Identificador?.Trim()?.ToUpper(),
                Estado = MapEstadoDetalle(det.Estado),
                CantidadAgendada = det.CantidadAgendada,
                CantidadRecibida = det.CantidadRecibida,
                CantidadAceptada = det.CantidadAceptada,
                CantidadAgendadaOriginal = det.CantidadAgendadaOriginal,
                CantidadCrossDocking = det.CantidadCrossDocking,
                Vencimiento = det.Vencimiento.ToString(CDateFormats.DATE_ONLY),
                FechaAlta = det.FechaAlta.ToString(CDateFormats.DATE_ONLY),
                FechaModificacion = det.FechaModificacion.ToString(CDateFormats.DATE_ONLY),
                FechaAceptacionProblema = det.FechaAceptacionRecepcion.ToString(CDateFormats.DATE_ONLY),
                PrecioCIF = det.CIF,
                PrecioFOB = det.Precio
            };

            return detalle;
        }

        public virtual short MapEstado(EstadoAgenda estado)
        {
            switch (estado)
            {
                case EstadoAgenda.Abierta: return EstadoAgendaDb.Abierta;
                case EstadoAgenda.Cerrada: return EstadoAgendaDb.Cerrada;
                case EstadoAgenda.Cancelada: return EstadoAgendaDb.Cancelada;
                case EstadoAgenda.IngresandoFactura: return EstadoAgendaDb.IngresandoFactura;
                case EstadoAgenda.ConferidaConDiferencias: return EstadoAgendaDb.ConferidaConDiferencias;
                case EstadoAgenda.ConferidaSinDiferencias: return EstadoAgendaDb.ConferidaSinDiferencias;
                case EstadoAgenda.AguardandoDesembarque: return EstadoAgendaDb.AguardandoDesembarque;

            }

            return -1;
        }
        public virtual short MapEstadoDetalle(EstadoAgendaDetalle estado)
        {
            switch (estado)
            {
                case EstadoAgendaDetalle.Abierta: return EstadoAgendaDetalleDb.Abierta;
                case EstadoAgendaDetalle.ConferidaConDiferencias: return EstadoAgendaDetalleDb.ConferidaConDiferencias;
                case EstadoAgendaDetalle.ConferidaSinDiferencias: return EstadoAgendaDetalleDb.ConferidaSinDiferencias;
            }

            return -1;
        }
    }
}
