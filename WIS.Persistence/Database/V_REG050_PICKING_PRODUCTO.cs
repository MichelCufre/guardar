namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;


    [Table("V_REG050_PICKING_PRODUCTO")]
    public partial class V_REG050_PICKING_PRODUCTO
    {
        public int NU_SEC_PICKING_PRODUTO { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

      
        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }


        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

      
        [StringLength(65)]
        [Column]
        public string DS_PRODUTO { get; set; }

        public decimal DS_QT_BULTO { get; set; }

        [StringLength(200)]
        [Column]
        public string AUXEMBR_2 { get; set; }

        [StringLength(200)]
        [Column]
        public string AUXEMBR_3 { get; set; }

       
        public decimal CD_FAIXA { get; set; }

      
        [StringLength(40)]
        [Column]
        public string CD_ENDERECO_SEPARACAO { get; set; }

        public int? QT_ESTOQUE_MINIMO { get; set; }

        public int? QT_ESTOQUE_MAXIMO { get; set; }

        public decimal QT_PADRAO_PICKING { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public int? QT_DESBORDE { get; set; }

      
        [StringLength(1)]
        [Column]
        public string TP_PICKING { get; set; }

        public DateTime? DT_UPDROW { get; set; }

      
        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }

      
        [StringLength(20)]
        public string CD_ZONA_UBICACION { get; set; }

        [StringLength(100)]
        public string DS_ZONA_UBICACION { get; set; }

        [StringLength(20)]
        public string CD_UNIDAD_CAJA_AUT { get; set; }

        public int? QT_UNIDAD_CAJA_AUT { get; set; }

        [StringLength(1)]
        public string FL_CONF_CD_BARRAS_AUT { get; set; }

        public int? NU_PRIORIDAD { get; set; }
    }
}
