using System.Collections.Generic;
using WIS.Domain.Automatismo;
using WIS.Domain.General;

namespace WIS.AutomationManager.Interfaces
{
    public interface IPtlService
    {
        PtlColor GetColor(int userId);
        List<PtlColor> GetColores();
        ValidationsResult ValidarColor(string color, int userId);
        ValidationsResult ValidarOperacion(string color, int empresa, string producto);
        ValidationsResult StartOfOperation();
        ValidationsResult ResetOfOperation();
        ValidationsResult PrenderLuces(PtlPosicionEnUso accion);
        ValidationsResult ProcesarConfirmacion(PtlPosicionEnUso accion);
        ValidationsResult CerrarUbicacion(PtlPosicionEnUso accion);
        ValidationsResult ConfirmarCerrarUbicacion(PtlPosicionEnUso accion);
        ValidationsResult DescartarLuz(int posicion, string color);
        List<PtlPosicionEnUso> GetLightsOn();
        void ClearColor(int userId);
        List<AutomatismoPtl> GetPtlByTipo();
        ValidationsResult UpdateLuzByPtlColor(PtlColorActivoRequest colorActivo);
        void FinishOperation(int userId, string nuColor);
        ValidationsResult ValidatePtlReferencia(string referencia);
    }
}
