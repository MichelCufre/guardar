using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WIS.Common.API.Attributes;

namespace WIS.WebhookClient.Models
{
    public class AgendaRequest
    {
        /// <summary>
        /// Número de agenda asignado por WIS
        /// </summary>
        /// <example>100</example>
        [ApiDtoExample("100")]
        [Required]
        public int Agenda { get; set; } // NU_AGENDA

        /// <summary>
        /// Código de empresa
        /// </summary>
        [ApiDtoExample("12")]
        [Required]
        public int Empresa { get; set; } // CD_EMPRESA

        /// <summary>
        /// Tipo de agente 
        /// PRO (Proveedor) - CLI (Cliente)
        /// </summary>
        [ApiDtoExample("PRO")]
        [Required]
        public string TipoAgente { get; set; } // TP_AGENTE

        /// <summary>
        /// Código del agente
        /// </summary>
        [ApiDtoExample("1999992")]
        [Required]
        public string CodigoAgente { get; set; } // CD_AGENTE

        /// <summary>
        /// Tipo de documento o movimiento en el ERP
        /// </summary>
        /// <example>BOLSA-OC</example>
        //[Required]
        //public string TipoRecepcionExterno { get; set; } // TP_RECEPCION_EXTERNO

        /// <summary>
        /// Identificación del tipo de recepción o flujo dentro de WIS
        /// </summary>
        [ApiDtoExample("WISBLI1")]
        [Required]
        public string TipoRecepcion { get; set; } // TP_RECEPCION

        /// <summary>
        /// Número de referencia si el ingreso es contra un documento único. En otro caso, vacío
        /// </summary>
        [ApiDtoExample("151615")]
        public string NumeroDocumento { get; set; } // NU_DOCUMENTO

        /// <summary>
        /// Fecha de ingreso
        /// </summary>
        /// <example>12/06/2016 13:19</example>
        [ApiDtoExample("12/06/2016 13:19")]
        [Required]
        public string FechaIngreso { get; set; } // DT_ADDROW

        /// <summary>
        /// Cierre de agenda
        /// </summary>
        [ApiDtoExample("12/06/2016 13:19")]
        [Required]
        public string FechaCierre { get; set; } // DT_CIERRE

        /// <summary>
        /// Información anexa
        /// </summary>
        /// <example></example>
        public string Anexo1 { get; set; } // DS_ANEXO1

        /// <summary>
        /// Información anexa
        /// </summary>
        /// <example></example>
        public string Anexo2 { get; set; } // DS_ANEXO2

        /// <summary>
        /// Información anexa
        /// </summary>
        /// <example></example>
        public string Anexo3 { get; set; } // DS_ANEXO3

        /// <summary>
        /// Información anexa
        /// </summary>
        /// <example></example>
        public string Anexo4 { get; set; } // DS_ANEXO4

        /// <summary>
        /// Predio
        /// </summary>
        [ApiDtoExample("1")]
        public string Predio { get; set; } // NU_PREDIO

        public List<AgendaDetalleRequest> Detalles { get; set; }

        public List<ReferenciaRequest> Referencias { get; set; }

        public List<FacturaRequest> Facturas { get; set; }

        public List<LpnRequest> Lpns { get; set; }

        public AgendaRequest()
        {
            Detalles = new List<AgendaDetalleRequest>();
            Referencias = new List<ReferenciaRequest>();
            Facturas = new List<FacturaRequest>();
            Lpns = new List<LpnRequest>();
        }
    }
}
