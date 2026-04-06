namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;


    [Table("T_ALM_SUGERENCIA")]
    public partial class T_ALM_SUGERENCIA
    {
        public T_ALM_SUGERENCIA()
        {
            this.T_ALM_SUGERENCIA_DET = new HashSet<T_ALM_SUGERENCIA_DET>();
        }

        [Key]
        public int NU_ALM_ESTRATEGIA { get; set; }

        [Key]
        [StringLength(24)]
        [Column]
        public string CD_ALM_OPERATIVA_ASOCIABLE { get; set; }

        [Key]
        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }

        [Key]
        [StringLength(2)]
        [Column]
        public string CD_CLASSE { get; set; }

        [Key]
        [StringLength(6)]
        [Column]
        public string CD_GRUPO { get; set; }

        [Key]
        public int CD_EMPRESA { get; set; }

        [Key]
        [StringLength(40)]
        [Column]
        public string CD_PRODUTO { get; set; }

        [Key]
        [StringLength(40)]
        [Column]
        public string CD_REFERENCIA { get; set; }

        [Key]
        [StringLength(40)]
        [Column]
        public string CD_AGRUPADOR { get; set; }

        public int? NU_ALM_LOGICA_INSTANCIA { get; set; }

        [Key]
        [StringLength(40)]
        [Column]
        public string CD_ENDERECO_SUGERIDO { get; set; }

        public decimal VL_TIEMPO_CALCULO { get; set; }

        [StringLength(1)]
        [Column]
        public string CD_ESTADO { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_MOTVO_RECHAZO { get; set; }

        public DateTime DT_ADDROW { get; set; }
        public DateTime? DT_UPDROW { get; set; }

        public int? CD_FUNCIONARIO { get; set; }
        public long? NU_TRANSACCION { get; set; }
        public long? NU_TRANSACCION_DELETE { get; set; }
        [Key]
        [StringLength(10)]
        [Column]
        public string TP_ALM_OPERATIVA_ASOCIABLE { get; set; }

        [Key]
        public long NU_ALM_SUGERENCIA { get; set; }

        public int? CD_EMPRESA_AGRUPADOR { get; set; }

        public virtual T_ALM_ESTRATEGIA T_ALM_ESTRATEGIA { get; set; }
        public virtual T_ALM_LOGICA_INSTANCIA T_ALM_LOGICA_INSTANCIA { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_ALM_SUGERENCIA_DET> T_ALM_SUGERENCIA_DET { get; set; }
    }
}
