namespace WIS.Domain.General.API.Dtos.Entrada
{
    public class UsuarioRequest
    {
        public string LoginName { get; set; }
        public byte[] Hash { get; set; }
    }
}
