using System.Collections.Generic;

namespace WIS.MiddlewareAPI.Dtos
{
    // Estos DTOs representan la estructura JSON que envia el ERP
    // del cliente.

    public class ErpProductosRequest
    {
        public int Empresa { get; set; }
        public string? Referencia { get; set; }
        public List<ErpProductoItem> Items { get; set; } = new List<ErpProductoItem>();
    }

    public class ErpProductoItem
    {
        public string? CodigoArticulo { get; set; }
        public string? NombreArticulo { get; set; }
        public string? Unidad { get; set; }
        public string? TipoFecha { get; set; }
        public short? Estado { get; set; }
        public string? ManejoLote { get; set; }
        public string? CodigoEmpresa { get; set; }
        public string? DescCorta { get; set; }
    }

    public class ErpAgentesRequest
    {
        public int Empresa { get; set; }
        public string? Referencia { get; set; }
        public List<ErpAgenteItem> Items { get; set; } = new List<ErpAgenteItem>();
    }

    public class ErpAgenteItem
    {
        public string? Codigo { get; set; }
        public string? TipoCliente { get; set; }
        public string? Nombre { get; set; }
        public short? EstadoCliente { get; set; }
        public short? NumeroRuta { get; set; }
        public string? DireccionEntrega { get; set; }
        public string? Telefono { get; set; }
        public string? CorreoElectronico { get; set; }
    }

    public class ErpCodigosBarrasRequest
    {
        public int Empresa { get; set; }
        public string? Referencia { get; set; }
        public List<ErpCodigoBarraItem> Items { get; set; } = new List<ErpCodigoBarraItem>();
    }

    public class ErpCodigoBarraItem
    {
        public string? CodigoBarra { get; set; }
        public string? CodigoArticulo { get; set; }
        public int? TipoCodigo { get; set; }
        public short? Prioridad { get; set; }
        public decimal? Cantidad { get; set; }
        public string? Operacion { get; set; }
    }

    public class ErpPedidosRequest
    {
        public int Empresa { get; set; }
        public string? Referencia { get; set; }
        public List<ErpPedidoItem> Items { get; set; } = new List<ErpPedidoItem>();
    }

    public class ErpPedidoItem
    {
        public string? NumeroPedido { get; set; }
        public string? CodigoCliente { get; set; }
        public string? TipoCliente { get; set; }
        public string? TipoMovimiento { get; set; }
        public string? TipoDespacho { get; set; }
        public string? Deposito { get; set; }
        public string? Condicion { get; set; }
        public List<ErpPedidoDetalle> Lineas { get; set; } = new List<ErpPedidoDetalle>();
    }

    public class ErpPedidoDetalle
    {
        public string? CodigoArticulo { get; set; }
        public decimal Cantidad { get; set; }
        public string? Lote { get; set; }
    }
}
