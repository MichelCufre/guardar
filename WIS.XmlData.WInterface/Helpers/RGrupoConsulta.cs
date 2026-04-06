using Microsoft.EntityFrameworkCore;
using WIS.Persistence.Database;

namespace WIS.XmlData.WInterface.Helpers
{
    public class RGrupoConsulta
    {
        public List<T_GRUPO_CONSULTA> GetGrupoConsultaUsuario(WISDB context, int userid)
        {
            return context.T_GRUPO_CONSULTA_FUNCIONARIO
                .Join(context.T_GRUPO_CONSULTA,
                    gcf => gcf.CD_GRUPO_CONSULTA,
                    gc => gc.CD_GRUPO_CONSULTA,
                    (gcf, gc) => new { GrupoConsultaFuncionario = gcf, GrupoConsulta = gc })
                .AsNoTracking()
                .Where(x => x.GrupoConsultaFuncionario.USERID == userid)
                .Select(x => x.GrupoConsulta)
                .ToList();
        }
    }
}
