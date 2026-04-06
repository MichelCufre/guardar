using System.Collections.Generic;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Domain.General.Enums;
using WIS.Domain.Registro;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class ProductoMapper : Mapper
    {
        protected readonly EmpresaMapper _empresaMapper;
        protected readonly ClaseMapper _claseMapper;
        protected readonly ProductoFamiliaMapper _familiaMapper;
        protected readonly ProductoRotatividadMapper _rotatividadMapper;

        public ProductoMapper()
        {
            this._empresaMapper = new EmpresaMapper();
            this._claseMapper = new ClaseMapper();
            this._familiaMapper = new ProductoFamiliaMapper();
            this._rotatividadMapper = new ProductoRotatividadMapper();
        }

        public virtual Producto MapToObject(T_PRODUTO entity)
        {
            if (entity == null) return null;

            return new Producto
            {
                CodigoClase = entity.CD_CLASSE,
                CodigoEmpresa = entity.CD_EMPRESA,
                Exclusivo = entity.CD_EXCLUSIVO,
                CodigoFamilia = entity.CD_FAMILIA_PRODUTO,
                GrupoConsulta = entity.CD_GRUPO_CONSULTA,
                CodigoMercadologico = entity.CD_MERCADOLOGICO,
                NAM = entity.CD_NAM,
                Nivel = entity.CD_NIVEL,
                CdOrigen = entity.CD_ORIGEM,
                Codigo = entity.CD_PRODUTO,
                CodigoProductoEmpresa = entity.CD_PRODUTO_EMPRESA,
                ProductoUnico = entity.CD_PRODUTO_UNICO,
                Ramo = entity.CD_RAMO_PRODUTO,
                CodigoRotatividad = entity.CD_ROTATIVIDADE,
                Situacion = entity.CD_SITUACAO,
                UnidadMedida = entity.CD_UNIDADE_MEDIDA,
                UndMedidaFact = entity.CD_UND_MEDIDA_FACT,
                UndEmb = entity.CD_UNID_EMB,
                Anexo1 = entity.DS_ANEXO1,
                Anexo2 = entity.DS_ANEXO2,
                Anexo3 = entity.DS_ANEXO3,
                Anexo4 = entity.DS_ANEXO4,
                Anexo5 = entity.DS_ANEXO5,
                DescDifPeso = entity.DS_DIFER_PESO_QTDE,
                DescripcionDisplay = entity.DS_DISPLAY,
                AyudaColector = entity.DS_HELP_COLECTOR,
                Descripcion = entity.DS_PRODUTO,
                DescripcionReducida = entity.DS_REDUZIDA,
                FechaIngreso = entity.DT_ADDROW,
                FechaSituacion = entity.DT_SITUACAO,
                FechaModificacion = entity.DT_UPDROW,
                AceptaDecimales = this.MapStringToBoolean(entity.FL_ACEPTA_DECIMALES),
                Conversion = entity.FT_CONVERSAO,
                Agrupacion = entity.ID_AGRUPACION,
                IdCrossDocking = entity.ID_CROSS_DOCKING,
                ManejaTomaDato = entity.ID_MANEJA_TOMA_DATO,
                ManejoIdentificador = this.MapManejoIdentificador(entity.ID_MANEJO_IDENTIFICADOR),
                RedondeoValidez = entity.ID_REDONDEO_VALIDEZ,
                Componente1 = entity.ND_FACTURACION_COMP1,
                Componente2 = entity.ND_FACTURACION_COMP2,
                ModalidadIngresoLote = entity.ND_MODALIDAD_INGRESO_LOTE,
                PesoBruto = entity.PS_BRUTO,
                PesoNeto = entity.PS_LIQUIDO,
                DiasDuracion = entity.QT_DIAS_DURACAO,
                DiasValidez = entity.QT_DIAS_VALIDADE,
                DiasLiberacion = entity.QT_DIAS_VALIDADE_LIBERACION,
                StockMaximo = entity.QT_ESTOQUE_MAXIMO,
                StockMinimo = entity.QT_ESTOQUE_MINIMO,
                CantidadGenerica = entity.QT_GENERICO,
                CantidadPadronStock = entity.QT_PADRON_STOCK,
                SubBulto = entity.QT_SUBBULTO,
                UnidadBulto = entity.QT_UND_BULTO,
                UnidadDistribucion = entity.QT_UND_DISTRIBUCION,
                SgProducto = entity.SG_PRODUTO,
                TipoDisplay = entity.TP_DISPLAY,
                TipoManejoFecha = entity.TP_MANEJO_FECHA,
                TipoPeso = entity.TP_PESO_PRODUTO,
                Altura = entity.VL_ALTURA,
                AvisoAjusteInventario = entity.VL_AVISO_AJUSTE,
                VolumenCC = entity.VL_CUBAGEM,
                UltimoCosto = entity.VL_CUSTO_ULT_ENT,
                Ancho = entity.VL_LARGURA,
                PrecioDistribucion = entity.VL_PRECIO_DISTRIB,
                PrecioEgreso = entity.VL_PRECIO_EGRESO,
                PrecioIngreso = entity.VL_PRECIO_INGRESO,
                PrecioSegDistribucion = entity.VL_PRECIO_SEG_DISTR,
                PrecioSegStock = entity.VL_PRECIO_SEG_STOCK,
                PrecioStock = entity.VL_PRECIO_STOCK,
                PrecioVenta = entity.VL_PRECO_VENDA,
                Profundidad = entity.VL_PROFUNDIDADE,
                CodigoBase = entity.CODIGO_BASE,
                Talle = entity.TALLE,
                Color = entity.COLOR,
                Temporada = entity.TEMPORADA,
                Categoria1 = entity.VL_CATEGORIA_01,
                Categoria2 = entity.VL_CATEGORIA_02,
                NumeroTransaccion = entity.NU_TRANSACCION,
                VentanaLiberacion = entity.CD_VENTANA_LIBERACION,
                Empresa = entity.T_EMPRESA == null ? null : _empresaMapper.MapToObject(entity.T_EMPRESA),
                Clase = entity.T_CLASSE == null ? null : _claseMapper.MapToObject(entity.T_CLASSE),
                Rotatividad = entity.T_ROTATIVIDADE == null ? null : _rotatividadMapper.MapToObject(entity.T_ROTATIVIDADE),
                Familia = entity.T_FAMILIA_PRODUTO == null ? null : _familiaMapper.MapToObject(entity.T_FAMILIA_PRODUTO),
            };
        }

        public virtual ProductoPallet MapToObject(T_PRODUTO_PALLET entity)
        {
            if (entity == null) return null;

            return new ProductoPallet
            {
                CodigoProducto = entity.CD_PRODUTO,
                Empresa = entity.CD_EMPRESA,
                CodigoPallet = entity.CD_PALLET,
                Embalaje = entity.CD_FAIXA,
                Unidades = entity.QT_UNIDADES,
                Prioridad = entity.NU_PRIORIDAD,
            };
        }

        public virtual ProductoProveedor MapToObject(T_PRODUTO_CONVERTOR entity)
        {
            if (entity == null)
                return null;

            return new ProductoProveedor
            {
                CodigoProducto = entity.CD_PRODUTO,
                Empresa = entity.CD_EMPRESA,
                Cliente = entity.CD_CLIENTE,
                CodigoExterno = entity.CD_EXTERNO,
                FechaIngreso = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UPDROW,
            };
        }

        public virtual T_PRODUTO MapToEntity(Producto obj)
        {
            return new T_PRODUTO
            {
                CD_CLASSE = obj.CodigoClase,
                CD_EMPRESA = obj.CodigoEmpresa,
                CD_EXCLUSIVO = obj.Exclusivo,
                CD_FAMILIA_PRODUTO = obj.CodigoFamilia,
                CD_GRUPO_CONSULTA = NullIfEmpty(obj.GrupoConsulta),
                CD_MERCADOLOGICO = obj.CodigoMercadologico,
                CD_NAM = NullIfEmpty(obj.NAM),
                CD_NIVEL = obj.Nivel,
                CD_ORIGEM = obj.CdOrigen,
                CD_PRODUTO = obj.Codigo,
                CD_PRODUTO_EMPRESA = obj.CodigoProductoEmpresa,
                CD_PRODUTO_UNICO = obj.ProductoUnico,
                CD_RAMO_PRODUTO = obj.Ramo,
                CD_ROTATIVIDADE = obj.CodigoRotatividad,
                CD_SITUACAO = obj.Situacion,
                CD_UNIDADE_MEDIDA = obj.UnidadMedida,
                CD_UND_MEDIDA_FACT = obj.UndMedidaFact,
                CD_UNID_EMB = obj.UndEmb,
                DS_ANEXO1 = obj.Anexo1,
                DS_ANEXO2 = obj.Anexo2,
                DS_ANEXO3 = obj.Anexo3,
                DS_ANEXO4 = obj.Anexo4,
                DS_ANEXO5 = obj.Anexo5,
                DS_DIFER_PESO_QTDE = obj.DescDifPeso,
                DS_DISPLAY = obj.DescripcionDisplay,
                DS_HELP_COLECTOR = obj.AyudaColector,
                DS_PRODUTO = obj.Descripcion,
                DS_REDUZIDA = obj.DescripcionReducida,
                DT_ADDROW = obj.FechaIngreso,
                DT_SITUACAO = obj.FechaSituacion,
                DT_UPDROW = obj.FechaModificacion,
                FL_ACEPTA_DECIMALES = this.MapBooleanToString(obj.AceptaDecimales),
                FT_CONVERSAO = obj.Conversion,
                ID_AGRUPACION = obj.Agrupacion,
                ID_CROSS_DOCKING = obj.IdCrossDocking,
                ID_MANEJA_TOMA_DATO = obj.ManejaTomaDato,
                ID_MANEJO_IDENTIFICADOR = this.MapManejoIdentificador(obj.ManejoIdentificador),
                ID_REDONDEO_VALIDEZ = obj.RedondeoValidez,
                ND_FACTURACION_COMP1 = NullIfEmpty(obj.Componente1)?.ToUpper(),
                ND_FACTURACION_COMP2 = NullIfEmpty(obj.Componente2)?.ToUpper(),
                ND_MODALIDAD_INGRESO_LOTE = obj.ModalidadIngresoLote,
                PS_BRUTO = obj.PesoBruto,
                PS_LIQUIDO = obj.PesoNeto,
                QT_DIAS_DURACAO = obj.DiasDuracion,
                QT_DIAS_VALIDADE = obj.DiasValidez,
                QT_DIAS_VALIDADE_LIBERACION = obj.DiasLiberacion,
                QT_ESTOQUE_MAXIMO = obj.StockMaximo,
                QT_ESTOQUE_MINIMO = obj.StockMinimo,
                QT_GENERICO = obj.CantidadGenerica,
                QT_PADRON_STOCK = obj.CantidadPadronStock,
                QT_SUBBULTO = obj.SubBulto,
                QT_UND_BULTO = obj.UnidadBulto,
                QT_UND_DISTRIBUCION = obj.UnidadDistribucion,
                SG_PRODUTO = obj.SgProducto,
                TP_DISPLAY = obj.TipoDisplay,
                TP_MANEJO_FECHA = obj.TipoManejoFecha,
                TP_PESO_PRODUTO = obj.TipoPeso,
                VL_ALTURA = obj.Altura,
                VL_AVISO_AJUSTE = obj.AvisoAjusteInventario,
                VL_CUBAGEM = obj.VolumenCC,
                VL_CUSTO_ULT_ENT = obj.UltimoCosto,
                VL_LARGURA = obj.Ancho,
                VL_PRECIO_DISTRIB = obj.PrecioDistribucion,
                VL_PRECIO_EGRESO = obj.PrecioEgreso,
                VL_PRECIO_INGRESO = obj.PrecioIngreso,
                VL_PRECIO_SEG_DISTR = obj.PrecioSegDistribucion,
                VL_PRECIO_SEG_STOCK = obj.PrecioSegStock,
                VL_PRECIO_STOCK = obj.PrecioStock,
                VL_PRECO_VENDA = obj.PrecioVenta,
                VL_PROFUNDIDADE = obj.Profundidad,
                CODIGO_BASE = obj.CodigoBase,
                TALLE = obj.Talle,
                COLOR = obj.Color,
                TEMPORADA = obj.Temporada,
                VL_CATEGORIA_01 = obj.Categoria1,
                VL_CATEGORIA_02 = obj.Categoria2,
                NU_TRANSACCION = obj.NumeroTransaccion,
                CD_VENTANA_LIBERACION= obj.VentanaLiberacion,
            };
        }

        public virtual T_PRODUTO_PALLET MapToEntity(ProductoPallet obj)
        {
            if (obj == null) return null;

            return new T_PRODUTO_PALLET
            {
                CD_PRODUTO = obj.CodigoProducto,
                CD_EMPRESA = obj.Empresa,
                CD_PALLET = obj.CodigoPallet,
                CD_FAIXA = obj.Embalaje,
                QT_UNIDADES = obj.Unidades,
                NU_PRIORIDAD = obj.Prioridad
            };
        }

        public virtual T_PRODUTO_CONVERTOR MapToEntity(ProductoProveedor obj)
        {
            return new T_PRODUTO_CONVERTOR
            {
                CD_PRODUTO = obj.CodigoProducto,
                CD_EMPRESA = obj.Empresa,
                CD_CLIENTE = obj.Cliente,
                CD_EXTERNO = obj.CodigoExterno,
                DT_ADDROW = obj.FechaIngreso,
                DT_UPDROW = obj.FechaModificacion,
            };
        }


        public virtual ManejoIdentificador MapManejoIdentificador(string manejoIdentificador)
        {
            switch (manejoIdentificador)
            {
                case ManejoIdentificadorDb.Producto:
                    return ManejoIdentificador.Producto;
                case ManejoIdentificadorDb.Lote:
                    return ManejoIdentificador.Lote;
                case ManejoIdentificadorDb.Serie:
                    return ManejoIdentificador.Serie;
                default:
                    return ManejoIdentificador.Unknown;
            }
        }

        public virtual string MapManejoIdentificador(ManejoIdentificador manejoIdentificador)
        {
            switch (manejoIdentificador)
            {
                case ManejoIdentificador.Producto:
                    return ManejoIdentificadorDb.Producto;
                case ManejoIdentificador.Lote:
                    return ManejoIdentificadorDb.Lote;
                case ManejoIdentificador.Serie:
                    return ManejoIdentificadorDb.Serie;
                default:
                    return null;
            }
        }

        public virtual ManejoFechaProducto MapManejoFecha(string manejoIdentificador)
        {
            switch (manejoIdentificador)
            {
                case ManejoFechaProductoDb.Duradero:
                    return ManejoFechaProducto.Duradero;
                case ManejoFechaProductoDb.Expirable:
                    return ManejoFechaProducto.Expirable;
                case ManejoFechaProductoDb.Fifo:
                    return ManejoFechaProducto.Fifo;
                default:
                    return ManejoFechaProducto.Unknown;
            }
        }

        public virtual string MapManejoFecha(ManejoFechaProducto manejoIdentificador)
        {
            switch (manejoIdentificador)
            {
                case ManejoFechaProducto.Duradero:
                    return ManejoFechaProductoDb.Duradero;
                case ManejoFechaProducto.Expirable:
                    return ManejoFechaProductoDb.Expirable;
                case ManejoFechaProducto.Fifo:
                    return ManejoFechaProductoDb.Fifo;
                default:
                    return null;
            }
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

                models.Add(model);
            }
            return models;
        }
    }
}
