using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Expedicion;
using WIS.Domain.General;
using WIS.Domain.Picking;
using WIS.Domain.Produccion.Interfaces;
using WIS.Domain.Produccion.Mappers;
using WIS.Domain.Produccion.Models;
using WIS.Domain.StockEntities;
using WIS.Exceptions;
using WIS.Security;
using WIS.TrafficOfficer;

namespace WIS.Domain.Produccion.Logic
{
    public abstract class LogicaProduccion : ILogicaProduccion
    {
        protected readonly IngresoProduccionMapper _mapper;
        protected readonly IIdentityService _identity;
        protected readonly IngresoProduccion _ingresoProduccion;
        protected readonly IUnitOfWork _uow;

        public LogicaProduccion(IUnitOfWork uow, IIdentityService identity, IngresoProduccion ingresoProduccion)
        {
            _mapper = new IngresoProduccionMapper();
            _identity = identity;
            _ingresoProduccion = ingresoProduccion;
            _uow = uow;
        }

        public virtual IngresoProduccion CrearIngresoProduccion(string tipoIngreso, int empresa, string predio, List<IngresoProduccionDetalleTeorico> detalles, string idExterno = null, string idEspacioProduccion = null)
        {
            _ingresoProduccion.Empresa = empresa;
            _ingresoProduccion.IdFormula = string.Empty;
            _ingresoProduccion.Predio = predio;
            _ingresoProduccion.Detalles = detalles;
            _ingresoProduccion.Funcionario = _identity.UserId;
            _ingresoProduccion.FechaAlta = DateTime.Now;
            _ingresoProduccion.Situacion = SituacionDb.PRODUCCION_CREADA;
            _ingresoProduccion.NumeroProduccionOriginal = _ingresoProduccion.Id;
            _ingresoProduccion.Tipo = tipoIngreso;
            _ingresoProduccion.PermitirAutoasignarLinea = "N";
            _ingresoProduccion.IdModalidadLote = null;
            _ingresoProduccion.NuTransaccion = _uow.GetTransactionNumber();
            _ingresoProduccion.IdManual = "S";

            _ingresoProduccion.IdProduccionExterno = idExterno;
            _ingresoProduccion.IdEspacioProducion = idEspacioProduccion;

            return _ingresoProduccion;
        }

        public virtual int GetEmpresa()
        {
            return (int)_ingresoProduccion.Empresa;
        }

        public virtual bool TieneEspacioProduccion()
        {
            return (_ingresoProduccion.EspacioProduccion != null);
        }

        public virtual string GetPredio()
        {
            return _ingresoProduccion.Predio;
        }

        public virtual void AddIngresoProduccion()
        {
            _uow.IngresoProduccionRepository.AddIngreso(_ingresoProduccion);
        }

