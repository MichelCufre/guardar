using WIS.Domain.Automatismo;

namespace WIS.AutomationManager.Interfaces
{
    public interface IAutomatismoEjecucionService
    {
        void SetIdentity(string application, int userId);
        AutomatismoEjecucion CrearEjecucion(int? nuAutomatismo, short cdInterfazExterna, string referencia, int? nuAutomatismoInterfaz, string VlIdentityUser);
        void Update(AutomatismoEjecucion obj);
    }
}
