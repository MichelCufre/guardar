namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_RESOURCES_WSEG020")]
    public partial class V_RESOURCES_WSEG020
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int RESOURCEID { get; set; }

        [Required]
        [StringLength(200)]
        [Column]
        public string DESCRIPTION { get; set; }

        [Required]
        [StringLength(100)]
        [Column]
        public string NAME { get; set; }

        [Required]
        [StringLength(100)]
        [Column]
        public string UNIQUENAME { get; set; }

        public int? USERTYPEID { get; set; }

        [StringLength(300)]
        [Column]
        public string APLICACION { get; set; }

        [StringLength(300)]
        [Column]
        public string SECCION { get; set; }

        [StringLength(300)]
        [Column]
        public string TIPO { get; set; }

        [StringLength(300)]
        [Column]
        public string NOMBRE { get; set; }

        [StringLength(50)]
        [Column]
        public string USERTYPE { get; set; }

        [Required]
        [StringLength(1)]
        [Column]
        public string FL_ACTIVO { get; set; }
    }
}
