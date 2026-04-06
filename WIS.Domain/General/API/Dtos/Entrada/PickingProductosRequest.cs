using System.Collections.Generic;
using WIS.Common.API.Attributes;
using WIS.Domain.Validation;

namespace WIS.Domain.General.API.Dtos.Entrada
{
    public class PickingProductosRequest : IApiEntradaRequest
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
        /// <example>Creación de ubicaciones de picking</example>
        [ApiDtoExample("Creación de ubicaciones de picking")]
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
        /// Lista de ubicaciones
        /// </summary>
        [RequiredListValidation]
        public List<PickingProductoRequest> PickingProductos { get; set; }

        public PickingProductosRequest()
        {
            PickingProductos = new List<PickingProductoRequest>();
        }

        public void Add(IApiEntradaItemRequest item)
        {
            PickingProductos.Add(item as PickingProductoRequest);
        }
    }

    public class PickingProductoRequest : IApiEntradaItemRequest
    {
        /// <summary>
        /// Código de producto.
        /// Solo puede estar compuesto por los siguientes caracteres: 01234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ-_/.* 
        /// </summary>
        /// <example>PR1</example>
        [ApiDtoExample("PR1")]
        [RequiredValidation]
        [StringLengthValidation(40, MinimumLength = 1)]
        public string CodigoProducto { get; set; }

        /// <summary>
        /// Ubicación a la que se le asigna el picking
        /// </summary>
        /// <example>1AA00000</example>
        [RequiredValidation]
        [StringLengthValidation(40, MinimumLength = 1)]
        public string Ubicacion { get; set; }

        /// <summary>
        /// Padron.
        /// </summary>
        /// <example>100</example>
        [ApiDtoExample("100")]
        [RequiredValidation]
        [RangeValidation(18, 3)]
        public decimal Padron { get; set; }

        /// <summary>
        /// Cantidad minima de la ubicación.
        /// </summary>
        /// <example>100</example>
        [ApiDtoExample("100")]
        [RequiredValidation]
        public int? StockMinimo { get; set; }

        /// <summary>
        /// Cantidad maxima de la ubicación.
        /// </summary>
        /// <example>1000</example>
        [ApiDtoExample("1000")]
        [RequiredValidation]
        public int? StockMaximo { get; set; }

        /// <summary>
        /// Cantidad desborde.
        /// </summary>
        /// <example>10000</example>
        [ApiDtoExample("10000")]
        [RequiredValidation]
        public int? CantidadDesborde { get; set; }

        public string codigoUnidadCajaAutomatismo { get; set; }
        public int? cantidadUnidadCajaAutomatismo { get; set; }        
        public string flagConfirmarCodBarrasAutomatismo { get; set; }
        public int prioridad { get; set; }

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
