namespace WIS.Domain.Automatismo.Dtos
{
    public interface IApiAutomatismoRequest
    {
        public int Empresa { get; set; }

        public string DsReferencia { get; set; }

        public string Archivo { get; set; }

        public void Add(IApiAutomatismoItemRequest item);
    }
}
