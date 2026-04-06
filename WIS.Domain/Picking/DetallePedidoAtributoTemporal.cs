using System;

namespace WIS.Domain.Picking
{
    public class DetallePedidoAtributoTemporal
    {
        public string Pedido { get; set; }                      //NU_PEDIDO
        public string Cliente { get; set; }                     //CD_CLIENTE
        public int Empresa { get; set; }                        //CD_EMPRESA
        public string Producto { get; set; }                    //NU_DET_PED_SAI_ATRIB
        public decimal Faixa { get; set; }                      //CD_FAIXA
        public string Identificador { get; set; }               //NU_IDENTIFICADOR
        public string IdEspecificaIdentificador { get; set; }   //ID_ESPECIFICA_IDENTIFICADOR
        public int IdAtributo { get; set; }                     // ID_ATRIBUTO
        public int UserId { get; set; }                         // USERID
        public string IdCabezal { get; set; }                   // FL_CABEZAL
        public string Valor { get; set; }                       // VL_ATRIBUTO        
        public long? Transaccion { get; set; }                  //NU_TRANSACCION
        public long? TransaccionDelete { get; set; }            //NU_TRANSACCION
        public DateTime? FechaAlta { get; set; }                //DT_ADDROW
        public DateTime? FechaModificacion { get; set; }        //DT_UPDROW      
    }
}
