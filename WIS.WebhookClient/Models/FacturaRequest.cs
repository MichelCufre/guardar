using System.Collections.Generic;
using WIS.Common.API.Attributes;

namespace WIS.WebhookClient.Models
{
    public class FacturaRequest
    {
        /// <summary>
        /// Número de serie
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        public string Serie { get; set; } // NU_SERIE

        /// <summary>
        /// Número de factura
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        public string Factura { get; set; } // NU_FACTURA

        /// <summary>
        /// Tipo de Factura
        /// </summary>
        /// <example></example>
        [ApiDtoExample("1")]
        public string TipoFactura { get; set; } // TP_FACTURA

        /// <summary>
        /// Fecha de emisión de la factura
        /// </summary>
        /// <example>12/06/2016 13:19</example>
        [ApiDtoExample("12/06/2016 13:19")]
        public string FechaEmision { get; set; } // DT_EMISION

        /// <summary>
        /// Total digitado
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        public decimal? TotalDigitado { get; set; } // IM_TOTAL_DIGITADO

        /// <summary>
        /// Identificador de origen
        /// </summary>
        /// <example></example>
        [ApiDtoExample("3")]
        public string Origen { get; set; } // ID_ORIGEN

        /// <summary>
        /// Codigo cliente factura
        /// </summary>
        /// <example></example>
        [ApiDtoExample("CLI01")]
        public string CodigoCliente { get; set; } // CD_CLIENTE
        /// <summary>
        /// Codigo cliente factura
        /// </summary>
        /// <example></example>
        [ApiDtoExample("1")]
        public string CodigoEmpresa { get; set; } // CD_EMPRESA
        /// <summary>
        /// Codigo moneda
        /// </summary>
        /// <example></example>
        [ApiDtoExample("2")]
        public string CodigoMoneda { get; set; } // CD_MONEDA
        /// <summary>
        /// Codigo situacion factura
        /// </summary>
        /// <example></example>
        [ApiDtoExample("4")]
        public string CodigoSituacion { get; set; } // CD_SITUACAO
        /// <summary>
        /// Informacion adicional 1
        /// </summary>
        /// <example></example>
        public string Anexo1 { get; set; } // DS_ANEXO1
        /// <summary>
        /// Informacion adicional 2
        /// </summary>
        /// <example></example>
        public string Anexo2 { get; set; } // DS_ANEXO2
        /// <summary>
        /// Informacion adicional 3
        /// </summary>
        /// <example></example>
        public string Anexo3 { get; set; } // DS_ANEXO3
        /// <summary>
        /// Observacion
        /// </summary>
        /// <example></example>
        public string Observacion { get; set; } // DS_OBSERVACION
        /// <summary>
        /// Fecha creacion factura
        /// </summary>
        /// <example></example>
        [ApiDtoExample("12/06/2016 13:19")]
        public string FechaCreacion { get; set; } // DT_ADDROW
        /// <summary>
        /// Fecha vencimiento factura
        /// </summary>
        /// <example></example>
        [ApiDtoExample("12/06/2016 13:19")]
        public string FechaVencimiento { get; set; } // DT_VENCIMIENTO
        /// <summary>
        /// Estado factura
        /// </summary>
        /// <example></example>
        [ApiDtoExample("1")]
        public string Estado { get; set; } // ND_ESTADO       

        /// <summary>
        /// Numero Orden Compra factura
        /// </summary>
        /// <example></example>
        [ApiDtoExample("1")]
        public string NumeroOrdenCompra { get; set; } // NU_ORDEN_COMPRA
        /// <summary>
        /// Predio factura
        /// </summary>
        /// <example></example>
        [ApiDtoExample("1")]
        public string Predio { get; set; } // NU_PREDIO
        /// <summary>
        /// Numero remito
        /// </summary>
        /// <example></example>
        [ApiDtoExample("1")]
        public string NumeroRemito { get; set; } // NU_REMITO

        public List<FacturaDetalleRequest> Detalles { get; set; }

        public FacturaRequest()
        {
            Detalles = new List<FacturaDetalleRequest>();
        }
    }
}
