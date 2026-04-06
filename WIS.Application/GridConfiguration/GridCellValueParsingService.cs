using System;
using System.Collections.Generic;
using System.Text;
using WIS.GridComponent.Build;
using WIS.Security;

namespace WIS.Application.GridConfiguration
{
    public class GridCellValueParsingService : IGridCellValueParsingService
    {
        protected readonly IIdentityService _identityService;

        public GridCellValueParsingService(IIdentityService identityService)
        {
            this._identityService = identityService;
        }

        public virtual string GetValue(DateTime dateValue)
        {
            return dateValue.ToString("o");
        }
        public virtual string GetValue(DateTime? dateValue)
        {
            return dateValue?.ToString("o");
        }
        public virtual string GetValue(object value)
        {
            return Convert.ToString(value, this._identityService.GetFormatProvider());
        }
    }
}
