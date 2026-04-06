using System;
using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.ManejoStock.Constants;
using WIS.Domain.Recepcion.Enums;
using WIS.Domain.StockEntities;
using WIS.Exceptions;

namespace WIS.Domain.Recepcion
{
    public class AjusteEtiquetaLote
    {
        protected short _situacionEtiquetaLote;
        protected short _situacionDetalleAgenda;
        protected decimal _cantidadProductoRecibidoEtiquetaLoteDetalle;
        protected decimal _cantidadProductoEtiquetaLoteDetalle;
        protected List<short> _situacionesAgendaPermitidas;

        protected string _tipoEtiqueta;
        protected string _numeroExternoEtiqueta;

        protected string _codigoProducto;
        protected decimal _faixa;
        protected int _idEmpresa;
        protected string _identificador;
        protected int _numeroEtiquetaLote;
        protected int _numeroAgenda;
        protected string _aplicacion;
        protected int _usuario;
        protected string _idUbicacion;
        protected string _idUbicacionSugerida;

        protected decimal _cantidadProductoAnteriorEtiquetaLoteDetalle;

        protected readonly IUnitOfWork _uow;
        protected readonly AgendaMapper _mapperAgenda = new AgendaMapper();
        protected readonly EtiquetaLoteMapper _mapperEtiqueta = new EtiquetaLoteMapper();

        /// <summary>
        /// Constructor para comprobación de etiqueta ajustable
        /// </summary>
        /// <param name="situacionEtiquetaLote">Situación del cabezal de etiqueta</param>
        /// <param name="situacionDetalleAgenda">Situación de detalle de agenda</param>
        /// <param name="cantidadProductoRecibidoEtiquetaLoteDetalle">Cantidad de producto recibida en el detalle de la etiqueta lote</param>
        /// <param name="cantidadProductoEtiquetaLoteDetalle">Cantidad de producto en el detalle de la etiqueta lote</param>
        public AjusteEtiquetaLote(IUnitOfWork uow, short situacionEtiquetaLote, short situacionDetalleAgenda, decimal cantidadProductoRecibidoEtiquetaLoteDetalle, decimal cantidadProductoEtiquetaLoteDetalle, int numeroAgenda)
        {
            this._situacionEtiquetaLote = situacionEtiquetaLote;
            this._situacionDetalleAgenda = situacionDetalleAgenda;
            this._cantidadProductoRecibidoEtiquetaLoteDetalle = cantidadProductoRecibidoEtiquetaLoteDetalle;
            this._cantidadProductoEtiquetaLoteDetalle = cantidadProductoEtiquetaLoteDetalle;
            this._numeroAgenda = numeroAgenda;
            this._uow = uow;

            this._situacionesAgendaPermitidas = new List<short>
            {
                EstadoAgendaDb.ConferidaConDiferencias,
                EstadoAgendaDb.ConferidaSinDiferencias
            };
        }

        // Constructor para realizar el ajuste de etiqueta
        public AjusteEtiquetaLote(IUnitOfWork uow, int usuario, string aplicacion, int numeroAgenda, string idUbicacion, string idUbicacionSugerida, string codigoProducto, decimal faixa, int idEmpresa, string identificador, int numeroEtiquetaLote, string tipoEtiqueta, string numeroExternoEtiqueta, short situacionEtiquetaLote, short situacionDetalleAgenda, decimal cantidadProductoRecibidoEtiquetaLoteDetalle, decimal cantidadProductoEtiquetaLoteDetalle) : this(uow, situacionEtiquetaLote, situacionDetalleAgenda, cantidadProductoRecibidoEtiquetaLoteDetalle, cantidadProductoEtiquetaLoteDetalle, numeroAgenda)
        {
            this._tipoEtiqueta = tipoEtiqueta;
            this._numeroExternoEtiqueta = numeroExternoEtiqueta;
            this._codigoProducto = codigoProducto;
            this._faixa = faixa;
            this._idEmpresa = idEmpresa;
            this._identificador = identificador;
            this._numeroEtiquetaLote = numeroEtiquetaLote;
            this._aplicacion = aplicacion;
            this._usuario = usuario;
            this._idUbicacion = idUbicacion;
            this._idUbicacionSugerida = idUbicacionSugerida;
            this._situacionDetalleAgenda = situacionDetalleAgenda;
            this._cantidadProductoRecibidoEtiquetaLoteDetalle = cantidadProductoRecibidoEtiquetaLoteDetalle;
        }

