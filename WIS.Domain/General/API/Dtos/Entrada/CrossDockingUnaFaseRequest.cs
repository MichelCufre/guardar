using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using WIS.Common.API.Attributes;
using WIS.Domain.Validation;

namespace WIS.Domain.General.API.Dtos.Entrada
{
    public class CrossDockingUnaFaseRequest : IApiEntradaRequest
    {
        public CrossDockingUnaFaseRequest()
        {
            CrossDocking = new List<CrossDockingRequest>();
        }
        public void Add(IApiEntradaItemRequest item)
        {
            CrossDocking.Add(item as CrossDockingRequest);
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

		[RequiredListValidation]
        public List<CrossDockingRequest> CrossDocking { get; set; }
    }

    public class CrossDockingRequest : IApiEntradaItemRequest
    {
        /// <summary>
        /// Código de Agenda
        /// </summary>
        /// <example>1</example>
        [RequiredValidation]
        public int Agenda { get; set; }
        /// <summary>
        /// Número de preparacion
        /// </summary>
        /// <example>2</example>
        [RequiredValidation]
        public int Preparacion { get; set; }
        /// <summary>
        /// Ubicación de recepción
        /// </summary>
        /// <example>2</example>
        [RequiredValidation]
        [StringLengthValidation(40, MinimumLength = 1)]
        public string Ubicacion { get; set; }

        [RequiredListValidation]
        public List<DetallesCrossDockingRequest> Detalles { get; set; }

        public CrossDockingRequest()
        {
            Detalles = new List<DetallesCrossDockingRequest>();
        }

        public void Add(IApiEntradaItemRequest item)
        {
            Detalles.Add(item as DetallesCrossDockingRequest);
        }
    }

    public class DetallesCrossDockingRequest : IApiEntradaItemRequest
    {
        /// <summary>
        /// Requerido - Código del agente para el cual se realiza el pedido
        /// </summary>
        /// <example>AGE01</example>
        [RequiredValidation]
        [StringLengthValidation(40, MinimumLength = 1)]
        public string CodigoAgente { get; set; }

        /// <summary>
        /// Requerido - Tipo de agente 
        /// PRO (Proveedor) - CLI (Cliente)
        /// </summary>
        /// <example>PRO</example>
        [RequiredValidation]
        [StringLengthValidation(3, MinimumLength = 1)]
        public string TipoAgente { get; set; }

        /// <summary>
        /// Código de producto
        /// </summary>
        /// <example>PR1</example>
        [RequiredValidation]
        [StringLengthValidation(40, MinimumLength = 1)]
        public string Producto { get; set; }

        /// <summary>
        /// Cantidad
        /// </summary>
        /// <example>100</example>
        [RequiredValidation]
        public decimal Cantidad { get; set; }

        /// <summary>
        /// Serie o Lote del producto en caso que lo maneje.
        /// Es un valor opcional y el sistema lo manejará en modalidad automática si no viene especificado.
        /// No puede tener espacios en blanco al principio ni al final.
        /// </summary>
        /// <example>LOTE1</example>
        [RequiredValidation]
        [StringLengthValidation(40, MinimumLength = 1)]
        public string Identificador { get; set; }

        /// <summary>
        /// Numero de etiqueta
        /// </summary>
        /// <example>1</example>
        [RequiredValidation]
        [StringLengthValidation(50, MinimumLength = 1)]
        public string IdExternoContenedor { get; set; }

        /// <summary>
        /// Tipo de Contenedor
        /// </summary>
        /// <example>W</example>
        [RequiredValidation]
        [StringLengthValidation(10, MinimumLength = 1)]
        public string TipoContenedor { get; set; }

        /// <summary>
        /// Estado Destino
        /// </summary>
        /// <example>604</example>
        [RequiredValidation]
        public int SituacionDestino { get; set; }
    }
}
