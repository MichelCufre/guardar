namespace WIS.Persistence.Database
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;


    [Table("V_REC410_SUGERENCIAS")]
    public partial class V_REC410_SUGERENCIAS
    {
        [Key]
        public int CD_EQUIPO { get; set; }

        [StringLength(40)]
        [Column]
        public string DS_EQUIPO { get; set; }

        [Key]
        public string NU_POSICION { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_ENDERECO_DESTINO { get; set; }

        public int? CD_ESTACION { get; set; }

        [StringLength(50)]
        public string DS_ESTACION { get; set; }

        [StringLength(20)]
        public string CD_ZONA { get; set; }

        [StringLength(100)]
        public string DS_ZONA { get; set; }

        [StringLength(30)]
        public string TP_OPERATIVA { get; set; }

        [StringLength(10)]
        public string NU_PREDIO { get; set; }
    }
}
