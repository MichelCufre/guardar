using System.Collections.Generic;
using System.Text.Json.Serialization;
using WIS.Domain.General.Filters;
using WIS.Domain.Validation;

namespace WIS.Domain.Automatismo.Dtos
{
    public class ProductosAutomatismoRequest : IApiAutomatismoRequest
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
        /// <example>Modificación de productos</example>
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
        /// Lista de productos
        /// </summary>
        [RequiredListValidation]
        public List<ProductoAutomatismoRequest> Productos { get; set; }

        public ProductosAutomatismoRequest()
        {
            Productos = new List<ProductoAutomatismoRequest>();
        }

        public void Add(IApiAutomatismoItemRequest item)
        {
            Productos.Add(item as ProductoAutomatismoRequest);
        }
    }

    public class ProductoAutomatismoRequest : IApiAutomatismoItemRequest
    {
        public string Codigo { get; set; }

        public string Descripcion { get; set; }

        public decimal? PesoBruto { get; set; }

        public decimal? Ancho { get; set; }

        public decimal? Altura { get; set; }

        public decimal? Profundidad { get; set; }

        public string ManejoIdentificador { get; set; }

        public string TipoManejoFecha { get; set; }

        [SwaggerIgnore]
        public string TipoOperacion { get; set; }

        [SwaggerIgnore]
        public string Predio { get; set; }

        [JsonIgnore]
        public string Zona { get; set; }

        [StringLengthValidation(4000, MinimumLength = 0)]
        public string Serializado { get; set; }

        public string ConfirmarCodigoBarras { get; set; }

        public string UnidadCaja { get; set; }

        public int? CantidadUnidadCaja { get; set; }

        public decimal? UnidadBulto { get; set; }
    }
}
