using System.Collections.Generic;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Domain.Picking;
using WIS.Domain.Picking.Dtos;
using WIS.WMS_API.Models.Mappers.Interfaces;

namespace WIS.WMS_API.Models.Mappers
{
    public class PreparacionMapper : IPreparacionMapper
    {
        public virtual List<AnularPickingPedidoPendiente> Map(AnularPickingPedidoPendienteRequest request)
        {
            List<AnularPickingPedidoPendiente> detalles = new List<AnularPickingPedidoPendiente>();
            foreach (var detalle in request.Detalles)
            {
                detalles.Add(new AnularPickingPedidoPendiente()
                {
                    TipoAgente = detalle.TipoAgente,
                    CodigoAgente = detalle.CodigoAgente,
                    Empresa = request.Empresa,
                    EstadoPicking = string.IsNullOrEmpty(detalle.EstadoPicking) ? EstadoDetallePreparacion.ESTADO_PREP_PENDIENTE : detalle.EstadoPicking,
                    Pedido = detalle.Pedido,
                    Preparacion = detalle.Preparacion
                });
            }

            return detalles;
        }

        public virtual List<DetallePreparacion> Map(PickingRequest request, int userId)
        {
            var models = new List<DetallePreparacion>();

            var estadoDetalle = string.IsNullOrEmpty(request.EstadoDetalle) ? EstadoDetallePreparacion.ESTADO_PREP_PENDIENTE : request.EstadoDetalle;
            foreach (var r in request.Detalles)
            {
                var contenedor = new Contenedor()
                {
                    Numero = -1,
                    IdExterno = r.IdExternoContenedor,
                    TipoContenedor = r.TipoContenedor,
                    Ubicacion = r.UbicacionContenedor,
                };

                models.Add(new DetallePreparacion()
                {
                    NumeroPreparacion = r.Preparacion,
                    Ubicacion = r.Ubicacion,
                    Empresa = request.Empresa,
                    Producto = r.CodigoProducto,
                    Lote = (!string.IsNullOrEmpty(r.Identificador) ? r.Identificador : ManejoIdentificadorDb.IdentificadorProducto)?.Trim()?.ToUpper(),
                    Faixa = 1,
                    Cantidad = r.Cantidad,
                    CantidadPickeo = r.Cantidad,
                    Estado = estadoDetalle,
                    UsuarioPickeo = userId,

                    Contenedor = contenedor,
                    IdExternoContenedor = r.IdExternoContenedor,
                    TipoContenedor = r.TipoContenedor,

                    Agrupacion = r.Agrupacion,
                    Pedido = r.Pedido,
                    CodigoAgente = r.CodigoAgente,
                    TipoAgente = r.TipoAgente,
                    Carga = r.Carga,
                    ComparteContenedorPicking = r.ComparteContenedorPicking
                });
            }

            return models;
        }

        public virtual List<AnularPickingPedidoPendienteAutomatismo> MapAutomatismo(AnularPickingPedidoPendienteRequest request)
        {
            List<AnularPickingPedidoPendienteAutomatismo> detalles = new List<AnularPickingPedidoPendienteAutomatismo>();
            foreach (var detalle in request.Detalles)
            {
                var colProdAnular = new List<AnularPickingPedidoPendienteDetalle>();

                foreach (var detProd in detalle.ProductosAnular)
                {
                    colProdAnular.Add(new AnularPickingPedidoPendienteDetalle()
                    {
                        CodigoProducto = detProd.CodigoProducto,
                        Identificador = detProd.Identificador,
                        CantidadAnular = detProd.CantidadAnular
                    });
                }

                detalles.Add(new AnularPickingPedidoPendienteAutomatismo()
                {
                    TipoAgente = detalle.TipoAgente,
                    CodigoAgente = detalle.CodigoAgente,
                    Empresa = request.Empresa,
                    EstadoPicking = string.IsNullOrEmpty(detalle.EstadoPicking) ? EstadoDetallePreparacion.ESTADO_PREP_PENDIENTE : detalle.EstadoPicking,
                    Pedido = detalle.Pedido,
                    Preparacion = detalle.Preparacion,
                    Carga = detalle.Carga,
                    ComparteContenedorPicking = detalle.ComparteContenedorPicking,

                    Detalle = colProdAnular
                });
            }

            return detalles;
        }
    }
}
