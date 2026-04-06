using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Security
{
    public class Resource
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UniqueName { get; set; }
        public string Description { get; set; }
        public int UserType { get; set; }
        public bool Enabled { get; set; }
    }
}
