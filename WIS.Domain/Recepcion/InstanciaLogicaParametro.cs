using System;

namespace WIS.Domain.Recepcion
{
    public class InstanciaLogicaParametro
    {
        public int Id { get; set; }
        public int Instancia { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public LogicaParametro Parametro { get; set; }
        public short Logica { get; set; }
        public string Valor { get; set; }
        public string DatoAuditoria { get; set; }

        public InstanciaLogicaParametro()
        {
            Parametro = new LogicaParametro();
        }
    }
}
