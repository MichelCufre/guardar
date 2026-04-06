using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WIS.BackendService.Configuration;
using WIS.FormComponent.Execution;

namespace WIS.BackendService.Services
{
    public class FormConfigProvider : IFormConfigProvider
    {
        private readonly string MaxSelectAsyncResultsParam;

        public FormConfigProvider(IOptions<SelectSettings> settings)
        {
            this.MaxSelectAsyncResultsParam = settings.Value.MaxSelectAsyncResults;
        }

        public int GetSelectResultLimit()
        {
            if (string.IsNullOrEmpty(this.MaxSelectAsyncResultsParam) || !int.TryParse(this.MaxSelectAsyncResultsParam, out int result))
                return 20;

            return result;
        }
    }
}
