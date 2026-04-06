namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_DOCUMENTO")]
    public partial class T_DOCUMENTO
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

        public int? CD_FUNCIONARIO { get; set; }

        [StringLength(2)]
        [Column]
        public string TP_DUA { get; set; }

        [StringLength(25)]
        [Column]
        public string NU_DUA { get; set; }

        [StringLength(15)]
        [Column]
        public string CD_MONEDA { get; set; }

        public decimal? VL_ARBITRAJE { get; set; }

        public DateTime? DT_DUA { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_GENERAR_AGENDA { get; set; }

        public int? NU_AGENDA { get; set; }

        public short? CD_SITUACAO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public int? CD_EMPRESA { get; set; }

        [StringLength(4)]
        [Column]
        public string CD_VIA { get; set; }

        [StringLength(200)]
        [Column]
        public string NU_FACTURA { get; set; }

        [StringLength(12)]
        [Column]
        public string NU_CONOCIMIENTO { get; set; }

        public decimal? QT_BULTO { get; set; }

        public int? CD_TRANSPORTISTA { get; set; }

        [StringLength(6)]
        [Column]
        public string ID_ESTADO { get; set; }

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

        public decimal? VL_SEGURO { get; set; }

        public DateTime? DT_ENVIADO { get; set; }

        public decimal? QT_VOLUMEN { get; set; }

        public decimal? QT_PESO { get; set; }

        public DateTime? DT_FINALIZADO { get; set; }

        public int? CD_CAMION { get; set; }

        public short? TP_ALMACENAJE_Y_SEGURO { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }

        public short? QT_CONTENEDOR20 { get; set; }

        public short? QT_CONTENEDOR40 { get; set; }

        public DateTime? DT_FACTURACION { get; set; }

        public decimal? VL_FLETE { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_MANUAL { get; set; }

        public int? CD_FORNECEDOR { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_AGENDAR_AUTOMATICAMENTE { get; set; }

        public decimal? VL_OTROS_GASTOS { get; set; }

        [StringLength(30)]
        [Column]
        public string NU_DOC_TRANSPORTE { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ANEXO1 { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ANEXO2 { get; set; }

        [StringLength(200)]
        public string DS_ANEXO3 { get; set; }

        [StringLength(200)]
        public string DS_ANEXO4 { get; set; }

        [StringLength(200)]
        public string DS_ANEXO5 { get; set; }

        [StringLength(200)]
        public string DS_ANEXO6 { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_FICTICIO { get; set; }

        [StringLength(50)]
        [Column]
        public string NU_CORRELATIVO { get; set; }

        [StringLength(30)]
        [Column]
        public string NU_DTI { get; set; }

        public DateTime? DT_DTI { get; set; }

        [StringLength(6)]
        [Column]
        public string TP_REFERENCIA_EXTERNA { get; set; }

        [StringLength(30)]
        [Column]
        public string NU_REFERENCIA_EXTERNA { get; set; }

        public DateTime? DT_REFERENCIA_EXTERNA { get; set; }

        public DateTime? DT_DECLARADO { get; set; }

        public DateTime? DT_MOVILIZA_CONTENEDOR { get; set; }

        [StringLength(50)]
        [Column]
        public string NU_CORRELATIVO_2 { get; set; }

        [StringLength(1)]
        [Column]
        public string VL_VALIDADO { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_CLIENTE { get; set; }

        [StringLength(64)]
        [Column]
        public string VL_DATO_AUDITORIA { get; set; }

        [StringLength(20)]
        public string NU_AGRUPADOR { get; set; }

        [StringLength(3)]
        public string TP_AGRUPADOR { get; set; }

        [StringLength(1)]
        public string FL_BALANCEADO { get; set; }

        [StringLength(20)]
        public string TP_DOCUMENTO_EXTERNO { get; set; }

        public decimal? ICMS { get; set; }

        public decimal? II { get; set; }

        public decimal? IPI { get; set; }

        public decimal? IISUSPENSO { get; set; }

        public decimal? IPISUSPENSO { get; set; }

        public decimal? PISCONFINS { get; set; }

        public int? CD_REGIMEN_ADUANA { get; set; }

        [StringLength(50)]
        [Column]
        public string NU_DOC1 { get; set; }

        [StringLength(50)]
        [Column]
        public string NU_DOC2 { get; set; }

        public long? NU_TRANSACCION_DELETE { get; set; }

        public virtual T_DOCUMENTO_TIPO T_DOCUMENTO_TIPO { get; set; }

        public virtual T_DOCUMENTO_TIPO_ESTADO T_DOCUMENTO_TIPO_ESTADO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_DET_DOCUMENTO> T_DET_DOCUMENTO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_DET_DOCUMENTO_EGRESO> T_DET_DOCUMENTO_EGRESO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_DET_DOCUMENTO_EGRESO> T_DET_DOCUMENTO_EGRESO_INGRESO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_DET_DOCUMENTO_ACTA> T_DET_DOCUMENTO_ACTA { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_DET_DOCUMENTO_ACTA> T_DET_DOCUMENTO_ACTA_INGRESO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_DOCUMENTO_PREPARACION_RESERV> T_DOCUMENTO_PREPARACION_RESERV { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LT_DELETE_DOCUMENTO_PREPARACION_RESERV> LT_DELETE_DOCUMENTO_PREPARACION_RESERV { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_DOCUMENTO_PRODUCCION> T_DOCUMENTO_PRODUCCION_INGRESO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_DOCUMENTO_PRODUCCION> T_DOCUMENTO_PRODUCCION_EGRESO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_DOCUMENTO_TRANSFERENCIA> T_DOCUMENTO_TRANSFERENCIA_INGRESO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_DOCUMENTO_TRANSFERENCIA> T_DOCUMENTO_TRANSFERENCIA_EGRESO { get; set; }
    }
}
