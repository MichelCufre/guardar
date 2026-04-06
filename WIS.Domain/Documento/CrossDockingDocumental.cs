using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Documento
{
    public class CrossDockingDocumental
    {
        public bool ManejaDocumental { get; set; }
        public bool ConsumirOtrosDocumentos { get; set; }
        public IDocumentoIngreso DocumentoOriginal { get; set; }
        public Dictionary<string, Dictionary<string, Dictionary<string, DocumentoLineaDesafectada>>> DocumentosDisponibles { get; set; }

        public List<DocumentoLineaDesafectada> LineasAModificar { get; set; }
        public List<DocumentoPreparacionReserva> NuevasReservas { get; set; }

        public CrossDockingDocumental()
        {
            DocumentosDisponibles = new Dictionary<string, Dictionary<string, Dictionary<string, DocumentoLineaDesafectada>>>();
            LineasAModificar = new List<DocumentoLineaDesafectada>();
            NuevasReservas = new List<DocumentoPreparacionReserva>();
        }
    }
}
