using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Filtering;
using WIS.Persistence.Database;
using WIS.Sorting;

namespace WIS.Domain.DataModel.Queries.Utils
{
    /// <summary>
    /// Clase de consulta genérica optimizada para grillas en memoria.
    /// </summary>
    public class InMemoryQuery<T> : QueryObject<T, WISDB>
    {
        protected List<T> _records;

        protected QueriedRecordsSorterAndFilter<T> _sorterAndFilter;

        public InMemoryQuery(List<T> records, List<FilterCommand> filters, List<SortCommand> sorts)
        {
            _records = records;
            _sorterAndFilter = new QueriedRecordsSorterAndFilter<T>(_records, filters, sorts);
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = _sorterAndFilter.SortAndFilter().AsQueryable();
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
