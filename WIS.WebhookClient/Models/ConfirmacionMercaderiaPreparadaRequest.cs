using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WIS.Common.API.Attributes;

namespace WIS.WebhookClient.Models
{
    public class ConfirmacionMercaderiaPreparadaRequest
    {
        /// <summary>
        /// Código del camión
        /// </summary>
        /// <example>1111</example>
        [ApiDtoExample("1111")]
        [Required]
        public int CodigoCamion { get; set; } // CD_CAMION

        /// <summary>
        ///  Descripción del camión
        /// </summary>
        /// <example>TEXTO LIBRE</example>
        [ApiDtoExample("TEXTO LIBRE")]
        public string DescripcionCamion { get; set; } // DS_CAMION

        /// <summary>
        /// Puerta de expedición
        /// Número de puerta donde se está realizando el egreso.
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        public short? PuertaExpedicion { get; set; } // CD_PORTA

        /// <summary>
        /// Ruta
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        public int? Ruta { get; set; } // CD_ROTA

        /// <summary>
        /// Código del vehículo
        /// </summary>
        /// <example></example>
        [ApiDtoExample("1")]
        public int? Vehiculo { get; set; } // CD_VEICULO


        /// <summary>
        ///  Matrícula del camión
        /// </summary>
        /// <example></example>
        public string MatriculaCamion { get; set; } // CD_PLACA_CARRO

        /// <summary>
        /// Código de la transportadora
        /// </summary>
        /// <example>0</example>
        [ApiDtoExample("0")]
        public int? Transportadora { get; set; } // CD_TRANSPORTADORA

        /// <summary>
        /// Fecha de facturación
        /// </summary>
        /// <example>10/06/2016 12:25</example>
        [ApiDtoExample("10/06/2016 12:25")]
        public string FechaFacturacion { get; set; } // DT_FACTURACION

        /// <summary>
        /// Número de predio.
        /// Este valor se notifica en el cabezal del camión y del pedido. 
        /// Como referencia debe tomarse siempre el del camión.
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        public string Predio { get; set; } // NU_PREDIO

        public List<PedidoRequest> Pedidos { get; set; }

        public List<ContenedorRequest> Contenedores { get; set; }

        public List<LpnRequest> Lpns { get; set; }

        public ConfirmacionMercaderiaPreparadaRequest() 
        {
            Pedidos = new List<PedidoRequest>();
            Contenedores = new List<ContenedorRequest>();
            Lpns = new List<LpnRequest>();
        }
    }
}
