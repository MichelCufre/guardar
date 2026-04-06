using System;
using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Domain.Expedicion;
using WIS.Domain.General;

namespace WIS.Domain.Documento
{
    public interface IDocumentoAgrupador
    {
        string Numero { get; set; }
        DocumentoAgrupadorTipo Tipo { get; set; }
        string Estado { get; set; }
        DateTime? FechaAlta { get; set; }
        int? Cantidad { get; set; }
        string NumeroLacre { get; set; }
        decimal? ValorTotal { get; set; }
        decimal? Peso { get; set; }
        DateTime? FechaSalida { get; set; }
        DateTime? FechaLlegada { get; set; }
        DateTime? FechaActualizacion { get; set; }
        DateTime? FechaImpreso { get; set; }
        int? Empresa { get; set; }
        decimal? PesoLiquido { get; set; }
        string Motorista { get; set; }
        string Placa { get; set; }
        string Anexo1 { get; set; }
        string Anexo2 { get; set; }
        string Anexo3 { get; set; }
        string Anexo4 { get; set; }
        string Predio { get; set; }
        string Motivo { get; set; }
        Transportista Transportadora { get; set; }
        VehiculoEspecificacion TipoVehiculo { get; set; }
        List<IDocumentoIngreso> LineasIngresoAgrupadas { get; set; }
        List<IDocumentoEgreso> LineasEgresoAgrupadas { get; set; }
        string ObtenerNumeroAgrupador(IUnitOfWork uow);
        void ConfirmarAgrupador();
        void EnviarAgrupador();
        void CancelarAgrupador();
        void MarcarImpresion();
    }
}
