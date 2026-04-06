using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.General;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class GrupoConsultaRepository
    {
        protected readonly int _userId;
        protected readonly WISDB _context;
        protected readonly IDapper _dapper;
        protected readonly string _cdAplicacion;
        protected readonly GrupoConsultaMapper _mapper;

        public GrupoConsultaRepository(WISDB context, string cdAplicacion, int userId, IDapper dapper)
        {
            this._context = context;
            this._cdAplicacion = cdAplicacion;
            this._userId = userId;
            this._mapper = new GrupoConsultaMapper();
            _dapper = dapper;
        }

        #region Any

        #endregion

        #region Get

        public virtual List<GrupoConsulta> GetGrupoConsultaAsignados(int usuario)
        {
            var user = this._context.USERS.Include("T_GRUPO_CONSULTA_FUNCIONARIO").Include("T_GRUPO_CONSULTA_FUNCIONARIO.T_GRUPO_CONSULTA").FirstOrDefault(x => x.USERID == usuario);

            return user.T_GRUPO_CONSULTA_FUNCIONARIO.Select(x => this._mapper.MapToObject(x.T_GRUPO_CONSULTA)).ToList();
        }

        public virtual List<GrupoConsulta> GetGruposConsulta()
        {
            var entities = this._context.T_GRUPO_CONSULTA.AsNoTracking()
               .ToList();

            var grupos = new List<GrupoConsulta>();

            foreach (var entity in entities)
            {
                grupos.Add(this._mapper.MapToObject(entity));
            }

            return grupos;
        }

        #endregion

        #region Add

        public virtual void AsignarGruposConsultaUsuarios(List<string> grupos, List<int> usuarios)
        {
            foreach (var grupo in grupos)
            {
                var grupoConsulta = this._context.T_GRUPO_CONSULTA.Include("T_GRUPO_CONSULTA_FUNCIONARIO").Where(d => d.CD_GRUPO_CONSULTA == grupo).FirstOrDefault();

                foreach (var usu in usuarios)
                {
                    grupoConsulta.T_GRUPO_CONSULTA_FUNCIONARIO.Add(new T_GRUPO_CONSULTA_FUNCIONARIO
                    {
                        USERS = this._context.USERS.FirstOrDefault(s => s.USERID == usu),
                        T_GRUPO_CONSULTA = grupoConsulta
                    });

                }
            }

        }

        #endregion

        #region Update

        #endregion

        #region Remove

        public virtual void RemoverGruposConsultaUsuarios(List<string> grupos, List<int> usuarios)
        {
            var gcus = this._context.T_GRUPO_CONSULTA_FUNCIONARIO
                .Where(gcu => grupos.Contains(gcu.CD_GRUPO_CONSULTA)
                    && usuarios.Contains(gcu.USERID));

            foreach (var gcu in gcus)
            {
                this._context.T_GRUPO_CONSULTA_FUNCIONARIO.Remove(gcu);
            }
        }

        #endregion

        #region Dapper

        #endregion
    }
}
