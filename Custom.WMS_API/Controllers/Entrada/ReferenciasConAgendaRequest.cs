using System.Collections.Generic;
using WIS.Common.API.Attributes;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Domain.Validation;

namespace Custom.WMS_API.Controllers.Entrada
{
    /// <summary>
    /// Request custom que extiende ReferenciasRecepcionRequest agregando la creación de agendas
    /// en la misma operación.
    /// </summary>
    public class ReferenciasConAgendaRequest : ReferenciasRecepcionRequest
    {
        /// <summary>
        /// Lista de agendas a crear junto con las referencias
        /// </summary>
        [RequiredListValidation]
        public List<AgendaRequest> Agendas { get; set; }

        public ReferenciasConAgendaRequest()
        {
            Agendas = new List<AgendaRequest>();
        }

        /// <summary>
        /// Convierte al request estándar de referencias para reutilizar el mapper existente
        /// </summary>
        public ReferenciasRecepcionRequest ToReferenciasRecepcionRequest()
        {
            return new ReferenciasRecepcionRequest
            {
                Empresa = this.Empresa,
                DsReferencia = this.DsReferencia,
                Archivo = this.Archivo,
                IdRequest = this.IdRequest,
                Referencias = this.Referencias
            };
        }

        /// <summary>
        /// Convierte al request estándar de agendas para reutilizar el mapper existente
        /// </summary>
        public AgendasRequest ToAgendasRequest()
        {
            return new AgendasRequest
            {
                Empresa = this.Empresa,
                DsReferencia = this.DsReferencia,
                Archivo = this.Archivo,
                IdRequest = this.IdRequest,
                Agendas = this.Agendas
            };
        }
    }
}