        public virtual bool EsEtiquetaAjustable(bool ignorarProducto = false)
        {
            bool cond1 = this._situacionEtiquetaLote == SituacionDb.PalletConferido;
            bool cond2 = this._situacionesAgendaPermitidas.Contains(this._situacionDetalleAgenda);
            bool cond3 = true;

            if (!ignorarProducto)
                cond3 = this._cantidadProductoEtiquetaLoteDetalle == this._cantidadProductoRecibidoEtiquetaLoteDetalle;

            bool cond4 = this._cantidadProductoRecibidoEtiquetaLoteDetalle >= 0;

            bool cond5 = _uow.AgendaRepository.GetAgendaEstado(_numeroAgenda) != EstadoAgenda.Cerrada;

            return cond1 && cond2 && cond3 && cond4 && cond5;
        }

        /// <summary>
        /// - Se actualiza cantidades de la etiqueta, se crea log de etiqueta
        /// - Se actualiza el stock y se crea transito de entrada si corresponde
        /// - Se eliminan controles de calidad sobre los productos si corresponde
        /// - Se actualizan problemas de recepción
        /// - Se actualizan estados (situaciones) de agenda y detalles
        /// 
        /// </summary>
        public virtual void AjustarEtiqueta(bool isFlujoLpn = false)
        {
            if (!isFlujoLpn && !EsEtiquetaAjustable(true))
                throw new ValidationFailedException("REC150_Sec0_Error_Er001_EtiquetaNoajustable", new string[] { this._tipoEtiqueta, this._numeroExternoEtiqueta });

            // Creo número agreupador de transacción
            _uow.CreateTransactionNumber("Ajuste etiqueta");

            // Recupero detalle de etiqueta
            var etiquetaLoteDetalle = _uow.EtiquetaLoteRepository.GetEtiquetaLoteDetalle(this._codigoProducto, this._faixa, this._idEmpresa, this._identificador, this._numeroEtiquetaLote);

            etiquetaLoteDetalle.NumeroTransaccion = _uow.GetTransactionNumber();

            // Guardo cantidad anterior para posterior uso en actualización de transito de entrada
            this._cantidadProductoAnteriorEtiquetaLoteDetalle = etiquetaLoteDetalle.Cantidad ?? 0;

            if (this._cantidadProductoRecibidoEtiquetaLoteDetalle != (etiquetaLoteDetalle.CantidadRecibida ?? 0))
            {
                // Diferencia entre lo recibido original y el ajuste
                decimal cantidadAjuste = this._cantidadProductoRecibidoEtiquetaLoteDetalle - (etiquetaLoteDetalle.CantidadRecibida ?? 0);

                //Todo actualizar detalle etiqueta
                this.ActualizarCantidadesDetalleEtiqueta(etiquetaLoteDetalle, cantidadAjuste);

                // Actualizo stock y transito de entrada
                this.ActualizarStock(cantidadAjuste);

                // Actualizo detalle de agenda
                var agenda = _uow.AgendaRepository.GetAgendaSinDetalles(_numeroAgenda);
                if (agenda.Estado != EstadoAgenda.Cerrada && agenda.Estado != EstadoAgenda.Cancelada)
                {
                    new ModificarAgendaDetalle(_uow, _usuario, _aplicacion, _numeroAgenda, _idEmpresa, _codigoProducto, _faixa, _identificador).ModificarDetalle(cantidadAjuste);
                }
            }

            // Actualizar cantidad de producto
            if (etiquetaLoteDetalle.Cantidad == 0)
            {
                // Al ajustar en cero se borrran todos los controles de calidad
                // para ese Producto y tambien el detalle de la etiqueta
                if (_uow.ControlDeCalidadRepository.AnyControlDeCalidadProducto(_idEmpresa, _codigoProducto))
                {
                    var controlesCalidad = _uow.ControlDeCalidadRepository.GetControlDeCalidadPendientes(_codigoProducto, _faixa, _idEmpresa, _numeroEtiquetaLote, _identificador);

                    foreach (var control in controlesCalidad)
                    {
                        _uow.ControlDeCalidadRepository.RemoveControlPendiente(control);
                    }
                }
            }

            //Actualizar situación de agenda
            new ModificarAgenda(_uow, _usuario, _aplicacion, _numeroAgenda).ActualizarEstadoAgendaSegunSituacionDetalles();
        }



