using System;
using System.Linq;
using WIS.Domain.DataModel.Mappers.Constants;

namespace WIS.Domain.Produccion
{
    public class IngresoBlackBox : IngresoProduccion, IIngresoPanel
    {
        public override void CerrarProduccion()
        {
            this.Situacion = SituacionDb.PRODUCCION_FINALIZADA;
            this.FechaActualizacion = DateTime.Now;
        }

        public override Pasada GetLatestPasada()
        {
            return this.Pasadas
                .OrderByDescending(d => new { d.NumeroFormula, d.Numero, d.Orden })
                .FirstOrDefault();
        }

        public override Pasada GetCurrentPasada()
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

        public override bool IsFinalizado()
        {
            Pasada pasada = this.GetCurrentPasada();
            return pasada.NumeroFormula > this.CantidadIteracionesFormula;
        }
    }
}
