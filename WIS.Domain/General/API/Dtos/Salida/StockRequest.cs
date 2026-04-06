using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using WIS.Common.API.Attributes;
using WIS.Domain.Validation;

namespace WIS.Domain.General.API.Dtos.Salida
{
    public class StockRequest
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
        /// Número de página a retornar
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RequiredValidation]
        [RangeValidation(10)]
        public int Pagina { get; set; }

        /// <summary>
        /// Filtros para aplicar a la consulta de stock que afectan directamente a los datos que se retornarán.
        /// </summary>
        public FiltrosStockRequest Filtros { get; set; }

        public class FiltrosStockRequest
        {
            /// <summary>
            /// Código de Ubicación 
            /// </summary>
            /// <example>1AA00000</example>   
            [ApiDtoExample("1AA00000")]
            public string Ubicacion { get; set; }

            /// <summary>
            /// Código de Producto 
            /// </summary>
            /// <example>PR4</example>    
            [ApiDtoExample("PR4")]
            public string Producto { get; set; }

            /// <summary>
            /// Código de clase.
            /// </summary>
            /// <example>GR</example>
            [ApiDtoExample("GR")]
            public string CodigoClase { get; set; }

            /// <summary>
            /// Código de la familia del producto. 
            /// La familia es una clasificación que engloba varios productos.
            /// </summary>
            /// <example>1</example>
            [ApiDtoExample("1")]
            public int? CodigoFamilia { get; set; }

            /// <summary>
            /// Código del ramo del producto. 
            /// El ramo es una clasificación que engloba varios productos.
            /// </summary>
            /// <example>1</example>
            [ApiDtoExample("1")]
            public short? Ramo { get; set; }

            /// <summary>
            /// Tipo de manejo de fecha para los productos. 
            /// Este campo define si un producto es duradero, FIFO o FEFO.
            /// F (FIFO) - D (Duradero) - E (Expirable)
            /// </summary>
            /// <example>D</example>
            [ApiDtoExample("D")]
            public string TipoManejoFecha { get; set; }

            /// <summary>        
            /// Tipo de manejo de identificador de los productos.
            /// P (Producto) - L (Lote) - S (Serie)
            /// </summary>
            /// <example>P</example>
            [ApiDtoExample("P")]
            public string ManejoIdentificador { get; set; }

            /// <summary>
            /// Número de Predio.
            /// </summary>
            /// <example>1</example>
            [ApiDtoExample("1")]
            public string Predio { get; set; }

            /// <summary>
            /// Parametro para determinar si el stock esta averiado y/o en un area de averia.
            /// En caso de no enviar ningun valor se toma false por defecto
            /// </summary>
            /// <example>true</example>
            [ApiDtoExample("true")]
            public bool? Averia { get; set; }

            /// <summary>
            /// Código de agrupador de producto.
            /// </summary>
            /// <example>S/N</example>
            [ApiDtoExample("S/N")]
            public string GrupoConsulta { get; set; }


        }
    }
}
