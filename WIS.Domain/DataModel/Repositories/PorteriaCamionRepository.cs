using WIS.Domain.DataModel.Mappers.Porteria;
using WIS.Domain.Extensions;
using WIS.Domain.Porteria;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class PorteriaCamionRepository
    {
        protected readonly WISDB _context;
        protected readonly string _application;
        protected readonly int _userId;
        protected readonly PorteriaCamionMapper _mapper;
        protected readonly IDapper _dapper;

        public PorteriaCamionRepository(WISDB context, string application, int userId, IDapper dapper)
        {
            this._context = context;
            this._application = application;
            this._userId = userId;
            this._mapper = new PorteriaCamionMapper();
            this._dapper = dapper;
        }

        public virtual void AddCamion(PorteriaCamion obj)
        {
            T_PORTERIA_VEHICULO_CAMION entity = this._mapper.MapToEntity(obj);
            entity.NU_PORTERIA_VEHICULO_CAMION = _context.GetNextSequenceValueInt(_dapper, "S_NU_PORTERIA_VEHICULO");
            this._context.T_PORTERIA_VEHICULO_CAMION.Add(entity);
        }
    }
}
