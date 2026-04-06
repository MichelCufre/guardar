namespace WIS.Domain.Parametrizacion
{
    public class LpnTipoPuntajePicking
    {
        public decimal? IgualSinReserva { get; set; }
        public decimal? IgualConReserva { get; set; }
        public decimal? MenorSinReserva { get; set; }
        public decimal? MenorConReserva { get; set; }
        public decimal? Mayor { get; set; }
        public decimal? Inexistente { get; set; }
        public decimal? Bonus { get; set; }
    }
}
