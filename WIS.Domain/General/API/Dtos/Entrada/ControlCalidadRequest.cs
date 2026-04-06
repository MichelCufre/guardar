using System.Collections.Generic;
using WIS.Common.API.Attributes;
using WIS.Domain.Validation;

namespace WIS.Domain.General.API.Dtos.Entrada
{
	public class ControlCalidadRequest : IApiEntradaRequest
	{
		#region >> Basic Properties

        /// <summary>
        /// Código de empresa de la ejecución.
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RequiredValidation]
        [ExisteEmpresaValidation()]
        public int Empresa { get; set; }

        /// <summary>
        /// Sirve para generar una referencia o un campo de búsqueda en el panel de ejecuciones de interfaces de WIS.
        /// Mediante este campo es posible identificar o buscar la traza de la ejecución de la interfaz.
        /// Ante un incidente o un procesamiento no esperado se puede reportar el problema haciendo referencia
        /// al valor de este campo.
        /// </summary>
        /// <example>Asociar/Aceptar control de calidad</example>
        [ApiDtoExample("Asociar/Aceptar control de calidad")]
        [RequiredValidation]
        [StringLengthValidation(200, MinimumLength = 1)]
        public string DsReferencia { get; set; }

        /// <summary>
        /// Generalmente se utiliza en sistemas externos que generan archivos y un middleware que oficia de
        /// intermediario entre el archivo y el API. Se puede guardar el nombre del archivo original en el cual
        /// vinieron los datos. En caso de que la implantación no utilice archivos se puede utilizar con otros fines.
        /// </summary>
        /// <example>Archivo</example>
        [ApiDtoExample("Archivo")]
        [StringLengthValidation(100)]
        public string Archivo { get; set; }

		/// <summary>
		/// Sirve para controlar la unicidad de las ejecuciones.
		/// </summary>
		/// <example>123</example>
		[ApiDtoExample("123")]
		[StringLengthValidation(50, MinimumLength = 0)]
		public string IdRequest { get; set; }

		#endregion

		public List <ControlCalidadItemRequest> ControlesDeCalidad { get; set; }

		public ControlCalidadRequest () => this.ControlesDeCalidad = new List <ControlCalidadItemRequest> ();

		public void Add (IApiEntradaItemRequest item)
		{
			throw new System.NotImplementedException ();
		}
	}

	public class ControlCalidadItemRequest : IApiEntradaItemRequest
	{
        /// <summary>
        /// El codigo del Control de Calidad a asociar/aprobar
        /// </summary>
        /// <example>100</example>
        [ApiDtoExample("100")]
        [RequiredValidation]
		public int? CodigoControlCalidad { get; set; }

		/// <summary>
		/// Indica la operacion Aprobar ("CTRAPR") o Asociar ("CTRASC")
		/// </summary>
		/// <example>CTRAPR</example>
		[ApiDtoExample("CTRAPR")]
        [RequiredValidation]
		[StringLengthValidation(10, MinimumLength = 1)]
		public string Estado { get; set; }

		/// <summary>
		/// Permite registrar un mensaje asociado a la aprobacion, describiendo detalles relevantes.
		/// </summary>
		/// <example>Aprobacion por Motivo1</example>
		[ApiDtoExample("Aprobacion por Motivo1")]
		[StringLengthValidation(200, MinimumLength = 0)]
		public string TextoInformativo { get; set; }

		public List <CriterioSeleccionItemRequest> CriteriosDeSeleccion { get; set; }

		public ControlCalidadItemRequest () => this.CriteriosDeSeleccion = new List <CriterioSeleccionItemRequest> ();
	}

	public class CriterioSeleccionItemRequest : IApiEntradaItemRequest
	{
		/// <summary>
		/// Permite cambiar el estado a todas las instancias de un control de calidad en un predio
		/// </summary>
		/// <example>1</example>
		[ApiDtoExample("1")]
        [RequiredValidation]
		[StringLengthValidation(10, MinimumLength = 1)]
		public string Predio { get; set; }

		/// <summary>
		/// Permite aprobar un control de calidad para un identificador especifico.
		/// </summary>
		/// <example>24</example>
		[ApiDtoExample("24")]
		[StringLengthValidation(50, MinimumLength = 0)]
		public string EtiquetaExterna { get; set; }

		/// <summary>
		/// Indica de que tipo es la matricula.
		/// </summary>
		/// <example>L24</example>
		[ApiDtoExample("L24")]
		[StringLengthValidation(10, MinimumLength = 0)]
		public string TipoEtiquetaExterna { get; set; }

		/// <summary>
		/// Permite aprobar un control de calidad aosicado a una ubicacion especifica.
		/// </summary>
		/// <example>1IP00010</example>
		[ApiDtoExample("1IP00010")]
		[StringLengthValidation(40, MinimumLength = 0)]
		public string Ubicacion { get; set; }

		/// <summary>
		/// Indica a que producto se le va a aplicar el control de calidad enviado.
		/// </summary>
		/// <example>PR1</example>
		[ApiDtoExample("PR1")]
        [RequiredValidation]
		[StringLengthValidation(40, MinimumLength = 1)]
		public string Producto { get; set; }

		/// <summary>
		/// Indica a que Lote del producto aplicar el control de calidad enivado.
		/// </summary>
		/// <example>L1</example>
		[ApiDtoExample("L1")]
		[StringLengthValidation(10, MinimumLength = 0)]
		public string Lote { get; set; }
	}
}
