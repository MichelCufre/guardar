using System;

namespace WIS.Persistence.InMemory
{
    public class PtlColorEnUsoEntity
    {
        public string NU_COLOR { get; set; }
        public int UserId { get; set; }
        public int NU_PTL { get; set; }

        public DateTime DT_ADDROW { get; set; }
        public DateTime DT_ULTIMA_ACCION { get; set; }
        public long Transaccion { get; set; }
    }
}
