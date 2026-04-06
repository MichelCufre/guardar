using NLog;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.ManejoStock.Constants;
using WIS.Domain.Recepcion;
using WIS.Domain.Reportes;

namespace WIS.Domain.Logic
{
    public class LReportes
    {
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();

        protected readonly IUnitOfWork _uow;
        protected string _aplicacion;
        protected int _userId;

        public LReportes(IUnitOfWork uow, int userId, string aplicacion)
        {
            this._uow = uow;
            this._userId = userId;
            this._aplicacion = aplicacion;

        }

        public virtual List<long> PrepararReportes(Agenda agenda, string predio)
        {
            //Obtener reportes que se deben generar para la agenda
            var tiposReportes = _uow.ReporteRepository.GetReportesIdsAgenda(agenda);
            var reportes = new List<long>();

            if (tiposReportes.Count() > 0)
            {
                foreach (var tipoReporte in tiposReportes)
                {
                    var keyAgenda = agenda.GetCompositeId();

                    switch (tipoReporte)
                    {
                        //case CReporte.NOTA_DEVOLUCION:
                        case CReporte.CONFIRMACION_RECEPCION:

                            if (!_uow.ReporteRepository.AnyReporte(keyAgenda, "T_AGENDA", tipoReporte))
                            {
                                var puerta = GetPuertaEmbarque(agenda);
                                var zonaPuerta = _uow.UbicacionRepository.GetUbicacion(puerta.CodigoUbicacion).IdUbicacionZona;
                                var reportId = CrearReporte("T_AGENDA", tipoReporte, keyAgenda, predio, zonaPuerta);

                                if (reportId != -1)
                                {
                                    reportes.Add(reportId);
                                }
                            }

                            break;
                    }
                }
            }

            return reportes;
        }

        public virtual PuertaEmbarque GetPuertaEmbarque(Agenda agenda)
        {
            PuertaEmbarque puerta = null;

            if (agenda.CodigoPuerta.HasValue)
            {
                puerta = _uow.PuertaEmbarqueRepository.GetPuertaEmbarque(agenda.CodigoPuerta.Value);
            }
            else
            {
                var log = _uow.EtiquetaLoteRepository.GetFirstLogEtiqueta(agenda.Id, TiposMovimiento.Recepcion);
                puerta = _uow.PuertaEmbarqueRepository.GetPuertaEmbarqueByUbicacion(log.Ubicacion);
            }

            return puerta;
        }

        public virtual long CrearReporte(string tabla, string idReporteDefinicion, string claveReferencia, string predio, string zona, string nombreArchivo = null)
        {
            var nuevoReporte = new Reporte
            {
                Id = -1,
                Tipo = idReporteDefinicion,
                Usuario = _userId,
                NombreArchivo = nombreArchivo,
                Estado = CReporte.Pendiente,
                Predio = predio,
                Zona = zona,
            };

            nuevoReporte.AddRelacion(tabla, claveReferencia);

            _uow.ReporteRepository.AddReporte(nuevoReporte);

            return nuevoReporte.Id;
        }
    }
}
