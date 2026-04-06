using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.Extensions;
using WIS.Domain.General;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class EquipoRepository
    {
        protected readonly WISDB _context;
        protected readonly string _application;
        protected readonly int _userId;
        protected readonly EquipoMapper _mapper;
        protected readonly UbicacionMapper _mapperUbicacion;
        protected readonly IDapper _dapper;

        public EquipoRepository(WISDB context, string application, int userId, IDapper dapper)
        {
            this._context = context;
            this._application = application;
            this._userId = userId;
            this._mapper = new EquipoMapper();
            this._mapperUbicacion = new UbicacionMapper();
            this._dapper = dapper;
        }

        #region Any

        public virtual bool AnyHerramienta(short id)
        {
            return _context.T_FERRAMENTAS.Any(h => h.CD_FERRAMENTA == id);
        }

        public virtual bool PuedeBorrarEquipo(Equipo equipo)
        {
            return !_context.T_TRACE_STOCK.AsNoTracking().Any(x => x.CD_ENDERECO == equipo.CodigoUbicacion);
        }

        public virtual bool AnyEquipoManual(int cdEquipo)
        {
            return this._context.T_EQUIPO
                .Include("T_FERRAMENTAS")
                .AsNoTracking()
                .Any(c => c.CD_EQUIPO == cdEquipo && c.T_FERRAMENTAS.ID_AUTOASIGNADO == "N");
        }

        public virtual bool AnyEquipoManualByUbicacion(string ubicacion)
        {
            return _context.T_EQUIPO
                    .Include("T_FERRAMENTAS")
                    .AsNoTracking()
                    .Any(c => c.CD_ENDERECO == ubicacion && c.T_FERRAMENTAS.ID_AUTOASIGNADO == "N");
        }
        #endregion

        #region Get

        public virtual int GetEquipoId()
        {
            return this._context.GetNextSequenceValueInt(_dapper, "S_EQUIPO");
        }

        public virtual Equipo GetEquipo(int codigo)
        {
            var entity = this._context.T_EQUIPO
                .Include("T_FERRAMENTAS")
                .Include("T_ENDERECO_ESTOQUE")
                .AsNoTracking()
                .FirstOrDefault(c => c.CD_EQUIPO == codigo);

            var equipo = this._mapper.MapToObject(entity);

            if (equipo != null)
                equipo.Ubicacion = this._mapperUbicacion.MapToObject(entity.T_ENDERECO_ESTOQUE);

            return equipo;
        }

        public virtual List<Equipo> GetEquipos()
        {
            return this._context.T_EQUIPO
                .Include("T_FERRAMENTAS")
                .Where(x => x.T_FERRAMENTAS.ID_AUTOASIGNADO == "N")
                .Select(x => this._mapper.MapToObject(x))
                .AsNoTracking()
                .ToList();
        }

        public virtual IEnumerable<Herramienta> GetHerramientas()
        {
            return _context.T_FERRAMENTAS
                .AsNoTracking()
                .Select(h => _mapper.MapToObject(h));
        }
        public virtual IEnumerable<Herramienta> GetHerramientas(bool autoAsignada)
        {
            return _context.T_FERRAMENTAS
                .AsNoTracking()
                .Where(h => h.ID_AUTOASIGNADO == (autoAsignada ? "S" : "N"))
                .Select(h => _mapper.MapToObject(h));
        }

        public virtual Herramienta GetHerramienta(short id)
        {
            return _context.T_FERRAMENTAS
                .AsNoTracking()
                .Where(x => x.CD_FERRAMENTA == id)
                .Select(x => _mapper.MapToObject(x))
                .FirstOrDefault();
        }
        public virtual bool GetEquipoManualByEndereco(string ubicacion, out Equipo equipoContenedor)
        {
            bool isEnderecoEquipoManual = false;
            var entity = this._context.T_EQUIPO
               .Include("T_FERRAMENTAS")
               .Include("T_ENDERECO_ESTOQUE")
               .AsNoTracking()
               .FirstOrDefault(c => c.CD_ENDERECO == ubicacion);

            equipoContenedor = this._mapper.MapToObject(entity);
            if (equipoContenedor != null && equipoContenedor.Herramienta != null && !equipoContenedor.Herramienta.Autoasignado)
            {
                isEnderecoEquipoManual = true;
            }
            return isEnderecoEquipoManual;
        }
        #endregion

        #region Add
        public virtual void AddEquipo(Equipo equipo)
        {
            var entity = this._mapper.MapToEntity(equipo);
            this._context.T_EQUIPO.Add(entity);
        }
        #endregion

        #region Update
        public virtual void UpdateEquipo(Equipo equipo)
        {
            var entity = this._mapper.MapToEntity(equipo);
            var attachedEntity = _context.T_EQUIPO.Local
                .FirstOrDefault(w => w.CD_EQUIPO == entity.CD_EQUIPO);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.T_EQUIPO.Attach(entity);
                this._context.Entry(entity).State = EntityState.Modified;
            }
        }
        public virtual void ModificarEquipo(Equipo equipo, int userId, bool modificarZona, string tipoOperacion = null, string ubicacionReal = null, string zona = null)
        {
            bool autoAsignado = equipo.Herramienta.Autoasignado;
            if (!autoAsignado)
            {
                equipo.CodigoUbicacionReal = ubicacionReal;
                equipo.TipoOperacion = tipoOperacion;
                if (modificarZona)
                {
                    equipo.CodigoZona = zona;
                }
                equipo.FechaModificacion = DateTime.Now;


                var entity = this._mapper.MapToEntity(equipo);
                var attachedEntity = _context.T_EQUIPO.Local
                    .FirstOrDefault(w => w.CD_EQUIPO == entity.CD_EQUIPO);

                if (attachedEntity != null)
                {
                    var attachedEntry = _context.Entry(attachedEntity);
                    attachedEntry.CurrentValues.SetValues(entity);
                    attachedEntry.State = EntityState.Modified;
                }
                else
                {
                    this._context.T_EQUIPO.Attach(entity);
                    this._context.Entry(entity).State = EntityState.Modified;
                }

            }
        }


        #endregion

        #region Remove
        public virtual void RemoveEquipo(Equipo equipo)
        {
            var entity = this._mapper.MapToEntity(equipo);
            var attachedEntity = _context.T_EQUIPO.Local
                .FirstOrDefault(w => w.CD_EQUIPO == entity.CD_EQUIPO);

            if (attachedEntity != null)
            {
                this._context.T_EQUIPO.Remove(attachedEntity);
            }
            else
            {
                this._context.T_EQUIPO.Attach(entity);
                this._context.T_EQUIPO.Remove(entity);
            }
        }
        #endregion



    }
}
