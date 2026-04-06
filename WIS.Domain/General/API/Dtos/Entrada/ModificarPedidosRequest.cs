using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using WIS.Common.API.Attributes;
using WIS.Domain.General.Filters;
using WIS.Domain.Validation;

namespace WIS.Domain.General.API.Dtos.Entrada
{
    public class ModificarPedidosRequest : IApiEntradaRequest
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
        /// <example>Modificación de pedidos</example>
        [ApiDtoExample("Modificación de pedidos")]
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
		/// Lista de pedidos. 
		/// Es recomendable enviar un pedido por solicitud y completar el DsReferencia con el identificador del mismo para un mejor trackeo de las interfaces, de todas formas se puede enviar una lista de pedidos si así se requiere.
		/// </summary>
		[RequiredListValidation]
        public List<ModificarPedidoRequest> Pedidos { get; set; }

        public ModificarPedidosRequest()
        {
            Pedidos = new List<ModificarPedidoRequest>();
        }

        public void Add(IApiEntradaItemRequest item)
        {
            Pedidos.Add(item as ModificarPedidoRequest);
        }
    }

    public class ModificarPedidoRequest : IApiEntradaItemRequest
    {
        /// <summary>
        /// Requerido - Valor alfanumérico que representa un pedido en el sistema, conforma un identificador único junto al código de agente, tipo de agente y la empresa a la que pertenece.
        /// </summary>
        /// <example>PED100</example>
        [ApiDtoExample("PED100")]
        [RequiredValidation]
        [StringLengthValidation(40, MinimumLength = 1)]
        public string NroPedido { get; set; }

        /// <summary>
        /// Requerido - Código del agente para el cual se realiza el pedido
        /// </summary>
        /// <example>AGE01</example>
        [ApiDtoExample("AGE01")]
        [RequiredValidation]
        [StringLengthValidation(40, MinimumLength = 1)]
        public string CodigoAgente { get; set; }

        /// <summary>
        /// Requerido - Tipo de agente 
        /// PRO (Proveedor) - CLI (Cliente)
        /// </summary>
        /// <example>PRO</example>
        [ApiDtoExample("PRO")]
        [RequiredValidation]
        [StringLengthValidation(3, MinimumLength = 1)]
        public string TipoAgente { get; set; }

        /// <summary>
        /// Permite agrupar el pedido dentro de una condición de liberación determinada.
        /// Se utiliza como un filtro para organizar y clasificar las ondas.  
        /// Los valores precargados en el sistema son:
        /// -WIS-SC: SIN CONDICION
        /// -DIA: DIARIOS-SIEMPRE
        /// -TRA: TRASPASOS
        /// -AAPM: AUTO-ASIGNAR-PREPARACION-MANUAL
        /// (Consulte valores disponibles con el responsable operativo de WIS)
        /// </summary>
        /// <example>WIS-SC</example>
        [ApiDtoExample("WIS-SC")]
        [StringLengthValidation(6, MinimumLength = 0)]
        public string CondicionLiberacion { get; set; }

        /// <summary>
        /// Punto de Entrega o código único de entrega para el cliente. Puede dejarse nulo, generalmente es asignado automáticamente cuando Tracking está instalado y georreferencia la dirección de entrega, asignándole un punto de entrega único.
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [StringLengthValidation(20, MinimumLength = 0)]
        public string PuntoEntrega { get; set; }

        /// <summary>
        /// Requerido con valor por defecto.
        /// Indica el código de ruta del pedido, detallando a cuál recorrido pertenece la entrega
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RangeValidation(3)]
        public short? Ruta { get; set; }

        /// <summary>
        /// Código de la empresa transportista relacionada al pedido
        /// </summary>
        /// <example>2</example>
        [ApiDtoExample("2")]
        [RangeValidation(10)]
        public int? CodigoTransportadora { get; set; }

        /// <summary>
        /// Especifica la zona a la que pertenece el pedido
        /// </summary>
        /// <example>ZONA1</example>
        [ApiDtoExample("ZONA1")]
        [StringLengthValidation(20, MinimumLength = 0)]
        public string Zona { get; set; }

        /// <summary>
        /// Información adicional
        /// </summary>
        /// <example></example>
        [StringLengthValidation(200, MinimumLength = 0)]
        public string Anexo1 { get; set; }

        /// <summary>
        /// Información adicional
        /// </summary>
        /// <example></example>
        [StringLengthValidation(200, MinimumLength = 0)]
        public string Anexo2 { get; set; }

        /// <summary>
        /// Información adicional
        /// </summary>
        /// <example></example>
        [StringLengthValidation(200, MinimumLength = 0)]
        public string Anexo3 { get; set; }

        /// <summary>
        /// Información adicional
        /// </summary>
        /// <example></example>
        [StringLengthValidation(200, MinimumLength = 0)]
        public string Anexo4 { get; set; }

        /// <summary>
        /// Dirección de entrega que se usará para la generación de punto de entrega y ruteo en el módulo de tracking.
        /// El formato recomendable es:
        /// Calle + Número, Código Postal + Localidad, Departamento, País
        /// </summary>
        /// <example> Arturo Santana 811, 20100 Punta del Este, Departamento de Maldonado,Uruguay</example>
        [ApiDtoExample(" Arturo Santana 811, 20100 Punta del Este, Departamento de Maldonado,Uruguay")]
        [StringLengthValidation(100, MinimumLength = 0)]
        public string Direccion { get; set; }

        /// <summary>
        /// Información adicional
        /// </summary>
        /// <example></example>
        [StringLengthValidation(1000, MinimumLength = 0)]
        public string Memo { get; set; }

        /// <summary>
        ///  Información adicional
        /// </summary>
        /// <example></example>
        [StringLengthValidation(1000, MinimumLength = 0)]
        public string Memo1 { get; set; }

        /// <summary>
        /// Fecha de emisión del pedido
        /// </summary>
        /// <example>2022-02-15</example>
        [ApiDtoExample("2022-02-15")]
        public DateTime? FechaEmision { get; set; }

        /// <summary>
        /// Fecha de entrega especificada para el pedido
        /// </summary>
        /// <example>2022-02-15</example>
        [ApiDtoExample("2022-02-15")]
        public DateTime? FechaEntrega { get; set; }

        /// <summary>
        /// Fecha mínima de liberación del pedido
        /// </summary>
        /// <example>2022-02-15</example>
        [ApiDtoExample("2022-02-15")]
        public DateTime? FechaLiberarDesde { get; set; }

        /// <summary>
        /// Fecha máxima de liberación del pedido
        /// </summary>
        /// <example>2022-03-20</example>
        [ApiDtoExample("2022-02-15")]
        public DateTime? FechaLiberarHasta { get; set; }

        /// <summary>
        /// Fecha genérica auxiliar
        /// </summary>
        /// <example>2022-05-20</example>
        [ApiDtoExample("2022-02-15")]
        public DateTime? FechaGenerica { get; set; }

        /// <summary>
        /// Indica cómo se va a agrupar el total del producto al liberar el pedido.
        /// P (Pedido) - C (Cliente) - O (Onda) - R (Ruta)
        /// </summary>
        /// <example>P</example>
        [ApiDtoExample("P")]
        [StringLengthValidation(1, MinimumLength = 0)]
        public string Agrupacion { get; set; }

        /// <summary>
        /// Número genérico auxiliar
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RangeValidation(15, 3)]
        public decimal? NuGenerico { get; set; }

        /// <summary>
        /// Orden de entrega
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RangeValidation(6)]
        public int? OrdenEntrega { get; set; }

        /// <summary>
        /// Requerido - Número de Predio.
        /// Especifica el predio del depósito desde que se quiere atender el pedido.
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [StringLengthValidation(10, MinimumLength = 0)]
        public string Predio { get; set; }

        /// <summary>
        /// Requerido con valor por defecto.
        /// Indica el tipo de expedición para el pedido, regula el flujo del pedido especificando tareas adicionales para la expedición (ej. facturación, expediciones con packing, normales, etc.)
        /// Valores predeterminados:
        /// -PRC: PROCESO PRODUCCION
        /// -WIS: FACTURABLES
        /// -WSF: NO FACTURABLES
        /// </summary>
        /// <example>WIS</example>
        [ApiDtoExample("WIS")]
        [StringLengthValidation(6, MinimumLength = 0)]
        public string TipoExpedicion { get; set; }

        /// <summary>
        /// Requerido con valor por defecto.
        /// Especifica el tipo de pedido el cual está directamente relacionado y tiene que ser compatible con el tipo de expedición.
        /// </summary>
        /// <example>VEN</example>
        [ApiDtoExample("VEN")]
        [StringLengthValidation(6, MinimumLength = 0)]
        public string TipoPedido { get; set; }

        /// <summary>
        /// Valor comparte contenedor entrega
        /// </summary>
        /// <example>Valor de ejemplo</example>
        [ApiDtoExample("Valor de ejemplo")]
        [StringLengthValidation(200, MinimumLength = 0)]
        public string ComparteContenedorEntrega { get; set; }

        /// <summary>
        /// Valor de agrupamiento de pedidos que regula la compatibilidad al momento de preparar mercadería
        /// </summary>
        /// <example>Valor de ejemplo</example>
        [ApiDtoExample("Valor de ejemplo")]
        [StringLengthValidation(200, MinimumLength = 0)]
        public string ComparteContenedorPicking { get; set; }

        /// <summary>
        /// Valor genérico auxiliar
        /// </summary>
        /// <example>Valor de ejemplo</example>
        [ApiDtoExample("Valor de ejemplo")]
        [StringLengthValidation(400, MinimumLength = 0)]
        public string DsGenerico { get; set; }

        /// <summary>
        /// Campo utilizado para mandar metadata en el cabezal de pedido.
        /// </summary>
        /// <example>Valor de ejemplo</example>
        [ApiDtoExample("Valor de ejemplo")]
        [StringLengthValidation(4000, MinimumLength = 0)]
        public string Serializado { get; set; }

        /// <summary>
        /// Teléfono
        /// </summary>
        /// <example>42255555</example>
        [ApiDtoExample("42255555")]
        [StringLengthValidation(30, MinimumLength = 0)]
        public string Telefono { get; set; }

        /// <summary>
        /// Teléfono secundario
        /// </summary>
        /// <example>42255554</example>
        [ApiDtoExample("42255554")]
        [StringLengthValidation(30, MinimumLength = 0)]
        public string TelefonoSecundario { get; set; }

        /// <summary>
        /// Valor de longitud util para los puntos de entrega de tracking 
        /// </summary>
        /// <example>0,2</example>
        [ApiDtoExample("-35.15151")]
        [RangeValidation(10, 7, false, false)]
        public decimal? Longitud { get; set; }

        /// <summary>
        /// Valor de latitud util para los puntos de entrega de tracking 
        /// </summary>
        /// <example>0,2</example>
        [ApiDtoExample("-35.15151")]
        [RangeValidation(9, 7, false, false)]
        public decimal? Latitud { get; set; }

        public List<ModificarPedidoLpnRequest> Lpns { get; set; }
        public List<ModificarDetallePedidoRequest> Detalles { get; set; }

        public ModificarPedidoRequest()
        {
            Lpns = new List<ModificarPedidoLpnRequest>();
            Detalles = new List<ModificarDetallePedidoRequest>();
        }
    }

    public class ModificarPedidoLpnRequest
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

    }

    public class ModificarDetallePedidoRequest
    {
        /// <summary>
        /// Código de producto
        /// </summary>
        /// <example>PR1</example>
        [ApiDtoExample("PR1")]
        [RequiredValidation]
        [StringLengthValidation(40, MinimumLength = 1)]
        public string CodigoProducto { get; set; }

        /// <summary>
        /// Lote o número que identifica al producto.
        /// </summary>
        /// <example>LOTE1</example>
        [ApiDtoExample("LOTE1")]
        [StringLengthValidation(40, MinimumLength = 0)]
        public string Identificador { get; set; }

        /// <summary>
        /// Cantidad pedida del producto
        /// </summary>
        /// <example>100</example>
        [ApiDtoExample("100")]
        //[RequiredValidation]
        [RangeValidation(12, 3, false)]
        public decimal? Cantidad { get; set; }

        /// <summary>
        /// Información adicional
        /// </summary>
        /// <example></example>
        [StringLengthValidation(200, MinimumLength = 0)]
        public string Memo { get; set; }

        /// <summary>
        /// Fecha genérica auxiliar
        /// </summary>
        /// <example>2025-10-15</example>
        [ApiDtoExample("2025-10-15")]
        public DateTime? FechaGenerica { get; set; }

        /// <summary>
        /// Número genérico auxiliar
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RangeValidation(15, 3)]
        public decimal? NuGenerico { get; set; }

        /// <summary>
        /// Valor genérico auxiliar
        /// </summary>
        /// <example>Valor de ejemplo</example>
        [ApiDtoExample("Valor de ejemplo")]
        [StringLengthValidation(400, MinimumLength = 0)]
        public string DsGenerico { get; set; }

        /// <summary>
        /// Campo utilizado para mandar metadata en el detalle del pedido.
        /// </summary>
        /// <example>Valor de ejemplo</example>
        [ApiDtoExample("Valor de ejemplo")]
        [StringLengthValidation(4000, MinimumLength = 0)]
        public string Serializado { get; set; }

        public List<ModificarDetallePedidoDuplicadoRequest> Duplicados { get; set; }
        public List<ModificarDetallePedidoLpnRequest> DetallesLpn { get; set; }
        public List<ModificarDetallePedidoAtributosRequest> Atributos { get; set; }

        public ModificarDetallePedidoRequest()
        {
            Duplicados = new List<ModificarDetallePedidoDuplicadoRequest>();
            DetallesLpn = new List<ModificarDetallePedidoLpnRequest>();
            Atributos = new List<ModificarDetallePedidoAtributosRequest>();
        }
    }

    public class ModificarDetallePedidoDuplicadoRequest
    {
        /// <summary>
        /// Identificación en el ERP o sistema externo para la línea del pedido. 
        /// Es obligatorio si se envía más de una línea para un mismo producto, en otro caso es opcional.
        /// Si está definido, se retorna en la confirmación para un matcheo en el sistema externo. 
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RequiredValidation]
        [StringLengthValidation(40, MinimumLength = 1)]
        public string IdLineaSistemaExterno { get; set; }

        /// <summary>
        /// Tipo de línea.
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [StringLengthValidation(200, MinimumLength = 0)]
        public string TipoLinea { get; set; }

        /// <summary>
        /// Cantidad pedida del producto
        /// </summary>
        /// <example>100</example>
        [ApiDtoExample("100")]
        [RequiredValidation]
        [RangeValidation(12, 3, false)]
        public decimal Cantidad { get; set; }

        /// <summary>
        /// Campo utilizado para mandar metadata en el detalle del duplicado.
        /// </summary>
        /// <example>Valor de ejemplo</example>
        [ApiDtoExample("Valor de ejemplo")]
        [StringLengthValidation(4000, MinimumLength = 0)]
        public string Serializado { get; set; }
    }

    public class ModificarDetallePedidoLpnRequest
    {
        /// <summary>
        /// Identificación del LPN en el ERP o sistema externo para la línea. 
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RequiredValidation]
        [StringLengthValidation(50, MinimumLength = 1)]
        public string IdLpnExterno { get; set; }

        /// <summary>
        /// Tipo de LPN.
        /// </summary>
        /// <example></example>
        [ApiDtoExample("1")]
        [RequiredValidation]
        [StringLengthValidation(10, MinimumLength = 0)]
        public string TipoLpn { get; set; }

        /// <summary>
        /// Cantidad pedida del producto
        /// </summary>
        /// <example>10</example>
        [ApiDtoExample("10")]
        [RequiredValidation]
        [RangeValidation(12, 3, false)]
        public decimal Cantidad { get; set; }
        [JsonIgnore]
        public List<ModificarDetallePedidoLpnAtributosRequest> Atributos { get; set; }

        public ModificarDetallePedidoLpnRequest()
        {
            Atributos = new List<ModificarDetallePedidoLpnAtributosRequest>();
        }
    }

    public class ModificarDetallePedidoAtributosRequest
    {
        /// <summary>
        /// Cantidad pedida del producto
        /// </summary>
        /// <example>10</example>
        [ApiDtoExample("10")]
        [RequiredValidation]
        [RangeValidation(12, 3, false)]
        public decimal Cantidad { get; set; }

        public List<ModificarDetallePedidoAtributoRequest> Atributos { get; set; }

        public ModificarDetallePedidoAtributosRequest()
        {
            Atributos = new List<ModificarDetallePedidoAtributoRequest>();
        }
    }

    public class ModificarDetallePedidoAtributoRequest
    {
        /// <summary>
        /// Nombre del atributo.
        /// </summary>
        /// <example>COLOR</example>
        [ApiDtoExample("COLOR")]
        [RequiredValidation]
        public string Nombre { get; set; }

        /// <summary>
        /// Valor del atributo.
        /// </summary>
        /// <example>ROJO</example>
        [ApiDtoExample("ROJO")]
        [RequiredValidation]
        [StringLengthValidation(400, MinimumLength = 1)]
        public string Valor { get; set; }

        /// <summary>
        /// Indica si se trata de un atributo de cabezal (C) o de detalle (D).
        /// </summary>
        /// <example>C</example>
        [ApiDtoExample("C")]
        [RequiredValidation]
        [StringLengthValidation(1)]
        public string Tipo { get; set; }
    }

    public class ModificarDetallePedidoLpnAtributosRequest
    {
        /// <summary>
        /// Cantidad pedida del producto
        /// </summary>
        /// <example>10</example>
        [ApiDtoExample("10")]
        [RequiredValidation]
        [RangeValidation(12, 3, false)]
        public decimal Cantidad { get; set; }

        public List<ModificarDetallePedidoLpnAtributoRequest> Atributos { get; set; }

        public ModificarDetallePedidoLpnAtributosRequest()
        {
            Atributos = new List<ModificarDetallePedidoLpnAtributoRequest>();
        }
    }

    public class ModificarDetallePedidoLpnAtributoRequest
    {
        /// <summary>
        /// Nombre del atributo.
        /// </summary>
        /// <example>COLOR</example>
        [ApiDtoExample("COLOR")]
        [RequiredValidation]
        public string Nombre { get; set; }

        /// <summary>
        /// Valor del atributo.
        /// </summary>
        /// <example>ROJO</example>
        [ApiDtoExample("ROJO")]
        [RequiredValidation]
        [StringLengthValidation(400, MinimumLength = 1)]
        public string Valor { get; set; }
    }
}