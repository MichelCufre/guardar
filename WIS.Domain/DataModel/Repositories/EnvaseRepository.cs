using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.Expedicion;
using WIS.Domain.General;
using WIS.Domain.StockEntities;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class EnvaseRepository
    {
        protected readonly WISDB _context;
        protected readonly string _cdAplicacion;
        protected readonly int _userId;
        protected readonly StockMapper _mapper;
        protected readonly AgenteMapper _agenteMapper;

        public EnvaseRepository(WISDB context, string cdAplicacion, int userId)
        {
            this._context = context;
            this._cdAplicacion = cdAplicacion;
            this._userId = userId;
            this._mapper = new StockMapper();
            this._agenteMapper = new AgenteMapper();
        }

        #region Any

        public virtual bool AnyEnvase(string ID_ENVASE, string ND_TP_ENVASE)
        {
            return this._context.T_STOCK_ENVASE
                .AsNoTracking()
                .Any(w => w.ID_ENVASE == ID_ENVASE && w.ND_TP_ENVASE == ND_TP_ENVASE
            );
        }

        public virtual bool AnyTipoEnvase(string ND_TP_ENVASE)
        {
            return _context.T_TIPO_CONTENEDOR
                .AsNoTracking()
                .Any(w => w.TP_CONTENEDOR == ND_TP_ENVASE && w.FL_RETORNABLE == "S");
        }

        #endregion

        #region Get   

        public virtual List<EnvaseCamion> GetEnvasesExpedicion(int camion)
        {
            return _context.T_DET_PICKING
                .Include("T_CONTENEDOR")
                .Join(
                    _context.T_CLIENTE,
                    cod => new { cod.CD_CLIENTE, cod.CD_EMPRESA },
                    cl => new { cl.CD_CLIENTE, cl.CD_EMPRESA },
                    (cod, cl) => new { Contenedor = cod.T_CONTENEDOR, Cliente = cl }
                )
                .Join(
                    _context.T_TIPO_CONTENEDOR,
                    tpc => tpc.Contenedor.TP_CONTENEDOR,
                    ttc => ttc.TP_CONTENEDOR,
                    (tpc, ttc) => new { tpc.Cliente, tpc.Contenedor, TipoContenedor = ttc }
                )
                .AsNoTracking()
                .Where(d => d.Contenedor.CD_CAMION == camion && d.TipoContenedor.FL_RETORNABLE == "S")
                .GroupBy(d => new { d.Contenedor.NU_CONTENEDOR, d.Contenedor.NU_PREPARACION, d.Contenedor.TP_CONTENEDOR, d.Contenedor.ID_EXTERNO_CONTENEDOR, d.Cliente.TP_AGENTE, d.Cliente.CD_AGENTE, d.Cliente.CD_EMPRESA })
                .Select(d => d.Key)
                .ToList()
                .Select(d => new EnvaseCamion
                {
                    CodigoAgente = d.CD_AGENTE,
                    TipoAgente = d.TP_AGENTE,
                    Tipo = d.TP_CONTENEDOR,
                    Contenedor = d.NU_CONTENEDOR,
                    IdExterno = d.ID_EXTERNO_CONTENEDOR,
                    Preparacion = d.NU_PREPARACION,
                    Empresa = d.CD_EMPRESA
                })
                .ToList();
        }

        public virtual Envase GetEnvase(string numero, string tipo)
        {
            return _mapper.Map(this._context.T_STOCK_ENVASE
                    .AsNoTracking()
                    .FirstOrDefault(w => w.ID_ENVASE == numero && w.ND_TP_ENVASE == tipo)
            );
        }

        public virtual List<Envase> GetEnvases()
        {
            return this._context.T_STOCK_ENVASE
                .AsNoTracking()
                .ToList()
                .Select(w => this._mapper.Map(w))
                .ToList();

        }

        public virtual List<Envase> GetEnvaseByKeysPartial(string valueSearch)
        {
            return this._context.T_STOCK_ENVASE
                .AsNoTracking()
                .Where(w => w.ID_ENVASE.ToLower().Contains(valueSearch.ToLower())
                         || w.ND_TP_ENVASE.ToLower().Contains(valueSearch.ToLower()))
                .ToList()
                .Select(w => this._mapper.Map(w))
                .ToList();

        }

        public virtual List<DominioDetalle> GetEstadosEnvases()
        {
            var _mapperDominio = new DominioMapper();

            return _context.T_DET_DOMINIO
                .AsNoTracking()
                .Where(w => w.CD_DOMINIO == "SENV")
                .ToList()
                .Select(w => _mapperDominio.MapToObject(w))
                .ToList();
        }

        public virtual List<DominioDetalle> GetTiposEnvases()
        {
            return _context.T_TIPO_CONTENEDOR
                .AsNoTracking()
                .Where(w => w.FL_RETORNABLE == "S")
                .ToList()
                .Select(w => new DominioDetalle { Valor = w.TP_CONTENEDOR, Descripcion = w.DS_TIPO_CONTENEDOR })
                .ToList();
        }

        #endregion

        #region Add

        public virtual void AddEnvase(Envase obj)
        {
            obj.FechaAlta = DateTime.Now;
            obj.FechaModificacion = DateTime.Now;
            obj.DescripcionUltimoMovimiento = $"Creación => Funcionario: {_userId};";

            T_STOCK_ENVASE entity = this._mapper.Map(obj);
            this._context.T_STOCK_ENVASE.Add(entity);
        }

        #endregion

        #region Update

        public virtual void UpdateEnvase(Envase obj)
        {
            obj.FechaModificacion = DateTime.Now;

            if (string.IsNullOrEmpty(obj.DescripcionUltimoMovimiento))
                obj.DescripcionUltimoMovimiento = $"Edición => Funcionario: {_userId};";

            T_STOCK_ENVASE entity = this._mapper.Map(obj);
            T_STOCK_ENVASE attachedEntity = _context.T_STOCK_ENVASE.Local
                .FirstOrDefault(x => x.ID_ENVASE == obj.Id && x.ND_TP_ENVASE == obj.TipoEnvase);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_STOCK_ENVASE.Attach(entity);
                _context.Entry<T_STOCK_ENVASE>(entity).State = EntityState.Modified;
            }
        }

        #endregion

        #region Remove

        public virtual void RemoveEnvase(Envase obj)
        {
            T_STOCK_ENVASE entity = this._mapper.Map(obj);
            T_STOCK_ENVASE attachedEntity = _context.T_STOCK_ENVASE.Local
                .FirstOrDefault(x => x.ID_ENVASE == obj.Id && x.ND_TP_ENVASE == obj.TipoEnvase);

            if (attachedEntity != null)
            {
                _context.T_STOCK_ENVASE.Remove(attachedEntity);
            }
            else
            {
                _context.T_STOCK_ENVASE.Attach(entity);
                _context.T_STOCK_ENVASE.Remove(entity);
            }
        }

        #endregion

    }
}
