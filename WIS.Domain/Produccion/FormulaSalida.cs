using System;
using WIS.Domain.Produccion.Enums;

namespace WIS.Domain.Produccion
{
    public class FormulaSalida
    {
        public string IdFormula { get; set; }
        public int Empresa { get; set; }
        public string Producto { get; set; }
        public decimal Faixa { get; set; }
        public DateTime? FechaAlta { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public decimal CantidadCompleta { get; set; }
        public decimal CantidadProducir { get; set; }
        public decimal CantidadIncompleta { get; set; }
        public int CantidadPasadas { get; set; }
        public FormulaResultadoTipo TipoResultado { get; set; }
        public int NumeroPasada { get; set; }

        #region API

        public string IdProductoFinal { get; set; }

        #endregion

        public virtual void UpdateCantidadCompleta(decimal pasadasPorFormula)
        {
            if (this.NumeroPasada == 0)
                throw new InvalidOperationException("Numero de pasada no definido");

            this.CantidadCompleta = Math.Floor(pasadasPorFormula / this.NumeroPasada) * this.CantidadProducir;
        }

    }
}
