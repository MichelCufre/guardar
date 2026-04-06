using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WIS.Common.API.Attributes;

namespace WIS.Domain.General.API.Dtos.Salida
{
    public class FacturacionResponse
    {
        /// <summary>
        /// Código del camión
        /// Principal identificador del egreso.
        /// </summary>
        /// <example>1111</example>
        [ApiDtoExample("1111")]
        [Required]
        public int CodigoCamion { get; set; } // CD_CAMION

        /// <summary>
        /// Descripción del camión
        /// Texto que describe el egreso, el ingresar un valor identificativo sería de ayuda al funcionario en la operativa
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
        /// Indica el código de ruta del egreso, detallando a cuál recorrido pertenece la entrega.
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        public int? Ruta { get; set; } // CD_ROTA

        /// <summary>
        /// Código del vehículo
        /// Número que identifica el vehículo usado para el egreso.
        /// </summary>
        /// <example></example>
        public int? Vehiculo { get; set; } // CD_VEICULO

        /// <summary>
        ///  Matrícula del vehículo utilizado para el egreso
        /// </summary>
        /// <example></example>
        public string MatriculaCamion { get; set; } // CD_PLACA_CARRO

        /// <summary>
        /// Código de la empresa transportista relacionada al egreso
        /// </summary>
        /// <example>0</example>
        [ApiDtoExample("0")]
        public int? Transportadora { get; set; } // CD_TRANSPORTADORA

        /// <summary>
        /// Fecha de facturación
        /// Fecha exacta de cuando se facturó el egreso
        /// </summary>
        /// <example>10/06/2016 12:25</example>
        [ApiDtoExample("10/06/2016 12:25")]
        public string FechaFacturacion { get; set; } // DT_FACTURACION

        /// <summary>
        /// Número de Predio.
        /// Especifica el predio del depósito donde se realizó el egreso.
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        public string Predio { get; set; } // NU_PREDIO

        public List<PedidoSalidaResponse> Pedidos { get; set; }
        public List<ContenedorResponse> Contenedores { get; set; }
        public List<LpnSalidaResponse> Lpns { get; set; }

        public FacturacionResponse()
        {
            Pedidos = new List<PedidoSalidaResponse>();
            Contenedores = new List<ContenedorResponse>();
            Lpns = new List<LpnSalidaResponse>();
        }
    }
}
