using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers.Constants;

namespace WIS.Domain.Produccion
{
    public class Formula
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Tipo { get; set; }
        public int Empresa { get; set; }
        public short? Estado { get; set; }
        public DateTime? FechaAlta { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public int CantidadPasadasPorFormula { get; set; }
        public List<FormulaEntrada> Entrada { get; set; }
        public List<FormulaSalida> Salida { get; set; }
        public List<FormulaConfiguracion> Configuracion { get; set; }

        public Formula()
        {
            this.Entrada = new List<FormulaEntrada>();
            this.Salida = new List<FormulaSalida>();
            this.Configuracion = new List<FormulaConfiguracion>();
        }

        public virtual void UpdateCantidadCompleta()
        {
            foreach (var entrada in this.Entrada)
            {
                entrada.UpdateCantidadCompleta(this.CantidadPasadasPorFormula);
            }

            foreach (var salida in this.Salida)
            {
                salida.UpdateCantidadCompleta(this.CantidadPasadasPorFormula);
            }
        }

        public virtual bool HasDuplicates()
        {
            if (this.Entrada.GroupBy(d => new { d.Componente, d.Prioridad }).DefaultIfEmpty().Any(d => d.Count() > 1))
                return true;

            if (this.Salida.GroupBy(d => d.Producto).DefaultIfEmpty().Any(d => d.Count() > 1))
                return true;

            if (this.Configuracion.GroupBy(d => d.Accion?.Id).DefaultIfEmpty().Any(d => d.Count() > 1))
                return true;

            return false;
        }

        public virtual void Enable()
        {
            this.Estado = SituacionDb.Activo;
        }

        public virtual void Disable()
        {
            this.Estado = SituacionDb.Inactivo;
        }

        public virtual bool AnyConfiguracion(int numeroPasada)
        {
            return this.Configuracion.Any(d => numeroPasada % d.Pasada == 0);
        }

        public virtual FormulaConfiguracion GetConfiguracionOrdenMayor(int numeroPasada, int orden = 0)
        {
            return this.Configuracion.Where(d => numeroPasada % d.Pasada == 0 && d.Orden > orden).OrderBy(d => d.Pasada).ThenBy(d => d.Orden).FirstOrDefault();
        }

        public virtual int GetCantOrdenPasada(int numeroPasada)
        {
            return this.Configuracion.Where(d => numeroPasada % d.Pasada == 0).Count();
        }

        public virtual List<FormulaEntrada> GetEntradasByPasada(int numeroPasada)
        {
            return this.Entrada.Where(d => numeroPasada % d.NumeroPasada == 0).ToList();
        }

        public virtual List<FormulaSalida> GetSalidasByPasada(int numeroPasada)
        {
            return this.Salida.Where(d => numeroPasada % d.NumeroPasada == 0).ToList();
        }
    }
}
