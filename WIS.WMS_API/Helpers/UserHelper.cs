using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Services.Interfaces;

namespace WIS.WMS_API.Helpers
{
    public static class UserHelper
    {
        public static List<string> GetGruposConsulta(HttpContext context, int empresa, IEjecucionService ejecucionService, IParameterService parameterService)
        {
            if (parameterService.GetValueByEmpresa(ParamManager.IS_FILTRA_GRUPO_CONSULTA, empresa) == "S")
            {
                string loginName = GetCurrentLoginName(context);
                return ejecucionService.GetGruposConsulta(loginName);
            }
            return null;
        }

        public static string GetCurrentLoginName(HttpContext context)
        {
            return context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;
        }
    }
}
