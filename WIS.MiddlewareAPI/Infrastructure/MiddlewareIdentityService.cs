using System;
using System.Globalization;
using WIS.Security;

namespace WIS.MiddlewareAPI.Infrastructure
{
    // Implementacion de IIdentityService para el contexto de la MiddlewareAPI.
    // No hay usuario autenticado, se usa un usuario de sistema fijo.
    public class MiddlewareIdentityService : IIdentityService
    {
        public int UserId { get; set; } = 0;
        public string Application { get; set; } = "MIDDLEWARE";
        public string Predio { get; set; } = string.Empty;

        public IFormatProvider GetFormatProvider() => CultureInfo.InvariantCulture;
    }
}
