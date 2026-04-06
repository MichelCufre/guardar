namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_INT103_ESTAN_REF_RECEPC_DET")]
    public partial class V_INT103_ESTAN_REF_RECEPC_DET
    {
        [StringLength(50)]
        [Column]
        public string CD_PRODUTO { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ANEXO { get; set; }

        [StringLength(50)]
        [Column]
        public string DT_VENCIMIENTO { get; set; }

        [StringLength(50)]
        [Column]
        public string ID_LINEA_SISTEMA_EXTERNO { get; set; }

        [StringLength(50)]
        [Column]
        public string NU_IDENTIFICADOR { get; set; }

        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long NU_INTERFAZ_EJECUCION { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string NU_REGISTRO { get; set; }

        [StringLength(50)]
        [Column]
        public string NU_REGISTRO_PADRE { get; set; }

        [StringLength(50)]
        [Column]
        public string QT_REFERENCIA { get; set; }
    }
}
