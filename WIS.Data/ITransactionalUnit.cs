namespace WIS.Data
{
    public interface ITransactionalUnit
    {
        void BeginTransaction();
        void Commit();
        void Rollback();
        int SaveChanges();
    }
}
