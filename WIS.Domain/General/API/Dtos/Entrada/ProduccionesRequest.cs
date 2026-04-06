using System;
using System.Collections.Generic;
using WIS.Common.API.Attributes;
using WIS.Domain.Picking;
using WIS.Domain.Produccion.Models;
using WIS.Domain.Validation;


namespace WIS.Domain.General.API.Dtos.Entrada
{
    public class ProduccionesRequest : IApiEntradaRequest
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
        /// <example>Creación de Agenda</example>
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

        /// <summary>
        /// Lista de Ingresos
        /// </summary>
        [RequiredListValidation]
        public List<ProduccionRequest> Ingresos { get; set; }

        public ProduccionesRequest()
        {
            Ingresos = new List<ProduccionRequest>();
        }

        public void Add(IApiEntradaItemRequest item)
        {
            Ingresos.Add(item as ProduccionRequest);
        }
    }

    public class ProduccionRequest : IApiEntradaItemRequest
    {
        [ApiDtoExample("Id 1")]
        [StringLengthValidation(100, MinimumLength = 0)]
        public string IdProduccionExterno { get; set; }

        [ApiDtoExample("TPINGPR_BLACKBOX")]
        [RequiredValidation]
        [StringLengthValidation(20, MinimumLength = 1)]
        public string Tipo { get; set; }

        [ApiDtoExample("1")]
        [StringLengthValidation(10, MinimumLength = 0)]
        public string EspacioProduccion { get; set; }

        [ApiDtoExample("1")]
        [RequiredValidation]
        [StringLengthValidation(10, MinimumLength = 1)]
        public string Predio { get; set; }

        [ApiDtoExample("CF1")]
        [StringLengthValidation(10, MinimumLength = 0)]
        public string CodigoFormula { get; set; }

        [ApiDtoExample("1")]
        [RangeValidation(6, distintoCero: false)]
        public int? CantidadFormula { get; set; }

        [ApiDtoExample("IDI")]
        [StringLengthValidation(20, MinimumLength = 0)]
        public string IdModalidadLote { get; set; }

        [ApiDtoExample("Flujo1")]
        [StringLengthValidation(40, MinimumLength = 1)]
        public string TipoDeFlujo { get; set; }

        [ApiDtoExample("true")]
        public bool? GenerarPedido { get; set; }

        [ApiDtoExample("true")]
        public bool? LiberarPedido { get; set; }

        [ApiDtoExample("Anexo1")]
        [StringLengthValidation(200, MinimumLength = 0)]
        public string Anexo1 { get; set; }

        [ApiDtoExample("Anexo2")]
        [StringLengthValidation(200, MinimumLength = 0)]
        public string Anexo2 { get; set; }

        [ApiDtoExample("Anexo3")]
        [StringLengthValidation(200, MinimumLength = 0)]
        public string Anexo3 { get; set; }

        [ApiDtoExample("Anexo4")]
        [StringLengthValidation(200, MinimumLength = 0)]
        public string Anexo4 { get; set; }

        [ApiDtoExample("Anexo5")]
        [StringLengthValidation(200, MinimumLength = 0)]
        public string Anexo5 { get; set; }

        public List<ProduccionInsumoRequest> Insumos { get; set; }

        public List<ProduccionProductoFinalesRequest> Productos { get; set; }

        public ProduccionRequest()
        {
            Insumos = new List<ProduccionInsumoRequest>();
            Productos = new List<ProduccionProductoFinalesRequest>();
        }
    }
}
