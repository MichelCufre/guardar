namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_PRE060_CONTENEDOR")]
    public partial class V_PRE060_CONTENEDOR
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_PREPARACION { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_CONTENEDOR { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(10)]
        public string TP_CONTENEDOR { get; set; }

        public short? CD_SITUACAO { get; set; }

        [StringLength(20)]
        [Column]
        public string ID_PRECINTO_1 { get; set; }

        [StringLength(20)]
        [Column]
        public string ID_PRECINTO_2 { get; set; }

        public int? CD_CAMION_FACTURADO { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_ENDERECO { get; set; }

        [StringLength(2)]
        [Column]
        public string CD_SUB_CLASSE { get; set; }

        public short? CD_PORTA { get; set; }

        public int? CD_CAMION { get; set; }

        public DateTime? DT_PULMON { get; set; }

        public DateTime? DT_EXPEDIDO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public int? QT_BULTO { get; set; }

        public int? CD_FUNCIONARIO_EXPEDICION { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_HABILITADO { get; set; }

        [StringLength(30)]
        [Column]
        public string DS_SITUACAO { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_SUB_CLASSE { get; set; }

        [StringLength(30)]
        [Column]
        public string DS_PORTA { get; set; }

        [StringLength(100)]
        [Column]
        public string FULLNAME { get; set; }

        [StringLength(50)]
        [Column]
        public string LOGINNAME { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_CLIENTE { get; set; }

        public int? CD_EMPRESA { get; set; }

        [StringLength(50)]
        [Column]
        public string DS_CAMION { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_AGENTE { get; set; }

        [StringLength(3)]
        [Column]
        public string TP_AGENTE { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_TIPO_AGENTE { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_AGENTE { get; set; }

        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(10)]
        public string NU_PREDIO { get; set; }

        [StringLength(1)]
        [Column]
        public string VL_CONTROL { get; set; }

        public short CD_AREA_ARMAZ { get; set; }

        [StringLength(15)]
        [Column]
        public string DS_AREA_ARMAZ { get; set; }

        [Column]
        [StringLength(40)]
        public string NU_PEDIDO { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_SEPARADO_DOS_FASES { get; set; }

		[StringLength(50)]
		[Column]
		public string ID_EXTERNO_CONTENEDOR { get; set; }

        [StringLength(60)]
        [Column]
        public string DS_CONTENEDOR { get; set; }

        [StringLength(100)]
        [Column]
        public string CD_BARRAS { get; set; }

        public long? NU_LPN { get; set; }

        public int? NU_UNIDAD_TRANSPORTE { get; set; }

        [StringLength(40)]
        public string NU_EXTERNO_UNIDAD { get; set; }

        [StringLength(3)]
        public string TP_UNIDAD_TRANSPORTE { get; set; }

        [StringLength(50)]
        public string CD_BARRAS_UT { get; set; }
    }
}
