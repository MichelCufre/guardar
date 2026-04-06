using System;
using System.Collections.Generic;
using WIS.Domain.DataModel;

namespace WIS.Domain.Documento
{
    public interface IDocumento : IDocumentoBase
    {
        string Descripcion { get; set; }
        string Estado { get; set; }
        int? Usuario { get; set; }
        DocumentoAduana DocumentoAduana { get; set; }
        DocumentoReferenciaExterna DocumentoReferenciaExterna { get; set; }
        string Moneda { get; set; }
        decimal? ValorArbitraje { get; set; }
        bool GeneraAgenda { get; set; }
        int? Agenda { get; set; }
        short? Situacion { get; set; }
        DateTime? FechaAlta { get; set; }
        DateTime? FechaModificacion { get; set; }
        int? Empresa { get; set; }
        string Via { get; set; }
        string Factura { get; set; }
        string Conocimiento { get; set; }
        decimal? CantidadBulto { get; set; }
        int? Transportista { get; set; }
        DateTime? FechaProgramado { get; set; }
        string UnidadMedida { get; set; }
        string NumeroImportacion { get; set; }
        string NumeroExportacion { get; set; }
        short? Despachante { get; set; }
        decimal? ValorSeguro { get; set; }
        DateTime? FechaEnviado { get; set; }
        decimal? Volumen { get; set; }
        decimal? Peso { get; set; }
        DateTime? FechaFinalizado { get; set; }
        int? Camion { get; set; }
        short? TipoAlmacenajeYSeguro { get; set; }
        string Predio { get; set; }
        short? CantidadContenedor20 { get; set; }
        short? CantidadContenedor40 { get; set; }
        DateTime? FechaFacturado { get; set; }
        decimal? ValorFlete { get; set; }
        string IdManual { get; set; }
        int? Proveedor { get; set; }
        bool AgendarAutomaticamente { get; set; }
        decimal? ValorOtrosGastos { get; set; }
        string DocumentoTransporte { get; set; }
        string Anexo1 { get; set; }
        string Anexo2 { get; set; }
        string Anexo3 { get; set; }
        string Anexo4 { get; set; }
        string Anexo5 { get; set; }
        string Anexo6 { get; set; }
        bool Ficticio { get; set; }
        string NumeroCorrelativo { get; set; }
        string NumeroDTI { get; set; }
        DateTime? FechaDTI { get; set; }
        DateTime? FechaDeclarado { get; set; }
        DateTime? FechaMovilizacionContenedor { get; set; }
        string NumeroCorrelativo2 { get; set; }
        bool Validado { get; set; }
        string Cliente { get; set; }
        string NumeroAgrupador { get; set; }
        string TipoAgrupador { get; set; }
        decimal? ICMS { get; set; }
        decimal? II { get; set; }
        decimal? IPI { get; set; }
        decimal? IISUSPENSO { get; set; }
        decimal? PISCONFINS { get; set; }   
        decimal? IPISUSPENSO { get; set; }
        int? RegimenAduana { get; set; }
        string NumeroDocumento1 { get; set; }
        string NumeroDocumento2 { get; set; }
        DocumentoConfiguracion Configuracion { get; set; }
        long? NumeroTransaccionDelete { get; set; }

        string GetNumeroDocumento(IUnitOfWork uow);
        string GetDescripcionEstado(IUnitOfWork uow);
        DocumentoLinea GetLinea(string cdProducto, string nroIdentificador, decimal faixa);
        bool CanEdit(IUnitOfWork uow);
        List<DocumentoAccion> GetEstadosHabilitadosParaCambio(IUnitOfWork uow);
        void Editar();
        void ConfirmarEdicion();
        void Cancelar();
        void GenerarLineas();
        void EnviarDocumento();
        void AprobarDocumento(DocumentoAduana dti, DocumentoAduana dua, int? nroAgenda);
        void IniciarVerificacion();
        void Finalizar();
        void Agrupar(string numeroAgrupador, string tipoAgrupador);
        void Desagrupar();
    }
}
