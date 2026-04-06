using System.Collections.Generic;
using WIS.Domain.General.API.Dtos.Salida;
using WIS.Domain.Interfaces;
using WIS.WMS_API.Models.Mappers.Interfaces;

namespace WIS.WMS_API.Models.Mappers
{
    public class EjecucionMapper : IEjecucionMapper
    {
        public virtual EjecucionesPendientesResponse MapToResponse(List<InterfazEjecucion> interfaces)
        {
            EjecucionesPendientesResponse response = new EjecucionesPendientesResponse();

            foreach (var i in interfaces)
            {
                response.Ejecuciones.Add(new EjecucionPendienteResponse
                {
                    NumeroInterfazEjecucion = i.Id,
                    CodigoInterfazExterna = (int)i.CdInterfazExterna
                });
            }

            return response;
        }

        public virtual EstadoEjecucionResponse MapToResponse(InterfazEstado estado)
        {
            var interfaz = estado.Interfaz;
            var errores = estado.Errores;
            var response = new EstadoEjecucionResponse();

            response.Estado = "OK";
            response.NumeroInterfazEjecucion = interfaz.Id;
            response.Empresa = interfaz.Empresa ?? -1;
            response.Mensaje = "Sin errores";

            if (errores != null && errores.Count > 0)
            {
                response.Estado = "ERROR";
                response.Mensaje = "Se encontraron errores";
                foreach (var e in errores)
                {
                    response.Errores.Add(new EstadoEjecucionErrorResponse
                    {
                        NroRegistro = e.Registro ?? -1,
                        NroError = e.NroError,
                        Error = e.Descripcion
                    });
                }
            }
            return response;
        }
    }
}
