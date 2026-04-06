using Custom.Domain.DataModel.Repositories;
using Custom.Persistence.Database;
using WIS.Data;
using WIS.Domain.DataModel;

namespace Custom.Domain.DataModel
{
    public interface IUnitOfWorkCustom : IUnitOfWork, IQueryObjectHandler<CUSTOMDB>
    {
        AgendaCustomRepository AgendaCustomRepository { get; }
    }
}
