using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WIS.Common.API.Attributes;

namespace WIS.Domain.General.API.Dtos.Salida
{
    public class ContenedorResponse
    {
        /// <summary>
        /// Número que identifica una preparación.
        /// La preparación es una instancia o un secuencial de una ola de pickeo.
        /// </summary>
        /// <example>1111</example>
        [ApiDtoExample("1111")]
        [Required]
        public int Preparacion { get; set; } // NU_PREPARACION

        /// <summary>
        /// Número de contenedor
        /// Identifica el contenedor usado para uno o varios pickeos. 
        /// Este valor puede repetirse para otras preparaciones futuras siempre y cuando el estado del mismo lo permita.
        /// </summary>
        /// <example>20000</example>
        [ApiDtoExample("20000")]
        [Required]
        public int NumeroContenedor { get; set; } // NU_CONTENEDOR

        /// <summary>
        /// Identificador externo del contenedor
        /// </summary>
        /// <example>ID-EXT-01</example>
        [ApiDtoExample("ID-EXT-01")]
        public string IdExternoContenedor { get; set; } // ID_EXTERNO_CONTENEDOR

        /// <summary>
        ///  Tipo de contenedor
        ///  Tipo que identifica a un contenedor y determina una serie de valores y 
        ///  estándares como el rango permitido, el envío de datos a tracking, entre otros.
        /// </summary>
        /// <example>W</example>
        [ApiDtoExample("W")]
        public string TipoContenedor { get; set; } // TP_CONTENEDOR

        /// <summary>
        /// Código de sub clase
        /// La sub-clase identifica y restringe la compatibilidad con los productos al realizar un pickeo.
        /// Posibles valores configurables:
        /// -T: Toxicos
        /// -F: Farmacos 
        /// -A: Alimentos 
        /// </summary>
        /// <example>T</example>
        [ApiDtoExample("T")]
        public string SubClase { get; set; } // CD_SUB_CLASSE

        /// <summary>
        /// Fecha de expedición
        /// </summary>
        /// <example>10/5/2016 14:30</example>
        [ApiDtoExample("10/5/2016 14:30")]
        public string FechaExpedicion { get; set; } // DT_EXPEDIDO

        /// <summary>
        /// Peso real
        /// </summary>
        /// <example></example>
        public decimal? PesoReal { get; set; } // PS_REAL

        /// <summary>
        /// Altura
        /// </summary>
        /// <example></example>
        public decimal? Altura { get; set; } // VL_ALTURA

        /// <summary>
        /// Largo
        /// </summary>
        /// <example></example>
        public decimal? Largo { get; set; } // VL_LARGURA

        /// <summary>
        /// Volumen
        /// </summary>
        /// <example></example>
        public decimal? Volumen { get; set; } // VL_CUBAGEM

        /// <summary>
        /// Profundidad
        /// </summary>
        /// <example></example>
        public decimal? Profundidad { get; set; } // VL_PROFUNDIDADE

        /// <summary>
        /// Código de unidad de bultos 
        /// </summary>
        /// <example></example>
        public string CodigoUnidadBulto { get; set; } // CD_UNIDAD_BULTO

        /// <summary>
        /// Cantidad de bultos 
        /// </summary>
        /// <example></example>
        public int? CantidadBultos { get; set; } // QT_BULTO

        /// <summary>
        /// Descripción del contenedor
        /// </summary>
        /// <example></example>
        public string DescripcionContenedor { get; set; } // DS_CONTENEDOR

        /// <summary>
        /// Información relacionada con el precinto
        /// </summary>
        /// <example></example>
        public string Precinto1 { get; set; } // ID_PRECINTO_1

        /// <summary>
        /// Información relacionada con el precinto
        /// </summary>
        /// <example></example>
        public string Precinto2 { get; set; } // ID_PRECINTO_2

        /// <summary>
        /// Código de barras del contenedor
        /// </summary>
        /// <example></example>
        public string CodigoBarras { get; set; } // CD_BARRAS

        /// <summary>
        /// Número de Lpn asociado al contenedor
        /// </summary>
        /// <example></example>
        [ApiDtoExample("451")]
        public long? NroLpn { get; set; } // NU_LPN

        /// <summary>
        /// La lista de detalles representa el contenido que tiene un contenedor, se detalla información de los productos pickeados junto con los pedidos a los cuales pertenecen.
        /// La suma de estos detalles coinciden con el total del pedido correspondiente del bloque previo
        /// </summary>
        /// <example></example>
        public List<ContenedorDetalleResponse> Detalles { get; set; }

        public ContenedorResponse()
        {
            Detalles = new List<ContenedorDetalleResponse>();
        }
    }

    public class ContenedorDetalleResponse
    {
        /// <summary>
        /// Número de pedido
        /// </summary>
        /// <example>PED1</example>
        [ApiDtoExample("PED1")]
        [Required]
        public string Pedido { get; set; } // NU_PEDIDO

        /// <summary>
        /// Código del agente
        /// </summary>
        /// <example>1555AAA</example>
        [ApiDtoExample("1555AAA")]
        [Required]
        public string CodigoAgente { get; set; } // CD_AGENTE

        /// <summary>
        /// Tipo de agente 
        /// PRO (Proveedor) - CLI (Cliente)
        /// </summary>
        /// <example>CLI</example>
        [ApiDtoExample("CLI")]
        [Required]
        public string TipoAgente { get; set; } // TP_AGENTE

        /// <summary>
        /// Código del producto
        /// </summary>
        /// <example>PR1AAA</example>
        [ApiDtoExample("PR1AAA")]
        [Required]
        public string Producto { get; set; } // CD_PRODUTO

        /// <summary>
        /// Identificador del producto (serie o lote)
        /// </summary>
        /// <example>LOTEAAAAA</example>
        [ApiDtoExample("LOTEAAAAA")]
        public string Identificador { get; set; } // NU_IDENTIFICADOR

        /// <summary>
        /// Fecha de vencimiento del pickeo
        /// </summary>
        /// <example>10/5/2018</example>
        [ApiDtoExample("10/5/2018")]
        public string FechaVencimientoPickeo { get; set; } // DT_VENCIMIENTO_PICKEO

        /// <summary>
        /// Avería pickeo 
        /// S (Sí) - N (No) 
        /// </summary>
        /// <example>N</example>
        [ApiDtoExample("N")]
        public string AveriaPickeo { get; set; } // ID_AVERIA_PICKEO

        /// <summary>
        /// Cantidad preparada 
        /// </summary>
        /// <example>120</example>
        [ApiDtoExample("120")]
        [Required]
        public decimal CantidadPreparada { get; set; } // QT_PREPARADO
    }
}
