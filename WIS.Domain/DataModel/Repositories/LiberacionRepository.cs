using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Extensions;
using WIS.Domain.Liberacion;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class LiberacionRepository
    {
        protected readonly WISDB _context;
        protected readonly string _application;
        protected readonly int _userId;
        protected readonly LiberacionMapper _mapper;
        protected readonly IDapper _dapper;

        public LiberacionRepository(WISDB context, string application, int userId, IDapper dapper)
        {
            this._context = context;
            this._application = application;
            this._userId = userId;
            this._mapper = new LiberacionMapper();
            this._dapper = dapper;
        }

        #region Any

        public virtual bool AnyOnda(int cdEmpresa, short cdOnda)
        {
            return this._context.V_ONDA.AsNoTracking().Any(d => d.CD_EMPRESA == cdEmpresa && d.CD_ONDA == cdOnda);
        }

        public virtual bool AnyOnda(int cdEmpresa)
        {
            return this._context.V_ONDA.AsNoTracking().Any(d => d.CD_EMPRESA == cdEmpresa);
        }

        public virtual bool AnyReglaCliente(int nuRegla, int cdEmpresa, string cdCliente)
        {
            return this._context.T_REGLA_CLIENTES.AsNoTracking().Any(d => d.CD_EMPRESA == cdEmpresa && d.CD_CLIENTE == cdCliente && d.NU_REGLA == nuRegla);
        }

        public virtual bool AnyOrden(short nuOrden, int? nuRegla)
        {
            var any = this._context.T_REGLA_LIBERACION.AsNoTracking().Any(d => d.NU_ORDEN == nuOrden);

            if (nuRegla != null)
                if (this._context.T_REGLA_LIBERACION.AsNoTracking().First(s => s.NU_REGLA == (int)nuRegla).NU_ORDEN == nuOrden)
                    any = false;

            return any;
        }

        public virtual bool AnyPedidosParaPrep(int cdEmpresa, short cdOnda, string predio)
        {
            try
            {
                if (!string.IsNullOrEmpty(predio))
                {
                    return _context.V_PRE050_PEND_LIB.AsNoTracking().Any(x => x.CD_EMPRESA == cdEmpresa && x.CD_ONDA == cdOnda && x.NU_PREDIO == predio);
                }
                else
                {
                    return _context.V_PRE050_PEND_LIB.AsNoTracking().Any(x => x.CD_EMPRESA == cdEmpresa && x.CD_ONDA == cdOnda && x.NU_PREDIO == null);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual bool TipoAgenteCompatible(int nuRegla, string tpAgente)
        {
            return this._context.V_PRE250_CLIENTES_SELECIONADOS.AsNoTracking().Any(d => d.NU_REGLA == nuRegla && d.TP_AGENTE == tpAgente);
        }

        #endregion

        #region Get

        public virtual List<Onda> GetOndaByCdDsOnda(string key, int cdEmpresa)
        {
            var res = new List<Onda>();
            var query = this._context.V_ONDA.AsNoTracking()
                .Where(s => s.CD_SITUACAO == SituacionDb.Activo && s.CD_EMPRESA == cdEmpresa);


            foreach (var o in query.ToList().Where(s => (s.CD_ONDA.ToString() == key || s.DS_ONDA.ToUpper().Contains(key.ToUpper()))))
                res.Add(_mapper.MapOndaToObject(o));

            return res;
        }

        public virtual ReglaCliente GetReglaCliente(int nuRegla, int cdEmpresa, string cdCliente)
        {
            return this._mapper.MapReglaClienteToObject(this._context.T_REGLA_CLIENTES.AsNoTracking().FirstOrDefault(d => d.CD_EMPRESA == cdEmpresa && d.CD_CLIENTE == cdCliente && d.NU_REGLA == nuRegla));
        }

        public virtual ReglaLiberacion GetReglaLiberacion(int nuRegla, bool addNavegables)
        {
            var r = this._context.T_REGLA_LIBERACION
                .Include("T_REGLA_TIPO_PEDIDO")
                .Include("T_REGLA_TIPO_EXPEDICION")
                .Include("T_REGLA_CONDICION_LIBERACION")
                .Include("T_REGLA_CLIENTES")
                .AsNoTracking()
                .FirstOrDefault(s => s.NU_REGLA == nuRegla);

            return r != null ? _mapper.MapReglaLiberacionToObject(r, addNavegables) : null;
        }

        public virtual ReglaLiberacion GetReglaLiberacion(int nuRegla)
        {
            var r = this._context.T_REGLA_LIBERACION.AsNoTracking().FirstOrDefault(s => s.NU_REGLA == nuRegla);

            return r != null ? _mapper.MapReglaLiberacionToObject(r, false) : null;
        }

        public virtual List<ReglaLiberacion> GetReglasPendienteLiberar()
        {
            var res = new List<ReglaLiberacion>();
            var nuReglas = _context.V_LIBERACION_AUTOMATICA_PEND.AsNoTracking().OrderBy(o => o.NU_ORDEN).Select(s => s.NU_REGLA).ToList();
            foreach (var nuRegla in nuReglas)
            {
                res.Add(GetReglaLiberacion(nuRegla, true));
            }
            return res;
        }

        public virtual List<PedidoPendLib> GetPedidosLiberar(ReglaLiberacion regla)
        {
            var res = new List<PedidoPendLib>();
            var pend = _context.V_PRE050_PEND_LIB
                .AsNoTracking()
                .AsEnumerable()
                .Where(s => (s.NU_PREDIO == regla.NuPredio || regla.NuPredio == null)
                    && s.CD_EMPRESA == regla.CdEmpresa
                    && s.CD_ONDA == regla.CdOnda
                    && (s.TP_AGENTE == regla.TpAgente || regla.TpAgente == null)
                    && (s.NU_ULT_PREPARACION == null || !regla.FlSoloPedidosNuevos));

            pend = pend.Where(s => (regla.LstReglaCliente.Any(x => s.CD_CLIENTE == x.Cliente && s.CD_EMPRESA == x.Empesa) || !regla.LstReglaCliente.Any()));
            pend = pend.Where(s => (regla.LstReglaCondicionLiberacion.Any(x => s.CD_CONDICION_LIBERACION == x.cdCondicionLiberacion)) || !regla.LstReglaCondicionLiberacion.Any());
            pend = pend.Where(s => (regla.LstReglaTipoExpedicion.Any(x => s.TP_EXPEDICION == x.tpExpedicion) || !regla.LstReglaTipoExpedicion.Any()));
            pend = pend.Where(s => (regla.LstReglaTipoPedido.Any(x => s.TP_PEDIDO == x.tpPedido) || !regla.LstReglaTipoPedido.Any()));

            foreach (var p in pend.ToList())
                res.Add(_mapper.MapPedidoPendLibToObject(p));

            return res;
        }

        public virtual List<ReglaCliente> GetReglaClientes(int nuRegla)
        {
            List<ReglaCliente> res = new List<ReglaCliente>();

            foreach (var p in _context.T_REGLA_CLIENTES.AsNoTracking().Where(s => s.NU_REGLA == nuRegla))
                res.Add(_mapper.MapReglaClienteToObject(p));

            return res;
        }

        public virtual List<CondicionLiberacion> GetCondicionLiberaciones()
        {
            List<CondicionLiberacion> res = new List<CondicionLiberacion>();

            foreach (var cond in this._context.T_CONDICION_LIBERACION.AsNoTracking().ToArray())
            {
                res.Add(_mapper.MapCondicionLiberacionToObject(cond));
            }
            ;

            return res;
        }

        public virtual List<string> GetCdCondLiberacionRegla(ICollection<ReglaCondicionLiberacion> LstReglaCondicionLiberacion)
        {
            List<string> res = new List<string>();

            foreach (var cond in LstReglaCondicionLiberacion)
            {
                res.Add(cond.cdCondicionLiberacion);
            }
            ;

            return res;
        }

        public virtual List<string> GetCdTpExpedicionRegla(ICollection<ReglaTipoExpedicion> LstReglaTipoExpedicion)
        {
            List<string> res = new List<string>();

            foreach (var tp in LstReglaTipoExpedicion)
            {
                res.Add(tp.tpExpedicion);
            }
            ;

            return res;
        }

        public virtual List<string> GetCdTpPedidoRegla(ICollection<ReglaTipoPedido> LstReglaTipoPedido)
        {
            List<string> res = new List<string>();

            foreach (var tp in LstReglaTipoPedido)
            {
                res.Add(tp.tpPedido);
            }
            ;

            return res;
        }

        public virtual CondicionLiberacion GetCondicion(string condicion)
        {
            return _mapper.MapCondicionLiberacionToObject(this._context.T_CONDICION_LIBERACION.AsNoTracking().FirstOrDefault(x => x.CD_CONDICION_LIBERACION == condicion));
        }

        public virtual decimal ObtenerMenorValorVidaUtil(int nuRegla)
        {
            decimal result = 0;
            if (this._context.V_PRE250_CLIENTES_SELECIONADOS.AsNoTracking().Any(s => s.NU_REGLA == nuRegla))
                result = this._context.V_PRE250_CLIENTES_SELECIONADOS.AsNoTracking().Where(s => s.NU_REGLA == nuRegla).Min(r => r.VL_PORCENTAJE_VIDA_UTIL);
            return result;
        }

        public virtual int GetNextNuReglaLiberacion()
        {
            return this._context.GetNextSequenceValueInt(_dapper, Secuencias.S_NU_REGLA_LIBERACION);
        }

        public virtual List<string> VariosGruposValidacion(short cdOnda, int cdEmpresa)
        {
            return (from gal in _context.V_PRE050_PEND_LIB
                    where gal.CD_ONDA == cdOnda && gal.CD_EMPRESA == cdEmpresa && gal.TP_PEDIDO != "WISMAC"
                    group gal by new { gal.CD_GRUPO_EXPEDICION } into x
                    select x.Key.CD_GRUPO_EXPEDICION).ToList();
        }

        public virtual void SeleccionPedidoCompatible(short? cdOnda, int cdEmpresa, out bool valido, out string msg)
        {
            msg = string.Empty;

            var pedCom = _context.V_PRE051_PEDIDOS_COMPATIBLES.AsNoTracking().Where(x => x.CD_EMPRESA == cdEmpresa && x.CD_EMPRESA_ESPECIFICADO == cdEmpresa && x.CD_ONDA == cdOnda && x.CD_ONDA_ESPECIFICADO == cdOnda).FirstOrDefault();

            if (pedCom != null)
            {
                valido = false;
                msg = String.Format("No se puede liberar, producto en modalidad AUTO y con lote especificado, Prod: {0} (AUTO) Ped: {1}  Cli: {2} C/Lote Ped: {3} Cli: {4} Separe los pedidos en dos liberaciones", pedCom.CD_PRODUTO, pedCom.NU_PEDIDO_AUTO, pedCom.CD_CLIENTE_AUTO, pedCom.NU_PEDIDO_ESPE, pedCom.CD_CLIENTE_ESPE);
            }
            else
            {
                valido = true;
            }
        }

        #endregion

        #region Add

        public virtual void AddReglaLiberacion(ReglaLiberacion obj)
        {
            T_REGLA_LIBERACION entity = this._mapper.MapReglaLiberacionToEntity(obj);
            entity.DT_ADDROW = DateTime.Now;
            this._context.T_REGLA_LIBERACION.Add(entity);
        }

        public virtual void AddReglaCondicionLiberacion(ReglaCondicionLiberacion obj)
        {
            T_REGLA_CONDICION_LIBERACION entity = this._mapper.MapReglaCondicionLiberacionToEntity(obj);
            entity.DT_ADDROW = DateTime.Now;
            this._context.T_REGLA_CONDICION_LIBERACION.Add(entity);
        }

        public virtual void AddReglaTipoExpedicion(ReglaTipoExpedicion obj)
        {
            T_REGLA_TIPO_EXPEDICION entity = this._mapper.MapReglaTipoExpedicionToEntity(obj);
            entity.DT_ADDROW = DateTime.Now;
            this._context.T_REGLA_TIPO_EXPEDICION.Add(entity);
        }

        public virtual void AddReglaTipoPedido(ReglaTipoPedido obj)
        {
            T_REGLA_TIPO_PEDIDO entity = this._mapper.MapReglaTipoPedidoToEntity(obj);
            entity.DT_ADDROW = DateTime.Now;
            this._context.T_REGLA_TIPO_PEDIDO.Add(entity);
        }

        public virtual void AddReglaCliente(ReglaCliente obj)
        {
            T_REGLA_CLIENTES entity = this._mapper.MapReglaClienteToEntity(obj);
            entity.DT_ADDROW = DateTime.Now;
            this._context.T_REGLA_CLIENTES.Add(entity);
        }

        public virtual void SetCondicionesLiberacion(short cdOnda, int empresa, List<ReglaCondicionLiberacion> colCondLib)
        {
            if (colCondLib.Any())
            {
                foreach (var cond in colCondLib)
                {
                    var condicion = _context.T_TEMP_CONDICION_LIBERACION.AsNoTracking().FirstOrDefault(x => x.CD_CONDICION_LIBERACION == cond.cdCondicionLiberacion && x.CD_ONDA == cdOnda && x.CD_EMPRESA == empresa);
                    if (condicion == null)
                    {
                        var nuevaCondicion = new T_TEMP_CONDICION_LIBERACION();
                        nuevaCondicion.CD_ONDA = cdOnda;
                        nuevaCondicion.CD_EMPRESA = empresa;
                        nuevaCondicion.CD_CONDICION_LIBERACION = cond.cdCondicionLiberacion;
                        nuevaCondicion.NU_TEMP_CONDICION_LIBERACION = _context.GetNextSequenceValueLong(_dapper, Secuencias.S_NU_TEMP_CONDICION_LIB);
                        _context.T_TEMP_CONDICION_LIBERACION.Add(nuevaCondicion);
                    }
                }
            }
            else
            {
                var condicion = _context.T_TEMP_CONDICION_LIBERACION.AsNoTracking().FirstOrDefault(x => x.CD_CONDICION_LIBERACION == "WIS-SC" && x.CD_ONDA == cdOnda && x.CD_EMPRESA == empresa);
                if (condicion == null)
                {
                    var nuevaCondicion = new T_TEMP_CONDICION_LIBERACION();
                    nuevaCondicion.CD_ONDA = cdOnda;
                    nuevaCondicion.CD_EMPRESA = empresa;
                    nuevaCondicion.CD_CONDICION_LIBERACION = "WIS-SC";
                    nuevaCondicion.NU_TEMP_CONDICION_LIBERACION = _context.GetNextSequenceValueLong(_dapper, Secuencias.S_NU_TEMP_CONDICION_LIB);
                    _context.T_TEMP_CONDICION_LIBERACION.Add(nuevaCondicion);
                }
            }
            _context.SaveChanges();

        }
        #endregion

        #region Update

        public virtual void UpdateReglaLiberacion(ReglaLiberacion obj)
        {
            var entity = this._mapper.MapReglaLiberacionToEntity(obj);
            var current = _context.T_REGLA_LIBERACION.First(s => s.NU_REGLA == entity.NU_REGLA);

            entity.DT_ADDROW = current.DT_ADDROW;
            entity.DT_UPDROW = DateTime.Now;

            if (obj.DtUltimaEjecucion == null)
                entity.DT_ULTIMA_EJECUCION = current.DT_ULTIMA_EJECUCION;

            var attachedEntity = _context.T_REGLA_LIBERACION.Local
                .FirstOrDefault(w => w.NU_REGLA == entity.NU_REGLA);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_REGLA_LIBERACION.Attach(entity);
                _context.Entry<T_REGLA_LIBERACION>(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateReglaCliente(ReglaCliente obj)
        {
            T_REGLA_CLIENTES entity = this._mapper.MapReglaClienteToEntity(obj);

            entity.DT_UPDROW = DateTime.Now;

            T_REGLA_CLIENTES attachedEntity = _context.T_REGLA_CLIENTES.Local
                .FirstOrDefault(w => w.NU_REGLA == entity.NU_REGLA
                    && w.CD_CLIENTE == entity.CD_CLIENTE
                    && w.CD_EMPRESA == entity.CD_EMPRESA);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_REGLA_CLIENTES.Attach(entity);
                _context.Entry<T_REGLA_CLIENTES>(entity).State = EntityState.Modified;
            }
        }




        public virtual void AsignarCondicionesLiberacion(short cdOnda, int empresa, List<CondicionLiberacion> colCondicion)
        {

            this.DeleteCondicionesLiberacionTemporales(cdOnda, empresa);
            this._context.SaveChanges();

            if (colCondicion.Count == 0)
            {
                T_TEMP_CONDICION_LIBERACION nuevaCondicion = new T_TEMP_CONDICION_LIBERACION
                {
                    CD_ONDA = cdOnda,
                    CD_EMPRESA = empresa,
                    CD_CONDICION_LIBERACION = "WIS-SC"
                };

                this._context.T_TEMP_CONDICION_LIBERACION.Add(nuevaCondicion);
            }
            else
            {
                colCondicion.ForEach(condi =>

                    this._context.T_TEMP_CONDICION_LIBERACION.Add(new T_TEMP_CONDICION_LIBERACION
                    {
                        CD_ONDA = cdOnda,
                        CD_EMPRESA = empresa,
                        CD_CONDICION_LIBERACION = condi.Condicion
                    })

                );
            }

            this._context.SaveChanges(); //Agregado Mauro 2021-05-14, para concretar cambio en T_TEMP_CONDICION_LIBERACION y que las consultas posteriores den correcto
        }
        #endregion

        #region Delete

        public virtual void DeleteReglaTipoPedidoByRegla(int nuRegla)
        {
            _context.T_REGLA_TIPO_PEDIDO.RemoveRange(_context.T_REGLA_TIPO_PEDIDO.Where(s => s.NU_REGLA == nuRegla));
        }

        public virtual void DeleteReglaTipoExpedicionByRegla(int nuRegla)
        {
            _context.T_REGLA_TIPO_EXPEDICION.RemoveRange(_context.T_REGLA_TIPO_EXPEDICION.Where(s => s.NU_REGLA == nuRegla));
        }

        public virtual void DeleteReglaCondicionLiberacionByRegla(int nuRegla)
        {
            _context.T_REGLA_CONDICION_LIBERACION.RemoveRange(_context.T_REGLA_CONDICION_LIBERACION.Where(s => s.NU_REGLA == nuRegla));
        }

        public virtual void DeleteCondicionesLiberacionTemporales(short onda, int empresa)
        {
            foreach (var condi in this._context.T_TEMP_CONDICION_LIBERACION.Where(x => x.CD_ONDA == onda && x.CD_EMPRESA == empresa).ToList())
            {
                if (condi != null)
                    this._context.T_TEMP_CONDICION_LIBERACION.Remove(condi);
            }
        }

        public virtual void DeleteReglaCliente(List<ReglaCliente> reglas)
        {
            List<T_REGLA_CLIENTES> entities = new List<T_REGLA_CLIENTES>();
            foreach (var regla in reglas)
            {
                entities.Add(_mapper.MapReglaClienteToEntity(regla));
            }

            if (entities.Count > 0)
            {
                _context.T_REGLA_CLIENTES.AttachRange(entities);
                _context.T_REGLA_CLIENTES.RemoveRange(entities);
            }
        }

        #endregion
    }
}
