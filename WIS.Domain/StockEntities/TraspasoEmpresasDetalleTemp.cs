using System;

namespace WIS.Domain.StockEntities
{
    public class TraspasoEmpresasDetalleTemp
    {
        public int NumeroPreparacion { get; set; }
        public long? NroTraspaso { get; set; }
        public string Ubicacion { get; set; }
        public int Empresa { get; set; }
        public string Producto { get; set; }
        public decimal Faixa { get; set; }
        public string Identificador { get; set; }
        public DateTime? Vencimiento { get; set; }
        public decimal Cantidad { get; set; }
        public string ManejoIdentificador { get; set; }
        public string ManejoFecha { get; set; }
        public string AceptaDecimales { get; set; }
        public int EmpresaDest { get; set; }
        public string IdentificadorDest { get; set; }
        public DateTime? VencimientoDest { get; set; }
        public string ProductoDest { get; set; }
        public decimal CantidadDestino { get; set; }
        public string ManejoIdentificadorDest { get; set; }
        public string ManejoFechaDest { get; set; }
        public string AceptaDecimalesDest { get; set; }
        public long IdAjusteStock { get; set; }
        public string Pedido { get; set; }
        public string Cliente { get; set; }
        public string PedidoDest { get; set; }
        public string ClienteDest { get; set; }
        public string UbicacionPicking { get; set; }
        public long? IdDetallePickingLpn { get; set; }
        public int? NroContenedor { get; set; }
        public int? IdLpnDet { get; set; }
        public int? IdLpnDetDest { get; set; }
        public long? NroLpn { get; set; }
        public long NroLpnDest { get; set; }
        public int NumeroSecuencia { get; set; }
        public string Agrupacion { get; set; }
        public int? NroContenedorDestino { get; set; }

        public string NroDocumento { get; set; }
        public string TipoDocumento { get; set; }
    }
}
