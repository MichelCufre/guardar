using System;
using WIS.Domain.Services.Interfaces;

namespace WIS.Domain.Impresiones
{
    public class ImpresionBuilder
    {
        protected readonly IImpresionDetalleBuildingStrategy _buildingStrategy;
        protected readonly IPrintingService _printingService;
        protected readonly Impresora _impresora;
        protected Impresion Impresion { get; set; }

        public ImpresionBuilder(Impresora impresora, 
            IImpresionDetalleBuildingStrategy strategy,
            IPrintingService printingService)
        {
            this.Impresion = new Impresion();
            this._buildingStrategy = strategy;
            this._impresora = impresora;
            this._printingService = printingService;
        }

        public virtual ImpresionBuilder GenerarCabezal(int usuario, string predio)
        {
            this.Impresion.Usuario = usuario;
            this.Impresion.Predio = predio;
            this.Impresion.CodigoImpresora = this._impresora.Id;
            this.Impresion.NombreImpresora = this._impresora.Descripcion;
            this.Impresion.Generado = DateTime.Now;
            this.Impresion.Estado = _printingService.GetEstadoInicial();

            return this;
        }

        public virtual ImpresionBuilder GenerarDetalle()
        {
            this.Impresion.Detalles = this._buildingStrategy.Generar(this._impresora);

            return this;
        }

        public virtual Impresion Build()
        {
            return this.Impresion;
        }
    }
}
