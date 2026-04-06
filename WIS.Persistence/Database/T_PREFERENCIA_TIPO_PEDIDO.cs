namespace WIS.Persistence.Database
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_PREFERENCIA_TIPO_PEDIDO")]
    public partial class T_PREFERENCIA_TIPO_PEDIDO
    {
        [Key]
        public int NU_PREFERENCIA { get; set; }

        [Key]
        [StringLength(6)]
        public string TP_PEDIDO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }
    }
}
