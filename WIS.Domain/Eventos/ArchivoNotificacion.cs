namespace WIS.Domain.Eventos
{
    public partial class eeeArchivoNotificacion
    {
        public int Id { get; set; }

        public int NumeroNotificacion { get; set; }

        public string Url { get; set; }

        public Notificacion Notificacion { get; set; }

    }
}
