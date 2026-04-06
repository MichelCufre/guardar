namespace WIS.Domain.General.API.Dtos.Entrada
{
    public interface IApiEntradaRequest
    {
        public int Empresa { get; set; }

        public string DsReferencia { get; set; }

        public string Archivo { get; set; }

        public string IdRequest {  get; set; }

        public void Add(IApiEntradaItemRequest item);
    }
}
