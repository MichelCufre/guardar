using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WIS.Common.API.Attributes;
using WIS.Domain.Validation;

namespace WIS.Domain.General.API.Dtos.Entrada
{
    public class EmpresasRequest : IApiEntradaRequest
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
        /// <example>Creación de empresas</example>
        [ApiDtoExample("Creación de empresas")]
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
		/// Lista de empresas
		/// </summary>
		[RequiredListValidation]
        public List<EmpresaRequest> Empresas { get; set; }

        public EmpresasRequest()
        {
            Empresas = new List<EmpresaRequest>();
        }

        public void Add(IApiEntradaItemRequest item)
        {
            Empresas.Add(item as EmpresaRequest);
        }
    }
    public class EmpresaRequest : IApiEntradaItemRequest
    {
        /// <summary>
        /// Código de empresa
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RequiredValidation]
        [RangeValidation(10)]
        public int Codigo { get; set; }

        /// <summary>
        /// Descripción de empresa
        /// </summary>
        /// <example>Empresa 1</example>
        [ApiDtoExample("Empresa 1")]
        [RequiredValidation]
        [StringLengthValidation(55, MinimumLength = 1)]
        public string Nombre { get; set; }

        /// <summary>
        /// Estado
        /// </summary>
        /// <example>15</example>
        [ApiDtoExample("15")]
        [RangeValidation(3)]
        public short? Estado { get; set; }

        /// <summary>
        /// Dirección
        /// </summary>
        /// <example>18 de Julio 515</example>
        [ApiDtoExample("18 de Julio 515")]
        [StringLengthValidation(100, MinimumLength = 0)]
        public string Direccion { get; set; }

        /// <summary>
        /// Teléfono
        /// </summary>
        /// <example>42222222</example>
        [ApiDtoExample("42222222")]
        [StringLengthValidation(30, MinimumLength = 0)]
        public string Telefono { get; set; }

        /// <summary>
        /// Número fiscal
        /// </summary>
        /// <example>1212</example>
        [ApiDtoExample("1212")]
        [StringLengthValidation(30, MinimumLength = 0)]
        public string NumeroFiscal { get; set; }

        /// <summary>
        /// Tipo fiscal
        /// RUT (RUT) - LIBRE (LIBRE)
        /// </summary>
        /// <example>RUT</example>
        [ApiDtoExample("RUT")]
        [StringLengthValidation(20, MinimumLength = 0)]
        public string TipoFiscal { get; set; }

        /// <summary>
        /// Código postal
        /// </summary>
        /// <example>20000</example>
        [ApiDtoExample("20000")]
        [StringLengthValidation(15, MinimumLength = 0)]
        public string CodigoPostal { get; set; }

        /// <summary>
        /// Cliente Armado Kit
        /// </summary>
        /// <example>WIS-KIT</example>
        [ApiDtoExample("WIS-KIT")]
        [StringLengthValidation(10, MinimumLength = 0)]
        public string ClienteArmadoKit { get; set; }

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
        /// Información anexa
        /// </summary>
        /// <example></example>
        [StringLengthValidation(200, MinimumLength = 0)]
        public string Anexo4 { get; set; }

        /// <summary>
        /// Código del tipo de almacenaje y seguro
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RangeValidation(3)]
        public short? TipoDeAlmacenajeYSeguro { get; set; }

        /// <summary>
        /// Mínimo stock
        /// </summary>
        /// <example>100</example>
        [ApiDtoExample("100")]
        [RangeValidation(17,3)]
        public decimal? ValorMinimoStock { get; set; }

        /// <summary>
        /// Código de empresa de consolidado
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RangeValidation(10)]
        public int? EmpresaConsolidado { get; set; }

        /// <summary>
        /// Proveedor devolución
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RangeValidation(7)]
        public int? ProveedorDevolucion { get; set; }

        /// <summary>
        /// Lista de precio
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RangeValidation(6)]
        public int? ListaPrecio { get; set; }

        /// <summary>
        /// IdDAP
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [StringLengthValidation(1, MinimumLength = 0)]
        public string IdDAP { get; set; }

        /// <summary>
        /// Operativo
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [StringLengthValidation(1, MinimumLength = 0)]
        public string IdOperativo { get; set; }

        /// <summary>
        /// Unidad Factura
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [StringLengthValidation(1, MinimumLength = 0)]
        public string IdUnidadFactura { get; set; }

        /// <summary>
        /// Cantidad de días
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RangeValidation(3)]
        public short? CantidadDiasPeriodo { get; set; }

        /// <summary>
        /// Valor pallet
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RangeValidation(15,2)]
        public decimal? ValorPallet { get; set; }

        /// <summary>
        /// Valor pallet por día
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RangeValidation(15, 2)]
        public decimal? ValorPalletDia { get; set; }

        /// <summary>
        /// Código de país (ISO 3166-1)
        /// </summary>
        /// <example>UY</example>
        [ApiDtoExample("UY")]
        [StringLengthValidation(2, MinimumLength = 0)]
        public string Pais { get; set; }

        /// <summary>
        /// Subdivisión - (ISO 3166-2)
        /// </summary>
        /// <example>UY-MA</example>
        [ApiDtoExample("UY-MA")]
        [StringLengthValidation(20, MinimumLength = 0)]
        public string Subdivision { get; set; }

        /// <summary>
        /// Municipio localidad
        /// </summary>
        /// <example>PDE</example>
        [ApiDtoExample("PDE")]
        [StringLengthValidation(10, MinimumLength = 0)]
        public string Localidad { get; set; }
    }
}
