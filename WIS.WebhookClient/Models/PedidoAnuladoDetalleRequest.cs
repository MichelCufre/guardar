using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WIS.Common.API.Attributes;

namespace WIS.WebhookClient.Models
{
    public class PedidoAnuladoDetalleRequest
    {
        /// <summary>
        /// Código del producto
        /// </summary>
        /// <example>PROD-01</example>
        [ApiDtoExample("PROD-01")]
        [Required]
        public string Producto { get; set; } // CD_PRODUTO

        /// <summary>
        /// Identificador del producto (serie o lote)
        /// </summary>
        /// <example>*</example>
        [ApiDtoExample("*")]
        [Required]
        public string Identificador { get; set; } // NU_IDENTIFICADOR

        /// <summary>
        /// Código de embalaje
        /// </summary>
        /// <example>1</example>
        public decimal Embalaje { get; set; } // CD_FAIXA

        /// <summary>
        /// Cantidad anulada
        /// </summary>
        /// <example></example>
        [ApiDtoExample("1")]
        public decimal? CantidadAnulada { get; set; } // QT_ANULADO

        /// <summary>
        /// Código del funcionario
        /// </summary>
        /// <example></example>
        [ApiDtoExample("005")]
        public int? Funcionario { get; set; } // CD_FUNCIONARIO

        /// <summary>
        /// Información adicional
        /// </summary>
        /// <example></example>
        public string Motivo { get; set; } // DS_MOTIVO

        /// <summary>
        /// Código de la aplicación
        /// </summary>
        /// <example></example>
        public string Aplicacion { get; set; } // CD_APLICACAO

        /// <summary>
        /// Fecha de alta de registro
        /// </summary>
        /// <example></example>
        public string FechaAlta { get; set; } // DT_ADDROW

        /// <summary>
        /// Id. Específica Identificador
        /// </summary>
        /// <example></example>
        public string EspecificaIdentificador { get; set; } // ID_ESPECIFICA_IDENTIFICADOR

        public List<PedidoDetalleDuplicadoRequest> Duplicados { get; set; }

        public List<DetallePedidoLpnAnuladoRequest> Lpns { get; set; }

        public List<DetallePedidoAtributoAnuladoRequest> Atributos { get; set; }

        public PedidoAnuladoDetalleRequest()
        {
            Duplicados = new List<PedidoDetalleDuplicadoRequest>();
            Lpns = new List<DetallePedidoLpnAnuladoRequest>();
            Atributos = new List<DetallePedidoAtributoAnuladoRequest>();
        }
    }

    public class DetallePedidoLpnAnuladoRequest
    {
        [ApiDtoExample("1")]
        public string IdExterno { get; set; } // ID_LPN_EXTERNO

        [ApiDtoExample("PALLET")]
        public string Tipo { get; set; } // TP_LPN_TIPO

        [ApiDtoExample("10")]
        public decimal CantidadAnulada { get; set; }

        //public List<DetallePedidoAtributoAnuladoRequest> DetallesAtributos { get; set; }

        public DetallePedidoLpnAnuladoRequest()
        {
            //DetallesAtributos = new List<DetallePedidoAtributoAnuladoRequest>();
        }
    }

    public class DetallePedidoAtributoAnuladoRequest
    {
        [ApiDtoExample("10")]
        public decimal CantidadAnulada { get; set; }

        public List<AtributoPedidoAnuladoRequest> Atributos { get; set; }

        public DetallePedidoAtributoAnuladoRequest()
        {
            Atributos = new List<AtributoPedidoAnuladoRequest>();
        }
    }

    public class AtributoPedidoAnuladoRequest
    {
        [ApiDtoExample("COLOR")]
        public string Nombre { get; set; }
        [ApiDtoExample("ROJO")]
        public string Valor { get; set; }
        [ApiDtoExample("C")]
        public string Tipo { get; set; }
    }
}
