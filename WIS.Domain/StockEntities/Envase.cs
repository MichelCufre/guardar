using System;
using WIS.Domain.DataModel.Mappers.Constants;

namespace WIS.Domain.StockEntities
{
    public class Envase
    {
        public string Id { get; set; }
        public string CodigoAgente { get; set; }
        public string TipoAgente { get; set; }
        public string TipoEnvase { get; set; }
        public string CodigoBarras { get; set; }
        public string Estado { get; set; }
        public string DescripcionUltimoMovimiento { get; set; }
        public string Observaciones { get; set; }
        public long? NumeroInterfaz { get; set; }
        public DateTime? FechaAlta { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public int? Empresa { get; set; }
        public DateTime? FechaUltimaRecepcion { get; set; }
        public DateTime? FechaUltimaCargaEnCamion { get; set; }
        public DateTime? FechaUltimaExpedicion { get; set; }
        public int? UsuarioUltimaRecepcion { get; set; }
        public int? UsuarioUltimaCargaEnCamion { get; set; }
        public int? UsuarioUltimaExpedicion { get; set; }
        public long? NumeroTransaccion { get; set; }


        public virtual void MarcarRetornoDeposito(string predio)
        {
            Empresa = null;
            CodigoAgente = predio;
            TipoAgente = TipoAgenteDb.Deposito;
            FechaModificacion = DateTime.Now;
        }


    }
}
