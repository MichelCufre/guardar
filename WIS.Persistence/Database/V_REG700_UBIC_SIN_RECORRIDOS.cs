using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_REG700_UBIC_SIN_RECORRIDOS")]
    public partial class V_REG700_UBIC_SIN_RECORRIDOS
    {
        [Key]
        [Column(Order = 0)]
        public int NU_RECORRIDO { get; set; }

        [Key]
        [StringLength(40)]
        [Column(Order = 1)]
        public string CD_ENDERECO { get; set; }

        [StringLength(100)]
        public string CD_BARRAS { get; set; }

        public int CD_EMPRESA { get; set; }

        [Required]
        [StringLength(2)]
        [Column]
        public string CD_CLASSE { get; set; }

        public short CD_ROTATIVIDADE { get; set; }

        [Required]
        [StringLength(30)]
        [Column]
        public string DS_ROTATIVIDADE { get; set; }
       
        public short CD_TIPO_ENDERECO { get; set; }

        [Required]
        [StringLength(30)]
        [Column]
        public string DS_TIPO_ENDERECO { get; set; }

        public int CD_FAMILIA_PRINCIPAL { get; set; }

        [Required]
        [StringLength(100)]
        [Column]
        public string DS_FAMILIA_PRODUTO { get; set; }

        public short CD_SITUACAO { get; set; }

        [Required]
        [StringLength(30)]
        [Column]
        public string DS_SITUACAO { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_ZONA_UBICACION { get; set; }

        [Required]
        [StringLength(1)]
        [Column]
        public string ID_ENDERECO_BAIXO { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_ENDERECO_SEP { get; set; }

        [Required]
        [StringLength(1)]
        [Column]
        public string ID_NECESSIDADE_RESUPRIR { get; set; }

        [StringLength(5)]
        [Column]
        public string CD_CONTROL { get; set; }

        public short? CD_AREA_ARMAZ { get; set; }

        [Required]
        [StringLength(15)]
        [Column]
        public string DS_AREA_ARMAZ { get; set; }

        [StringLength(4)]
        [Column]
        public string NU_COMPONENTE { get; set; }

        [Required]
        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }

        [StringLength(10)]
        [Column]
        public string ID_BLOQUE { get; set; }

        [StringLength(10)]
        [Column]
        public string ID_CALLE { get; set; }

        public int? NU_COLUMNA { get; set; }

        public int? NU_ALTURA { get; set; }

        public int? NU_PROFUNDIDAD { get; set; }

        public DateTime DT_UPDROW { get; set; }

        public DateTime DT_ADDROW { get; set; }

    }
}
