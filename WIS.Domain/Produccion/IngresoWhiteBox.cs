using System;
using System.Linq;
using WIS.Domain.DataModel.Mappers.Constants;

namespace WIS.Domain.Produccion
{
    public class IngresoWhiteBox : IngresoProduccion, IIngresoPanel
    {
        public override void CerrarProduccion()
        {
            this.Situacion = SituacionDb.PRODUCCION_FINALIZADA;
            this.FechaActualizacion = DateTime.Now;
        }

        //public virtual LineaConsumida GetOrAddLineaConsumida(Pasada pasada, string producto, int empresa, decimal faixa, string identificador)
        //{
        //    //var consumido = this.Consumidos
        //    //    .Where(d => d.Iteracion == pasada.NumeroFormula 
        //    //        && d.Pasada == pasada.Numero
        //    //        && d.Producto == producto 
        //    //        && d.Empresa == empresa 
        //    //        && d.Faixa == faixa 
        //    //        && d.Identificador == identificador)
        //    //    .FirstOrDefault();

        //    //if (consumido == null)
        //    //{
        //    //    consumido = new LineaConsumida
        //    //    {
        //    //        Iteracion = pasada.NumeroFormula,
        //    //        Pasada = pasada.Numero,
        //    //        Producto = producto,
        //    //        Faixa = faixa,
        //    //        Empresa = empresa,
        //    //        Identificador = identificador,
        //    //        FechaAlta = DateTime.Now,
        //    //        Cantidad = 0
        //    //    };

        //    //    this.Consumidos.Add(consumido);
        //    //}

        //    //return consumido;
        //}
        public override Pasada GetLatestPasada()
        {
            return this.Pasadas
                .OrderByDescending(d => d.NumeroFormula)
                .ThenByDescending(d => d.Numero)
                .ThenByDescending(d => d.Orden)
                .FirstOrDefault();
        }

        public override Pasada GetCurrentPasada()
        {
            Pasada pasada = this.GetLatestPasada();
            int numeroFormula = 1;
            int numeroPasada = 1;
            int orden = 0;

            if(pasada != null)
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

        public virtual void FinalizarIngreso()
        {
            this.Situacion = SituacionDb.PRODUCCION_FINALIZADA;
            this.FechaActualizacion = DateTime.Now;
        }

        public virtual bool ShouldEnsamblar()
        {
            Pasada pasada = this.GetLatestPasada();

            if (pasada == null)
                return false;

            FormulaConfiguracion configuracion = this.Formula.GetConfiguracionOrdenMayor(pasada.Numero, pasada.Orden);

            return configuracion == null;
        }
    }
}
