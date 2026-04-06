namespace WIS.Persistence.Database
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_DOCUMENTO_TIPO_EDITABLE_DET")]
    public partial class T_DOCUMENTO_TIPO_EDITABLE_DET
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_DOCUMENTO_TIPO_EDITABLE_DET()
        {
            
        }

        [Key]
        [Column(Order = 0)]
        [StringLength(6)]
        public string TP_DOCUMENTO { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string NM_DATAFIELD { get; set; }
    }
}
