using System;
using WIS.Domain.DataModel;

namespace WIS.Domain.Produccion
{
    public class CierreProduccionWhiteBox
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IngresoWhiteBox _ingreso;
        protected readonly int _usuario;
        protected readonly long _transaccion;

        public CierreProduccionWhiteBox(IUnitOfWork uow, IngresoWhiteBox ingreso, int usuario, long transaccion)
        {
            this._uow = uow;
            this._ingreso = ingreso;
            this._usuario = usuario;
            this._transaccion = transaccion;
        }

        public virtual void CerrarProduccion()
        {
            this._ingreso.FinalizarIngreso();
            this._uow.ProduccionRepository.UpdateIngresoWhiteBox(this._ingreso);

            this._ingreso.Linea.NumeroIngreso = null;
            this._ingreso.Linea.FechaModificacion = DateTime.Now;

            this._uow.LineaRepository.UpdateLinea(this._ingreso.Linea);
        }

        public virtual void GenerarHistorico()
        {
            this.GenerarHistoricoPasada();
            this.RemoverPasadas();

            this.GenerarHistoricoConsumo();
            this.RemoverConsumo();

            this.GenerarHistoricoProducido();
            this.RemoverProducido();
        }

        public virtual void GenerarHistoricoPasada()
        {
            foreach (var pasada in this._ingreso.Pasadas)
            {
                if (pasada.Accion != null)
                {
                    PasadaHistorica historica = new PasadaHistorica()
                    {
                        AccionIntancia = pasada.Accion.Id,
                        CantidadPasadas = pasada.Numero,
                        FechaCreacion = pasada.FechaAlta,
                        FechaHistorica = DateTime.Now,
                        Ingreso = this._ingreso.Id,
                        Linea = this._ingreso.Linea != null ? this._ingreso.Linea.Id : null,
                        NumeroFormulaEnsamblada = pasada.NumeroFormula,
                        Orden = pasada.Orden,
                        ValorAccionInstancia = pasada.Valor,
                        NumeroHistorico = this._uow.ProduccionRepository.ObtenerNumeroPasadaHistorica()
                    };

                    this._uow.ProduccionRepository.AddPasadaHistorico(historica, this._transaccion);
                }
            }
        }

        public virtual void RemoverPasadas()
        {
            foreach (var pasada in this._ingreso.Pasadas)
            {
                this._uow.ProduccionRepository.RemovePasada(this._ingreso, pasada);
            }
        }

        public virtual void GenerarHistoricoConsumo()
        {
            foreach (var consumo in this._ingreso.Consumidos)
            {
                LineaConsumidaHistorica historica = new LineaConsumidaHistorica()
                {
                    //Cantidad = consumo.Cantidad,
                    //Faixa = consumo.Faixa,
                    //FechaAlta = DateTime.Now,
                    //FechaConsumo = consumo.FechaAlta,
                    //Identificador = consumo.Identificador,
                    //Iteracion = consumo.Iteracion,
                    //Pasada = consumo.Pasada,
                    //Producto = consumo.Producto,
                    //NumeroHistorico = this._uow.ProduccionRepository.ObtenerNumeroLineaConsumoHistorica()
                };

                this._uow.ProduccionRepository.AddConsumoHistorico(this._ingreso, historica, this._transaccion);
            }
        }

        public virtual void RemoverConsumo()
        {
            foreach (var consumo in this._ingreso.Consumidos)
            {
                //this._uow.ProduccionRepository.RemoveConsumo(this._ingreso, consumo);
            }
        }

        public virtual void GenerarHistoricoProducido()
        {
            foreach (var producido in this._ingreso.Producidos)
            {
                LineaProducidaHistorica historica = new LineaProducidaHistorica()
                {
                    //Cantidad = producido.Cantidad,
                    //Faixa = producido.Faixa,
                    //FechaAlta = DateTime.Now,
                    //FechaProducido = producido.FechaAlta,
                    //FechaVencimiento = producido.Vencimiento,
                    //Identificador = producido.Identificador,
                    //Iteracion = producido.Iteracion,
                    //Pasada = producido.Pasada,
                    //Producto = producido.Producto,
                    //NumeroHistorico = this._uow.ProduccionRepository.ObtenerNumeroLineaProducidoHistorica()
                };

                this._uow.ProduccionRepository.AddProducidoHistorico(this._ingreso, historica, this._transaccion);
            }
        }

        public virtual void RemoverProducido()
        {
            foreach (var producido in this._ingreso.Producidos)
            {
                //this._uow.ProduccionRepository.RemoveProducido(this._ingreso, producido);
            }
        }
    }
}
