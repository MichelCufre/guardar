using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Liberacion;
using WIS.Domain.Services.Interfaces;
using WIS.Exceptions;

namespace WIS.Domain.Services
{
    public class LiberacionService : ILiberacionService
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly ILogger<LiberacionService> _logger;

        public LiberacionService(IUnitOfWorkFactory uowFactory, ILogger<LiberacionService> logger)
        {
            this._uowFactory = uowFactory;
            this._logger = logger;
        }

        public virtual void Start(ReglaLiberacion reglaUnica = null, int userId = 0)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                this._logger.LogDebug($"Iniciando proceso");

                var reglas = new List<ReglaLiberacion>();

                if (reglaUnica != null)
                    reglas.Add(reglaUnica);
                else
                    reglas = uow.LiberacionRepository.GetReglasPendienteLiberar();


                foreach (var regla in reglas)
                {
                    this._logger.LogDebug($"Procesando regla {regla.NuRegla}");

                    uow.CreateTransactionNumber("Liberación Automática");
                    uow.BeginTransaction();

                    try
                    {
                        Validar(uow, regla);

                        var pedidos = GetPedidosParaLiberar(uow, regla);
                        var actualizarUltimaEjecucion = (reglaUnica == null || !reglaUnica.RespetarIntervalo);

                        Liberar(uow, pedidos, regla, userId, actualizarUltimaEjecucion);
                    }
                    catch (Exception ex)
                    {
                        uow.Rollback();
                        this._logger.LogError(ex, $"Error al procesar la regla {regla.NuRegla}");
                    }
                    finally
                    {
                        uow.EndTransaction();
                    }
                }

