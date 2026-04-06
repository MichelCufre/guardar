using System;
using System.ComponentModel.DataAnnotations;
using WIS.Common.API.Attributes;

namespace WIS.Domain.General.API.Dtos.Salida
{
    public class ProductoProduccionResponse
    {
        [ApiDtoExample("PR4")]
        public string CodigoProducto { get; set; }

        [ApiDtoExample("L1")]
        public string Identificador { get; set; }

        [ApiDtoExample("12/06/2016")]
        public string Vencimiento { get; set; }

        [ApiDtoExample("10")]
        public decimal? CantidadTeorica { get; set; }

        [ApiDtoExample("10")]
        public decimal? CantidadProducida { get; set; }

        [ApiDtoExample("1")]
        public string ModalidadCalculoLote { get; set; }

        [ApiDtoExample("1")]
        public string CodigoMotivo { get; set; }

        [ApiDtoExample("Observaciones")]
        public string Observaciones { get; set; }
    }
}
