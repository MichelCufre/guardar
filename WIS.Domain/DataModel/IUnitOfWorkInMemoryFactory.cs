namespace WIS.Domain.DataModel
{
    public interface IUnitOfWorkInMemoryFactory
    {
        UnitOfWorkCoreInMemory GetUnitOfWork();
    }
}
