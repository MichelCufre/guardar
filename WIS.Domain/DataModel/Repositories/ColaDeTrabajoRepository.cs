using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.Extensions;
using WIS.Domain.Picking;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class ColaDeTrabajoRepository
    {
        protected int userId;
        protected WISDB _context;
        protected string _cdAplicacion;
        protected int _userId;
        protected ColaDeTrabajoMapper _mapper;
        protected readonly IDapper _dapper;

        public ColaDeTrabajoRepository(WISDB _context, string application, int userId, IDapper dapper)
        {
            this._dapper = dapper;
            this._context = _context;
            this._cdAplicacion = application;
            this._userId = userId;
            this._mapper = new ColaDeTrabajoMapper();
        }

        #region Any

        public virtual bool AnyColaDeTrabajo(int nuColaDeTrabajo)
        {
            return this._context.T_COLA_TRABAJO.Any(x => x.NU_COLA_TRABAJO == nuColaDeTrabajo);
        }

        public virtual bool AnyPonderadorDetalle(int colaDeTrabajo, string ponderador, string instancia)
        {
            return this._context.T_COLA_TRABAJO_PONDERADOR_DET.Any(x => x.NU_COLA_TRABAJO == colaDeTrabajo
                                                                     && x.CD_PONDERADOR == ponderador
                                                                     && x.CD_INST_PONDERADOR == instancia);
        }

        public virtual bool IsFuncionarioAsignado(string producto, int empresa, string lote, decimal faixa, int preparacion, string predio, string cliente, string ubicacion, int secPreparacion)
        {
            var entity = this._context.V_ZOOMIN_PRE812.AsNoTracking().FirstOrDefault(x => x.CD_PRODUTO == producto &&
                                                                                                  x.CD_EMPRESA == empresa &&
                                                                                                  x.NU_IDENTIFICADOR == lote &&
                                                                                                  x.CD_FAIXA == faixa &&
                                                                                                  x.NU_PREPARACION == preparacion &&
                                                                                                  x.NU_PEDIDO == predio &&
                                                                                                  x.CD_CLIENTE == cliente &&
                                                                                                  x.CD_ENDERECO == ubicacion &&
                                                                                                  x.NU_SEQ_PREPARACION == secPreparacion);

            if (entity != null && entity.CD_FUNC_ASIGNADO == null)
            {
                return false;
            }
            return true;
        }

        #endregion

        #region Get

        public virtual List<ColaDeTrabajo> GetColasDeTrabajo()
        {
            List<ColaDeTrabajo> col = new List<ColaDeTrabajo>();

            foreach (var item in this._context.T_COLA_TRABAJO.AsNoTracking().Select(s => s).ToList())
            {
                col.Add(this._mapper.MapToObject(item));
            }

            return col;
        }

        public virtual List<ColaDeTrabajo> GetColasDeTrabajo(string predio)
        {
            List<ColaDeTrabajo> col = new List<ColaDeTrabajo>();

            foreach (var item in this._context.T_COLA_TRABAJO.AsNoTracking().Where(w => w.NU_PREDIO == predio).ToList())
            {
                col.Add(this._mapper.MapToObject(item));
            }

            return col;
        }

        public virtual ColaDeTrabajoPonderadorDetalle GetPonderadorDetalle(int colaDeTrabajo, string ponderador, string instancia)
        {
            return this._mapper.MapToObject(this._context.T_COLA_TRABAJO_PONDERADOR_DET.AsNoTracking().FirstOrDefault(x => x.NU_COLA_TRABAJO == colaDeTrabajo
                                                                                               && x.CD_PONDERADOR == ponderador
                                                                                               && x.CD_INST_PONDERADOR == instancia));
        }

        public virtual List<ColaDeTrabajoPonderador> GetPonderadoresByNumero(int nroColaDeTrabajo)
        {
            List<ColaDeTrabajoPonderador> col = new List<ColaDeTrabajoPonderador>();
            foreach (var item in this._context.T_COLA_TRABAJO_PONDERADOR.AsNoTracking().Where(x => x.NU_COLA_TRABAJO == nroColaDeTrabajo).ToList())
            {
                col.Add(this._mapper.MapToObject(item));
            }

            return col;
        }

        public virtual ColaDeTrabajoPonderador GetPonderador(int nuColaDeTrabajo, string ponderador)
        {
            return this._mapper.MapToObject(this._context.T_COLA_TRABAJO_PONDERADOR.AsNoTracking().FirstOrDefault(x => x.NU_COLA_TRABAJO == nuColaDeTrabajo && x.CD_PONDERADOR == ponderador));
        }

        public virtual PonderadorInstancia GetPonderadorDefault(string ponderador)
        {
            return this._mapper.MapToObject(this._context.T_COLA_TRABAJO_PONDERADOR_INST.AsNoTracking().FirstOrDefault(x => x.CD_PONDERADOR == ponderador));
        }

        public virtual ColaDeTrabajo GetColaDeTrabajo(int nuColaTrabajo)
        {
            return this._mapper.MapToObject(this._context.T_COLA_TRABAJO.AsNoTracking().FirstOrDefault(w => w.NU_COLA_TRABAJO == nuColaTrabajo));

        }

        #endregion

        #region Add

        public virtual void AddPonderadorDetalle(ColaDeTrabajoPonderadorDetalle det)
        {
            T_COLA_TRABAJO_PONDERADOR_DET entity = this._mapper.MapToEntity(det);
            entity.DT_ADDROW = DateTime.Now;
            this._context.T_COLA_TRABAJO_PONDERADOR_DET.Add(entity);
        }

        public virtual void AddColaDeTrabajo(ColaDeTrabajo obj)
        {
            T_COLA_TRABAJO entity = this._mapper.MapToEntity(obj);
            entity.DT_ADDROW = DateTime.Now;
            this._context.T_COLA_TRABAJO.Add(entity);
        }

        public virtual void AddPonderador(ColaDeTrabajoPonderador obj)
        {
            T_COLA_TRABAJO_PONDERADOR entity = this._mapper.MapToEntity(obj);
            entity.DT_ADDROW = DateTime.Now;
            this._context.T_COLA_TRABAJO_PONDERADOR.Add(entity);
        }

        #endregion

        #region Update

        public virtual void UpdatePonderadorDetalle(ColaDeTrabajoPonderadorDetalle obj)
        {

            T_COLA_TRABAJO_PONDERADOR_DET entity = this._mapper.MapToEntity(obj);

            this._context.T_COLA_TRABAJO_PONDERADOR_DET.Attach(entity);
            entity.DT_UPDROW = DateTime.Now;
            this._context.Entry(entity).State = EntityState.Modified;

        }

        public virtual void UpdatePonderador(ColaDeTrabajoPonderador obj)
        {
            T_COLA_TRABAJO_PONDERADOR entity = this._mapper.MapToEntity(obj);

            this._context.T_COLA_TRABAJO_PONDERADOR.Attach(entity);
            entity.DT_UPDROW = DateTime.Now;
            this._context.Entry(entity).State = EntityState.Modified;
        }

        public virtual void DesasignarFuncionarios(string producto, int empresa, string lote, decimal faixa, int preparacion, string predio, string cliente, string ubicacion, int secPreparacion)
        {
            var entity = this._context.T_DET_PICKING.AsNoTracking().FirstOrDefault(x => x.CD_PRODUTO == producto &&
                                                                                                  x.CD_EMPRESA == empresa &&
                                                                                                  x.NU_IDENTIFICADOR == lote &&
                                                                                                  x.CD_FAIXA == faixa &&
                                                                                                  x.NU_PREPARACION == preparacion &&
                                                                                                  x.NU_PEDIDO == predio &&
                                                                                                  x.CD_CLIENTE == cliente &&
                                                                                                  x.CD_ENDERECO == ubicacion &&
                                                                                                  x.NU_SEQ_PREPARACION == secPreparacion);

            if (entity != null && entity.NU_COLA_PICKING != null)
            {
                this._context.T_DET_PICKING.Attach(entity);
                entity.NU_COLA_PICKING = null;
                entity.DT_UPDROW = DateTime.Now;
                entity.NU_TRANSACCION = this._context.GetTransactionNumber();
                this._context.Entry(entity).State = EntityState.Modified;
            }
        }

        public virtual void AsignarFuncionarios(string producto, int empresa, string lote, decimal faixa, int preparacion, string predio, string cliente, string ubicacion, int secPreparacion, int userId)
        {
            var nuColaPicking = this._context.GetNextSequenceValueInt(_dapper, "S_COLA_PICKING");
            var colaPicking = new T_COLA_PICKING
            {
                NU_COLA_PICKING = nuColaPicking,
                DT_ADDROW = DateTime.Now,
                USERID = userId
            };

            this._context.T_COLA_PICKING.Add(colaPicking);

            var entity = this._context.T_DET_PICKING.AsNoTracking().FirstOrDefault(x => x.CD_PRODUTO == producto &&
                                                                                                  x.CD_EMPRESA == empresa &&
                                                                                                  x.NU_IDENTIFICADOR == lote &&
                                                                                                  x.CD_FAIXA == faixa &&
                                                                                                  x.NU_PREPARACION == preparacion &&
                                                                                                  x.NU_PEDIDO == predio &&
                                                                                                  x.CD_CLIENTE == cliente &&
                                                                                                  x.CD_ENDERECO == ubicacion &&
                                                                                                  x.NU_SEQ_PREPARACION == secPreparacion);

            if (entity != null && entity.NU_COLA_PICKING == null)
            {
                this._context.T_DET_PICKING.Attach(entity);
                entity.NU_COLA_PICKING = colaPicking.NU_COLA_PICKING;
                entity.DT_UPDROW = DateTime.Now;
                entity.NU_TRANSACCION = this._context.GetTransactionNumber();
                this._context.Entry(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateColaDeTrabajo(ColaDeTrabajo obj)
        {
            T_COLA_TRABAJO entity = this._mapper.MapToEntity(obj);

            this._context.T_COLA_TRABAJO.Attach(entity);
            entity.DT_UPDROW = DateTime.Now;
            this._context.Entry(entity).State = EntityState.Modified;
        }

        #endregion

        #region Remove

        public virtual void RemovePonderadorDetalle(ColaDeTrabajoPonderadorDetalle det)
        {
            {
                var entity = this._mapper.MapToEntity(det);

                this._context.T_COLA_TRABAJO_PONDERADOR_DET.Attach(entity);

                this._context.T_COLA_TRABAJO_PONDERADOR_DET.Remove(entity);
            }
        }

        #endregion

        #region Dapper

        #endregion
    }
}
