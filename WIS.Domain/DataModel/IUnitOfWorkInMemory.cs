using WIS.Data;
using WIS.Domain.DataModel.Repositories;

namespace WIS.Domain.DataModel
{
    public interface IUnitOfWorkInMemory : ITransactionalUnit
    {
        PosicionRepository PosicionRepository { get; }
        ColorRepository ColorRepository { get; }
    }
}
