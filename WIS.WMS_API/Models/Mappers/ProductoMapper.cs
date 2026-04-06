using System.Collections.Generic;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Domain.General.Enums;
using WIS.Extension;
using WIS.WMS_API.Models.Mappers.Interfaces;

namespace WIS.WMS_API.Models.Mappers
{
    public class ProductoMapper : Mapper, IProductoMapper
    {
        public ProductoMapper()
        {
        }

        public virtual List<Producto> Map(ProductosRequest request)
        {
            List<Producto> models = new List<Producto>();

            foreach (var p in request.Productos)
            {
                Producto model = new Producto(p.ManejoIdentificador, p.AceptaDecimales);
                model.Agrupacion = p.Agrupacion;
                model.Altura = p.Altura;
                model.Ancho = p.Ancho;
                model.Anexo1 = p.Anexo1;
                model.Anexo2 = p.Anexo2;
                model.Anexo3 = p.Anexo3;
                model.Anexo4 = p.Anexo4;
                model.Anexo5 = p.Anexo5;
                model.AvisoAjusteInventario = p.AvisoAjusteInventario;
                model.AyudaColector = p.AyudaColector;
                model.CantidadGenerica = p.CantidadGenerica;
                model.CantidadPadronStock = p.CantidadPadronStock;
                model.CdOrigen = p.CodigoOrigen;
                model.Codigo = p.Codigo?.Trim();
                model.CodigoClase = p.CodigoClase;
                model.CodigoEmpresa = request.Empresa;
                model.CodigoFamilia = p.CodigoFamilia ?? -1;
                model.CodigoMercadologico = p.CodigoMercadologico?.Trim();
                model.CodigoProductoEmpresa = p.CodigoProductoEmpresa;
                model.CodigoRotatividad = p.CodigoRotatividad;
                model.Componente1 = p.Componente1;
                model.Componente2 = p.Componente2;
                model.Conversion = p.Conversion;
                model.DescDifPeso = p.DescDifPeso;
                model.Descripcion = p.Descripcion;
                model.DescripcionDisplay = p.DescripcionDisplay;
                model.DescripcionReducida = p.DescripcionReducida;
                model.DiasDuracion = p.DiasDuracion;
                model.DiasLiberacion = p.DiasLiberacion;
                model.DiasValidez = p.DiasValidez;
                model.Exclusivo = p.Exclusivo;
                model.GrupoConsulta = p.GrupoConsulta;
                model.ModalidadIngresoLote = p.ModalidadIngresoLote;
                model.NAM = p.NAM;
                model.Nivel = p.Nivel;
                model.PesoBruto = p.PesoBruto;
                model.PesoNeto = p.PesoNeto;
                model.PrecioDistribucion = p.PrecioDistribucion;
                model.PrecioEgreso = p.PrecioEgreso;
                model.PrecioIngreso = p.PrecioIngreso;
                model.PrecioSegDistribucion = p.PrecioSegDistribucion;
                model.PrecioSegStock = p.PrecioSegStock;
                model.PrecioStock = p.PrecioStock;
                model.PrecioVenta = p.PrecioVenta;
                model.ProductoUnico = p.ProductoUnico;
                model.Profundidad = p.Profundidad;
                model.Ramo = p.Ramo ?? -1;
                model.SgProducto = p.SgProducto;
                model.Situacion = p.Situacion ?? -1;
                model.StockMaximo = p.StockMaximo;
                model.StockMinimo = p.StockMinimo;
                model.SubBulto = p.SubBulto;
                model.TipoDisplay = p.TipoDisplay;
                model.TipoManejoFecha = p.TipoManejoFecha;
                model.TipoPeso = p.TipoPeso;
                model.UltimoCosto = p.UltimoCosto;
                model.UndEmb = p.UndEmb;
                model.UndMedidaFact = p.UndMedidaFact;
                model.UnidadBulto = p.UnidadBulto ?? -1;
                model.UnidadDistribucion = p.UnidadDistribucion ?? -1;
                model.UnidadMedida = p.UnidadMedida;
                model.VolumenCC = p.VolumenCC;
                model.CodigoBase = p.CodigoBase;
                model.Talle = p.Talle;
                model.Color = p.Color;
                model.Temporada = p.Temporada;
                model.Categoria1 = p.Categoria1;
                model.Categoria2 = p.Categoria2;
                model.VentanaLiberacion = string.IsNullOrEmpty(p.VentanaLiberacion) ? CodigoDominioDb.VentanaPorDefecto : p.VentanaLiberacion;

                models.Add(model);
            }
            return models;
        }
        public virtual ProductoResponse MapToResponse(Producto producto)
        {
            return new ProductoResponse()
            {
                CodigoClase = producto.CodigoClase,
                CodigoEmpresa = producto.CodigoEmpresa,
                Exclusivo = producto.Exclusivo,
                CodigoFamilia = producto.CodigoFamilia,
                GrupoConsulta = producto.GrupoConsulta,
                CodigoMercadologico = producto.CodigoMercadologico,
                NAM = producto.NAM,
                Nivel = producto.Nivel,
                CodOrigen = producto.CdOrigen,
                Codigo = producto.Codigo,
                CodigoProductoEmpresa = producto.CodigoProductoEmpresa,
                ProductoUnico = producto.ProductoUnico,
                Ramo = producto.Ramo,
                CodigoRotatividad = producto.CodigoRotatividad,
                Situacion = producto.Situacion,
                UnidadMedida = producto.UnidadMedida,
                UndMedidaFact = producto.UndMedidaFact,
                UndEmb = producto.UndEmb,
                Anexo1 = producto.Anexo1,
                Anexo2 = producto.Anexo2,
                Anexo3 = producto.Anexo3,
                Anexo4 = producto.Anexo4,
                Anexo5 = producto.Anexo5,
                DescDifPeso = producto.DescDifPeso,
                DescripcionDisplay = producto.DescripcionDisplay,
                AyudaColector = producto.AyudaColector,
                Descripcion = producto.Descripcion,
                DescripcionReducida = producto.DescripcionReducida,
                FechaIngreso = producto.FechaIngreso.ToString(CDateFormats.DATE_ONLY),
                FechaSituacion = producto.FechaSituacion.ToString(CDateFormats.DATE_ONLY),
                FechaModificacion = producto.FechaModificacion.ToString(CDateFormats.DATE_ONLY),
                AceptaDecimales = this.MapBooleanToString(producto.AceptaDecimales),
                Conversion = producto.Conversion,
                Agrupacion = producto.Agrupacion,
                ManejoIdentificador = this.MapManejoIdentificador(producto.ManejoIdentificador),
                Componente1 = producto.Componente1,
                Componente2 = producto.Componente2,
                ModalidadIngresoLote = producto.ModalidadIngresoLote,
                PesoBruto = producto.PesoBruto,
                PesoNeto = producto.PesoNeto,
                DiasDuracion = producto.DiasDuracion,
                DiasValidez = producto.DiasValidez,
                DiasLiberacion = producto.DiasLiberacion,
                StockMaximo = producto.StockMaximo,
                StockMinimo = producto.StockMinimo,
                CantidadGenerica = producto.CantidadGenerica,
                CantidadPadronStock = producto.CantidadPadronStock,
                SubBulto = producto.SubBulto,
                UnidadBulto = producto.UnidadBulto,
                UnidadDistribucion = producto.UnidadDistribucion,
                SgProducto = producto.SgProducto,
                TipoDisplay = producto.TipoDisplay,
                TipoManejoFecha = producto.TipoManejoFecha,
                TipoPeso = producto.TipoPeso,
                Altura = producto.Altura,
                AvisoAjusteInventario = producto.AvisoAjusteInventario,
                VolumenCC = producto.VolumenCC,
                UltimoCosto = producto.UltimoCosto,
                Ancho = producto.Ancho,
                PrecioDistribucion = producto.PrecioDistribucion,
                PrecioEgreso = producto.PrecioEgreso,
                PrecioIngreso = producto.PrecioIngreso,
                PrecioSegDistribucion = producto.PrecioSegDistribucion,
                PrecioSegStock = producto.PrecioSegStock,
                PrecioStock = producto.PrecioStock,
                PrecioVenta = producto.PrecioVenta,
                Profundidad = producto.Profundidad,
                CodigoBase = producto.CodigoBase,
                Talle = producto.Talle,
                Color = producto.Color,
                Temporada = producto.Temporada,
                Categoria1 = producto.Categoria1,
                Categoria2 = producto.Categoria2,
                VentanaLiberacion = producto.VentanaLiberacion,
            };
        }

