namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_INT050_EMPRESAS_BLOQUEADAS")]
    public partial class V_INT050_EMPRESAS_BLOQUEADAS
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [Required]
        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }

        public short CD_SITUACAO { get; set; }

        [StringLength(30)]
        [Column]
        public string DS_SITUACAO { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_ENDERECO { get; set; }

        [StringLength(30)]
        [Column]
        public string NU_TELEFONE { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ANEXO1 { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ANEXO2 { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ANEXO3 { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ANEXO4 { get; set; }

        [StringLength(2)]
        [Column]
        public string CD_PAIS { get; set; }

        [StringLength(60)]
        [Column]
        public string DS_PAIS { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_SUBDIVISION { get; set; }

        [StringLength(100)]
        [Column]
        public string NM_SUBDIVISION { get; set; }

        public long? ID_LOCALIDAD { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_LOCALIDAD { get; set; }

        [StringLength(100)]
        [Column]
        public string NM_LOCALIDAD { get; set; }
    }
}
