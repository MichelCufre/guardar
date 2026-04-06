using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.OrdenTarea;
using WIS.Domain.OrdenTarea.Constants;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class InsumoManipuleoRepository
    {
        protected readonly WISDB _context;
        protected readonly string _cdAplicacion;
        protected readonly int _userId;
        protected readonly InsumoManipuleoMapper _mapper;

        public InsumoManipuleoRepository(WISDB context, string cdAplicacion, int userId)
        {
            this._context = context;
            this._cdAplicacion = cdAplicacion;
            this._userId = userId;
            this._mapper = new InsumoManipuleoMapper();
        }

        #region Any

        public virtual bool ExisteInsumoManipuleo(string cdInsumoManipuleo)
        {
            return this._context.T_ORT_INSUMO_MANIPULEO.Any(cb => cb.CD_INSUMO_MANIPULEO == cdInsumoManipuleo);
        }

        #endregion

        #region Get
        
        public virtual InsumoManipuleo GetInsumoManipuleo(string cdInsumoManipuleo)
        {
            return this._mapper.MapToObject(this._context.T_ORT_INSUMO_MANIPULEO.FirstOrDefault(x => x.CD_INSUMO_MANIPULEO == cdInsumoManipuleo));
        }

        public virtual List<InsumoManipuleo> GetInsumosManipuleos(int cdEmpresa)
        {
            var insumosManipuleos = new List<InsumoManipuleo>();
            var entities = this._context.T_ORT_INSUMO_MANIPULEO
                .Where(im => im.FL_TODA_EMPRESA == "S" ||
                            this._context.T_ORT_INSUMO_MANIPULEO_EMPRESA.Any(ime =>
                                ime.CD_INSUMO_MANIPULEO == im.CD_INSUMO_MANIPULEO &&
                                ime.CD_EMPRESA == cdEmpresa))
                .ToList();


            foreach (var entity in entities)
            {
                insumosManipuleos.Add(this._mapper.MapToObject(entity));
            }

            return insumosManipuleos;
        }

        public virtual List<InsumoManipuleo> GetInsumos(int cdEmpresa)
        {
            var insumos = new List<InsumoManipuleo>();
            var entities = this._context.T_ORT_INSUMO_MANIPULEO
                .Where(im => im.TP_INSUMO_MANIPULEO == OrdenTareaDb.TipoInsumo &&
                            (im.FL_TODA_EMPRESA == "S" ||
                            this._context.T_ORT_INSUMO_MANIPULEO_EMPRESA.Any(ime =>
                                ime.CD_INSUMO_MANIPULEO == im.CD_INSUMO_MANIPULEO &&
                                ime.CD_EMPRESA == cdEmpresa)))
                .ToList();

            foreach (var entity in entities)
            {
                insumos.Add(this._mapper.MapToObject(entity));
            }

            return insumos;
        }

        public virtual List<InsumoManipuleo> GetManipuleos(int cdEmpresa)
        {
            var manipuleos = new List<InsumoManipuleo>();
            var entities = this._context.T_ORT_INSUMO_MANIPULEO
                .Where(im => im.TP_INSUMO_MANIPULEO == OrdenTareaDb.TipoManipuleo &&
                            (im.FL_TODA_EMPRESA == "S" ||
                            this._context.T_ORT_INSUMO_MANIPULEO_EMPRESA.Any(ime =>
                                ime.CD_INSUMO_MANIPULEO == im.CD_INSUMO_MANIPULEO &&
                                ime.CD_EMPRESA == cdEmpresa)))
                .ToList();

            foreach (var entity in entities)
            {
                manipuleos.Add(this._mapper.MapToObject(entity));
            }

            return manipuleos;
        }

        #endregion

        #region Add
        
        public virtual void AddInsumoManipuleo(InsumoManipuleo insumo)
        {
            T_ORT_INSUMO_MANIPULEO entity = this._mapper.MapToEntity(insumo);
            this._context.T_ORT_INSUMO_MANIPULEO.Add(entity);
        }

        public virtual void AsignarEmpresas(string cdInsumoManipuleo, List<int> empresas)
        {
            foreach (var emp in empresas?.Distinct())
            {
                if (!this._context.T_ORT_INSUMO_MANIPULEO_EMPRESA.Any(x => x.CD_INSUMO_MANIPULEO == cdInsumoManipuleo && x.CD_EMPRESA == emp))
                {
                    this._context.T_ORT_INSUMO_MANIPULEO_EMPRESA.Add(new T_ORT_INSUMO_MANIPULEO_EMPRESA
                    {
                        CD_INSUMO_MANIPULEO = cdInsumoManipuleo,
                        CD_EMPRESA = emp
                    });
                }
            }
        }
        
        #endregion

        #region Update
        
        public virtual void UpdateInsumoManipuleo(InsumoManipuleo insumoManipuleo)
        {
            T_ORT_INSUMO_MANIPULEO entity = this._mapper.MapToEntity(insumoManipuleo);
            T_ORT_INSUMO_MANIPULEO attachedEntity = _context.T_ORT_INSUMO_MANIPULEO.Local
                .FirstOrDefault(w => w.CD_INSUMO_MANIPULEO == entity.CD_INSUMO_MANIPULEO);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_ORT_INSUMO_MANIPULEO.Attach(entity);
                _context.Entry<T_ORT_INSUMO_MANIPULEO>(entity).State = EntityState.Modified;
            }
        }
        
        #endregion

        #region Remove
        
        public virtual void DeleteInsumoManipuleo(string cdInsumoManipuleo)
        {
            var entity = this._context.T_ORT_INSUMO_MANIPULEO
                .FirstOrDefault(x => x.CD_INSUMO_MANIPULEO == cdInsumoManipuleo);
            var attachedEntity = _context.T_ORT_INSUMO_MANIPULEO.Local
                .FirstOrDefault(w => w.CD_INSUMO_MANIPULEO == entity.CD_INSUMO_MANIPULEO);

            if (attachedEntity != null)
                _context.T_ORT_INSUMO_MANIPULEO.Remove(attachedEntity);
            else
                _context.T_ORT_INSUMO_MANIPULEO.Remove(entity);
        }

        public virtual void RemoverEmpresas(string cdInsumoManipuleo, List<int> empresas)
        {
            foreach (var emp in empresas)
            {
                var entity = this._context.T_ORT_INSUMO_MANIPULEO_EMPRESA
                    .FirstOrDefault(x => x.CD_INSUMO_MANIPULEO == cdInsumoManipuleo && x.CD_EMPRESA == emp);
                var attachedEntity = this._context.T_ORT_INSUMO_MANIPULEO_EMPRESA.Local
                    .FirstOrDefault(x => x.CD_INSUMO_MANIPULEO == cdInsumoManipuleo && x.CD_EMPRESA == emp);

                if (attachedEntity != null)
                {
                    this._context.T_ORT_INSUMO_MANIPULEO_EMPRESA.Remove(attachedEntity);
                }
                else
                {
                    this._context.T_ORT_INSUMO_MANIPULEO_EMPRESA.Remove(entity);
                }

            }
        }

        public virtual void RemoverTodasEmpresas(string cdInsumoManipuleo)
        {
            var entities = this._context.T_ORT_INSUMO_MANIPULEO_EMPRESA
                .Where(x => x.CD_INSUMO_MANIPULEO == cdInsumoManipuleo)
                .ToList();

            foreach (var entity in entities)
            {
                var attachedEntity = this._context.T_ORT_INSUMO_MANIPULEO_EMPRESA.Local
                    .FirstOrDefault(x => x.CD_INSUMO_MANIPULEO == entity.CD_INSUMO_MANIPULEO && x.CD_EMPRESA == entity.CD_EMPRESA);

                this._context.T_ORT_INSUMO_MANIPULEO_EMPRESA.Remove(attachedEntity ?? entity);
            }
        }
        
        #endregion

    }
}
