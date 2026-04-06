using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers.Produccion;
using WIS.Domain.Produccion;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class IdentificadorConsumirRepository
    {
        protected readonly WISDB _context;
        protected readonly string _application;
        protected readonly int _userId;
        protected readonly IdentificadorConsumirMapper _mapper;

        public IdentificadorConsumirRepository(WISDB context, string application, int userId)
        {
            this._context = context;
            this._application = application;
            this._userId = userId;
            this._mapper = new IdentificadorConsumirMapper();
        }

        #region Any

        #endregion

        #region Get

        public virtual List<IdentificadorConsumir> GetIdentificadores(int? empresa, List<string> productos, string ubicacion)
        {
            var identificadores = new List<IdentificadorConsumir>();
            var entities = this._context.T_PRDC_CONSUMO_IDENTIFICADOR
                .Where(d => d.CD_ENDERECO == ubicacion
                    && d.CD_EMPRESA == empresa
                    && productos.Contains(d.CD_PRODUTO)
                    && d.QT_STOCK > 0)
                .OrderBy(d => d.NU_ORDEN).ToList();

            foreach (var entity in entities)
            {
                identificadores.Add(this._mapper.MapEntityToObject(entity));
            }

            return identificadores;
        }

        #endregion

        #region Add

        #endregion

        #region Update

        public virtual void UpdateIdentificador(IdentificadorConsumir identificador)
        {
            var entity = this._mapper.MapObjectToEntity(identificador);
            var attachedEntity = this._context.T_PRDC_CONSUMO_IDENTIFICADOR.Local
                .Where(d => d.CD_ENDERECO == identificador.Ubicacion
                    && d.CD_EMPRESA == identificador.Empresa
                    && d.CD_FAIXA == identificador.Faixa
                    && d.CD_PRODUTO == identificador.Producto
                    && d.NU_ORDEN == identificador.Orden)
                .FirstOrDefault();

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.T_PRDC_CONSUMO_IDENTIFICADOR.Attach(entity);
                this._context.Entry(entity).State = EntityState.Modified;
            }
        }

        #endregion

        #region Remove

        public virtual void DeleteIdentificador(IdentificadorConsumir identificador)
        {
            var entity = this._mapper.MapObjectToEntity(identificador);
            var attachedEntity = this._context.T_PRDC_CONSUMO_IDENTIFICADOR.Local
                .Where(d => d.CD_ENDERECO == identificador.Ubicacion
                    && d.CD_EMPRESA == identificador.Empresa
                    && d.CD_FAIXA == identificador.Faixa
                    && d.CD_PRODUTO == identificador.Producto
                    && d.NU_ORDEN == identificador.Orden)
                .FirstOrDefault();

            if (attachedEntity != null)
            {
                this._context.T_PRDC_CONSUMO_IDENTIFICADOR.Remove(attachedEntity);
            }
            else
            {
                this._context.T_PRDC_CONSUMO_IDENTIFICADOR.Attach(entity);
                this._context.T_PRDC_CONSUMO_IDENTIFICADOR.Remove(entity);
            }
        }

        #endregion

        #region Dapper

        #endregion
    }
}
