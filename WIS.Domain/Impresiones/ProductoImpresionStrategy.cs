using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.General;
using WIS.Domain.Impresiones.Utils;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.Impresiones
{
    public class ProductoImpresionStrategy : IImpresionDetalleBuildingStrategy
    {
        protected readonly IEstiloTemplate _estilo;
        protected readonly List<Producto> _productos;
        protected readonly string _tipoCodigoBarra;
        protected readonly IUnitOfWork _uow;
        protected readonly ProductoMapper _productoMapper;
        protected readonly ImpresionUtils _utiltyImpresion;
        protected readonly IPrintingService _printingService;

        public ProductoImpresionStrategy(
            IEstiloTemplate estilo,
            IUnitOfWork uow,
            IPrintingService printingService,
            string tipoCodigoBarra)
        {
            this._estilo          = estilo;
            this._uow             = uow;
            this._productoMapper  = new ProductoMapper ();
            this._utiltyImpresion = new ImpresionUtils ();
            this._printingService = printingService;
            this._tipoCodigoBarra = tipoCodigoBarra;
        }

        public ProductoImpresionStrategy(
            IEstiloTemplate estilo,
            string tipoCodigoBarras,
            List<Producto> productos,
            IUnitOfWork uow,
            IPrintingService printingService)
        {
            this._estilo = estilo;
            this._productos = productos;
            this._tipoCodigoBarra = tipoCodigoBarras;
            this._uow = uow;
            this._productoMapper = new ProductoMapper();
            this._utiltyImpresion = new ImpresionUtils();
            this._printingService = printingService;
        }

        public virtual List<DetalleImpresion> Generar(Impresora impresora)
        {
            var template = this._estilo.GetTemplate(impresora);

            var detalles = new List<DetalleImpresion>();

            foreach (var producto in this._productos.OrderBy(s => s.Codigo))
			{
                var codigoBarrasProducto = _uow.CodigoBarrasRepository.GetCodigoBarras(producto.Codigo, producto.CodigoEmpresa, int.Parse(_tipoCodigoBarra));
                var claves = GetDiccionarioInformacion(producto, codigoBarrasProducto);

                detalles.Add(new DetalleImpresion
                {
                    Contenido = template.Parse(claves),
                    Estado = _printingService.GetEstadoInicial(),
                    FechaProcesado = DateTime.Now,
                });
            }

            return detalles;
        }

        public virtual Dictionary<string, string> GetDiccionarioInformacion(Producto producto, CodigoBarras codigoBarra)
        {
            var claves = new Dictionary<string, string>()
            {
                { "T_PRODUTO.CD_PRODUTO_EMPRESA", producto.CodigoProductoEmpresa},
                { "T_PRODUTO.CD_PRODUTO" , producto.Codigo },
                { "T_PRODUTO.DS_PRODUTO",producto.Descripcion},
                { "T_PRODUTO.DS_ANEXO1 " , producto.Anexo1 },
                { "T_PRODUTO.CD_EMPRESA", producto.Empresa?.Id.ToString() },
                { "T_PRODUTO.CD_NAM", producto.NAM},
                { "T_PRODUTO.CD_MERCADOLOGICO", producto.CodigoMercadologico},
                { "T_PRODUTO.CD_UNIDADE_MEDIDA" , producto.UnidadMedida},
                { "T_PRODUTO.CD_FAMILIA_PRODUTO" , producto.CodigoFamilia.ToString()},
                { "T_PRODUTO.CD_ROTATIVIDADE" , producto.CodigoRotatividad?.ToString()},
                { "T_PRODUTO.CD_CLASSE" , producto.CodigoClase},
                { "T_PRODUTO.QT_ESTOQUE_MINIMO" ,producto.StockMinimo?.ToString()},
                { "T_PRODUTO.QT_ESTOQUE_MAXIMO" ,producto.StockMaximo?.ToString()},
                { "T_PRODUTO.PS_LIQUIDO" ,producto.PesoNeto?.ToString()},
                { "T_PRODUTO.PS_BRUTO" , producto.PesoBruto?.ToString()},
                { "T_PRODUTO.VL_CUBAGEM" , producto.VolumenCC?.ToString()},
                { "T_PRODUTO.VL_PRECO_VENDA" , producto.PrecioVenta?.ToString()},
                { "T_PRODUTO.VL_CUSTO_ULT_ENT" ,producto.UltimoCosto?.ToString()},
                { "T_PRODUTO.DS_REDUZIDA" , producto.DescripcionReducida},
                { "T_PRODUTO.CD_SITUACAO" , producto.Situacion.ToString()},
                { "T_PRODUTO.DT_SITUACAO" , producto.FechaSituacion?.ToString()},
                { "T_PRODUTO.QT_DIAS_VALIDADE" , producto.DiasValidez?.ToString()},
                { "T_PRODUTO.QT_DIAS_DURACAO" , producto.DiasDuracion?.ToString()},
                { "T_PRODUTO.ID_MANEJO_IDENTIFICADOR", this._productoMapper.MapManejoIdentificador(producto.ManejoIdentificador)},
                { "T_PRODUTO.TP_DISPLAY", producto.TipoDisplay},
                { "T_PRODUTO.DS_ANEXO1", producto.Anexo1},
                { "T_PRODUTO.DS_ANEXO2",producto.Anexo2},
                { "T_PRODUTO.DS_ANEXO3",producto.Anexo3},
                { "T_PRODUTO.DS_ANEXO4",producto.Anexo4},
                { "T_PRODUTO.DT_UPDROW" , producto.FechaModificacion?.ToString()},
                { "T_PRODUTO.DT_ADDROW" ,producto.FechaIngreso?.ToString()},
                { "T_PRODUTO.VL_ALTURA" , producto.Altura?.ToString()},
                { "T_PRODUTO.VL_LARGURA" , producto.Ancho?.ToString()},
                { "T_PRODUTO.VL_PROFUNDIDADE" , producto.Profundidad?.ToString()},
                { "T_PRODUTO.TP_MANEJO_FECHA" , producto.TipoManejoFecha?.ToString()},
                { "T_PRODUTO.VL_AVISO_AJUSTE" , producto.AvisoAjusteInventario?.ToString()},
                { "T_PRODUTO.DS_HELP_COLECTOR" , producto.AyudaColector},
                { "T_PRODUTO.QT_SUBBULTO" , producto.SubBulto?.ToString()},
                { "T_PRODUTO.CD_EXCLUSIVO" , producto.Exclusivo?.ToString()},
                { "T_PRODUTO.QT_UND_DISTRIBUCION" , producto.UnidadDistribucion.ToString()},
                { "T_PRODUTO.QT_DIAS_VALIDADE_LIBERACION" , producto.DiasLiberacion?.ToString() },
                { "T_PRODUTO.QT_UND_BULTO" , producto.UnidadBulto.ToString()},
                { "T_PRODUTO.DS_CLASSE" , producto.Clase.Descripcion},
                { "T_PRODUTO.DS_FAMILIA_PRODUTO" , producto.Familia?.Descripcion},
                { "T_PRODUTO.NM_EMPRESA" , producto.Empresa?.Nombre},
                { "T_PRODUTO.DS_ROTATIVIDADE" , producto.Rotatividad?.Descripcion},
                { "T_PRODUTO.TP_PESO_PRODUTO", producto.TipoPeso?.ToString()},
                { "T_PRODUTO.SG_PRODUTO", producto.SgProducto },
                { "T_PRODUTO.DS_DIFER_PESO_QTDE", producto.DescDifPeso },
                { "T_PRODUTO.FT_CONVERSAO", producto.Conversion?.ToString() },
                { "T_PRODUTO.CD_ORIGEM", producto.CdOrigen },
                { "T_PRODUTO.CD_NIVEL", producto.Nivel },
                { "T_PRODUTO.CD_UNID_EMB", producto.UndEmb },
                { "T_PRODUTO.ID_CROSS_DOCKING", producto.IdCrossDocking },
                { "T_PRODUTO.ID_REDONDEO_VALIDEZ", producto.RedondeoValidez },
                { "T_PRODUTO.ID_AGRUPACION", producto.Agrupacion },
                { "T_PRODUTO.ID_MANEJA_TOMA_DATO", producto.ManejaTomaDato },
                { "T_PRODUTO.DS_ANEXO5", producto.Anexo5 },
                { "T_PRODUTO.CD_GRUPO_CONSULTA", producto.GrupoConsulta },
                { "T_PRODUTO.DS_DISPLAY", producto.DescripcionDisplay },
                { "T_PRODUTO.VL_PRECIO_SEG_DISTR", producto.PrecioSegDistribucion?.ToString() },
                { "T_PRODUTO.VL_PRECIO_SEG_STOCK", producto.PrecioSegStock?.ToString() },
                { "T_PRODUTO.VL_PRECIO_DISTRIB", producto.PrecioDistribucion?.ToString() },
                { "T_PRODUTO.VL_PRECIO_EGRESO", producto.PrecioEgreso?.ToString() },
                { "T_PRODUTO.VL_PRECIO_INGRESO", producto.PrecioIngreso?.ToString() },
                { "T_PRODUTO.VL_PRECIO_STOCK", producto.PrecioStock?.ToString() },
                { "T_PRODUTO.CD_UND_MEDIDA_FACT", producto.UndMedidaFact },
                { "T_PRODUTO.CD_PRODUTO_UNICO", producto.ProductoUnico },
                { "T_PRODUTO.CD_RAMO_PRODUTO", producto.Ramo.ToString() },
                { "T_PRODUTO.FL_ACEPTA_DECIMALES", producto.AceptaDecimales ? "S" : "N" },
                { "T_PRODUTO.ND_FACTURACION_COMP1", producto.Componente1 },
                { "T_PRODUTO.ND_FACTURACION_COMP2", producto.Componente2 },
                { "T_PRODUTO.ND_MODALIDAD_INGRESO_LOTE", producto.ModalidadIngresoLote },
                { "T_PRODUTO.QT_PADRON_STOCK", producto.CantidadPadronStock?.ToString() },
                { "T_PRODUTO.QT_GENERICO", producto.CantidadGenerica?.ToString() },
                { "T_PRODUTO.CODIGO_BASE", producto.CodigoBase },
                { "T_PRODUTO.TALLE", producto.Talle },
                { "T_PRODUTO.COLOR", producto.Color },
                { "T_PRODUTO.TEMPORADA", producto.Temporada },
                { "T_PRODUTO.VL_CATEGORIA_01", producto.Categoria1 },
                { "T_PRODUTO.VL_CATEGORIA_02", producto.Categoria2 },
                { "T_PRODUTO.CD_VENTANA_LIBERACION", producto.VentanaLiberacion },
            };

            var descripcion1 = string.Empty;
            var descripcion2 = string.Empty;

            if (!string.IsNullOrEmpty(producto.Descripcion))
            {
                if (producto.Descripcion.Length > 37)
                {
                    descripcion1 = producto.Descripcion.Substring(0, 37);
                    descripcion2 = producto.Descripcion.Substring(37, (producto.Descripcion.Length - 37));
                }
                else
                {
                    descripcion1 = producto.Descripcion;
                }
            }

            claves.Add("DS_PRODUCTO_1", descripcion1);
            claves.Add("DS_PRODUCTO_2", descripcion2);

            claves.Add("T_CODIGO_BARRAS.CD_BARRAS", codigoBarra == null ? string.Empty : codigoBarra.Codigo);

            return claves;
        }

    }
}
