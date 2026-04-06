using System.Collections.Generic;
using System.Text.Json.Serialization;
using WIS.Domain.Validation;

namespace WIS.Domain.Automatismo.Dtos
{
    public class UbicacionesPickingAutomatismoRequest : IApiAutomatismoRequest
    {
        /// <summary>
        /// Código de empresa de la ejecución
        /// </summary>
        /// <example>1</example>
        [RequiredValidation]
        [ExisteEmpresaValidation()]
        public int Empresa { get; set; }

        /// <summary>
        /// Sirve para generar una referencia o un campo de búsqueda en el panel de ejecuciones de interfaces de WIS. Mediante este campo es posible identificar o buscar la traza de la ejecución de la interfaz. Ante un incidente o un procesamiento no esperado se puede reportar el problema haciendo referencia al valor de este campo.
        /// </summary>
        /// <example>Creación de códigos de barras</example>
        [RequiredValidation]
        [StringLengthValidation(200, MinimumLength = 1)]
        public string DsReferencia { get; set; }

        /// <summary>
        /// Generalmente se utiliza en sistemas externos que generan archivos y un middleware que oficia de intermediario entre el archivo y el API. 
        /// En este campo se puede guardar el nombre del archivo original en el cual vinieron los datos. En caso de que la implantación no utilice archivos se puede utilizar con otros fines.
        /// </summary>
        /// <example>Archivo</example>
        [StringLengthValidation(100)]
        public string Archivo { get; set; }


        /// <summary>
        /// Valor serializado. 
        /// Reservado para datos adicionales.
        /// Debe tener una estructura JSON válida.
        /// </summary>
        /// <example></example>
        [StringLengthValidation(4000, MinimumLength = 0)]
        public string Serializado { get; set; }

        /// <summary>
        /// Lista de ubicaciones de picking
        /// </summary>
        [RequiredListValidation]
        public List<UbicacionPickingAutomatismoRequest> Ubicaciones { get; set; }

        public UbicacionesPickingAutomatismoRequest()
        {
            Ubicaciones = new List<UbicacionPickingAutomatismoRequest>();
        }

        public void Add(IApiAutomatismoItemRequest item)
        {
            Ubicaciones.Add(item as UbicacionPickingAutomatismoRequest);
        }
    }

    public class UbicacionPickingAutomatismoRequest : IApiAutomatismoItemRequest
    {
        [JsonIgnore]
        public int Empresa { get; set; }

        public string Ubicacion { get; set; }

        public string Producto { get; set; }

        public string TipoOperacion { get; set; }

        public string Serializado { get; set; }
    }
}
