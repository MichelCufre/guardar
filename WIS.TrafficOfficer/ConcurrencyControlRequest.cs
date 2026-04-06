namespace WIS.TrafficOfficer
{
    public class ConcurrencyControlRequest
    {
        public int? Userid { get; set; }
        public string Token_thread { get; set; }
        public string Entidad { get; set; }
        public string Id_Bloqueo { get; set; }
        public string PaginaOrigen { get; set; }
        public string PaginaDestino { get; set; }
        public string Transaccion { get; set; }
        public string Sistema { get; set; }
        public bool IsGlobal { get; set; }
    }
}
