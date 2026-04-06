using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.General
{
    public class EstanPedidoSalida
    {
        public string IdProcesado { get; set; }
        public long Id { get; set; }
        public string  Registro { get; set; }
        public string Pedido { get; set; }
        public string Cliente { get; set; }
        public string Empresa { get; set; }
        public string Rota { get; set; }
        public string DtLiberarDesde { get; set; }
        public string DtLiberarHasta { get; set; }
        public string DtEntrega { get; set; }
        public string DtEmitido { get; set; }
        public string AddRow { get; set; }
        public string Memo { get; set; }
        public string Memo1 { get; set; }
        public string Origen { get; set; }
        public string CondicionLibreacion { get; set; }
        public string Predio { get; set; }
        public string TpPedido { get; set; }
        public string TpExpedicion { get; set; }
        public string Transportadora { get; set; }
        public string DsAnexo1 { get; set; }
        public string DsAnexo2 { get; set; }
        public string DsAnexo3 { get; set; }
        public string DsAnexo4{ get; set; }
        public string DescUbicacion { get; set; }
        public string Zona{ get; set; }
        public string PuntoEntrega{ get; set; }
        public DateTime? AddRowInterfaz { get; set; }
        public string ModoPedidoNro { get; set; }
        public string VlComparteContenedorPicking { get; set; }
        public string VlSerializado1 { get; set; }
        public string DtGenerico1 { get; set; }
        public string NuGenerico1 { get; set; }
        public string VlGenerico1 { get; set; }
        public string VlComparteContenedorEntrega { get; set; }

    }
}
