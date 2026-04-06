using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WIS.Common.API.Attributes;
using WIS.Domain.General.API.Dtos.Salida;

namespace WIS.Domain.General.API.Dtos.Salida
{
    public class PedidosAnuladosResponse
    {
        public List<PedidoAnuladoResponse> PedidosAnulados { get; set; }

        public PedidosAnuladosResponse()
        {
            PedidosAnulados = new List<PedidoAnuladoResponse>();
        }
    }

    public class PedidoAnuladoResponse
    {
        /// <summary>
        /// Número de pedido
        /// </summary>
        /// <example></example>
        public string Pedido { get; set; } // NU_PEDIDO

        /// <summary>
        /// Código de la empresa
        /// </summary>
        /// <example></example>
        public int? Empresa { get; set; } // CD_EMPRESA

        /// <summary>
        /// Código del agente
        /// </summary>
        /// <example>LOCAL11</example>
        [ApiDtoExample("LOCAL11")]
        public string CodigoAgente { get; set; } // CD_AGENTE

        /// <summary>
        /// Tipo de agente 
        /// PRO (Proveedor) - CLI (Cliente)
        /// </summary>
        /// <example>CLI</example>
        [ApiDtoExample("CLI")]
        public string TipoAgente { get; set; } // TP_AGENTE

        public List<PedidoAnuladoDetalleResponse> Detalles { get; set; }

        public PedidoAnuladoResponse()
        {
            Detalles = new List<PedidoAnuladoDetalleResponse>();
        }
    }

    public class PedidoAnuladoDetalleResponse
    {
        /// <summary>
        /// Código del producto
        /// </summary>
        /// <example>PROD-01</example>
        [ApiDtoExample("PED-01")]

        public string Producto { get; set; } // CD_PRODUTO

        /// <summary>
        /// Identificador del producto (serie o lote)
        /// </summary>
        /// <example>*</example>
        [ApiDtoExample("*")]

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
        public decimal? CantidadAnulada { get; set; } // QT_ANULADO

        /// <summary>
        /// Código del funcionario
        /// </summary>
        /// <example></example>
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

        public List<DetallePedidoSalidaDuplicadoResponse> Duplicados { get; set; }

        public List<DetallePedidoLpnAnuladoResponse> Lpns { get; set; }

        public List<DetallePedidoAtributoAnuladoResponse> Atributos { get; set; }

        public PedidoAnuladoDetalleResponse()
        {
            Duplicados = new List<DetallePedidoSalidaDuplicadoResponse>();
            Lpns = new List<DetallePedidoLpnAnuladoResponse>();
            Atributos = new List<DetallePedidoAtributoAnuladoResponse>();
        }
    }

    public class DetallePedidoLpnAnuladoResponse
    {
        [ApiDtoExample("1")]
        public string IdExterno { get; set; } // ID_LPN_EXTERNO
        [ApiDtoExample("PALLET")]
        public string Tipo { get; set; } // TP_LPN_TIPO
        [ApiDtoExample("10")]
        public decimal CantidadAnulada { get; set; } // QT_ANULADA

        //public List<DetallePedidoAtributoAnuladoResponse> DetallesAtributos { get; set; }

        public DetallePedidoLpnAnuladoResponse()
        {
            //DetallesAtributos = new List<DetallePedidoAtributoAnuladoResponse>();
        }
    }

    public class DetallePedidoAtributoAnuladoResponse
    {
        [ApiDtoExample("10")]
        public decimal CantidadAnulada { get; set; } // QT_ANULADA

        public List<AtributoPedidoAnuladoResponse> Atributos { get; set; }

        public DetallePedidoAtributoAnuladoResponse()
        {
            Atributos = new List<AtributoPedidoAnuladoResponse>();
        }
    }

    public class AtributoPedidoAnuladoResponse
    {
        [ApiDtoExample("COLOR")]
        public string Nombre { get; set; }
        [ApiDtoExample("ROJO")]
        public string Valor { get; set; }
        [ApiDtoExample("C")]
        public string Tipo { get; set; }
    }

    public class PedidoAnuladoDetalleAuxResponse
    {
        /// <summary>
        /// Código del producto
        /// </summary>
        /// <example>PROD-01</example>
        [ApiDtoExample("PED-01")]

        public string Producto { get; set; } // CD_PRODUTO

        /// <summary>
        /// Identificador del producto (serie o lote)
        /// </summary>
        /// <example>*</example>
        [ApiDtoExample("*")]

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
        public decimal? CantidadAnulada { get; set; } // QT_ANULADO

        /// <summary>
        /// Código del funcionario
        /// </summary>
        /// <example></example>
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

        public List<DetallePedidoSalidaDuplicadoResponse> Duplicados { get; set; }

        public List<DetallePedidoLpnAnuladoResponse> Lpns { get; set; }

        public List<DetallePedidoAtributoAnuladoResponse> Atributos { get; set; }

        public List<long> IdAnulaciones { get; set; }

        public PedidoAnuladoDetalleAuxResponse()
        {
            Duplicados = new List<DetallePedidoSalidaDuplicadoResponse>();
            Lpns = new List<DetallePedidoLpnAnuladoResponse>();
            Atributos = new List<DetallePedidoAtributoAnuladoResponse>();
            IdAnulaciones = new List<long>();
        }

        public virtual PedidoAnuladoDetalleResponse GetDetalleFinal()
        {
            return new PedidoAnuladoDetalleResponse()
            {
                Producto = this.Producto,
                Identificador = this.Identificador,
                Embalaje = this.Embalaje,
                CantidadAnulada = this.CantidadAnulada,
                Funcionario = this.Funcionario,
                Motivo = this.Motivo,
                Aplicacion = this.Aplicacion,
                FechaAlta = this.FechaAlta,
                EspecificaIdentificador = this.EspecificaIdentificador,
                Duplicados = this.Duplicados,
                Lpns = this.Lpns,
                Atributos = this.Atributos,
            };
        }
    }
}
