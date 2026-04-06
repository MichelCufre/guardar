using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Picking
{
    public class DetallePedidoExpedido
    {

        public int Camion { get; set; }               //CD_CAMION
        public string Pedido { get; set; }            //NU_PEDIDO                                                      
        public string Cliente { get; set; }           //CD_CLIENTE                                                    
        public int Empresa { get; set; }              //CD_EMPRESA
        public string Producto { get; set; }          //CD_PRODUTO                                                      
        public string Identificador { get; set; }     //NU_IDENTIFICADOR
        public decimal Faixa { get; set; }            //CD_FAIXA
        public decimal? Cantidad { get; set; }        //QT_PRODUTO
        public DateTime? FechaExpedicion { get; set; }//DT_EXPEDICION
        public bool EspecificaLote { get; set; }      //ID_ESPECIFICA_IDENTIFICADOR

        #region WMS_API
        public string EspecificaLoteId { get; set; }  //ID_ESPECIFICA_IDENTIFICADOR
        public string Memo { get; set; }
        public string Serializado { get; set; }
        #endregion
    }
}
