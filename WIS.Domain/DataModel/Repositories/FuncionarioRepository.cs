using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
	public class FuncionarioRepository
    {
        protected readonly WISDB _context;
        protected readonly string _application;
        protected readonly int _userId;
        protected readonly FuncionarioMapper _mapper;

        public FuncionarioRepository(WISDB context, string application, int userId)
        {
            this._context = context;
            this._application = application;
            this._userId = userId;
            this._mapper = new FuncionarioMapper();
        }

        #region Any

        public virtual bool ExisteFuncionario(int codigo)
        {
            return this._context.T_FUNCIONARIO
                .AsNoTracking()
                .Any(x => x.CD_FUNCIONARIO == codigo);
        }

        #endregion

        #region Get

        public virtual Funcionario GetFuncionario(int codigo)
        {
            var funcionario = this._mapper.MapToObject(this._context.T_FUNCIONARIO.FirstOrDefault(x => x.CD_FUNCIONARIO == codigo));

            funcionario.NombreLogin = this._context.USERS.FirstOrDefault(x => x.USERID == codigo).LOGINNAME;

            return funcionario;
        }

        public virtual List<Funcionario> GetFuncionarios()
        {
            var entities = this._context.T_FUNCIONARIO.AsNoTracking().ToList();

            var colFuncionarios = new List<Funcionario>();

            foreach (var entity in entities)
            {
                colFuncionarios.Add(this._mapper.MapToObject(entity));
            }

            return colFuncionarios;
        }

        public virtual bool AnyFuncionarioPermisionByEmpresa(int empresa, int userId)
        {
            return _context.T_EMPRESA_FUNCIONARIO.Any(x => x.USERID == userId && x.CD_EMPRESA == empresa);
        }


        #endregion

        #region Add

        #endregion

        #region Update

        public virtual void UpdateFuncionarioInicioAmigable(int cdFuncioanrio, int numeroOrden)
        {
            T_FUNCIONARIO entity = this._context.T_FUNCIONARIO.FirstOrDefault(x => x.CD_FUNCIONARIO == cdFuncioanrio);
            T_FUNCIONARIO attachedEntity = _context.T_FUNCIONARIO.Local.FirstOrDefault(c => c.CD_FUNCIONARIO == cdFuncioanrio);

            entity.NU_IP_COLECTOR = FacturacionDb.CALCULO_A;
            entity.NU_ORDEN_TRABAJO = numeroOrden;

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_FUNCIONARIO.Attach(entity);
                _context.Entry<T_FUNCIONARIO>(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateFuncionarioFinAmigable(Funcionario funcionario)
        {
            T_FUNCIONARIO entity = this._mapper.MapToEntity(funcionario);
            T_FUNCIONARIO attachedEntity = _context.T_FUNCIONARIO.Local.FirstOrDefault(c => c.CD_FUNCIONARIO == funcionario.Id);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_FUNCIONARIO.Attach(entity);
                _context.Entry<T_FUNCIONARIO>(entity).State = EntityState.Modified;
            }
        }

        #endregion

        #region Remove

        #endregion

        #region Dapper

        #endregion
    }
}
