using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.General;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class CodigoMultidatoRepository
    {
        protected readonly WISDB _context;
        protected readonly string _application;
        protected readonly int _userId;
        protected readonly IDapper _dapper;
        protected readonly CodigoMultidatoMapper _mapper;

        public CodigoMultidatoRepository(WISDB context, string application, int userId, IDapper dapper)
        {
            this._context = context;
            this._application = application;
            this._userId = userId;
            this._dapper = dapper;
            this._mapper = new CodigoMultidatoMapper();
        }

        #region Any
        public virtual bool ExisteCampoAplicacion(string campo, string aplicacion)
        {
            return this._context.V_APLICACION_CAMPO
                .AsNoTracking()
                .Any(x => x.CD_CAMPO == campo && x.CD_APLICACION == aplicacion);
        }

        public virtual bool ExisteDetalleCodigoMultidatoEmpresa(int empresa, string codigoMultidato, string codigoAplicacion, string codigoCampo, string codigoAI)
        {
            return this._context.V_CODIGO_MULTIDATO_EMPRESA_DET
                .AsNoTracking()
                .Any(x => x.CD_EMPRESA == empresa
                    && x.CD_CODIGO_MULTIDATO == codigoMultidato
                    && x.CD_APLICACION == codigoAplicacion
                    && x.CD_CAMPO == codigoCampo
                    && x.CD_AI == codigoAI);
        }

        public virtual bool ExisteDetalleCodigoMultidatoEmpresa(int empresa, string codigoMultidato)
        {
            return this._context.V_CODIGO_MULTIDATO_EMPRESA_DET
                .AsNoTracking()
                .Any(x => x.CD_EMPRESA == empresa
                    && x.CD_CODIGO_MULTIDATO == codigoMultidato);
        }

        #endregion

        #region Get

        public virtual List<Aplicacion> GetAplicacionesHabilitadaByDescriptionOrCodePartial(string value)
        {
            var aplicacionHabilitadas = this._context.V_APLICACION_CAMPO
                .AsNoTracking()
                .Where(x => x.FL_CODIGO_MULTIDATO == "S")
                .Select(x => x.CD_APLICACION)
                .Distinct()
                .ToList();

            return this._context.V_APLICACION
                .AsNoTracking()
                .Where(x => (x.CD_APLICACION.ToUpper().Contains(value.ToUpper())
                    || x.DS_APLICACION.ToUpper().Contains(value.ToUpper()))
                    && aplicacionHabilitadas.Contains(x.CD_APLICACION))
                .ToList()
                .Select(y => this._mapper.MapToObject(y))
                .ToList();
        }

        public virtual List<General.CodigoMultidato> GetCodigoMultiByDescriptionOrCodePartial(string value)
        {
            return this._context.V_CODIGO_MULTIDATO
                .AsNoTracking()
                .Where(x => x.CD_CODIGO_MULTIDATO.ToUpper().Contains(value.ToUpper())
                    || x.DS_CODIGO_MULTIDATO.ToUpper().Contains(value.ToUpper()))
                .ToList()
                .Select(y => this._mapper.MapToObject(y))
                .ToList(); ;
        }

        public virtual List<AplicacionCampo> GetCampoHabilitadoByDescriptionOrCodePartial(string aplicacion, string value, int cdEmpresa, string cdCodigoMultidato)
        {
            return this._context.V_APLICACION_CAMPO
                .AsNoTracking()
                .Where(x => (x.CD_CAMPO.ToUpper().Contains(value.ToUpper())
                    || x.DS_CAMPO.ToUpper().Contains(value.ToUpper()))
                    && x.FL_CODIGO_MULTIDATO == "S"
                    && x.CD_APLICACION == aplicacion)
                .ToList()
                .Select(y => this._mapper.MapToObject(y))
                .ToList();
        }

        public virtual short ObtenerProximoNuOrden(int codigoEmpresa, string codigoMultidato, string codigoAplicacion, string codigoCampo)
        {
            var ultimoNuOrden = _context.V_CODIGO_MULTIDATO_EMPRESA_DET
                .Where(x => x.CD_EMPRESA == codigoEmpresa
                         && x.CD_CODIGO_MULTIDATO == codigoMultidato
                         && x.CD_APLICACION == codigoAplicacion
                         && x.CD_CAMPO == codigoCampo)
                .Select(x => (short?)x.QT_AIS)
                .FirstOrDefault() ?? 0;

            return (short)(ultimoNuOrden + 1);
        }

        public virtual List<DetalleCodigoMultidato> GetCodigoAiByDescriptionOrCodePartial(string codigoMultidato, string value)
        {
            return this._context.V_CODIGO_MULTIDATO_DET
               .AsNoTracking()
               .Where(x => (x.CD_AI.ToUpper().Contains(value.ToUpper())
                   || x.DS_AI.ToUpper().Contains(value.ToUpper()))
                   && x.CD_CODIGO_MULTIDATO == codigoMultidato)
               .ToList()
               .Select(y => this._mapper.MapToObject(y))
               .ToList();
        }

        public virtual General.CodigoMultidato GetCodigoMultidato(string codigo)
        {
            var entity = _context.T_CODIGO_MULTIDATO
                .AsNoTracking()
                .FirstOrDefault(x => x.CD_CODIGO_MULTIDATO == codigo);

            return this._mapper.MapToObject(entity);
        }

        public virtual Aplicacion GetAplicacion(string codigo)
        {
            var entity = _context.V_APLICACION
                .AsNoTracking()
                .FirstOrDefault(x => x.CD_APLICACION == codigo);

            return this._mapper.MapToObject(entity);
        }

        public virtual AplicacionCampo GetCampo(string aplicacion, string codigo)
        {
            var entity = _context.V_APLICACION_CAMPO
                .AsNoTracking()
                .FirstOrDefault(x => x.CD_APLICACION == aplicacion && x.CD_CAMPO == codigo);

            return this._mapper.MapToObject(entity);
        }

        public virtual DetalleCodigoMultidato GetCodigoAi(string codigoMultidato, string codigoAi)
        {
            var entity = _context.V_CODIGO_MULTIDATO_DET
                .AsNoTracking()
                .FirstOrDefault(x => x.CD_CODIGO_MULTIDATO == codigoMultidato && x.CD_AI == codigoAi);

            return this._mapper.MapToObject(entity);
        }

        public virtual CodigoMultidatoEmpresaDetalle GetDetalleCodigoMultidatoEmpresa(int empresa, string codigoMultidato, string aplicacion, string campo, string codigoAI)
        {
            var entity = _context.T_CODIGO_MULTIDATO_EMPRESA_DET
                .AsNoTracking()
                .FirstOrDefault(d => d.CD_EMPRESA == empresa
                    && d.CD_CODIGO_MULTIDATO == codigoMultidato
                    && d.CD_APLICACION == aplicacion
                    && d.CD_CAMPO == campo
                    && d.CD_AI == codigoAI);

            return this._mapper.MapToObject(entity);
        }

        public virtual CodigoMultidatoEmpresaDetalle GetDetalleCodigoMultidatoEmpresaCambiarOrden(int empresa, string codigoMultidato, string aplicacion, string campo, short numeroOrden)
        {
            var entity = _context.T_CODIGO_MULTIDATO_EMPRESA_DET
                .AsNoTracking()
                .FirstOrDefault(d => d.CD_EMPRESA == empresa
                    && d.CD_CODIGO_MULTIDATO == codigoMultidato
                    && d.CD_APLICACION == aplicacion
                    && d.CD_CAMPO == campo
                    && d.NU_ORDEN == numeroOrden);

            return this._mapper.MapToObject(entity);
        }

        public virtual List<CodigoMultidatoEmpresaDetalle> GetDetalleCodigoMultidatoEmpresaPosteriores(CodigoMultidatoEmpresaDetalle detalleCodigoMultidatoEmpresa)
        {
            return _context.T_CODIGO_MULTIDATO_EMPRESA_DET
                .AsNoTracking()
                .Where(d => d.CD_EMPRESA == detalleCodigoMultidatoEmpresa.CodigoEmpresa
                    && d.CD_CODIGO_MULTIDATO == detalleCodigoMultidatoEmpresa.CodigoMultidato
                    && d.CD_APLICACION == detalleCodigoMultidatoEmpresa.CodigoAplicacion
                    && d.CD_CAMPO == detalleCodigoMultidatoEmpresa.CodigoCampo
                    && d.NU_ORDEN > detalleCodigoMultidatoEmpresa.NumeroOrden
                    )
                .Select(d => this._mapper.MapToObject(d))
                .ToList();
        }

        public virtual CodigoMultidatoEmpresa GetCodigoMultidatoEmpresa(int empresa, string codigoMultidato)
        {
            var entity = _context.T_CODIGO_MULTIDATO_EMPRESA
                .AsNoTracking()
                .FirstOrDefault(d => d.CD_EMPRESA == empresa
                    && d.CD_CODIGO_MULTIDATO == codigoMultidato);

            return this._mapper.MapToObject(entity);
        }

        public virtual List<CodigoMultidatoEmpresaDetalle> GetDetallesCodigoMultidatoEmpresaPorDefecto(int empresa, string codigoMultidato)
        {
            return _context.T_CODIGO_MULTIDATO_APLICACION
                .AsNoTracking()
                .Where(d => d.CD_CODIGO_MULTIDATO == codigoMultidato)
                .Select(d => this._mapper.MapToObject(empresa, d))
                .ToList();
        }

        public virtual List<CodigoMultidatoEmpresaDetalle> GetDetallesCodigoMultidatoEmpresa(int empresa, string codigoMultidato, string aplicacion)
        {
            return _context.T_CODIGO_MULTIDATO_EMPRESA_DET
                .AsNoTracking()
                .Where(d => d.CD_CODIGO_MULTIDATO == codigoMultidato
                    && d.CD_EMPRESA == empresa
                    && d.CD_APLICACION == aplicacion)
                .OrderBy(d => d.CD_CAMPO)
                .ThenBy(d => d.NU_ORDEN)
                .Select(d => _mapper.MapToObject(d))
                .ToList();
        }

        public virtual List<CodigoMultidatoEmpresaDetalle> GetDetallesCodigoMultidatoEmpresa(int empresa, string codigoMultidato)
        {
            return _context.T_CODIGO_MULTIDATO_EMPRESA_DET
                .AsNoTracking()
                .Where(d => d.CD_CODIGO_MULTIDATO == codigoMultidato
                    && d.CD_EMPRESA == empresa)
                .OrderBy(d => d.CD_CAMPO)
                .ThenBy(d => d.NU_ORDEN)
                .Select(d => _mapper.MapToObject(d))
                .ToList();
        }

        public virtual List<DetalleCodigoMultidato> GetDetallesCodigoMultidatoEmpresa(string cdCodigoMultidato)
        {
            return _context.T_CODIGO_MULTIDATO_DET
                .AsNoTracking()
                .Where(d => d.CD_CODIGO_MULTIDATO == cdCodigoMultidato)
                .Select(d => _mapper.MapToObject(d))
                .ToList();
        }

        public virtual int? GetFirstEmpresaHabilitada(string codigoMultidato, int userId, out int cantidad)
        {
            var empresas = _context.T_CODIGO_MULTIDATO_EMPRESA
                    .Join(_context.T_EMPRESA_FUNCIONARIO,
                        c => new { c.CD_EMPRESA },
                        ef => new { ef.CD_EMPRESA },
                        (c, ef) => new { Codigo = c, EmpresaFuncionario = ef})
                    .Where(x => x.Codigo.CD_CODIGO_MULTIDATO == codigoMultidato
                        && x.Codigo.FL_HABILITADO == "S"
                        && x.EmpresaFuncionario.USERID == userId)
                    .GroupBy(c => new { c.Codigo.CD_EMPRESA })
                    .OrderBy(x => x.Key.CD_EMPRESA)
                    .AsNoTracking()
                    .Select(x => x.Key.CD_EMPRESA);

            cantidad = empresas.Count();

            return empresas.FirstOrDefault();
        }

        #endregion

        #region Add

        public virtual void AddDetalleCodigoMultidatoEmpresa(CodigoMultidatoEmpresaDetalle detalle)
        {
            T_CODIGO_MULTIDATO_EMPRESA_DET entity = this._mapper.MapToEntity(detalle);
            this._context.T_CODIGO_MULTIDATO_EMPRESA_DET.Add(entity);
        }

        public virtual void AddCodigoMultidatoEmpresa(CodigoMultidatoEmpresa asociacionCodigoEmpresa)
        {
            T_CODIGO_MULTIDATO_EMPRESA entity = this._mapper.MapToEntity(asociacionCodigoEmpresa);
            this._context.T_CODIGO_MULTIDATO_EMPRESA.Add(entity);
        }

        #endregion

        #region Remove

        public virtual void DeleteDetalleCodigoMultidatoEmpresa(CodigoMultidatoEmpresaDetalle detalle)
        {
            var entity = this._mapper.MapToEntity(detalle);
            var attachedEntity = this._context.T_CODIGO_MULTIDATO_EMPRESA_DET.Local
                .FirstOrDefault(d => d.CD_EMPRESA == detalle.CodigoEmpresa
                    && d.CD_CODIGO_MULTIDATO == detalle.CodigoMultidato
                    && d.CD_APLICACION == detalle.CodigoAplicacion
                    && d.CD_CAMPO == detalle.CodigoCampo
                    && d.CD_AI == detalle.CodigoAI);

            if (attachedEntity != null)
            {
                this._context.T_CODIGO_MULTIDATO_EMPRESA_DET.Remove(attachedEntity);
            }
            else
            {
                this._context.T_CODIGO_MULTIDATO_EMPRESA_DET.Attach(entity);
                this._context.T_CODIGO_MULTIDATO_EMPRESA_DET.Remove(entity);
            }
        }

        public virtual void RemoveCodigoMultidatoEmpresa(CodigoMultidatoEmpresa value)
        {
            var entity = this._mapper.MapToEntity(value);
            var attachedEntity = this._context.T_CODIGO_MULTIDATO_EMPRESA.Local
                .FirstOrDefault(d => d.CD_EMPRESA == value.CodigoEmpresa
                    && d.CD_CODIGO_MULTIDATO == value.CodigoMultidato);

            if (attachedEntity != null)
            {
                this._context.T_CODIGO_MULTIDATO_EMPRESA.Remove(attachedEntity);
            }
            else
            {
                this._context.T_CODIGO_MULTIDATO_EMPRESA.Attach(entity);
                this._context.T_CODIGO_MULTIDATO_EMPRESA.Remove(entity);
            }
        }


        #endregion

        #region Update

        public virtual void UpdateDetalleCodigoMultidatoEmpresa(CodigoMultidatoEmpresaDetalle detalle)
        {
            var entity = this._mapper.MapToEntity(detalle);
            var attachedEntity = this._context.T_CODIGO_MULTIDATO_EMPRESA_DET.Local
                .FirstOrDefault(d => d.CD_EMPRESA == detalle.CodigoEmpresa
                    && d.CD_CODIGO_MULTIDATO == detalle.CodigoMultidato
                    && d.CD_APLICACION == detalle.CodigoAplicacion
                    && d.CD_CAMPO == detalle.CodigoCampo
                    && d.CD_AI == detalle.CodigoAI);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_CODIGO_MULTIDATO_EMPRESA_DET.Attach(entity);
                _context.Entry<T_CODIGO_MULTIDATO_EMPRESA_DET>(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateCodigoMultidatoEmpresa(CodigoMultidatoEmpresa value)
        {
            var entity = this._mapper.MapToEntity(value);
            var attachedEntity = this._context.T_CODIGO_MULTIDATO_EMPRESA.Local
                .FirstOrDefault(d => d.CD_EMPRESA == value.CodigoEmpresa
                    && d.CD_CODIGO_MULTIDATO == value.CodigoMultidato);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_CODIGO_MULTIDATO_EMPRESA.Attach(entity);
                _context.Entry<T_CODIGO_MULTIDATO_EMPRESA>(entity).State = EntityState.Modified;
            }
        }

        #endregion
    }
}
