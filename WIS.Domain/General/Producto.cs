
using System;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General.Enums;

namespace WIS.Domain.General
{
    public class Producto
    {
        private readonly ProductoMapper _mapper = new ProductoMapper();

        public Producto()
        {
        }

        public Producto(string manejoIdentificadorId, string aceptaDecimalesId)
        {
            ManejoIdentificadorId = manejoIdentificadorId;
            AceptaDecimalesId = aceptaDecimalesId;
        }

        public string Codigo { get; set; }                   //CD_PRODUTO
        public int CodigoEmpresa { get; set; }               //CD_EMPRESA
        public string CodigoProductoEmpresa { get; set; }    //CD_PRODUTO_EMPRESA
        public string Descripcion { get; set; }              //DS_PRODUTO
        public string DescripcionReducida { get; set; }      //DS_REDUZIDA
        public string NAM { get; set; }                      //CD_NAM
        public string CodigoMercadologico { get; set; }      //CD_MERCADOLOGICO
        public string TipoManejoFecha { get; set; }          //TP_MANEJO_FECHA
        public short Situacion { get; set; }                 //CD_SITUACAO
        public DateTime? FechaSituacion { get; set; }        //DT_SITUACAO
        public ManejoIdentificador ManejoIdentificador { get; set; } //ID_MANEJO_IDENTIFICADOR
        public bool AceptaDecimales { get; set; }            //FL_ACEPTA_DECIMALES
        public string UnidadMedida { get; set; }             //CD_UNIDADE_MEDIDA 
        public short? DiasLiberacion { get; set; }           //QT_DIAS_VALIDADE_LIBERACION
        public short? DiasDuracion { get; set; }             //QT_DIAS_DURACAO
        public short? DiasValidez { get; set; }              //QT_DIAS_VALIDADE
        public string ModalidadIngresoLote { get; set; }     //ND_MODALIDAD_INGRESO_LOTE
        public int? StockMinimo { get; set; }                //QT_ESTOQUE_MINIMO
        public int? StockMaximo { get; set; }                //QT_ESTOQUE_MAXIMO
        public decimal? PesoBruto { get; set; }              //PS_BRUTO
        public decimal? PesoNeto { get; set; }               //PS_LIQUIDO
        public decimal? Altura { get; set; }                 //VL_ALTURA
        public decimal? Ancho { get; set; }                  //VL_LARGURA
        public decimal? Profundidad { get; set; }            //VL_PROFUNDIDADE
        public decimal? VolumenCC { get; set; }              //VL_CUBAGEM
        public decimal UnidadBulto { get; set; }             //QT_UND_BULTO
        public decimal UnidadDistribucion { get; set; }      //QT_UND_DISTRIBUCION
        public decimal? AvisoAjusteInventario { get; set; }  //VL_AVISO_AJUSTE
        public string TipoDisplay { get; set; }              //TP_DISPLAY
        public string DescripcionDisplay { get; set; }       //DS_DISPLAY
        public short? SubBulto { get; set; }                 //QT_SUBBULTO
        public decimal? UltimoCosto { get; set; }            //VL_CUSTO_ULT_ENT
        public decimal? PrecioVenta { get; set; }            //VL_PRECO_VENDA
        public string AyudaColector { get; set; }            //DS_HELP_COLECTOR
        public string Componente1 { get; set; }              //ND_FACTURACION_COMP1
        public string Componente2 { get; set; }              //ND_FACTURACION_COMP2
        public string Anexo1 { get; set; }                   //DS_ANEXO1
        public string Anexo2 { get; set; }                   //DS_ANEXO2
        public string Anexo3 { get; set; }                   //DS_ANEXO3
        public string Anexo4 { get; set; }                   //DS_ANEXO4
        public string Anexo5 { get; set; }                   //DS_ANEXO5
        public DateTime? FechaIngreso { get; set; }          //DT_ADDROW
        public DateTime? FechaModificacion { get; set; }     //DT_UPDROW
        public string CodigoClase { get; set; }              //CD_CLASSE
        public int CodigoFamilia { get; set; }              //CD_FAMILIA_PRODUTO
        public short? CodigoRotatividad { get; set; }        //CD_ROTATIVIDADE
        public string GrupoConsulta { get; set; }            //CD_GRUPO_CONSULTA
        public short? Exclusivo { get; set; }                //CD_EXCLUSIVO
        public short? TipoPeso { get; set; }                 //TP_PESO_PRODUTO
        public string Nivel { get; set; }                    //CD_NIVEL
        public string CdOrigen { get; set; }                //CD_ORIGEM
        public string ProductoUnico { get; set; }            //CD_PRODUTO_UNICO
        public short Ramo { get; set; }                     //CD_RAMO_PRODUTO
        public string UndMedidaFact { get; set; }           //CD_UND_MEDIDA_FACT
        public string UndEmb { get; set; }                   //CD_UNID_EMB
        public string DescDifPeso { get; set; }              //DS_DIFER_PESO_QTDE
        public decimal? Conversion { get; set; }             //FT_CONVERSAO
        public string Agrupacion { get; set; }               //ID_AGRUPACION
        public string IdCrossDocking { get; set; }           //ID_CROSS_DOCKING
        public string ManejaTomaDato { get; set; }           //ID_MANEJA_TOMA_DATO
        public string RedondeoValidez { get; set; }          //ID_REDONDEO_VALIDEZ
        public decimal? CantidadGenerica { get; set; }       //QT_GENERICO
        public int? CantidadPadronStock { get; set; }        //QT_PADRON_STOCK
        public string SgProducto { get; set; }               //SG_PRODUTO
        public decimal? PrecioDistribucion { get; set; }     //VL_PRECIO_DISTRIB
        public decimal? PrecioEgreso { get; set; }           //VL_PRECIO_EGRESO
        public decimal? PrecioIngreso { get; set; }          //VL_PRECIO_INGRESO
        public decimal? PrecioSegDistribucion { get; set; }  //VL_PRECIO_SEG_DISTR
        public decimal? PrecioSegStock { get; set; }         //VL_PRECIO_SEG_STOCK
        public decimal? PrecioStock { get; set; }            //VL_PRECIO_STOCK
        public string CodigoBase { get; set; }               //CODIGO_BASE
        public string Talle { get; set; }                   //TALLE
        public string Color { get; set; }               //COLOR
        public string Temporada { get; set; }               //TEMPORADA
        public string Categoria1 { get; set; }               //VL_CATEGORIA_01
        public string Categoria2 { get; set; }               //VL_CATEGORIA_02
        public long? NumeroTransaccion { get; set; }         //NU_TRANSACCION
        public string VentanaLiberacion { get; set; }

