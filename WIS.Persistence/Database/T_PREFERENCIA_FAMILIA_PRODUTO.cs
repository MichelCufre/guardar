namespace WIS.Persistence.Database
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_PREFERENCIA_FAMILIA_PRODUTO")]
    public partial class T_PREFERENCIA_FAMILIA_PRODUTO
    {
        [Key]
        public int NU_PREFERENCIA { get; set; }

        [Key]
        public int CD_FAMILIA_PRODUTO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }
    }
}
