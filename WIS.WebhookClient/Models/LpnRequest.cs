using System.Collections.Generic;
using WIS.Common.API.Attributes;

namespace WIS.WebhookClient.Models
{
    public class LpnRequest
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

        public List<AtributoRequest> Atributos { get; set; }
        public List<LpnDetalleRequest> Detalles { get; set; }

        public LpnRequest()
        {
            Atributos = new List<AtributoRequest>();
            Detalles = new List<LpnDetalleRequest>();
        }
    }

    public class LpnDetalleRequest
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

        public List<AtributoRequest> Atributos { get; set; }

        public LpnDetalleRequest()
        {
            Atributos = new List<AtributoRequest>();
        }
    }

    public class AtributoRequest
    {
        [ApiDtoExample("COLOR")]
        public string Nombre { get; set; }

        [ApiDtoExample("ROJO")]
        public string Valor { get; set; }
    }
}
