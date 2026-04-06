using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WIS.Common.API.Attributes;
using WIS.Domain.Validation;

namespace WIS.Domain.General.API.Dtos.Salida
{
    public class FacturaResponse
    {
        /// <summary>
        ///  Identificador interno de la factura 
        /// </summary>
        /// <example>1999</example>
        public string Id { get; set; } // NU_RECEPCION_FACTURA

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
        public string Origen { get; set; } // ID_ORIGEN
        /// <summary>
        /// Codigo cliente factura
        /// </summary>
        /// <example></example>
        public string CodigoCliente { get; set; } // CD_CLIENTE
        /// <summary>
        /// Codigo cliente factura
        /// </summary>
        /// <example></example>
        public string CodigoEmpresa { get; set; } // CD_EMPRESA
        /// <summary>
        /// Codigo moneda
        /// </summary>
        /// <example></example>
        public string CodigoMoneda { get; set; } // CD_MONEDA
        /// <summary>
        /// Codigo situacion factura
        /// </summary>
        /// <example></example>
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
        public string FechaCreacion { get; set; } // DT_ADDROW
        /// <summary>
        /// Fecha vencimiento factura
        /// </summary>
        /// <example></example>
        public string FechaVencimiento { get; set; } // DT_VENCIMIENTO
        /// <summary>
        /// Estado factura
        /// </summary>
        /// <example></example>
        public string Estado { get; set; } // ND_ESTADO       

        /// <summary>
        /// Numero Orden Compra factura
        /// </summary>
        /// <example></example>
        public string NumeroOrdenCompra { get; set; } // NU_ORDEN_COMPRA
        /// <summary>
        /// Predio factura
        /// </summary>
        /// <example></example>
        public string Predio { get; set; } // NU_PREDIO
        /// <summary>
        /// Numero remito
        /// </summary>
        /// <example></example>
        public string NumeroRemito { get; set; } // NU_REMITO
        /// <summary>
        /// Importe IVA Base
        /// </summary>
        /// <example></example>
        public decimal? ImporteIvaBase { get; set; } // IM_IVA_BASE
        /// <summary>
        /// Importe IVA Minimo
        /// </summary>
        /// <example></example>
        public decimal? ImporteIvaMinimo { get; set; } // IM_IVA_MINIMO
        /// <summary>
        /// Agenda
        /// </summary>
        /// <example></example>
        public string Agenda { get; set; } // NU_AGENDA

        public List<FacturaDetalleResponse> Detalles { get; set; }

        public FacturaResponse()
        {
            Detalles = new List<FacturaDetalleResponse>();
        }
    }

    public class FacturaDetalleResponse
    {
        /// <summary>
        /// Código del producto
        /// </summary>
        /// <example>PRO-1</example>
        [ApiDtoExample("PRO-1")]
        public string Producto { get; set; } // CD_PRODUTO

        /// <summary>
        /// Serie o Lote del producto en caso que lo maneje.
        /// Es un valor opcional y WIS lo manejará en modalidad automática si no viene especificado
        /// </summary>
        /// <example>AAA</example>
        [ApiDtoExample("AAA")]
        public string Identificador { get; set; } // NU_IDENTIFICADOR

        /// <summary>
        /// Cantidad facturada
        /// </summary>
        /// <example>100</example>
        [ApiDtoExample("100")]
        public decimal CantidadFacturada { get; set; } // QT_FACTURADA

        /// <summary>
        /// Cantidad validada
        /// </summary>
        /// <example>100</example>
        [ApiDtoExample("100")]
        public decimal CantidadValidada { get; set; } // QT_VALIDADA

        /// <summary>
        /// Cantidad recibida
        /// </summary>
        /// <example>100</example>
        [ApiDtoExample("100")]
        public decimal CantidadRecibida { get; set; } // QT_RECIBIDA

        /// <summary>
        /// Fecha de vencimiento
        /// </summary>
        /// <example>12/06/2016 13:19</example>
        [ApiDtoExample("12/06/2016 13:19")]
        public string FechaVencimiento { get; set; } // DT_VENCIMIENTO
        /// <summary>
        /// Importe unitario
        /// </summary>
        /// <example>100</example>
        [ApiDtoExample("100")]
        public string ImporteUnitario { get; set; } // IM_UNITARIO_DIGITADO
        /// <summary>
        /// Fecha de creacion
        /// </summary>
        /// <example>12/06/2016 13:19</example>
        [ApiDtoExample("12/06/2016 13:19")]
        public string FechaCreacion { get; set; } // DT_ADDROW

        /// <summary>
        /// Informacion adicional 1
        /// </summary>
        /// <example>Anexo 1</example>
        public string Anexo1 { get; set; } // DS_ANEXO1
        /// <summary>
        /// Informacion adicional 2
        /// </summary>
        /// <example>Anexo 2</example>
        public string Anexo2 { get; set; } // DS_ANEXO2
        /// <summary>
        /// Informacion adicional 3
        /// </summary>
        /// <example>Anexo 3</example>
        public string Anexo3 { get; set; } // DS_ANEXO3

        /// <summary>
        /// Informacion adicional 4
        /// </summary>
        /// <example>Anexo 4</example>
        public string Anexo4 { get; set; } // DS_ANEXO4

        public List<ReferenciaDetalleFacturaResponse> Referencias { get; set; }

        public FacturaDetalleResponse()
        {
            Referencias = new List<ReferenciaDetalleFacturaResponse>();
        }
    }

    public class ReferenciaDetalleFacturaResponse
    {

        /// <summary>
        ///  Identificador de la referencia para recepción (número de OC, de devolución, etc.)
        /// </summary>
        /// <example>1999</example>
        public string NumeroReferencia { get; set; }

        /// <summary>
        /// Tipo de referencia
        /// OC (Orden de Compra) - RR (Referencia de Recepción) - OD (Orden de Devolución) - ODC (Devolución Canje)
        /// OC y RR corresponden al tipo de agente PRO
        /// OD y ODC corresponden al tipo de agente CLI
        /// </summary>
        /// <example>OD</example>
        public string TipoReferencia { get; set; }

        /// <summary>
        /// Cantidad referencia
        /// </summary>
        /// <example>100</example>
        public decimal CantidadReferencia { get; set; }

        /// <summary>
        /// Cantidad recibida
        /// </summary>
        /// <example>100</example>
        public decimal CantidadRecibida { get; set; }
    }
}
