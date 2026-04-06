using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Expedicion;
using WIS.Domain.Extensions;
using WIS.Domain.General;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.StockEntities;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class ContenedorRepository
    {
        protected WISDB _context;
        protected string application;
        protected int userId;
        protected readonly ContenedorMapper _mapper;
        protected readonly UnidadTransporteMapper _mapperUnidad;
        protected readonly CamionMapper _mapperCamion;
        protected readonly PreparacionMapper _mapperPreparacion;
        protected readonly IDapper _dapper;

        public ContenedorRepository(WISDB context, string application, int userId, IDapper dapper)
        {
            this._context = context;
            this.application = application;
            this.userId = userId;
            this._dapper = dapper;
            _mapper = new ContenedorMapper();
            _mapperCamion = new CamionMapper();
            _mapperPreparacion = new PreparacionMapper();
            _mapperUnidad = new UnidadTransporteMapper();
        }

        #region Any

        public virtual bool ExisteTipoContenedor(string tipo)
        {
            return this._context.T_TIPO_CONTENEDOR
                .AsNoTracking()
                .Any(f => f.TP_CONTENEDOR == tipo);
        }

        public virtual bool TipoContenedorEnvase(string tipo)
        {
            return _context.T_TIPO_CONTENEDOR
                .AsNoTracking()
                .Any(w => w.TP_CONTENEDOR == tipo && w.FL_RETORNABLE == "S");
        }

        public virtual bool TipoContenedorPredefinido(string tipo)
        {
            return _context.T_TIPO_CONTENEDOR
                .AsNoTracking()
                .Any(x => x.TP_CONTENEDOR == tipo && x.ID_CLIENTE_PREDEFINIDO == "S");
        }

        public virtual bool TipoContenedorEnUso(string tipo)
        {
            return _context.T_CONTENEDOR
                .AsNoTracking()
                .Any(c => c.TP_CONTENEDOR == tipo);
        }

        public virtual bool ExisteContenedorEnPreparacion(int numeroContenedor, int numeroPreparacion)
        {
            return _context.T_CONTENEDOR
                .Any(x => x.NU_PREPARACION == numeroPreparacion
                    && x.NU_CONTENEDOR == numeroContenedor
                    && x.CD_SITUACAO == SituacionDb.ContenedorEnPreparacion);
        }

        public virtual bool IsContenedorEmpaque(int? nuContenedor, int? nuPreparacion)
        {
            if (nuContenedor != null && nuPreparacion != null)
                return _context.T_CONTENEDOR
                    .Any(x => x.NU_CONTENEDOR == nuContenedor
                        && x.NU_PREPARACION == nuPreparacion
                        && x.ID_CONTENEDOR_EMPAQUE == "S");

            return false;
        }

        public virtual bool ExisteContenedorActivoByCodigoBarras(string barras)
        {
            return _context.T_CONTENEDOR
                .Any(x => x.CD_BARRAS == barras
                    && x.CD_SITUACAO != SituacionDb.ContenedorEnviado);
        }

        public virtual bool IsFacturado(int nuContenedor, int nuPreparacion)
        {
            return _context.T_CONTENEDOR
                .Any(x => x.NU_CONTENEDOR == nuContenedor
                    && x.NU_PREPARACION == nuPreparacion
                    && x.CD_CAMION_FACTURADO != null);
        }

        public virtual bool ContenedorTieneUnPedido(int nuContenedor, int nuPreparacion)
        {
            var lista = _context.T_DET_PICKING
                .AsNoTracking()
                .Where(w => w.ND_ESTADO == EstadoDetallePreparacion.ESTADO_PREPARADO
                    && w.NU_CONTENEDOR == nuContenedor
                    && w.NU_PREPARACION == nuPreparacion)
                .ToList()
                .Select(w => $"{w.NU_PEDIDO}${w.CD_CLIENTE}${w.CD_EMPRESA}")
                .ToList()
                .Distinct();

            if (lista.Count() > 1)
                return false;

            return true;
        }

        public virtual bool HayCargasConContenedoresCompartidos(List<long> cargasContenedor)
        {
            foreach (long carga in cargasContenedor)
            {
                if (this.VariosContenedorCarga(carga))
                {
                    return true;
                }
            }
            return false;
        }

        public virtual bool VariosContenedorCarga(long carga)
        {
            return (_context.T_DET_PICKING
                .Include("T_CONTENEDOR")
                .AsNoTracking()
                .Where(dp => dp.NU_CARGA == carga
                     && dp.T_CONTENEDOR.CD_CAMION_FACTURADO == null
                     && dp.T_CONTENEDOR != null
                     && dp.ND_ESTADO == EstadoDetallePreparacion.ESTADO_PREPARADO //Se excluyen contenedores que ya esten facturados y no preparados
                     && dp.T_CONTENEDOR.CD_SITUACAO == SituacionDb.ContenedorEnPreparacion) //Agregado Mauro 2017-12-29 para mejorar performance
                 .GroupBy(grp => grp.T_CONTENEDOR.NU_CONTENEDOR)
                 .Select(s => s.Key)
                 .Count() > 1);
        }

        public virtual bool ExisteContenedor(int nuContenedor)
        {
            return _context.T_CONTENEDOR
                .Any(x => x.NU_CONTENEDOR == nuContenedor);
        }

        public virtual bool AnyContenedorEnUt(int? ut)
        {
            return _context.T_CONTENEDOR.Any(x => x.NU_UNIDAD_TRANSPORTE == ut);
        }

        public virtual bool AnyContenedorCargadoEnCamion(int cdCamion)
        {
            return _context.T_CONTENEDOR.Any(x => x.CD_CAMION == cdCamion);
        }

        #endregion

        #region Get

        public virtual int GetNextNuContenedor()
        {
            return this._context.GetNextSequenceValueInt(_dapper, Secuencias.S_NU_CONTENEDOR);
        }

        public virtual List<TipoContenedor> GetTiposContenedores()
        {
            return _context.T_TIPO_CONTENEDOR
                .AsNoTracking()
                .Select(t => _mapper.MapToObject(t))
                .ToList();
        }

        public virtual int GetUltimaSecuenciaTipoContenedor(string tipo)
        {
            var entity = this._context.T_TIPO_CONTENEDOR.FirstOrDefault(x => x.TP_CONTENEDOR == tipo);

            if (entity == null) return 0;

            return _context.GetNextSequenceValueInt(_dapper, entity.NM_SECUENCIA);
        }

        public virtual TipoContenedor GetTipoContenedor(string tipo)
        {
            return _mapper.MapToObject(_context.T_TIPO_CONTENEDOR
                .AsNoTracking()
                .FirstOrDefault(c => c.TP_CONTENEDOR == tipo));
        }

        public virtual string GetTipoContenedorByNroContenedor(int nuContenedor)
        {
            return _context.T_CONTENEDOR
                .AsNoTracking()
                .FirstOrDefault(c => c.NU_CONTENEDOR == nuContenedor)?.TP_CONTENEDOR;
        }

        public virtual Contenedor GetContenedorByCodigoBarras(string barras)
        {
            var entity = _context.T_CONTENEDOR
                .AsNoTracking()
                .FirstOrDefault(c => c.CD_BARRAS == barras
                    && c.CD_SITUACAO != SituacionDb.ContenedorEnviado
                    && c.CD_SITUACAO != SituacionDb.ContenedorEnsambladoKit
                    && c.CD_SITUACAO != SituacionDb.ContenedorTransferido);

            return _mapper.MapToObject(entity);
        }

        public virtual Contenedor GetContenedorByCodigoBarrasLpn(string codigoBarras, int userId, int? empresa, out int cantidadEtiqueta, out bool isBarraLpn)
        {
            Contenedor contenedor = null;
            List<T_LPN> listLpnBarras = null;

            isBarraLpn = _context.T_LPN_BARRAS
                 .Any(x => x.CD_BARRAS == codigoBarras);

            var query = _context.T_LPN_BARRAS
                 .Join(_context.T_LPN,
                    cb => cb.NU_LPN,
                    l => l.NU_LPN,
                    (cb, l) => new { Barras = cb, Lpn = l })
                .Join(_context.T_EMPRESA_FUNCIONARIO,
                    c => new { c.Lpn.CD_EMPRESA },
                    ef => new { ef.CD_EMPRESA },
                    (c, ef) => new { Codigo = c, EmpresaFuncionario = ef })
                .Where(x => x.EmpresaFuncionario.USERID == userId
                    && x.Codigo.Barras.CD_BARRAS == codigoBarras
                    && (empresa != null ? x.Codigo.Lpn.CD_EMPRESA == empresa : true))
                .Select(x => x.Codigo.Lpn)
                .Join(_context.T_CONTENEDOR.Include("T_PICKING"),
                    l => new { ID_EXTERNO_CONTENEDOR = l.ID_LPN_EXTERNO, TP_CONTENEDOR = l.TP_LPN_TIPO, CD_EMPRESA = (int?)l.CD_EMPRESA },
                    c => new { c.ID_EXTERNO_CONTENEDOR, c.TP_CONTENEDOR, c.T_PICKING.CD_EMPRESA },
                    (l, c) => new { Lpn = l, Contenerdor = c })
                .Where(x => x.Contenerdor.CD_SITUACAO != SituacionDb.ContenedorEnviado
                    && x.Contenerdor.CD_SITUACAO != SituacionDb.ContenedorEnsambladoKit
                    && x.Contenerdor.CD_SITUACAO != SituacionDb.ContenedorTransferido);

            listLpnBarras = query.Select(x => x.Lpn).ToList();

            contenedor = _mapper.MapToObject(query.Select(x => x.Contenerdor).FirstOrDefault());

            cantidadEtiqueta = listLpnBarras
                .GroupBy(b => new { b.CD_EMPRESA })
                .Count();

            return contenedor;
        }

        public virtual List<Empresa> GetEmpresasByCodigoBarrasContenedorLpn(string codigoBarras, int userId, int? cdEmpresa, bool isCodigoMultidato)
        {
            var query = _context.T_LPN_BARRAS
                 .Join(_context.T_LPN,
                    cb => cb.NU_LPN,
                    l => l.NU_LPN,
                    (cb, l) => new { Barras = cb, Lpn = l })
                .Join(_context.T_EMPRESA_FUNCIONARIO,
                    c => new { c.Lpn.CD_EMPRESA },
                    ef => new { ef.CD_EMPRESA },
                    (c, ef) => new { Codigo = c, EmpresaFuncionario = ef })
                .Where(x => x.EmpresaFuncionario.USERID == userId
                    && x.Codigo.Barras.CD_BARRAS == codigoBarras
                    && (cdEmpresa == null || x.Codigo.Lpn.CD_EMPRESA == cdEmpresa))
                .Select(x => x.Codigo.Lpn)
                .Join(_context.T_CONTENEDOR.Include("T_PICKING"),
                    l => new { ID_EXTERNO_CONTENEDOR = l.ID_LPN_EXTERNO, TP_CONTENEDOR = l.TP_LPN_TIPO, CD_EMPRESA = (int?)l.CD_EMPRESA },
                    c => new { c.ID_EXTERNO_CONTENEDOR, c.TP_CONTENEDOR, c.T_PICKING.CD_EMPRESA },
                    (l, c) => new { Lpn = l, Contenerdor = c })
                .Where(c => c.Contenerdor.CD_SITUACAO != SituacionDb.ContenedorEnviado
                    && c.Contenerdor.CD_SITUACAO != SituacionDb.ContenedorEnsambladoKit
                    && c.Contenerdor.CD_SITUACAO != SituacionDb.ContenedorTransferido)
                .GroupBy(x => new { x.Lpn.CD_EMPRESA })
                .Select(x => new { x.Key.CD_EMPRESA })
                .Join(_context.T_EMPRESA,
                    x => new { x.CD_EMPRESA },
                    e => new { e.CD_EMPRESA },
                    (x, e) => e);

            if (isCodigoMultidato)
            {
                query = query
                    .Join(_context.T_CODIGO_MULTIDATO_EMPRESA,
                        e => new { e.CD_EMPRESA },
                        cme => new { cme.CD_EMPRESA },
                        (e, cme) => new { Empresa = e, Codigo = cme })
                    .Where(x => x.Codigo.FL_HABILITADO == "S")
                    .Select(x => x.Empresa);
            }

            return query.OrderBy(e => e.CD_EMPRESA)
                .Select(e => new EmpresaMapper().MapToObject(e))
                .ToList();
        }

        public virtual Contenedor GetContenedorByIdExternoTipo(string idExternoContenedor, string tipoContenedor)
        {
            var entity = _context.T_CONTENEDOR
                .AsNoTracking()
                .FirstOrDefault(c => c.ID_EXTERNO_CONTENEDOR == idExternoContenedor && c.TP_CONTENEDOR == tipoContenedor
                    && c.CD_SITUACAO != SituacionDb.ContenedorEnviado
                    && c.CD_SITUACAO != SituacionDb.ContenedorEnsambladoKit
                    && c.CD_SITUACAO != SituacionDb.ContenedorTransferido);

            return _mapper.MapToObject(entity);
        }

        public virtual ContenedorPredefinido GetContenedorPredefinidoByIdExternoTipo(string idExterno, string tipoContenedor)
        {
            var entity = _context.T_CONTENEDORES_PREDEFINIDOS
                .AsNoTracking()
                .FirstOrDefault(c => c.ID_EXTERNO_CONTENEDOR == idExterno
                    && c.TP_CONTENEDOR == tipoContenedor);

            return entity == null ? null : _mapper.MapToObject(entity);
        }

        public virtual List<long> GetCargasContenedor(int nroPreparacion, int nroContenedor)
        {
            return _context.T_DET_PICKING
                .AsNoTracking()
                .Where(c => c.NU_PREPARACION == nroPreparacion && c.NU_CONTENEDOR == nroContenedor && c.NU_CARGA != null)
                .Select(c => c.NU_CARGA ?? -1)
                .Distinct()
                .ToList();
        }

        public virtual List<Contenedor> GetContenedoresCarga(CargaCamion carga)
        {
            List<Contenedor> list = new List<Contenedor>();
            var contenedores = _context.T_DET_PICKING
                .Include("T_CONTENEDOR")
                .AsNoTracking()
                .Where(dp => dp.CD_CLIENTE == carga.Cliente
                     && dp.CD_EMPRESA == carga.Empresa
                     && dp.NU_CARGA == carga.Carga
                     && dp.T_CONTENEDOR.CD_CAMION_FACTURADO == null
                     && dp.T_CONTENEDOR != null
                     && dp.ND_ESTADO == EstadoDetallePreparacion.ESTADO_PREPARADO //Se excluyen contenedores que ya esten facturados y no preparados
                     && dp.T_CONTENEDOR.CD_SITUACAO == SituacionDb.ContenedorEnPreparacion) //Agregado Mauro 2017-12-29 para mejorar performance
                 .Select(dp => dp.T_CONTENEDOR)
                 .Distinct()
                 .ToList();

            foreach (var cont in contenedores)
            {
                list.Add(_mapper.MapToObject(cont));
            }

            return list;
        }

        public virtual decimal GetPesoTotalContenedor(int preparacionDestino, int contenedorDestino)
        {
            return (_context.V_EXP110_CONT_PREP.FirstOrDefault(c => c.NU_CONTENEDOR == contenedorDestino && c.NU_PREPARACION == preparacionDestino)?.PS_NETO ?? 0);
        }

        public virtual List<Contenedor> GetContenedoresCargaFacturado(CargaCamion carga)
        {
            List<Contenedor> list = new List<Contenedor>();
            var contenedores = _context.T_DET_PICKING.AsNoTracking().Include("T_CONTENEDOR").Where(dp => dp.CD_CLIENTE == carga.Cliente
                 && dp.CD_EMPRESA == carga.Empresa
                 && dp.NU_CARGA == carga.Carga
                 && dp.T_CONTENEDOR != null
                 && dp.ND_ESTADO == EstadoDetallePreparacion.ESTADO_PREPARADO //Se excluyen contenedores que ya esten facturados y no preparados
                 && dp.T_CONTENEDOR.CD_SITUACAO == SituacionDb.ContenedorEnPreparacion) //Agregado Mauro 2017-12-29 para mejorar performance
                 .Select(dp => dp.T_CONTENEDOR)
                 .Distinct().ToList();

            foreach (var cont in contenedores)
            {
                list.Add(_mapper.MapToObject(cont));
            }
            return list;
        }

        public virtual List<Contenedor> GetContenedoreEnPuerta(int cdCamion, string ubicacionPuerta)
        {
            List<Contenedor> List = new List<Contenedor>();
            List<T_CONTENEDOR> colContenedores = _context.T_CONTENEDOR.AsNoTracking().Where(x => x.CD_CAMION == cdCamion && x.CD_ENDERECO == ubicacionPuerta && x.CD_SITUACAO == SituacionDb.ContenedorEnCamion).ToList();

            foreach (var a in colContenedores)
            {
                List.Add(_mapper.MapToObject(a));
            }
            return List;
        }

        public virtual Contenedor GetContenedoresEnPreparacion(int numeroPreparacion, int nuContenedor)
        {
            T_CONTENEDOR entity = _context.T_CONTENEDOR.AsNoTracking()
               .FirstOrDefault(con => con.NU_CONTENEDOR == nuContenedor && con.NU_PREPARACION == numeroPreparacion && con.CD_SITUACAO == 601);
            return entity == null ? null : _mapper.MapToObject(entity);
        }

        public virtual Contenedor GetContenedor(int nuContenedor)
        {
            var entity = _context.T_CONTENEDOR
                .AsNoTracking()
                .FirstOrDefault(con => con.NU_CONTENEDOR == nuContenedor);

            return entity == null ? null : _mapper.MapToObject(entity);
        }

        public virtual Contenedor GetContenedor(int numeroPreparacion, int nuContenedor)
        {
            var entity = _context.T_CONTENEDOR
                .AsNoTracking()
                .FirstOrDefault(con => con.NU_CONTENEDOR == nuContenedor
                    && con.NU_PREPARACION == numeroPreparacion);

            return entity == null ? null : _mapper.MapToObject(entity);
        }

        public virtual Contenedor GetContenedorWithPreparacion(int numeroPreparacion, int nuContenedor)
        {
            var entity = _context.T_CONTENEDOR
                .Include("T_PICKING")
                .AsNoTracking()
                .FirstOrDefault(con => con.NU_CONTENEDOR == nuContenedor
                    && con.NU_PREPARACION == numeroPreparacion);

            var contenedor = entity == null ? null : _mapper.MapToObject(entity);

            if (contenedor != null)
                contenedor.Preparacion = this._mapperPreparacion.MapToObject(entity.T_PICKING);

            return contenedor;
        }

        public virtual Camion GetCamionAsignado(int nuContenedor, int preparacion)
        {
            var camion = (from CC in _context.V_EXP011_CONTENEDOR_CAMION
                          join CA in _context.T_CAMION on CC.CD_CAMION equals CA.CD_CAMION
                          where CC.NU_CONTENEDOR == nuContenedor &&
                              CC.NU_PREPARACION == preparacion
                          select CA)?.FirstOrDefault();

            return _mapperCamion.MapToObject(camion);
        }

        public virtual UnidadTransporte GetUnidadTransporte(int? ut)
        {
            return _mapperUnidad.MapToObject(_context.T_UNIDAD_TRANSPORTE.FirstOrDefault(x => x.NU_UNIDAD_TRANSPORTE == ut));
        }

        #endregion

        #region Add
        public virtual void AddTipoContenedor(TipoContenedor tipo)
        {
            var entity = this._mapper.MapToEntity(tipo);
            this._context.T_TIPO_CONTENEDOR.Add(entity);
        }

        public virtual T_STOCK_ENVASE CrearEnvase(string numero, string tipo, string cdBarras, long transaccion)
        {
            return new T_STOCK_ENVASE
            {
                ID_ENVASE = numero,
                ND_TP_ENVASE = tipo,
                DT_ADDROW = DateTime.Now,
                DT_UDPROW = DateTime.Now,
                ND_ESTADO_ENVASE = "SENVNEW",
                CD_BARRAS = cdBarras,
                NU_TRANSACCION = transaccion
            };
        }

        public virtual void AddContenedor(Contenedor contenedor)
        {
            T_CONTENEDOR entity = this._mapper.MapToEntity(contenedor);
            this._context.T_CONTENEDOR.Add(entity);
        }

        public virtual void MarcarImpresionContenedor(Contenedor contenedor, string predio, long transaccion, bool reImpresion = false)
        {
            if (!this.TipoContenedorEnvase(contenedor.TipoContenedor)) return;

            T_STOCK_ENVASE envase = _context.T_STOCK_ENVASE
                .FirstOrDefault(x => x.ID_ENVASE == contenedor.IdExterno
                    && x.ND_TP_ENVASE == contenedor.TipoContenedor);

            if (envase == null)
            {
                envase = CrearEnvase(contenedor.IdExterno, contenedor.TipoContenedor, contenedor.CodigoBarras, transaccion);
                _context.T_STOCK_ENVASE.Add(envase);
            }

            if (reImpresion)
            {
                envase.ND_ESTADO_ENVASE = "SENVRIM"; //Cambiar ENUM
            }

            envase.CD_AGENTE = predio;
            envase.TP_AGENTE = "DEP";
            envase.CD_EMPRESA = null;

            envase.DT_UDPROW = DateTime.Now;
            envase.DS_ULTIMO_MOVIMIENTO = contenedor.DescripcionContenedor;
            envase.NU_TRANSACCION = transaccion;
        }

        #endregion

        #region Update

        public virtual void UpdateContenedor(Contenedor contenedor)
        {
            contenedor.FechaModificado = DateTime.Now;
            T_CONTENEDOR entity = this._mapper.MapToEntity(contenedor);
            T_CONTENEDOR attachedEntity = _context.T_CONTENEDOR.Local.FirstOrDefault(x => x.NU_PREPARACION == contenedor.NumeroPreparacion && x.NU_CONTENEDOR == contenedor.Numero);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.State = EntityState.Modified;
                attachedEntry.CurrentValues.SetValues(entity);
            }
            else
            {
                _context.T_CONTENEDOR.Attach(entity);
                _context.Entry<T_CONTENEDOR>(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateUnidadTransporte(UnidadTransporte unidadTranporte)
        {
            unidadTranporte.FechaModificacion = DateTime.Now;
            T_UNIDAD_TRANSPORTE entity = this._mapperUnidad.MapToEntity(unidadTranporte);
            T_UNIDAD_TRANSPORTE attachedEntity = _context.T_UNIDAD_TRANSPORTE.Local.FirstOrDefault(x => x.NU_UNIDAD_TRANSPORTE == unidadTranporte.NumeroUnidadTransporte);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.State = EntityState.Modified;
                attachedEntry.CurrentValues.SetValues(entity);
            }
            else
            {
                _context.T_UNIDAD_TRANSPORTE.Attach(entity);
                _context.Entry<T_UNIDAD_TRANSPORTE>(entity).State = EntityState.Modified;
            }
        }

        #endregion

        #region Remove
        public virtual void DeleteTipoContenedor(TipoContenedor tipo)
        {
            var entity = _mapper.MapToEntity(tipo);
            var attachedEntity = this._context.T_TIPO_CONTENEDOR.Local
                .FirstOrDefault(d => d.TP_CONTENEDOR == entity.TP_CONTENEDOR);

            if (attachedEntity != null)
            {
                this._context.T_TIPO_CONTENEDOR.Remove(attachedEntity);
            }
            else
            {
                this._context.T_TIPO_CONTENEDOR.Attach(entity);
                this._context.T_TIPO_CONTENEDOR.Remove(entity);
            }
        }
        #endregion

        #region Dapper
        public virtual List<Contenedor> GetContenedoresActivos(IEnumerable<Contenedor> contenedores)
        {
            IEnumerable<Contenedor> resultado = new List<Contenedor>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_CONTENEDOR_TEMP (ID_EXTERNO_CONTENEDOR, TP_CONTENEDOR) VALUES (:IdExterno,:TipoContenedor)";
                    _dapper.Execute(connection, sql, contenedores, transaction: tran);

                    sql = GetSqlSelectContenedor() +
                        @" FROM T_CONTENEDOR co 
                            INNER JOIN T_CONTENEDOR_TEMP T ON co.ID_EXTERNO_CONTENEDOR = T.ID_EXTERNO_CONTENEDOR AND co.TP_CONTENEDOR = T.TP_CONTENEDOR
                            WHERE co.CD_SITUACAO IN (:ContenedorEnPreparacion , :ContenedorEnCamion)";

                    resultado = _dapper.Query<Contenedor>(connection, sql, param: new
                    {
                        ContenedorEnPreparacion = SituacionDb.ContenedorEnPreparacion,
                        ContenedorEnCamion = SituacionDb.ContenedorEnCamion
                    }, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado.ToList();

        }

        public virtual List<Contenedor> GetContenedores(IEnumerable<Contenedor> contenedores)
        {
            IEnumerable<Contenedor> resultado = new List<Contenedor>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_CONTENEDOR_TEMP (ID_EXTERNO_CONTENEDOR, TP_CONTENEDOR) VALUES (:IdExterno, :TipoContenedor)";
                    _dapper.Execute(connection, sql, contenedores, transaction: tran);

                    sql = GetSqlSelectContenedor() +
                          @" FROM T_CONTENEDOR co 
                            INNER JOIN T_CONTENEDOR_TEMP T ON co.ID_EXTERNO_CONTENEDOR = T.ID_EXTERNO_CONTENEDOR AND co.TP_CONTENEDOR= T.TP_CONTENEDOR";

                    resultado = _dapper.Query<Contenedor>(connection, sql, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado.ToList();

        }

        public virtual int GetQtBulto(int nuPreparacion, string comparteContenedor, string codigoCliente, string nuPedido, string dsEntrega)
        {

            string query = @"   SELECT 
                                      COALESCE(MAX(CO.QT_BULTO),0)
                                FROM  T_CONTENEDOR   CO,
                                      T_DET_PICKING  DP,
                                      T_PEDIDO_SAIDA PS
                                WHERE 
                                      CO.nu_contenedor  = DP.nu_contenedor
                                  AND CO.nu_preparacion = DP.nu_preparacion
                                  AND CO.nu_preparacion = :nuPreparacion
                                  AND PS.nu_pedido      = DP.nu_pedido
                                  AND PS.cd_cliente     = DP.cd_cliente    
                                  AND PS.cd_empresa     = DP.cd_empresa
                                  AND ((:comparteContenedor = 'N' and :nuPedido = PS.nu_pedido) OR :comparteContenedor = 'S')
                                  AND PS.Cd_Cliente = :codigoCliente
                                  AND COALESCE(PS.VL_COMPARTE_CONTENEDOR_PICKING,'N') = :comparteContenedor
                                  AND COALESCE(PS.ds_endereco,'-') = COALESCE(:dsEntrega,'-')
                                  AND CO.id_contenedor_empaque = 'S'";

            var result = _dapper.Get<int>(query, new DynamicParameters(new
            {
                nuPreparacion = nuPreparacion,
                comparteContenedor = comparteContenedor,
                codigoCliente = codigoCliente,
                nuPedido = nuPedido,
                dsEntrega = dsEntrega
            }));

            return result;
        }

        public virtual string GetSqlSelectContenedor()
        {
            return @"SELECT 
                          co.NU_CONTENEDOR as Numero,
                          co.NU_PREPARACION as NumeroPreparacion,
                          co.TP_CONTENEDOR as TipoContenedor,
                          co.CD_CAMION as CodigoCamion,
                          co.CD_AGRUPADOR as CodigoAgrupador,
                          co.CD_CAMION_CONGELADO as CodigoCamionCongelado,
                          co.CD_CAMION_FACTURADO as CamionFacturado,
                          co.CD_CANAL as CodigoCanal,
                          co.CD_ENDERECO as Ubicacion,
                          co.CD_FUNCIONARIO_EXPEDICION as CodigoFuncionarioExpedicion,
                          co.CD_PORTA as CodigoPuerta,
                          co.CD_SITUACAO as EstadoId,
                          co.CD_SUB_CLASSE as CodigoSubClase,
                          co.CD_UNIDAD_BULTO as CodigoUnidadBulto,
                          co.DS_CONTENEDOR as DescripcionContenedor,
                          co.DT_ADDROW as FechaAgregado,
                          co.DT_EXPEDIDO as FechaExpedido,
                          co.DT_PULMON as FechaPulmon,
                          co.DT_UPDROW as FechaModificado,
                          co.FL_HABILITADO as Habilitado,
                          co.FL_SEPARADO_DOS_FASES as SegundaFase,
                          co.ID_CONTENEDOR_EMPAQUE as IdContenedorEmpaque,
                          co.ID_PRECINTO_1 as Precinto1,
                          co.ID_PRECINTO_2 as Precinto2,
                          co.NU_TRANSACCION as NumeroTransaccion,
                          co.NU_UNIDAD_TRANSPORTE as NumeroUnidadTransporte,
                          co.NU_VIAJE as NumeroViaje,
                          co.PS_REAL as PesoReal,
                          co.QT_BULTO as CantidadBulto,
                          co.TP_CONTROL as TipoControl,
                          co.VL_ALTURA as Altura,
                          co.VL_CONTROL as ValorControl,
                          co.VL_CUBAGEM as ValorCubagem,
                          co.VL_LARGURA as Largo,
                          co.VL_PROFUNDIDADE as Profundidad,
                          co.NU_TRANSACCION_DELETE as NumeroTransaccionDelete,                          
                          co.ID_EXTERNO_CONTENEDOR as IdExterno,
                          co.CD_BARRAS as CodigoBarras,
                          co.NU_LPN as NroLpn,
                          co.ID_EXTERNO_TRACKING as IdExternoTracking ";
        }

        public virtual List<int> GetNewContenedores(int count)
        {
            var connection = _context.Database.GetDbConnection();
            var tran = _context.Database.CurrentTransaction?.GetDbTransaction();

            return _dapper.GetNextSequenceValues<int>(connection, Secuencias.S_NU_CONTENEDOR, count, tran).ToList();
        }

        #endregion
    }
}
