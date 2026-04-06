using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.DataModel
{
    public class EntityChanges<T>
    {
        private IEqualityComparer<T> Comparer { get; set; }
        public List<T> DeletedRecords { get; set; }
        public List<T> UpdatedRecords { get; set; }
        public List<T> AddedRecords { get; set; }

        public EntityChanges()
        {
            this.DeletedRecords = new List<T>();
            this.UpdatedRecords = new List<T>();
            this.AddedRecords = new List<T>();
        }

        public EntityChanges(IEqualityComparer<T> comparer)
        {
            this.Comparer = comparer;
            this.DeletedRecords = new List<T>();
            this.UpdatedRecords = new List<T>();
            this.AddedRecords = new List<T>();
        }

    }
}
