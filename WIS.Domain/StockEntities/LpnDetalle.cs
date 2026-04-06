using System;
using System.Collections.Generic;
using WIS.Domain.Parametrizacion;

namespace WIS.Domain.StockEntities
{
    public class LpnDetalle
    {

        public int Id { get; set; }                             //ID_LPN_DET
        public long NumeroLPN { get; set; }                     //NU_LPN
        public int Empresa { get; set; }                        //CD_EMPRESA
        public string CodigoProducto { get; set; }              //CD_PRODUTO
        public decimal Faixa { get; set; }                      //CD_FAIXA
        public string Lote { get; set; }                        //NU_IDENTIFICADOR
        public decimal Cantidad { get; set; }                   //QT_ESTOQUE
        public long? NumeroTransaccion { get; set; }            //NU_TRANSACCION
        public long? NumeroTransaccionDelete { get; set; }      //NU_TRANSACCION_DELETE
        public DateTime? Vencimiento { get; set; }              //DT_FABRICACAO
        public decimal? CantidadDeclarada { get; set; }         //QT_DECLARADA
        public decimal? CantidadRecibida { get; set; }          //QT_RECIBIDA
        public string IdLineaSistemaExterno { get; set; }       //ID_LINEA_SISTEMA_EXTERNO
        public decimal? CantidadReserva { get; set; }           //QT_RESERVA_SAIDA
        public decimal? CantidadExpedida { get; set; }          //QT_EXPEDIDA
        public string IdAveria { get; set; }                    //ID_AVERIA
        public string IdInventario { get; set; }                //ID_INVENTARIO
        public string IdCtrlCalidad { get; set; }               //ID_CTRL_CALIDAD
        public string MotivoAveria { get; set; }                //CD_MOTIVO_AVERIA
        public string DescripcionMotivoAveria { get; set; }     //T_DET_DOMINIO.DS_DOMINIO_VALOR

        #region Api
        public List<AtributoValor> AtributosSinDefinir { get; set; }

        public string IdExterno { get; set; }
        public string Tipo { get; set; }

        public string Ubicacion { get; set; }
        public string Predio { get; set; }
        public int IdDestino { get; set; }                             //ID_LPN_DET_DEST
        public long NumeroLPNDestino { get; set; }                     //NU_LPN_DEST
        public decimal CantidadDestino { get; set; } 
        public int EmpresaDestino { get; set; }
        public string CodigoProductoDestino { get; set; }
        public string IdentificadorDestino { get; set; }
        public DateTime? VencimientoDestino { get;  set; }
        #endregion
    }
}
