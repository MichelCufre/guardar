namespace WIS.Domain.Picking
{
    public class PonderadorInstancia
    {
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public bool Habilitado { get; set; }
        public int? IncrementoDefault { get; set; }
        public int? PonderacionDefault { get; set; }
        public string TipoDato { get; set; }

    }
}
