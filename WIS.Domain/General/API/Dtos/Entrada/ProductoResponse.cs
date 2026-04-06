using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WIS.Domain.General.API.Dtos.Entrada
{
    public class ProductoResponse
    {
        [StringLength(40)]
        public string Codigo { get; set; } //CD_PRODUTO
        public int CodigoEmpresa { get; set; } //CD_EMPRESA
        
        [StringLength(40)]
        public string CodigoProductoEmpresa { get; set; } //CD_PRODUTO_EMPRESA
        
        [StringLength(65)]
        public string Descripcion { get; set; } //DS_PRODUTO
        
        [StringLength(20)]
        public string DescripcionReducida { get; set; } //DS_REDUZIDA
        
        [StringLength(20)]
        public string NAM { get; set; } //CD_NAM
        
        [StringLength(40)]
        public string CodigoMercadologico { get; set; } //CD_MERCADOLOGICO
        
        [StringLength(1)]
        public string TipoManejoFecha { get; set; } //TP_MANEJO_FECHA
        public short Situacion { get; set; } //CD_SITUACAO
        public string FechaSituacion { get; set; } //DT_SITUACAO
        
        [StringLength(1)] 
        public string ManejoIdentificador { get; set; } //ID_MANEJO_IDENTIFICADOR
        
        [StringLength(1)] 
        public string AceptaDecimales { get; set; } //FL_ACEPTA_DECIMALES
        
        [StringLength(10)] 
        public string UnidadMedida { get; set; } //CD_UNIDADE_MEDIDA
        public short? DiasLiberacion { get; set; } //QT_DIAS_VALIDADE_LIBERACION
        public short? DiasDuracion { get; set; } //QT_DIAS_DURACAO
        public short? DiasValidez { get; set; } //QT_DIAS_VALIDADE
        
        [StringLength(10)] 
        public string ModalidadIngresoLote { get; set; } //ND_MODALIDAD_INGRESO_LOTE
        public int? StockMinimo { get; set; } //QT_ESTOQUE_MINIMO
        public int? StockMaximo { get; set; } //QT_ESTOQUE_MAXIMO
        public decimal? PesoBruto { get; set; } //PS_BRUTO
        public decimal? PesoNeto { get; set; } //PS_LIQUIDO
        public decimal? Altura { get; set; } //VL_ALTURA
        public decimal? Ancho { get; set; } //VL_LARGURA
        public decimal? Profundidad { get; set; } //VL_PROFUNDIDADE
        public decimal? VolumenCC { get; set; } //VL_CUBAGEM
        public decimal UnidadBulto { get; set; } //QT_UND_BULTO
        public decimal UnidadDistribucion { get; set; } //QT_UND_DISTRIBUCION
        public decimal? AvisoAjusteInventario { get; set; } //VL_AVISO_AJUSTE
        
        [StringLength(1)] 
        public string TipoDisplay { get; set; } //TP_DISPLAY
        
        [StringLength(200)] 
        public string DescripcionDisplay { get; set; } //DS_DISPLAY
        public short? SubBulto { get; set; } //QT_SUBBULTO
        public decimal? UltimoCosto { get; set; } //VL_CUSTO_ULT_ENT
        public decimal? PrecioVenta { get; set; } //VL_PRECO_VENDA
        
        [StringLength(200)] 
        public string AyudaColector { get; set; } //DS_HELP_COLECTOR
        
        [StringLength(20)] 
        public string Componente1 { get; set; } //ND_FACTURACION_COMP1
        
        [StringLength(20)] 
        public string Componente2 { get; set; } //ND_FACTURACION_COMP2
        
        [StringLength(200)] 
        public string Anexo1 { get; set; } //DS_ANEXO1
        
        [StringLength(200)] 
        public string Anexo2 { get; set; } //DS_ANEXO2
        
        [StringLength(200)] 
        public string Anexo3 { get; set; } //DS_ANEXO3
        
        [StringLength(200)] 
        public string Anexo4 { get; set; } //DS_ANEXO4
        
        [StringLength(18)] 
        public string Anexo5 { get; set; } //DS_ANEXO5
        public string FechaIngreso { get; set; } //DT_ADDROW
        public string FechaModificacion { get; set; } //DT_UPDROW
        
        [StringLength(2)] 
        public string CodigoClase { get; set; } //CD_CLASSE
        public int? CodigoFamilia { get; set; } //CD_FAMILIA_PRODUTO
        public short? CodigoRotatividad { get; set; } //CD_ROTATIVIDADE
        
        [StringLength(20)] 
        public string GrupoConsulta { get; set; } //CD_GRUPO_CONSULTA
        public short? Exclusivo { get; set; } //CD_EXCLUSIVO
        public short? TipoPeso { get; set; } //TP_PESO_PRODUTO
        
        [StringLength(11)] 
        public string Nivel { get; set; } //CD_NIVEL
        
        [StringLength(1)] 
        public string CodOrigen { get; set; } //CD_ORIGEM
        
        [StringLength(15)] 
        public string ProductoUnico { get; set; } //CD_PRODUTO_UNICO
        public short? Ramo { get; set; } //CD_RAMO_PRODUTO
        
        [StringLength(10)] 
        public string UndMedidaFact { get; set; } //CD_UND_MEDIDA_FACT
        
        [StringLength(10)] 
        public string UndEmb { get; set; } //CD_UNID_EMB
        
        [StringLength(4)] 
        public string DescDifPeso { get; set; } //DS_DIFER_PESO_QTDE
        public decimal? Conversion { get; set; } //FT_CONVERSAO
        
        [StringLength(1)] 
        public string Agrupacion { get; set; } //ID_AGRUPACION
        
        [StringLength(1)] 
        public string IdCrossDocking { get; set; } //ID_CROSS_DOCKING
        
        [StringLength(1)] 
        public string ManejaTomaDato { get; set; } //ID_MANEJA_TOMA_DATO
        
        [StringLength(1)] 
        public string RedondeoValidez { get; set; } //ID_REDONDEO_VALIDEZ
        public decimal? CantidadGenerica { get; set; } //QT_GENERICO
        public int? CantidadPadronStock { get; set; } //QT_PADRON_STOCK

        [StringLength(13)] 
        public string SgProducto { get; set; } //SG_PRODUTO
        public decimal? PrecioDistribucion { get; set; } //VL_PRECIO_DISTRIB
        public decimal? PrecioEgreso { get; set; } //VL_PRECIO_EGRESO
        public decimal? PrecioIngreso { get; set; } //VL_PRECIO_INGRESO
        public decimal? PrecioSegDistribucion { get; set; } //VL_PRECIO_SEG_DISTR
        public decimal? PrecioSegStock { get; set; } //VL_PRECIO_SEG_STOCK
        public decimal? PrecioStock { get; set; } //VL_PRECIO_STOCK

        [StringLength(40)]
        public string CodigoBase { get; set; }               //CODIGO_BASE

        [StringLength(40)]
        public string Talle { get; set; }               //TALLE

        [StringLength(40)]
        public string Color { get; set; }               //COLOR

        [StringLength(40)]
        public string Temporada { get; set; }               //TEMPORADA

        [StringLength(40)]
        public string Categoria1 { get; set; }               //VL_CATEGORIA1

        [StringLength(40)]
        public string Categoria2 { get; set; }               //VL_CATEGORIA2

        [StringLength(20)]
        public string VentanaLiberacion { get; set; }               //CD_VENTANA_LIBERACION
    }
}
