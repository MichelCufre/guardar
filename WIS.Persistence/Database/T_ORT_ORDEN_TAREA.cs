namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_ORT_ORDEN_TAREA")]
    public partial class T_ORT_ORDEN_TAREA
    {
        public T_ORT_ORDEN_TAREA()
        {
            this.T_ORT_ORDEN_TAREA_FUNCIONARIO = new HashSet<T_ORT_ORDEN_TAREA_FUNCIONARIO>();
            this.T_ORT_ORDEN_TAREA_EQUIPO = new HashSet<T_ORT_ORDEN_TAREA_EQUIPO>();
        }

        [Key]
        public long NU_ORDEN_TAREA { get; set; }

        [StringLength(10)]
        public string CD_TAREA { get; set; }

        [StringLength(1)]
        public string FL_RESUELTA { get; set; }

        public int NU_ORT_ORDEN { get; set; }

        public int CD_EMPRESA { get; set; }

        public int? CD_FUNCIONARIO_ADDROW { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public virtual T_ORT_ORDEN T_ORT_ORDEN { get; set; }

        public virtual T_ORT_TAREA T_ORT_TAREA { get; set; }

        public virtual ICollection<T_ORT_ORDEN_TAREA_FUNCIONARIO> T_ORT_ORDEN_TAREA_FUNCIONARIO { get; set; }
        public virtual ICollection<T_ORT_ORDEN_TAREA_EQUIPO> T_ORT_ORDEN_TAREA_EQUIPO { get; set; }

    }
}
