using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Services.Interfaces
{
    public interface IParameterService
    {
        Dictionary<string, string> GetValues(List<string> parameters);
        string GetValue(string parameter);

        string GetValueByEmpresa(string parameter, int empresa);
        string GetValueByUser(string parameter, int user);
        string GetValueByDomain(string parameter, string domain);

        bool AnyValueEmpresa(string parameter, int empresa);
        Dictionary<int, string> GetValuesByEmpresa(string parameter, List<int> empresas);
    }
}
