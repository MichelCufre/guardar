using WIS.Domain.General.Enums;

namespace WIS.Domain.Picking.Dtos
{
    public class DetallePedidoLpnEspecifico
    {
        public string Pedido { get; set; }
        public string Cliente { get; set; }
        public int Empresa { get; set; }
        public string Producto { get; set; }
        public decimal Faixa { get; set; }
        public string Identificador { get; set; }
        public string IdEspecificaIdentificador { get; set; }
        public string TipoLpn { get; set; }
        public string IdExternoLpn { get; set; }
        public decimal Cantidad { get; set; }

        public bool Update { get; set; }
        public long? IdConfiguracion { get; set; }
        public bool Detalle { get; set; }
        public int? UserId { get; set; }

        public bool PageReanOnly { get; set; }
        public ManejoIdentificador ManejoIdentificador { get; set; }

        public virtual string GetLockId(bool lpn)
        {
            if (lpn)
                return $"{this.Pedido}#{this.Cliente}#{this.Empresa}#{this.Producto}#{this.Faixa.ToString("#.##")}#{this.Identificador}#{this.IdEspecificaIdentificador}#{this.TipoLpn}#{this.IdExternoLpn}#{this.IdConfiguracion.Value}";
            else
                return $"{this.Pedido}#{this.Cliente}#{this.Empresa}#{this.Producto}#{this.Faixa.ToString("#.##")}#{this.Identificador}#{this.IdEspecificaIdentificador}#{this.IdConfiguracion.Value}";
        }
    }
}