        /// <summary>
        /// Se actualizan las cantidades del detalle de etiqueta, se crea registro en T_LOG_ETIQUETA
        /// </summary>
        /// <param name="etiquetaLoteDetalle">Detalle de etiqueta a actualizar</param>
        /// <param name="cantidadAjuste">Diferencia en el ajuste (Valor nuevo - valor anterior)</param>
        public virtual void ActualizarCantidadesDetalleEtiqueta(EtiquetaLoteDetalle etiquetaLoteDetalle, decimal cantidadAjuste)
        {
            var nuTransaccion = _uow.GetTransactionNumber();
            etiquetaLoteDetalle.Cantidad = etiquetaLoteDetalle.Cantidad + cantidadAjuste;
            etiquetaLoteDetalle.CantidadRecibida = etiquetaLoteDetalle.CantidadRecibida + cantidadAjuste;
            etiquetaLoteDetalle.CantidadAjusteRecibido = cantidadAjuste;
            etiquetaLoteDetalle.NumeroTransaccion = nuTransaccion;
            etiquetaLoteDetalle.Modificacion = DateTime.Now;

            this._uow.EtiquetaLoteRepository.UpdateEtiquetaLoteDetalle(etiquetaLoteDetalle);

            var logEtiqueta = new LogEtiqueta()
            {
                Agenda = this._numeroAgenda,
                NumeroEtiqueta = this._numeroEtiquetaLote,
                CodigoProducto = this._codigoProducto,
                Faixa = this._faixa,
                Empresa = this._idEmpresa,
                Identificador = this._identificador,
                Cantidad = cantidadAjuste,
                Ubicacion = this._idUbicacion,
                FechaOperacion = DateTime.Now,
                NroTransaccion = nuTransaccion,
                Vencimiento = etiquetaLoteDetalle.Vencimiento,
                TipoMovimiento = TiposMovimiento.AjusteEtiqueta,
                Aplicacion = this._aplicacion,
                Funcionario = this._usuario,
            };

            this._uow.EtiquetaLoteRepository.AddLogEtiqueta(logEtiqueta);
        }

