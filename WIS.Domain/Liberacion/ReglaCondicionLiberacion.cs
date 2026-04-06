using System;

namespace WIS.Domain.Liberacion
{
    public partial class ReglaCondicionLiberacion
    {
        public int nuRegla { get; set; }

        public string cdCondicionLiberacion { get; set; }

        public DateTime? dtAddRow { get; set; }

        public DateTime? dtUpdRow { get; set; }

        public virtual ReglaLiberacion ReglaLiberacion { get; set; }
    }
}