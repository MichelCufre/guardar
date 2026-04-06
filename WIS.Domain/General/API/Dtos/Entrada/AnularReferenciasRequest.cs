using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WIS.Common.API.Attributes;
using WIS.Domain.Validation;

namespace WIS.Domain.General.API.Dtos.Entrada
{
    public class AnularReferenciasRequest : IApiEntradaRequest
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
        /// <example>Anulación de referencias</example>
        [ApiDtoExample("Anulación de referencias")]
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
        public List<AnularReferenciaRequest> Referencias { get; set; }

        public AnularReferenciasRequest()
        {
            Referencias = new List<AnularReferenciaRequest>();
        }

        public void Add(IApiEntradaItemRequest item)
        {
            Referencias.Add(item as AnularReferenciaRequest);
        }
    }

    public class AnularReferenciaRequest : IApiEntradaItemRequest
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
        /// OC (Orden de Compra) - RR (Referencia de Recepción) - OD (Orden de Devolución)
        /// OC y RR corresponden al tipo de agente PRO
        /// OD corresponde al tipo de agente CLI
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
    }
}
