using System;

namespace WIS.Domain.Produccion
{
    public class Pasada
    {
        public readonly IIngreso _ingreso;

        public int Numero { get; set; }
        public int NumeroFormula { get; set; }
        public int Orden { get; set; }
        public FormulaAccion Accion { get; set; }
        public string Valor { get; set; }
        public DateTime? FechaAlta { get; set; }
        public string NuIngreso { get; set; }
        public string Linea { get; set; }

        public Pasada(IIngreso ingreso)
        {
            this._ingreso = ingreso;
        }

        public virtual int GetNumeroPasadasEnFormula()
        {
            return this.Numero - ((this.NumeroFormula - 1) * this._ingreso.Formula.CantidadPasadasPorFormula);
        }

        public virtual int GetNumeroFormulasCompletas()
        {
            return this.NumeroFormula - 1;
        }

        public virtual bool IsUltimaPasada()
        {
            return this.GetNumeroPasadasEnFormula() > this._ingreso.Formula.CantidadPasadasPorFormula;
        }
    }
}
