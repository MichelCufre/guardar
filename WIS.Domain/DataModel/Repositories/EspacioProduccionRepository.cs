using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Extensions;
using WIS.Domain.Produccion.Mappers;
using WIS.Domain.Produccion.Models;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class EspacioProduccionRepository
    {
        protected readonly WISDB _context;
        protected readonly string _application;
        protected readonly int _userId;
        protected readonly IDapper _dapper;
        protected readonly EspacioProduccionMapper _mapper;

        public EspacioProduccionRepository(WISDB context, string application, int userId, IDapper dapper)
        {
            this._context = context;
            this._application = application;
            this._userId = userId;
            this._dapper = dapper;
            _mapper = new EspacioProduccionMapper();
        }

        #region Any

        public virtual bool AnyEspacioProduccionPredio(string predio)
        {
            return _context.T_PRDC_LINEA.Any(x => x.NU_PREDIO == predio);
        }

        public virtual bool AnyIngresoActivoEspacio(string idEspacio)
        {
            return _context.T_PRDC_INGRESO.Any(x => x.CD_PRDC_LINEA == idEspacio
                && x.CD_SITUACAO != SituacionDb.PRODUCCION_FINALIZADA);
        }

        public virtual bool AnyContenedorAsociado(string idIngreso)
        {
            string idEspacioOriginal = _context.T_PRDC_INGRESO.FirstOrDefault(x => x.NU_PRDC_INGRESO == idIngreso).CD_PRDC_LINEA?.ToString();
            if (!string.IsNullOrEmpty(idEspacioOriginal))
            {
                EspacioProduccion espacio = _mapper.MapToObjet(_context.T_PRDC_LINEA.FirstOrDefault(x => x.CD_PRDC_LINEA == idEspacioOriginal));
                return _context.V_CONTENEDORES_PRODUCCION.Any(x => (x.CD_ENDERECO == espacio.IdUbicacionEntrada
                    || x.CD_ENDERECO == espacio.IdUbicacionProduccion)
                    && x.NU_PRDC_INGRESO == idIngreso);
            }
            else
                return false;

        }

        #endregion

        #region Get

        public virtual EspacioProduccion GetEspacioProduccion(string id)
        {
            T_PRDC_LINEA entity = _context.T_PRDC_LINEA
                .AsNoTracking()
                .FirstOrDefault(s => s.CD_PRDC_LINEA == id);

            return entity == null ? null : _mapper.MapToObjet(entity);
        }

        public virtual EspacioProduccion GetEspacioProduccionByIngreso(string id)
        {
            T_PRDC_LINEA entity = _context.T_PRDC_INGRESO.Where(x => x.NU_PRDC_INGRESO == id).Join(_context.T_PRDC_LINEA,
                prdi => prdi.CD_PRDC_LINEA,
                prdl => prdl.CD_PRDC_LINEA,
                (prdi, prdl) => prdl)
                .AsNoTracking()
                .FirstOrDefault();

            return entity == null ? null : _mapper.MapToObjet(entity);
        }
        public virtual List<EspacioProduccion> GetAllEspaciosProduccion()
        {
            return _context.T_PRDC_LINEA
                .AsNoTracking()
                .Select(entity => _mapper.MapToObjet(entity))
                .ToList();
        }

        public virtual List<EspacioProduccion> GetAllEspaciosProduccionByPredio(string predio)
        {
            return _context.T_PRDC_LINEA
                .AsNoTracking()
                .Where(entity => entity.NU_PREDIO == predio)
                .Select(entity => _mapper.MapToObjet(entity))
                .ToList();
        }

        public virtual List<string> GetUbicacionesDeProduccion(string codigoEspacio)
        {
            //Consultar pensada a futuro para cuando exista más de una ubicacion de produccion por espacio, de momento es unica.
            //Llegado el caso, la query debe ser modificada.

            return _context.T_PRDC_LINEA
                .Where(e => e.CD_PRDC_LINEA == codigoEspacio)
                .Select(e => e.CD_ENDERECO_PRODUCCION)
                .ToList();            
        }

        public virtual string GetNumeroEspacio()
        {
            return _context.GetNextSequenceValueDecimal(_dapper, "S_PRDC_LINEA").ToString();
        }

        public virtual int GetIngresosActivosEspacio(string idEspacio)
        {
            return _context.T_PRDC_INGRESO
                .Where(x => x.CD_PRDC_LINEA == idEspacio
                    && x.CD_SITUACAO != SituacionDb.PRODUCCION_FINALIZADA)
                .Count();
        }
        public virtual string GetFirstProduccionExternoOrdenAsociadaEspacio(string idEspacio)
        {
            var ingreso = _context.T_PRDC_INGRESO
                .FirstOrDefault(x => x.CD_PRDC_LINEA == idEspacio
                    && x.CD_SITUACAO != SituacionDb.PRODUCCION_FINALIZADA);

            return ingreso?.ID_PRODUCCION_EXTERNO;
        }
        #endregion

        #region Add

        public virtual void AddEspacioProduccion(EspacioProduccion espacioProduccion)
        {
            T_PRDC_LINEA entity = _mapper.MapObjectToEntity(espacioProduccion);

            _context.T_PRDC_LINEA.Add(entity);
        }

        #endregion

        #region Update

        public virtual void UpdateEspacio(EspacioProduccion espacio)
        {
            T_PRDC_LINEA entity = _mapper.MapObjectToEntity(espacio);

            entity.DT_UPDROW = DateTime.Now;

            T_PRDC_LINEA attachedEntity = _context.T_PRDC_LINEA.Local
                .FirstOrDefault(c => c.CD_PRDC_LINEA == espacio.Id);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_PRDC_LINEA.Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;
            }
        }

        #endregion

        #region Remove

        #endregion

        #region Dapper

        public virtual IEnumerable<EstacionDeTrabajo> GetEspaciosProduccion(IEnumerable<EstacionDeTrabajo> espacios)
        {
            IEnumerable<EstacionDeTrabajo> resultado = new List<EstacionDeTrabajo>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_PRDC_INGRESO_TEMP (CD_PRDC_LINEA) VALUES (:Id)";
                    _dapper.Execute(connection, sql, espacios, transaction: tran);

                    sql = @"SELECT 
								PL.CD_PRDC_LINEA as Id,
								PL.DS_PRDC_LINEA as Descripcion,
								PL.DT_ADDROW as FechaAlta,
								PL.DT_UPDROW as FechaModificacion,
								PL.CD_ENDERECO_ENTRADA as IdUbicacionEntrada,
								PL.CD_ENDERECO_SALIDA as IdUbicacionSalida,
								PL.NU_PRDC_INGRESO as NumeroIngreso,
								PL.CD_ENDERECO_PRODUCCION as IdUbicacionProduccion,
								PL.ND_TIPO_LINEA as Tipo,
								PL.NU_PREDIO as Predio,
								PL.FL_CONF_MAN as FlConfirmacionManual,
								PL.FL_STOCK_CONSUMIBLE as FlStockConsumible,
								PL.NU_TRANSACCION as NumeroTransaccion,
								PL.CD_ENDERECO_SALIDA_TRAN as IdUbicacionSalidaTran                            
                        FROM T_PRDC_LINEA PL 
                        INNER JOIN T_PRDC_INGRESO_TEMP T ON PL.CD_PRDC_LINEA = T.CD_PRDC_LINEA";

                    resultado = _dapper.Query<EstacionDeTrabajo>(connection, sql, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        #endregion
    }
}
