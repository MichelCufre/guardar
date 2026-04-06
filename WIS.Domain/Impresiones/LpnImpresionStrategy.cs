using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.Impresiones.Utils;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.StockEntities;

namespace WIS.Domain.Impresiones
{
    public class LpnImpresionStrategy : IImpresionDetalleBuildingStrategy
    {
        protected readonly IEstiloTemplate _estilo;
        protected readonly List<Lpn> _lpns;
        protected readonly IUnitOfWork _uow;
        protected readonly AtributoMapper _mapper;
        protected readonly ImpresionUtils _utiltyImpresion;
        protected readonly IPrintingService _printingService;
        protected const string TextoVarios = "(VARIOS)";

        public LpnImpresionStrategy(IEstiloTemplate estilo,
            List<Lpn> lpns,
            IUnitOfWork uow,
            IPrintingService printingService)
        {
            this._estilo = estilo;
            this._lpns = lpns;
            this._uow = uow;
            this._mapper = new AtributoMapper();
            this._utiltyImpresion = new ImpresionUtils();
            this._printingService = printingService;
        }

        public virtual List<DetalleImpresion> Generar(Impresora impresora)
        {
            TemplateImpresion template = this._estilo.GetTemplate(impresora);

            var detalles = new List<DetalleImpresion>();

            foreach (var lpn in this._lpns.OrderBy(s => s.NumeroLPN))
            {
                Dictionary<string, string> claves = this.GetDiccionarioInformacion(lpn);

                detalles.Add(new DetalleImpresion
                {
                    Contenido = template.Parse(claves),
                    Estado = _printingService.GetEstadoInicial(),
                    FechaProcesado = DateTime.Now,
                });
            }

            return detalles;
        }

        public virtual Dictionary<string, string> GetDiccionarioInformacion(Lpn lpn)
        {
            var empresa = _uow.EmpresaRepository.GetEmpresa(lpn.Empresa);
            var tipo = _uow.ManejoLpnRepository.GetTipoLpn(lpn.Tipo);

            var claves = new Dictionary<string, string>()
            {
                { "T_LPN.CD_EMPRESA", lpn.Empresa.ToString()},
                { "T_LPN.CD_ENDERECO" , lpn.Ubicacion },
                { "T_LPN.DT_ACTIVACION",lpn.FechaActivacion.ToString()},
                { "T_LPN.DT_ADDROW " , lpn.FechaAdicion.ToString() },
                { "T_LPN.ID_ESTADO", lpn.Estado},
                { "T_LPN.ID_LPN_EXTERNO", lpn.IdExterno},
                { "T_LPN.NU_LPN" , lpn.NumeroLPN.ToString()},
                { "T_LPN.TP_LPN_TIPO" , lpn.Tipo?.ToString()},
                { "T_LPN.NM_LPN_TIPO" , tipo.Nombre},
                { "T_LPN.NM_EMPRESA",empresa.Nombre},
                { "T_LPN.ID_PACKING",lpn.IdPacking},
            };

            var detalles = _uow.ManejoLpnRepository.GetDetallesLpn(lpn.NumeroLPN);
            if (detalles.Count > 0)
            {
                var productos = detalles.GroupBy(x => new { x.CodigoProducto, x.Empresa }).Select(x => new { x.Key.CodigoProducto, x.Key.Empresa }).ToList();
                if (productos.Count > 1)
                {
                    claves.Add("T_LPN_DET.CD_PRODUTO", TextoVarios);
                    claves.Add("T_LPN_DET.DS_PRODUTO", "");

                    var lotes = detalles.GroupBy(x => x.Lote).Select(x => new { x.Key }).ToList();
                    if (lotes.Count == 1)
                        claves.Add("T_LPN_DET.NU_IDENTIFICADOR", lotes.FirstOrDefault().Key);
                    else
                        claves.Add("T_LPN_DET.NU_IDENTIFICADOR", TextoVarios);
                }
                else
                {
                    var prod = productos.FirstOrDefault();
                    var producto = _uow.ProductoRepository.GetProducto(prod.Empresa, prod.CodigoProducto);

                    claves.Add("T_LPN_DET.CD_PRODUTO", producto.Codigo);
                    claves.Add("T_LPN_DET.DS_PRODUTO", producto.Descripcion);

                    var lotes = detalles.GroupBy(x => x.Lote).Select(x => new { x.Key }).ToList();
                    if (lotes.Count == 1)
                        claves.Add("T_LPN_DET.NU_IDENTIFICADOR", lotes.FirstOrDefault().Key);
                    else
                        claves.Add("T_LPN_DET.NU_IDENTIFICADOR", TextoVarios);

                    AddProductoProperties(claves, producto);
                }

                var atributosDetalles = _uow.ManejoLpnRepository.GetAllLpnDetalleAtributo(lpn.NumeroLPN);
                if (atributosDetalles.Count > 0)
                {
                    foreach (var ad in atributosDetalles)
                    {
                        var atributo = _uow.AtributoRepository.GetAtributo(ad.IdAtributo);

                        if (!claves.Any(x => x.Key == $"T_LPN_DET_ATRIBUTO.{atributo.Nombre}"))
                        {
                            claves.Add($"T_LPN_DET_ATRIBUTO.{atributo.Nombre}", ad.ValorAtributo);
                        }
                        else
                        {
                            claves.Remove($"T_LPN_DET_ATRIBUTO.{atributo.Nombre}");
                            claves.Add($"T_LPN_DET_ATRIBUTO.{atributo.Nombre}", TextoVarios);
                        }
                    }
                }
            }

            var codigoBarras = _uow.ManejoLpnRepository.GetCodigoDeBarras(lpn.NumeroLPN);
            if (codigoBarras.Count > 0)
            {
                claves.Add("T_LPN_BARRAS", codigoBarras.FirstOrDefault().CodigoBarras);

                foreach (var barra in codigoBarras)
                {
                    claves.Add($"T_LPN_BARRAS.{barra.Orden}", barra.CodigoBarras);
                }
            }

            var atributos = _uow.ManejoLpnRepository.GetAllLpnAtributo(lpn.NumeroLPN);
            foreach (var a in atributos)
            {
                var atributo = _uow.AtributoRepository.GetAtributo(a.Id);

                if (!claves.Any(x => x.Key == $"T_LPN_ATRIBUTO.{atributo.Nombre}"))
                {
                    claves.Add($"T_LPN_ATRIBUTO.{atributo.Nombre}", a.Valor);
                }
            }

            return claves;
        }

