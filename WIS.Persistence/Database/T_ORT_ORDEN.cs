namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_ORT_ORDEN")]
    public partial class T_ORT_ORDEN
    {
        public T_ORT_ORDEN()
        {
            this.T_ORT_ORDEN_TAREA = new HashSet<T_ORT_ORDEN_TAREA>();
        }

        [Key]
        public int NU_ORT_ORDEN { get; set; }
        
        [StringLength(60)]
        public string DS_ORT_ORDEN { get; set; }
        
        [StringLength(20)]
        public string ID_ESTADO { get; set; }
        
        [StringLength(100)]
        public string DS_REFERENCIA { get; set; }

        public int? CD_FUNCIONARIO_ADDROW { get; set; }

        public DateTime? DT_INICIO { get; set; }

        public DateTime? DT_FIN { get; set; }

        public DateTime? DT_ULTIMA_OPERACION { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public virtual ICollection<T_ORT_ORDEN_TAREA> T_ORT_ORDEN_TAREA { get; set; }

    }
}