        #region WMS_API
        public string ManejoIdentificadorId { get; set; }  //ID_MANEJO_IDENTIFICADOR
        public string AceptaDecimalesId { get; set; }      //FL_ACEPTA_DECIMALES
        #endregion

        public Empresa Empresa { get; set; }
        public ProductoFamilia Familia { get; set; }
        public Clase Clase { get; set; }
        public ProductoRotatividad Rotatividad { get; set; }

        public virtual DateTime? GetFechaVencimiento()
        {
            if (this.DiasDuracion == null || this.DiasDuracion <= 0 || !ManejaFechaVencimiento())
                return null;

            return DateTime.Now.Date.AddDays((double)this.DiasDuracion);
        }

        public virtual bool IsFifo()
        {
            return this.TipoManejoFecha == ManejoFechaProductoDb.Fifo;
        }

        public virtual bool IsFefo()
        {
            return this.TipoManejoFecha == ManejoFechaProductoDb.Expirable;
        }

        public virtual bool ManejaFechaVencimiento()
        {
            return this.IsFifo() || this.IsFefo();
        }

        public virtual string GetDefaultIdentificador()
        {
            return this.IsIdentifiedByProducto() ? ManejoIdentificadorDb.IdentificadorProducto : string.Empty;
        }

        public virtual bool IsIdentifiedByProducto()
        {
            return this.ManejoIdentificador == ManejoIdentificador.Producto;
        }

        public virtual bool IsIdentifiedByLote()
        {
            return this.ManejoIdentificador == ManejoIdentificador.Lote;
        }

        public virtual bool IsIdentifiedBySerie()
        {
            return this.ManejoIdentificador == ManejoIdentificador.Serie;
        }

        public virtual string ParseIdentificador(string nroIdentificador)
        {
            if (string.IsNullOrEmpty(nroIdentificador))
            {
                if (this.IsIdentifiedByLote() || this.IsIdentifiedBySerie())
                    return ManejoIdentificadorDb.IdentificadorAuto;

                return ManejoIdentificadorDb.IdentificadorProducto;
            }

            if (this.IsIdentifiedByProducto())
                return ManejoIdentificadorDb.IdentificadorProducto;

            return nroIdentificador.ToUpper();
        }

        public static bool EspecificaIdentificador(string identificador)
        {
            return !string.IsNullOrEmpty(identificador) && identificador != ManejoIdentificadorDb.IdentificadorAuto;
        }
    }
}
