using Microsoft.EntityFrameworkCore;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.Facturacion;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class CuentaContableRepository
    {
        protected readonly WISDB _context;
        protected readonly string _cdAplicacion;
        protected readonly int _userId;
        protected readonly CuentaContablenMapper _mapper;

        public CuentaContableRepository(WISDB context, string cdAplicacion, int userId)
        {
            this._context = context;
            this._cdAplicacion = cdAplicacion;
            this._userId = userId;
            this._mapper = new CuentaContablenMapper();
        }

        #region Any

        public virtual bool ExisteCuentaContable(string idCuentaContable)
        {
            return this._context.T_FACTURACION_CUENTA_CONTABLE.Any(cb => cb.NU_CUENTA_CONTABLE == idCuentaContable);
        }

        #endregion

        #region Get

        public virtual CuentaContable GetCuentaContable(string idCuentaContable)
        {
            return this._mapper.MapToObject(this._context.T_FACTURACION_CUENTA_CONTABLE.FirstOrDefault(x => x.NU_CUENTA_CONTABLE == idCuentaContable));
        }

        #endregion

        #region Add
        public virtual void AddCuentaContable(CuentaContable cuentaContable)
        {
            T_FACTURACION_CUENTA_CONTABLE entity = this._mapper.MapToEntity(cuentaContable);
            this._context.T_FACTURACION_CUENTA_CONTABLE.Add(entity);
        }
        #endregion

        #region Update
        public virtual void UpdateCuentaContable(CuentaContable cuentaContable)
        {
            T_FACTURACION_CUENTA_CONTABLE entity = this._mapper.MapToEntity(cuentaContable);
            T_FACTURACION_CUENTA_CONTABLE attachedEntity = _context.T_FACTURACION_CUENTA_CONTABLE.Local
                .FirstOrDefault(w => w.NU_CUENTA_CONTABLE == entity.NU_CUENTA_CONTABLE);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_FACTURACION_CUENTA_CONTABLE.Attach(entity);
                _context.Entry<T_FACTURACION_CUENTA_CONTABLE>(entity).State = EntityState.Modified;
            }
        }

        #endregion

        #region Remove

        #endregion

        #region Dapper

        #endregion
    }
}
