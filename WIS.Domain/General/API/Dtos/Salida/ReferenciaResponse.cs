using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WIS.Common.API.Attributes;

namespace WIS.Domain.General.API.Dtos.Salida
{
    public class ReferenciaResponse
    {
        /// <summary>
        ///  Identificador de la referencia para recepción (número de OC, de devolución, etc.)
        /// </summary>
        /// <example>1999</example>
        [ApiDtoExample("1999")]
        [Required]
        public string NumeroReferencia { get; set; } // NU_REFERENCIA

        /// <summary>
        /// Tipo de referencia
        /// OC (Orden de Compra) - RR (Referencia de Recepción) - OD (Orden de Devolución) - ODC (Devolución Canje)
        /// OC y RR corresponden al tipo de agente PRO
        /// OD y ODC corresponden al tipo de agente CLI
        /// </summary>
        /// <example>OD</example>
        [ApiDtoExample("OD")]
        [Required]
        public string TipoReferencia { get; set; }  // TP_REFERENCIA

        /// <summary>
        /// Tipo de agente 
        /// PRO (Proveedor) - CLI (Cliente)
        /// </summary>
        /// <example>CLI</example>
        [ApiDtoExample("CLI")]
        [Required]
        public string TipoAgente { get; set; } // TP_AGENTE

        /// <summary>
        /// Código del agente
        /// </summary>
        /// <example>LOC1111</example>
        [ApiDtoExample("LOC1111")]
        [Required]
        public string CodigoAgente { get; set; } // CD_AGENTE

        /// <summary>
        /// Información adicional devuelta en la confirmación de la recepción
        /// </summary>
        /// <example></example>
        public string Memo { get; set; } // DS_MEMO

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
        /// Serializado
        /// </summary>
        /// <example></example>
        public string Serializado { get; set; } // VL_SERIALIZADO

        /// <summary>
        /// Número de predio
        /// </summary>
        /// <example></example>
        public string Predio { get; set; } // NU_PREDIO

        public List<ReferenciaDetalleResponse> Detalles { get; set; }

        public ReferenciaResponse()
        {
            Detalles = new List<ReferenciaDetalleResponse>();
        }
    }

    public class ReferenciaDetalleResponse
    {
        /// <summary>
        /// Identificación en el ERP o sistema externo para la línea de la referencia.  
        /// Si está definido después en la confirmación, se retorna para un matcheo en el sistema externo.
        /// </summary>
        /// <example>LIN1</example>
        [ApiDtoExample("LIN1")]
        public string IdLineaSistemaExterno { get; set; } // ID_LINEA_SISTEMA_EXTERNO

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
        /// Cantidad de la Referencia
        /// </summary>
        /// <example>100</example>
        [ApiDtoExample("100")]
        [Required]
        public decimal CantidadReferencia { get; set; } // QT_REFERENCIA

        /// <summary>
        /// Cantidad consumida
        /// </summary>
        /// <example>100</example>
        [ApiDtoExample("100")]
        public decimal? CantidadConsumida { get; set; } // QT_CONSUMIDA

        /// <summary>
        /// Información adicional
        /// </summary>
        /// <example></example>
        public string Anexo { get; set; } // DS_ANEXO1

        /// <summary>
        /// Cantidad consumida agendada
        /// </summary>
        /// <example></example>
        public decimal? CantidadConsumidaAgenda { get; set; } // QT_CONSUMIDA_AGENDA

        /// <summary>
        /// Fecha de vencimiento
        /// </summary>
        /// <example>10/10/2022</example>
        [ApiDtoExample("10/10/2022")]
        public string FechaVencimiento { get; set; } // DT_VENCIMIENTO

        /// <summary>
        /// Cantidad asignada a una factura
        /// </summary>
        /// <example>5</example>
        [ApiDtoExample("5")]
        public decimal? CantidadAsignadaFactura { get; set; }
    }
}
