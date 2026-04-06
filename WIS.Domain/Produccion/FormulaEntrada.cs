using System;

namespace WIS.Domain.Produccion
{
    public class FormulaEntrada
    {
        public string IdFormula { get; set; }
        public string Componente { get; set; }
        public int Empresa { get; set; }
        public int? EmpresaPedido { get; set; }
        public decimal Faixa { get; set; }
        public string Producto { get; set; }
        public short Prioridad { get; set; }
        public DateTime? FechaAlta { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public decimal CantidadCompleta { get; set; }
        public decimal CantidadConsumir { get; set; }
        public decimal CantidadIncompleta { get; set; }
        public int CantidadPasadas { get; set; }
        public int NumeroPasada { get; set; }

        public virtual void UpdateCantidadCompleta(decimal pasadasPorFormula)
        {
            if (this.NumeroPasada == 0)
                throw new InvalidOperationException("Numero de pasada no definido");

            this.CantidadCompleta = Math.Floor(pasadasPorFormula / this.NumeroPasada) * this.CantidadConsumir;
        }

        public virtual decimal GetCantidadParaPedido(int totalPasadas)
        {
            decimal multiplicador = Math.Floor((decimal)totalPasadas / this.NumeroPasada);

            decimal cantidadPedido = multiplicador * this.CantidadConsumir;

            return cantidadPedido;
        }
    }
}
