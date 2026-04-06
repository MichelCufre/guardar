using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.Extensions;
using WIS.Domain.Recepcion;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class RecepcionTipoRepository
    {
        protected readonly WISDB _context;
        protected readonly string _cdAplicacion;
        protected readonly int _userId;
        protected readonly RecepcionTipoMapper _mapper;
        protected readonly IDapper _dapper;

        public RecepcionTipoRepository(WISDB context, string cdAplicacion, int userId, IDapper dapper)
        {
            this._context = context;
            this._cdAplicacion = cdAplicacion;
            this._userId = userId;
            this._mapper = new RecepcionTipoMapper();
            this._dapper = dapper;
        }

        #region Any

        public virtual bool AnyRecepcionTipoEmpresaHabilitado(int idEmpresa, string tipoAgente)
        {
            return this._context.T_RECEPCION_REL_EMPRESA_TIPO.Any(a => a.CD_EMPRESA == idEmpresa && a.T_RECEPCION_TIPO.TP_AGENTE == tipoAgente && a.FL_HABILITADO == "S");
        }

        public virtual bool GeneraReporte(string tipoRecepcionInterno, int idEmpresa)
        {
            return _context.T_RECEPCION_EMP_TIPO_REPORTE.AsNoTracking().Any(s => s.TP_RECEPCION_EXTERNO == tipoRecepcionInterno && s.CD_EMPRESA == idEmpresa);
        }

        public virtual bool AnyEmpresaRecepcionTipo(int idEmpresa, string tipoExterno)
        {
            return this._context.T_RECEPCION_REL_EMPRESA_TIPO.Any(e => e.CD_EMPRESA == idEmpresa && e.TP_RECEPCION_EXTERNO == tipoExterno);
        }

        #endregion

        #region Get

        public virtual RecepcionTipo GetRecepcionTipo(string tipo)
        {
            var entity = _context.T_RECEPCION_TIPO.FirstOrDefault(r => r.TP_RECEPCION == tipo);

            return this._mapper.MapToObject(entity);
        }

        public virtual RecepcionTipo GetRecepcionTipoByInterno(int idEmpresa, string tipoInterno)
        {
            var entity = this._context.T_RECEPCION_REL_EMPRESA_TIPO.Include("T_RECEPCION_TIPO")
                .Where(s => s.CD_EMPRESA == idEmpresa && s.TP_RECEPCION == tipoInterno)
                .Select(s => s.T_RECEPCION_TIPO).FirstOrDefault();

            return this._mapper.MapToObject(entity);
        }

        public virtual RecepcionTipo GetRecepcionTipoByExterno(int idEmpresa, string tipoExterno)
        {
            var entity = this._context.T_RECEPCION_REL_EMPRESA_TIPO.Include("T_RECEPCION_TIPO")
                .Where(s => s.CD_EMPRESA == idEmpresa && s.TP_RECEPCION_EXTERNO == tipoExterno)
                .Select(s => s.T_RECEPCION_TIPO).FirstOrDefault();

            return this._mapper.MapToObject(entity);
        }

        public virtual List<RecepcionTipo> GetRecepcionTiposConReportes()
        {
            return this._context.T_RECEPCION_TIPO.Include("T_RECEPCION_TIPO_REPORTE_DEF").Include("T_RECEPCION_TIPO_REPORTE_DEF.T_REPORTE_DEFINICION")
                    .Select(r => _mapper.MapToObject(r))
                    .ToList();
        }

        public virtual List<RecepcionTipo> GetTiposRecepcion()
        {
            return this._context.T_RECEPCION_TIPO
                .AsNoTracking()
                .Select(r => _mapper.MapToObject(r))
                .ToList();
        }

        public virtual EmpresaRecepcionTipo GetEmpresaRecepcionTipoById(int id)
        {
            var entity = this._context.T_RECEPCION_REL_EMPRESA_TIPO.Include("T_RECEPCION_TIPO")
                .AsNoTracking().FirstOrDefault(s => s.NU_RECEPCION_REL_EMPRESA_TIPO == id);

            return this._mapper.MapToObject(entity);
        }

        public virtual List<EmpresaRecepcionTipo> GetRecepcionTiposHabilitadosByEmpresa(int idEmpresa)
        {
            return this._context.T_RECEPCION_REL_EMPRESA_TIPO.Include("T_RECEPCION_TIPO")
                .Where(s => s.CD_EMPRESA == idEmpresa && s.FL_HABILITADO == "S")
                .Select(r => _mapper.MapToObject(r))
                .ToList();
        }

        public virtual EmpresaRecepcionTipo GetRecepcionTipoExterno(int idEmpresa, string tipoExterno)
        {
            var entity = this._context.T_RECEPCION_REL_EMPRESA_TIPO.Include("T_RECEPCION_TIPO")
                .FirstOrDefault(s => s.CD_EMPRESA == idEmpresa && s.TP_RECEPCION_EXTERNO == tipoExterno);

            return this._mapper.MapToObject(entity);

        }

        public virtual EmpresaRecepcionTipo GetRecepcionTipoExternoByInterno(int idEmpresa, string tipoInterno)
        {
            var entity = this._context.T_RECEPCION_REL_EMPRESA_TIPO.Include("T_RECEPCION_TIPO")
                .FirstOrDefault(s => s.CD_EMPRESA == idEmpresa && s.TP_RECEPCION == tipoInterno);

            return this._mapper.MapToObject(entity);

        }

        public virtual List<EmpresaRecepcionTipo> GetRecepcionTiposEmpresaHabilitados(int idEmpresa, string tipoAgente)
        {
            return this._context.T_RECEPCION_REL_EMPRESA_TIPO
                    .Include("T_RECEPCION_TIPO")
                    .Where(a => a.CD_EMPRESA == idEmpresa && a.T_RECEPCION_TIPO.TP_AGENTE == tipoAgente && a.FL_HABILITADO == "S")
                    .Select(r => _mapper.MapToObject(r))
                    .ToList();
        }

        #endregion

        #region Add

        public virtual void AddEmpresaRecepcionTipo(EmpresaRecepcionTipo empresaTipoRecepcion)
        {
            empresaTipoRecepcion.Id = this._context.GetNextSequenceValueInt(_dapper, "S_RECEPCION_REL_EMPRESA_TIPO");

            T_RECEPCION_REL_EMPRESA_TIPO entity = this._mapper.MapToEntity(empresaTipoRecepcion);

            this._context.T_RECEPCION_REL_EMPRESA_TIPO.Add(entity);

        }

        public virtual void AddEmpresaRecepcionTipoReporte(EmpresaRecepcionTipoReporte tipoRecepcionReporte)
        {
            tipoRecepcionReporte.Id = this._context.GetNextSequenceValueInt(_dapper, "S_NU_REC_EMP_TIPO_REP");

            T_RECEPCION_EMP_TIPO_REPORTE entity = this._mapper.MapToEntity(tipoRecepcionReporte);

            this._context.T_RECEPCION_EMP_TIPO_REPORTE.Add(entity);

        }

        #endregion

        #region Update
        public virtual void UpdateEmpresaRecepcionTipo(EmpresaRecepcionTipo empresa)
        {
            T_RECEPCION_REL_EMPRESA_TIPO entity = this._mapper.MapToEntity(empresa);

            this._context.T_RECEPCION_REL_EMPRESA_TIPO.Attach(entity);

            this._context.Entry(entity).State = EntityState.Modified;
        }

        #endregion

        #region Remove

        #endregion
    }
}
