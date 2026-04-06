using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WIS.Persistence.Database
{
    [Table("T_LPN")]
    public partial class T_LPN
    {
        public T_LPN()
        {
            T_LPN_DET = new HashSet<T_LPN_DET>();
        }

        [Key]
        [Column(Order = 0)]
        public long NU_LPN { get; set; }

        [Required]
        [StringLength(50)]
        public string ID_LPN_EXTERNO { get; set; }

        [Required]
        [StringLength(10)]
        public string TP_LPN_TIPO { get; set; }

        [Required]
        [StringLength(6)]
        public string ID_ESTADO { get; set; }

        [StringLength(40)]
        public string CD_ENDERECO { get; set; }

        public DateTime DT_ADDROW { get; set; }
        public DateTime? DT_UPDROW { get; set; }

        public DateTime? DT_ACTIVACION { get; set; }

        public DateTime? DT_FIN { get; set; }

        public long? NU_TRANSACCION { get; set; }

        public long? NU_TRANSACCION_DELETE { get; set; }

        public int CD_EMPRESA { get; set; }

        [StringLength(50)]
        public string ID_PACKING { get; set; }

        public int? NU_AGENDA { get; set; }

        [StringLength(1)]
        public string FL_DISPONIBLE_LIBERACION { get; set; }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_LPN_DET> T_LPN_DET { get; set; }
        public virtual T_LPN_TIPO T_LPN_TIPO { get; set; }

    }
}
