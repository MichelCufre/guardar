using System.Collections.Generic;
using WIS.Domain.Interfaces;

namespace WIS.Domain.General.API.Dtos.Entrada
{
    public class LpnsResponse : EntradaResponse
    {
        public List<CreateLpnResponse> Lpns { get; set; } = new List<CreateLpnResponse>();

        public LpnsResponse(InterfazEjecucion interfaz, List<CreateLpnResponse> lpns) : base(interfaz)
        {
            Lpns = lpns;
        }
    }

    public class CreateLpnResponse
    {
        public string Tipo { get; set; }
        public string IdExterno { get; set; }
        public long Numero { get; set; }

        public CreateLpnResponse(string tipo, string idExterno, long nuLpn)
        {
            Tipo = tipo;
            IdExterno = idExterno;
            Numero = nuLpn;
        }
    }
}
