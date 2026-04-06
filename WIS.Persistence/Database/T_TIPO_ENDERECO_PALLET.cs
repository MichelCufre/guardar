namespace WIS.Persistence.Database
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_TIPO_ENDERECO_PALLET")]
    public partial class T_TIPO_ENDERECO_PALLET
    {
        [Key]
        public short CD_TIPO_ENDERECO { get; set; }
        [Key]
        public short CD_PALLET { get; set; }
        public short QT_PALLET { get; set; }

        public virtual T_TIPO_ENDERECO T_TIPO_ENDERECO { get; set; }
    }
}
