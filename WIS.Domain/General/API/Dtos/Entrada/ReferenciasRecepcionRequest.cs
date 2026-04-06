using System;
using System.Collections.Generic;
using WIS.Common.API.Attributes;
using WIS.Domain.Validation;

namespace WIS.Domain.General.API.Dtos.Entrada
{
    public class ReferenciasRecepcionRequest : IApiEntradaRequest
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
        /// <example>Creación de Referencias</example>
        [ApiDtoExample("Creación de Referencias")]
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
		/// Lista de referencias
		/// </summary>
		[RequiredListValidation]
        public List<ReferenciaRecepcionRequest> Referencias { get; set; }

        public ReferenciasRecepcionRequest()
        {
            Referencias = new List<ReferenciaRecepcionRequest>();
        }

        public void Add(IApiEntradaItemRequest item)
        {
            Referencias.Add(item as ReferenciaRecepcionRequest);
        }
    }

    public class ReferenciaRecepcionRequest : IApiEntradaItemRequest
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
        /// OC (Orden de Compra) - RR (Referencia de Recepción) - OD (Orden de Devolución) - ODC (Devolución Canje)
        /// OC y RR corresponden al tipo de agente PRO
        /// OD y ODC corresponden al tipo de agente CLI
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
        /// El tipo de Agente aceptado depende del tipo de referencia.
        /// Tipos de referencia de devolución, tipo de agente CLI (Cliente), tipos de referencia normales PRO (Proveedor)
        /// </summary>
        /// <example>PRO</example>
        [ApiDtoExample("PRO")]
        [RequiredValidation]
        [StringLengthValidation(3, MinimumLength = 1)]
        public string TipoAgente { get; set; }

        /// <summary>
        /// Moneda
        /// </summary>
        /// <example>USD</example>
        [ApiDtoExample("USD")]
        [StringLengthValidation(6, MinimumLength = 0)]
        public string Moneda { get; set; }

        /// <summary>
        /// Información anexa
        /// </summary>
        /// <example></example>
        [StringLengthValidation(200, MinimumLength = 0)]
        public string Anexo1 { get; set; }

        /// <summary>
        /// Información anexa
        /// </summary>
        /// <example></example>
        [StringLengthValidation(200, MinimumLength = 0)]
        public string Anexo2 { get; set; }

        /// <summary>
        /// Información anexa
        /// </summary>
        /// <example></example>
        [StringLengthValidation(200, MinimumLength = 0)]
        public string Anexo3 { get; set; }

        /// <summary>
        /// Memo
        /// </summary>
        /// <example></example>
        [StringLengthValidation(200, MinimumLength = 0)]
        public string Memo { get; set; }

        /// <summary>
        /// Número de Predio
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [StringLengthValidation(10, MinimumLength = 0)]
        public string Predio { get; set; }

        /// <summary>
        /// Valor serializado. 
        /// Reservado para datos adicionales.
        /// Debe tener una estructura JSON válida.
        /// </summary>
        /// <example></example>
        [StringLengthValidation(200, MinimumLength = 0)]
        public string Serializado { get; set; }

        /// <summary>
        /// Fecha emisión
        /// </summary>
        /// <example>2025-10-15</example>
        [ApiDtoExample("2025-10-15")]
        public DateTime? FechaEmitida { get; set; }

        /// <summary>
        /// Fecha de entrega
        /// </summary>
        /// <example>2025-10-15</example>
        [ApiDtoExample("2025-10-15")]
        public DateTime? FechaEntrega { get; set; }

        /// <summary>
        /// Fecha de vencimiento
        /// </summary>
        /// <example>2025-10-15</example>
        [ApiDtoExample("2025-10-15")]
        public DateTime? FechaVencimientoOrden { get; set; }
                
        [RequiredListValidation(true)]
        public List<ReferenciaRecepcionDetalleRequest> Detalles { get; set; }

        public ReferenciaRecepcionRequest()
        {
            Detalles = new List<ReferenciaRecepcionDetalleRequest>();
        }
    }

    public class ReferenciaRecepcionDetalleRequest
    {
        /// <summary>
        /// Identificación en el ERP o sistema externo para la línea de la referencia. 
        /// Es obligatorio si se envía más de una línea para un mismo producto, en otro caso es opcional.
        /// Si está definido, se retorna en la confirmación para un matcheo en el sistema externo. 
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
        /// Serie o Lote del producto en caso que lo maneje.
        /// Es un valor opcional y el sistema lo manejará en modalidad automática si no viene especificado.
        /// No puede tener espacios en blanco al principio ni al final.
        /// </summary>
        /// <example>LOTE1</example>
        [ApiDtoExample("LOTE1")]
        [StringLengthValidation(40, MinimumLength = 0)]
        public string Identificador { get; set; }

        /// <summary>
        /// Cantidad referencia
        /// </summary>
        /// <example>100</example>
        [ApiDtoExample("100")]
        [RequiredValidation]
        [RangeValidation(15, 3)]
        public decimal CantidadReferencia { get; set; }

        /// <summary>
        /// Importe unitario
        /// </summary>
        /// <example>150</example>
        [ApiDtoExample("150")]
        [RangeValidation(15, 3)]
        public decimal? ImporteUnitario { get; set; }

        /// <summary>
        /// Información anexa
        /// </summary>
        /// <example></example>
        [StringLengthValidation(200, MinimumLength = 0)]
        public string Anexo1 { get; set; }

        /// <summary>
        /// Fecha de vencimiento
        /// </summary>
        /// <example>2025-10-15</example>
        [ApiDtoExample("2025-10-15")]
        public DateTime? FechaVencimiento { get; set; }
    }
}
