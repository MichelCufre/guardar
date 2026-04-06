using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.Extensions;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.StockEntities;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class UnidadTransporteRepository
    {
        protected readonly UnidadTransporteMapper _mapper;
        protected readonly WISDB _context;
        protected readonly IDapper _dapper;

        public UnidadTransporteRepository(WISDB context, string application, int userId, IDapper dapper)
        {
            this._context = context;
            this._mapper = new UnidadTransporteMapper();
            this._dapper = dapper;
        }

        public virtual int GetProximoNumeroEtiquetaUnidadTransporte()
        {
            return this._context.GetNextSequenceValueInt(this._dapper, Secuencias.S_UT_EXTERNO_NRO);
        }

        public virtual int GetProximoNumeroUnidadTransporte()
        {
            return this._context.GetNextSequenceValueInt(this._dapper, Secuencias.S_UNIDAD_TRANSPORTE);
        }

        public virtual UnidadTransporte GetUnidadTransporte(int nroUt)
        {
            var entity = _context.T_UNIDAD_TRANSPORTE.FirstOrDefault(f => f.NU_UNIDAD_TRANSPORTE == nroUt);
            return this._mapper.MapToObject(entity);
        }
    }
}
