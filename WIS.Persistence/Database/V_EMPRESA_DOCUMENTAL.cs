namespace WIS.Persistence.Database
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;


    [Table("V_EMPRESA_DOCUMENTAL")]
    public partial class V_EMPRESA_DOCUMENTAL
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_DOCUMENTAL { get; set; }
    }
}
