using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.Eventos;
using WIS.Domain.Extensions;
using WIS.Domain.General;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class DestinatarioRepository
    {
        protected readonly WISDB _context;
        protected readonly string _application;
        protected readonly int _userId;
        protected readonly DestinatarioMapper _mapper;
        protected readonly IDapper _dapper;

        public DestinatarioRepository(WISDB context, string application, int userId, IDapper dapper)
        {
            this._context = context;
            this._application = application;
            this._userId = userId;
            this._mapper = new DestinatarioMapper();
            this._dapper = dapper;
        }

        #region Any

        public virtual bool AnyMailEmpresa(string email, int? empresa = null)
        {
            if (empresa == null)
                return false;
            else
                return this._context.T_CONTACTO
                    .AsNoTracking()
                    .Any(x => x.DS_EMAIL == email
                        && x.CD_EMPRESA == empresa);
        }

        public virtual bool AnyNombreGrupo(string nmGrupo)
        {
            return this._context.T_CONTACTO_GRUPO
                .AsNoTracking()
                .Any(x => x.NM_GRUPO.ToUpper() == nmGrupo.ToUpper());
        }

        public virtual bool AnyNombreGrupo(string nmGrupo, int nuGrupo)
        {
            return this._context.T_CONTACTO_GRUPO
                .AsNoTracking()
                .Any(x => x.NM_GRUPO.ToUpper() == nmGrupo.ToUpper()
                    && x.NU_CONTACTO_GRUPO != nuGrupo);
        }

        public virtual bool AnyContacto(Agente agente)
        {
            return this._context.T_CONTACTO
                .AsNoTracking()
                .Any(x => x.CD_EMPRESA == agente.Empresa
                    && x.CD_CLIENTE == agente.CodigoInterno);
        }

        #endregion

        #region Get

        public virtual Contacto GetContacto(int nuContacto)
        {
            T_CONTACTO entity = _context.T_CONTACTO.AsNoTracking().FirstOrDefault(w => w.NU_CONTACTO == nuContacto);

            return entity == null ? null : _mapper.MapContactoToObject(entity);

        }

        public virtual DestinatarioInstancia GetDestinatarioInstancia(int nuContacto, int nuInstancia)
        {
            T_EVENTO_GRUPO_INSTANCIA_REL entity = _context.T_EVENTO_GRUPO_INSTANCIA_REL
                .AsNoTracking()
                .FirstOrDefault(w => w.NU_CONTACTO == nuContacto
                    && w.NU_EVENTO_INSTANCIA == nuInstancia);

            return entity == null ? null : _mapper.MapDestinatarioInstanciaToObject(entity);
        }

        public virtual DestinatarioInstancia GetGrupoInstancia(int grupo, int nuInstancia)
        {
            T_EVENTO_GRUPO_INSTANCIA_REL entity = _context.T_EVENTO_GRUPO_INSTANCIA_REL
                .AsNoTracking()
                .FirstOrDefault(w => w.NU_CONTACTO_GRUPO == grupo
                    && w.NU_EVENTO_INSTANCIA == nuInstancia);

            return entity == null ? null : _mapper.MapDestinatarioInstanciaToObject(entity);
        }

        public virtual List<DestinatarioInstancia> GetGruposInstancia(int nuInstancia)
        {
            return this._context.T_EVENTO_GRUPO_INSTANCIA_REL
                .AsNoTracking()
                .Where(w => w.NU_EVENTO_INSTANCIA == nuInstancia)
                .Select(d => this._mapper.MapDestinatarioInstanciaToObject(d))
                .ToList();
        }

        public virtual Contacto GetContacto(int codigoEmpresa, string codigoCliente)
        {
            T_CONTACTO entity = _context.T_CONTACTO.AsNoTracking().FirstOrDefault(w => w.CD_EMPRESA == codigoEmpresa && w.CD_CLIENTE == codigoCliente);

            return entity == null ? null : _mapper.MapContactoToObject(entity);
        }

        public virtual List<Contacto> GetContactos(int nuInstancia)
        {
            List<Contacto> res = new List<Contacto>();
            var lst = _context.V_CONTACTOS_INSTANCIA.AsNoTracking().Where(w => w.NU_EVENTO_INSTANCIA == nuInstancia);

            foreach (var r in lst)
                res.Add(_mapper.MapContactoToObject(r));
            return res;
        }

        public virtual GrupoContacto GetGrupo(int nuGrupo)
        {
            T_CONTACTO_GRUPO entity = _context.T_CONTACTO_GRUPO.AsNoTracking().FirstOrDefault(w => w.NU_CONTACTO_GRUPO == nuGrupo);

            return entity == null ? null : _mapper.MapGrupoToObject(entity);
        }

        public virtual ContactoGrupo GetContactoGrupo(int numeroGrupo, int numeroContacto)
        {
            T_CONTACTO_GRUPO_REL entity = _context.T_CONTACTO_GRUPO_REL.AsNoTracking().FirstOrDefault(w => w.NU_CONTACTO_GRUPO == numeroGrupo && w.NU_CONTACTO == numeroContacto);

            return entity == null ? null : _mapper.MapToObject(entity);
        }

        public virtual int GetNextNuDestinatarioInstancia()
        {
            return this._context.GetNextSequenceValueInt(_dapper, "S_NU_EVENTO_DESTINATARIO");
        }

        public virtual int GetNextNuContacto()
        {
            return this._context.GetNextSequenceValueInt(_dapper, Secuencias.S_CONTACTO);
        }

        public virtual int GetNextNuGrupo()
        {
            return this._context.GetNextSequenceValueInt(_dapper, "S_NU_CONTACTO_GRUPO");
        }

        #endregion

        #region Add

        public virtual void AddContacto(Contacto contacto)
        {
            var entity = this._mapper.MapContactoToEntity(contacto);
            this._context.T_CONTACTO.Add(entity);
        }

        public virtual void AddGrupo(GrupoContacto obj)
        {
            var entity = this._mapper.MapGrupoToEntity(obj);
            this._context.T_CONTACTO_GRUPO.Add(entity);
        }

        public virtual void AddDestinatarioToInstancia(DestinatarioInstancia obj)
        {
            T_EVENTO_GRUPO_INSTANCIA_REL entity = _mapper.MapDestinatarioInstanciaToEntity(obj);
            _context.T_EVENTO_GRUPO_INSTANCIA_REL.Add(entity);
        }

        public virtual void AddContactoToGrupo(ContactoGrupo contactoGrupo)
        {
            T_CONTACTO_GRUPO_REL entity = _mapper.MapToEntity(contactoGrupo);
            _context.T_CONTACTO_GRUPO_REL.Add(entity);
        }

        #endregion

        #region Update

        public virtual void UpdateGrupo(GrupoContacto obj)
        {
            T_CONTACTO_GRUPO entity = this._mapper.MapGrupoToEntity(obj);
            T_CONTACTO_GRUPO attachedEntity = _context.T_CONTACTO_GRUPO.Local
                .FirstOrDefault(w => w.NU_CONTACTO_GRUPO == entity.NU_CONTACTO_GRUPO);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_CONTACTO_GRUPO.Attach(entity);
                _context.Entry<T_CONTACTO_GRUPO>(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateContactoGrupo(ContactoGrupo obj)
        {
            T_CONTACTO_GRUPO_REL entity = this._mapper.MapToEntity(obj);
            T_CONTACTO_GRUPO_REL attachedEntity = _context.T_CONTACTO_GRUPO_REL.Local
                .FirstOrDefault(w => w.NU_CONTACTO_GRUPO == entity.NU_CONTACTO_GRUPO
                && w.NU_CONTACTO == entity.NU_CONTACTO);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_CONTACTO_GRUPO_REL.Attach(entity);
                _context.Entry<T_CONTACTO_GRUPO_REL>(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateContacto(Contacto contacto)
        {
            T_CONTACTO entity = this._mapper.MapContactoToEntity(contacto);
            T_CONTACTO attachedEntity = _context.T_CONTACTO.Local
                .FirstOrDefault(c => c.NU_CONTACTO == entity.NU_CONTACTO);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_CONTACTO.Attach(entity);
                _context.Entry<T_CONTACTO>(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateContacto(Agente agente)
        {
            _context.T_CONTACTO
                .Where(c => c.CD_CLIENTE == agente.CodigoInterno && c.CD_EMPRESA == agente.Empresa)
                .ExecuteUpdate(setters => setters
                    .SetProperty(e => e.DS_EMAIL, e => agente.Email)
                    .SetProperty(e => e.NU_TRANSACCION, e => agente.Transaccion)
                    .SetProperty(e => e.DT_UPDROW, e => DateTime.Now));
        }

        public virtual void UpdateDestinatarioInstancia(DestinatarioInstancia contacto)
        {
            T_EVENTO_GRUPO_INSTANCIA_REL entity = this._mapper.MapDestinatarioInstanciaToEntity(contacto);
            T_EVENTO_GRUPO_INSTANCIA_REL attachedEntity = _context.T_EVENTO_GRUPO_INSTANCIA_REL.Local
                .FirstOrDefault(w => w.NU_CONTACTO == contacto.NumeroContacto
                    && w.NU_EVENTO_INSTANCIA == contacto.NumeroInstancia);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_EVENTO_GRUPO_INSTANCIA_REL.Attach(entity);
                _context.Entry<T_EVENTO_GRUPO_INSTANCIA_REL>(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateGrupoInstancia(DestinatarioInstancia contacto)
        {
            T_EVENTO_GRUPO_INSTANCIA_REL entity = this._mapper.MapDestinatarioInstanciaToEntity(contacto);
            T_EVENTO_GRUPO_INSTANCIA_REL attachedEntity = _context.T_EVENTO_GRUPO_INSTANCIA_REL.Local
                .FirstOrDefault(w => w.NU_CONTACTO_GRUPO == contacto.NumeroGrupo
                    && w.NU_EVENTO_INSTANCIA == contacto.NumeroInstancia);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_EVENTO_GRUPO_INSTANCIA_REL.Attach(entity);
                _context.Entry<T_EVENTO_GRUPO_INSTANCIA_REL>(entity).State = EntityState.Modified;
            }
        }

        #endregion

        #region Remove
        
        public virtual void RemoveContactoOfInstancia(DestinatarioInstancia destinatarioInstancia)
        {
            var entity = _context.T_EVENTO_GRUPO_INSTANCIA_REL
                .FirstOrDefault(w => w.NU_CONTACTO == destinatarioInstancia.NumeroContacto
                    && w.NU_EVENTO_INSTANCIA == destinatarioInstancia.NumeroInstancia);
            var attachedEntity = _context.T_EVENTO_GRUPO_INSTANCIA_REL.Local
                .FirstOrDefault(w => w.NU_CONTACTO == destinatarioInstancia.NumeroContacto
                    && w.NU_EVENTO_INSTANCIA == destinatarioInstancia.NumeroInstancia);

            if (attachedEntity != null)
            {
                _context.T_EVENTO_GRUPO_INSTANCIA_REL.Remove(attachedEntity);
            }
            else if (entity != null)
            {
                _context.T_EVENTO_GRUPO_INSTANCIA_REL.Remove(entity);
            }
        }

        public virtual void RemoveGrupoOfInstancia(DestinatarioInstancia grupoInstancia)
        {
            var entity = _context.T_EVENTO_GRUPO_INSTANCIA_REL
                .FirstOrDefault(w => w.NU_CONTACTO_GRUPO == grupoInstancia.NumeroGrupo
                    && w.NU_EVENTO_INSTANCIA == grupoInstancia.NumeroInstancia);
            var attachedEntity = _context.T_EVENTO_GRUPO_INSTANCIA_REL.Local
                .FirstOrDefault(w => w.NU_CONTACTO_GRUPO == grupoInstancia.NumeroGrupo
                    && w.NU_EVENTO_INSTANCIA == grupoInstancia.NumeroInstancia);

            if (attachedEntity != null)
            {
                _context.T_EVENTO_GRUPO_INSTANCIA_REL.Remove(attachedEntity);
            }
            else if (entity != null)
            {
                _context.T_EVENTO_GRUPO_INSTANCIA_REL.Remove(entity);
            }
        }

        public virtual void RemoveContactoOfGrupo(ContactoGrupo contactoGrupo)
        {
            T_CONTACTO_GRUPO_REL entity = this._context.T_CONTACTO_GRUPO_REL
                .FirstOrDefault(x => x.NU_CONTACTO_GRUPO == contactoGrupo.NumeroContactoGrupo
                && x.NU_CONTACTO == contactoGrupo.NumeroContacto);

            T_CONTACTO_GRUPO_REL attachedEntity = _context.T_CONTACTO_GRUPO_REL.Local
                .FirstOrDefault(x => x.NU_CONTACTO_GRUPO == contactoGrupo.NumeroContactoGrupo
                && x.NU_CONTACTO == contactoGrupo.NumeroContacto);

            if (attachedEntity != null)
            {
                _context.T_CONTACTO_GRUPO_REL.Remove(attachedEntity);
            }
            else
            {
                _context.T_CONTACTO_GRUPO_REL.Attach(entity);
                _context.T_CONTACTO_GRUPO_REL.Remove(entity);
            }
        }

        #endregion

        #region Dapper

        #endregion
    }
}
