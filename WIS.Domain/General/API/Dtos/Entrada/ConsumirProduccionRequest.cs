using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.Office2013.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Common.API.Attributes;
using WIS.Domain.Validation;

namespace WIS.Domain.General.API.Dtos.Entrada
{
    public class ConsumirProduccionRequest : IApiEntradaRequest
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
        /// <example>Consumir producción</example>
        [ApiDtoExample("Consumir producción")]
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
        /// Especifica el código del espacio de producción sobre el cual se realizará el consumo
        /// </summary>
        /// <example>Id 1</example>
        [ApiDtoExample("1")]
        [StringLengthValidation(10, MinimumLength = 0)]
        public string IdEspacio { get; set; }

        /// <summary>
        /// Valor booleano que especifica si se quiere iniciar el ingreso a producción
        /// </summary>
        /// <example>Id false</example>
        [ApiDtoExample("false")]
        public bool IniciarProduccion { get; set; }

        /// <summary>
		/// Lista de insumos a consumir        
		/// </summary>
		public List<InsumosConsumirRequest> Insumos { get; set; }

        public void Add(IApiEntradaItemRequest item)
        {
        }
    }

    public class InsumosConsumirRequest : IApiEntradaItemRequest
    {
        /// <summary>
        /// Código del producto a consumir
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
        /// Referencia del insumo. Si contiene valor, los insumos a consumir se limitan a los que tienen dicho valor
        /// </summary>
        /// <example>Ref 1</example>
        [ApiDtoExample("Ref 1")]
        public string Referencia { get; set; }

        /// <summary>
        /// Cantidad a producir.
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RequiredValidation]
        [RangeValidation(12, 3)]
        public decimal Cantidad { get; set; }

        /// <summary>
        /// Motivo del consumo.
        /// Valores: MOT_CONS_CON (Consumo), MOT_CONS_ADS (Ajuste de Stock)
        /// </summary>
        /// <example>MOT_CONS_CON</example>
        [ApiDtoExample("MOT_CONS_CON")]
        [RequiredValidation]
        [StringLengthValidation(20, MinimumLength = 1)]
        public string Motivo { get; set; }

        /// <summary>
        /// Indica si se debe consumir únicamente de la cantidad reservada del insumo
        /// </summary>
        /// <example>false</example>
        [ApiDtoExample("false")]
        [RequiredValidation]
        public bool UsarSoloReserva { get; set; }
    }
}
