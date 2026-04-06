using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data.Middleware;
using WIS.Filtering;
using WIS.Sorting;

namespace WIS.Data
{
    public abstract class QueryObject<T, ContextDataType> : IQueryObject<T, ContextDataType>
    {
        protected IQueryable<T> Query { get; set; }

        public abstract void BuildQuery(ContextDataType context);

        public void ApplyFilter(IFilterInterpreter filter, string filterString)
        {
            if (!string.IsNullOrEmpty(filterString))
                this.Query = this.Query.Where(filter.Interpret<T>(filterString));
        }

        public void ApplyFilter(IFilterInterpreter filter, List<FilterCommand> commands)
        {
            var filtros = commands.Where(c => !string.IsNullOrEmpty(c.Value.Trim())).ToList();
            if (filtros.Any())
                this.Query = this.Query.Where(filter.Interpret<T>(filtros));
        }

        public void ApplySort(ISortingService sorter, List<SortCommand> commands)
        {
            this.Query = sorter.ApplySorting(this.Query, commands);
        }

        public void ApplyMiddleware(IQueryMiddleware filter)
        {
            this.Query = filter.ApplyFilter(this.Query);
        }

        public void SkipRecords(int recordsToSkip)
        {
            this.Query = this.Query.Skip(recordsToSkip);
        }

        public void TakeRecords(int recordsToTake)
        {
            this.Query = this.Query.Take(recordsToTake);
        }

        public bool Any()
        {
            if (this.Query == null)
                throw new InvalidOperationException("General_Sec0_Error_QueryNotReady");

            return this.Query.Any();
        }

        /// <summary>
        /// Obtiene resultado de query. Pensado para trabajar con UnitOfWork. No usar directamente.
        /// </summary>
        /// <returns>Lista de entidades de modelo</returns>
        public virtual IList<T> GetResult()
        {
            return this.Query.ToList();

        }
    }
}
