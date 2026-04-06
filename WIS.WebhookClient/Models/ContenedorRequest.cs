using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WIS.Common.API.Attributes;

namespace WIS.WebhookClient.Models
{
    public class ContenedorRequest
    {
        /// <summary>
        /// Número de preparación
        /// </summary>
        /// <example>1111</example>
        [ApiDtoExample("1111")]
        [Required]
        public int Preparacion { get; set; } // NU_PREPARACION

        /// <summary>
        /// Número de contenedor
        /// </summary>
        /// <example>2000</example>
        [ApiDtoExample("2000")]
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
        /// </summary>
        /// <example>W</example>
        [ApiDtoExample("W")]
        public string TipoContenedor { get; set; } // TP_CONTENEDOR

        /// <summary>
        /// Código de sub clase
        /// La sub-clase identifica y restringe la compatilidad con los productos al realizar un pickeo.
        /// </summary>
        /// <example></example>
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
        [ApiDtoExample("10,5")]
        public decimal? Altura { get; set; } // VL_ALTURA

        /// <summary>
        /// Largo
        /// </summary>
        /// <example></example>
        [ApiDtoExample("10,5")]
        public decimal? Largo { get; set; } // VL_LARGURA

        /// <summary>
        /// Volumen
        /// </summary>
        /// <example></example>
        [ApiDtoExample("10,5")]
        public decimal? Volumen { get; set; } // VL_CUBAGEM

        /// <summary>
        /// Profundidad
        /// </summary>
        /// <example></example>
        [ApiDtoExample("10,5")]
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
        [ApiDtoExample("10")]
        public int? CantidadBultos { get; set; } // QT_BULTO

        /// <summary>
        /// Descripción del contenedor
        /// </summary>
        /// <example></example>
        public string DescripcionContenedor { get; set; } // DS_CONTENEDOR

        /// <summary>
        /// Id. Precinto 1
        /// </summary>
        /// <example></example>
        [ApiDtoExample("1")]
        public string Precinto1 { get; set; } // ID_PRECINTO_1

        /// <summary>
        /// Id. Precinto 2
        /// </summary>
        /// <example></example>
        [ApiDtoExample("1")]
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

        public List<ContenedorDetalleRequest> Detalles { get; set; }

        public ContenedorRequest() 
        {
            Detalles = new List<ContenedorDetalleRequest>();
        }   
    }
}
