using System.Collections.Generic;

namespace WIS.Domain.Automatismo.Interfaces
{
    public interface IPtl : IAutomatismo
    {
        public List<PtlColor> GetColoresActivos();
        string GetColorCerrado();
        string GetColorError();
        string GetTipo();
        int GetTipoAgrupacion();
        bool ManejaApagadoLuz();
        string GetCodigoError();
        bool RequiereConfirmacionCierre();
        string GetCodigoCancelacion();
        string GetCodigoLleno();
        string GetCodigoCierre();
    }
}
