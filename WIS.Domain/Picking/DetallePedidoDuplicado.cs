using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Picking
{
    public class DetallePedidoDuplicado
    {
        public string Pedido { get; set; }                      //NU_PEDIDO
        
        public int Empresa { get; set; }                        //CD_EMPRESA
        
        public string Cliente { get; set; }                     //CD_CLIENTE
        
        public decimal Faixa { get; set; }                      //CD_FAIXA
        
        public string Producto { get; set; }                    //CD_PRODUTO
        
        public DateTime? FechaAlta { get; set; }                //DT_ADDROW
        
        public DateTime? FechaModificacion { get; set; }        //DT_UPDROW

        public string IdEspecificaIdentificador { get; set; }   //ID_ESPECIFICA_IDENTIFICADOR
        
        public string Identificador { get; set; }               //NU_IDENTIFICADOR
        
        public string IdLineaSistemaExterno { get; set; }       //ID_LINEA_SISTEMA_EXTERNO
        
        public string TipoLinea { get; set; }                   //TP_LINEA
        
        public decimal? CantidadAnulada { get; set; }           //QT_ANULADO
        
        public decimal? CantidadExpedida { get; set; }          //QT_EXPEDIDO
        
        public decimal? CantidadFacturada { get; set; }         //QT_FACTURADO
        
        public decimal CantidadPedida { get; set; }             //QT_PEDIDO

        public string DatosSerializados { get; set; }           //VL_SERIALIZADO_1

        public long? Transaccion { get; set; }                  //NU_TRANSACCION
        
    }
}
