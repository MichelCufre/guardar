using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.Extensions;
using WIS.Domain.Parametrizacion;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class AtributoRepository
    {
        protected WISDB _context;
        protected string _application;
        protected int _userId;
        protected AtributoMapper _mapper;
        protected readonly IDapper _dapper;

        public AtributoRepository(WISDB context, string application, int userid, IDapper dapper)
        {
            this._dapper = dapper;
            this._userId = userid;
            this._context = context;
            this._application = application;
            this._mapper = new AtributoMapper();
        }

        #region Any
        public virtual bool AnyFormatoFecha(string formato)
        {
            return this._context.V_PAR400_MASCARA_FECHA
                .AsNoTracking()
                .Any(x => x.VL_MASCARA == formato);
        }

        public virtual bool AnyFormatoHora(string formato)
        {
            return this._context.V_PAR400_MASCARA_HORA
                .AsNoTracking()
                .Any(x => x.VL_MASCARA_HORA == formato);
        }

        public virtual bool AnyValidacionAsociadaAtributo(int numeroAtributo)
        {
            return this._context.V_PAR400_ATRIBUTOS_VALIDACION_ASOCIADA
                .AsNoTracking()
                .Any(x => x.ID_ATRIBUTO == numeroAtributo);
        }

        public virtual bool AnyLpnTipoAsociadaAtributo(int numeroAtributo)
        {
            return this._context.V_PAR400_ATRIBUTOS_LPN_TIPO
                .AsNoTracking()
                .Any(x => x.ID_ATRIBUTO == numeroAtributo);
        }

        public virtual bool AnyLpnTipoAtributo(string tipoLpn)
        {
            return this._context.T_LPN_TIPO_ATRIBUTO.Any(x => x.TP_LPN_TIPO == tipoLpn);
        }

        public virtual bool AnyLpnTipoAtributoDetalle(string tipoLpn)
        {
            return this._context.T_LPN_TIPO_ATRIBUTO_DET.Any(x => x.TP_LPN_TIPO == tipoLpn);
        }

        public virtual bool AnyAtributoTipoLpnDet(string tipoLpn, int atributo)
        {
            return this._context.T_LPN_TIPO_ATRIBUTO_DET.AsNoTracking().Any(x => x.TP_LPN_TIPO == tipoLpn && x.ID_ATRIBUTO == atributo);
        }

        public virtual bool AnyLpnTipoAtributo(string tipoLpn, int atributo)
        {
            return this._context.T_LPN_TIPO_ATRIBUTO.AsNoTracking().Any(x => x.TP_LPN_TIPO == tipoLpn && x.ID_ATRIBUTO == atributo);
        }

        public virtual bool AnyAtributoNombre(string nombreAtributo, int? idAtributo = null)
        {
            return _context.T_ATRIBUTO
                .AsNoTracking()
                .Any(a => a.NM_ATRIBUTO.ToUpper() == nombreAtributo.ToUpper()
                && (idAtributo.HasValue ? a.ID_ATRIBUTO != idAtributo.Value : true));
        }

        #endregion

        #region Get

        public virtual List<Atributo> GetAtributos()
        {
            return _context.T_ATRIBUTO
                .AsNoTracking()
                .Select(a => _mapper.MapToObject(a))
                .ToList();
        }

        public virtual List<AtributoDisponible> GetAtributosDisponibles(string tipoLpn)
        {
            return _context.V_PAR401_ASOCIAR_ATRIBUTO_TIPO
                .AsNoTracking()
                .Where(x => x.TP_LPN_TIPO == tipoLpn)
                .Select(a => _mapper.MapToObject(a))
                .ToList();
        }

        public virtual List<AtributoDisponible> GetAtributosDisponiblesDetalle(string tipoLpn)
        {
            return _context.V_PAR401_ASOCIAR_ATRIBUTO_TIPO_DET
                .AsNoTracking()
                .Where(x => x.TP_LPN_TIPO == tipoLpn)
                .Select(a => _mapper.MapToObject(a))
                .ToList();
        }

        public virtual string GetIdAtributoTipo(int idAtributo)
        {
            return this._context.T_ATRIBUTO.AsNoTracking().FirstOrDefault(x => x.ID_ATRIBUTO == idAtributo).ID_ATRIBUTO_TIPO;
        }

        public virtual Atributo GetAtributo(int idAtributo)
        {
            var atributo = this._context.T_ATRIBUTO.AsNoTracking().FirstOrDefault(x => x.ID_ATRIBUTO == idAtributo);
            return this._mapper.MapToObject(atributo);
        }

        public virtual int GetCantidadAtributoTipo(string tipoLpn)
        {
            return (this._context.T_LPN_TIPO_ATRIBUTO.AsNoTracking().Where(x => x.TP_LPN_TIPO == tipoLpn).Count() + 1);
        }

        public virtual string GetDescripcionAtributoTipo(string idTipo)
        {
            return _context.T_ATRIBUTO_TIPO.FirstOrDefault(x => x.ID_ATRIBUTO_TIPO == idTipo)?.DS_ATRIBUTO_TIPO;
        }

        public virtual List<string> GetAllFormatosHora()
        {
            return _context.V_PAR400_MASCARA_HORA
                .Select(r => r.VL_MASCARA_HORA)
                .OrderBy(m => m)
                .ToList();
        }

        public virtual List<string> GetAllFormatosFecha()
        {
            return _context.V_PAR400_MASCARA_FECHA
                .Select(r => r.VL_MASCARA)
                .OrderBy(m => m)
                .ToList();
        }

        public virtual List<AtributoTipo> GetAllTipoAtributos()
        {
            return _context.T_ATRIBUTO_TIPO
                .AsNoTracking()
                .OrderBy(a => a.DS_ATRIBUTO_TIPO)
                .Select(a => _mapper.MapToObject(a))
                .ToList();
        }

        public virtual List<AtributoSistema> GetAllAtributoSistema()
        {
            return _context.T_ATRIBUTO_SISTEMA
                .AsNoTracking()
                .Select(a => _mapper.MapToObject(a))
                .ToList();
        }

        public virtual AtributoTipo GetTipoAtributoById(string id)
        {
            return this._mapper.MapToObject(this._context.T_ATRIBUTO_TIPO
                .AsNoTracking()
                .FirstOrDefault(x => x.ID_ATRIBUTO_TIPO == id));
        }

        public virtual AtributoTipo GetTipoAtributoByDescription(string descripcion)
        {
            return this._mapper.MapToObject(this._context.T_ATRIBUTO_TIPO
                .AsNoTracking()
                .FirstOrDefault(x => x.DS_ATRIBUTO_TIPO == descripcion));
        }

        public virtual AtributoValidacion GetAtributoValidacion(int id)
        {
            T_ATRIBUTO_VALIDACION entities = this._context.T_ATRIBUTO_VALIDACION.FirstOrDefault(x => x.ID_VALIDACION == id);
            if (entities == null)
                return null;
            return _mapper.MapToObject(entities);
        }

        public virtual List<AtributoValidacionAsociada> GetValidacionesAsociada(int idatributo)
        {
            List<AtributoValidacionAsociada> validacionesAsociadas = new List<AtributoValidacionAsociada>();
            List<T_ATRIBUTO_VALIDACION_ASOCIADA> lista = this._context.T_ATRIBUTO_VALIDACION_ASOCIADA.AsNoTracking().Where(x => x.ID_ATRIBUTO == idatributo).ToList();
            foreach (var item in lista)
            {
                validacionesAsociadas.Add(_mapper.MapToObject(item));
            }
            return validacionesAsociadas;
        }

        public virtual List<short> GetIdValidacionesAsociada(int idatributo)
        {
            List<short> ListaValidacionAsociada = new List<short>();
            List<T_ATRIBUTO_VALIDACION_ASOCIADA> lista = this._context.T_ATRIBUTO_VALIDACION_ASOCIADA.AsNoTracking().Where(x => x.ID_ATRIBUTO == idatributo).ToList();
            foreach (var item in lista)
            {
                ListaValidacionAsociada.Add(item.ID_VALIDACION);
            }
            return ListaValidacionAsociada;
        }

        public virtual AtributoValidacionAsociada GetAtributoValidacionAsociada(int idValidacion, int idAtributo)
        {
            T_ATRIBUTO_VALIDACION_ASOCIADA entities = this._context.T_ATRIBUTO_VALIDACION_ASOCIADA.FirstOrDefault(x => x.ID_VALIDACION == idValidacion && x.ID_ATRIBUTO == idAtributo);
            if (entities == null)
                return null;
            return _mapper.MapToObject(entities);
        }

        public virtual List<AtributoEstado> GetEstados()
        {
            return _context.T_ATRIBUTO_ESTADO
                .Select(a => _mapper.MapToObject(a))
                .ToList();
        }
        #endregion

        #region Add

        public virtual void AddAtributo(Atributo atributo)
        {
            T_ATRIBUTO entity = this._mapper.MapToEntity(atributo);
            entity.ID_ATRIBUTO = _context.GetNextSequenceValueInt(_dapper, "S_LPN_ATRIBUTO");
            this._context.T_ATRIBUTO.Add(entity);
        }

        public virtual void AddAtributoValidacionAsociada(AtributoValidacionAsociada new_Asociada)
        {
            T_ATRIBUTO_VALIDACION_ASOCIADA entity = this._mapper.MapToEntity(new_Asociada);

            this._context.T_ATRIBUTO_VALIDACION_ASOCIADA.Add(entity);
        }

        #endregion

        #region Update

        public virtual void UpdateAtributo(Atributo atributo)
        {
            T_ATRIBUTO entity = this._mapper.MapToEntity(atributo);
            T_ATRIBUTO attachedEntity = _context.T_ATRIBUTO.AsNoTracking()
                .FirstOrDefault(c => c.ID_ATRIBUTO == entity.ID_ATRIBUTO);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_ATRIBUTO.Attach(entity);
                _context.Entry<T_ATRIBUTO>(entity).State = EntityState.Modified;
            }
        }

        #endregion

        #region Delete

        public virtual void DeleteAtributo(Atributo atributo)
        {
            T_ATRIBUTO entity = this._context.T_ATRIBUTO
                .FirstOrDefault(x => x.ID_ATRIBUTO == atributo.Id);
            T_ATRIBUTO attachedEntity = _context.T_ATRIBUTO.Local
                .FirstOrDefault(x => x.ID_ATRIBUTO == entity.ID_ATRIBUTO);

            if (attachedEntity != null)
            {
                _context.T_ATRIBUTO.Remove(attachedEntity);
            }
            else
            {
                _context.T_ATRIBUTO.Attach(entity);
                _context.T_ATRIBUTO.Remove(entity);
            }
        }

        public virtual void DeleteAtributoValidacionAsociada(AtributoValidacionAsociada deleteAsociada)
        {
            var entity = this._mapper.MapToEntity(deleteAsociada);
            var attachedEntity = _context.T_ATRIBUTO_VALIDACION_ASOCIADA.Local
                .FirstOrDefault(x => x.ID_VALIDACION == entity.ID_VALIDACION
                    && x.ID_ATRIBUTO == entity.ID_ATRIBUTO);

            if (attachedEntity != null)
            {
                this._context.T_ATRIBUTO_VALIDACION_ASOCIADA.Remove(attachedEntity);
            }
            else
            {
                this._context.T_ATRIBUTO_VALIDACION_ASOCIADA.Attach(entity);
                this._context.T_ATRIBUTO_VALIDACION_ASOCIADA.Remove(entity);
            }
        }

        #endregion

        #region Dapper

        public virtual IEnumerable<Atributo> GetAtributos(IEnumerable<Atributo> atributos)
        {
            IEnumerable<Atributo> resultado = new List<Atributo>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_LPN_TIPO_ATRIBUTO_TEMP (ID_ATRIBUTO) VALUES (:Id)";
                    _dapper.Execute(connection, sql, atributos, transaction: tran);

                    sql = GetSqlSelectAtributo() +
                        @" INNER JOIN T_LPN_TIPO_ATRIBUTO_TEMP T ON a.ID_ATRIBUTO = T.ID_ATRIBUTO ";

                    resultado = _dapper.Query<Atributo>(connection, sql, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual IEnumerable<Atributo> GetAtributosByNombre(IEnumerable<Atributo> atributos)
        {
            IEnumerable<Atributo> resultado = new List<Atributo>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_ATRIBUTO_TEMP (NM_ATRIBUTO) VALUES (:Nombre)";
                    _dapper.Execute(connection, sql, atributos, transaction: tran);

                    sql = GetSqlSelectAtributo() +
                        @" INNER JOIN T_ATRIBUTO_TEMP T ON a.NM_ATRIBUTO = T.NM_ATRIBUTO ";

                    resultado = _dapper.Query<Atributo>(connection, sql, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public static string GetSqlSelectAtributo()
        {
            return @"SELECT 
	                    a.ID_ATRIBUTO as Id,
	                    a.NM_ATRIBUTO as Nombre,
	                    a.DS_ATRIBUTO as Descripcion,
	                    a.ID_ATRIBUTO_TIPO as IdTipo,
	                    a.VL_MASCARA_INGRESO as MascaraIngreso,
	                    a.VL_MASCARA_DISPLAY as MascaraDisplay,
	                    a.CD_DOMINIO as CodigoDominio,
	                    a.NM_CAMPO as Campo,
	                    a.NU_DECIMALES as Decimales,
	                    a.VL_SEPARADOR as Separador,
	                    a.NU_LARGO as Largo
                    FROM T_ATRIBUTO a ";
        }

        public virtual IEnumerable<AtributoValidacion> GetValidaciones()
        {
            IEnumerable<AtributoValidacion> resultado = new List<AtributoValidacion>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                string sql = GetSqlSelectValidacion();

                resultado = _dapper.Query<AtributoValidacion>(connection, sql);
            }

            return resultado;
        }

        public static string GetSqlSelectValidacion()
        {
            return @"SELECT 
	                    av.ID_VALIDACION as Id,
	                    av.NM_VALIDACION as NombreValidacion,
	                    av.DS_VALIDACION as Descripcion,
	                    av.ID_ATRIBUTO_TIPO as AtributoTipo,
	                    av.NM_ARGUMENTO as NombreArgumento,
	                    av.TP_ARGUMENTO as TipoArgumento,
	                    av.DS_ERROR as Error
                    FROM T_ATRIBUTO_VALIDACION av ";
        }

        public virtual IEnumerable<AtributoValidacionAsociada> GetValidacionesAsociadas(IEnumerable<Atributo> atributos)
        {
            IEnumerable<AtributoValidacionAsociada> resultado = new List<AtributoValidacionAsociada>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_LPN_TIPO_ATRIBUTO_TEMP (ID_ATRIBUTO) VALUES (:Id)";
                    _dapper.Execute(connection, sql, atributos, transaction: tran);

                    sql = GetSqlSelectValidacionAsociada() +
                        @" INNER JOIN T_LPN_TIPO_ATRIBUTO_TEMP T ON a.ID_ATRIBUTO = T.ID_ATRIBUTO ";

                    resultado = _dapper.Query<AtributoValidacionAsociada>(connection, sql, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public static string GetSqlSelectValidacionAsociada()
        {
            return @"SELECT 
	                    a.ID_ATRIBUTO as IdAtributo,
	                    a.ID_VALIDACION as IdValidacion,
	                    a.VL_ARGUMENTO as Valor
                    FROM T_ATRIBUTO_VALIDACION_ASOCIADA a ";
        }

        #endregion
    }
}
