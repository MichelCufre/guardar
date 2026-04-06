namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_PRE050_EMPRESAS_PED_PENDIEN")]
    public partial class V_PRE050_EMPRESAS_PED_PENDIEN
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [Required]
        [Column]
        [StringLength(55)]
        public string NM_EMPRESA { get; set; }

        public int USERID { get; set; }
    }
}
