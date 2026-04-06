using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Extensions;
using WIS.Domain.General;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class ParametroRepository
    {
        protected readonly int _userId;
        protected readonly WISDB _context;
        protected readonly IDapper _dapper;
        protected readonly string _application;
        protected readonly ParametroMapper _mapper;

        public ParametroRepository(WISDB context, string application, int userId, IDapper dapper)
        {
            this._context = context;
            this._application = application;
            this._userId = userId;
            this._mapper = new ParametroMapper();
            this._dapper = dapper;
        }

        #region Any

        public virtual bool AnyParameter(string parameterName, Dictionary<string, string> entities = null)
        {
            return GetParameter(parameterName, entities) != null;
        }

        public virtual bool ExisteParametroConfiguracion(string cdParametro)
        {
            return this._context.T_LPARAMETRO_CONFIGURACION.Any(a => a.CD_PARAMETRO == cdParametro);
        }

        public virtual bool ExisteParametroConfiguracion(string cdParametro, string entidad)
        {
            return this._context.T_LPARAMETRO_CONFIGURACION.Any(a => a.CD_PARAMETRO == cdParametro && a.ND_ENTIDAD == entidad);
        }

        #endregion

        #region Get

        public virtual string GetParameter(string parameterName, Dictionary<string, string> entities = null)
        {
            var parameters = this.GetParameterList(new List<string> { parameterName });

            if (parameters == null || !parameters.Any())
                return null;

            foreach (var parameter in parameters)
            {
                if (entities != null && entities.Any(entity => entity.Key == parameter.Entidad && entity.Value == parameter.EntidadValor))
                    return parameter.Valor;
            }

            if (entities != null)
            {
                var lastParameter = parameters.Last();

                foreach (var entity in entities)
                {
                    if (entity.Key == lastParameter.Entidad)
                    {
                        return null;
                    }
                }
            }

            return parameters.Last().Valor;
        }

        public virtual ParametroConfiguracion GetParametroConfiguracion(string cdParametro, string clave)
        {
            var entity = this._context.T_LPARAMETRO_CONFIGURACION
                .FirstOrDefault(x => x.CD_PARAMETRO == cdParametro && x.ND_ENTIDAD == clave);

            return this._mapper.MapToObject(entity);
        }

        public virtual List<ParametroConfiguracion> GetParams(string cdParametro)
        {
            return _context.T_LPARAMETRO_CONFIGURACION.Where(x => x.CD_PARAMETRO == cdParametro)
                .Select(x => _mapper.MapToObject(x))
                .ToList();
        }

        public virtual Dictionary<string, string> GetParameters(List<string> parameterNames, Dictionary<string, string> entities = null)
        {
            List<Parametro> parameters = this.GetParameterList(parameterNames);

            Dictionary<string, string> result = new Dictionary<string, string>();

            foreach (var name in parameterNames)
            {
                result[name] = null;
            }

            if (parameters == null || !parameters.Any())
                return result;

            var groupedParameters = parameters.GroupBy(d => d.Codigo).ToList();

            foreach (var group in groupedParameters)
            {
                if (entities != null)
                {
                    var parameter = group.Where(d => entities.Any(entity => entity.Key == d.Entidad && entity.Value == d.EntidadValor)).FirstOrDefault();

                    result[parameter.Codigo] = parameter.Valor;
                }
                else
                {
                    var parameter = group.Last();

                    result[parameter.Codigo] = parameter.Valor;
                }
            }

            return result;
        }

        public virtual List<Parametro> GetParameterList(List<string> parameterNames)
        {
            return this._context.T_LPARAMETRO_CONFIGURACION.Join(
                this._context.T_LPARAMETRO_NIVEL,
                pc => new { pc.CD_PARAMETRO, pc.DO_ENTIDAD_PARAMETRIZABLE },
                pn => new { pn.CD_PARAMETRO, pn.DO_ENTIDAD_PARAMETRIZABLE },
                (pc, pn) => new { Configuracion = pc, Nivel = pn })
            .Where(d => parameterNames.Contains(d.Configuracion.CD_PARAMETRO))
            .Select(d => new Parametro
            {
                Codigo = d.Configuracion.CD_PARAMETRO,
                Entidad = d.Configuracion.DO_ENTIDAD_PARAMETRIZABLE,
                EntidadValor = d.Configuracion.ND_ENTIDAD,
                Valor = d.Configuracion.VL_PARAMETRO,
                Nivel = d.Nivel.NU_NIVEL ?? -1
            })
            .OrderByDescending(d => d.Nivel)
            .ToList();
        }

        #endregion

        #region Add

        public virtual void AddParametroConfiguracion(ParametroConfiguracion objInterfaz)
        {
            var entity = this._mapper.MapToEntity(objInterfaz);

            if (entity.NU_PARAMETRO_CONFIGURACION == 0)
                entity.NU_PARAMETRO_CONFIGURACION = this._context.GetNextSequenceValueInt(_dapper, Secuencias.S_NU_PARAMETRO_CONFIGURACION);

            objInterfaz.NuParametroConfiguracion = entity.NU_PARAMETRO_CONFIGURACION;

            this._context.T_LPARAMETRO_CONFIGURACION.Add(entity);
        }

        #endregion

        #region Update

        public virtual void UpdateParametroConfiguracion(ParametroConfiguracion paramConf)
        {
            var entity = this._mapper.MapToEntity(paramConf);
            var attachedEntity = _context.T_LPARAMETRO_CONFIGURACION.Local
                .FirstOrDefault(x => x.NU_PARAMETRO_CONFIGURACION == entity.NU_PARAMETRO_CONFIGURACION);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.State = EntityState.Modified;
                attachedEntry.CurrentValues.SetValues(entity);
            }
            else
            {
                _context.T_LPARAMETRO_CONFIGURACION.Attach(entity);
                _context.Entry<T_LPARAMETRO_CONFIGURACION>(entity).State = EntityState.Modified;
            }
        }

        #endregion

        #region Remove

        public virtual void DeleteParametroConfiguracion(string cdParametro, string entidad)
        {
            var entity = this._context.T_LPARAMETRO_CONFIGURACION
                .FirstOrDefault(x => x.CD_PARAMETRO == cdParametro && x.ND_ENTIDAD == entidad);

            var attachedEntity = _context.T_LPARAMETRO_CONFIGURACION.Local
                .FirstOrDefault(w => w.CD_PARAMETRO == entity.CD_PARAMETRO && entity.ND_ENTIDAD == entidad);

            if (attachedEntity != null)
            {
                _context.T_LPARAMETRO_CONFIGURACION.Remove(attachedEntity);
            }
            else
            {
                _context.T_LPARAMETRO_CONFIGURACION.Remove(entity);
            }
        }

        #endregion

        #region Dapper

        public virtual async Task<string> GetParametro(string codigo, string entidadParam = ParamManager.PARAM_GRAL, string entidad = null, CancellationToken cancelToken = default)
        {
            string sql =
                @"SELECT VL_PARAMETRO  
                FROM T_LPARAMETRO_CONFIGURACION
                WHERE CD_PARAMETRO = :codigo and ND_ENTIDAD = :entidad";

            var param = new DynamicParameters(new
            {
                codigo = codigo,
                entidad = !string.IsNullOrEmpty(entidad) ? $"{entidadParam}_{entidad}" : ParamManager.PARAM_GRAL
            });

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                var query = _dapper.Query<string>(connection, sql, param: param, commandType: CommandType.Text);

                if (query.FirstOrDefault() != null)
                    return query.FirstOrDefault();
                else
                {
                    param = new DynamicParameters(new
                    {
                        codigo = codigo,
                        entidad = ParamManager.PARAM_GRAL
                    });

                    query = _dapper.Query<string>(connection, sql, param: param, commandType: CommandType.Text);

                    if (query.FirstOrDefault() != null)
                        return query.FirstOrDefault();
                }

                return null;
            }
        }

        public virtual async Task<IEnumerable<Parametro>> GetParametros(IEnumerable<Parametro> parametros, CancellationToken cancelToken = default)
        {
            var param = new DynamicParameters(new
            {
                codigos = parametros.Select(p => p.Codigo),
            });

            string sql =
                @"SELECT CD_PARAMETRO AS Codigo
                , DO_ENTIDAD_PARAMETRIZABLE AS Entidad
                , ND_ENTIDAD AS EntidadValor
                , VL_PARAMETRO AS Valor
                FROM T_LPARAMETRO_CONFIGURACION
                WHERE CD_PARAMETRO IN :codigos";

            var dict = new Dictionary<string, Parametro>();

            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);

                var query = _dapper.Query<Parametro>(connection, sql, param: param, commandType: CommandType.Text);

                foreach (var p in query)
                {
                    dict[GetKey(p)] = p;
                }
            }

            foreach (var p in parametros)
            {
                p.Valor = dict.GetValueOrDefault(GetKey(p))?.Valor ?? dict.GetValueOrDefault(GetKey(p.Codigo))?.Valor;
            }

            return parametros;
        }

        public virtual string GetKey(Parametro p)
        {
            return GetKey(p.Codigo, p.Entidad, p.EntidadValor);
        }

        public virtual string GetKey(string codigo, string entidad = ParamManager.PARAM_GRAL, string entidadValor = ParamManager.PARAM_GRAL)
        {
            return string.Join(".", codigo, entidad, entidadValor);
        }

        public virtual void AddParametroConfiguracion(IEnumerable<ParametroConfiguracion> parametros)
        {
            string sql = @" INSERT INTO T_LPARAMETRO_CONFIGURACION (CD_PARAMETRO,DO_ENTIDAD_PARAMETRIZABLE,ND_ENTIDAD,VL_PARAMETRO) 
                            VALUES (:CodigoParametro, :TipoParametro, :Clave, :Valor)";

            _dapper.Execute(_context.Database.GetDbConnection(), sql, parametros, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        public virtual bool GetParamInterfazHabilitada(int codigoInterfazExterna, int empresa, DbConnection connection, DbTransaction tran)
        {
            var sql = @"SELECT VL_PARAMETRO_HABILITACION FROM T_INTERFAZ_EXTERNA WHERE CD_INTERFAZ_EXTERNA = :nuAjuste";

            var parametro = _dapper.Query<string>(connection, sql, new
            {
                nuAjuste = codigoInterfazExterna
            }, CommandType.Text, transaction: tran).FirstOrDefault();

            if (string.IsNullOrEmpty(parametro))
                parametro = ParamManager.CUSTOM_API_MAPPING;

            if (parametro != ParamManager.CUSTOM_API_MAPPING)
            {
                return (GetParametro(parametro, ParamManager.PARAM_EMPR, empresa.ToString()).Result ?? "N") == "S";
            }
            else
            {
                var vlParametro = GetParametro(parametro, ParamManager.PARAM_EMPR, empresa.ToString()).Result ?? null;

                if (string.IsNullOrEmpty(vlParametro))
                    return false;
                else
                    parametro = string.Empty;

                var datosApis = vlParametro.Split('@', StringSplitOptions.RemoveEmptyEntries);

                foreach (var datosApi in datosApis)
                {
                    var datos = datosApi.Split('#', StringSplitOptions.RemoveEmptyEntries);
                    if (codigoInterfazExterna.ToString() == datos[2])
                    {
                        parametro = datos[1];
                        break;
                    }
                }

                if (string.IsNullOrEmpty(parametro))
                    return false;
                else
                    return (GetParametro(parametro, ParamManager.PARAM_EMPR, empresa.ToString()).Result ?? "N") == "S";
            }
        }
        #endregion
    }
}
