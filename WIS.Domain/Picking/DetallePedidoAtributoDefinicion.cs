using System;

namespace WIS.Domain.Picking
{
    public class DetallePedidoAtributoDefinicion
    {
        public long IdConfiguracion { get; set; }           // NU_DET_PED_SAI_ATRIB
        public int IdAtributo { get; set; }                 // ID_ATRIBUTO
        public string IdCabezal { get; set; }               // FL_CABEZAL
        public string Valor { get; set; }                   // VL_ATRIBUTO        
        public long? Transaccion { get; set; }              //NU_TRANSACCION
        public long? TransaccionDelete { get; set; }        //NU_TRANSACCION
        public DateTime FechaAlta { get; set; }             //DT_ADDROW
        public DateTime? FechaModificacion { get; set; }    //DT_UPDROW

        #region Api salida 
        public string Nombre { get; set; }
        #endregion
    }
}
