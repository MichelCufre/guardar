using System;
using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Domain.Documento.Constants;
using WIS.Domain.Expedicion;
using WIS.Domain.General;

namespace WIS.Domain.Documento
{
    public class DocumentoAgrupador : IDocumentoAgrupador
    {
        public string Numero { get; set; }
        public DocumentoAgrupadorTipo Tipo { get; set; }
        public string Estado { get; set; }
        public DateTime? FechaAlta { get; set; }
        public int? Cantidad { get; set; }
        public string NumeroLacre { get; set; }
        public decimal? ValorTotal { get; set; }
        public decimal? Peso { get; set; }
        public DateTime? FechaSalida { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public DateTime? FechaLlegada { get; set; }
        public int? Empresa { get; set; }
        public decimal? PesoLiquido { get; set; }
        public string Motorista { get; set; }
        public string Placa { get; set; }
        public string Anexo1 { get; set; }
        public string Anexo2 { get; set; }
        public string Anexo3 { get; set; }
        public string Anexo4 { get; set; }
        public string Motivo { get; set; }
        public string Predio { get; set; }
        public Transportista Transportadora { get; set; }
        public VehiculoEspecificacion TipoVehiculo { get; set; }
        public virtual List<IDocumentoIngreso> LineasIngresoAgrupadas { get; set; }
        public virtual List<IDocumentoEgreso> LineasEgresoAgrupadas { get; set; }
        public DateTime? FechaImpreso { get; set; }

        public virtual void ConfirmarAgrupador()
        {
            this.Estado = EstadoDocumentoAgrupador.CONFIRMADO;
            this.FechaActualizacion = DateTime.Now;
        }

        public virtual void EnviarAgrupador()
        {
            this.Estado = EstadoDocumentoAgrupador.ENVIADO;
            this.FechaActualizacion = DateTime.Now;
        }

        public virtual void CancelarAgrupador()
        {
            this.Estado = EstadoDocumentoAgrupador.CANCELADO;
            this.FechaActualizacion = DateTime.Now;
        }

        public virtual string ObtenerNumeroAgrupador(IUnitOfWork uow)
        {
            return uow.DocumentoRepository.GetNumeroAgrupador(Tipo.Secuencia);
        }

        public virtual void MarcarImpresion()
        {
            this.FechaImpreso = DateTime.Now;
        }
    }
}