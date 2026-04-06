using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WIS.Persistence.Database
{
    [Table("I_E_PRDC_SALIDA_PRD_INSUMO")]
    public partial class I_E_PRDC_SALIDA_PRD_INSUMO
    {
        [StringLength(1)]
        public string ID_PROCESADO { get; set; }

        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long NU_INTERFAZ_EJECUCION { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string NU_REGISTRO { get; set; }

        [Required]
        [StringLength(20)]
        public string NU_REGISTRO_PADRE { get; set; }

        [Required]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        public decimal CD_FAIXA { get; set; }

        [Required]
        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        [Required]
        [StringLength(40)]
        public string ND_ACCION_MOVIMIENTO { get; set; }

        public decimal QT_SALIDA { get; set; }

        [StringLength(2)]
        public string FL_SEMIACABADO { get; set; }
        [StringLength(1)]
        public string FL_CONSUMIBLE { get; set; }
    }

}
