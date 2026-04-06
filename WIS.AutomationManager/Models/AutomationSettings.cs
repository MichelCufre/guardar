namespace WIS.AutomationManager.Models
{
    public class AutomationSettings
    {
        public const string Position = "AutomationSettings";

        public int MaxProductos { get; set; }
        public int MaxCodigosBarras { get; set; }
        public int MaxEntradas { get; set; }
        public int MaxSalidas { get; set; }
        public int MaxNotificacionAjuste { get; set; }
        public int MaxConfirmacionEntrada { get; set; }
        public int MaxConfirmacionSalida { get; set; }
        public int MaxUbicacionesPicking { get; set; }
    }
}
