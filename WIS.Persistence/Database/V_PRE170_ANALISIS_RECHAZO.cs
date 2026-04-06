namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_PRE170_ANALISIS_RECHAZO")]
    public partial class V_PRE170_ANALISIS_RECHAZO
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_PREPARACION { get; set; }

        public DateTime? DT_INICIO { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [Required]
        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [Required]
        [StringLength(65)]
        [Column]
        public string DS_PRODUTO { get; set; }

        public decimal CD_FAIXA { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(1)]
        public string ID_ESPECIFICA_IDENTIFICADOR { get; set; }

        public decimal? QT_RECHAZADO { get; set; }

        public decimal? QT_ESTOQUE { get; set; }

        public decimal? QT_AVERIA { get; set; }

        public decimal? QT_PREPARACION { get; set; }

        public decimal? QT_DIFERENCIA { get; set; }

        public decimal? QT_CONTENEDOR { get; set; }

        public decimal? QT_PENDIENTE_ALMACENAR { get; set; }

        public decimal? QT_TRANSFERENCIA { get; set; }

        public decimal? QT_CTRL_CALIDAD { get; set; }

        public decimal? QT_DUAS { get; set; }

        public decimal? QT_EN_AREA_NO_DISP { get; set; }

        public decimal? QT_VENCIDO { get; set; }

        public decimal? QT_DESPREPARADO { get; set; }

        public decimal? QT_OTRO_PREDIO { get; set; }

        public decimal? QT_ESTOQUE_ALMACEN { get; set; }

        public decimal? QT_ESTOQUE_MENUDENCIA { get; set; }

        public decimal? QT_DUAS_FILTRO { get; set; }

        [StringLength(2)]
        [Column]
        public string NU_PREDIO { get; set; }
    }
}
