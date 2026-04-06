using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WIS.Persistence.Database
{

    [Table("T_ATRIBUTO")]
    public partial class T_ATRIBUTO
    {
        public T_ATRIBUTO()
        {
            T_LPN_ATRIBUTO = new HashSet<T_LPN_ATRIBUTO>();
            T_LPN_DET_ATRIBUTO = new HashSet<T_LPN_DET_ATRIBUTO>();
        }

        [Key]
        [Column(Order = 0)]
        public int ID_ATRIBUTO { get; set; }

        [Required]
        [StringLength(50)]
        public string NM_ATRIBUTO { get; set; }

        [StringLength(400)]
        public string DS_ATRIBUTO { get; set; }

        [Required]
        [StringLength(10)]
        public string ID_ATRIBUTO_TIPO { get; set; }

        [StringLength(20)]
        public string VL_MASCARA_INGRESO { get; set; }

        [StringLength(20)]
        public string VL_MASCARA_DISPLAY { get; set; }

        [StringLength(10)]
        public string CD_DOMINIO { get; set; }

        [StringLength(100)]
        public string NM_CAMPO { get; set; }

        public short? NU_DECIMALES { get; set; }

        public string VL_SEPARADOR { get; set; }
        public short? NU_LARGO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_LPN_ATRIBUTO> T_LPN_ATRIBUTO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_LPN_DET_ATRIBUTO> T_LPN_DET_ATRIBUTO { get; set; }
    }
}
