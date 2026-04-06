using System.Collections.Generic;

namespace WIS.Domain.General.API.Bulks
{
    public class AjusteBulkOperationContext
    {
        public List<object> NewAjustes = new List<object>();

        public List<object> NewStock = new List<object>();

        public List<object> UpdateStock = new List<object>();
    }
}
