namespace WIS.Domain.Expedicion
{
    public class ConfiguracionExpedicionPedido
    {
        public string Tipo { get; set; }
        public string Descripcion { get; set; }
        public bool RequiereBox { get; set; }
        public string TipoArmadoEgreso { get; set; }
        public bool DebeFacturarEnEmpaquetado { get; set; }
        public bool CierreCamionEnBox { get; set; } //TODO: Definir que significa esto
        public bool DebeExpedirContenedor { get; set; } 
        public bool DebeEmpaquetarContenedor { get; set; }
        public bool TraspasoEmpresa { get; set; }
        public bool FlProduccion { get; set; } //TODO: Definir que significa esto
        public bool IsConsumoInterno { get; set; }
        public bool PermiteFacturacionParcial { get; set; }
        public bool FlCierreCamionEnEmpaque { get; set; }
        public bool FlFacturaAutoCompletar { get; set; }
        public bool FlPermiteAnulacionParcial { get; set; }
        public bool FlModalidadEmpaquetado { get; set; }
        public string CodigoGrupoExpedicion { get; set; }
        public bool IsDisponible { get; set; }
        public bool FlAnulaPendientesAlLiberar { get; set; }
        public bool DebeExpedirCompleto { get; set; }
        public bool PermiteFacturacionSinPrecinto { get; set; }
        public short? CantidadPrecintos { get; set; }
        public bool IsFacturacionRequerida { get; set; }
        public bool IsFacturacionParcial { get; set; }
        public bool IsFacturacionPorPedido { get; set; }
        public bool RequiereLiberacionTotal { get; set; }
        public bool FlTracking { get; set; }
        public bool FlPreparable { get; set; }
        public bool FlEmpaquetaSoloUnProducto { get; set; }
    }
}
