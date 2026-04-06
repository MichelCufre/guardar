using Microsoft.EntityFrameworkCore;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Extensions;
using WIS.Domain.Recepcion;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class EtiquetaTransferenciaRepository
    {
        protected readonly WISDB _context;
        protected readonly string _cdAplicacion;
        protected readonly int _userId;
        protected readonly EtiquetaTransferenciaMapper _mapper;
        protected readonly IDapper _dapper;

        public EtiquetaTransferenciaRepository(WISDB context, string cdAplicacion, int userId, IDapper dapper)
        {
            this._context = context;
            this._cdAplicacion = cdAplicacion;
            this._userId = userId;
            this._mapper = new EtiquetaTransferenciaMapper();
            this._dapper = dapper;
        }

        #region Any

        #endregion

        #region Get

        public virtual EtiquetaTransferencia GetEtiquetaTransferencia(decimal etiqueta)
        {
            return this._mapper.MapToObject(this._context.T_PALLET_TRANSFERENCIA
                .AsNoTracking()
                .FirstOrDefault(x => x.NU_ETIQUETA == etiqueta));
        }

        public virtual EtiquetaTransferencia GetEtiquetaTransferencia(string tpEtiqueta, string nuExterno)
        {
            return this._mapper.MapToObject(this._context.T_PALLET_TRANSFERENCIA
                .AsNoTracking()
                .FirstOrDefault(e => e.TP_ETIQUETA_TRANSFERENCIA == tpEtiqueta
                    && e.ID_EXTERNO_ETIQUETA == nuExterno 
                    && e.CD_SITUACAO == SituacionDb.EnTransferencia));
        }

		public virtual DetalleEtiquetaTransferencia GetDetalleEtiquetaTransferencia(decimal nuEtiqueta, int nuSecEtiqueta, string ubicacionOrigen, string identificador, string codigoProducto, decimal faixa, int empresa)
		{

			var detalle = _context.T_DET_PALLET_TRANSFERENCIA
				.AsNoTracking()
				.FirstOrDefault(e => e.NU_ETIQUETA == nuEtiqueta &&
									 e.NU_SEC_ETIQUETA == nuSecEtiqueta &&
									 e.CD_ENDERECO_ORIGEN == ubicacionOrigen &&
									 e.NU_IDENTIFICADOR == identificador &&
									 e.CD_PRODUTO == codigoProducto &&
									 e.CD_FAIXA == faixa &&
									 e.CD_EMPRESA == empresa);

			if (detalle == null)
				return null;

			return this._mapper.MapToObject(detalle);
		}

		public virtual decimal GetProximoNumeroEtiqueta()
        {
            return this._context.GetNextSequenceValueDecimal(_dapper, Secuencias.S_TRAN_NRO);
        }

        public virtual int GetProximoNumeroSecDetalle(EtiquetaTransferencia etiqueta)
        {
            return _context.T_DET_PALLET_TRANSFERENCIA
                .AsNoTracking()
                .AsEnumerable()
                .Where(d => d.NU_ETIQUETA == etiqueta.NumeroEtiqueta
                    && d.NU_SEC_ETIQUETA == etiqueta.NumeroSecEtiqueta)
                .OrderByDescending(d => d.NU_SEC_DETALLE)
                .Select(d => d.NU_SEC_DETALLE + 1)
                .DefaultIfEmpty(0)
                .FirstOrDefault();
        }

		#endregion

        #region Add

        public virtual void AddEtiqueta(EtiquetaTransferencia etiqueta)
        {
            T_PALLET_TRANSFERENCIA entity = this._mapper.MapToEntity(etiqueta);

            this._context.T_PALLET_TRANSFERENCIA.Add(entity);
        }

        public virtual void AddDetalleEtiqueta(DetalleEtiquetaTransferencia detalle)
        {
            T_DET_PALLET_TRANSFERENCIA entity = this._mapper.MapToEntity(detalle);

            this._context.T_DET_PALLET_TRANSFERENCIA.Add(entity);
        }

        #endregion

		#region Update

		public virtual void UpdateDetalleEtiquetaTransferencia(DetalleEtiquetaTransferencia detalle)
		{
			var entity = this._mapper.MapToEntity(detalle);

			var attachedEntity = _context.T_DET_PALLET_TRANSFERENCIA.Local.FirstOrDefault(e => e.NU_ETIQUETA == detalle.NumeroEtiqueta &&
									 e.NU_SEC_ETIQUETA == detalle.NumeroSecEtiqueta &&
									 e.CD_ENDERECO_ORIGEN == detalle.UbicacionOrigen &&
									 e.NU_IDENTIFICADOR == detalle.Identificador &&
									 e.CD_PRODUTO == detalle.Producto &&
									 e.CD_FAIXA == detalle.Faixa &&
									 e.CD_EMPRESA == detalle.Empresa &&
									 e.NU_SEC_DETALLE == detalle.NumeroSecDetalle);

			if (attachedEntity != null)
			{
				var attachedEntry = _context.Entry(attachedEntity);
				attachedEntry.CurrentValues.SetValues(entity);
				attachedEntry.State = EntityState.Modified;
			}
			else
			{
				_context.T_DET_PALLET_TRANSFERENCIA.Attach(entity);
				_context.Entry(entity).State = EntityState.Modified;
			}
		}

        #endregion

        #region Remove

        #endregion

        #region Dapper

        #endregion
    }
}
