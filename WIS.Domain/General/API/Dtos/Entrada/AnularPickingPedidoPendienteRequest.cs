using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using WIS.Common.API.Attributes;
using WIS.Domain.General.Filters;
using WIS.Domain.Validation;

namespace WIS.Domain.General.API.Dtos.Entrada
{
    public class AnularPickingPedidoPendienteRequest : IApiEntradaRequest
    {
        public AnularPickingPedidoPendienteRequest()
        {
            Detalles = new List<AnulacionPedidoPendienteRequest>();
        }
        public void Add(IApiEntradaItemRequest item)
        {
            Detalles.Add(item as AnulacionPedidoPendienteRequest);
        }
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
        /// <example>Anulación Pedido Pendiente</example>
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
        public List<AnulacionPedidoPendienteRequest> Detalles { get; set; }
    }

    public class AnulacionPedidoPendienteRequest : IApiEntradaItemRequest
    {
        public AnulacionPedidoPendienteRequest()
        {
            ProductosAnular = new List<AnulacionPedidoPendienteDetalleRequest>();
        }
        /// <summary>
        /// Código del Pedido
        /// </summary>
        /// <example>12</example>
        public string Pedido { get; set; }

        /// <summary>
        /// Requerido - Código del agente para el cual se realiza el pedido
        /// </summary>
        /// <example>AGE01</example>
        [StringLengthValidation(40, MinimumLength = 1)]
        public string CodigoAgente { get; set; }

        /// <summary>
        /// Requerido - Tipo de agente 
        /// PRO (Proveedor) - CLI (Cliente)
        /// </summary>
        /// <example>PRO</example>
        [StringLengthValidation(3, MinimumLength = 1)]
        public string TipoAgente { get; set; }
        /// <summary>
        /// Número de preparacion
        /// </summary>
        /// <example>2</example>
        [RequiredValidation]
        public int Preparacion { get; set; }
        /// <summary>
        /// Estado Picking
        /// </summary>
        /// <example>ESTAD_PREPARADO</example>
        [StringLengthValidation(40, MinimumLength = 1)]
        public string EstadoPicking { get; set; }

        public string AgrupacionPreparacion { get; set; }

        public long? Carga { get; set; }

        public string ComparteContenedorPicking { get; set; }

        public List<AnulacionPedidoPendienteDetalleRequest> ProductosAnular { get; set; }
    }

    public class AnulacionPedidoPendienteDetalleRequest
    {
        public string CodigoProducto { get; set; }
        public string Identificador { get; set; }
        public decimal CantidadAnular { get; set; }
    }
}
