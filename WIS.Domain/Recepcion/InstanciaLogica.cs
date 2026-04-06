using System;
using System.Collections.Generic;

namespace WIS.Domain.Recepcion
{
    public class InstanciaLogica
    {
        public int Id { get; set; } //NU_ALM_LOGICA_INSTANCIA
        public string Descripcion { get; set; } //DS_ALM_LOGICA_INSTANCIA
        public DateTime FechaRegistro { get; set; } //DT_ADDROW
        public DateTime? FechaModificacion { get; set; } //DT_UPDROW
        public string OrdenarAscendente { get; set; } //FL_ORDEN_ASC
        public int Estrategia { get; set; } //NU_ALM_ESTRATEGIA
        public short Logica { get; set; } //NU_ALM_LOGICA
        public short Orden { get; set; } //NU_ORDEN
        public string DatoAuditoria { get; set; } //VL_AUDITORIA
        public List<InstanciaLogicaParametro> Parametros { get; set; }

        public InstanciaLogica()
        {
            Parametros = new List<InstanciaLogicaParametro>();
        }
    }
}