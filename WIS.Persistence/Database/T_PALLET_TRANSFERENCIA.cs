namespace WIS.Persistence.Database
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_PALLET_TRANSFERENCIA")]
    public partial class T_PALLET_TRANSFERENCIA
    {
        [Key]
        [Column(Order = 0)]
        public decimal NU_ETIQUETA { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_SEC_ETIQUETA { get; set; }

        public short CD_SITUACAO { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_ENDERECO_REAL { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_ENDERECO_DESTINO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public DateTime? DT_FINALIZACION { get; set; }

        [StringLength(30)]
        [Column]
        public string CD_APLICACAO_ORIGEN { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }

        [StringLength(20)]
        [Column]
        public string TP_MODALIDAD_USO { get; set; }

        public long? NU_TRANSACCION { get; set; }

        public string ID_EXTERNO_ETIQUETA { get; set; }

        public string TP_ETIQUETA_TRANSFERENCIA { get; set; }

        public long? NU_LPN { get; set; }

    }
}
