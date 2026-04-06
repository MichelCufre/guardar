using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Impresiones
{
    public class ServidorImpresion
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public string ClientId { get; set; }
        public string DominioServidor { get; set; }
        public string PassServidor { get; set; }
        public string UrlServidor { get; set; }
        public string UserServidor { get; set; }
    }
}
