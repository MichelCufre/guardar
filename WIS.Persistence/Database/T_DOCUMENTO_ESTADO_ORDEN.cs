namespace WIS.Persistence.Database
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_DOCUMENTO_ESTADO_ORDEN")]
    public partial class T_DOCUMENTO_ESTADO_ORDEN
    {
        [Key]
        public int NU_ESTADO_ORDEN { get; set; }

        [StringLength(6)]
        [Column]
        public string TP_DOCUMENTO { get; set; }

        [StringLength(6)]
        [Column]
        public string ID_ESTADO_ORIGEN { get; set; }

        [StringLength(6)]
        [Column]
        public string ID_ESTADO_DESTINO { get; set; }

        [StringLength(6)]
        [Column]
        public string CD_ACCION { get; set; }

        public virtual T_DOCUMENTO_ESTADO T_DOCUMENTO_ESTADO_ORIGEN { get; set; }

        public virtual T_DOCUMENTO_ESTADO T_DOCUMENTO_ESTADO_DESTINO { get; set; }

        public virtual T_DOCUMENTO_TIPO T_DOCUMENTO_TIPO { get; set; }
    }
}
