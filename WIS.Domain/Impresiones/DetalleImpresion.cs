using System;
using WIS.Domain.DataModel.Mappers.Constants;

namespace WIS.Domain.Impresiones
{
    public class DetalleImpresion
    {
        public int Registro { get; set; }
        public int NumeroImpresion { get; set; }
        public string Contenido { get; set; }
        public string Estado { get; set; }
        public DateTime? FechaProcesado { get; set; }
        public string Error { get; set; }

        public virtual void SetEstadoRecibido()
        {
            this.FechaProcesado = DateTime.Now;
            this.Estado = EstadoImpresionDb.Recibido;
        }

        public virtual void SetEstadoError(string error)
        {
            this.FechaProcesado = DateTime.Now;
            this.Error = error;
            this.Estado = EstadoImpresionDb.EnvioFallido;
        }
    }
}
