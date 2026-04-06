namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_ORT_TAREA")]
    public partial class T_ORT_TAREA
    {
        public T_ORT_TAREA()
        {
            this.T_ORT_ORDEN_TAREA = new HashSet<T_ORT_ORDEN_TAREA>();
        }

        [Key]
        [StringLength(10)]
        public string CD_TAREA { get; set; }
        
        [StringLength(60)]
        public string DS_TAREA { get; set; }
        
        [StringLength(6)]
        public string TP_TAREA { get; set; }
        
        [StringLength(20)]
        public string NU_COMPONENTE { get; set; }
        
        [StringLength(1)]
        public string FL_REGISTRO_HORAS { get; set; }
        
        [StringLength(1)]
        public string FL_REGISTRO_MANIPULEO { get; set; }
        
        [StringLength(1)]
        public string FL_REGISTRO_INSUMOS { get; set; }
        
        public short CD_SITUACAO { get; set; }

        [StringLength(1)]
        public string FL_REGISTRO_HORAS_EQUIPO { get; set; }

        public virtual ICollection<T_ORT_ORDEN_TAREA> T_ORT_ORDEN_TAREA { get; set; }
    }
}
