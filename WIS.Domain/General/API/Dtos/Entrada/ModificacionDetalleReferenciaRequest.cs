using System.Collections.Generic;
using WIS.Common.API.Attributes;
using WIS.Domain.Validation;

namespace WIS.Domain.General.API.Dtos.Entrada
{
    public class ModificacionDetalleReferenciaRequest : IApiEntradaRequest
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
        /// <example>Modificación de detalle de referencias</example>
        [ApiDtoExample("Modificación de detalle de referencias")]
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
		/// Lista de agentes
		/// </summary>
		[RequiredListValidation]
        public List<ReferenciaModificacionRequest> Referencias { get; set; }

        public ModificacionDetalleReferenciaRequest()
        {
            Referencias = new List<ReferenciaModificacionRequest>();
        }

        public void Add(IApiEntradaItemRequest item)
        {
            Referencias.Add(item as ReferenciaModificacionRequest);
        }
    }

    public class ReferenciaModificacionRequest : IApiEntradaItemRequest
    {
        /// <summary>
        /// Número de referencia
        /// </summary>
        /// <example>REF01</example>
        [ApiDtoExample("REF01")]
        [RequiredValidation]
        [StringLengthValidation(20, MinimumLength = 1)]
        public string Referencia { get; set; }

        /// <summary>
        /// Tipo de referencia
        /// </summary>
        /// <example>OC</example>
        [ApiDtoExample("OC")]
        [RequiredValidation]
        [StringLengthValidation(6, MinimumLength = 1)]
        public string TipoReferencia { get; set; }

        /// <summary>
        /// Código de agente
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

        [RequiredListValidation(true)]
        public List<DetalleModificacionRequest> Detalles { get; set; }

        public ReferenciaModificacionRequest()
        {
            Detalles = new List<DetalleModificacionRequest>();
        }
    }

    public class DetalleModificacionRequest
    {
        /// <summary>
        /// Id Línea Sistema Externo
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RequiredValidation]
        [StringLengthValidation(40, MinimumLength = 1)]
        public string IdLineaSistemaExterno { get; set; }

        /// <summary>
        /// Código de producto
        /// </summary>
        /// <example>PR1</example>
        [ApiDtoExample("PR1")]
        [RequiredValidation]
        [StringLengthValidation(40, MinimumLength = 1)]
        public string CodigoProducto { get; set; }

        /// <summary>
        /// Identificador
        /// </summary>
        /// <example>LOTE1</example>
        [ApiDtoExample("LOTE1")]
        [StringLengthValidation(40, MinimumLength = 0)]
        public string Identificador { get; set; }

        /// <summary>
        /// Cantidad 
        /// </summary>
        /// <example>100</example>
        [ApiDtoExample("100")]
        [RangeValidation(15, 3, false)]
        public decimal CantidadOperacion { get; set; }

        /// <summary>
        /// Tipo de operación
        /// A (Anular) - M (Modificar) - R (Reemplazar)
        /// </summary>
        /// <example>M</example>
        [ApiDtoExample("M")]
        [RequiredValidation]
        [StringLengthValidation(1)]
        public string TipoOperacion { get; set; }

    }
}
