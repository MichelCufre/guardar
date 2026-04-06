using Dapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WIS.Components.Common.Select;
using WIS.Domain.Automatismo;
using WIS.Domain.DataModel.Mappers.Automatismo;
using WIS.Domain.Extensions;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class AutomatismoCaracteristicaRepository
    {
        protected readonly WISDB _context;
        protected readonly string _application;
        protected readonly int _userId;
        protected readonly IDapper _dapper;
        protected readonly AutomatismoCaracteristicaMapper _mapper;
        protected readonly AutomatismoCaracteristicaConfiguracionMapper _configuracionMapper;

        public AutomatismoCaracteristicaRepository(WISDB context, string application, int userId, IDapper dapper)
        {
            this._context = context;
            this._application = application;
            this._userId = userId;
            this._dapper = dapper;
            this._mapper = new AutomatismoCaracteristicaMapper();
            this._configuracionMapper = new AutomatismoCaracteristicaConfiguracionMapper();
        }

        #region Any

        public virtual bool AutomatismoHasAnyCaracteristica(int nuAutomatismo)
        {
            return _context.T_AUTOMATISMO_CARACTERISTICA.Any(i => i.NU_AUTOMATISMO == nuAutomatismo);
        }

        #endregion

        #region Get

        public virtual AutomatismoCaracteristica GetAutomatismoCaracteristicaById(long id)
        {
            return _mapper.Map(_context.T_AUTOMATISMO_CARACTERISTICA.AsNoTracking().FirstOrDefault(i => i.NU_AUTOMATISMO_CARACTERISTICA == id));
        }

        public virtual AutomatismoCaracteristica GetAutomatismoCaracteristicaByPuestoAndCodigo(string puesto, string caracteristica)
        {
            int? nuAutomatismo = _context.T_AUTOMATISMO_PUESTO.AsNoTracking().FirstOrDefault(w => w.ID_PUESTO == puesto)?.NU_AUTOMATISMO;

            if (nuAutomatismo != null)
            {
                return _mapper.Map(_context.T_AUTOMATISMO_CARACTERISTICA.AsNoTracking().FirstOrDefault(w => w.NU_AUTOMATISMO == nuAutomatismo && w.CD_AUTOMATISMO_CARACTERISTICA == caracteristica));
            }

            return null;
        }

        public virtual List<AutomatismoCaracteristica> GetCaracteristicas()
        {
            return _mapper.Map(_context.T_AUTOMATISMO_CARACTERISTICA.AsNoTracking().ToList());
        }

        public virtual List<AutomatismoCaracteristica> GetCaracteristicasByNumeroAutomatismo(int nuAutomatismo)
        {
            return _mapper.Map(_context.T_AUTOMATISMO_CARACTERISTICA.Where(i => i.NU_AUTOMATISMO == nuAutomatismo).ToList());
        }

        public virtual List<AutomatismoCaracteristicaConfiguracion> GetCaracteristicasPorDefectoPorTipoAutomatismo(string tipo)
        {
            return _configuracionMapper.Map(_context.T_AUTOMATISMO_CARACTERISTICA_CONFIG.AsNoTracking()?.Where(i => i.TP_AUTOMATISMO == tipo)?.ToList());
        }

        public virtual List<AutomatismoCaracteristicaConfiguracion> GetCaracteristicasPorDefecto()
        {
            return _configuracionMapper.Map(_context.T_AUTOMATISMO_CARACTERISTICA_CONFIG.AsNoTracking().ToList());
        }

        public virtual string GetOpcionesDinamicas(string tipo, string codigoCaracteristica)
        {
            return this._context.T_AUTOMATISMO_CARACTERISTICA_CONFIG.FirstOrDefault(i => i.TP_AUTOMATISMO == tipo && i.CD_AUTOMATISMO_CARACTERISTICA == codigoCaracteristica)?.VL_OPCIONES;
        }

        public virtual int GetNextNumeroCaracteristica()
        {
            return this._context.GetNextSequenceValueInt(_dapper, Secuencias.S_NU_AUTOMATISMO_CARACTERISTICA);
        }

        #endregion

        #region Add

        public virtual void Add(AutomatismoCaracteristica obj)
        {
            obj.Id = GetNextNumeroCaracteristica();
            obj.FechaAlta = DateTime.Now;
            obj.Transaccion = _context.GetTransactionNumber();

            _context.T_AUTOMATISMO_CARACTERISTICA.Add(_mapper.MapToEntity(obj));
        }

        public virtual void InsertOrUpdateOrDelete(List<AutomatismoCaracteristica> automatismoPosicions)
        {
            var affect = automatismoPosicions.Where(w => w.Transaccion != null).ToList();

            var updates = affect.Where(w => w.Transaccion == 1).ToList();
            var deletes = affect.Where(w => w.Transaccion < 0).ToList();
            var inserts = affect.Where(w => w.Transaccion == 0).ToList();


            foreach (var obj in updates) Update(obj);
            foreach (var obj in deletes) Remove(obj);
            foreach (var obj in inserts) Add(obj);


        }

        #endregion

        #region Update

        public virtual void Update(AutomatismoCaracteristica obj)
        {
            obj.Transaccion = _context.GetTransactionNumber();

            var entity = _mapper.MapToEntity(obj);
            var attachedEntity = _context.T_AUTOMATISMO_CARACTERISTICA.Local.FirstOrDefault(x => x.NU_AUTOMATISMO_CARACTERISTICA == entity.NU_AUTOMATISMO_CARACTERISTICA);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_AUTOMATISMO_CARACTERISTICA.Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;
            }
        }

        #endregion

        #region Remove

        public virtual void Remove(AutomatismoCaracteristica caracteristica)
        {
            caracteristica.Transaccion = _context.GetTransactionNumber();

            var entity = this._mapper.MapToEntity(caracteristica);
            var attachedEntity = _context.T_AUTOMATISMO_CARACTERISTICA.Local.FirstOrDefault(w => w.NU_AUTOMATISMO_CARACTERISTICA == entity.NU_AUTOMATISMO_CARACTERISTICA);

            if (attachedEntity != null)
            {
                this._context.T_AUTOMATISMO_CARACTERISTICA.Remove(attachedEntity);
            }
            else
            {
                this._context.T_AUTOMATISMO_CARACTERISTICA.Attach(entity);
                this._context.T_AUTOMATISMO_CARACTERISTICA.Remove(entity);
            }
        }

        #endregion

        #region Dapper

        public virtual List<SelectOption> GetOpcionesDeCaracteristica(string sql, DynamicParameters parameters)
        {
            return _dapper.GetAll<SelectOption>(sql, parameters, CommandType.Text);
        }

        #endregion
    }
}
