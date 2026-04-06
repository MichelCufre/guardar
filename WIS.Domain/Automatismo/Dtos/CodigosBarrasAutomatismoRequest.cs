using System.Collections.Generic;
using System.Text.Json.Serialization;
using WIS.Domain.General.Filters;
using WIS.Domain.Validation;

namespace WIS.Domain.Automatismo.Dtos
{
    public class CodigosBarrasAutomatismoRequest : IApiAutomatismoRequest
    {
        /// <summary>
        /// Código de empresa de la ejecución
        /// </summary>
        /// <example>1</example>
        public int Empresa { get; set; }

        /// <summary>
        /// Sirve para generar una referencia o un campo de búsqueda en el panel de ejecuciones de interfaces de WIS. Mediante este campo es posible identificar o buscar la traza de la ejecución de la interfaz. Ante un incidente o un procesamiento no esperado se puede reportar el problema haciendo referencia al valor de este campo.
        /// </summary>
        /// <example>Creación de códigos de barras</example>
        public string DsReferencia { get; set; }

        /// <summary>
        /// Generalmente se utiliza en sistemas externos que generan archivos y un middleware que oficia de intermediario entre el archivo y el API. 
        /// En este campo se puede guardar el nombre del archivo original en el cual vinieron los datos. En caso de que la implantación no utilice archivos se puede utilizar con otros fines.
        /// </summary>
        /// <example>Archivo</example>
        public string Archivo { get; set; }

        /// <summary>
        /// Valor serializado. 
        /// Reservado para datos adicionales.
        /// Debe tener una estructura JSON válida.
        /// </summary>
        /// <example></example>
        public string Serializado { get; set; }

        /// <summary>
        /// Lista de códigos de barras
        /// </summary>
        public List<CodigoBarraAutomatismoRequest> CodigosDeBarras { get; set; }

        public CodigosBarrasAutomatismoRequest()
        {
            CodigosDeBarras = new List<CodigoBarraAutomatismoRequest>();
        }

        public void Add(IApiAutomatismoItemRequest item)
        {
            CodigosDeBarras.Add(item as CodigoBarraAutomatismoRequest);
        }
    }

    public class CodigoBarraAutomatismoRequest : IApiAutomatismoItemRequest
    {
        public string Codigo { get; set; }

        public string Producto { get; set; }

        public string TipoOperacion { get; set; }

        [SwaggerIgnore]
        public string Predio { get; set; }

        [JsonIgnore]
        public string Zona { get; set; }

        [StringLengthValidation(4000, MinimumLength = 0)]
        public string Serializado { get; set; }
    }
}
