using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using WIS.Common.API.Attributes;
using WIS.Domain.General.Filters;
using WIS.Domain.Validation;

namespace WIS.Domain.General.API.Dtos.Entrada
{
    public class AjustesDeStockRequest : IApiEntradaRequest
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
        /// <example>Ajuste Stock</example>
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
		/// Sirve para controlar la unicidad de las ejecuciones
		/// </summary>
		/// <example>123</example>
		[ApiDtoExample("123")]
		[StringLengthValidation(50, MinimumLength = 0)]
		public string IdRequest { get; set; }

		[SwaggerIgnore]
        public UsuarioRequest Usuario { get; set; }

        [RequiredListValidation]
        public List<AjusteStockRequest> Ajustes { get; set; }

        public AjustesDeStockRequest()
        {
            Ajustes = new List<AjusteStockRequest>();
        }

        public void Add(IApiEntradaItemRequest item)
        {
            Ajustes.Add(item as AjusteStockRequest);
        }
    }

    public class AjusteStockRequest : IApiEntradaItemRequest
    {
        /// <summary>
        /// Ubicación de stock
        /// </summary>
        /// <example>1AA00000</example>
        [RequiredValidation]
        [StringLengthValidation(40, MinimumLength = 1)]
        public string Ubicacion { get; set; }

        /// <summary>
        /// Código de producto
        /// </summary>
        /// <example>PR1</example>
        [RequiredValidation]
        [StringLengthValidation(40, MinimumLength = 1)]
        public string Producto { get; set; }

        /// <summary>
        /// Serie o Lote del producto en caso que lo maneje.
        /// Es un valor opcional y el sistema lo manejará en modalidad automática si no viene especificado.
        /// No puede tener espacios en blanco al principio ni al final.
        /// </summary>
        /// <example>LOTE1</example>

        [StringLengthValidation(40, MinimumLength = 0)]
        public string Identificador { get; set; }

        /// <summary>
        /// Cantidad de ajuste
        /// </summary>
        /// <example>100</example>
        [RequiredValidation]
        public decimal Cantidad { get; set; }

        /// <summary>
        /// Fecha de vencimiento
        /// </summary>
        /// <example>2025-10-15</example>
        public DateTime? FechaVencimiento { get; set; }


        /// <summary>
        /// Código de motivo de ajuste
        /// </summary>
        /// <example>1</example>
        [StringLengthValidation(3, MinimumLength = 0)]
        public string MotivoAjuste { get; set; }

        /// <summary>
        /// Descripción de motivo
        /// </summary>
        /// <example>1</example>
        [StringLengthValidation(50, MinimumLength = 0)]
        public string DescripcionMotivo { get; set; }

        /// <summary>
        /// Tipo de ajuste
        /// </summary>
        /// <example>1</example>
        [SwaggerIgnore]
        [StringLengthValidation(2, MinimumLength = 0)]
        public string TipoAjuste { get; set; }

        /// <summary>
        /// Valor serializado. 
        /// Reservado para datos adicionales.
        /// Debe tener una estructura JSON válida.
        /// </summary>
        /// <example></example>
        [StringLengthValidation(4000, MinimumLength = 0)]
        public string Serializado { get; set; }

        /// <summary>
        /// Fecha de Motivo
        /// </summary>
        /// <example>2025-10-15</example>
        public DateTime? FechaMotivo { get; set; }


    }
}