        public virtual void AddProductoProperties(Dictionary<string, string> claves, Producto producto)
        {
            var productoMapper = new ProductoMapper();

            claves.Add("T_PRODUTO.CD_PRODUTO", producto.Codigo);
            claves.Add("T_PRODUTO.CD_EMPRESA", producto.CodigoEmpresa.ToString());
            claves.Add("T_PRODUTO.CD_PRODUTO_EMPRESA", producto.CodigoProductoEmpresa);
            claves.Add("T_PRODUTO.DS_PRODUTO", producto.Descripcion);
            claves.Add("T_PRODUTO.DS_REDUZIDA", producto.DescripcionReducida);
            claves.Add("T_PRODUTO.CD_NAM", producto.NAM);
            claves.Add("T_PRODUTO.CD_MERCADOLOGICO", producto.CodigoMercadologico);
            claves.Add("T_PRODUTO.TP_MANEJO_FECHA", producto.TipoManejoFecha);
            claves.Add("T_PRODUTO.CD_SITUACAO", producto.Situacion.ToString());
            claves.Add("T_PRODUTO.DT_SITUACAO", producto.FechaSituacion?.ToString(CDateFormats.DATE_ONLY));
            claves.Add("T_PRODUTO.ID_MANEJO_IDENTIFICADOR", productoMapper.MapManejoIdentificador(producto.ManejoIdentificador));
            claves.Add("T_PRODUTO.FL_ACEPTA_DECIMALES", productoMapper.MapBooleanToString(producto.AceptaDecimales));
            claves.Add("T_PRODUTO.CD_UNIDADE_MEDIDA ", producto.UnidadMedida);
            claves.Add("T_PRODUTO.QT_DIAS_VALIDADE_LIBERACION", producto.DiasLiberacion?.ToString());
            claves.Add("T_PRODUTO.QT_DIAS_DURACAO", producto.DiasDuracion?.ToString());
            claves.Add("T_PRODUTO.QT_DIAS_VALIDADE", producto.DiasValidez?.ToString());
            claves.Add("T_PRODUTO.ND_MODALIDAD_INGRESO_LOTE", producto.ModalidadIngresoLote);
            claves.Add("T_PRODUTO.QT_ESTOQUE_MINIMO", producto.StockMinimo?.ToString());
            claves.Add("T_PRODUTO.QT_ESTOQUE_MAXIMO", producto.StockMaximo?.ToString());
            claves.Add("T_PRODUTO.PS_BRUTO", producto.PesoBruto?.ToString());
            claves.Add("T_PRODUTO.PS_LIQUIDO", producto.PesoNeto?.ToString());
            claves.Add("T_PRODUTO.VL_ALTURA", producto.Altura?.ToString());
            claves.Add("T_PRODUTO.VL_LARGURA", producto.Ancho?.ToString());
            claves.Add("T_PRODUTO.VL_PROFUNDIDADE", producto.Profundidad?.ToString());
            claves.Add("T_PRODUTO.VL_CUBAGEM", producto.VolumenCC?.ToString());
            claves.Add("T_PRODUTO.QT_UND_BULTO", producto.UnidadBulto.ToString());
            claves.Add("T_PRODUTO.QT_UND_DISTRIBUCION", producto.UnidadDistribucion.ToString());
            claves.Add("T_PRODUTO.VL_AVISO_AJUSTE", producto.AvisoAjusteInventario?.ToString());
            claves.Add("T_PRODUTO.TP_DISPLAY", producto.TipoDisplay);
            claves.Add("T_PRODUTO.DS_DISPLAY", producto.DescripcionDisplay);
            claves.Add("T_PRODUTO.QT_SUBBULTO", producto.SubBulto?.ToString());
            claves.Add("T_PRODUTO.VL_CUSTO_ULT_ENT", producto.UltimoCosto?.ToString());
            claves.Add("T_PRODUTO.VL_PRECO_VENDA", producto.PrecioVenta?.ToString());
            claves.Add("T_PRODUTO.DS_HELP_COLECTOR", producto.AyudaColector);
            claves.Add("T_PRODUTO.ND_FACTURACION_COMP1", producto.Componente1);
            claves.Add("T_PRODUTO.ND_FACTURACION_COMP2", producto.Componente2);
            claves.Add("T_PRODUTO.DS_ANEXO1", producto.Anexo1);
            claves.Add("T_PRODUTO.DS_ANEXO2", producto.Anexo2);
            claves.Add("T_PRODUTO.DS_ANEXO3", producto.Anexo3);
            claves.Add("T_PRODUTO.DS_ANEXO4", producto.Anexo4);
            claves.Add("T_PRODUTO.DS_ANEXO5", producto.Anexo5);
            claves.Add("T_PRODUTO.DT_ADDROW", producto.FechaIngreso?.ToString(CDateFormats.DATE_ONLY));
            claves.Add("T_PRODUTO.DT_UPDROW", producto.FechaModificacion?.ToString(CDateFormats.DATE_ONLY));
            claves.Add("T_PRODUTO.CD_CLASSE", producto.CodigoClase);
            claves.Add("T_PRODUTO.CD_FAMILIA_PRODUTO", producto.CodigoFamilia.ToString());
            claves.Add("T_PRODUTO.CD_ROTATIVIDADE", producto.CodigoRotatividad?.ToString());
            claves.Add("T_PRODUTO.CD_GRUPO_CONSULTA", producto.GrupoConsulta);
            claves.Add("T_PRODUTO.CD_EXCLUSIVO", producto.Exclusivo?.ToString());
            claves.Add("T_PRODUTO.TP_PESO_PRODUTO", producto.TipoPeso?.ToString());
            claves.Add("T_PRODUTO.CD_NIVEL", producto.Nivel);
            claves.Add("T_PRODUTO.CD_ORIGEM", producto.CdOrigen);
            claves.Add("T_PRODUTO.CD_PRODUTO_UNICO", producto.ProductoUnico);
            claves.Add("T_PRODUTO.CD_RAMO_PRODUTO", producto.Ramo.ToString());
            claves.Add("T_PRODUTO.CD_UND_MEDIDA_FACT", producto.UndMedidaFact);
            claves.Add("T_PRODUTO.CD_UNID_EMB", producto.UndEmb);
            claves.Add("T_PRODUTO.DS_DIFER_PESO_QTDE", producto.DescDifPeso);
            claves.Add("T_PRODUTO.FT_CONVERSAO", producto.Conversion?.ToString());
            claves.Add("T_PRODUTO.ID_AGRUPACION", producto.Agrupacion);
            claves.Add("T_PRODUTO.ID_CROSS_DOCKING", producto.IdCrossDocking);
            claves.Add("T_PRODUTO.ID_MANEJA_TOMA_DATO", producto.ManejaTomaDato);
            claves.Add("T_PRODUTO.ID_REDONDEO_VALIDEZ", producto.RedondeoValidez);
            claves.Add("T_PRODUTO.QT_GENERICO", producto.CantidadGenerica?.ToString());
            claves.Add("T_PRODUTO.QT_PADRON_STOCK", producto.CantidadPadronStock?.ToString());
            claves.Add("T_PRODUTO.SG_PRODUTO", producto.SgProducto);
            claves.Add("T_PRODUTO.VL_PRECIO_DISTRIB", producto.PrecioDistribucion?.ToString());
            claves.Add("T_PRODUTO.VL_PRECIO_EGRESO", producto.PrecioEgreso?.ToString());
            claves.Add("T_PRODUTO.VL_PRECIO_INGRESO", producto.PrecioIngreso?.ToString());
            claves.Add("T_PRODUTO.VL_PRECIO_SEG_DISTR", producto.PrecioSegDistribucion?.ToString());
            claves.Add("T_PRODUTO.VL_PRECIO_SEG_STOCK", producto.PrecioSegStock?.ToString());
            claves.Add("T_PRODUTO.VL_PRECIO_STOCK", producto.PrecioStock?.ToString());
            claves.Add("T_PRODUTO.CODIGO_BASE", producto.CodigoBase);
            claves.Add("T_PRODUTO.TALLE", producto.Talle);
            claves.Add("T_PRODUTO.COLOR", producto.Color);
            claves.Add("T_PRODUTO.TEMPORADA", producto.Temporada);
            claves.Add("T_PRODUTO.VL_CATEGORIA_01", producto.Categoria1);
            claves.Add("T_PRODUTO.VL_CATEGORIA_02", producto.Categoria2);
        }
    }
}
