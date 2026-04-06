namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_RECEPCION_REL_EMPRESA_TIPO")]
    public partial class T_RECEPCION_REL_EMPRESA_TIPO
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_RECEPCION_REL_EMPRESA_TIPO { get; set; }

        [StringLength(20)]
        [Column]
        public string TP_RECEPCION_EXTERNO { get; set; }

        [StringLength(60)]
        [Column]
        public string DS_RECEPCION_EXTERNO { get; set; }

        [StringLength(6)]
        [Column]
        public string TP_RECEPCION { get; set; }

        public int? CD_EMPRESA { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_MANEJO_INTERFAZ { get; set; }

        public int? CD_INTERFAZ_EXTERNA { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_HABILITADO { get; set; }


        public virtual T_EMPRESA T_EMPRESA { get; set; }

        public virtual T_RECEPCION_TIPO T_RECEPCION_TIPO { get; set; }


    }
}
