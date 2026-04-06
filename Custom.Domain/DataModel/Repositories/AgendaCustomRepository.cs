using Custom.Domain.DataModel.Mappers;
using Custom.Persistence.Database;
using WIS.Domain.DataModel.Repositories;
using WIS.Domain.Services.Interfaces;

namespace Custom.Domain.DataModel.Repositories
{
    public class AgendaCustomRepository : AgendaRepository
    {
        private readonly CUSTOMDB _context;
        private readonly AgendaCustomMapper _mapper; 
        public AgendaCustomRepository(CUSTOMDB context, string application, int userId, IDapper dapper = null) : base (context, application, userId, dapper)
        {
            this._context = context;
            this._mapper = new AgendaCustomMapper();
        }

        #region >> Add



        #endregion << Add

        #region >> Remove



        #endregion << Remove

        #region >> Update



        #endregion << Update

        #region >> Any



        #endregion << Any

        #region >> Sequence



        #endregion << Sequence

        #region >> Get



        #endregion << Get

        #region >> Auxiliares



        #endregion << Auxiliares

    }
}
