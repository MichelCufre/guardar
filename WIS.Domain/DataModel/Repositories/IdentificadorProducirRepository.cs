using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers.Produccion;
using WIS.Domain.Produccion;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class IdentificadorProducirRepository
    {
        protected readonly WISDB _context;
        protected readonly string _application;
        protected readonly int _userId;
        protected readonly IdentificadorProducirMapper _mapper;

        public IdentificadorProducirRepository(WISDB context, string application, int userId)
        {
            this._context = context;
            this._application = application;
            this._userId = userId;
            this._mapper = new IdentificadorProducirMapper();
        }

        #region Any

        public virtual bool AnyIdentificador(string ubicacion, int empresa, int orden, string producto)
        {
            return this._context.T_PRDC_EGRESO_IDENTIFICADOR
                .Any(d => d.CD_ENDERECO == ubicacion
                    && d.CD_EMPRESA == empresa
                    && d.NU_ORDEN == orden
                    && d.CD_PRODUTO == producto);
        }

        #endregion

        #region Get

        public virtual IdentificadorProducir GetIdentificador(string ubicacion, int empresa, string producto, int orden)
        {
            var entity = this._context.T_PRDC_EGRESO_IDENTIFICADOR
                .Where(d => d.CD_ENDERECO == ubicacion
                    && d.CD_EMPRESA == empresa
                    && d.CD_PRODUTO == producto
                    && d.NU_ORDEN == orden)
                .AsNoTracking()
                .FirstOrDefault();

            return this._mapper.MapEntityToObject(entity);
        }

        public virtual List<IdentificadorProducir> GetIdentificadores(int? empresa, List<string> productos, string ubicacion)
        {
            var identificadores = new List<IdentificadorProducir>();
            var entities = this._context.T_PRDC_EGRESO_IDENTIFICADOR
                .Where(d => d.CD_ENDERECO == ubicacion
                    && d.CD_EMPRESA == empresa
                    && productos.Contains(d.CD_PRODUTO)
                    && d.QT_STOCK > 0)
                .OrderBy(d => d.NU_ORDEN)
                .ToList();

            foreach (var entity in entities)
            {
                identificadores.Add(this._mapper.MapEntityToObject(entity));
            }

            return identificadores;
        }

        #endregion

        #region Add

        public virtual void AddIdentificador(IdentificadorProducir identificador)
        {
            T_PRDC_EGRESO_IDENTIFICADOR entity = this._mapper.MapObjectToEntity(identificador);

            this._context.T_PRDC_EGRESO_IDENTIFICADOR.Add(entity);
        }

        #endregion

        #region Update

        public virtual void UpdateIdentificador(IdentificadorProducir identificador)
        {
            var entity = this._mapper.MapObjectToEntity(identificador);
            var attachedEntity = this._context.T_PRDC_EGRESO_IDENTIFICADOR.Local
                .FirstOrDefault(d => d.CD_ENDERECO == entity.CD_ENDERECO
                    && d.CD_EMPRESA == entity.CD_EMPRESA
                    && d.CD_FAIXA == entity.CD_FAIXA
                    && d.CD_PRODUTO == entity.CD_PRODUTO
                    && d.NU_ORDEN == entity.NU_ORDEN);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.T_PRDC_EGRESO_IDENTIFICADOR.Attach(entity);
                this._context.Entry(entity).State = EntityState.Modified;
            }
        }

        #endregion

        #region Remove
        public virtual void DeleteIdentificador(IdentificadorProducir identificador)
        {
            var entity = this._mapper.MapObjectToEntity(identificador);
            var attachedEntity = this._context.T_PRDC_EGRESO_IDENTIFICADOR.Local
                .FirstOrDefault(d => d.CD_ENDERECO == entity.CD_ENDERECO
                    && d.CD_EMPRESA == entity.CD_EMPRESA
                    && d.CD_FAIXA == entity.CD_FAIXA
                    && d.CD_PRODUTO == entity.CD_PRODUTO
                    && d.NU_ORDEN == entity.NU_ORDEN);

            if (attachedEntity != null)
            {
                this._context.T_PRDC_EGRESO_IDENTIFICADOR.Remove(attachedEntity);
            }
            else
            {
                this._context.T_PRDC_EGRESO_IDENTIFICADOR.Attach(entity);
                this._context.T_PRDC_EGRESO_IDENTIFICADOR.Remove(entity);
            }
        }

        #endregion

        #region Dapper

        #endregion
    }
}