        public virtual void AsociarEspacioProduccion(string idEspacio)
        {
            try
            {
                var espacioProduccion = _uow.EspacioProduccionRepository.GetEspacioProduccion(idEspacio);

                if (espacioProduccion == null)
                    throw new ValidationFailedException("General_Sec0_Error_Error97", new string[] { idEspacio });

                _ingresoProduccion.EspacioProduccion = espacioProduccion;
                _ingresoProduccion.NuTransaccion = _uow.GetTransactionNumber();
                _ingresoProduccion.PosicionEnCola = _uow.IngresoProduccionRepository.GetNextPosicionEnCola(espacioProduccion.Id);

                _uow.IngresoProduccionRepository.UpdateIngresoProduccion(_ingresoProduccion);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public virtual void AddDetalleTeorico(IngresoProduccionDetalleTeorico detalle)
        {
            if (_ingresoProduccion.Detalles.Any(d => d.Producto == detalle.Producto && d.Identificador == detalle.Identificador && d.Tipo == detalle.Tipo))
                throw new ValidationFailedException("PRE110_Sec0_Error_Er003_DetalleDuplicados");

            _uow.IngresoProduccionRepository.AddDetalle(detalle);
            _ingresoProduccion.AddDetalleTeorico(detalle);
        }

        public virtual void UpdateDetalleTeorico(IngresoProduccionDetalleTeorico detalle)
        {
            _uow.IngresoProduccionRepository.UpdateDetalle(detalle);
        }

        public virtual void DeleteDetalleTeorico(IngresoProduccionDetalleTeorico detalle)
        {
            _uow.IngresoProduccionRepository.DeleteDetalle(detalle);
            _ingresoProduccion.RemoveDetalleTeorico(detalle);
        }

        public virtual IngresoProduccionDetalleTeorico GetDetalleTeorico(int idDetalle)
        {
            return _ingresoProduccion.GetDetalleTeorico(idDetalle);
        }

        public virtual void UpdateSituacion(short situacion)
        {
            _ingresoProduccion.Situacion = situacion;
            _ingresoProduccion.NuTransaccion = _uow.GetTransactionNumber();

            _uow.IngresoProduccionRepository.UpdateIngresoProduccion(_ingresoProduccion);
        }

        public virtual void FinalizarProduccion()
        {
            _ingresoProduccion.Situacion = SituacionDb.PRODUCCION_FINALIZADA;
            _ingresoProduccion.NuTransaccion = _uow.GetTransactionNumber();
            _ingresoProduccion.FechaFinProduccion = DateTime.Now;

            _uow.IngresoProduccionRepository.UpdateIngresoProduccion(_ingresoProduccion);
        }

        public virtual void IniciarProduccion()
        {
            _ingresoProduccion.Situacion = SituacionDb.PRODUCCION_INICIADA;
            _ingresoProduccion.NuTransaccion = _uow.GetTransactionNumber();
            _ingresoProduccion.FechaInicioProduccion = DateTime.Now;

            _uow.IngresoProduccionRepository.UpdateIngresoProduccion(_ingresoProduccion);
        }

        public virtual bool IsSituacion(short situacion)
        {
            return (_ingresoProduccion.Situacion == situacion);
        }

        public virtual bool ProduccionHabilitadaParaFabricar()
        {
            return (IsSituacion(SituacionDb.PRODUCCION_PARCIALMENTE_NOTIF)
                       || IsSituacion(SituacionDb.PRODUCCION_INICIADA)
                       || IsSituacion(SituacionDb.PRODUCIENDO));
        }

        public abstract bool PuedeIniciarProduccion(out string mensaje, out List<string> errorArg);

        public virtual bool ProduccionHabilitadaParaNotificar()
        {
            return (IsSituacion(SituacionDb.PRODUCCION_PARCIALMENTE_NOTIF)
                       || IsSituacion(SituacionDb.PRODUCCION_INICIADA)
                       || IsSituacion(SituacionDb.PRODUCIENDO));
        }

        public virtual bool ProduccionEnProcesoDeNotificacion()
        {
            return (IsSituacion(SituacionDb.PRODUCCION_PENDIENTE_NOTIFICACION_PARCIAL)
                       || IsSituacion(SituacionDb.PRODUCCION_PENDIENTE_NOTIFICACION_FINAL));
        }

        public virtual bool EsProductoEsperado(int empresa, string producto, string tipoRegistro)
        {
            return _ingresoProduccion.EsProductoEsperado(empresa, producto, tipoRegistro);
        }

        public virtual bool HayDiferenciasEnProduccion()
        {
            return _uow.IngresoProduccionRepository.HayDiferenciasEnProduccion(_ingresoProduccion.Id);
        }

        public virtual EspacioProduccion GetEspacioProduccion()
        {
            return _ingresoProduccion.EspacioProduccion;
        }

        public virtual IngresoProduccionDetalleReal GetInsumoProduccion(long idInsumo)
        {
            return _ingresoProduccion.GetDetalleInsumo(idInsumo);
        }

        public virtual void AddDetalleProductoNoEsperado(string ubicacion, string producto, decimal faixa, int empresa, string lote, decimal producido, string codMotivo, string dsMotivo, DateTime? vencimiento)
        {
            IngresoProduccionDetalleSalida productoFinal = _uow.IngresoProduccionRepository.GetDetalleSalidaReal(_ingresoProduccion.Id, producto, empresa, lote);

            if (productoFinal == null)
            {
                productoFinal = new IngresoProduccionDetalleSalida();
                productoFinal.NuTransaccion = _uow.GetTransactionNumber();
                productoFinal.DtAddrow = DateTime.Now;
                productoFinal.QtProducido = producido;
                productoFinal.NuPrdcIngreso = _ingresoProduccion.Id;
                productoFinal.QtNotificado = 0;
                productoFinal.Identificador = lote;
                productoFinal.NdMotivo = codMotivo;
                productoFinal.DsMotivo = dsMotivo;
                productoFinal.Faixa = faixa;
                productoFinal.Producto = producto;
                productoFinal.Empresa = empresa;
                productoFinal.DtVencimiento = vencimiento;
                productoFinal.NuOrden = _ingresoProduccion.GetMaximoNumeroOrdenProductosReales();
                productoFinal.NuOrden++;

                _uow.IngresoProduccionRepository.AddDetalleSalidaReal(productoFinal);
            }
            else
            {
                productoFinal.QtProducido = (productoFinal.QtProducido ?? 0) + producido;
                productoFinal.NuTransaccion = _uow.GetTransactionNumber();
                productoFinal.NdMotivo = codMotivo;
                productoFinal.DsMotivo = dsMotivo;

                _uow.IngresoProduccionRepository.UpdateDetalleSalidaProduccion(productoFinal);
            }

            SalidaProduccionDetalle salida = new SalidaProduccionDetalle()
            {
                NuTransaccion = _uow.GetTransactionNumber(),
                FechaAlta = DateTime.Now,
                Cantidad = producido,
                NuPrdcIngreso = _ingresoProduccion.Id,
                Identificador = lote,
                Faixa = 1,
                Producto = producto,
                Empresa = empresa,
                Ubicacion = ubicacion,
                Vencimiento = vencimiento,
                Motivo = codMotivo,
                FlPendienteNotificar = "S",
                DsMotivo = dsMotivo
            };

            _uow.IngresoProduccionRepository.AddDetalleSalidaProducido(salida);
        }

        public virtual List<IngresoProduccionDetalleReal> GetInsumosProduccion()
        {
            return _uow.IngresoProduccionRepository.GetInsumosReales(_ingresoProduccion.Id);
        }

        public virtual void AddInsumoProduccion(IngresoProduccionDetalleReal detalle)
        {
            detalle.NuPrdcIngreso = _ingresoProduccion.Id;

            _uow.IngresoProduccionRepository.AddDetalleRealProduccion(detalle);
            _ingresoProduccion.AddDetalleInsumo(detalle);
        }

        public abstract void GenerarProductoNoEsperado(Producto producto, decimal faixa, int empresa, string lote, decimal producido, DateTime? vencimiento, string codMotivo, string dsMotivo, out string keyAjuste);

        public abstract void ConsumirInsumoCompleto(long idInsumo, string ubicacion, decimal cantidadConsumir, out DateTime? vencimiento, bool isConsumible = false);

        public abstract void ConsumirInsumoParcial(long idInsumo, string ubicacion, decimal qtConsumir, out DateTime? vencimiento, bool isConsumible = false);

        public virtual Pedido GenerarPedido(List<IngresoProduccionDetallePedidoTemporal> detallesTeporalesPedidoInsumos)
        {
            _ingresoProduccion.GeneraPedido = true;

            if (_ingresoProduccion.Situacion == SituacionDb.PRODUCCION_CREADA)
            {
                _ingresoProduccion.Situacion = SituacionDb.PEDIDO_GENERADO;
            }

            _ingresoProduccion.NuTransaccion = _uow.GetTransactionNumber();

            _uow.IngresoProduccionRepository.UpdateIngresoProduccion(_ingresoProduccion);

            var empresa = _uow.EmpresaRepository.GetEmpresa(_ingresoProduccion.Empresa.Value);
            var pedido = new Pedido
            {
                Id = _uow.PedidoRepository.GetNextNuPedidoManual().ToString(),
                Cliente = empresa.CdClienteArmadoKit,
                Empresa = (int)_ingresoProduccion.Empresa,
                Predio = _ingresoProduccion.Predio,
                Tipo = TipoPedidoDb.Produccion,
                FechaEmision = DateTime.Now,
                FechaLiberarDesde = DateTime.Now,
                FechaAlta = DateTime.Now,
                Estado = SituacionDb.PedidoAbierto,
                IsManual = false,
                Agrupacion = Agrupacion.Pedido,
                CondicionLiberacion = CondicionLiberacionDb.SinCondicion,
                Origen = "PRD112",
                Memo1 = "Pedido generado para producción en PRD112",
                Memo = $"Pedido generado para producción {_ingresoProduccion.Id}.",
                ConfiguracionExpedicion = new ConfiguracionExpedicionPedido() { Tipo = Domain.DataModel.Mappers.Constants.TipoExpedicion.Produccion },
                ComparteContenedorPicking = $"{_ingresoProduccion.Id}.{empresa.CdClienteArmadoKit}.{empresa.Id}",
                IngresoProduccion = _ingresoProduccion.Id,
                Transaccion = _uow.GetTransactionNumber(),
            };

            Agente agente = _uow.AgenteRepository.GetAgenteConRelaciones(pedido.Empresa, pedido.Cliente);

            if (agente.RutasPorDefecto.Any(d => d.Predio == pedido.Predio))
            {
                AgenteRutaPredio rutaPredio = agente.RutasPorDefecto.Where(d => d.Predio == pedido.Predio).FirstOrDefault();

                Ruta rutaDefault = _uow.RutaRepository.GetRuta(rutaPredio.Ruta);

                if (rutaDefault != null) pedido.Ruta = rutaDefault.Id;
            }
            else
            {
                Ruta rutaDefault = _uow.RutaRepository.GetRuta(agente.Ruta.Id);

                if (rutaDefault != null) pedido.Ruta = rutaDefault.Id;
            }

            _uow.PedidoRepository.AddPedido(pedido);

            foreach (var detalleTemporal in detallesTeporalesPedidoInsumos)
            {
                var detalle = new DetallePedido
                {
                    Id = pedido.Id,
                    Empresa = detalleTemporal.Empresa,
                    Producto = detalleTemporal.Producto,
                    Cantidad = detalleTemporal.CantidadAPedir,
                    CantidadOriginal = detalleTemporal.CantidadAPedir,
                    Identificador = detalleTemporal.Lote,
                    CantidadAnulada = 0,
                    CantidadLiberada = 0,
                    EspecificaIdentificador = Producto.EspecificaIdentificador(detalleTemporal.Lote),
                    FechaAlta = DateTime.Now,
                    Faixa = detalleTemporal.Faixa,
                    Transaccion = _uow.GetTransactionNumber()
                };

                _uow.PedidoRepository.AddDetallePedido(pedido, detalle);

                pedido.Lineas.Add(detalle);
            }

            return pedido;
        }

        public abstract void DefinirLotesPedido(Pedido pedido, List<DetallePedido> detalleDefinido, IFormatProvider format);

        public abstract void UpdatePedido(Pedido pedido);

        public virtual void DesafectarInsumo(long idInsumo, string ubicacion, ITrafficOfficerService concurrencyControl, TrafficOfficerTransaction transactionTO)
        {
            IngresoProduccionDetalleReal insumo = _ingresoProduccion.GetDetalleInsumo(idInsumo);

            if (insumo == null)
                throw new Exception("PRD510_grid1_Error_InsumoSinRegistro");

            if (insumo.QtReal == 0)
                throw new Exception("PRD510_grid1_Error_InsumoSinSaldo");

            decimal saldoConsumir = (insumo.QtReal ?? 0);
            insumo.QtReal -= saldoConsumir;
            insumo.NuTransaccion = _uow.GetTransactionNumber();

            _uow.IngresoProduccionRepository.UpdateDetalleRealProduccion(insumo);

            var stock = _uow.StockRepository.GetStock(insumo.Empresa ?? 0, insumo.Producto, insumo.Faixa ?? 1, ubicacion, insumo.Identificador);
            if (stock == null)
                throw new Exception("PRD113_grid1_Error_InsumoSinSaldo");

            var idBloqueo = stock.GetLockId(_identity.GetFormatProvider());

            if (concurrencyControl.IsLocked("T_STOCK", idBloqueo, true))
                throw new EntityLockedException("General_Sec1_Error_StockBloqueado");

            concurrencyControl.AddLock("T_STOCK", idBloqueo, transactionTO, true);

            if (stock != null)
            {
                if (stock.ReservaSalida > saldoConsumir)
                    stock.ReservaSalida = (stock.ReservaSalida ?? 0) - saldoConsumir;
                else
                    stock.ReservaSalida = 0;
                stock.NumeroTransaccion = _uow.GetTransactionNumber();
                stock.FechaModificacion = DateTime.Now;

                _uow.StockRepository.UpdateStock(stock);
            }

            _uow.SaveChanges();

            concurrencyControl.RemoveLockByIdLock("T_STOCK", idBloqueo, _identity.UserId);
        }

        public virtual void DesafectarInsumosConSaldo(string ubicacionProduccion, ITrafficOfficerService concurrencyControl, TrafficOfficerTransaction transactionTO)
        {
            List<IngresoProduccionDetalleReal> insumosConSaldo = _ingresoProduccion.Consumidos.Where(w => w.QtReal > 0).ToList();

            if (insumosConSaldo != null && insumosConSaldo.Count > 0)
            {
                foreach (IngresoProduccionDetalleReal insumo in insumosConSaldo)
                {
                    DesafectarInsumo(insumo.NuPrdcIngresoReal, ubicacionProduccion, concurrencyControl, transactionTO);
                }
            }
            _uow.SaveChanges();

        }

        public virtual bool HayPendientesDeNotificacion()
        {
            return _ingresoProduccion.HayPendientesNotificar();
        }

        public virtual void AfectarSobrantes(string cdProducto, string nuIdentificador, int cdEmpresa, decimal cdFaixa, decimal qtAfectarTotal, string ubicacion)
        {
            decimal cantidadAfectarSaldo = qtAfectarTotal;
            List<Stock> stocksInsumo = this._uow.StockRepository.GetStockProduccion(cdEmpresa, cdProducto, cdFaixa, ubicacion, nuIdentificador);

            if (stocksInsumo.Count() == 0 || stocksInsumo == null)
                throw new Exception("PRD113_grid1_Error_InsumoSinSaldo");

            foreach (var stockInsumo in stocksInsumo)
            {
                decimal cantidadAfectar = 0;

                if ((stockInsumo.Cantidad - stockInsumo.ReservaSalida) >= cantidadAfectarSaldo)
                {
                    cantidadAfectar = cantidadAfectarSaldo;
                    stockInsumo.ReservaSalida = (stockInsumo.ReservaSalida ?? 0) + cantidadAfectar;

                    stockInsumo.NumeroTransaccion = _uow.GetTransactionNumber();
                    stockInsumo.FechaModificacion = DateTime.Now;
                    cantidadAfectarSaldo = 0;
                }
                else
                {
                    cantidadAfectar = ((stockInsumo.Cantidad ?? 0) - (stockInsumo.ReservaSalida ?? 0));
                    cantidadAfectarSaldo = cantidadAfectarSaldo - cantidadAfectar;
                    stockInsumo.ReservaSalida = (stockInsumo.ReservaSalida ?? 0) + cantidadAfectar;

                    stockInsumo.NumeroTransaccion = _uow.GetTransactionNumber();
                    stockInsumo.FechaModificacion = DateTime.Now;
                }

                _uow.StockRepository.UpdateStock(stockInsumo);

                var insumosRealesExistentes = GetInsumosProduccion()
                    .Where(f => f.Producto == cdProducto
                        && f.Identificador == stockInsumo.Identificador
                        && f.Empresa == cdEmpresa
                        && f.Faixa == cdFaixa
                        && !string.IsNullOrEmpty(f.Referencia)
                        && f.QtReal < f.QtRealOriginal)
                    .OrderBy(f => f.NuOrden)
                    .ToList();

                foreach (var insumoRealExistente in insumosRealesExistentes)
                {
                    var cantidadAfectarParcial = (insumoRealExistente.QtRealOriginal ?? 0) - (insumoRealExistente.QtReal ?? 0);

                    if (cantidadAfectar > cantidadAfectarParcial)
                    {
                        cantidadAfectar = cantidadAfectar - cantidadAfectarParcial;
                    }
                    else
                    {
                        cantidadAfectarParcial = cantidadAfectar;
                        cantidadAfectar = 0;
                    }

                    insumoRealExistente.QtReal += cantidadAfectarParcial;

                    _uow.IngresoProduccionRepository.UpdateDetalleRealProduccion(insumoRealExistente);
                }

                if (cantidadAfectar > 0)
                {
                    var insumoReal = GetInsumosProduccion()
                        .Where(f => f.Producto == cdProducto
                            && f.Identificador == stockInsumo.Identificador
                            && f.Empresa == cdEmpresa
                            && f.Faixa == cdFaixa
                            && string.IsNullOrEmpty(f.Referencia))
                        .FirstOrDefault();

                    if (insumoReal != null)
                    {
                        insumoReal.QtReal += cantidadAfectar;

                        _uow.IngresoProduccionRepository.UpdateDetalleRealProduccion(insumoReal);
                    }
                    else
                    {
                        insumoReal = new IngresoProduccionDetalleReal
                        {
                            Empresa = cdEmpresa,
                            Faixa = cdFaixa,
                            Producto = cdProducto,
                            Identificador = stockInsumo.Identificador,
                            QtReal = cantidadAfectar,
                            QtMerma = 0,
                            QtNotificado = 0,
                            NuPrdcIngreso = _ingresoProduccion.Id,
                            NuOrden = _uow.IngresoProduccionRepository.GetNextValueNuOrdenDetalleReal(_ingresoProduccion.Id),
                        };

                        AddInsumoProduccion(insumoReal);
                    }
                }

                if (cantidadAfectarSaldo == 0)
                    break;
            }

            if (cantidadAfectarSaldo > 0 || stocksInsumo == null)
                throw new Exception("PRD113_grid1_Error_InsumoSinSaldo");
        }

        public virtual void DesafectarSobrantes(string cdProducto, string nuIdentificador, int cdEmpresa, decimal cdFaixa, decimal cantidadDesafectarTotal, string ubicacion)
        {
            decimal cantidadDesafectarSaldo = cantidadDesafectarTotal;
            List<Stock> stocksInsumo = this._uow.StockRepository.GetStockProduccion(cdEmpresa, cdProducto, cdFaixa, ubicacion, nuIdentificador);

            if (stocksInsumo.Count() == 0 || stocksInsumo == null)
                throw new Exception("PRD113_grid1_Error_InsumoSinSaldo");

            foreach (var stockInsumo in stocksInsumo)
            {
                var insumosRealesExistentes = GetInsumosProduccion()
                    .Where(f => f.Producto == cdProducto
                        && f.Identificador == stockInsumo.Identificador
                        && f.Empresa == cdEmpresa
                        && f.Faixa == cdFaixa)
                    .OrderBy(i => string.IsNullOrEmpty(i.Referencia) ? 0 : 1)
                    .ThenByDescending(i => i.NuOrden)
                    .ToList();

                if (insumosRealesExistentes != null && insumosRealesExistentes.Count > 0)
                {
                    foreach (var insumoRealExistente in insumosRealesExistentes)
                    {
                        Stock stock = this._uow.StockRepository.GetStock(cdEmpresa, cdProducto, cdFaixa, ubicacion, insumoRealExistente.Identificador);

                        decimal cantidadDesafectar = 0;

                        if (cantidadDesafectarSaldo <= insumoRealExistente.QtReal)
                        {
                            cantidadDesafectar = cantidadDesafectarSaldo;
                            insumoRealExistente.QtReal -= cantidadDesafectar;
                            cantidadDesafectarSaldo = 0;
                        }
                        else
                        {
                            cantidadDesafectar = insumoRealExistente.QtReal ?? 0;
                            insumoRealExistente.QtReal = 0;
                            cantidadDesafectarSaldo = cantidadDesafectarSaldo - cantidadDesafectar;
                        }

                        if ((stock.ReservaSalida ?? 0) - cantidadDesafectar < 0)
                        {
                            throw new Exception("PRD113_grid1_Error_InconsistenciaDeStock");
                        }

                        stock.ReservaSalida = (stock.ReservaSalida ?? 0) - cantidadDesafectar;
                        stock.NumeroTransaccion = _uow.GetTransactionNumber();
                        stock.FechaModificacion = DateTime.Now;

                        _uow.StockRepository.UpdateStock(stock);

                        _ingresoProduccion.UpdateDetalleInsumo(insumoRealExistente);
                        _uow.IngresoProduccionRepository.UpdateDetalleRealProduccion(insumoRealExistente);

                        _uow.SaveChanges();
                    }
                }

                if (cantidadDesafectarSaldo == 0)
                    break;
            }

            if (cantidadDesafectarSaldo > 0 || stocksInsumo == null)
                throw new Exception("PRD113_grid1_Error_InconsistenciaDeStock");
        }

        public abstract IngresoProduccionDetalleReal ExisteIngresoReal(string codigoProducto, string identificador);

        public virtual IngresoProduccion GetIngresoProduccion()
        {
            return _ingresoProduccion;
        }

    }
}
