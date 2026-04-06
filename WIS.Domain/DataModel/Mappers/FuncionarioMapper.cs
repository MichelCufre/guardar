using WIS.Domain.General;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class FuncionarioMapper : Mapper
    {

        public virtual T_FUNCIONARIO MapToEntity(Funcionario func)
        {
            return new T_FUNCIONARIO
            {
                CD_ATIVIDADE = func.Actividad,
                CD_EQUIPO = func.Equipo,
                CD_FUNCIONARIO = func.Id,
                CD_IDIOMA = func.Idioma,
                CD_OPID = func.OperadorId,
                CD_SITUACAO = func.Estado,
                CD_USER_ORCLE = func.UsuarioOracle,
                CD_USER_UNIX = func.UsuarioUnix,
                DS_DIR_ARCHIVOS_EXCEL = func.DireccionArchivosExcel,
                DS_EMAIL = func.Email,
                DS_FUNCAO = func.Descripcion,
                DT_ADDROW = func.FechaInsercion,
                DT_UPDROW = func.FechaModificacion,
                NM_ABREV_FUNC = func.NombreAbreviado,
                NM_FUNCIONARIO = func.Nombre,
                NU_IP_COLECTOR = func.IpColector,
                NU_ORDEN_TRABAJO = func.OrdenTrabajo,
                NU_PTS = func.Puntos,
                QT_CARGA_HORARIA = func.CargaHoraria

            };
        }

        public virtual Funcionario MapToObject(T_FUNCIONARIO func)
        {
            return new Funcionario
            {
                Actividad = func.CD_ATIVIDADE,
                Equipo = func.CD_EQUIPO,
                Id = func.CD_FUNCIONARIO,
                Idioma = func.CD_IDIOMA,
                OperadorId = func.CD_OPID,
                Estado = func.CD_SITUACAO,
                UsuarioOracle = func.CD_USER_ORCLE,
                UsuarioUnix = func.CD_USER_UNIX,
                DireccionArchivosExcel = func.DS_DIR_ARCHIVOS_EXCEL,
                Email = func.DS_EMAIL,
                Descripcion = func.DS_FUNCAO,
                FechaInsercion = func.DT_ADDROW,
                FechaModificacion = func.DT_UPDROW,
                NombreAbreviado = func.NM_ABREV_FUNC,
                Nombre = func.NM_FUNCIONARIO,
                IpColector = func.NU_IP_COLECTOR,
                Puntos = func.NU_PTS,
                OrdenTrabajo = func.NU_ORDEN_TRABAJO,
                CargaHoraria = func.QT_CARGA_HORARIA,
            };
        }

    }
}
