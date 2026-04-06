namespace WIS.Documento.Execution
{
    public class DocumentoCambioLoteResponse
    {
        public string Tipo { get; set; }
        public string Numero { get; set; }

        public DocumentoCambioLoteResponse(string tipo, string numero)
        {
            Tipo = tipo;
            Numero = numero;
        }
    }
}
