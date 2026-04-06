// ReSharper disable CollectionNeverQueried.Global
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.General.Enums;
using WIS.Domain.Interfaces;
using WIS.Domain.Recepcion;
using WIS.Domain.StockEntities;

namespace WIS.Domain.General.API.Dtos.Entrada
{
    public class ControlCalidadResponse : EntradaResponse
    {
        public List<ControlCalidadItemResponse> ControlesDeCalidad { get; set; }

        public List<long?> InstanciasGeneradas { get; set; }

        public List<int> ControlesDeCalidadAprobados { get; set; }

        public bool ProcesadoOk { get; set; }

        public ControlCalidadResponse()
            : base()
        {
            ControlesDeCalidad = new List<ControlCalidadItemResponse>();
            ControlesDeCalidadAprobados = new List<int>();
            InstanciasGeneradas = new List<long?>();
        }

        private CriterioCalidadItemResponse AddCriterio(CriterioControlCalidadAPI criterio) =>
            new CriterioCalidadItemResponse
            {
                Predio = criterio.Predio,
                Producto = criterio.Producto,
                Identificador = criterio.Lote,
                IdExternoEtiqueta = criterio.EtiquetaExterna,
                TipoEtiqueta = criterio.TipoEtiquetaExterna,
                Ubicacion = criterio.Ubicacion,
                Empresa = criterio.Empresa,
            };

        public void AddControl(
            ControlCalidadAPI nuevoControl,
            long? instancia = null)
        {
            var nuevoControlResponse =
                new ControlCalidadItemResponse
                {
                    Control = nuevoControl.CodigoControlCalidad,
                    TextoInformativo = nuevoControl.Descripcion,
                    Instancia = instancia,
                    Estado = nuevoControl.Estado.ToString(),
                };

            foreach (var criterio in nuevoControl.Criterios)
                nuevoControlResponse.Criterios.Add(this.AddCriterio(criterio));

            this.ControlesDeCalidad.Add(nuevoControlResponse);
            if (instancia != null)
                this.InstanciasGeneradas.Add(instancia);
        }

        public void AddAprobados(List<ControlDeCalidadPendiente> pendientes)
        {
            ControlesDeCalidadAprobados.AddRange(pendientes.Select(x => x.Id));
        }
    }

    public class ControlCalidadItemResponse
    {
        public ControlCalidadItemResponse()
        {
            this.Criterios = new List<CriterioCalidadItemResponse>();
        }

        public int Control { get; set; }
        public string TextoInformativo { get; set; }
        public long? Instancia { get; set; }
        public string Estado { get; set; }
        public List<CriterioCalidadItemResponse> Criterios { get; set; }
    }

    public class CriterioCalidadItemResponse
    {
        public string Predio { get; set; }
        public int Empresa { get; set; }
        public string Producto { get; set; }
        public string Identificador { get; set; }
        public string IdExternoEtiqueta { get; set; }
        public string Ubicacion { get; set; }
        public string TipoEtiqueta { get; set; }
    }
}
