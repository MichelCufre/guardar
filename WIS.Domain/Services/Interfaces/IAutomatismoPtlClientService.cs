using System.Collections.Generic;
using WIS.Domain.Automatismo;
using WIS.Domain.Automatismo.Dtos;
using WIS.Domain.General;

namespace WIS.Domain.Services.Interfaces
{
    public interface IAutomatismoPtlClientService
    {
        (ValidationsResult, PtlColorResponse) GetPtlColor(int userId, string zona);
        (ValidationsResult, string) PrenderLuces(int userId, string codigoPtl, string codigoColor, int empresa, string detail, string referencia, string agrupacion);
        (ValidationsResult, List<PtlCommandConfirmRequest>) GetLightsOnByPtl(string cdZonaUbicacion);
        (ValidationsResult, string) GetPtlByTipoAutomatismo(string tipoAutomatismo);
        (ValidationsResult, List<PtlCommandConfirmRequest>) GetColoresActivosRecords(string ptl);
        (ValidationsResult, bool) ClearColor(string zona, string color, int userId);
        (ValidationsResult, bool) FinishOperation(string zona, string color, int userId);

        (ValidationsResult, bool) UpdateLuzByPtlColor(PtlColorActivoRequest colorActivo);
        (ValidationsResult, bool) ValidatePtlReferencia(string cdZonaUbicacion, string referencia);
        (ValidationsResult, bool) ValidarOperacion(string cdZonaUbicacion, string color, int empresa, string producto);
        (ValidationsResult, List<PtlColorResponse>) GetColoresActivosByPtl(string ptl);
    }
}
