namespace WIS.TrafficOfficer
{
    public class TrafficOfficerErrorCode
    {
        public const string ErrorGenerico = "100";
        public const string ErrorGenericoNoControlado = "101";
        public const string ErrorGenericoControlado = "200"; //Si es controlado por que es generico?
        public const string UsuarioRequerido = "201";
        public const string SistemaRequerido = "202"; //Que es el sistema?
        public const string PaginaRequerida = "203";
        public const string TokenRequerido = "204";
        public const string EntidadRequerida = "205";
        public const string IdBloqueoRequerido = "206";
        public const string TokenNoCreado = "207"; //Esto no es una razon es un resultado
        public const string PaginaOrigenRequerida = "208";
        public const string PaginaDestinoRequerida = "209";
        public const string PaginaSinBloqueosTraspaso = "210";
        public const string ErrorGenericoNegocio = "300"; //???
        public const string RegistroBloqueado = "301";
        public const string ErrorAddLock = "400"; //???
        public const string TooManySessions = "500";
        public const string ExpiredLicense = "501";
        public const string InvalidLicense = "502";
    }
}
