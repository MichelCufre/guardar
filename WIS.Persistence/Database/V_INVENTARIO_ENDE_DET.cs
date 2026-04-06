using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_INVENTARIO_ENDE_DET")]
    public partial class V_INVENTARIO_ENDE_DET
    {
        [Key]
        public decimal NU_INVENTARIO { get; set; }

        [StringLength(200)]
        public string DS_INVENTARIO { get; set; }

        [StringLength(9)]
        public string TP_INVENTARIO { get; set; }

        [StringLength(20)]
        public string ND_CIERRE_CONTEO { get; set; }

        [StringLength(20)]
        public string ND_ESTADO_INVENTARIO { get; set; }

        public int? CD_EMPRESA_INV { get; set; }

        [StringLength(10)]
        public string NU_PREDIO { get; set; }

        public DateTime? DT_ADDROW_INV { get; set; }

        public DateTime? DT_UPDROW_INV { get; set; }

        public decimal? NU_CONTEO_INV { get; set; }

        [StringLength(1)]
        public string FL_CREACION_WEB { get; set; }

        [StringLength(1)]
        public string FL_SOLO_REGISTROS_FOTO { get; set; }

        [StringLength(1)]
        public string FL_CONTROLAR_VENCIMIENTO { get; set; }

        [StringLength(1)]
        public string FL_MODIFICAR_STOCK_EN_DIF { get; set; }

        [StringLength(1)]
        public string FL_PERMITE_INGRESAR_MOTIVO { get; set; }

        [StringLength(1)]
        public string FL_BLOQ_USR_CONTEO_SUCESIVO { get; set; }

        [StringLength(1)]
        public string FL_ACTUALIZAR_CONTEO_FIN_AUTO { get; set; }

        [StringLength(1)]
        public string FL_EXCLUIR_SUELTOS { get; set; }

        [StringLength(1)]
        public string FL_EXCLUIR_LPNS { get; set; }

        [StringLength(1)]
        public string FL_RESTABLECER_LPN_FINALIZADO { get; set; }

        [StringLength(1)]
        public string FL_GENERAR_PRIMER_CONTEO { get; set; }

        [Key]
        public decimal NU_INVENTARIO_ENDERECO { get; set; }

        [StringLength(40)]
        public string CD_ENDERECO { get; set; }

        [StringLength(20)]
        public string ND_ESTADO_INVENTARIO_ENDERECO { get; set; }


        [Key]
        public decimal NU_INVENTARIO_ENDERECO_DET { get; set; }

        public decimal? NU_CONTEO_INV_DET { get; set; }

        public int? CD_EMPRESA_INV_DET { get; set; }

        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        public decimal? CD_FAIXA { get; set; }

        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        public DateTime? DT_VENCIMIENTO { get; set; }

        public decimal? QT_INVENTARIO { get; set; }

        public decimal? QT_DIFERENCIA { get; set; }

        [StringLength(20)]
        public string ND_ESTADO_INV_ENDERECO_DET { get; set; }

        public long? NU_LPN { get; set; }

        public int? ID_LPN_DET { get; set; }

        public DateTime? DT_ADDROW_INV_DET { get; set; }

        public DateTime? DT_UPDROW_INV_DET { get; set; }

        [StringLength(3)]
        public string CD_MOTIVO_AJUSTE { get; set; }

        public long? NU_INSTANCIA_CONTEO { get; set; }

        public int? CD_USUARIO { get; set; }

        public decimal? QT_TIEMPO_INSUMIDO { get; set; }

        [StringLength(1)]
        public string FL_CONTEO_ESPERADO { get; set; }
    }
}
