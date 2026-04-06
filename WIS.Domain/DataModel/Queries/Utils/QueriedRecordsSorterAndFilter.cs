using System.Collections.Generic;
using System.Linq;
using WIS.Filtering;
using WIS.Sorting;

namespace WIS.Domain.DataModel.Queries.Utils
{
    public class QueriedRecordsSorterAndFilter<T>
    {

        protected List<T> _records;
        protected List<FilterCommand> _filters;
        protected List<SortCommand> _sorts;

        public QueriedRecordsSorterAndFilter(List<T> records, List<FilterCommand> filters = null, List<SortCommand> sorts = null)
        {
            _records = records;
            _filters = filters;
            _sorts = sorts;
        }

        public virtual List<T> SortAndFilter()
        {
            SortRecords();
            FilterRecords();

            return _records;
        }

        public virtual List<T> Filter()
        {
            FilterRecords();

            return _records;
        }

        public virtual void SortRecords()
        {
            foreach (var sort in _sorts)
            {
                var property = typeof(T).GetProperties().FirstOrDefault(i => i.Name == sort.ColumnId);

                if (sort.Direction == SortDirection.Ascending)
                    _records.Sort((a, b) => property.GetValue(a).ToString().CompareTo(property.GetValue(b).ToString()));

                else if (sort.Direction == SortDirection.Descending)
                    _records.Sort((a, b) => property.GetValue(b).ToString().CompareTo(property.GetValue(a).ToString()));
            }
        }

        public virtual void FilterRecords()
        {
            foreach (var filter in _filters)
            {
                var property = typeof(T).GetProperties().FirstOrDefault(i => i.Name == filter.ColumnId);

                var filterValue = filter.Value;

                if (filterValue != null)
                {
                    filterValue = filterValue.ToLower();

                    if (filterValue.Contains("null"))
                    {
                        if (filterValue.StartsWith("!"))
                            _records = _records.Where(w => property.GetValue(w) != null).ToList();

                        else
                            _records = _records.Where(w => property.GetValue(w) == null).ToList();

                    }
                    else if (filterValue.Contains("%"))
                    {
                        var vlSinCaracteresEspeciales = filterValue.Replace("%", "");

                        if (filterValue.StartsWith("%") && filterValue.EndsWith("%"))
                            _records = _records.Where(w => property.GetValue(w) != null && property.GetValue(w).ToString().ToLower().Contains(vlSinCaracteresEspeciales)).ToList();

                        else if (filterValue.StartsWith("%"))
                            _records = _records.Where(w => property.GetValue(w) != null && property.GetValue(w).ToString().ToLower().EndsWith(vlSinCaracteresEspeciales)).ToList();

                        else if (filterValue.EndsWith("%"))
                            _records = _records.Where(w => property.GetValue(w) != null && property.GetValue(w).ToString().ToLower().StartsWith(vlSinCaracteresEspeciales)).ToList();

                    }
                    else
                    {
                        _records = _records.Where(w => property.GetValue(w)?.ToString().ToLower() == filterValue).ToList();
                    }
                }
            }

        }
    }
}
