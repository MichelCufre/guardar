namespace WIS.Domain.DataModel
{
    public interface IUnitOfWorkFactory
    {
        UnitOfWork GetUnitOfWork();
    }
}
