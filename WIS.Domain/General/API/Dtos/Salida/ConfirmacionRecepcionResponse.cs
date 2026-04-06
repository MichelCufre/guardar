using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WIS.Common.API.Attributes;
using WIS.Domain.StockEntities;

namespace WIS.Domain.General.API.Dtos.Salida
{
    public class ConfirmacionRecepcionResponse
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
        /// <example>12</example>
        [ApiDtoExample("12")]
        [Required]
        public int Empresa { get; set; } // CD_EMPRESA

        /// <summary>
        /// Tipo de agente 
        /// PRO (Proveedor) - CLI (Cliente)
        /// </summary>
        /// <example>PRO</example>
        [ApiDtoExample("PRO")]
        [Required]
        public string TipoAgente { get; set; } // TP_AGENTE

        /// <summary>
        /// Código del agente
        /// </summary>
        /// <example>19990022</example>
        [ApiDtoExample("19990022")]
        [Required]
        public string CodigoAgente { get; set; } // CD_AGENTE

        /// <summary>
        /// Tipo de documento o movimiento en el ERP
        /// </summary>
        /// <example>BOLSA-OC</example>
        /// [ApiDtoExample("BOLSA-OC")]
        //[Required]
        //public string TipoRecepcionExterno { get; set; } // TP_RECEPCION_EXTERNO

        /// <summary>
        /// Identificación del tipo de recepción o flujo dentro de WIS
        /// </summary>
        /// <example>WISBL1</example>
        [ApiDtoExample("WISBL1")]
        [Required]
        public string TipoRecepcion { get; set; } // TP_RECEPCION

        /// <summary>
        /// Número de referencia si el ingreso es contra un documento único. En otro caso, vacío
        /// </summary>
        /// <example></example>
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
        /// <example>12/06/2016 13:19</example>
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
        /// <example>1</example>
        [ApiDtoExample("1")]
        public string Predio { get; set; } // NU_PREDIO

        public List<DetalleAgendaResponse> Detalles { get; set; }

        public List<ReferenciaResponse> Referencias { get; set; }

        public List<FacturaResponse> Facturas { get; set; }

        public List<LpnSalidaResponse> Lpns { get; set; }

        public List<EtiquetasResponse> Etiquetas { get; set; }

        public ConfirmacionRecepcionResponse()
        {
            Detalles = new List<DetalleAgendaResponse>();
            Referencias = new List<ReferenciaResponse>();
            Facturas = new List<FacturaResponse>();
            Lpns = new List<LpnSalidaResponse>();
            Etiquetas = new List<EtiquetasResponse>();
        }
    }

    public class DetalleAgendaResponse
    {
        /// <summary>
        /// Código del producto
        /// </summary>
        /// <example>PRO-1</example>
        [ApiDtoExample("PRO-1")]
        [Required]
        public string Producto { get; set; } // CD_PRODUTO

        /// <summary>
        /// Serie o Lote del producto en caso que lo maneje.
        /// Es un valor opcional y WIS lo manejará en modalidad automática si no viene especificado
        /// </summary>
        /// <example>AAA</example>
        [ApiDtoExample("AAA")]
        public string Identificador { get; set; } // NU_IDENTIFICADOR

        /// <summary>
        /// Cantidad teórica 
        /// </summary>
        /// <example>100</example>
        [ApiDtoExample("100")]
        [Required]
        public decimal CantidadTeorica { get; set; } // QT_AGENDADO_ORIGINAL

        /// <summary>
        /// Cantidad recibida 
        /// </summary>
        /// <example>100</example>
        [ApiDtoExample("100")]
        public decimal? CantidadRecibida { get; set; } // QT_AGENDADO_ORIGINAL

        /// <summary>
        /// Fecha de vencimiento
        /// </summary>
        /// <example>10/10/2022</example>
        [ApiDtoExample("10/10/2022")]
        public string FechaVencimiento { get; set; } // DT_FABRICACAO
    }
}
