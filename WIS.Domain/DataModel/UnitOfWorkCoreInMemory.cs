using System;
using WIS.Domain.DataModel.Repositories;
using WIS.Persistence.InMemory;

namespace WIS.Domain.DataModel
{
    public class UnitOfWorkCoreInMemory : IUnitOfWorkInMemory, IDisposable
    {
        protected WISDBInMemory _context { get; private set; }

        protected readonly string _application;
        protected readonly int _userId;


        #region General
        public UnitOfWorkCoreInMemory(IDatabaseOptionProvider dbConfigService, string application, int userId, bool openContext = true)
        {
            if (openContext)
            {
                this.SetContext(new WISDBInMemory(dbConfigService));
            }

            this._application = application;
            this._userId = userId;
        }

        protected virtual void SetContext(WISDBInMemory context)
        {
            this._context = context;
        }



        public void BeginTransaction()
        {
            if (this._context.GetNumberTransaction() == 0)
            {
                this._context.CreateNumberTransaccion();
            }

        }

        public void Commit()
        {
            this._context.ClearTransaction();
        }

        public void Dispose()
        {
            this._context.Dispose();
        }

        public void Rollback()
        {
            if (this._context.GetNumberTransaction() != 0)
            {
                this._context.RemoveAllByTransaction();
                this.SaveChanges();
            }
        }



        public int SaveChanges()
        {
            return this._context.SaveChanges();
        }

        #endregion

        #region Repository

        private PosicionRepository _PosicionRepository; public PosicionRepository PosicionRepository => this._PosicionRepository ?? (this._PosicionRepository = new PosicionRepository(this._context, this._application, this._userId));

        private ColorRepository _ColorRepository; public ColorRepository ColorRepository => this._ColorRepository ?? (this._ColorRepository = new ColorRepository(this._context, this._application, this._userId));

        #endregion
    }
}
