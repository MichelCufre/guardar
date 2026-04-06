namespace WIS.Domain.Documento.Serializables.Entrada
{
    public class DocumentoUpdateRequest
    {
        public string Numero { get; set; }
        public string Tipo { get; set; }
        public int NumeroAgenda { get; set; }
    }
}
