namespace WIS.Domain.Automatismo
{
    public class AutomatismoCaracteristicaConfiguracion
    {
        public string TipoAutomatismo { get; set; }
        public string Codigo { get; set; }
        public string Valor { get; set; }
        public string Descripcion { get; set; }
        public string ValorAuxiliar { get; set; }
        public long? NumeroAuxiliar { get; set; }
        public decimal? CantidadAuxiliar { get; set; }
        public bool FlagAuxiliar { get; set; }
        public string Opciones { get; set; }
    }
}
