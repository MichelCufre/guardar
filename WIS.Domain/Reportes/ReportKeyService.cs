using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Reportes
{
    public class ReportKeyService : IReportKeyService
    {
        public virtual string ResolveKey(params string[] keys)
        {
            return string.Join("$", keys);
        }
    }
}
