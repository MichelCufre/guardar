using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WIS.Common.API.Attributes;

namespace WIS.Domain.General.API.Dtos.Salida
{
    public class AjustesResponse
    {
        public List<AjusteResponse> Ajustes { get; set; }

        public AjustesResponse()
        {
            Ajustes = new List<AjusteResponse>();
        }
    }

    public class AjusteResponse 
    {
        /// <summary>
        /// Módulo de WIS que generó el ajuste
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        public string Aplicacion { get; set; } // CD_APLICACAO

        /// <summary>
        /// Código de la empresa
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [Required]
        public int Empresa { get; set; } // CD_EMPRESA

        /// <summary>
        /// Ubicación de WIS correspondiente al ajuste
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        public string Ubicacion { get; set; } // CD_ENDERECO

        /// <summary>
        /// Usuario de WIS que realizó el ajuste
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        public int? Funcionario { get; set; } // CD_FUNCIONARIO

        /// <summary>
        /// Tipo de ajuste
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        public string TipoAjuste { get; set; } // TP_AJUSTE

        /// <summary>
        /// Código de motivo de ajuste
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [Required]
        public string Motivo { get; set; } // CD_MOTIVO_AJUSTE

        /// <summary>
        /// Descripción de motivo
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        public string DescripcionMotivo { get; set; } // DS_MOTIVO

        /// <summary>
        /// Fecha de motivo
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        public string FechaMotivo { get; set; } // DT_MOTIVO

        /// <summary>
        /// Código del producto
        /// </summary>
        /// <example>PROD-1</example>
        [ApiDtoExample("PROD-1")]
        [Required]
        public string Producto { get; set; } // CD_PRODUTO

        /// <summary>
        /// Código faixa
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        public decimal Faixa { get; set; } // CD_FAIXA

        /// <summary>
        /// Identificador del producto (Serie o lote)
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [Required]
        public string Identificador { get; set; } // NU_IDENTIFICADOR

        /// <summary>
        /// Fecha de realización
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [Required]
        public string FechaRealizacion { get; set; } // DT_REALIZADO

        /// <summary>
        /// Fecha de última modificación
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        public string FechaUltimaModificacion { get; set; } // DT_UPDROW

        /// <summary>
        /// Indica si el ajuste fue realizado sobre una ubicación de avería
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        public string AreaAveria { get; set; } // ID_AREA_AVERIA

        /// <summary>
        /// Número de ajuste de stock en WIS
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [Required]
        public int NumeroAjusteStock { get; set; } // NU_AJUSTE_STOCK

        /// <summary>
        /// Número de interfaz asociada
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        public long? InterfazEjecucion { get; set; } // NU_INTERFAZ_EJECUCION

        /// <summary>
        /// Fecha de vencimiento del producto
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        public string FechaVencimiento { get; set; } // DT_FABRICACAO

        /// <summary>
        /// Número de predio
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        public string Predio { get; set; } // NU_PREDIO

        /// <summary>
        /// Cantidad del movimiento - / +
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        public decimal? CantidadMovimiento { get; set; } // QT_MOVIMIENTO

        /// <summary>
        /// Serializado
        /// </summary>
        /// <example></example>
        public string Serializado { get; set; }

        /// <summary>
        /// Número de Log de inventario
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        public int? NroLogInventario { get; set; }

        /// <summary>
        /// Atributos del LPN asociado al ajuste
        /// </summary>
        /// <example></example>
        public string Atributos { get; set; }

    }
}
