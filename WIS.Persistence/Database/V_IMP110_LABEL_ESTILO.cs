namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_IMP110_LABEL_ESTILO")]
    public partial class V_IMP110_LABEL_ESTILO
    {
        [StringLength(30)]
        [Column]
        public string DS_LABEL_ESTILO { get; set; }

        [Key]
        [Column(Order = 0)]
        [StringLength(15)]
        public string CD_LABEL_ESTILO { get; set; }

        [Required]
        [StringLength(20)]
        [Column]
        public string TP_LABEL { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(10)]
        public string TP_CONTENEDOR { get; set; }

		[Required]
		[StringLength(1)]
		[Column]
		public string FL_HABILITADO { get; set; }
	}
}
