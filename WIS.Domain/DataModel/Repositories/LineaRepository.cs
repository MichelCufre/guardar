using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Domain.DataModel.Mappers.Produccion;
using WIS.Domain.Extensions;
using WIS.Domain.Produccion;
using WIS.Domain.Produccion.Enums;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class LineaRepository
    {
        protected readonly WISDB _context;
        protected readonly string _cdAplicacion;
        protected readonly int _userId;
        protected readonly LineaMapper _mapper;
        protected readonly IDapper _dapper;

        public LineaRepository(WISDB context, string cdAplicacion, int userId, IDapper dapper)
        {
            this._context = context;
            this._cdAplicacion = cdAplicacion;
            this._userId = userId;
            this._mapper = new LineaMapper();
            this._dapper = dapper;
        }

        #region Any

        public virtual bool AnyLinea(string codigo)
        {
            return this._context.T_PRDC_LINEA.Any(d => d.CD_PRDC_LINEA == codigo);
        }

        #endregion

        #region Get

        public virtual ILinea GetLinea(string id)
        {
            var entity = this._context.T_PRDC_LINEA
                .AsNoTracking()
                .FirstOrDefault(s => s.CD_PRDC_LINEA == id);

            return this._mapper.MapLineaEntityToObject(entity);
        }

        public virtual ILinea GetLineaByUbicacionSalida(string ubicacion)
        {
            var entity = this._context.T_PRDC_LINEA
                .Where(d => d.CD_ENDERECO_SALIDA == ubicacion)
                .AsNoTracking()
                .FirstOrDefault();

            return this._mapper.MapLineaEntityToObject(entity);
        }

        public virtual string GetNumeroLinea()
        {
            return this._context.GetNextSequenceValueDecimal(_dapper, "S_PRDC_LINEA").ToString();
        }

        public virtual LineaBlackBox GetLineaBlackBoxPorUbicacionEntrada(string ubicacionEntrada)
        {
            var tipoLinea = this._mapper.MapTipoLineaToString(TipoProduccionLinea.BlackBox);
            T_PRDC_LINEA lineaBlackBox = this._context.T_PRDC_LINEA
                .FirstOrDefault(l => l.CD_ENDERECO_ENTRADA == ubicacionEntrada
                    && l.ND_TIPO_LINEA == tipoLinea);

            if (lineaBlackBox == null)
                return null;

            return this._mapper.MapLineaBlackBoxEntityToObject(lineaBlackBox);
        }

        public virtual LineaBlackBox GetLineaBlackBoxPorUbicacionBB(string ubicacionEntrada)
        {
            var tipoLinea = this._mapper.MapTipoLineaToString(TipoProduccionLinea.BlackBox);
            T_PRDC_LINEA lineaBlackBox = this._context.T_PRDC_LINEA
                .FirstOrDefault(l => l.CD_ENDERECO_PRODUCCION == ubicacionEntrada
                    && l.ND_TIPO_LINEA == tipoLinea);

            if (lineaBlackBox == null)
                return null;

            return this._mapper.MapLineaBlackBoxEntityToObject(lineaBlackBox);
        }

        public virtual DetallePedidoE GetDetallePedido(int empresa, string producto, string identificador, decimal faixa, string ingreso)
        {
            var tipoLinea = this._mapper.MapTipoLineaToString(TipoProduccionLinea.BlackBox);
            var lineaBlackBox = this._context.V_KIT200_DET_PEDIDO_SAIDA
                .FirstOrDefault(l => l.CD_EMPRESA == empresa
                    && l.CD_PRODUTO == producto
                    && l.NU_IDENTIFICADOR == identificador
                    && l.CD_FAIXA == faixa
                    && l.NU_PEDIDO == ingreso);

            if (lineaBlackBox == null)
                return null;

            return this._mapper.MapEntityToObject(lineaBlackBox);
        }
        
        #endregion

        #region Add
        
        public virtual void AddLinea(ILinea linea)
        {
            var entity = this._mapper.MapObjectToEntity(linea);

            entity.DT_ADDROW = DateTime.Now;
            entity.DT_UPDROW = DateTime.Now;

            this._context.T_PRDC_LINEA.Add(entity);
        }

        #endregion

        #region Update
        
        public virtual void UpdateLinea(ILinea linea)
        {
            T_PRDC_LINEA entity = this._mapper.MapObjectToEntity(linea);

            entity.DT_UPDROW = DateTime.Now;

            T_PRDC_LINEA attachedEntity = _context.T_PRDC_LINEA.Local
                .FirstOrDefault(c => c.CD_PRDC_LINEA == linea.Id);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_PRDC_LINEA.Attach(entity);
                _context.Entry<T_PRDC_LINEA>(entity).State = EntityState.Modified;
            }
        }

        #endregion

        #region Remove
        
        public virtual void RemoveLinea(ILinea linea)
        {
            var entity = this._mapper.MapObjectToEntity(linea);
            var attachedEntity = _context.T_PRDC_LINEA.Local
                .FirstOrDefault(c => c.CD_PRDC_LINEA == entity.CD_PRDC_LINEA);

            if (attachedEntity != null)
            {
                this._context.T_PRDC_LINEA.Remove(attachedEntity);
            }
            else
            {
                this._context.T_PRDC_LINEA.Attach(entity);
                this._context.T_PRDC_LINEA.Remove(entity);
            }
        }

        #endregion

        #region Dapper

        #endregion
    }
}
