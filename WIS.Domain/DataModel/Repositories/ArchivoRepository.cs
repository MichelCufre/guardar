using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.Eventos;
using WIS.Domain.Extensions;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
	public class ArchivoRepository
    {
        protected readonly WISDB _context;
        protected readonly string _cdAplicacion;
        protected readonly int _userId;
        protected readonly ArchivoMapper _mapper;
        protected readonly IDapper _dapper;

        public ArchivoRepository(WISDB context, string cdAplicacion, int userId, IDapper dapper)
        {
            this._context = context;
            this._cdAplicacion = cdAplicacion;
            this._userId = userId;
            this._mapper = new ArchivoMapper();
            this._dapper = dapper;
        }

        #region Any

        public virtual bool AnyArchivoAdjunto(long NU_ARCHIVO_ADJUNTO, int CD_EMPRESA, string CD_MANEJO, string DS_REFERENCIA)
        {
            return this._context.T_ARCHIVO_ADJUNTO.AsNoTracking().Any(w =>
                        w.NU_ARCHIVO_ADJUNTO == NU_ARCHIVO_ADJUNTO
                         && w.CD_EMPRESA == CD_EMPRESA
                         && w.CD_MANEJO == CD_MANEJO
                         && w.DS_REFERENCIA == DS_REFERENCIA
            );
        }
        
        public virtual bool AnyArchivoAdjunto(int CD_EMPRESA, string CD_MANEJO, string DS_REFERENCIA, string TP_DOCUMENTO)
        {
            return this._context.T_ARCHIVO_ADJUNTO.AsNoTracking().Any(w =>
                         w.CD_EMPRESA == CD_EMPRESA
                         && w.CD_MANEJO == CD_MANEJO
                          && w.TP_DOCUMENTO == TP_DOCUMENTO
                         && w.DS_REFERENCIA == DS_REFERENCIA
            );
        }
        
        public virtual bool AnyArchivoVersion(long NU_ARCHIVO_ADJUNTO, int CD_EMPRESA, string CD_MANEJO, string DS_REFERENCIA, long NU_VERSION)
        {
            return this._context.T_ARCHIVO_ADJUNTO_VERSION.AsNoTracking().Any(w =>
                        w.NU_ARCHIVO_ADJUNTO == NU_ARCHIVO_ADJUNTO
                         && w.CD_EMPRESA == CD_EMPRESA
                         && w.CD_MANEJO == CD_MANEJO
                         && w.DS_REFERENCIA == DS_REFERENCIA
                         && w.NU_VERSION == NU_VERSION
            );
        }

        public virtual bool GetCumpleCondicionEnvioArchivoConfirmado(List<long> nuarchivo, List<ParametroInstancia> parametros, out List<ArchivoAdjunto> Archivos)
        {
            var query = _context.T_ARCHIVO_ADJUNTO.AsNoTracking().Where(x => nuarchivo.Contains(x.NU_ARCHIVO_ADJUNTO));
            foreach (var p in parametros)
            {
                if (!string.IsNullOrEmpty(p.Valor))
                {
                    var array = p.Valor.Split(';');
                    var filtersIn = array.Where(s => s.First() != '!');
                    var filtersNotIn = new List<string>();
                    foreach (var n in array.Where(s => s.First() == '!').ToList())
                        filtersNotIn.Add(n.Remove(0, 1));
                    switch (p.Codigo)
                    {
                        case "CD_EMPRESA":
                            if (filtersIn.Any())
                                query = query.Where(s => filtersIn.Contains(s.CD_EMPRESA.ToString()));
                            if (filtersNotIn.Any())
                                query = query.Where(s => !filtersNotIn.Contains(s.CD_EMPRESA.ToString()));
                            break;
                    }
                }
            }

            Archivos = query.ToList().Select(w => this._mapper.Map(w)).ToList();

            return query.Any();
        }

        #endregion

        #region Get

        public virtual ArchivoAdjunto GetArchivoAdjunto(long NU_ARCHIVO_ADJUNTO, int CD_EMPRESA, string CD_MANEJO, string DS_REFERENCIA)
        {
            return _mapper.Map(
                    this._context.T_ARCHIVO_ADJUNTO.AsNoTracking()
                              .FirstOrDefault(w =>
                        w.NU_ARCHIVO_ADJUNTO == NU_ARCHIVO_ADJUNTO
                         && w.CD_EMPRESA == CD_EMPRESA
                         && w.CD_MANEJO == CD_MANEJO
                         && w.DS_REFERENCIA == DS_REFERENCIA
                   )
            );
        }

        public virtual ArchivoAdjunto GetArchivoAdjunto(long NU_ARCHIVO_ADJUNTO)
        {
            return _mapper.Map(
                    this._context.T_ARCHIVO_ADJUNTO.AsNoTracking()
                              .FirstOrDefault(w =>
                        w.NU_ARCHIVO_ADJUNTO == NU_ARCHIVO_ADJUNTO

                   )
            );
        }

        public virtual ArchivoManejo GetManejo(string cdManejo)
        {
            return _mapper.Map(
               this._context.T_ARCHIVO_MANEJO.AsNoTracking()
                         .FirstOrDefault(w =>
                   w.CD_MANEJO == cdManejo
              )
       );
        }

        public virtual List<ArchivoAdjunto> GetArchivoAdjuntos()
        {
            return this._context.T_ARCHIVO_ADJUNTO
                .AsNoTracking()
                .ToList()
                .Select(w => this._mapper.Map(w))
                .ToList();
        }

        public virtual List<ArchivoAdjunto> GetArchivoAdjuntoByKeysPartial(string valueSearch)
        {
            return this._context.T_ARCHIVO_ADJUNTO
                .AsNoTracking()
                .Where(w => w.NU_ARCHIVO_ADJUNTO.ToString().Contains(valueSearch.ToLower())
                    || w.CD_EMPRESA.ToString().Contains(valueSearch.ToLower())
                    || w.CD_MANEJO.ToLower().Contains(valueSearch.ToLower())
                    || w.DS_REFERENCIA.ToLower().Contains(valueSearch.ToLower()))
                .ToList()
                .Select(w => this._mapper.Map(w))
                .ToList();
        }

        public virtual ArchivoVersion GetUltimaVersion(ArchivoAdjunto archivo)
        {
            return _mapper.Map(
                    this._context.T_ARCHIVO_ADJUNTO_VERSION.AsNoTracking()
                              .FirstOrDefault(w =>
                        w.NU_ARCHIVO_ADJUNTO == archivo.NU_ARCHIVO_ADJUNTO
                         && w.CD_EMPRESA == archivo.CD_EMPRESA
                         && w.CD_MANEJO == archivo.CD_MANEJO
                         && w.DS_REFERENCIA == archivo.DS_REFERENCIA
                         && w.NU_VERSION == archivo.NU_VERSION_ACTIVA
                   )
            );
        }

        public virtual List<ArchivoVersion> GetArchivoVersions()
        {
            return this._context.T_ARCHIVO_ADJUNTO_VERSION.AsNoTracking().ToList()
                   .Select(w => this._mapper.Map(w)).ToList();

        }

        public virtual List<ArchivoVersion> GetArchivoVersionByKeysPartial(string valueSearch)
        {
            return this._context.T_ARCHIVO_ADJUNTO_VERSION
                .AsNoTracking()
                .Where(w => w.NU_ARCHIVO_ADJUNTO.ToString().Contains(valueSearch.ToLower())
                    || w.CD_EMPRESA.ToString().Contains(valueSearch.ToLower())
                    || w.CD_MANEJO.ToLower().Contains(valueSearch.ToLower())
                    || w.DS_REFERENCIA.ToLower().Contains(valueSearch.ToLower())
                    || w.NU_VERSION.ToString().Contains(valueSearch.ToLower()))
                .ToList()
                .Select(w => this._mapper.Map(w))
                .ToList();
        }

        public virtual ArchivoVersion GetArchivoVersion(ArchivoAdjunto a)
        {
            T_ARCHIVO_ADJUNTO_VERSION version = this._context.T_ARCHIVO_ADJUNTO_VERSION
                .FirstOrDefault(x => x.NU_ARCHIVO_ADJUNTO == a.NU_ARCHIVO_ADJUNTO
                    && x.NU_VERSION == a.NU_VERSION_ACTIVA);

            return this._mapper.Map(version);
        }

        public virtual List<ArchivoDocumento> GetTipoDocumento()
        {
            return this._context.T_ARCHIVO_DOCUMENTO.AsNoTracking()
                    .ToList()
                    .Select(w => this._mapper.Map(w)).ToList();

        }

        public virtual List<ArchivoDocumento> GetTipoDocumentoByManejo(string tipoManejo)
        {
            return this._context.T_ARCHIVO_MANEJO.Include("T_ARCHIVO_DOCUMENTO").AsNoTracking()
                    .FirstOrDefault(w => w.CD_MANEJO == tipoManejo)
                     ?.T_ARCHIVO_MANEJO_DOCUMENTO
                    .ToList()
                    .Select(w => this._mapper.Map(w.T_ARCHIVO_DOCUMENTO)).ToList();

        }

        #endregion

        #region Add

        public virtual void AddArchivoAdjunto(ArchivoAdjunto archivo, ArchivoVersion version)
        {
            archivo.DT_ADDROW = DateTime.Now;
            archivo.NU_ARCHIVO_ADJUNTO = _context.GetNextSequenceValueLong(_dapper, "S_NU_ARCHIVO_ADJUNTO");

            archivo.NU_VERSION_ACTIVA = archivo.NU_VERSION_ACTIVA++;

            T_ARCHIVO_ADJUNTO entity = this._mapper.Map(archivo);
            this._context.T_ARCHIVO_ADJUNTO.Add(entity);

            this.AddArchivoVersion(archivo, version);

        }

        public virtual void AddArchivoVersion(ArchivoAdjunto archivo, ArchivoVersion version)
        {
            if (string.IsNullOrEmpty(version.TP_ARCHIVO)) return;

            version.LK_RUTA = $"{version.SUB_LINK}\\{archivo.CD_EMPRESA}#{archivo.DS_REFERENCIA}\\{archivo.GetNombreArchivo()}.{version.TP_ARCHIVO.ToLower()}";
            version.DT_ADDROW = DateTime.Now;
            version.NU_VERSION = archivo.NU_VERSION_ACTIVA;
            version.TP_ARCHIVO = $"ARCTP{version.TP_ARCHIVO.ToUpper()}";
            T_ARCHIVO_ADJUNTO_VERSION entityVersion = this._mapper.Map(archivo, version);
            this._context.T_ARCHIVO_ADJUNTO_VERSION.Add(entityVersion);
        }

        #endregion

        #region Update

        public virtual void UpdateArchivoAdjunto(ArchivoAdjunto archivo, ArchivoVersion version)
        {
            archivo.DT_UPDROW = DateTime.Now;

            if (version != null)
            {
                if (!string.IsNullOrEmpty(version.TP_ARCHIVO))
                    archivo.NU_VERSION_ACTIVA++;
            }

            var entity = this._mapper.Map(archivo);
            var attachedEntity = _context.T_ARCHIVO_ADJUNTO.Local
                .FirstOrDefault(w => w.NU_ARCHIVO_ADJUNTO == entity.NU_ARCHIVO_ADJUNTO
                    && w.CD_EMPRESA == entity.CD_EMPRESA
                    && w.CD_MANEJO == entity.CD_MANEJO
                    && w.DS_REFERENCIA == entity.DS_REFERENCIA);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.T_ARCHIVO_ADJUNTO.Attach(entity);
                this._context.Entry(entity).State = EntityState.Modified;
            }

            if (version != null)
            {
                this.AddArchivoVersion(archivo, version);
            }
        }

        #endregion

        #region Remove

        public virtual void RemoveArchivoAdjunto(ArchivoAdjunto obj)
        {
            var entity = this._mapper.Map(obj);
            var attachedEntity = _context.T_ARCHIVO_ADJUNTO.Local
                .FirstOrDefault(w => w.NU_ARCHIVO_ADJUNTO == entity.NU_ARCHIVO_ADJUNTO
                    && w.CD_EMPRESA == entity.CD_EMPRESA
                    && w.CD_MANEJO == entity.CD_MANEJO
                    && w.DS_REFERENCIA == entity.DS_REFERENCIA);

            if (attachedEntity != null)
            {
                this._context.T_ARCHIVO_ADJUNTO.Remove(attachedEntity);
            }
            else
            {
                _context.T_ARCHIVO_ADJUNTO.Attach(entity);
                _context.T_ARCHIVO_ADJUNTO.Remove(entity);
            }
        }

        public virtual List<string> DeleteArchivo(long nuArchivoAdjunto, string ruta)
        {
            List<string> listRutas = new List<string>();


            // _context.T_ARCHIVO_ADJUNTO_VERSION.RemoveRange(_context.T_ARCHIVO_ADJUNTO_VERSION.Where(w => w.NU_ARCHIVO_ADJUNTO == nuArchivoAdjunto));
            //   _context.T_ARCHIVO_ADJUNTO.Remove(_context.T_ARCHIVO_ADJUNTO.FirstOrDefault(w=> w.NU_ARCHIVO_ADJUNTO == nuArchivoAdjunto));

            foreach (var item in _context.T_ARCHIVO_ADJUNTO_VERSION.Where(w => w.NU_ARCHIVO_ADJUNTO == nuArchivoAdjunto))
            {
                _context.T_ARCHIVO_ADJUNTO_VERSION.Remove(item);
                string R = ruta + item.LK_RUTA;
                listRutas.Add(R);
            }
            _context.T_ARCHIVO_ADJUNTO.Remove(_context.T_ARCHIVO_ADJUNTO.FirstOrDefault(w => w.NU_ARCHIVO_ADJUNTO == nuArchivoAdjunto));
            return listRutas;

        }

        #endregion

    }
}
