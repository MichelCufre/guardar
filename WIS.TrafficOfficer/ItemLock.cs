namespace WIS.TrafficOfficer
{
    public class ItemLock
    {
        public int? Userid { get; set; }
        public string Pagina { get; set; }
        public string Sistema { get; set; }
        public bool Mono_hilo { get; set; }
        public string Token_thread { get; set; }
        public string Entidad { get; set; }
        public string Id_Bloqueo { get; set; }
        public string Estado { get; set; }
        public string PaginaOrigen { get; set; }
        public string PaginaDestino { get; set; }

        public string IngresoThread { get; set; }
        public string UltimaActividadThread { get; set; }
        public string IngresoBloqueo { get; set; }
        public string UltimaActividadBloqueo { get; set; }

        public string Transaccion { get; set; }
        public bool IsGlobal { get; set; }
    }
}
