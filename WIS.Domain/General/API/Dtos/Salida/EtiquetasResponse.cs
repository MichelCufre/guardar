using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WIS.Common.API.Attributes;

namespace WIS.Domain.General.API.Dtos.Salida
{
    public class EtiquetasResponse
    {
        /// <summary>
        /// Número de etiqueta
        /// </summary>
        /// <example>ET001</example>
        [ApiDtoExample("ET001")]
        [Required]
        public string NumeroEtiqueta { get; set; } // NU_ETIQUETA

        /// <summary>
        /// Número de agenda asociado
        /// </summary>
        /// <example>100</example>
        [ApiDtoExample("100")]
        public int? NumeroAgenda { get; set; } // NU_AGENDA

        /// <summary>
        /// Código del endereco (dirección/ubicación)
        /// </summary>
        /// <example>A-01-01</example>
        [ApiDtoExample("A-01-01")]
        public string CodigoEndereco { get; set; } // CD_ENDERECO

        /// <summary>
        /// Código del endereco sugerido
        /// </summary>
        /// <example>A-01-02</example>
        [ApiDtoExample("A-01-02")]
        public string CodigoEnderecoSugerido { get; set; } // CD_ENDERECO_SUGERIDO

        /// <summary>
        /// Código de situacao (situación/estado)
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        public int? CodigoSituacao { get; set; } // CD_SITUACAO

        /// <summary>
        /// Código de función de recepción
        /// </summary>
        /// <example>10</example>
        [ApiDtoExample("10")]
        public int? CodigoFuncRecepcion { get; set; } // CD_FUNC_RECEPCION

        /// <summary>
        /// Fecha de recepción
        /// </summary>
        /// <example>2024-01-15</example>
        [ApiDtoExample("2024-01-15")]
        public DateTime? FechaRecepcion { get; set; } // DT_RECEPCION

        /// <summary>
        /// Código de función de almacenamiento
        /// </summary>
        /// <example>20</example>
        [ApiDtoExample("20")]
        public int? CodigoFuncAlmacenamiento { get; set; } // CD_FUNC_ALMACENAMIENTO

        /// <summary>
        /// Fecha de almacenamiento
        /// </summary>
        /// <example>2024-01-16</example>
        [ApiDtoExample("2024-01-16")]
        public DateTime? FechaAlmacenamiento { get; set; } // DT_ALMACENAMIENTO

        /// <summary>
        /// Código del cliente
        /// </summary>
        /// <example>CLI001</example>
        [ApiDtoExample("CLI001")]
        public string CodigoCliente { get; set; } // CD_CLIENTE

        /// <summary>
        /// Código del grupo
        /// </summary>
        /// <example>GRP001</example>
        [ApiDtoExample("GRP001")]
        public string CodigoGrupo { get; set; } // CD_GRUPO

        /// <summary>
        /// Código del pallet
        /// </summary>
        /// <example>PALLET001</example>
        [ApiDtoExample("PALLET001")]
        public int? CodigoPallet { get; set; } // CD_PALLET

        /// <summary>
        /// Código de barras
        /// </summary>
        /// <example>7891234567890</example>
        [ApiDtoExample("7891234567890")]
        public string CodigoBarras { get; set; } // CD_BARRAS


        /// <summary>
        /// Detalles de la etiqueta
        /// </summary>
        public List<EtiquetasDetalleResponse> Detalles { get; set; }

        public EtiquetasResponse()
        {
            Detalles = new List<EtiquetasDetalleResponse>();
        }
    }

    public class EtiquetasDetalleResponse
    {
        /// <summary>
        /// Número de etiqueta lote
        /// </summary>
        /// <example>ET001</example>
        [ApiDtoExample("ET001")]
        [Required]
        public string NumeroEtiquetaLote { get; set; } // NU_ETIQUETA_LOTE

        /// <summary>
        /// Código del producto
        /// </summary>
        /// <example>PRO-001</example>
        [ApiDtoExample("PRO-001")]
        public string CodigoProducto { get; set; } // CD_PRODUTO

        /// <summary>
        /// Código de faixa (banda/franja)
        /// </summary>
        /// <example>5</example>
        [ApiDtoExample("5")]
        public int? CodigoFaixa { get; set; } // CD_FAIXA

        /// <summary>
        /// Código de empresa
        /// </summary>
        /// <example>10</example>
        [ApiDtoExample("10")]
        public decimal? CodigoEmpresa { get; set; } // CD_EMPRESA

        /// <summary>
        /// Número identificador
        /// </summary>
        /// <example>ID001</example>
        [ApiDtoExample("ID001")]
        public string NumeroIdentificador { get; set; } // NU_IDENTIFICADOR

        /// <summary>
        /// Cantidad de producto recibido
        /// </summary>
        /// <example>100</example>
        [ApiDtoExample("100")]
        public decimal? CantidadProductoRecibido { get; set; } // QT_PRODUCTO_RECIBIDO

        /// <summary>
        /// Cantidad de producto
        /// </summary>
        /// <example>100</example>
        [ApiDtoExample("100")]
        public decimal? CantidadProducto { get; set; } // QT_PRODUTO

        /// <summary>
        /// Cantidad de ajuste recibido
        /// </summary>
        /// <example>5</example>
        [ApiDtoExample("5")]
        public decimal? CantidadAjusteRecibido { get; set; } // QT_AJUSTE_RECIBIDO

        /// <summary>
        /// Cantidad de etiqueta generada
        /// </summary>
        /// <example>10</example>
        [ApiDtoExample("10")]
        public decimal? CantidadEtiquetaGenerada { get; set; } // QT_ETIQUETA_GENERADA

        /// <summary>
        /// Cantidad almacenado
        /// </summary>
        /// <example>95</example>
        [ApiDtoExample("95")]
        public decimal? CantidadAlmacenado { get; set; } // QT_ALMACENADO

        /// <summary>
        /// Fecha de fabricación
        /// </summary>
        /// <example>2024-01-01</example>
        [ApiDtoExample("2024-01-01")]
        public DateTime? FechaFabricacion { get; set; } // DT_FABRICACAO
    }
}
