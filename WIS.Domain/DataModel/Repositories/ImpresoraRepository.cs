using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Impresiones;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class ImpresoraRepository
    {
        protected readonly WISDB _context;
        protected readonly string _application;
        protected readonly int _userId;
        protected readonly ImpresoraMapper _mapper;
        protected readonly IDapper _dapper;

        public ImpresoraRepository(WISDB context, string application, int userId, IDapper dapper)
        {
            this._context = context;
            this._application = application;
            this._userId = userId;
            this._mapper = new ImpresoraMapper();
            this._dapper = dapper;
        }

        #region Any

        public virtual bool ExisteImpresoraPredio(string codigo, string predio)
        {
            return this._context.T_IMPRESORA.Any(x => x.CD_IMPRESORA == codigo && x.NU_PREDIO == predio);
        }

        public virtual bool ExisteImpresora(string predio)
        {
            return this._context.T_IMPRESORA.Any(x => x.NU_PREDIO == predio);
        }

        public virtual bool ExisteImpresora(string codigo, string predio)
        {
            if (predio != GeneralDb.PredioSinDefinir)
                return this._context.T_IMPRESORA.Any(x => x.CD_IMPRESORA == codigo && x.NU_PREDIO == predio);
            else
                return this._context.T_IMPRESORA.Any(x => x.CD_IMPRESORA == codigo);

        }
        
        #endregion

        #region Get

        public virtual Impresora GetImpresora(string codigo, string predio)
        {
            var entity = this._context.T_IMPRESORA.FirstOrDefault(x => x.CD_IMPRESORA == codigo && x.NU_PREDIO == predio);

            if (entity == null)
                return null;

            return this._mapper.MapToObject(entity);
        }

        public virtual List<Impresora> GetListaImpresorasPredio(string predio = null)
        {
            List<Impresora> ListaRetorno = new List<Impresora>();
            List<T_IMPRESORA> lista = new List<T_IMPRESORA>();

            if (!string.IsNullOrEmpty(predio))
                lista = this._context.T_IMPRESORA.Where(x => x.NU_PREDIO == predio).ToList();
            else
                lista = this._context.T_IMPRESORA.ToList();

            lista.ForEach(item =>
                ListaRetorno.Add(this._mapper.MapToObject(item))
            );

            return ListaRetorno;
        }

        #endregion

        #region Add

        public virtual void AddImpresora(Impresora obj)
        {
            T_IMPRESORA entity = this._mapper.MapToEntity(obj);

            this._context.T_IMPRESORA.Add(entity);
        }

        #endregion

        #region Update

        public virtual void UpdateImpresora(Impresora impre)
        {
            T_IMPRESORA entity = this._mapper.MapToEntity(impre);
            T_IMPRESORA attachedEntity = _context.T_IMPRESORA.Local
                .FirstOrDefault(w => w.CD_IMPRESORA == entity.CD_IMPRESORA && w.NU_PREDIO == entity.NU_PREDIO);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_IMPRESORA.Attach(entity);
                _context.Entry<T_IMPRESORA>(entity).State = EntityState.Modified;
            }
        }

        #endregion

        #region Remove

        #endregion

        #region Dapper

        public virtual async Task<ImpresoraServidor> GetImpresora(string nombre)
        {
            var param = new DynamicParameters(new
            {
                nombre
            });

            var impresora = await Task.FromResult(_dapper.Get<ImpresoraServidor>(
                @"SELECT 
                    imp.DS_IMPRESORA AS Descripcion,
                    imp.CD_IMPRESORA AS Id,
                    serv.VL_DOMINIO_SERVIDOR AS Address,
                    serv.CLIENTID AS ClientId
                FROM T_IMPRESORA imp
                INNER JOIN T_IMPRESORA_SERVIDOR serv ON imp.CD_SERVIDOR = serv.CD_SERVIDOR
                WHERE imp.CD_IMPRESORA = :nombre",
                param,
                commandType: CommandType.Text));

            return impresora;
        }

        #endregion
    }
}
