using System.Linq;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.MiddlewareAPI.Dtos;

namespace WIS.MiddlewareAPI.Mappers
{
    // Traduce los DTOs del ERP del cliente a los Request que espera la WMS API.

    public static class ErpMapper
    {
        public static ProductosRequest ToWms(ErpProductosRequest erp)
        {
            return new ProductosRequest
            {
                Empresa = erp.Empresa,
                DsReferencia = erp.Referencia,
                Productos = erp.Items.Select(p => new ProductoRequest
                {
                    Codigo = p.CodigoArticulo,
                    Descripcion = p.NombreArticulo,
                    UnidadMedida = p.Unidad,
                    TipoManejoFecha = p.TipoFecha,
                    Situacion = p.Estado,
                    ManejoIdentificador = p.ManejoLote,
                    CodigoProductoEmpresa = p.CodigoEmpresa,
                    DescripcionReducida = p.DescCorta,
                }).ToList()
            };
        }

        public static AgentesRequest ToWms(ErpAgentesRequest erp)
        {
            return new AgentesRequest
            {
                Empresa = erp.Empresa,
                DsReferencia = erp.Referencia,
                Agentes = erp.Items.Select(a => new AgenteRequest
                {
                    CodigoAgente = a.Codigo,
                    Tipo = a.TipoCliente,
                    Descripcion = a.Nombre,
                    Estado = a.EstadoCliente,
                    Ruta = a.NumeroRuta,
                    Direccion = a.DireccionEntrega,
                    TelefonoPrincipal = a.Telefono,
                    Email = a.CorreoElectronico,
                }).ToList()
            };
        }

        public static CodigosBarrasRequest ToWms(ErpCodigosBarrasRequest erp)
        {
            return new CodigosBarrasRequest
            {
                Empresa = erp.Empresa,
                DsReferencia = erp.Referencia,
                CodigosDeBarras = erp.Items.Select(c => new CodigoBarraRequest
                {
                    Codigo = c.CodigoBarra,
                    Producto = c.CodigoArticulo,
                    TipoCodigo = c.TipoCodigo,
                    PrioridadUso = c.Prioridad,
                    CantidadEmbalaje = c.Cantidad,
                    TipoOperacion = c.Operacion,
                }).ToList()
            };
        }

        public static PedidosRequest ToWms(ErpPedidosRequest erp)
        {
            return new PedidosRequest
            {
                Empresa = erp.Empresa,
                DsReferencia = erp.Referencia,
                Pedidos = erp.Items.Select(p => new PedidoRequest
                {
                    NroPedido = p.NumeroPedido,
                    CodigoAgente = p.CodigoCliente,
                    TipoAgente = p.TipoCliente,
                    TipoPedido = p.TipoMovimiento,
                    TipoExpedicion = p.TipoDespacho,
                    Predio = p.Deposito,
                    CondicionLiberacion = p.Condicion,
                    Detalles = p.Lineas.Select(d => new DetallePedidoRequest
                    {
                        CodigoProducto = d.CodigoArticulo,
                        Cantidad = d.Cantidad,
                        Identificador = d.Lote,
                    }).ToList()
                }).ToList()
            };
        }
    }
}
