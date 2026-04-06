using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.Produccion.Models;
using WIS.Domain.Produccion.Enums;
using WIS.Domain.DataModel.Mappers.Constants;

namespace WIS.Domain.Produccion
{
    public class IngresoColector : IIngresoColector
    {
        public string Id { get; set; }
        public int? Empresa { get; set; }
        public Formula Formula { get; set; }
        public int CantidadIteracionesFormula { get; set; }
        public bool GeneraPedido { get; set; }
        public short? Situacion { get; set; }
        public TipoProduccionIngreso TipoProduccion { get; set; }
        public int? Funcionario { get; set; }
        public DateTime? FechaAlta { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public string NumeroProduccionOriginal { get; set; }
        public string Anexo1 { get; set; }
        public string Anexo2 { get; set; }
        public string Anexo3 { get; set; }
        public string Anexo4 { get; set; }
        public long? EjecucionEntrada { get; set; }
        public string Predio { get; set; }
        public DateTime? FechaInicioProduccion { get; set; }
        public DateTime? FechaFinProduccion { get; set; }
        public string IdManual { get; set; }
        public long? NroUltInterfazEjecucion { get; set; }

        public List<Pasada> Pasadas { get; set; }
        public List<IngresoProduccionDetalleReal> Consumidos { get; set; }
        public List<IngresoProduccionDetalleSalida> Producidos { get; set; }

        public virtual void CerrarProduccion()
        {
            this.Situacion = SituacionDb.PRODUCCION_FINALIZADA;
            this.FechaActualizacion = DateTime.Now;
        }

        public virtual Pasada GetLatestPasada()
        {
            return this.Pasadas.OrderByDescending(d => new { d.NumeroFormula, d.Numero, d.Orden }).FirstOrDefault();
        }

        public virtual int GetTotalPasadas()
        {
            return this.CantidadIteracionesFormula * this.Formula.CantidadPasadasPorFormula;
        }

        public virtual Pasada GetCurrentPasada()
        {
            Pasada pasada = this.GetLatestPasada();

            int numeroFormula = 1;
            int numeroPasada = 1;
            int orden = 0;

            if (pasada != null)
            {
                numeroFormula = pasada.NumeroFormula;
                numeroPasada = pasada.Numero;
                orden = pasada.Orden;
            }

            FormulaConfiguracion configuracion = null;

            if (this.Formula.AnyConfiguracion(numeroPasada))
            {
                configuracion = this.Formula.GetConfiguracionOrdenMayor(numeroPasada, orden);

                if (configuracion == null)
                {
                    numeroPasada++;

                    configuracion = this.Formula.GetConfiguracionOrdenMayor(numeroPasada);
                }
            }
            else
            {
                if (pasada != null)
                {
                    numeroPasada++;

                    configuracion = this.Formula.GetConfiguracionOrdenMayor(numeroPasada);
                }
            }

            Pasada nuevaPasada = new Pasada(this)
            {
                Numero = numeroPasada,
                NumeroFormula = numeroFormula,
                FechaAlta = DateTime.Now,
                Orden = 1
            };

            if (configuracion != null)
            {
                nuevaPasada.Accion = configuracion.Accion;
                nuevaPasada.Orden = configuracion.Orden;
            }

            if (nuevaPasada.GetNumeroPasadasEnFormula() > this.Formula.CantidadPasadasPorFormula)
                nuevaPasada.NumeroFormula++;

            return nuevaPasada;
        }
        public virtual bool IsFinalizado()
        {
            Pasada pasada = this.GetCurrentPasada();

            return pasada.NumeroFormula > this.CantidadIteracionesFormula;
        }
    }
}
