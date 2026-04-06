using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Extensions;
using WIS.Domain.General;
using WIS.Domain.General.API.Bulks;
using WIS.Domain.General.Enums;
using WIS.Domain.Recepcion;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class AgenteRepository
    {
        protected readonly WISDB _context;
        protected readonly string _cdAplicacion;
        protected readonly int _userId;
        protected readonly IDapper _dapper;
        protected readonly AgenteMapper _mapper;

        public AgenteRepository(WISDB context, string cdAplicacion, int userId, IDapper dapper)
        {
            this._context = context;
            this._cdAplicacion = cdAplicacion;
            this._userId = userId;
            this._mapper = new AgenteMapper();
            this._dapper = dapper;
        }

        #region Any

        public virtual bool AnyCliente(string codigoInternoAgente)
        {
            return this._context.V_AGENTE
                .AsNoTracking()
                .Any(d => d.CD_CLIENTE == codigoInternoAgente);
        }

        public virtual bool AnyCliente(string codigoInternoAgente, int codigoEmpresa)
        {
            return this._context.V_AGENTE
                .AsNoTracking()
                .Any(d => d.CD_CLIENTE == codigoInternoAgente && d.CD_EMPRESA == codigoEmpresa);
        }

        public virtual bool AnyAgente(string codigoAgente)
        {
            return this._context.T_CLIENTE
                .AsNoTracking()
                .Any(d => d.CD_AGENTE == codigoAgente);
        }

        public virtual bool AnyAgente(string codigoAgente, int empresa)
        {
            return this._context.T_CLIENTE
                .AsNoTracking()
                .Any(d => d.CD_AGENTE == codigoAgente && d.CD_EMPRESA == empresa);
        }

        public virtual bool AnyAgente(string codigoAgente, string tipoAgente)
        {
            return this._context.T_CLIENTE
                .AsNoTracking()
                .Any(d => d.CD_AGENTE == codigoAgente && d.TP_AGENTE == tipoAgente);
        }

        public virtual bool AnyAgente(string codigoAgente, string tipoAgente, int codigoEmpresa)
        {
            return this._context.T_CLIENTE
                .AsNoTracking().
                Any(d => d.CD_AGENTE == codigoAgente && d.TP_AGENTE == tipoAgente && d.CD_EMPRESA == codigoEmpresa);
        }

        public virtual bool AnyAgentePorRuta(short ordenCarga, short ruta, string tipoAgente, string codigoAgente, int codigoEmpresa, out Agente agente)
        {
            T_CLIENTE entity = this._context.T_CLIENTE
                .AsNoTracking()
                .FirstOrDefault(d => d.CD_ROTA == ruta
                    && d.NU_PRIOR_CARGA == ordenCarga
                    && (!d.CD_AGENTE.Equals(codigoAgente) || !d.TP_AGENTE.Equals(tipoAgente) || d.CD_EMPRESA != codigoEmpresa));

            if (entity == null)
            {
                agente = null;
                return false;
            }
            else
            {
                agente = this._mapper.MapToObject(entity);
                return true;
            }
        }

        public virtual bool AnyLoteAuto(int nroAgenda, string cdProducto, decimal cdFaixa)
        {
            return _context.T_DET_AGENDA
                .AsNoTracking()
                .Any(d => d.NU_AGENDA == nroAgenda
                    && d.CD_PRODUTO == cdProducto
                    && d.CD_FAIXA == cdFaixa
                    && d.NU_IDENTIFICADOR == ManejoIdentificadorDb.IdentificadorAuto
                    && d.QT_AGENDADO > (d.QT_CROSS_DOCKING ?? 0));
        }

        public virtual bool AnyClienteRutaPredio(string codigoClienteAgente, int idEmpresa, string predio, short idRutaActual)
        {
            return this._context.T_CLIENTE_RUTA_PREDIO
                .AsNoTracking()
                .Any(d => d.CD_CLIENTE == codigoClienteAgente
                    && d.CD_EMPRESA == idEmpresa
                    && d.NU_PREDIO == predio
                    && d.CD_ROTA != idRutaActual);
        }

        public virtual bool AnyClienteRutaPredio(string codigoClienteAgente, int idEmpresa, string predio)
        {
            return this._context.T_CLIENTE_RUTA_PREDIO
                .AsNoTracking()
                .Any(d => d.CD_CLIENTE == codigoClienteAgente
                    && d.CD_EMPRESA == idEmpresa
                    && d.NU_PREDIO == predio);
        }

        public virtual bool AnyProblemaDetalle(AgendaDetalle detAgenda)
        {
            List<short> situacionesExcluir = new List<short>{
                SituacionDb.AgendaAbierta,
                SituacionDb.AgendaAguardandoDesembarque,
                SituacionDb.AgendaNfCoincidenAgenda,
                SituacionDb.AgendaRecepcionEnProgreso
            };

            return _context.T_RECEPCION_AGENDA_PROBLEMA
                .AsNoTracking()
                .Any(x => x.NU_AGENDA == detAgenda.IdAgenda
                    && x.CD_FAIXA == detAgenda.Faixa
                    && x.CD_PRODUTO == detAgenda.CodigoProducto
                    && x.NU_IDENTIFICADOR == detAgenda.Identificador
                    && x.FL_ACEPTADO == "N");
        }

        public virtual bool AnyProblema(Agenda agenda)
        {
            List<short> situacionesExcluir = new List<short>{
                SituacionDb.AgendaAbierta,
                SituacionDb.AgendaAguardandoDesembarque,
                SituacionDb.AgendaNfCoincidenAgenda,
                SituacionDb.AgendaRecepcionEnProgreso
            };

            return _context.T_RECEPCION_AGENDA_PROBLEMA
                .AsNoTracking()
                .Any(d => d.NU_AGENDA == agenda.Id && d.FL_ACEPTADO == "N");
        }

        #endregion

        #region Get
        public virtual string GetDescripcionAgente(int codigoEmpresa, string codigoInternoAgente)
        {
            return this._context.T_CLIENTE.Where(d => d.CD_EMPRESA == codigoEmpresa && d.CD_CLIENTE == codigoInternoAgente).FirstOrDefault()?.DS_CLIENTE;
        }

        public virtual string GetTipoAgente(string codigoInternoAgente)
        {
            return this._context.T_CLIENTE.Where(d => d.CD_CLIENTE == codigoInternoAgente).FirstOrDefault()?.TP_AGENTE;
        }

        public virtual IQueryable<T_CLIENTE> GetAgente(Expression<Func<T_CLIENTE, bool>> where)
        {
            return this._context.T_CLIENTE
                .Include("T_ROTA")
                .Include("T_CLIENTE_RUTA_PREDIO")
                .Include("T_CLIENTE_RUTA_PREDIO.T_ROTA")
                .AsNoTracking()
                .Where(where);
        }

        public virtual Agente GetAgente(int codigoEmpresa, string codigoInternoAgente)
        {
            T_CLIENTE cliente = this.GetAgente(d => d.CD_EMPRESA == codigoEmpresa && d.CD_CLIENTE == codigoInternoAgente)
                .FirstOrDefault();

            return this._mapper.MapToObject(cliente, null, cliente?.T_ROTA, null);
        }

        public virtual Agente GetAgenteByCodigoAgente(string codigoAgente)
        {
            T_CLIENTE cliente = this.GetAgente(d => d.CD_AGENTE == codigoAgente)
                .FirstOrDefault();

            return this._mapper.MapToObject(cliente, null, cliente.T_ROTA, null);
        }

        public virtual Agente GetAgente(int cdEmpresa, AgenteTipo tipo, string cdAgente)
        {
            string tipoString = this._mapper.MapTipo(tipo);
            return GetAgente(cdEmpresa, cdAgente, tipoString);
        }

        public virtual Agente GetAgente(int empresa, string codigo, string tipo)
        {
            T_CLIENTE cliente = this.GetAgente(d => d.CD_EMPRESA == empresa && d.CD_AGENTE == codigo && d.TP_AGENTE == tipo)
                .FirstOrDefault();

            return this._mapper.MapToObject(cliente, null, cliente.T_ROTA, null);
        }

        public virtual Agente GetAgenteConRelaciones(int codigoEmpresa, string codigoInternoAgente)
        {
            T_CLIENTE cliente = this._context.T_CLIENTE.Include("T_ROTA").Include("T_CLIENTE_RUTA_PREDIO").AsNoTracking()
                .FirstOrDefault(d => d.CD_EMPRESA == codigoEmpresa && d.CD_CLIENTE == codigoInternoAgente);

            T_PAIS_SUBDIVISION_LOCALIDAD localidad = this._context.T_PAIS_SUBDIVISION_LOCALIDAD.Include("T_PAIS_SUBDIVISION")
                .AsNoTracking().FirstOrDefault(s => s.ID_LOCALIDAD == cliente.ID_LOCALIDAD);

            if (localidad != null && localidad.T_PAIS_SUBDIVISION != null && localidad.T_PAIS_SUBDIVISION != null)
                localidad.T_PAIS_SUBDIVISION.T_PAIS = this._context.T_PAIS.FirstOrDefault(p => p.CD_PAIS == localidad.T_PAIS_SUBDIVISION.CD_PAIS);

            var tipoFiscal = _context.T_DET_DOMINIO.AsNoTracking().FirstOrDefault(w => w.CD_DOMINIO == CodigoDominioDb.TiposFiscales
                                                                              && w.NU_DOMINIO == cliente.ND_TIPO_FISCAL);

            return this._mapper.MapToObject(cliente, localidad, cliente.T_ROTA, tipoFiscal);
        }

        public virtual Agente GetAgenteContenedor(int numeroContenedor, int numeroPreparacion)
        {
            return this._mapper.MapToObject(
                        (
                        from dp in _context.T_DET_PICKING.AsNoTracking()
                        join c in _context.T_CLIENTE.Include("T_CLIENTE_RUTA_PREDIO").AsNoTracking() on new { dp.CD_EMPRESA, dp.CD_CLIENTE } equals new { c.CD_EMPRESA, c.CD_CLIENTE }
                        where
                            dp.NU_CONTENEDOR == numeroContenedor
                           && dp.NU_PREPARACION == numeroPreparacion
                        select c
                        ).FirstOrDefault()
                    );
        }

        public virtual List<Agente> GetClienteByDescripcionOrAgentePartial(string searchValue, int idEmpresa)
        {
            var agentes = new List<Agente>();

            var entries = this.GetAgente(d => d.CD_EMPRESA == idEmpresa && d.TP_AGENTE == TipoAgenteDb.Cliente
                && (d.DS_CLIENTE.ToLower().Contains(searchValue.ToLower()) || d.CD_AGENTE.ToLower() == searchValue.ToLower())
                && d.CD_SITUACAO == SituacionDb.Activo).ToList();

            foreach (var entry in entries)
            {
                agentes.Add(this._mapper.MapToObject(entry));
            }

            return agentes;
        }

        public virtual List<Agente> GetByDescripcionOrAgentePartial(string searchValue, int idEmpresa)
        {
            var agentes = new List<Agente>();

            var entries = this.GetAgente(d => d.CD_EMPRESA == idEmpresa
                && (d.DS_CLIENTE.ToLower().Contains(searchValue.ToLower()) || d.CD_AGENTE.ToLower() == searchValue.ToLower()) && d.CD_SITUACAO == SituacionDb.Activo).ToList();

            foreach (var entry in entries)
            {
                agentes.Add(this._mapper.MapToObject(entry));
            }
            return agentes;
        }

        public virtual List<Agente> GetAgenteByKeysPartial(string valueSearch, string tipoAgente, int codigoEmpresa)
        {
            return this.GetAgente(w =>
                        w.CD_EMPRESA == codigoEmpresa
                        && w.TP_AGENTE == tipoAgente
                        && (
                            w.CD_AGENTE.ToLower().Contains(valueSearch.ToLower())
                            || w.DS_CLIENTE.ToLower().Contains(valueSearch.ToLower())
                        )
                    ).ToList()
                   .Select(w => this._mapper.MapToObject(w)).ToList();
        }

        public virtual List<Agente> GetAllClientes()
        {
            return this._context.T_CLIENTE.Include("T_ROTA").Include("T_PAIS_SUBDIVISION_LOCALIDAD")
                .AsNoTracking().ToList().Select(w => this._mapper.MapToObject(w, w.T_PAIS_SUBDIVISION_LOCALIDAD, w.T_ROTA)).ToList();
        }

        public virtual List<Agente> GetAgentesNoSincronizados(DateTime? fechaInicial = null)
        {
            var agentes = new List<Agente>();

            var query = this._context.T_CLIENTE.Include("T_ROTA").Include("T_CLIENTE_RUTA_PREDIO").Where(c => c.FL_SYNC_REALIZADA != "S");
            if (fechaInicial != null)
                query = this._context.T_CLIENTE.Include("T_ROTA").Include("T_CLIENTE_RUTA_PREDIO").Where(c => c.FL_SYNC_REALIZADA != "S" && c.DT_SITUACAO >= fechaInicial);

            foreach (var cliente in query)
            {
                var localidad = this._context.T_PAIS_SUBDIVISION_LOCALIDAD.Include("T_PAIS_SUBDIVISION").AsNoTracking().FirstOrDefault(s => s.ID_LOCALIDAD == cliente.ID_LOCALIDAD);

                if (localidad != null && localidad.T_PAIS_SUBDIVISION != null && localidad.T_PAIS_SUBDIVISION != null)
                    localidad.T_PAIS_SUBDIVISION.T_PAIS = this._context.T_PAIS.FirstOrDefault(p => p.CD_PAIS == localidad.T_PAIS_SUBDIVISION.CD_PAIS);

                var tipoFiscal = _context.T_DET_DOMINIO.AsNoTracking().FirstOrDefault(w => w.CD_DOMINIO == CodigoDominioDb.TiposFiscales && w.NU_DOMINIO == cliente.ND_TIPO_FISCAL);

                agentes.Add(this._mapper.MapToObject(cliente, localidad, cliente.T_ROTA, tipoFiscal));
            }

            return agentes;
        }

        public virtual DominioDetalle GetTipoAgente(Agente agente)
        {
            var _mapperDominio = new DominioMapper();

            var valorDominioTipoAgente = agente.Tipo;

            var entity = _context.T_DET_DOMINIO
                .AsNoTracking()
                .FirstOrDefault(w => w.CD_DOMINIO == CodigoDominioDb.TiposDeAgentes
                    && w.CD_DOMINIO_VALOR == valorDominioTipoAgente);

            return _mapperDominio.MapToObject(entity);
        }

        public virtual List<DominioDetalle> GetTiposAgente()
        {
            var _mapperDominio = new DominioMapper();

            return _context.T_DET_DOMINIO
                .AsNoTracking()
                .Where(w => w.CD_DOMINIO == "TAGE")
                .ToList()
                .Select(w => _mapperDominio.MapToObject(w))
                .ToList();
        }

        public virtual List<DominioDetalle> GetTiposAgenteFiscales()
        {
            var _mapperDominio = new DominioMapper();

            return _context.T_DET_DOMINIO
                .AsNoTracking()
                .Where(w => w.CD_DOMINIO == "TIPOFISCAL")
                .ToList()
                .Select(w => _mapperDominio.MapToObject(w))
                .ToList();
        }

        public virtual List<Agente> GetAgenteByNombrePartial(int cdEmpresa, AgenteTipo tipo, string nombre)
        {
            string tipoString = this._mapper.MapTipo(tipo);

            return this._context.T_CLIENTE
                .AsNoTracking()
                .Where(d => d.CD_EMPRESA == cdEmpresa
                    && d.TP_AGENTE == tipoString
                    && ((d.CD_AGENTE.ToLower().Contains(nombre.ToLower()))
                        || d.DS_CLIENTE.ToLower().Contains(nombre.ToLower()))
                    && d.CD_SITUACAO != SituacionDb.Inactivo)
                .Select(d => this._mapper.MapToObject(d, null, null, null, null))
                .ToList();
        }

        public virtual AgenteRutaPredio GetAgenteRutaPredio(int codigoEmpresa, string codigoInternoAgente, string predio, short idRuta)
        {
            T_CLIENTE_RUTA_PREDIO ruta = this._context.T_CLIENTE_RUTA_PREDIO
                .AsNoTracking()
                .FirstOrDefault(d => d.CD_CLIENTE == codigoInternoAgente
                    && d.CD_EMPRESA == codigoEmpresa && d.NU_PREDIO == predio
                    && d.CD_ROTA == idRuta);

            return this._mapper.MapToObject(ruta);
        }

        public virtual ClienteDiasValidezVentana GetVentanaLiberacionCliente(int empresa, string cliente, string ventanaLiracion)
        {
            return this._mapper.MapToObject(this._context.T_CLIENTE_DIASVALIDEZ_VENTANA.AsNoTracking()
                .FirstOrDefault(x => x.CD_EMPRESA == empresa &&
                    x.CD_CLIENTE == cliente &&
                    x.CD_VENTANA_LIBERACION == ventanaLiracion));
        }

        #endregion

        #region Add
        public virtual void AddAgente(Agente agente)
        {
            agente.CodigoInterno = agente.Codigo;

            if (agente.Tipo == TipoAgenteDb.Proveedor && agente.CodigoInterno != AgentesPorDefectoEmpresaDb.ProveedorMuestrasCodigo)
            {
                int Id = this._context.GetNextSequenceValueInt(_dapper, "S_AGENTE_PROVEEDOR");
                agente.CodigoInterno = $"{TipoAgenteDb.Proveedor}-{Id}";
            }

            agente.FechaAlta = DateTime.Now;
            agente.FechaSituacion = DateTime.Now;

            T_CLIENTE entity = this._mapper.MapToEntity(agente);
            this._context.T_CLIENTE.Add(entity);
        }

        public virtual void AddClienteRutaPredio(AgenteRutaPredio ruta)
        {
            T_CLIENTE_RUTA_PREDIO entity = this._mapper.MapToEntity(ruta);

            this._context.T_CLIENTE_RUTA_PREDIO.Add(entity);
        }

        public virtual void AddVentanaLiberacionCliente(ClienteDiasValidezVentana configVentana)
        {
            configVentana.FechaModificacion = DateTime.Now;
            T_CLIENTE_DIASVALIDEZ_VENTANA entity = this._mapper.MapToEntity(configVentana);
            this._context.T_CLIENTE_DIASVALIDEZ_VENTANA.Add(entity);
        }

        #endregion

        #region Update

        public virtual void UpdateAgente(Agente agente)
        {
            agente.FechaModificacion = DateTime.Now;
            T_CLIENTE entity = this._mapper.MapToEntity(agente);
            T_CLIENTE attachedEntity = _context.T_CLIENTE.Local
                .FirstOrDefault(x => x.CD_CLIENTE == entity.CD_CLIENTE && x.CD_EMPRESA == entity.CD_EMPRESA);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.State = EntityState.Modified;
                attachedEntry.CurrentValues.SetValues(entity);
            }
            else
            {
                _context.T_CLIENTE.Attach(entity);
                _context.Entry<T_CLIENTE>(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateClienteRutaPredio(AgenteRutaPredio ruta)
        {
            var entity = this._mapper.MapToEntity(ruta);
            var attachedEntity = _context.T_CLIENTE_RUTA_PREDIO.Local
                .FirstOrDefault(w => w.CD_EMPRESA == entity.CD_EMPRESA
                    && w.CD_CLIENTE == entity.CD_CLIENTE
                    && w.NU_PREDIO == entity.NU_PREDIO);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.T_CLIENTE_RUTA_PREDIO.Attach(entity);
                this._context.Entry(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateVentanaLiberacionCliente(ClienteDiasValidezVentana configVentana)
        {
            configVentana.FechaModificacion = DateTime.Now;

            var entity = this._mapper.MapToEntity(configVentana);
            var attachedEntity = _context.T_CLIENTE_DIASVALIDEZ_VENTANA.Local
                .FirstOrDefault(x => x.CD_EMPRESA == configVentana.Empresa &&
                    x.CD_CLIENTE == configVentana.Cliente &&
                    x.CD_VENTANA_LIBERACION == configVentana.VentanaLiberacion);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.State = EntityState.Modified;
                attachedEntry.CurrentValues.SetValues(entity);
            }
            else
            {
                _context.T_CLIENTE_DIASVALIDEZ_VENTANA.Attach(entity);
                _context.Entry<T_CLIENTE_DIASVALIDEZ_VENTANA>(entity).State = EntityState.Modified;
            }
        }

        #endregion

        #region Remove

        public virtual void RemoveClienteRutaPredio(AgenteRutaPredio ruta)
        {
            var entity = this._mapper.MapToEntity(ruta);
            var attachedEntity = _context.T_CLIENTE_RUTA_PREDIO.Local
                .FirstOrDefault(w => w.CD_EMPRESA == entity.CD_EMPRESA
                    && w.CD_CLIENTE == entity.CD_CLIENTE
                    && w.NU_PREDIO == entity.NU_PREDIO);

            if (attachedEntity != null)
            {
                this._context.T_CLIENTE_RUTA_PREDIO.Remove(attachedEntity);
            }
            else
            {
                this._context.T_CLIENTE_RUTA_PREDIO.Attach(entity);
                this._context.T_CLIENTE_RUTA_PREDIO.Remove(entity);
            }
        }

        public virtual void DeleteVentanaLiberacionCLiente(ClienteDiasValidezVentana configVentana)
        {

            var entity = this._mapper.MapToEntity(configVentana);
            var attachedEntity = _context.T_CLIENTE_DIASVALIDEZ_VENTANA.Local
                .FirstOrDefault(x => x.CD_EMPRESA == configVentana.Empresa &&
                    x.CD_CLIENTE == configVentana.Cliente &&
                    x.CD_VENTANA_LIBERACION == configVentana.VentanaLiberacion);

            if (attachedEntity != null)
            {
                this._context.T_CLIENTE_DIASVALIDEZ_VENTANA.Remove(attachedEntity);
            }
            else
            {
                this._context.T_CLIENTE_DIASVALIDEZ_VENTANA.Attach(entity);
                this._context.T_CLIENTE_DIASVALIDEZ_VENTANA.Remove(entity);
            }

        }

        #endregion

        #region Dapper

        public virtual IEnumerable<string> GetTiposAgentes()
        {
            var sql = @"SELECT CD_DOMINIO_VALOR FROM T_DET_DOMINIO WHERE CD_DOMINIO = 'TAGE'";

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();
                return _dapper.Query<string>(connection, sql);
            }
        }

        public virtual IEnumerable<Agente> GetAgentes(IEnumerable<Agente> agentes)
        {
            IEnumerable<Agente> resultado = new List<Agente>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_CLIENTE_TEMP (CD_AGENTE, TP_AGENTE, CD_EMPRESA) VALUES (:Codigo, :Tipo, :Empresa)";
                    _dapper.Execute(connection, sql, agentes, transaction: tran);

                    sql = GetSqlSelectAgente() +
                        @"INNER JOIN T_CLIENTE_TEMP T ON A.CD_AGENTE = T.CD_AGENTE 
                            AND A.TP_AGENTE = T.TP_AGENTE 
                            AND A.CD_EMPRESA = T.CD_EMPRESA";

                    resultado = _dapper.Query<Agente>(connection, sql, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual IEnumerable<Agente> GetAgentesById(IEnumerable<Agente> agentes)
        {
            IEnumerable<Agente> resultado = new List<Agente>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_CLIENTE_TEMP (CD_CLIENTE, CD_EMPRESA) VALUES (:CodigoInterno, :Empresa)";
                    _dapper.Execute(connection, sql, agentes, transaction: tran);

                    sql = GetSqlSelectAgente() +
                        @"INNER JOIN T_CLIENTE_TEMP T ON A.CD_CLIENTE = T.CD_CLIENTE
                            AND A.CD_EMPRESA = T.CD_EMPRESA";

                    resultado = _dapper.Query<Agente>(connection, sql, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual async Task AddAgentes(List<Agente> agentes, IAgenteServiceContext context, CancellationToken cancelToken = default)
        {
            await AddAgentes(GetBulkOperationContext(agentes, context), cancelToken);
        }

        public virtual AgenteBulkOperationContext GetBulkOperationContext(List<Agente> agentes, IAgenteServiceContext serviceContext)
        {
            var context = new AgenteBulkOperationContext();
            var newAgentes = new Dictionary<string, Agente>();
            var newClientesiasValidezVentana = new List<ClienteDiasValidezVentana>();

            var cantidadAgentesNew = agentes
                .GroupJoin(serviceContext.Agentes.Values,
                    a => new { a.Codigo, a.Tipo },
                    sca => new { sca.Codigo, sca.Tipo },
                    (a, sca) => new { a, sca })
                 .SelectMany(asca => asca.sca.DefaultIfEmpty(),
                    (asca, sca) => new { Agente = asca.a, sca = sca })
                .Where(x => x.sca == null && x.Agente.Tipo == TipoAgenteDb.Proveedor || x.Agente.Tipo == TipoAgenteDb.Deposito).Count();
            List<int> secuencias = null;
            if (cantidadAgentesNew > 0)
            {
                secuencias = GetNewIdCliente(cantidadAgentesNew);
            }

            foreach (var agente in agentes)
            {
                var model = serviceContext.Agentes.Values.FirstOrDefault(x => x.Codigo == agente.Codigo && x.Tipo == agente.Tipo);

                if (model == null)
                {
                    agente.CodigoInterno = agente.Codigo;

                    if (agente.Tipo == TipoAgenteDb.Proveedor)
                    {
                        var id = secuencias.FirstOrDefault();
                        secuencias.Remove(id);
                        agente.CodigoInterno = TipoAgenteDb.Proveedor + "-" + id;
                    }
                    else if (agente.Tipo == TipoAgenteDb.Deposito)
                    {
                        var id = secuencias.FirstOrDefault();
                        secuencias.Remove(id);
                        agente.CodigoInterno = TipoAgenteDb.Deposito + "-" + id;
                    }

                    var key = $"{agente.Tipo}.{agente.Codigo}";

                    if (!agente.IdLocalidad.HasValue && !string.IsNullOrEmpty(agente.MunicipioId) && !string.IsNullOrEmpty(agente.SubdivisionId))
                    {
                        agente.IdLocalidad = serviceContext.GetLocalidadId(agente.MunicipioId, agente.SubdivisionId)?.Id;
                    }

                    newAgentes[key] = agente;

                    foreach (var det in serviceContext.ClienteDiasValidezVentanas.Where(x => x.Empresa == agente.Empresa).ToList())
                    {
                        newClientesiasValidezVentana.Add(new ClienteDiasValidezVentana()
                        {
                            Empresa = det.Empresa,
                            Cliente = agente.CodigoInterno,
                            VentanaLiberacion = det.VentanaLiberacion,
                            CantidadDiasValidezLiberacion = 0,
                            FechaAlta = DateTime.Now
                        });

                    }
                }
                else
                {
                    var entity = Map(agente, model);
                    context.UpdAgentes.Add(GetAgenteEntity(entity));
                }
            }

            context.NewAgentes = newAgentes.Values;
            context.NewClientesiasValidezVentana = newClientesiasValidezVentana;

            return context;
        }

        public virtual List<int> GetNewIdCliente(int count)
        {
            var connection = _context.Database.GetDbConnection();
            var tran = _context.Database.CurrentTransaction?.GetDbTransaction();
            return _dapper.GetNextSequenceValues<int>(connection, Secuencias.S_AGENTE_PROVEEDOR, count, tran).ToList();
        }

        public virtual async Task AddAgentes(AgenteBulkOperationContext context, CancellationToken cancelToken = default)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);

                using (var tran = connection.BeginTransaction())
                {
                    await BulkInsertAgentes(connection, tran, context.NewAgentes, context.NewClientesiasValidezVentana);
                    await BulkUpdateAgentes(connection, tran, context.UpdAgentes);

                    tran.Commit();
                }
            }
        }

        public virtual async Task BulkUpdateAgentes(DbConnection connection, DbTransaction tran, List<object> agentes)
        {
            var sqlUpdAgentes = @"
                        UPDATE T_CLIENTE SET                 
                            CD_CATEGORIA = :Categoria,
                            CD_CEP = :CodigoPostal,
                            CD_CGC_CLIENTE = :NumeroFiscal,
                            CD_CLIENTE_EN_CONSOLIDADO = :ClienteConsolidado,              
                            CD_EMPRESA_CONSOLIDADA = :EmpresaConsolidada,
                            CD_FORNECEDOR = :Fornecedor,
                            CD_GLN = :NumeroLocalizacionGlobal,
                            CD_PUNTO_ENTREGA = :PuntoDeEntrega,
                            CD_ROTA = :Ruta,
                            CD_SITUACAO = :Situacion,
                            DS_ANEXO1 = :Anexo1,
                            DS_ANEXO2 = :Anexo2,
                            DS_ANEXO3 = :Anexo3,
                            DS_ANEXO4 = :Anexo4,
                            DS_BAIRRO = :Barrio,
                            DS_CLIENTE = :Descripcion,
                            DS_ENDERECO = :Direccion,
                            DT_ALTERACAO = :FechaModificacion,
                            FL_ACEPTA_DEVOLUCION = :AceptaDevolucion,
                            ID_CLIENTE_FILIAL = :IdClienteFilial,
                            NU_DDD = :NroDDD,
                            NU_DV_CLIENTE = :NuDvCliente,
                            NU_FAX = :TelefonoSecundario,
                            NU_INSCRICAO = :OtroDatoFiscal,
                            NU_PRIOR_CARGA = :OrdenDeCarga,
                            NU_TELEFONE = :TelefonoPrincipal,
                            TP_ATIVIDADE = :TipoActividad,
                            CD_GRUPO_CONSULTA = :GrupoConsulta,
                            ID_LOCALIDAD = :IdLocalidad,
                            ND_TIPO_FISCAL = :TipoFiscal,
                            VL_PORCENTAJE_VIDA_UTIL = :ValorManejoVidaUtil,
                            DS_EMAIL = :Email
                        WHERE CD_CLIENTE = :CodigoInterno AND CD_EMPRESA = :Empresa";

            await _dapper.ExecuteAsync(connection, sqlUpdAgentes, agentes, transaction: tran);

            var sqlUpdContactos = "UPDATE T_CONTACTO SET DS_EMAIL = :Email, DT_UPDROW = :FechaModificacion, NU_TRANSACCION = :Transaccion WHERE CD_CLIENTE = :CodigoInterno AND CD_EMPRESA = :Empresa ";

            await _dapper.ExecuteAsync(connection, sqlUpdContactos, agentes, transaction: tran);

            var sqlNewContactos = @"INSERT INTO T_CONTACTO (NU_CONTACTO, CD_EMPRESA, CD_CLIENTE, DS_CONTACTO, NM_CONTACTO, NU_TELEFONO, DS_EMAIL, DT_ADDROW, DT_UPDROW, NU_TRANSACCION) 
                        SELECT NULL, cli.CD_EMPRESA, cli.CD_CLIENTE, :Descripcion, :Descripcion, :TelefonoPrincipal, :Email, :FechaModificacion, :FechaModificacion, :Transaccion 
                        FROM T_CLIENTE cli
                        LEFT JOIN T_CONTACTO con ON cli.CD_CLIENTE = con.CD_CLIENTE AND cli.CD_EMPRESA = con.CD_EMPRESA 
                        WHERE cli.CD_CLIENTE = :CodigoInterno AND cli.CD_EMPRESA = :Empresa AND con.CD_CLIENTE IS NULL ";

            await _dapper.ExecuteAsync(connection, sqlNewContactos, agentes, transaction: tran);
        }

        public virtual async Task BulkInsertAgentes(DbConnection connection, DbTransaction tran, IEnumerable<Agente> agentes, List<ClienteDiasValidezVentana> newClientesiasValidezVentana)
        {
            List<object> agenteEntities = new List<object>();
            List<object> contactoEntities = new List<object>();

            foreach (var agente in agentes)
            {

                agente.SincronizacionRealizadaId = "N";

                agenteEntities.Add(GetAgenteEntity(agente));
                contactoEntities.Add(GetContatoEntity(agente));

            }

            await BulkInsertAgentes(connection, tran, agenteEntities);
            await BulkInsertContactos(connection, tran, contactoEntities);
            await BulkInsertClienteVentanaValidezVentana(connection, tran, newClientesiasValidezVentana);
        }

        public virtual async Task BulkInsertClienteVentanaValidezVentana(DbConnection connection, DbTransaction tran, List<ClienteDiasValidezVentana> newClientesiasValidezVentana)
        {
            var sql = @"INSERT INTO T_CLIENTE_DIASVALIDEZ_VENTANA (CD_CLIENTE,CD_EMPRESA,QT_DIAS_VALIDADE_LIBERACION,CD_VENTANA_LIBERACION,DT_ADDROW)
                        values (:Cliente,:Empresa,:CantidadDiasValidezLiberacion,:VentanaLiberacion,:FechaAlta)";

            await _dapper.ExecuteAsync(connection, sql, newClientesiasValidezVentana, transaction: tran);
        }

        public virtual async Task BulkInsertAgentes(DbConnection connection, DbTransaction tran, List<object> agentes)
        {
            string sql = @"INSERT INTO T_CLIENTE(
                            CD_AGENTE,
                            CD_CATEGORIA,
                            CD_CEP,
                            CD_CGC_CLIENTE,
                            CD_CLIENTE,
                            CD_CLIENTE_EN_CONSOLIDADO,
                            CD_EMPRESA,
                            CD_EMPRESA_CONSOLIDADA,
                            CD_FORNECEDOR,
                            CD_GLN,
                            CD_PUNTO_ENTREGA,
                            CD_ROTA,
                            CD_SITUACAO,
                            DS_ANEXO1,
                            DS_ANEXO2,
                            DS_ANEXO3,
                            DS_ANEXO4,
                            DS_BAIRRO,
                            DS_CLIENTE,
                            DS_ENDERECO,
                            DT_CADASTRAMENTO,
                            DT_SITUACAO,
                            FL_ACEPTA_DEVOLUCION,
                            ID_CLIENTE_FILIAL,
                            NU_DDD,
                            NU_DV_CLIENTE,
                            NU_FAX,
                            NU_INSCRICAO,
                            NU_PRIOR_CARGA,
                            NU_TELEFONE,
                            TP_AGENTE,
                            TP_ATIVIDADE,
                            CD_GRUPO_CONSULTA,
                            ID_LOCALIDAD,
                            ND_TIPO_FISCAL,
                            VL_PORCENTAJE_VIDA_UTIL,
                            FL_SYNC_REALIZADA,
                            DS_EMAIL) 
                            values (
                            :Codigo,
                            :Categoria,
                            :CodigoPostal,
                            :NumeroFiscal,
                            :CodigoInterno,
                            :ClienteConsolidado,
                            :Empresa,
                            :EmpresaConsolidada,
                            :Fornecedor,
                            :NumeroLocalizacionGlobal,
                            :PuntoDeEntrega,
                            :Ruta,
                            :Situacion,
                            :Anexo1,
                            :Anexo2,
                            :Anexo3,
                            :Anexo4,
                            :Barrio,
                            :Descripcion,
                            :Direccion,
                            :FechaAlta,
                            :FechaSituacion,
                            :AceptaDevolucion,
                            :IdClienteFilial,
                            :NroDDD,
                            :NuDvCliente,
                            :TelefonoSecundario,
                            :OtroDatoFiscal,
                            :OrdenDeCarga,
                            :TelefonoPrincipal,
                            :Tipo,
                            :TipoActividad,
                            :GrupoConsulta,
                            :IdLocalidad,
                            :TipoFiscal,
                            :ValorManejoVidaUtil,
                            :SincronizacionRealizada,
                            :Email)";

            await _dapper.ExecuteAsync(connection, sql, agentes, transaction: tran);
        }

        public virtual object GetContatoEntity(Agente agente)
        {
            return new
            {
                Empresa = agente.Empresa,
                Cliente = agente.CodigoInterno,
                Descripcion = agente.Descripcion,
                Nombre = agente.Codigo,
                Telefono = agente.TelefonoPrincipal,
                Email = agente.Email,
                FechaAlta = DateTime.Now,
                FechaModificacion = DateTime.Now,
                Tipo = agente.Tipo,
                Codigo = agente.Codigo,
                Transaccion = agente.Transaccion,
            };
        }

        public static object GetAgenteEntity(Agente agente)
        {
            return new
            {
                Codigo = agente.Codigo,
                Categoria = agente.Categoria,
                CodigoPostal = agente.CodigoPostal,
                NumeroFiscal = agente.NumeroFiscal,
                CodigoInterno = agente.CodigoInterno,
                ClienteConsolidado = agente.ClienteConsolidado,
                Empresa = agente.Empresa,
                EmpresaConsolidada = agente.EmpresaConsolidada,
                Fornecedor = agente.Fornecedor,
                NumeroLocalizacionGlobal = agente.NumeroLocalizacionGlobal,
                GrupoConsulta = agente.GrupoConsulta,
                PuntoDeEntrega = agente.PuntoDeEntrega,
                Ruta = agente.RutaId,
                Situacion = agente.EstadoId ?? SituacionDb.Activo,
                Anexo1 = agente.Anexo1,
                Anexo2 = agente.Anexo2,
                Anexo3 = agente.Anexo3,
                Anexo4 = agente.Anexo4,
                Barrio = agente.Barrio,
                Descripcion = agente.Descripcion,
                Direccion = agente.Direccion,
                FechaAlta = agente.FechaAlta ?? DateTime.Now,
                FechaModificacion = agente.FechaModificacion ?? DateTime.Now,
                FechaSituacion = agente.FechaSituacion ?? DateTime.Now,
                AceptaDevolucion = agente.AceptaDevolucionId,
                IdClienteFilial = agente.IdClienteFilial,
                IdLocalidad = agente.IdLocalidad,
                TipoFiscal = agente.TipoFiscalId,
                NroDDD = agente.NuDDD,
                NuDvCliente = agente.NuDvCliente,
                TelefonoSecundario = agente.TelefonoSecundario,
                OtroDatoFiscal = agente.OtroDatoFiscal,
                OrdenDeCarga = agente.OrdenDeCarga,
                TelefonoPrincipal = agente.TelefonoPrincipal,
                Tipo = agente.Tipo,
                TipoActividad = agente.TipoActividad,
                ValorManejoVidaUtil = agente.ValorManejoVidaUtil,
                SincronizacionRealizada = agente.SincronizacionRealizadaId,
                Email = agente.Email,
                Transaccion = agente.Transaccion,
            };
        }

        public virtual async Task BulkInsertContactos(DbConnection connection, DbTransaction tran, List<object> contactos)
        {
            var sql = @"INSERT INTO T_CONTACTO (NU_CONTACTO, CD_EMPRESA, CD_CLIENTE, DS_CONTACTO, NM_CONTACTO, NU_TELEFONO, DS_EMAIL, DT_ADDROW, DT_UPDROW, NU_TRANSACCION) 
                        SELECT NULL, :Empresa, CD_CLIENTE, :Descripcion, :Nombre, :Telefono, :Email, :FechaAlta, :FechaModificacion, :Transaccion 
                        FROM T_CLIENTE 
                        WHERE TP_AGENTE = :Tipo AND CD_AGENTE = :Codigo AND CD_EMPRESA = :Empresa";

            await _dapper.ExecuteAsync(connection, sql, contactos, transaction: tran);
        }

        public virtual async Task<Agente> GetAgenteOrNull(int empresa, string codigo, string tipo)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync();

                Agente model = GetAgente(connection, new Agente()
                {
                    Empresa = empresa,
                    Codigo = codigo,
                    Tipo = tipo
                });

                Fill(model);

                return model;
            }
        }

        public static void Fill(Agente model)
        {
            if (model != null)
            {
                model.TipoFiscal = new DominioDetalle() { Id = model.TipoFiscalId };

                if (model.RutaId.HasValue)
                {
                    model.Ruta = new Ruta() { Id = model.RutaId.Value };
                }

                if (model.EstadoId.HasValue)
                {
                    int estado = (int)model.EstadoId.Value;
                    if (Enum.IsDefined(typeof(EstadoAgente), estado))
                        model.Estado = (EstadoAgente)estado;
                    else
                        model.Estado = EstadoAgente.Unknown;
                }
            }
        }

        public virtual async Task<Agente> GetAgenteOrNull(int empresa, string codigoInterno)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync();

                Agente model = GetAgente(connection, new Agente()
                {
                    Empresa = empresa,
                    CodigoInterno = codigoInterno
                });

                Fill(model);

                return model;
            }
        }

        public static string GetSqlSelectAgente()
        {
            return @"SELECT 
                        A.CD_AGENTE as Codigo,
                        A.CD_CATEGORIA as Categoria,
                        A.CD_CEP as CodigoPostal,
                        A.CD_CGC_CLIENTE as NumeroFiscal,
                        A.CD_CLIENTE as CodigoInterno,
                        A.CD_CLIENTE_EN_CONSOLIDADO as ClienteConsolidado,
                        A.CD_EMPRESA as Empresa,
                        A.CD_EMPRESA_CONSOLIDADA as EmpresaConsolidada,
                        A.CD_FORNECEDOR as Fornecedor,
                        A.CD_GLN as NumeroLocalizacionGlobal,
                        A.CD_PUNTO_ENTREGA as PuntoDeEntrega,
                        A.CD_ROTA as RutaId,
                        A.CD_SITUACAO as EstadoId,
                        A.DS_ANEXO1 as Anexo1,
                        A.DS_ANEXO2 as Anexo2,
                        A.DS_ANEXO3 as Anexo3,
                        A.DS_ANEXO4 as Anexo4,
                        A.DS_BAIRRO as Barrio,
                        A.DS_CLIENTE as Descripcion,
                        A.DS_ENDERECO as Direccion,
                        A.DT_ALTERACAO as FechaModificacion,
                        A.DT_CADASTRAMENTO as FechaAlta,
                        A.DT_SITUACAO as FechaSituacion,
                        A.FL_ACEPTA_DEVOLUCION as AceptaDevolucionId,
                        A.ID_CLIENTE_FILIAL as IdClienteFilial,                    
                        A.NU_DDD as NuDDD,
                        A.NU_DV_CLIENTE as NuDvCliente,
                        A.NU_FAX as TelefonoSecundario,
                        A.NU_INSCRICAO as OtroDatoFiscal,
                        A.NU_PRIOR_CARGA as OrdenDeCarga,
                        A.NU_TELEFONE as TelefonoPrincipal,
                        A.TP_AGENTE as Tipo,
                        A.TP_ATIVIDADE as TipoActividad,
                        A.ID_LOCALIDAD as IdLocalidad,
                        A.CD_GRUPO_CONSULTA as GrupoConsulta, 
                        A.ND_TIPO_FISCAL as TipoFiscalId,
                        A.VL_PORCENTAJE_VIDA_UTIL as ValorManejoVidaUtil,
                        A.FL_SYNC_REALIZADA as SincronizacionRealizadaId,
                        A.DS_EMAIL as Email 
                    FROM T_CLIENTE A ";
        }

        public virtual Agente GetAgente(DbConnection connection, Agente model)
        {
            string sql = GetSqlSelectAgente() +
                @"WHERE CD_EMPRESA = :cdEmpresa ";

            if (string.IsNullOrEmpty(model.CodigoInterno))
            {
                sql += @"AND A.CD_AGENTE = :codigo AND A.TP_AGENTE = :tipo";
            }
            else
            {
                sql += @"AND A.CD_CLIENTE = :codigoInterno";
            }

            var query = _dapper.Query<Agente>(connection, sql, param: new
            {
                codigoInterno = model.CodigoInterno,
                codigo = model.Codigo,
                cdEmpresa = model.Empresa,
                tipo = model.Tipo
            }, commandType: CommandType.Text);

            return query.FirstOrDefault();
        }

        public virtual Agente Map(Agente agente, Agente model = null)
        {
            agente.AceptaDevolucionId = agente.AceptaDevolucionId ?? model?.AceptaDevolucionId;
            agente.Anexo1 = agente.Anexo1 ?? model?.Anexo1;
            agente.Anexo2 = agente.Anexo2 ?? model?.Anexo2;
            agente.Anexo3 = agente.Anexo3 ?? model?.Anexo3;
            agente.Anexo4 = agente.Anexo4 ?? model?.Anexo4;
            agente.Barrio = agente.Barrio ?? model?.Barrio;
            agente.Categoria = agente.Categoria ?? model?.Categoria;
            agente.ClienteConsolidado = agente.ClienteConsolidado ?? model?.ClienteConsolidado;
            agente.Codigo = agente.Codigo ?? model?.Codigo;
            agente.CodigoInterno = agente.CodigoInterno ?? model?.CodigoInterno;
            agente.CodigoPostal = agente.CodigoPostal ?? model?.CodigoPostal;
            agente.Descripcion = agente.Descripcion ?? model?.Descripcion;
            agente.Direccion = agente.Direccion ?? model?.Direccion;
            agente.EmpresaConsolidada = agente.EmpresaConsolidada ?? model?.EmpresaConsolidada;
            agente.EstadoId = agente.EstadoId ?? model?.EstadoId;
            agente.Fornecedor = agente.Fornecedor ?? model?.Fornecedor;
            agente.GrupoConsulta = agente.GrupoConsulta ?? model?.GrupoConsulta;
            agente.IdClienteFilial = agente.IdClienteFilial ?? model?.IdClienteFilial;
            agente.IdLocalidad = agente.IdLocalidad ?? model?.IdLocalidad;
            agente.NuDDD = agente.NuDDD ?? model?.NuDDD;
            agente.NuDvCliente = agente.NuDvCliente ?? model?.NuDvCliente;
            agente.NumeroFiscal = agente.NumeroFiscal ?? model?.NumeroFiscal;
            agente.NumeroLocalizacionGlobal = agente.NumeroLocalizacionGlobal ?? model?.NumeroLocalizacionGlobal;
            agente.OrdenDeCarga = agente.OrdenDeCarga ?? model?.OrdenDeCarga;
            agente.OtroDatoFiscal = agente.OtroDatoFiscal ?? model?.OtroDatoFiscal;
            agente.PuntoDeEntrega = agente.PuntoDeEntrega ?? model?.PuntoDeEntrega;
            agente.SincronizacionRealizadaId = agente.SincronizacionRealizadaId ?? model?.SincronizacionRealizadaId;
            agente.TelefonoPrincipal = agente.TelefonoPrincipal ?? model?.TelefonoPrincipal;
            agente.TelefonoSecundario = agente.TelefonoSecundario ?? model?.TelefonoSecundario;
            agente.Tipo = agente.Tipo ?? model?.Tipo;
            agente.TipoActividad = agente.TipoActividad ?? model?.TipoActividad;
            agente.TipoFiscalId = agente.TipoFiscalId ?? model?.TipoFiscalId;
            agente.ValorManejoVidaUtil = agente.ValorManejoVidaUtil ?? model?.ValorManejoVidaUtil;
            agente.Email = agente.Email ?? model?.Email;
            agente.Transaccion = agente.Transaccion ?? model?.Transaccion;

            return agente;
        }

        public virtual async Task<Dictionary<string, Agente>> GetAgentesEgreso(int cdCamion)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync();

                string sql = GetSqlSelectAgente() +
                          @"INNER JOIN T_CLIENTE_CAMION CA ON A.CD_CLIENTE = CA.CD_CLIENTE AND A.CD_EMPRESA = CA.CD_EMPRESA 
                            WHERE CA.CD_CAMION = :cdCamion 
                            GROUP BY 
                                A.CD_AGENTE,
                                A.CD_CATEGORIA,
                                A.CD_CEP,
                                A.CD_CGC_CLIENTE,
                                A.CD_CLIENTE,
                                A.CD_CLIENTE_EN_CONSOLIDADO,
                                A.CD_EMPRESA,
                                A.CD_EMPRESA_CONSOLIDADA,
                                A.CD_FORNECEDOR,
                                A.CD_GLN,
                                A.CD_PUNTO_ENTREGA,
                                A.CD_ROTA,
                                A.CD_SITUACAO,
                                A.DS_ANEXO1,
                                A.DS_ANEXO2,
                                A.DS_ANEXO3,
                                A.DS_ANEXO4,
                                A.DS_BAIRRO,
                                A.DS_CLIENTE,
                                A.DS_ENDERECO,
                                A.DT_ALTERACAO,
                                A.DT_CADASTRAMENTO,
                                A.DT_SITUACAO,
                                A.FL_ACEPTA_DEVOLUCION,
                                A.ID_CLIENTE_FILIAL,                    
                                A.NU_DDD,
                                A.NU_DV_CLIENTE,
                                A.NU_FAX,
                                A.NU_INSCRICAO,
                                A.NU_PRIOR_CARGA,
                                A.NU_TELEFONE,
                                A.TP_AGENTE,
                                A.TP_ATIVIDADE,
                                A.ID_LOCALIDAD,
                                A.CD_GRUPO_CONSULTA, 
                                A.ND_TIPO_FISCAL,
                                A.VL_PORCENTAJE_VIDA_UTIL,
                                A.FL_SYNC_REALIZADA";

                return _dapper.Query<Agente>(connection, sql, param: new { cdCamion = cdCamion }, commandType: CommandType.Text)
                    .ToDictionary(a => $"{a.CodigoInterno}{a.Empresa}", a => a);
            }
        }

        #endregion
    }
}
