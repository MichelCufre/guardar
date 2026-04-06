using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WIS.Common.API.Attributes;

namespace WIS.Domain.General.API.Dtos.Salida
{
    public class PedidoSalidaResponse
    {
        /// <summary>
        /// Código de la empresa a la que pertenece el pedido
        /// </summary>
        /// <example>10</example>
        [ApiDtoExample("10")]
        [Required]
        public int Empresa { get; set; } // CD_EMPRESA

        /// <summary>
        /// Tipo de agente 
        /// PRO (Proveedor) - CLI (Cliente)
        /// </summary>
        /// <example>CLI</example>
        [ApiDtoExample("CLI")]
        [Required]
        public string TipoAgente { get; set; } // TP_AGENTE

        /// <summary>
        /// Código del agente para el cual se realiza el pedido
        /// </summary>
        /// <example>LOCAL11</example>
        [ApiDtoExample("LOCAL11")]
        [Required]
        public string CodigoAgente { get; set; } // CD_AGENTE

        /// <summary>
        /// Valor alfanumérico que representa un pedido en el sistema, conforma un identificador único junto al código de agente, tipo de agente y la empresa a la que pertenece.
        /// </summary>
        /// <example>PED100</example>
        [ApiDtoExample("PED100")]
        [Required]
        public string Pedido { get; set; } // NU_PEDIDO

        /// <summary>
        /// Código de origen
        /// Este código sirve para identificar donde se creó el pedido.
        /// Valores posible:
        /// PRE100: Código de la pantalla de pedidos del panel web.
        /// API: Api de entrada.
        /// </summary>
        /// <example>API</example>
        [ApiDtoExample("API")]
        public string CodigoOrigen { get; set; } // CD_ORIGEN

        /// <summary>
        /// Información adicional
        /// </summary>
        /// <example></example>
        public string Memo { get; set; } // DS_MEMO

        /// <summary>
        /// Información adicional
        /// </summary>
        /// <example></example>
        public string Memo1 { get; set; } // DS_MEMO_1

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
        public string CondicionLiberacion { get; set; } // CD_CONDICION_LIBERACION

        /// <summary>
        /// Número de predio.
        /// Especifica el predio del depósito donde se realizó el pedido.
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        public string Predio { get; set; } // NU_PREDIO

        /// <summary>
        /// Tipo de pedido
        /// Especifica el tipo de pedido el cual está directamente relacionado y tiene que ser compatible con el tipo de expedición.
        /// </summary>
        /// <example>URG</example>
        [ApiDtoExample("URG")]
        public string TipoPedido { get; set; } // TP_PEDIDO

        /// <summary>
        /// Tipo de expedición
        /// Indica el tipo de expedición para el pedido, regula el flujo del pedido especificando tareas adicionales para la expedición (ej. facturación, expediciones con packing, normales, etc.)
        /// Valores predeterminados:
        /// -PRC: PROCESO PRODUCCION
        /// -WIS: FACTURABLES
        /// -WSF: NO FACTURABLES
        /// </summary>
        /// <example>NORMAL</example>
        [ApiDtoExample("NORMAL")]
        public string TipoExpedicion { get; set; } // TP_EXPEDICION

        /// <summary>
        /// Destino de la expedición, información clave para el módulo de tracking.
        /// </summary>
        /// <example></example>
        public string Direccion { get; set; } // DS_ENDERECO

        /// <summary>
        /// Código de punto de entrega
        /// Punto de Entrega o código único de entrega para el cliente. 
        /// Generalmente es asignado automáticamente cuando Tracking está activo y georreferencia la dirección de entrega, asignándole un punto de entrega único.
        /// </summary>
        /// <example></example>
        public string PuntoEntrega { get; set; } // CD_PUNTO_ENTREGA

        /// <summary>
        /// Serializado
        /// En este campo se puede enviar información serializada que sea útil para personalizar el funcionamiento.
        /// Por defecto se retornará el mismo valor con el cual se recibió.
        /// </summary>
        /// <example></example>
        public string Serializado { get; set; } // VL_SERIALIZADO_1

        public List<DetallePedidoSalidaResponse> Detalles { get; set; }

        public PedidoSalidaResponse()
        {
            Detalles = new List<DetallePedidoSalidaResponse>();
        }
    }

    public class DetallePedidoSalidaResponse
    {
        /// <summary>
        /// Código del producto
        /// </summary>
        /// <example>PR1</example>
        [ApiDtoExample("PR1")]
        [Required]
        public string Producto { get; set; } // CD_PRODUTO

        /// <summary>
        /// Lote o número que identifica al producto.
        /// </summary>
        /// <example>LOTEAAA</example>
        [ApiDtoExample("LOTEAAA")]
        public string Identificador { get; set; } // NU_IDENTIFICADOR

        /// <summary>
        /// Especifica identificador
        /// Se carga automáticamente dependiendo si se detalla el identificador al momento de crear el pedido.
        /// En caso de mandar un identificador, el valor de este campo será S de lo contrario N.
        /// S (Sí) - N (No) 
        /// </summary>
        /// <example>S</example>
        [ApiDtoExample("S")]
        public string EspecificaIdentificador { get; set; } // ID_ESPECIFICA_IDENTIFICADOR

        /// <summary>
        /// Cantidad de producto para el pedido. En caso de confirmación de mercadería preparada, refiere a la cantidad preparada. En caso de cierre de egreso, refiere a la cantidad expedida en el camión.
        /// </summary>
        /// <example>100</example>
        [ApiDtoExample("100")]
        [Required]
        public decimal CantidadProducto { get; set; } // QT_PRODUTO

        /// <summary>
        /// Información adicional
        /// </summary>
        /// <example></example>
        public string Memo { get; set; } // DS_MEMO

        /// <summary>
        /// Serializado
        /// En este campo se puede enviar información serializada que sea útil para personalizar el funcionamiento.
        /// Por defecto se retornará el mismo valor con el cual se recibió.
        /// </summary>
        /// <example></example>
        public string Serializado { get; set; } // VL_SERIALIZADO_1

        public List<DetallePedidoSalidaDuplicadoResponse> Duplicados { get; set; }

        public DetallePedidoSalidaResponse()
        {
            Duplicados = new List<DetallePedidoSalidaDuplicadoResponse>();
        }
    }

    public class DetallePedidoSalidaDuplicadoResponse
    {
        /// <summary>
        /// Identificación en el ERP o sistema externo para la línea del pedido. 
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [Required]
        public string IdLineaSistemaExterno { get; set; }

        /// <summary>
        /// Tipo de línea. 
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        public string TipoLinea { get; set; }

        /// <summary>
        /// Cantidad de producto para el pedido. En caso de confirmación de mercadería preparada, refiere a la cantidad preparada. En caso de cierre de egreso, refiere a la cantidad expedida en el camión.
        /// </summary>
        /// <example>100</example>
        [ApiDtoExample("100")]
        public decimal CantidadProducto { get; set; }

        /// <summary>
        /// Serializado
        /// En este campo se puede enviar información serializada que sea útil para personalizar el funcionamiento.
        /// Por defecto se retornará el mismo valor con el cual se recibió.
        /// </summary>
        /// <example></example>
        public string Serializado { get; set; }
    }
}
