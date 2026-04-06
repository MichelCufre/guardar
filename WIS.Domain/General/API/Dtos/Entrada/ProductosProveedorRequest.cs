using System.Collections.Generic;
using WIS.Common.API.Attributes;
using WIS.Domain.Validation;

namespace WIS.Domain.General.API.Dtos.Entrada
{
    public class ProductosProveedorRequest : IApiEntradaRequest
    {
        /// <summary>
        /// Código de empresa de la ejecución
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RequiredValidation]
        [ExisteEmpresaValidation()]
        public int Empresa { get; set; }

        /// <summary>
        /// Sirve para generar una referencia o un campo de búsqueda en el panel de ejecuciones de interfaces de WIS. Mediante este campo es posible identificar o buscar la traza de la ejecución de la interfaz. Ante un incidente o un procesamiento no esperado se puede reportar el problema haciendo referencia al valor de este campo.
        /// </summary>
        /// <example>Creación de productos proveedor</example>
        [ApiDtoExample("Creación de productos proveedor")]
        [RequiredValidation]
        [StringLengthValidation(200, MinimumLength = 1)]
        public string DsReferencia { get; set; }

        /// <summary>
        /// Generalmente se utiliza en sistemas externos que generan archivos y un middleware que oficia de intermediario entre el archivo y el API. 
        /// En este campo se puede guardar el nombre del archivo original en el cual vinieron los datos. En caso de que la implantación no utilice archivos se puede utilizar con otros fines.
        /// </summary>
        /// <example>Archivo</example>
        [ApiDtoExample("Archivo")]
        [StringLengthValidation(100)]
        public string Archivo { get; set; }

		/// <summary>
		/// Sirve para controlar la unicidad de las ejecuciones
		/// </summary>
		/// <example>123</example>
		[ApiDtoExample("123")]
		[StringLengthValidation(50, MinimumLength = 0)]
		public string IdRequest { get; set; }

		/// <summary>
		/// Lista de productos
		/// </summary>
		[RequiredListValidation]
        public List<ProductoProveedorRequest> Productos { get; set; }

        public ProductosProveedorRequest()
        {
            Productos = new List<ProductoProveedorRequest>();
        }

        public void Add(IApiEntradaItemRequest item)
        {
            Productos.Add(item as ProductoProveedorRequest);
        }
    }

    public class ProductoProveedorRequest : IApiEntradaItemRequest
    {
        /// <summary>
        /// Código de producto
        /// </summary>
        /// <example>PR1</example>
        [ApiDtoExample("PR1")]
        [RequiredValidation]
        [StringLengthValidation(40, MinimumLength = 1)]
        public string CodigoProducto { get; set; }

        /// <summary>
        /// Referencia o código del producto para el proveedor.
        /// Es el código de producto con el cual generalmente viene especificada la factura del proveedor.
        /// </summary>
        /// <example>PR1</example>
        [ApiDtoExample("PR1")]
        [RequiredValidation]
        [StringLengthValidation(30, MinimumLength = 1)]
        public string CodigoExterno { get; set; }

        /// <summary>
        /// Código del agente al cual pertenecen las referencias.
        /// En WIS pueden existir n-proveedores que traen un mismo producto y cada proveedor puede tener sus referencias o codificaciones sobre el mismo producto.
        /// </summary>
        /// <example>AGE01</example>
        [ApiDtoExample("AGE01")]
        [RequiredValidation]
        [StringLengthValidation(40, MinimumLength = 1)]
        public string CodigoAgente { get; set; }

        /// <summary>
        /// Tipo de agente 
        /// PRO (Proveedor) - CLI (Cliente)
        /// </summary>
        /// <example>PRO</example>
        [ApiDtoExample("PRO")]
        [RequiredValidation]
        [StringLengthValidation(3, MinimumLength = 1)]
        public string TipoAgente { get; set; }

        /// <summary>
        /// Tipo de Operación
        /// A (Alta) - B (Baja)
        /// </summary>
        /// <example>A</example>
        [ApiDtoExample("A")]
        [StringLengthValidation(1, MinimumLength = 0)]
        public string TipoOperacion { get; set; }
    }
}
