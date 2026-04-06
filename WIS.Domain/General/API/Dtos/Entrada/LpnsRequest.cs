using System;
using System.Collections.Generic;
using WIS.Common.API.Attributes;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Domain.Validation;

namespace WIS.WMS_API.Controllers.Entrada
{
    public class LpnsRequest : IApiEntradaRequest
    {
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
        /// Ante un incidente o un procesamiento no esperado se puede reportar el problema haciendo referencia al valor de este campo.
        /// </summary>
        /// <example>Creación de LPN</example>
        [ApiDtoExample("Creación de LPN")]
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
		/// Lista de Lpns.
		/// </summary>
		[RequiredListValidation]
        public List<LpnRequest> Lpns { get; set; }

        public LpnsRequest()
        {
            Lpns = new List<LpnRequest>();
        }

        public void Add(IApiEntradaItemRequest item)
        {
            Lpns.Add(item as LpnRequest);
        }
    }

    public class LpnRequest : IApiEntradaItemRequest
    {
        /// <summary>
        /// Identificador del ERP o sistema externo para el Lpn.
        /// </summary>
        /// <example>AZ-39</example>
        [ApiDtoExample("AZ-39")]
        [RequiredValidation]
        [StringLengthValidation(50, MinimumLength = 1)]
        public string IdExterno { get; set; }

        /// <summary>
        /// Tipo de Lpn.
        /// En caso de no enviar un valor se tomará el del parámetro de configuración asignado. IE_535_TP_LPN_TIPO
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [StringLengthValidation(10, MinimumLength = 0)]
        public string Tipo { get; set; }

        /// <summary>
        /// Sirve para identificar un conjunto de LPNs.
        /// </summary>
        /// <example>ID-12</example>      
        [ApiDtoExample("ID-12")]
        [StringLengthValidation(50, MinimumLength = 0)]
        public string IdPacking { get; set; }

        /// <summary>
        /// Opcional.
        /// Lista de código de barras del Lpn.
        /// Independientemente de su envío se generará un código de barras interno que también se asociará al Lpn.
        /// </summary>  
        public List<BarrasRequest> Barras { get; set; }

        /// <summary>
        /// Opcional.
        /// Lista de atributos del Lpn. 
        /// En caso de no enviar ninguno se tomaran los valores iniciales definidos en el tipo de lpn correspondiente.
        /// </summary>  
        public List<AtributoRequest> Atributos { get; set; }

        /// <summary>
        /// Lista de detalles de un Lpn.
        /// </summary>
        [RequiredListValidation(true)]
        public List<LpnDetalleRequest> Detalles { get; set; }

        public LpnRequest()
        {
            Detalles = new List<LpnDetalleRequest>();
            Atributos = new List<AtributoRequest>();
        }
    }

    public class LpnDetalleRequest
    {
        /// <summary>
        /// Identificador en el ERP o sistema externo para la línea del detalle. 
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RequiredValidation]
        [StringLengthValidation(40, MinimumLength = 1)]
        public string IdLineaSistemaExterno { get; set; }

        /// <summary>
        /// Código de producto.
        /// </summary>
        /// <example>PR1</example>
        [ApiDtoExample("PR1")]
        [RequiredValidation]
        [StringLengthValidation(40, MinimumLength = 1)]
        public string CodigoProducto { get; set; }

        /// <summary>
        /// Lote o número que identifica al producto.
        /// El mismo es requerido si el producto maneja Lote.
        /// </summary>
        /// <example>LOTE1</example>
        [ApiDtoExample("LOTE1")]
        [StringLengthValidation(40, MinimumLength = 0)]
        public string Identificador { get; set; }

        /// <summary>
        /// Cantidad declarada para el producto.
        /// </summary>
        /// <example>100</example>
        [ApiDtoExample("100")]
        [RequiredValidation]
        [RangeValidation(12, 3)]
        public decimal CantidadDeclarada { get; set; }

        /// <summary>
        /// Fecha de vencimiento del producto. 
        /// Requerido si el producto tiene TipoManejoFecha igual a E (Expirable).
        /// </summary>
        /// <example>2025-10-15</example>
        [ApiDtoExample("2025-10-15")]
        public DateTime? FechaVencimiento { get; set; }

        /// <summary>
        /// Opcional.        
        /// Lista de atributos del detalle de un Lpn.
        /// En caso de no enviar ninguno se tomaran los valores iniciales definidos en el tipo de lpn correspondiente.
        /// </summary>  
        public List<AtributoRequest> Atributos { get; set; }

        public LpnDetalleRequest()
        {
            Atributos = new List<AtributoRequest>();
        }
    }

    public class AtributoRequest
    {
        /// <summary>
        /// Identifica el tipo de dato a ingresar.
        /// </summary>
        /// <example>224</example>
        [ApiDtoExample("224")]
        [RequiredValidation]
        public string Nombre { get; set; }

        /// <summary>
        /// Valor del atributo. Las validaciones del mismo dependeran el atributo especificado.
        /// </summary>
        /// <example>TEXTO</example>
        [ApiDtoExample("TEXTO")]
        [RequiredValidation]
        [StringLengthValidation(400, MinimumLength = 1)]
        public string Valor { get; set; }
    }

    public class BarrasRequest
    {
        /// <summary>
        /// Código de barras externo de la etiqueta.
        /// </summary>
        /// <example>0000000000007018</example>
        [ApiDtoExample("0000000000007018")]
        [RequiredValidation]
        [StringLengthValidation(100, MinimumLength = 1)]
        public string CodigoBarras { get; set; }

        /// <summary>
        /// Tipo de código de barras
        /// En caso de no enviar uno se tomará el tipo CB por defecto
        /// </summary>
        /// <example>CB</example>
        [ApiDtoExample("CB")]
        [StringLengthValidation(2, MinimumLength = 0)]
        public string Tipo { get; set; }
    }

}
