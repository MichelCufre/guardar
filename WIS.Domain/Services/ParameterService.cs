using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.Produccion;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.Services
{
    public class ParameterService : IParameterService
    {
        public readonly IUnitOfWorkFactory _uowFactory;

        public ParameterService(IUnitOfWorkFactory uowFactory)
        {
            _uowFactory = uowFactory;
        }

        public virtual Dictionary<string, string> GetValues(List<string> parameters)
        {
            using var uow = _uowFactory.GetUnitOfWork();

            return uow.ParametroRepository.GetParameters(parameters);
        }

        public virtual string GetValue(string parameter)
        {
            using var uow = _uowFactory.GetUnitOfWork();

            return uow.ParametroRepository.GetParameter(parameter);
        }

        public virtual string GetValueByEmpresa(string parameter, int empresa)
        {
            using var uow = _uowFactory.GetUnitOfWork();

            return uow.ParametroRepository.GetParameter(parameter, new Dictionary<string, string>()
            {
                [ParamManager.PARAM_EMPR] = $"{ParamManager.PARAM_EMPR}_{empresa}"
            });
        }
        public virtual string GetValueByUser(string parameter, int user)
        {
            using var uow = _uowFactory.GetUnitOfWork();

            return uow.ParametroRepository.GetParameter(parameter, new Dictionary<string, string>()
            {
                [ParamManager.PARAM_USER] = $"{ParamManager.PARAM_USER}_{user}"
            });
        }
        public virtual string GetValueByDomain(string parameter, string domain)
        {
            using var uow = _uowFactory.GetUnitOfWork();

            return uow.ParametroRepository.GetParameter(parameter, new Dictionary<string, string>()
            {
                [ParamManager.PARAM_DOMAIN] = domain
            });
        }

        public virtual bool AnyValueEmpresa(string parameter, int empresa)
        {
            using var uow = _uowFactory.GetUnitOfWork();

            return uow.ParametroRepository.AnyParameter(parameter, new Dictionary<string, string>()
            {
                [ParamManager.PARAM_EMPR] = $"{ParamManager.PARAM_EMPR}_{empresa}"
            });
        }

        public virtual Dictionary<int, string> GetValuesByEmpresa(string parameter, List<int> empresas)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            var result = new Dictionary<int, string>();

            foreach (var emp in empresas)
            {
                result[emp] = (GetValueByEmpresa(parameter, emp) ?? "N");
            }

            return result;
        }
    }
}
