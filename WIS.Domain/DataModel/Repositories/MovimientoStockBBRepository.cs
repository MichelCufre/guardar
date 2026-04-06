using WIS.Domain.DataModel.Mappers;
using WIS.Domain.Extensions;
using WIS.Domain.ManejoStock;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
	public class MovimientoStockBBRepository
	{
		protected readonly WISDB _context;
		protected readonly string _cdAplicacion;
		protected readonly int _userId;
		protected readonly MovimientoStockBBMapper _mapper;
		protected readonly IDapper _dapper;

		public MovimientoStockBBRepository(WISDB context, string cdAplicacion, int userId, IDapper dapper)
		{
			this._context = context;
			this._cdAplicacion = cdAplicacion;
			this._userId = userId;
			this._mapper = new MovimientoStockBBMapper();
			this._dapper = dapper;
		}

        #region Any

        #endregion

        #region Get

        public virtual string GetNumeroMovimiento()
        {
            return this._context.GetNextSequenceValueDecimal(_dapper, "S_MOVIMIENTO_BB").ToString();
        }

        #endregion

        #region Add

        public virtual void AddMovimientoBlackBox(MovimientoStockBlackBox movimiento)
        {
            var entity = this._mapper.MapFromMovimientoBlackBox(movimiento);

            this._context.T_PRDC_MOVIMIENTO_BB.Add(entity);
        }

        #endregion

        #region Update

        #endregion

        #region Remove

        #endregion

        #region Dapper

        #endregion
    }
}