                this._logger.LogDebug($"Proceso finalizado");
            }
        }

        public virtual void Validar(IUnitOfWork uow, ReglaLiberacion regla)
        {
            var onda = regla.CdOnda;
            var empresa = regla.CdEmpresa;
            var predio = regla.NuPredio;
            var gruposExpedicion = uow.LiberacionRepository.VariosGruposValidacion(onda, empresa);

            if (string.IsNullOrEmpty(predio))
                predio = null;

            if (gruposExpedicion.Count() > 1)
            {
                throw new ValidationFailedException($"Existe más de un grupo de expedición para la onda {onda} y la empresa {empresa}. Sólo se puede liberar un grupo por preparación");
            }
            else if (gruposExpedicion.Count() == 0)
            {
                throw new ValidationFailedException($"No hay pedidos pendientes para la onda {onda} y la empresa {empresa}.");
            }

            if (!uow.LiberacionRepository.AnyPedidosParaPrep(empresa, onda, predio))
                throw new ValidationFailedException("No hay pedidos pendientes de liberar para los valores ingresados");

            string[] pedidosInvalidos = { "1", "2", "3" };

            if (pedidosInvalidos.Contains(regla.CdOrdenPedidos))
                throw new ValidationFailedException($"Opción deshabilitada para la regla {regla.NuRegla}");

        }

        public virtual List<PedidoPendLib> GetPedidosParaLiberar(IUnitOfWork uow, ReglaLiberacion regla)
        {
            uow.LiberacionRepository.SetCondicionesLiberacion(regla.CdOnda, regla.CdEmpresa, regla.LstReglaCondicionLiberacion.ToList());

            var pedidos = uow.LiberacionRepository.GetPedidosLiberar(regla);

            if (regla.CdOrdenPedidosAuto == "CLI")
            {
                if (regla.LstReglaCliente.Any())
                    pedidos = pedidos.OrderBy(s => regla.LstReglaCliente.FirstOrDefault(a => a.Cliente == s.cdCliente && a.Empesa == s.cdEmpresa).NuOrden ?? short.MaxValue).ThenBy(n => n.cdCliente).ToList();
                else
                    pedidos.OrderBy(n => n.cdCliente).ToList();
            }

            if (!pedidos.Any())
                throw new ValidationFailedException($"No hay pedidos para liberar bajo la regla {regla.NuRegla}");

            return pedidos;
        }

        public virtual void Liberar(IUnitOfWork uow, List<PedidoPendLib> pedidos, ReglaLiberacion regla, int userId, bool actualizarUltimaEjecucion)
        {
            var clientesPreparados = new List<(int cdEmpresa, string cdCliente)>();
            var nuPrep = -1;
            var valido = true;
            var msg = string.Empty;

            while (pedidos.Any())
            {
                var pendLib = pedidos.First();
                bool nuevaPrep = !clientesPreparados.Any(s => s.cdEmpresa == pendLib.cdEmpresa && s.cdCliente == pendLib.cdCliente);

                nuevaPrep = nuevaPrep && (clientesPreparados.Count() >= (regla.NuClisPorPreparacion ?? 1) || clientesPreparados.Count() == 0);

                if (nuevaPrep)
                {
                    clientesPreparados.Clear();

                    if (nuPrep != -1)
                    {
                        uow.LiberacionRepository.SeleccionPedidoCompatible(regla.CdOnda, regla.CdEmpresa, out valido, out msg);

                        if (valido)
                            uow.SaveChanges();
                    }

                    var preparacion = GetNewPreparacion(regla, userId, uow.GetTransactionNumber());
                    nuPrep = uow.PreparacionRepository.AddPreparacion(preparacion);
                }

                if (!clientesPreparados.Any(s => s.cdEmpresa == pendLib.cdEmpresa && s.cdCliente == pendLib.cdCliente))
                    clientesPreparados.Add((pendLib.cdEmpresa, pendLib.cdCliente));

                int empresa = pendLib.cdEmpresa;
                string cliente = pendLib.cdCliente;
                string nuPedido = pendLib.nuPedido;

                if (string.IsNullOrEmpty(nuPedido))
                    nuPedido = null;

                if (pendLib.nuOrdenLiberacion != pendLib.auxNuOrdenLiberacion)
                {
                    if (pendLib.cdCliente != null)
                    {
                        var pedido = uow.PedidoRepository.GetPedido(empresa, cliente, nuPedido);

                        if (pedido.PreparacionProgramada == null)
                        {
                            pedido.NumeroOrdenLiberacion = pendLib.auxNuOrdenLiberacion;
                            pedido.PreparacionProgramada = nuPrep;
                            pedido.Transaccion = uow.GetTransactionNumber();
                            uow.PedidoRepository.UpdatePedido(pedido);
                        }
                    }
                }

                pedidos.Remove(pendLib);
            }

            if (actualizarUltimaEjecucion)
            {
                if (regla.TpFrecuencia == "D" && regla.HrInicio != null)
                    regla.DtUltimaEjecucion = DateTime.Today.Add((TimeSpan)regla.HrInicio);
                else
                    regla.DtUltimaEjecucion = DateTime.Now;

                uow.LiberacionRepository.UpdateReglaLiberacion(regla);
                uow.SaveChanges();
            }

            if (valido)
            {
                uow.SaveChanges();
                uow.Commit();
            }
            else
                uow.Rollback();
        }

        public virtual WIS.Domain.Picking.Preparacion GetNewPreparacion(ReglaLiberacion regla, int userId, long nuTransaccion)
        {
            return new WIS.Domain.Picking.Preparacion()
            {
                FechaInicio = DateTime.Now,                                             //DT_INICIO
                Descripcion = "Liberación de Onda Automática Regla: " + regla.NuRegla,  //DS_PREPARACION 
                Usuario = userId,                                                       //CD_FUNCIONARIO
                Empresa = regla.CdEmpresa,                                              //CD_EMPRESA
                Onda = regla.CdOnda,                                                    //CD_ONDA
                Predio = regla.NuPredio,                                                //NU_PREDIO 
                Tipo = TipoPreparacionDb.Normal,                                        //TP_PREPARACION
                Situacion = SituacionDb.PreparacionPendiente,
                PrepararSoloConCamion = regla.CdPrepararSoloCamion == "S",              //FL_PREPARAR_SOLO_CON_CAMION
                PickingEsAgrupadoPorCamion = regla.CdAgruparPorCamion == "S",           //FL_PICK_AGRUPADO_POR_CAMION
                RespetarFifoEnLoteAUTO = regla.CdRespetarFifo == "S",                   //FL_RESPETAR_FIFO_EN_LOTE_AUTO
                DebeLiberarPorUnidades = regla.CdLiberarPorUnidad == "S",               //FL_LIBERAR_POR_UNIDADES 
                DebeLiberarPorCurvas = regla.CdLiberarPorCurvas == "S",                 //FL_LIBERAR_POR_CURVAS
                ControlaStockDocumental = regla.CdControlaStock == "S",                 //FL_CONTROLA_STOCK_DOCUMENTO
                ModalPalletCompleto = regla.CdPalletCompeto,                            //FL_MODAL_PALLET_COMPLETO
                ModalPalletIncompleto = regla.CdpalletIncompleto,                       //FL_MODAL_PALLET_INCOMPLETO
                RepartirEscasez = regla.CdRepartirEscasez,                              //VL_REPARTIR_ESCASEZ
                Agrupacion = regla.CdAgrupacion,                                        //ID_AGRUPACION
                CursorStock = regla.CdStock,                                            //VL_CURSOR_STOCK
                CursorPedido = regla.CdOrdenPedidos,                                    //VL_CURSOR_PEDIDO
                PriozarDesborde = regla.PriozarDesborde,                                //FL_PRIORIZAR_DESBORDE
                ManejaVidaUtil = regla.ManejaVidaUtil,                                  //FL_VENTANA_POR_CLIENTE
                ValorVidaUtil = regla.ValorVidaUtil ?? 0,                               //VL_PORCENTAJE_VENTANA
                Transaccion = nuTransaccion,                                            //NU_TRANSACCION
                ExcluirUbicacionesPicking = regla.ExcluirUbicacionesPicking,            //FL_EXCLUIR_UBICACIONES_PICKING
                UsarSoloStkPicking = regla.UsarSoloStkPicking,                          //FL_USAR_SOLO_STK_PICKING
                PermitePickVencido = false,                                       //FL_PICK_MANUAL_AVERIADO
                ValidarProductoProveedor = false,                                       //FL_VALIDAR_PRODUCTO_PROVEEDOR
            };
        }
    }
}
