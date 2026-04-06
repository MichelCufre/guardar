using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.Documento.Constants;

namespace WIS.Domain.Documento
{
    public abstract partial class Documento : IDocumento
    {
        public string Numero { get; set; }
        public string Tipo { get; set; }
        public string Descripcion { get; set; }
        public string Estado { get; set; }
        public int? Usuario { get; set; }
        public DocumentoAduana DocumentoAduana { get; set; }
        public DocumentoReferenciaExterna DocumentoReferenciaExterna { get; set; }
        public string Moneda { get; set; }
        public decimal? ValorArbitraje { get; set; }
        public bool GeneraAgenda { get; set; }
        public int? Agenda { get; set; }
        public short? Situacion { get; set; }
        public DateTime? FechaAlta { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public int? Empresa { get; set; }
        public string Via { get; set; }
        public string Factura { get; set; }
        public string Conocimiento { get; set; }
        public decimal? CantidadBulto { get; set; }
        public int? Transportista { get; set; }
        public DateTime? FechaProgramado { get; set; }
        public string UnidadMedida { get; set; }
        public string NumeroImportacion { get; set; }
        public string NumeroExportacion { get; set; }
        public short? Despachante { get; set; }
        public decimal? ValorSeguro { get; set; }
        public DateTime? FechaEnviado { get; set; }
        public decimal? Volumen { get; set; }
        public decimal? Peso { get; set; }
        public DateTime? FechaFinalizado { get; set; }
        public int? Camion { get; set; }
        public short? TipoAlmacenajeYSeguro { get; set; }
        public string Predio { get; set; }
        public short? CantidadContenedor20 { get; set; }
        public short? CantidadContenedor40 { get; set; }
        public DateTime? FechaFacturado { get; set; }
        public decimal? ValorFlete { get; set; }
        public string IdManual { get; set; }
        public int? Proveedor { get; set; }
        public bool AgendarAutomaticamente { get; set; }
        public decimal? ValorOtrosGastos { get; set; }
        public string DocumentoTransporte { get; set; }
        public string Anexo1 { get; set; }
        public string Anexo2 { get; set; }
        public string Anexo3 { get; set; }
        public string Anexo4 { get; set; }
        public string Anexo5 { get; set; }
        public string Anexo6 { get; set; }
        public bool Ficticio { get; set; }
        public string NumeroCorrelativo { get; set; }
        public string NumeroDTI { get; set; }
        public DateTime? FechaDTI { get; set; }
        public string NumeroDocumentoTransporte { get; set; }
        public DateTime? FechaDeclarado { get; set; }
        public DateTime? FechaMovilizacionContenedor { get; set; }
        public string NumeroCorrelativo2 { get; set; }
        public bool Validado { get; set; }
        public string Cliente { get; set; }
        public decimal? ICMS { get; set; }
        public decimal? II { get; set; }
        public decimal? IPI { get; set; }
        public decimal? IISUSPENSO { get; set; }
        public decimal? PISCONFINS { get; set; }
        public decimal? IPISUSPENSO { get; set; }
        public int? RegimenAduana { get; set; }
        public string NumeroAgrupador { get; set; }
        public string TipoAgrupador { get; set; }
        public string NumeroDocumento1 { get; set; }
        public string NumeroDocumento2 { get; set; }
        public long? NumeroTransaccionDelete { get; set; }
        public string Auditoria { get; set; }
        public List<DocumentoLinea> Lineas { get; set; }
        public DocumentoConfiguracion Configuracion { get; set; }

        public Documento()
        {
            this.Lineas = new List<DocumentoLinea>();
            this.Configuracion = new DocumentoConfiguracion();
        }

        public virtual string GetNumeroDocumento(IUnitOfWork uow)
        {
            return uow.DocumentoRepository.GetNumeroDocumento(Tipo);
        }

        public virtual DocumentoLinea GetLinea(string cdProducto, string nroIdentificador, decimal faixa)
        {
            return this.Lineas
                .Where(d => d.Producto == cdProducto
                    && d.Empresa == (this.Empresa ?? -1)
                    && d.Identificador == nroIdentificador
                    && d.Faixa == faixa)
                .FirstOrDefault();
        }

        public virtual string GetDescripcionEstado(IUnitOfWork uow)
        {
            return uow.DocumentoRepository.GetEstado(this.Estado).Descripcion;
        }

        public virtual bool CanEdit(IUnitOfWork uow)
        {
            var documentosEditables = uow.DocumentoTipoRepository.GetDocumentosHabilitadosParaEdicion();
            return documentosEditables.ContainsKey(this.Tipo) && documentosEditables[this.Tipo].Contains(this.Estado);
        }

        public virtual List<DocumentoAccion> GetEstadosHabilitadosParaCambio(IUnitOfWork uow)
        {
            var acciones = uow.DocumentoRepository.GetEstadosDestino(this.Estado, this.Tipo);

            acciones.RemoveAll(a => AccionDocumento.ExcluirEnCambioEstado(a.Codigo));

            return acciones;
        }

        public virtual void Editar()
        {
            this.FechaModificacion = DateTime.Now;
        }

        public virtual void ConfirmarEdicion()
        {
            this.FechaModificacion = DateTime.Now;
        }

        public virtual void Cancelar()
        {
            this.FechaModificacion = DateTime.Now;
        }

        public virtual void GenerarLineas()
        {
            this.FechaModificacion = DateTime.Now;
        }

        public virtual void EnviarDocumento()
        {
            this.FechaModificacion = DateTime.Now;
            this.FechaEnviado = DateTime.Now;
        }

        public virtual void AprobarDocumento(DocumentoAduana dti, DocumentoAduana dua, int? nroAgenda)
        {
            if (dti != null)
            {
                this.NumeroDTI = dti.Numero;
                this.FechaDTI = dti.Fecha;
            }

            if (dua != null)
                this.DocumentoAduana = dua;

            if (nroAgenda != null)
                this.Agenda = nroAgenda;

            this.FechaDeclarado = DateTime.Now;
            this.FechaModificacion = DateTime.Now;
        }

        public virtual void IniciarVerificacion()
        {
            this.FechaModificacion = DateTime.Now;
        }

        public virtual void Finalizar()
        {
            this.FechaFinalizado = DateTime.Now;
            this.FechaModificacion = DateTime.Now;
        }

        public virtual void Agrupar(string numeroAgrupador, string tipoAgrupador)
        {
            this.NumeroAgrupador = numeroAgrupador;
            this.TipoAgrupador = tipoAgrupador;
            this.FechaModificacion = DateTime.Now;
        }

        public virtual void Desagrupar()
        {
            this.NumeroAgrupador = null;
            this.TipoAgrupador = null;
            this.FechaModificacion = DateTime.Now;
        }
    }
}
