using System;
using System.Collections.Generic;

namespace WIS.Domain.Eventos
{

    public partial class Bandeja
    {
        public Bandeja()
        {
            LstInstanciaBandeja = new List<InstanciaBandeja>();
        }

        public int NU_EVENTO_BANDEJA { get; set; }

        public int NU_EVENTO { get; set; }

        public string VL_SEREALIZADO { get; set; }

        public EstadoBandeja ND_ESTADO { get; set; }

        public  Evento Evento { get; set; }

        public DateTime DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public List<InstanciaBandeja> LstInstanciaBandeja { get; set; }

        public static EstadoBandeja GetEstado(string value)
        {
            switch (value) 
            {
                case "EST_PEND": return EstadoBandeja.EST_PEND;
                case "EST_CON_ERRORES": return EstadoBandeja.EST_CON_ERRORES;
                case "EST_FIN_CORRECTO": return EstadoBandeja.EST_FIN_CORRECTO;
                case "EST_SIN_LOGIC_EVENTO": return EstadoBandeja.EST_SIN_LOGIC_EVENTO;
                case "EST_INST_DESHABILITADA": return EstadoBandeja.EST_INST_DESHABILITADA;
                default: return EstadoBandeja.Unknown;
            }
        }
    }
}