        /// <summary>
        /// Actualiza el stock o crea una entrada de stock, actualiza transito de entrada de la ubicación sugerida
        /// </summary>
        /// <param name="cantidadAjuste"></param>
        public virtual void ActualizarStock(decimal cantidadAjuste)
        {
            // Actualizo stock

            var stock = this._uow.StockRepository.GetStock(this._idEmpresa, this._codigoProducto, this._faixa, this._idUbicacion, this._identificador);

            if (stock == null)
            {
                stock = new Stock()
                {
                    Ubicacion = this._idUbicacion,
                    Empresa = this._idEmpresa,
                    Producto = this._codigoProducto,
                    Identificador = this._identificador,
                    Faixa = this._faixa,
                    Cantidad = cantidadAjuste,
                    ReservaSalida = cantidadAjuste,
                    CantidadTransitoEntrada = 0,
                    FechaModificacion = DateTime.Now,
                    Averia = "N",
                    NumeroTransaccion = this._uow.GetTransactionNumber()
                };

                this._uow.StockRepository.AddStock(stock);
            }
            else
            {
                stock.Cantidad = stock.Cantidad + cantidadAjuste;
                stock.ReservaSalida = stock.ReservaSalida + cantidadAjuste;
                stock.NumeroTransaccion = this._uow.GetTransactionNumber();
                stock.FechaModificacion = DateTime.Now;

                this._uow.StockRepository.UpdateStock(stock);
            }

            // Actualizo transito de entrada si existe ubicación sugerida

            if (!string.IsNullOrEmpty(this._idUbicacionSugerida))
            {
                decimal modifTransitoEntrada = 0;

                //Calculo como se tiene que modificar el transito de Entrada                
                if (this._cantidadProductoAnteriorEtiquetaLoteDetalle > 0)
                {
                    modifTransitoEntrada = cantidadAjuste;

                    if (cantidadAjuste < (this._cantidadProductoAnteriorEtiquetaLoteDetalle * -1))
                    {
                        //Se hizo un ajuste mayor a lo generado, solo resto transito hasta lo generado
                        modifTransitoEntrada = (this._cantidadProductoAnteriorEtiquetaLoteDetalle * -1);
                    }
                }
                else
                {
                    if (cantidadAjuste > (this._cantidadProductoAnteriorEtiquetaLoteDetalle * -1))
                    {
                        //Se hizo un ajuste mayor a lo generado, solo resto transito hasta lo generado
                        modifTransitoEntrada = cantidadAjuste - (this._cantidadProductoAnteriorEtiquetaLoteDetalle * -1);
                    }
                }

                if (modifTransitoEntrada > 0)
                {

                    if (this._idUbicacion != this._idUbicacionSugerida)
                    {
                        var stockUbicacionSugerida = _uow.StockRepository.GetStock(this._idEmpresa, this._codigoProducto, this._faixa, this._idUbicacionSugerida, this._identificador);

                        if (stockUbicacionSugerida == null)
                        {

                            stockUbicacionSugerida = new Stock()
                            {
                                Ubicacion = this._idUbicacion,
                                Empresa = this._idEmpresa,
                                Producto = this._codigoProducto,
                                Identificador = this._identificador,
                                Faixa = this._faixa,
                                Cantidad = 0,
                                ReservaSalida = 0,
                                CantidadTransitoEntrada = (cantidadAjuste >= 0 ? (this._cantidadProductoAnteriorEtiquetaLoteDetalle + cantidadAjuste) : 0),
                                FechaModificacion = DateTime.Now,
                                Averia = "N",
                                NumeroTransaccion = this._uow.GetTransactionNumber()
                            };

                            this._uow.StockRepository.AddStock(stockUbicacionSugerida);
                        }
                        else
                        {
                            stockUbicacionSugerida.CantidadTransitoEntrada = ((stockUbicacionSugerida.CantidadTransitoEntrada + modifTransitoEntrada) > 0 ? (stockUbicacionSugerida.CantidadTransitoEntrada + modifTransitoEntrada) : 0);
                            stockUbicacionSugerida.NumeroTransaccion = this._uow.GetTransactionNumber();
                            stockUbicacionSugerida.FechaModificacion = DateTime.Now;

                            this._uow.StockRepository.UpdateStock(stockUbicacionSugerida);
                        }
                    }
                    else
                    {
                        // La ubicación sugerida es la misma que la que se esta modificando
                        stock.CantidadTransitoEntrada = ((stock.CantidadTransitoEntrada + modifTransitoEntrada) > 0 ? (stock.CantidadTransitoEntrada + modifTransitoEntrada) : 0);
                        stock.NumeroTransaccion = this._uow.GetTransactionNumber();
                        stock.FechaModificacion = DateTime.Now;

                        this._uow.StockRepository.UpdateStock(stock);
                    }
                }
            }
        }
    }
}
