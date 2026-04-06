namespace WIS.Domain.Documento
{
    public class DocumentoTipo
    {
        public string TipoDocumento { get; set; }
        public string TipoOperacion { get; set; }
        public bool NumeroAutogenerado { get; set; }
        public bool IngresoManual { get; set; }
        public bool Habilitado { get; set; }
        public bool ManejaCambioEstado { get; set; }
        public bool RequiereDUA { get; set; }
        public bool RequiereDTI { get; set; }
        public bool RequiereReferenciaExterna { get; set; }
        public bool RequiereFactura { get; set; }
        public bool AutoAgendable { get; set; }
        public bool ManejaAgenda { get; set; }
        public bool ManejaCamion { get; set; }
        public string DescripcionTipoDocumento { get; set; }
        public string Secuencia { get; set; }
        public bool PermiteAgregarDetalle { get; set; }
        public bool PermiteRemoverDetalle { get; set; }
        public string Mask { get; set; }
        public string MaskChars { get; set; }
        public bool InterfazEntradaHabilitada { get; set; }
        public short LargoMaximoNumeroDocumento { get; set; }
        public short LargoPrefijo { get; set; }
        public decimal CantidadMinimaIngresada { get; set; }
    }
}
