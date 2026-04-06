namespace WIS.Domain.Security
{
    public class UsuarioPermiso
    {
        public int Id { get; set; }
        public string UniqueName { get; set; }

        public UsuarioPermiso()
        {
        }

        public UsuarioPermiso(int id, string uniquename)
        {
            Id = id;
            UniqueName = uniquename;
        }
    }
}
