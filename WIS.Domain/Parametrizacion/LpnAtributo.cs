using System.Text.Json.Serialization;

namespace WIS.Domain.Parametrizacion
{
    public class LpnAtributo
    {
        public long NumeroLpn { get; set; }                 //NU_LPN        
        public string Tipo { get; set; }                    //TP_LPN_TIPO
        public int Id { get; set; }                         //ID_ATRIBUTO
        public string Nombre { get; set; }                  //NM_ATRIBUTO
        public string Valor { get; set; }                   //VL_LPN_ATRIBUTO        
        public string Estado { get; set; }                  //ID_ESTADO        
        public long? NumeroTransaccion { get; set; }        //NU_TRANSACCION
        public long? NumeroTransaccionDelete { get; set; }  //NU_TRANSACCION_DELETE
    }
}
