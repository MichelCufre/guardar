using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.Validation;

namespace WIS.Domain.Services.Interfaces
{
    public interface IResourceValidationService
    {
        string Translate(string loginName, Error error);
    }
}
