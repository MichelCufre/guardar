using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.Eventos;
using WIS.Domain.Extensions;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class EventoRepository
    {
        protected readonly WISDB _context;
        protected readonly string _application;
        protected readonly int _userId;
        protected readonly EventoMapper _mapper;
        protected readonly IDapper _dapper;

        public EventoRepository(WISDB context, string application, int userId, IDapper dapper)
        {
            this._context = context;
            this._application = application;
            this._userId = userId;
            this._mapper = new EventoMapper();
            this._dapper = dapper;
        }

        #region ANY

        public virtual bool AnyEvento(int nuEvento)
        {
            return _context.T_EVENTO
                .AsNoTracking()
                .Any(e => e.NU_EVENTO == nuEvento);
        }

        public virtual bool AnyInstancia(int nuInstancia)
        {
            return _context.T_EVENTO_INSTANCIA
                .AsNoTracking()
                .Any(e => e.NU_EVENTO_INSTANCIA == nuInstancia);
        }

        public virtual bool AnyInstancia(int nuEvento, string tpNotificacion, string cdPlantilla)
        {
            return _context.T_EVENTO_INSTANCIA
                .AsNoTracking()
                .Any(e => e.NU_EVENTO == nuEvento && e.TP_NOTIFICACION == tpNotificacion && e.CD_LABEL_ESTILO == cdPlantilla);
        }

        public virtual bool AnyInstanciaNotificacion(int nuInstancia)
        {
            return _context.T_EVENTO_NOTIFICACION
                .AsNoTracking()
                .Any(e => e.NU_EVENTO_INSTANCIA == nuInstancia);
        }

        public virtual bool AnyTemplate(int nuEvento, string tpNotificacion, string cdPlantilla)
        {
            return _context.T_EVENTO_TEMPLATE.Any(s => s.NU_EVENTO == nuEvento && s.TP_NOTIFICACION == tpNotificacion && s.CD_LABEL_ESTILO == cdPlantilla);
        }

        #endregion

        #region GET

        public virtual int GetNextNuEvento()
        {
            return this._context.GetNextSequenceValueInt(_dapper, "S_NU_EVENTO");
        }

        public virtual int GetNextNuInstancia()
        {
            return this._context.GetNextSequenceValueInt(_dapper, "S_NU_EVENTO_INSTANCIA");
        }

        public virtual List<Bandeja> GetBandejasInstanciasPendientes()
        {
            List<Bandeja> res = new List<Bandeja>();
            List<T_EVENTO_BANDEJA> lst = _context.T_EVENTO_BANDEJA
                .AsNoTracking()
                .Where(s => s.ND_ESTADO == EstadoBandeja.EST_PEND.ToString())
                .ToList();

            foreach (var ev in lst)
                res.Add(_mapper.MapBandejaToObject(ev));

            return res;
        }

        public virtual List<Evento> GetEventos()
        {
            List<T_EVENTO> entities = _context.T_EVENTO.AsNoTracking().ToList();
            List<Evento> objs = new List<Evento>();

            foreach (T_EVENTO entity in entities)
            {
                objs.Add(_mapper.MapEventoToObject(entity));
            }

            return objs;
        }

        public virtual Evento GetEvento(int nuEvento)
        {
            return this._mapper.MapEventoToObject(_context.T_EVENTO.AsNoTracking().Include("T_EVENTO_INSTANCIA").AsNoTracking().FirstOrDefault(w => w.NU_EVENTO == nuEvento));
        }

        public virtual Evento GetEvento(string nmEvento)
        {
            return this._mapper.MapEventoToObject(_context.T_EVENTO.AsNoTracking().FirstOrDefault(w => w.NM_EVENTO == nmEvento));
        }

        public virtual Instancia GetInstancia(int nuInstancia)
        {
            T_EVENTO_INSTANCIA entity = _context.T_EVENTO_INSTANCIA.FirstOrDefault(w => w.NU_EVENTO_INSTANCIA == nuInstancia);

            return entity == null ? null : _mapper.MapInstanciaToObject(entity);
        }

        public virtual ParametroInstancia GetParametroInstancia(int nuEvento, string CdEventoParametro, int NuInstancia, string tpNotificacion)
        {
            T_EVENTO_PARAMETRO_INSTANCIA entity = _context.T_EVENTO_PARAMETRO_INSTANCIA
                .FirstOrDefault(w => w.NU_EVENTO == nuEvento
                    && w.TP_NOTIFICACION == tpNotificacion
                    && w.NU_EVENTO_INSTANCIA == NuInstancia
                    && w.CD_EVENTO_PARAMETRO == CdEventoParametro);

            return entity == null ? null : _mapper.MapParametroInstanciaToObject(entity);
        }

        public virtual List<ParametroInstancia> GetParametrosInstancia(int NuInstancia)
        {
            return this._context.T_EVENTO_PARAMETRO_INSTANCIA
                .AsNoTracking()
                .Where(w => w.NU_EVENTO_INSTANCIA == NuInstancia)
                .Select(d => this._mapper.MapParametroInstanciaToObject(d, true))
                .ToList();
        }

        public virtual List<EventoTemplate> GeEventoTemplatoByEventoTipoNotificacion(int nuEvento, string tpNotificacion)
        {
            return this._context.T_EVENTO_TEMPLATE
                .AsNoTracking()
                .Where(e => e.NU_EVENTO == nuEvento
                    && e.TP_NOTIFICACION == tpNotificacion)
                .Select(d => this._mapper.MapEventoTemplateToObject(d, true))
                .ToList();
        }

        public virtual EventoTemplate GetEventoTemplate(int nuEvento, string tpNotificacion, string cdLabelEstilo)
        {
            return this._context.T_EVENTO_TEMPLATE
                .AsNoTracking()
                .Where(e => e.NU_EVENTO == nuEvento
                    && e.TP_NOTIFICACION == tpNotificacion
                    && e.CD_LABEL_ESTILO == cdLabelEstilo)
                .Select(d => this._mapper.MapEventoTemplateToObject(d, true))
                .FirstOrDefault();
        }

        public virtual List<Instancia> GetInstanciasHabilitadas(int nuEvento, string tpNotificacion, string cdParametro, string vlParametro)
        {
            return (from i in _context.T_EVENTO_INSTANCIA
                    join pi in _context.T_EVENTO_PARAMETRO_INSTANCIA on i.NU_EVENTO_INSTANCIA equals pi.NU_EVENTO_INSTANCIA
                    where i.NU_EVENTO == nuEvento
                     && i.TP_NOTIFICACION == tpNotificacion
                     && !string.IsNullOrEmpty(i.FL_HABILITADO)
                     && i.FL_HABILITADO.ToUpper() == "S"
                     && pi.CD_EVENTO_PARAMETRO == cdParametro
                     && pi.VL_PARAMETRO == vlParametro
                    select _mapper.MapInstanciaToObject(i, true)).AsNoTracking().ToList();
        }

        public virtual List<Instancia> GetInstanciasHabilitadas(string nmEvento, Dictionary<string, string> parametros)
        {
            var nuEvento = _context.T_EVENTO.FirstOrDefault(e => e.NM_EVENTO == nmEvento)?.NU_EVENTO ?? 0;
            var instancias = _context.T_EVENTO_INSTANCIA
                .Where(i => i.FL_HABILITADO == "S"
                    && i.NU_EVENTO == nuEvento);

            foreach (var cdParametro in parametros.Keys)
            {
                var vlParametro = parametros[cdParametro];

                if (!string.IsNullOrEmpty(vlParametro))
                {
                    var parametrosInstancias = _context.T_EVENTO_PARAMETRO_INSTANCIA
                        .Where(pi => pi.NU_EVENTO == nuEvento
                            && pi.CD_EVENTO_PARAMETRO == cdParametro
                            && (pi.VL_PARAMETRO == vlParametro
                                || string.IsNullOrEmpty(pi.VL_PARAMETRO)));

                    instancias = instancias
                        .Join(parametrosInstancias,
                            i => new { i.NU_EVENTO_INSTANCIA },
                            pi => new { pi.NU_EVENTO_INSTANCIA },
                            (i, pi) => i);
                }
            }

            return instancias
                .AsNoTracking()
                .Include("T_EVENTO_PARAMETRO_INSTANCIA")
                .Include("T_EVENTO_TEMPLATE")
                .Select(i => _mapper.MapInstanciaToObject(i, true))
                .ToList();
        }

        public virtual List<Instancia> GetInstanciasProgramadasHabilitadas()
        {
            return _context.T_EVENTO_INSTANCIA
               .Include("T_EVENTO")
               .Include("T_EVENTO_PARAMETRO_INSTANCIA")
               .Include("T_EVENTO_TEMPLATE")
               .Where(i => i.FL_HABILITADO == "S"
                   && i.T_EVENTO.FL_PROGRAMADO == "S")
               .Select(i => _mapper.MapInstanciaToObject(i, true))
               .ToList();
        }

        public virtual EjecucionInstancia GetEjecucionInstancia(int nuEventoInstancia)
        {
            var entity = _context.T_EVENTO_INSTANCIA_EJECUCION
                .FirstOrDefault(i => i.NU_EVENTO_INSTANCIA == nuEventoInstancia);

            return entity == null ? null : _mapper.MapToObject(entity);
        }


        #endregion

        #region ADD
        public virtual void AddInstancia(Instancia obj)
        {
            T_EVENTO_INSTANCIA entity = this._mapper.MapInstanciaToEntity(obj);
            this._context.T_EVENTO_INSTANCIA.Add(entity);
        }

        public virtual void AddEjecucionInstancia(EjecucionInstancia obj)
        {
            T_EVENTO_INSTANCIA_EJECUCION entity = this._mapper.MapToEntity(obj);
            this._context.T_EVENTO_INSTANCIA_EJECUCION.Add(entity);
        }

        public virtual void AddParametroInstancia(ParametroInstancia obj)
        {
            T_EVENTO_PARAMETRO_INSTANCIA entity = this._mapper.MapParametroInstanciaToEntity(obj);
            this._context.T_EVENTO_PARAMETRO_INSTANCIA.Add(entity);
        }

        public virtual void AddBandeja(Bandeja obj)
        {
            obj.NU_EVENTO_BANDEJA = _context.GetNextSequenceValueInt(_dapper, "S_NU_EVENTO_BANDEJA");
            T_EVENTO_BANDEJA entity = this._mapper.MapBandejaToEntity(obj);
            this._context.T_EVENTO_BANDEJA.Add(entity);
        }

        public virtual void AddBandejaInstancia(InstanciaBandeja obj)
        {
            obj.NU_EVENTO_BANDEJA_INSTANCIA = _context.GetNextSequenceValueInt(_dapper, "S_NU_EVENTO_BANDEJA_INSTANCIA");
            T_EVENTO_BANDEJA_INSTANCIA entity = this._mapper.MapInstanciaBandejaToEntity(obj);
            this._context.T_EVENTO_BANDEJA_INSTANCIA.Add(entity);
        }

        public virtual void AddTemplate(EventoTemplate obj)
        {
            T_EVENTO_TEMPLATE entity = this._mapper.MapEventoTemplateToEntity(obj, false);
            this._context.T_EVENTO_TEMPLATE.Add(entity);
        }

        #endregion

        #region UPDATE
        public virtual void UpdateEjecucionInstancia(EjecucionInstancia obj)
        {
            T_EVENTO_INSTANCIA_EJECUCION entity = this._mapper.MapToEntity(obj);

            T_EVENTO_INSTANCIA_EJECUCION attachedEntity = _context.T_EVENTO_INSTANCIA_EJECUCION.Local.FirstOrDefault(w => w.NU_EVENTO_INSTANCIA == entity.NU_EVENTO_INSTANCIA);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_EVENTO_INSTANCIA_EJECUCION.Attach(entity);
                _context.Entry<T_EVENTO_INSTANCIA_EJECUCION>(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateInstancia(Instancia obj)
        {
            T_EVENTO_INSTANCIA entity = this._mapper.MapInstanciaToEntity(obj);

            T_EVENTO_INSTANCIA attachedEntity = _context.T_EVENTO_INSTANCIA.Local.FirstOrDefault(w => w.NU_EVENTO_INSTANCIA == entity.NU_EVENTO_INSTANCIA);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_EVENTO_INSTANCIA.Attach(entity);
                _context.Entry<T_EVENTO_INSTANCIA>(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateBandeja(Bandeja obj)
        {
            T_EVENTO_BANDEJA entity = this._mapper.MapBandejaToEntity(obj);

            T_EVENTO_BANDEJA attachedEntity = _context.T_EVENTO_BANDEJA.Local.FirstOrDefault(w => w.NU_EVENTO_BANDEJA == entity.NU_EVENTO_BANDEJA);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_EVENTO_BANDEJA.Attach(entity);
                _context.Entry<T_EVENTO_BANDEJA>(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateInstanciaBandeja(InstanciaBandeja obj)
        {
            T_EVENTO_BANDEJA_INSTANCIA entity = this._mapper.MapInstanciaBandejaToEntity(obj);

            T_EVENTO_BANDEJA_INSTANCIA attachedEntity = _context.T_EVENTO_BANDEJA_INSTANCIA.Local.FirstOrDefault(w => w.NU_EVENTO_BANDEJA_INSTANCIA == entity.NU_EVENTO_BANDEJA_INSTANCIA);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_EVENTO_BANDEJA_INSTANCIA.Attach(entity);
                _context.Entry<T_EVENTO_BANDEJA_INSTANCIA>(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateParametroInstancia(ParametroInstancia obj)
        {
            T_EVENTO_PARAMETRO_INSTANCIA entity = this._mapper.MapParametroInstanciaToEntity(obj);

            T_EVENTO_PARAMETRO_INSTANCIA attachedEntity = _context.T_EVENTO_PARAMETRO_INSTANCIA.Local.FirstOrDefault(w => w.NU_EVENTO_INSTANCIA == entity.NU_EVENTO_INSTANCIA && w.CD_EVENTO_PARAMETRO == entity.CD_EVENTO_PARAMETRO);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_EVENTO_PARAMETRO_INSTANCIA.Attach(entity);
                _context.Entry<T_EVENTO_PARAMETRO_INSTANCIA>(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateTemplate(EventoTemplate obj)
        {
            T_EVENTO_TEMPLATE entity = this._mapper.MapEventoTemplateToEntity(obj);

            T_EVENTO_TEMPLATE attachedEntity = _context.T_EVENTO_TEMPLATE.Local
                .FirstOrDefault(w => w.NU_EVENTO == entity.NU_EVENTO && w.TP_NOTIFICACION == entity.TP_NOTIFICACION && w.CD_LABEL_ESTILO == entity.CD_LABEL_ESTILO);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_EVENTO_TEMPLATE.Attach(entity);
                _context.Entry<T_EVENTO_TEMPLATE>(entity).State = EntityState.Modified;
            }
        }

        #endregion

        #region DELETE

        public virtual void RemoveInstancia(Instancia instancia)
        {
            T_EVENTO_INSTANCIA entity = this._context.T_EVENTO_INSTANCIA
                .FirstOrDefault(x => x.NU_EVENTO_INSTANCIA == instancia.Id);

            T_EVENTO_INSTANCIA attachedEntity = _context.T_EVENTO_INSTANCIA.Local
                .FirstOrDefault(x => x.NU_EVENTO_INSTANCIA == instancia.Id);

            if (attachedEntity != null)
            {
                _context.T_EVENTO_INSTANCIA.Remove(attachedEntity);
            }
            else
            {
                _context.T_EVENTO_INSTANCIA.Attach(entity);
                _context.T_EVENTO_INSTANCIA.Remove(entity);
            }
        }

        public virtual void RemoveParametroInstancia(ParametroInstancia parametroInstancia)
        {
            T_EVENTO_PARAMETRO_INSTANCIA entity = this._mapper.MapParametroInstanciaToEntity(parametroInstancia);

            T_EVENTO_PARAMETRO_INSTANCIA attachedEntity = _context.T_EVENTO_PARAMETRO_INSTANCIA.Local
                .FirstOrDefault(w => w.NU_EVENTO_INSTANCIA == entity.NU_EVENTO_INSTANCIA
                && w.CD_EVENTO_PARAMETRO == entity.CD_EVENTO_PARAMETRO);

            if (attachedEntity != null)
            {
                _context.T_EVENTO_PARAMETRO_INSTANCIA.Remove(attachedEntity);
            }
            else
            {
                _context.T_EVENTO_PARAMETRO_INSTANCIA.Attach(entity);
                _context.T_EVENTO_PARAMETRO_INSTANCIA.Remove(entity);
            }
        }

        public virtual void RemoveTemplate(EventoTemplate plantilla)
        {
            T_EVENTO_TEMPLATE entity = this._context.T_EVENTO_TEMPLATE
               .FirstOrDefault(x => x.NU_EVENTO == plantilla.nuEvento && x.TP_NOTIFICACION == plantilla.TipoNotificacion.ToString() && x.CD_LABEL_ESTILO == plantilla.CdEstilo);
            T_EVENTO_TEMPLATE attachedEntity = _context.T_EVENTO_TEMPLATE.Local
                .FirstOrDefault(w => w.NU_EVENTO == plantilla.nuEvento && w.TP_NOTIFICACION == plantilla.TipoNotificacion.ToString() && w.CD_LABEL_ESTILO == plantilla.CdEstilo);

            if (attachedEntity != null)
                _context.T_EVENTO_TEMPLATE.Remove(attachedEntity);
            else
                _context.T_EVENTO_TEMPLATE.Attach(entity);
                _context.T_EVENTO_TEMPLATE.Remove(entity);
        }

        #endregion

    }
}
