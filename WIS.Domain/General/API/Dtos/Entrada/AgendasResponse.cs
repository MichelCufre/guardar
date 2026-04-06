using System.Collections.Generic;
using WIS.Domain.Interfaces;

namespace WIS.Domain.General.API.Dtos.Entrada
{
    public class AgendasResponse : EntradaResponse
    {
        public List<int> Agendas { get; set; } = new List<int>();

        public AgendasResponse(InterfazEjecucion interfaz, List<int> agendas) : base(interfaz)
        {
            Agendas = agendas;
        }
    }
}
