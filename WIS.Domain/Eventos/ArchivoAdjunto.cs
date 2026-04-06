using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Eventos
{
    public class ArchivoAdjunto
    {
        public string NombreArchivo { get; set; }
        public long NU_ARCHIVO_ADJUNTO { get; set; }
        public int CD_EMPRESA { get; set; }
        public string CD_MANEJO { get; set; }
        public string DS_REFERENCIA { get; set; }
        public string DS_REFERENCIA2 { get; set; }
        public string TipoDocumento { get; set; }
        public string Observacion { get; set; }
        public string Anexo1 { get; set; }
        public string Anexo2 { get; set; }
        public string Anexo3 { get; set; }
        public string Anexo4 { get; set; }
        public string Anexo5 { get; set; }
        public string Anexo6 { get; set; }

        public short? CD_SITUACAO { get; set; }
        public long NU_VERSION_ACTIVA { get; set; }
        public DateTime? DT_ADDROW { get; set; }
        public DateTime? DT_UPDROW { get; set; }

        public virtual string GetNombreArchivo()
        {
            return this.NombreArchivo ?? $"{this.NU_ARCHIVO_ADJUNTO}#{this.NU_VERSION_ACTIVA}";
        }

    }
}
