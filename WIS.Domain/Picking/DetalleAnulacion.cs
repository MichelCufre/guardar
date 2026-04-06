using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Picking
{
    public class DetalleAnulacion
    {
        public int NroAnulacionPreparacion { get; set; }    //NU_ANULACION_PREPARACION
        public int Preparacion { get; set; }                //NU_PREPARACION
        public string Cliente { get; set; }                 //CD_CLIENTE
        public int Empresa { get; set; }                    //CD_EMPRESA
        public string Pedido{ get; set; }                   //NU_PEDIDO
        public string Ubicacion { get; set; }               //CD_ENDERECO
        public string Producto { get; set; }                //CD_PRODUTO
        public decimal Faixa { get; set; }                  //CD_FAIXA
        public string Identificador { get; set; }           //NU_IDENTIFICADOR
        public int NroSeqPrepracion { get; set; }           //NU_SEQ_PREPARACION
        public string Observaciones { get; set; }           //DS_OBSERVACIONES
        public DateTime? Alta { get; set; }                 //DT_ADDROW
        public DateTime? Modificacion { get; set; }         //DT_UDPROW
    }
}
