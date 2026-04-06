namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_DOCUMENTO_DOC095")]
    public partial class V_DOCUMENTO_DOC095
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(10)]
        public string NU_DOCUMENTO { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(6)]
        public string TP_DOCUMENTO { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_DOCUMENTO { get; set; }

        [StringLength(6)]
        [Column]
        public string TP_DUA { get; set; }

        [StringLength(25)]
        [Column]
        public string NU_DUA { get; set; }

        public DateTime? DT_DECLARADO { get; set; }

        public int? CD_EMPRESA { get; set; }

        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }

        public long? QT_LINEAS { get; set; }

        public long? QT_PRODUCTO { get; set; }

        public decimal? QT_DOCUMENTO { get; set; }

        public decimal? QT_CIF { get; set; }

        [StringLength(6)]
        [Column]
        public string ID_ESTADO { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_ESTADO { get; set; }

        public DateTime? DT_FINALIZADO { get; set; }

        public int? NU_AGENDA { get; set; }

        public int? CD_CAMION { get; set; }

        public decimal? VL_SEGURO { get; set; }

        public decimal? VL_FLETE { get; set; }

        [StringLength(200)]
        [Column]
        public string NU_FACTURA { get; set; }

        public DateTime? DT_FACTURACION { get; set; }

        public int? QT_BULTO { get; set; }

        public decimal? QT_PESO { get; set; }

        public decimal? QT_VOLUMEN { get; set; }

        public int? CD_FUNCIONARIO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        [StringLength(50)]
        [Column]
        public string NU_CORRELATIVO { get; set; }

        [StringLength(50)]
        [Column]
        public string NU_CORRELATIVO_2 { get; set; }

        [StringLength(15)]
        [Column]
        public string CD_MONEDA { get; set; }

        public decimal? VL_ARBITRAJE { get; set; }

        public DateTime? DT_DUA { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_GENERAR_AGENDA { get; set; }

        public short? CD_SITUACAO { get; set; }

        [StringLength(4)]
        [Column]
        public string CD_VIA { get; set; }

        [StringLength(12)]
        [Column]
        public string NU_CONOCIMIENTO { get; set; }

        public int? CD_TRANSPORTISTA { get; set; }

        public DateTime? DT_PROGRAMADO { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_UNIDAD_MEDIDA_BULTO { get; set; }

        [StringLength(20)]
        [Column]
        public string NU_IMPORT { get; set; }

        [StringLength(20)]
        [Column]
        public string NU_EXPORT { get; set; }

        public short? CD_DESPACHANTE { get; set; }

        public DateTime? DT_ENVIADO { get; set; }

        public short? TP_ALMACENAJE_Y_SEGURO { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }

        public short? QT_CONTENEDOR20 { get; set; }

        public short? QT_CONTENEDOR40 { get; set; }

        public decimal? VL_OTROS_GASTOS { get; set; }

        [StringLength(30)]
        [Column]
        public string NU_DOC_TRANSPORTE { get; set; }

        [StringLength(1)]
        [Column]
        public string VL_VALIDADO { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ANEXO1 { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ANEXO2 { get; set; }

        [StringLength(30)]
        [Column]
        public string NU_DTI { get; set; }

        public DateTime? DT_DTI { get; set; }

        [StringLength(30)]
        [Column]
        public string NU_REFERENCIA_EXTERNA { get; set; }

        [StringLength(6)]
        [Column]
        public string TP_REFERENCIA_EXTERNA { get; set; }

        public DateTime? DT_REFERENCIA_EXTERNA { get; set; }
    }
}
