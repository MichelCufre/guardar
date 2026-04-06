using WIS.Common.API.Attributes;

namespace WIS.Domain.General.API.Dtos.Salida
{
    public class InsumoProduccionResponse
	{
        [ApiDtoExample("PR4")]
        public string CodigoProducto { get; set; }

        [ApiDtoExample("L1")]
        public string Identificador { get; set; }

        [ApiDtoExample("10")]
        public decimal? CantidadTeorica { get; set; }

        [ApiDtoExample("10")]
        public decimal? CantidadConsumida { get; set; }

        [ApiDtoExample("1")]
        public string CodigoMotivo { get; set; }

    }
}
