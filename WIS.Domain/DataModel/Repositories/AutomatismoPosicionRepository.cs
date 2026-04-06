using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.Automatismo;
using WIS.Domain.DataModel.Mappers.Automatismo;
using WIS.Domain.Extensions;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class AutomatismoPosicionRepository
    {
        protected readonly WISDB _context;
        protected readonly string _application;
        protected readonly int _userId;
        protected readonly IDapper _dapper;
        protected readonly AutomatismoPosicionMapper _mapper;

        public AutomatismoPosicionRepository(WISDB context, string application, int userId, IDapper dapper)
        {
            this._context = context;
            this._application = application;
            this._userId = userId;
            this._dapper = dapper;
            this._mapper = new AutomatismoPosicionMapper();
        }

        #region Any
        public virtual bool AnyPosicionAutomatismo(int numeroAutomatismo)
        {
            return _context.T_AUTOMATISMO_POSICION.Any(i => i.NU_AUTOMATISMO == numeroAutomatismo);
        }
        #endregion

        #region Get
        public virtual AutomatismoPosicion GetAutomatismoPosicionById(int id)
        {
            return this._mapper.Map(this._context.T_AUTOMATISMO_POSICION.FirstOrDefault(f => f.NU_AUTOMATISMO_POSICION == id));
        }
        public virtual List<AutomatismoPosicion> GetUbicacionesDisponibleByAgrupacion(int automatismo, int tipoAgrupacion, string comparteAgrupacion)
        {
            return _context.T_AUTOMATISMO_POSICION.AsNoTracking()
                .Where(w => w.NU_AUTOMATISMO == automatismo
                && w.TP_AGRUPACION_UBIC == tipoAgrupacion
                && (w.VL_COMPARTE_AGRUPACION == null || w.VL_COMPARTE_AGRUPACION == comparteAgrupacion))
                .Select(w => _mapper.Map(w))
                .ToList();
        }
        public virtual int GetNumeroAlturaAutomatismo(int automatismo, string tipoUbicacion)
        {
            var numeroAltura = (from pos in _context.T_AUTOMATISMO_POSICION
                                join ee in _context.T_ENDERECO_ESTOQUE on
                                    pos.CD_ENDERECO equals ee.CD_ENDERECO
                                where pos.NU_AUTOMATISMO == automatismo &&
                                      pos.ND_TIPO_ENDERECO == tipoUbicacion
                                select ee.NU_ALTURA).Max();

            return (numeroAltura + 1) ?? 1;
        }
        public virtual int GetNextNumeroAutomatismoPosicion()
        {
            return this._context.GetNextSequenceValueInt(_dapper, Secuencias.S_NU_AUTOMATISMO_INTERFAZ);
        }
        #endregion

        #region Add
        public virtual void Add(AutomatismoPosicion automatismoPosicion)
        {
            automatismoPosicion.Transaccion = _context.GetTransactionNumber();

            if (automatismoPosicion.Id == 0)
                automatismoPosicion.Id = this.GetNextNumeroAutomatismoPosicion();

            if (string.IsNullOrEmpty(automatismoPosicion.PosicionExterna))
                automatismoPosicion.PosicionExterna = automatismoPosicion.Id.ToString();

            automatismoPosicion.FechaRegistro = DateTime.Now;

            this._context.T_AUTOMATISMO_POSICION.Add(this._mapper.MapToEntity(automatismoPosicion));
        }
        #endregion

        #region Update
        public virtual void Update(AutomatismoPosicion posicion)
        {
            var entity = _mapper.MapToEntity(posicion);
            var attachedEntity = _context.T_AUTOMATISMO_POSICION.Local.FirstOrDefault(x => x.NU_AUTOMATISMO_POSICION == entity.NU_AUTOMATISMO_POSICION);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_AUTOMATISMO_POSICION.Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;
            }
        }

        public virtual void InsertOrUpdateOrDelete(List<AutomatismoPosicion> automatismoPosicions)
        {
            var affect = automatismoPosicions.Where(w => w.Transaccion != null).ToList();

            var updates = affect.Where(w => w.Transaccion == 1);
            var deletes = affect.Where(w => w.Transaccion < 0);
            var inserts = affect.Where(w => w.Transaccion == 0);


            foreach (var obj in updates) Update(obj);
            foreach (var obj in deletes) Remove(obj);
            foreach (var obj in inserts) Add(obj);


        }
        #endregion

        #region Remove
        public virtual void Remove(AutomatismoPosicion posicion)
        {
            posicion.Transaccion = _context.GetTransactionNumber();

            var entity = this._mapper.MapToEntity(posicion);
            var attachedEntity = _context.T_AUTOMATISMO_POSICION.Local.FirstOrDefault(w => w.NU_AUTOMATISMO_POSICION == entity.NU_AUTOMATISMO_POSICION);

            if (attachedEntity != null)
            {
                this._context.T_AUTOMATISMO_POSICION.Remove(attachedEntity);
            }
            else
            {
                this._context.T_AUTOMATISMO_POSICION.Attach(entity);
                this._context.T_AUTOMATISMO_POSICION.Remove(entity);
            }
        }
        public virtual void Remove(int id)
        {
            T_AUTOMATISMO_POSICION entity = _context.T_AUTOMATISMO_POSICION.FirstOrDefault(x => x.NU_AUTOMATISMO_POSICION == id);
            _context.T_AUTOMATISMO_POSICION.Remove(entity);
        }
        #endregion

        #region Dapper
        public virtual IEnumerable<string> GetUbicacionesEntrada()
        {
            IEnumerable<string> resultado = [];

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {

                    var sql = @"SELECT 
                                ap.CD_ENDERECO
                            FROM T_AUTOMATISMO_POSICION ap 
                            WHERE ap.ND_TIPO_ENDERECO = 'TPAUTPOSE'";

                    resultado = _dapper.Query<string>(connection, sql, transaction: tran);
                }
            }

            return resultado;
        }
        #endregion
    }
}