        public virtual List<ProductoProveedor> Map(ProductosProveedorRequest request)
        {
            List<ProductoProveedor> productos = new List<ProductoProveedor>();

            foreach (var p in request.Productos)
            {
                ProductoProveedor codigo = new ProductoProveedor(p.TipoOperacion, p.TipoAgente, p.CodigoAgente);
                codigo.CodigoProducto = p.CodigoProducto;
                codigo.Empresa = request.Empresa;
                codigo.CodigoExterno = p.CodigoExterno;
                productos.Add(codigo);
            }
            return productos;
        }
        public virtual ProductoProveedorResponse MapToResponse(ProductoProveedor productoProveedor, string tipoAgente, string codigoAgente)
        {
            return new ProductoProveedorResponse()
            {
                CodigoProducto = productoProveedor.CodigoProducto,
                Empresa = productoProveedor.Empresa,
                TipoAgente = tipoAgente,
                CodigoAgente = codigoAgente,
                CodigoExterno = productoProveedor.CodigoExterno
            };
        }

        public virtual string MapManejoIdentificador(ManejoIdentificador manejoIdentificador)
        {
            switch (manejoIdentificador)
            {
                case ManejoIdentificador.Lote: return "L";
                case ManejoIdentificador.Producto: return "P";
                case ManejoIdentificador.Serie: return "S";
            }
            return null;
        }

    }
}
