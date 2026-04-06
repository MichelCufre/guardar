using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.Services.Interfaces;

namespace WIS.Domain.Services
{
    public class ServiceContext : IServiceContext
    {
        protected readonly IUnitOfWork _uow;

        public int Empresa { get; private set; } = 0;
        public int UserId { get; private set; } = 0;

        protected List<Parametro> _params = new List<Parametro>();
        protected Dictionary<string, string> _dictParams = new Dictionary<string, string>();
        protected HashSet<string> _camposInmutables = new HashSet<string>();
        protected string _paramCamposInmutables;
        protected string _caracteresNoPermitidosIdentificador;

        public bool PermiteProductoInactivos { get; private set; }

        public ServiceContext(IUnitOfWork uow, int userId, int empresa)
        {
            _uow = uow;
            Empresa = empresa;
            UserId = userId;
        }

        public virtual async Task Load()
        {
            AddParametro(ParamManager.MOBILE_LOV_ID_SEPARATOR);
            AddParametro(ParamManager.NUMBER_DECIMAL_SEPARATOR);
            AddParametro(ParamManager.DATETIME_FORMAT_DATE_SECONDS);
            AddParametro(ParamManager.CARACTERES_NO_PERMITIDOS_LOTE);
            AddParametroEmpresa(ParamManager.USAR_PRODUCTOS_INACTIVOS, Empresa);

            if (_params != null && _params.Count > 0)
            {
                foreach (var p in await _uow.ParametroRepository.GetParametros(_params))
                {
                    _dictParams[p.Codigo] = p.Valor;
                }

                if (!string.IsNullOrEmpty(_paramCamposInmutables))
                    LoadCamposInmutables();

                LoadCaracteresNoPermitidosIdentificador();

                PermiteProductoInactivos = (GetParametro(ParamManager.USAR_PRODUCTOS_INACTIVOS) ?? "S") == "S";
            }
        }

        public virtual string GetParametro(string codigo)
        {
            return _dictParams.GetValueOrDefault(codigo);
        }

        public virtual void SetParametroCamposInmutables(string paramCamposInmutables)
        {
            _paramCamposInmutables = paramCamposInmutables;
        }

        public virtual void LoadCamposInmutables()
        {
            var camposInmutables = GetParametro(_paramCamposInmutables);

            if (!string.IsNullOrEmpty(camposInmutables))
            {
                var inmutables = camposInmutables
                    .Split(";", StringSplitOptions.RemoveEmptyEntries)
                    .Select(c => c.Trim());

                foreach (var campo in inmutables)
                {
                    if (!string.IsNullOrEmpty(campo) && !_camposInmutables.Contains(campo))
                    {
                        _camposInmutables.Add(campo);
                    }
                }
            }
        }

        public virtual void LoadCaracteresNoPermitidosIdentificador()
        {
            _caracteresNoPermitidosIdentificador = ("$" + GetParametro(ParamManager.MOBILE_LOV_ID_SEPARATOR) + GetParametro(ParamManager.CARACTERES_NO_PERMITIDOS_LOTE));
        }

        public virtual void AddParametroEmpresa(string codigo, int empresa)
        {
            AddParametro(codigo, ParamManager.PARAM_EMPR, $"{ParamManager.PARAM_EMPR}_{empresa}");
        }

        public virtual void AddParametroPredio(string codigo, string predio)
        {
            AddParametro(codigo, ParamManager.PARAM_PRED, $"{ParamManager.PARAM_PRED}_{predio}");
        }

        public virtual void AddParametro(string codigo, string entidad = ParamManager.PARAM_GRAL, string entidadValor = ParamManager.PARAM_GRAL)
        {
            _params.Add(new Parametro()
            {
                Codigo = codigo,
                Entidad = entidad,
                EntidadValor = entidadValor,
            });
        }

        public virtual HashSet<string> GetCamposInmutables()
        {
            return _camposInmutables;
        }

        public virtual string GetCaracteresNoPermitidosIdentificador()
        {
            return _caracteresNoPermitidosIdentificador;
        }
    }
}
