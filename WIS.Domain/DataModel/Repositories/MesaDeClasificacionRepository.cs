using Microsoft.EntityFrameworkCore;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Extensions;
using WIS.Domain.Recepcion;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class MesaDeClasificacionRepository
    {
        protected readonly WISDB _context;
        protected readonly string _cdAplicacion;
        protected readonly int _userId;
        protected readonly IDapper _dapper;
        protected readonly MesaDeClasificacionMapper _mapper;
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();

        public MesaDeClasificacionRepository(WISDB context, string cdAplicacion, int userId, IDapper dapper)
        {
            this._context = context;
            this._cdAplicacion = cdAplicacion;
            this._userId = userId;
            this._mapper = new MesaDeClasificacionMapper();
            _dapper = dapper;

        }

        #region Any

        public virtual bool AnyEstacionDeClasificacion(int codigo)
        {
            return this._context.T_ESTACION_CLASIFICACION
                .AsNoTracking()
                .Any(x => x.CD_ESTACION == codigo);
        }

        #endregion

        #region Get

        public virtual int GetNextCodigoEstacion()
        {
            return this._context.GetNextSequenceValueInt(_dapper, "S_ESTACION_CLASIFICACION");
        }

        public virtual EstacionDeClasificacion GetEstacionDeClasificacion(int codigo)
        {
            return this._mapper.MapToObject(this._context.T_ESTACION_CLASIFICACION
                .AsNoTracking()
                .FirstOrDefault(x => x.CD_ESTACION == codigo));
        }

        public virtual EstacionDeClasificacion GetEstacionDeClasificacion(string ubicacion)
        {
            return this._mapper.MapToObject(this._context.T_ESTACION_CLASIFICACION
                .AsNoTracking()
                .FirstOrDefault(x => x.CD_ENDERECO == ubicacion));
        }

        public virtual List<EstacionDeClasificacion> GetEstacionByNombreOrCodePartial(int userid, string predio)
        {
            return this._context.T_ESTACION_CLASIFICACION
                .Join(this._context.T_PREDIO_USUARIO
                    .Where(pu => pu.USERID == userid
                        && (predio == GeneralDb.PredioSinDefinir || pu.NU_PREDIO == predio)),
                    ec => new { ec.NU_PREDIO },
                    pu => new { pu.NU_PREDIO },
                    (ec, ee) => ec)
                .AsNoTracking()
                .Select(e => this._mapper.MapToObject(e))
                .ToList();
        }

        public virtual int GetTotalSugerenciasDeClasificacionExcluyendoEquiposVacios(EstacionDeClasificacion estacion, string destino, string zona)
        {
            zona = string.IsNullOrEmpty(zona) ? null : zona;
            return _context.V_REC410_SUGERENCIAS
                .Where(s => s.NU_PREDIO == estacion.Predio
                    && s.CD_ESTACION == estacion.Codigo
                    && s.TP_OPERATIVA == TipoOperacionDb.Clasificacion
                    && ((string.IsNullOrEmpty(s.CD_ENDERECO_DESTINO) && s.CD_ZONA == zona)
                        || (!string.IsNullOrEmpty(destino) && s.CD_ENDERECO_DESTINO == destino)))
                .Count();
        }

        public virtual List<SugerenciaDeClasificacion> GetSugerenciasDeClasificacion(EstacionDeClasificacion estacion, string destino, string zona)
        {
            zona = string.IsNullOrEmpty(zona) ? null : zona;
            return _context.V_REC410_SUGERENCIAS
                .AsNoTracking()
                .Where(s => s.NU_PREDIO == estacion.Predio
                    && (!s.CD_ESTACION.HasValue || s.CD_ESTACION == estacion.Codigo)
                    && ((string.IsNullOrEmpty(s.CD_ENDERECO_DESTINO) && string.IsNullOrEmpty(s.CD_ZONA) && string.IsNullOrEmpty(s.TP_OPERATIVA)) // Equipo vacio
                        || (s.TP_OPERATIVA == TipoOperacionDb.Clasificacion
                            && (
                                (string.IsNullOrEmpty(s.CD_ENDERECO_DESTINO) && s.CD_ZONA == zona)
                                || (!string.IsNullOrEmpty(destino) && s.CD_ENDERECO_DESTINO == destino))
                            )
                        ))
                .Select(s => this._mapper.MapToObject(s))
                .ToList();
        }

        public virtual List<DetalleEtiquetaSinClasificar> GetEtiquetaConStockSinClasificar(int nuEtiquetaLote)
        {
            return _context.V_DET_ETIQUETA_SIN_CLASIFICAR
                  .Where(s => s.NU_ETIQUETA_LOTE == nuEtiquetaLote)
                  .Select(s => this._mapper.MapToEntity(s)).ToList();
        }

        #endregion

        #region Add

        public virtual void AddEstacion(EstacionDeClasificacion estacion)
        {
            T_ESTACION_CLASIFICACION entity = this._mapper.MapToEntity(estacion);
            this._context.T_ESTACION_CLASIFICACION.Add(entity);
        }

        #endregion

        #region Update

        public virtual void UpdateEstacion(EstacionDeClasificacion estacion)
        {
            T_ESTACION_CLASIFICACION entity = this._mapper.MapToEntity(estacion);
            T_ESTACION_CLASIFICACION attachedEntity = _context.T_ESTACION_CLASIFICACION.Local
                .FirstOrDefault(c => c.CD_ESTACION == estacion.Codigo);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_ESTACION_CLASIFICACION.Attach(entity);
                _context.Entry<T_ESTACION_CLASIFICACION>(entity).State = EntityState.Modified;
            }
        }
      
        #endregion

        #region Remove

        #endregion
    }
}