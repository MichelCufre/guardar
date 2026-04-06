using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Common.API.Attributes;
using WIS.Domain.Validation;

namespace WIS.Domain.General.API.Dtos.Entrada
{
    public class CodigosBarrasRequest : IApiEntradaRequest
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
        /// <example>Creación de códigos de barras</example>
        [ApiDtoExample("Creación de códigos de barras")]
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
		/// Lista de códigos de barras
		/// </summary>
		[RequiredListValidation]
        public List<CodigoBarraRequest> CodigosDeBarras { get; set; }

        public CodigosBarrasRequest()
        {
            CodigosDeBarras = new List<CodigoBarraRequest>();
        }

        public void Add(IApiEntradaItemRequest item)
        {
            CodigosDeBarras.Add(item as CodigoBarraRequest);
        }
    }

    public class CodigoBarraRequest : IApiEntradaItemRequest
    {
        /// <summary>
        /// Código de barra
        /// </summary>
        /// <example>CB1</example>
        [ApiDtoExample("CB1")]
        [RequiredValidation]
        [StringLengthValidation(50, MinimumLength = 1)]
        public string Codigo { get; set; }

        /// <summary>
        /// Código de producto
        /// </summary>
        /// <example>PR1</example>
        [ApiDtoExample("PR1")]
        [RequiredValidation]
        [StringLengthValidation(40, MinimumLength = 1)]
        public string Producto { get; set; }

        /// <summary>
        /// Tipo del código de barras
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RangeValidation(8, 0, false)]
        public int? TipoCodigo { get; set; }

        /// <summary>
        /// Prioridad de uso si hay que reimprimir códigos de barras
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RangeValidation(2, 0, false)]
        public short? PrioridadUso { get; set; }

        /// <summary>
        /// Cantidad de unidades asociadas al código de barras.
        /// Lo normal es establecer este valor en 1 y no automatizar las cantidades por el código de barras
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RangeValidation(10, 2, false)]
        public decimal? CantidadEmbalaje { get; set; }

        /// <summary>
        /// Tipo de operación
        /// A (Alta) - S (Sobrescritura) - B (Baja)
        /// </summary>
        /// <example>A</example>
        [ApiDtoExample("A")]
        [RequiredValidation]
        [StringLengthValidation(1)]
        public string TipoOperacion { get; set; }

    }
}
