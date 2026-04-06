using System.Collections.Generic;
using WIS.Common.API.Attributes;

namespace WIS.Domain.General.API.Dtos.Salida
{
    public class LpnSalidaResponse
    {
        [ApiDtoExample("1")]
        public long Numero { get; set; }

        [ApiDtoExample("1")]
        public string IdExterno { get; set; }

        [ApiDtoExample("PALLET")]
        public string Tipo { get; set; }

        [ApiDtoExample("1")]
        public int Empresa { get; set; }

        [ApiDtoExample("PKG-01")]
        public string IdPacking { get; set; }

        public List<AtributoResponse> Atributos { get; set; }
        public List<LpnSalidaDetalleResponse> Detalles { get; set; }

        public LpnSalidaResponse()
        {
            Atributos = new List<AtributoResponse>();
            Detalles = new List<LpnSalidaDetalleResponse>();
        }
    }

    public class LpnSalidaDetalleResponse
    {
        [ApiDtoExample("1")]
        public int Id { get; set; }

        [ApiDtoExample("PRO-1")]
        public string CodigoProducto { get; set; }

        [ApiDtoExample("1")]
        public decimal Faixa { get; set; }

        [ApiDtoExample("AAA")]
        public string Lote { get; set; }

        [ApiDtoExample("100")]
        public decimal Cantidad { get; set; }

        [ApiDtoExample("10/10/2022")]
        public string Vencimiento { get; set; }

        [ApiDtoExample("1")]
        public string IdLineaSistemaExterno { get; set; }

        public List<AtributoResponse> Atributos { get; set; }

        public LpnSalidaDetalleResponse()
        {
            Atributos = new List<AtributoResponse>();
        }
    }

    public class AtributoResponse
    {
        [ApiDtoExample("COLOR")]
        public string Nombre { get; set; }

        [ApiDtoExample("ROJO")]
        public string Valor { get; set; }
    }
}
