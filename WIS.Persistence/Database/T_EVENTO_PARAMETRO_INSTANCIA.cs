namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_EVENTO_PARAMETRO_INSTANCIA")]
    public partial class T_EVENTO_PARAMETRO_INSTANCIA
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(50)]
        public string CD_EVENTO_PARAMETRO { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_EVENTO_INSTANCIA { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_EVENTO { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(60)]
        public string TP_NOTIFICACION { get; set; }

        [StringLength(100)]
        [Column]
        public string VL_PARAMETRO { get; set; }

        public DateTime DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public long? NU_TRANSACCION { get; set; }

        public long? NU_TRANSACCION_DELETE { get; set; }


        public virtual T_EVENTO T_EVENTO { get; set; }

        public virtual T_EVENTO_INSTANCIA T_EVENTO_INSTANCIA { get; set; }

        public virtual T_EVENTO_PARAMETRO T_EVENTO_PARAMETRO { get; set; }
    }
}
