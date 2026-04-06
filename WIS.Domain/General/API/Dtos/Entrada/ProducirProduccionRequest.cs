using DocumentFormat.OpenXml.Presentation;
using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Common.API.Attributes;
using WIS.Domain.Eventos;
using WIS.Domain.Produccion;
using WIS.Domain.Validation;
using WIS.Persistence.Database;

namespace WIS.Domain.General.API.Dtos.Entrada
{
    public class ProducirProduccionRequest : IApiEntradaRequest
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
        /// <example>Producir producción</example>
        [ApiDtoExample("Producir producción")]
        [RequiredValidation]
        [StringLengthValidation(200, MinimumLength = 1)]
        public string DsReferencia { get; set; }

        /// <summary>
        /// Generalmente se utiliza en sistemas externos que generan archivos y un middleware que oficia de intermediario entre el archivo y el API. 
        /// En este campo se puede guardar el nombre del archivo original en el cual vinieron los datos. En caso de que la implantación no utilice archivos se puede utilizar con otros fines.
        /// </summary>
        /// <example>Archivo</example>
        [StringLengthValidation(100)]
        [ApiDtoExample("Archivo")]
        public string Archivo { get; set; }

        /// <summary>
        /// Sirve para controlar la unicidad de las ejecuciones
        /// </summary>
        /// <example>123</example>
        [ApiDtoExample("123")]
        [StringLengthValidation(50, MinimumLength = 0)]
        public string IdRequest { get; set; }

        /// <summary>
        /// Id de la producción
        /// </summary>
        /// <example>Id 1</example>
        [ApiDtoExample("Id 1")]
        [StringLengthValidation(100, MinimumLength = 0)]
        [IdProduccionExternoValidation()]
        public string IdProduccionExterno { get; set; }

        /// <summary>
        /// Indica si el movimiento debe notificarse a sistemas externos
        /// </summary>
        /// <example>false</example>
        [ApiDtoExample("false")]
        [RequiredValidation]
        public bool ConfirmarMovimiento { get; set; }

        /// <summary>
        /// Indica si debe finalizarse la producción
        /// </summary>
        /// <example>false</example>
        [ApiDtoExample("false")]
        [RequiredValidation]
        public bool FinalizarProduccion { get; set; }

        /// <summary>
        /// Especifica el código del espacio de producción sobre el cual se realizará la producción
        /// </summary>
        /// <example>Id 1</example>
        [ApiDtoExample("1")]
        [StringLengthValidation(10, MinimumLength = 0)]
        public string IdEspacio { get; set; }

        /// <summary>
		/// Lista de productos a producir        
		/// </summary>
        [RequiredListValidation]
        public List<ProductosProducirRequest> Productos { get; set; }

        public void Add(IApiEntradaItemRequest item)
        {
			Productos.Add(item as ProductosProducirRequest);
        }
    }

    public class ProductosProducirRequest : IApiEntradaItemRequest
    {

        /// <summary>
        /// Código del producto a producir
        /// </summary>
        /// <example>PR1</example>
        [ApiDtoExample("PR1")]
        [RequiredValidation]
        [StringLengthValidation(40, MinimumLength = 1)]
        public string Producto { get; set; }

        /// <summary>
        /// Lote o número que identifica al producto.
        /// </summary>
        /// <example>LOTE1</example>
        [ApiDtoExample("LOTE1")]
        [StringLengthValidation(40, MinimumLength = 0)]
        public string Identificador { get; set; }

        /// <summary>
        /// Ubicación de la producción.
        /// </summary>
        /// <example>1PRD150-P01</example>
        [ApiDtoExample("1PRD150-P01")]
        public string Ubicacion { get; set; }

        /// <summary>
        /// Vencimiento del producto.
        /// </summary>
        /// <example>2029-10-15</example>
        [ApiDtoExample("2029-10-15")]
        public DateTime? Vencimiento { get; set; }

        /// <summary>
        /// Cantidad a producir.
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RequiredValidation]
        [RangeValidation(12, 3)]
        public decimal Cantidad { get; set; }

        /// <summary>
        /// Motivo de la producción.
        /// Valores: MOT_PROD_PRO (Producción), MOT_PROD_SOB (Sobrante), MOT_PROD_ADS (Ajuste de Stock)
        /// </summary>
        /// <example>MOT_PROD_PRO</example>
        [ApiDtoExample("MOT_PROD_PRO")]
        [RequiredValidation]
        [StringLengthValidation(20, MinimumLength = 1)]
        public string Motivo { get; set; }
    }
}
