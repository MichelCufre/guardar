namespace WIS.Persistence.Database
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_ETIQUETAS_EN_USO")]
    public partial class T_ETIQUETAS_EN_USO
    {
        [Key]
        public string NU_EXTERNO_ETIQUETA { get; set; }
        [Key]
        public string TP_ETIQUETA { get; set; }
        public int NU_ETIQUETA_LOTE { get; set; }
    }
}
