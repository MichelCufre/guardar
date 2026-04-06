namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("V_CONT_SIN_EMBARCAR_WEXP041")]
    public partial class V_CONT_SIN_EMBARCAR_WEXP041
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_CONTENEDOR { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_ENDERECO { get; set; }

        public int CD_CAMION { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(10)]
        public string CD_CLIENTE { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_CLIENTE { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [StringLength(20)]
        [Column]
        public string ID_PRECINTO_1 { get; set; }

        [StringLength(20)]
        [Column]
        public string ID_PRECINTO_2 { get; set; }

        [StringLength(2)]
        [Column]
        public string NU_PREDIO { get; set; }

		[StringLength(50)]
		[Column]
		public string ID_EXTERNO_CONTENEDOR { get; set; }
	}
}
