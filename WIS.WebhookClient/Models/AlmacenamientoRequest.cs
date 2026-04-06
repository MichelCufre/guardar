using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WIS.WebhookClient.Models
{
    public class AlmacenamientoRequest
    {
        /// <summary>
        /// Número de agenda
        /// </summary>
        /// <example>100</example>
        public int? Agenda { get; set; }

        /// <summary>
        /// Tipo de agente 
        /// PRO (Proveedor) - CLI (Cliente)
        /// </summary>
        /// <example>PRO</example>
        public string TipoAgente { get; set; }

        /// <summary>
        /// Código del agente
        /// </summary>
        /// <example>PRO</example>
        public string CodigoAgente { get; set; }

        /// <summary>
        /// Numero de etiqueta que se almacenó
        /// </summary>
        /// <example>22</example>
        public string Etiqueta { get; set; }

        /// <summary>
        /// Tipo de etiqeuta
        /// </summary>
        /// <example>WE</example>
        public string TipoEtiqueta { get; set; }

        /// <summary>
        /// Serializado
        /// En este campo se puede enviar información serializada que sea útil para personalizar el funcionamiento.
        /// </summary>
        /// <example></example>
        public string Serializado { get; set; }

        public List<DetalleAlmacenamientoRequest> Detalles { get; set; }

        public AlmacenamientoRequest()
        {
            Detalles = new List<DetalleAlmacenamientoRequest>();
        }
    }

    public class DetalleAlmacenamientoRequest
    {
        /// <summary>
        /// Código de la empresa
        /// </summary>
        /// <example>1</example>
        public int? Empresa { get; set; }

        /// <summary>
        /// Código del producto
        /// </summary>
        /// <example>PROD-01</example>    
        public string Producto { get; set; }

        /// <summary>
        /// Código de embalaje
        /// </summary>
        /// <example>1</example>
        public decimal? Faixa { get; set; }

        /// <summary>
        /// Identificador del producto (serie o lote)
        /// </summary>
        /// <example>*</example>  
        public string Identificador { get; set; }

        /// <summary>
        /// Fecha de vencimiento del producto
        /// </summary>
        /// <example>1</example>
        public string Vencimiento { get; set; }

        /// <summary>
        /// Cantidad almacenada
        /// </summary>
        /// <example>1</example>
        public decimal? CantidadAlmacenada { get; set; }

        /// <summary>
        /// Cantidad disponible en la etiqueta post-almacenado para el producto/lote
        /// </summary>
        /// <example>1</example>
        public decimal? CantidadDisponible { get; set; }

        /// <summary>
        /// Serializado
        /// En este campo se puede enviar información serializada que sea útil para personalizar el funcionamiento.
        /// </summary>
        /// <example></example>
        public string Serializado { get; set; }

        /// <summary>
        /// Ubicación donde se almacenó el producto
        /// </summary>
        /// <example>1AA00000</example>
        public string Ubicacion { get; set; }

        /// <summary>
        /// Fecha de almacenamiento
        /// </summary>
        /// <example>10/10/2023</example>
        public string FechaOperacion { get; set; }

        /// <summary>
        /// Código del funcionario
        /// </summary>
        /// <example>0</example>
        public int? Funcionario { get; set; }
    }
}
