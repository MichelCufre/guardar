namespace WIS.Documento.Execution
{
    public class ModificarReservaDocumentalResponse
    {
        public bool Success { get; set; }
        public string ErrorMsg { get; set; }
        public string[] StrArguments { get; set; }

        public ModificarReservaDocumentalResponse()
        {
            Success = true;
        }
    }
}
