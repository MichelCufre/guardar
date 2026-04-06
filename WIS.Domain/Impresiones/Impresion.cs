using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers.Constants;

namespace WIS.Domain.Impresiones
{
    public class Impresion
    {
        public int Id { get; set; }
        public int? Usuario { get; set; }
        public string CodigoImpresora { get; set; }
        public string Referencia { get; set; }
        public DateTime? Generado { get; set; }
        public DateTime? Procesado { get; set; }
        public string Estado { get; set; }
        public string Predio { get; set; }
        public string NombreImpresora { get; set; }
        public string Estilo { get; set; }
        public int? CantRegistros { get; set; }
        public string Error { get; set; }

        public List<DetalleImpresion> Detalles { get; set; }

        public Impresion()
        {
            this.Detalles = new List<DetalleImpresion>();
        }

        public virtual bool IsTodoRecibido()
        {
            return this.Detalles.All(d => d.Estado == EstadoImpresionDb.Recibido);
        }

        public virtual bool AnyError()
        {
            return this.Detalles.Any(d => d.Estado == EstadoImpresionDb.EnvioFallido);
        }

        public virtual void SetEstadoRecibido()
        {
            this.Procesado = DateTime.Now;
            this.Estado = EstadoImpresionDb.Recibido;
        }

        public virtual void SetEstadoError(string error)
        {
            this.Procesado = DateTime.Now;
            this.Error = error;
            this.Estado = EstadoImpresionDb.EnvioFallido;
        }
    }
}
