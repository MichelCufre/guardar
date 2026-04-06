namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("V_FACTURA_UNIDAD_MEDID_WFAC200")]
    public partial class V_FACTURA_UNIDAD_MEDID_WFAC200
    {
        [Key]
        [StringLength(10)]
        public string CD_UNIDADE_MEDIDA { get; set; }
        
        [StringLength(30)]
        public string DS_UNIDADE_MEDIDA { get; set; }
        
        [StringLength(20)]
        public string NU_COMPONENTE { get; set; }
        
        [StringLength(1)]
        public string ID_USO { get; set; }
    }
}
