namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("V_PAR400_ATRIBUTOS")]
    public partial class V_PAR400_ATRIBUTOS
    {
        [StringLength(10)]
        [Column]
        public string CD_DOMINIO { get; set; }

        [StringLength(40)]
        [Column]
        public string DS_ATRIBUTO { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_ATRIBUTO_TIPO { get; set; }

        [Key]
        public int ID_ATRIBUTO { get; set; }

        [StringLength(10)]
        [Column]
        public string ID_ATRIBUTO_TIPO { get; set; }

        [StringLength(50)]
        [Column]
        public string NM_ATRIBUTO { get; set; }

        [StringLength(100)]
        [Column]
        public string NM_CAMPO { get; set; }

        public short? NU_LARGO { get; set; }

        public short? NU_DECIMALES { get; set; }

        [StringLength(20)]
        [Column]
        public string VL_MASCARA_DISPLAY { get; set; }

        [StringLength(20)]
        [Column]
        public string VL_MASCARA_INGRESO { get; set; }

        [StringLength(1)]
        [Column]
        public string VL_SEPARADOR { get; set; }

    }
}
