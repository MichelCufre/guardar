using System;

namespace WIS.Domain.Picking
{
    public class DetallePedidoConfigAtributo
    {
        public long IdConfiguracion { get; set; }           // NU_DET_PED_SAI_ATRIB
        public int IdAtributo { get; set; }                 // ID_ATRIBUTO
        public string Nombre { get; set; }                  // NM_ATRIBUTO
        public string Valor { get; set; }                   // VL_ATRIBUTO
        public string IdCabezal { get; set; }               // FL_CABEZAL
        public long? Transaccion { get; set; }              //NU_TRANSACCION
        public DateTime FechaAlta { get; set; }             //DT_ADDROW
        public DateTime? FechaModificacion { get; set; }    //DT_UPDROW
        public Guid InternalId { get; set; }
        public string Tipo { get; set; }
    }
}
