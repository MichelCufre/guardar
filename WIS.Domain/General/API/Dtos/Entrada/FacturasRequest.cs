using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Common.API.Attributes;
using WIS.Domain.Validation;

namespace WIS.Domain.General.API.Dtos.Entrada
{
    public class FacturasRequest : IApiEntradaRequest
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
        /// <example>Creación de Factura</example>
        [ApiDtoExample("Creación de Factura")]
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

        [RequiredListValidation]
        public List<FacturaRequest> Facturas { get; set; }

        public FacturasRequest()
        {
            Facturas = new List<FacturaRequest>();
        }

        public void Add(IApiEntradaItemRequest item)
        {
            Facturas.Add(item as FacturaRequest);
        }
    }

    public class FacturaRequest : IApiEntradaItemRequest
    {
        [RequiredValidation]
        [StringLengthValidation(10)]
        public string Predio { get; set; }

        /// <summary>
        /// Número de Serie
        /// </summary>
        /// <example>1</example>
        [RequiredValidation]
        [StringLengthValidation(3)]
        public string Serie { get; set; }

        /// <summary>
        /// Número de Factura
        /// </summary>
        /// <example>1400022</example>
        [RequiredValidation]
        [StringLengthValidation(12)]
        public string Factura { get; set; }

        /// <summary>
        /// Tipo de Factura
        /// </summary>
        [RequiredValidation]
        [StringLengthValidation(12)]
        public string TipoFactura { get; set; }

        /// <summary>
        /// Fecha de emisión
        /// </summary>
        /// <example>2025-10-15</example>
        public DateTime? FechaEmision { get; set; }

        /// <summary>
        /// Total Digitado
        /// </summary>
        [RangeValidation(15, 4)]
        public decimal? TotalDigitado { get; set; }

        /// <summary>
        /// Código de agente
        /// </summary>
        /// <example>PRO040</example>
        [RequiredValidation]
        [StringLengthValidation(10, MinimumLength = 1)]
        public string CodigoAgente { get; set; }

        /// <summary>
        /// Fecha de vencimiento
        /// </summary>
        /// <example>2025-10-15</example>
        public DateTime? FechaVencimiento { get; set; }

        /// <summary>
        /// Moneda
        /// </summary>
        /// <example>USD</example>
        [StringLengthValidation(6, MinimumLength = 0)]
        public string CodigoMoneda { get; set; }

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
        /// Observación
        /// </summary>
        /// <example></example>
        [StringLengthValidation(200, MinimumLength = 0)]
        public string Observacion { get; set; }

        /// <summary>
        /// IVA Base
        /// </summary>
        [RangeValidation(15, 4)]
        public decimal? IvaBase { get; set; }

        /// <summary>
        /// IVA Minimo
        /// </summary>
        [RangeValidation(15, 4)]
        public decimal? IvaMinimo { get; set; }

        [RequiredListValidation(true)]
        public List<FacturaDetalleRequest> Detalles { get; set; }

        public FacturaRequest()
        {
            this.Detalles = new List<FacturaDetalleRequest>();
        }
    }

    public class FacturaDetalleRequest
    {
        /// <summary>
        /// Código de producto
        /// </summary>
        /// <example>PR1</example>
        [RequiredValidation]
        [StringLengthValidation(40, MinimumLength = 1)]
        public string Producto { get; set; }

        /// <summary>
        /// Serie o Lote
        /// </summary>
        /// <example>PR1</example>
        [StringLengthValidation(40, MinimumLength = 0)]
        public string Identificador { get; set; }

        /// <summary>
        /// Cantidad Facturada
        /// /// </summary>
        /// 
        [RequiredValidation]
        [RangeValidation(12, 3)]
        public decimal? CantidadFacturada { get; set; }

        /// <summary>
        /// Unitario Digitado
        /// </summary>
        [RangeValidation(15, 4)]
        public decimal? UnitarioDigitado { get; set; }

        [ApiDtoExample("2025-10-15")]
        public DateTime? FechaVencimiento { get; set; }

        /// <summary>
        /// Información anexa
        /// </summary>
        /// <example>Anexo 1</example>
        [StringLengthValidation(200, MinimumLength = 0)]
        public string Anexo1 { get; set; }

        /// <summary>
        /// Información anexa
        /// </summary>
        /// <example>Anexo 2</example>
        [StringLengthValidation(200, MinimumLength = 0)]
        public string Anexo2 { get; set; }

        /// <summary>
        /// Información anexa
        /// </summary>
        /// <example>Anexo 3</example>
        [StringLengthValidation(200, MinimumLength = 0)]
        public string Anexo3 { get; set; }


        /// <summary>
        /// Información anexa
        /// </summary>
        /// <example>Anexo 4</example>
        [StringLengthValidation(200, MinimumLength = 0)]
        public string Anexo4 { get; set; }
    }
}
