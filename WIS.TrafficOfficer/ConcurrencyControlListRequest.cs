using System.Collections.Generic;

namespace WIS.TrafficOfficer
{
    public class ConcurrencyControlListRequest  
    {
        public int? Userid { get; set; }
        public string Token_thread { get; set; }
        public string Entidad { get; set; }
        public List<string> Ids_Bloqueo { get; set; }
        public string PaginaOrigen { get; set; }
        public string Transaccion { get; set; }
        public bool IsGlobal { get; set; }
    }
}
