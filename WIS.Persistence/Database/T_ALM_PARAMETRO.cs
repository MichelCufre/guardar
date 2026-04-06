namespace WIS.Persistence.Database
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;


    [Table("T_ALM_PARAMETRO")]
    public partial class T_ALM_PARAMETRO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_ALM_PARAMETRO()
        {
            this.T_ALM_LOGICA_INSTANCIA_PARAM = new HashSet<T_ALM_LOGICA_INSTANCIA_PARAM>();
        }

        [Key]
        public short NU_ALM_PARAMETRO { get; set; }
        public string NM_ALM_PARAMETRO { get; set; }
        public string DS_ALM_PARAMETRO { get; set; }
        public string VL_ALM_PARAMETRO_DEFAULT { get; set; }
        public string VL_ALM_VISTA_LOV { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_ALM_LOGICA_INSTANCIA_PARAM> T_ALM_LOGICA_INSTANCIA_PARAM { get; set; }
    }
}
