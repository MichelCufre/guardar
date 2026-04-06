using WIS.Domain.Interfaces;

namespace WIS.Domain.General.API.Dtos.Entrada
{
    public class EntradaResponse
    {
        public int CodigoEmpresa { get; set; }

        public int CodigoInterfaz { get; set; }

        public long NumeroInterfaz { get; set; }

        public EntradaResponse()
        {
        }

        public EntradaResponse(InterfazEjecucion interfaz)
        {
            CodigoEmpresa = interfaz.Empresa.Value;
            CodigoInterfaz = interfaz.CdInterfazExterna.Value;
            NumeroInterfaz = interfaz.Id;
        }
    }
}
