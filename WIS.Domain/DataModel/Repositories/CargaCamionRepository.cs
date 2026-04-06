using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.Expedicion;
using WIS.Domain.Picking;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class CargaCamionRepository
    {
        protected WISDB _context;
        protected string application;
        protected int userId;
        protected CargaCamionMapper _mapper;
        protected readonly IDapper _dapper;

        public CargaCamionRepository(WISDB context, string application, int userId, IDapper dapper)
        {
            this._context = context;
            this.application = application;
            this.userId = userId;
            this._mapper = new CargaCamionMapper();
            this._dapper = dapper;
        }

        #region Any
        public virtual bool ExisteCargaCamion(long? NuCarga, string CdCliente, int CdCamion)
        {
            return _context.T_CLIENTE_CAMION.Any(x => x.NU_CARGA == NuCarga && x.CD_CLIENTE == CdCliente && x.CD_CAMION == CdCamion);
        }

        #endregion

        #region Get
        public virtual List<CargaCamion> GetsCargasCamion(Camion camion)
        {
            List<CargaCamion> list = new List<CargaCamion>();

            List<T_CLIENTE_CAMION> cargas = _context.T_CLIENTE_CAMION.AsNoTracking().Where(cc => cc.CD_CAMION == camion.Id && cc.CD_EMPRESA == (camion.Empresa ?? cc.CD_EMPRESA)).ToList();
            foreach (var a in cargas)
            {
                CargaCamion car = _mapper.MapToObject(a);
                list.Add(car);
            }

            return list;
        }
        public virtual List<CargaCamion> GetsCargasCamion(int CdCamion)
        {
            List<CargaCamion> list = new List<CargaCamion>();

            List<T_CLIENTE_CAMION> cargas = _context.T_CLIENTE_CAMION.AsNoTracking().Where(cc => cc.CD_CAMION == CdCamion).ToList();
            foreach (var a in cargas)
            {
                CargaCamion car = _mapper.MapToObject(a);
                list.Add(car);
            }

            return list;
        }
        public virtual CargaCamion GetCamionCarga(long numeroCarga, int codigoEmpresa, string codigoCliente)
        {
            T_CLIENTE_CAMION entity = _context.T_CLIENTE_CAMION.AsNoTracking().FirstOrDefault(cc => cc.NU_CARGA == numeroCarga && cc.CD_EMPRESA == codigoEmpresa && cc.CD_CLIENTE == codigoCliente);
            return entity == null ? null : _mapper.MapToObject(entity);
        }

        public virtual CargaCamion GetFirstCargaCamion(long nuCarga)
        {

            return _mapper.MapToObject(_context.T_CLIENTE_CAMION.FirstOrDefault(c => c.NU_CARGA == nuCarga));
        }
        #endregion

        #region Add
        public virtual void AddCargaCamion(CargaCamion obj)
        {
            T_CLIENTE_CAMION entity = this._mapper.MapToEntity(obj);

            entity.ID_CARGAR = "S";
            entity.T_CAMION = this._context.T_CAMION.Local.FirstOrDefault(w => w.CD_CAMION == entity.CD_CAMION) ?? this._context.T_CAMION.FirstOrDefault(x => x.CD_CAMION == entity.CD_CAMION);

            this._context.T_CLIENTE_CAMION.Add(entity);
        }

        #endregion

        #region Update

        public virtual void UpdateCargaCamion(CargaCamion cargaCamion)
        {
            T_CLIENTE_CAMION entity = this._mapper.MapToEntity(cargaCamion);
            T_CLIENTE_CAMION attachedEntity = _context.T_CLIENTE_CAMION.Local.FirstOrDefault(x => x.CD_CAMION == entity.CD_CAMION && x.NU_CARGA == entity.NU_CARGA && x.CD_CLIENTE == entity.CD_CLIENTE);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.State = EntityState.Modified;
                attachedEntry.CurrentValues.SetValues(entity);
            }
            else
            {
                _context.T_CLIENTE_CAMION.Attach(entity);
                _context.Entry<T_CLIENTE_CAMION>(entity).State = EntityState.Modified;
            }
        }

        public virtual void MarcarCargasSincronizadas(int camion)
        {
            _context.T_CLIENTE_CAMION
                .Where(d => d.CD_CAMION == camion && d.FL_SYNC_REALIZADA != "S")
                .ExecuteUpdate(setters => setters
                    .SetProperty(d => d.FL_SYNC_REALIZADA, "S")
                    .SetProperty(d => d.DT_UPDROW, DateTime.Now));
        }
        #endregion

        #region Remove
        public virtual void DeleteCargaCamiones(CargaCamion clienteCamion)
        {
            T_CLIENTE_CAMION entity = this._mapper.MapToEntity(clienteCamion);
            T_CLIENTE_CAMION attachedEntity = _context.T_CLIENTE_CAMION.Local
                .FirstOrDefault(x => x.CD_CAMION == entity.CD_CAMION
                    && x.NU_CARGA == entity.NU_CARGA
                    && x.CD_CLIENTE == entity.CD_CLIENTE);

            if (attachedEntity != null)
            {
                _context.T_CLIENTE_CAMION.Remove(attachedEntity);
            }
            else
            {
                _context.T_CLIENTE_CAMION.Attach(entity);
                _context.T_CLIENTE_CAMION.Remove(entity);
            }
        }

        #endregion

        #region Dapper
        public virtual IEnumerable<CargaCamion> GetCargaCamion(IEnumerable<Carga> cargas)
        {
            IEnumerable<CargaCamion> resultado = new List<CargaCamion>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_DET_PICKING_TEMP (NU_CARGA) VALUES (:Id)";
                    _dapper.Execute(connection, sql, cargas, transaction: tran);

                    sql = @"SELECT 
                                CC.CD_CAMION as Camion,
                                CC.NU_CARGA as Carga,
                                CC.CD_CLIENTE as Cliente,
                                CC.CD_EMPRESA as Empresa,
                                CC.ID_CARGAR as IdCargar,
                                CC.DT_UPDROW as FechaModificacion,
                                CC.DT_ADDROW  as FechaAlta
                            FROM T_CLIENTE_CAMION CC
                            INNER JOIN T_DET_PICKING_TEMP T ON CC.NU_CARGA = T.NU_CARGA ";

                    resultado = _dapper.Query<CargaCamion>(connection, sql, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }


        public virtual IEnumerable<CargaCamion> GetCargasCamion(IEnumerable<CargaCamion> cargas)
        {
            IEnumerable<CargaCamion> resultado = new List<CargaCamion>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_CLIENTE_CAMION_TEMP (NU_CARGA, CD_CLIENTE, CD_EMPRESA) VALUES (:Carga, :Cliente, :Empresa)";
                    _dapper.Execute(connection, sql, cargas, transaction: tran);

                    sql = @" SELECT 
                            CC.CD_CAMION as Camion,
                            CC.NU_CARGA as Carga,
                            CC.CD_CLIENTE as Cliente,
                            CC.CD_EMPRESA as Empresa,
                            CC.ID_CARGAR as IdCargar,
                            CC.DT_ADDROW as FechaAlta, 
                            CC.DT_UPDROW as FechaModificacion,
                            CC.TP_MODALIDAD as TipoModalidad,
                            CC.FL_SYNC_REALIZADA as SincronizacionRealizadaId
                        FROM T_CLIENTE_CAMION CC 
                        INNER JOIN T_CLIENTE_CAMION_TEMP T ON CC.NU_CARGA = T.NU_CARGA AND CC.CD_CLIENTE = T.CD_CLIENTE AND CC.CD_EMPRESA = T.CD_EMPRESA 
                        GROUP BY 
                            CC.CD_CAMION,
                            CC.NU_CARGA, 
                            CC.CD_CLIENTE,
                            CC.CD_EMPRESA,
                            CC.ID_CARGAR, 
                            CC.DT_ADDROW,
                            CC.DT_UPDROW,
                            CC.TP_MODALIDAD,
                            CC.FL_SYNC_REALIZADA";

                    resultado = _dapper.Query<CargaCamion>(connection, sql, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }


        public virtual IEnumerable<Carga> GetCargasCompartidasPedidos(IEnumerable<CargaCamion> cargas)
        {
            IEnumerable<Carga> resultado = new List<Carga>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_CARGA_TEMP (NU_CARGA, CD_CLIENTE, CD_EMPRESA) VALUES (:Carga, :Cliente, :Empresa)";
                    _dapper.Execute(connection, sql, cargas, transaction: tran);

                    sql = @"SELECT 
                                ca.NU_CARGA as Id,    
                                ca.DS_CARGA as Descripcion,
                                ca.CD_ROTA as Ruta,    
                                ca.NU_PREPARACION as Preparacion,    
                                ca.DT_ADDROW as FechaAlta
                            FROM T_CARGA ca
                            INNER JOIN (    
                                SELECT 
                                    dp.NU_CARGA, 
                                    dp.CD_EMPRESA, 
                                    dp.CD_CLIENTE,
                                    COUNT(DISTINCT(dp.NU_PEDIDO)) AS Pedidos
                                FROM T_CARGA_TEMP TEMP
                                INNER JOIN T_DET_PICKING dp ON TEMP.NU_CARGA = dp.NU_CARGA AND TEMP.CD_EMPRESA = dp.CD_EMPRESA AND TEMP.CD_CLIENTE = dp.CD_CLIENTE    
                                GROUP BY 
                                    dp.NU_CARGA,
                                    dp.CD_EMPRESA,
                                    dp.CD_CLIENTE
                                HAVING COUNT(DISTINCT(dp.NU_PEDIDO)) > 1
                            ) dp ON ca.NU_CARGA = dp.NU_CARGA";

                    resultado = _dapper.Query<Carga>(connection, sql, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual IEnumerable<Carga> GetCargasCompartidasContenedores(IEnumerable<CargaCamion> cargas)
        {
            IEnumerable<Carga> resultado = new List<Carga>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_CARGA_TEMP (NU_CARGA, CD_CLIENTE, CD_EMPRESA) VALUES (:Carga, :Cliente, :Empresa)";
                    _dapper.Execute(connection, sql, cargas, transaction: tran);

                    sql = @"SELECT 
                                ca.NU_CARGA as Id,    
                                ca.DS_CARGA as Descripcion,
                                ca.CD_ROTA as Ruta,    
                                ca.NU_PREPARACION as Preparacion,    
                                ca.DT_ADDROW as FechaAlta
                            FROM T_CARGA ca
                            INNER JOIN (    
                                SELECT 
                                    dp.NU_CARGA, 
                                    dp.CD_EMPRESA, 
                                    dp.CD_CLIENTE,
                                    COUNT(DISTINCT(dp.NU_CONTENEDOR)) AS Contenedores
                                FROM T_CARGA_TEMP TEMP
                                INNER JOIN T_DET_PICKING dp ON TEMP.NU_CARGA = dp.NU_CARGA AND TEMP.CD_EMPRESA = dp.CD_EMPRESA AND TEMP.CD_CLIENTE = dp.CD_CLIENTE
                                WHERE dp.NU_CONTENEDOR IS NOT NULL
                                GROUP BY 
                                    dp.NU_CARGA,
                                    dp.CD_EMPRESA,
                                    dp.CD_CLIENTE
                                HAVING COUNT(DISTINCT(dp.NU_CONTENEDOR)) > 1
                            ) dp ON ca.NU_CARGA = dp.NU_CARGA";                   
                    
                    resultado = _dapper.Query<Carga>(connection, sql, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }
        #endregion
    }
}
