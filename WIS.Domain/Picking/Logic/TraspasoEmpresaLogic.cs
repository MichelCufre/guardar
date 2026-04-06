using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Documento.TiposDocumento;
using WIS.Domain.General;
using WIS.Domain.Picking.Logic.Bulks;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.StockEntities;
using WIS.Domain.StockEntities.Constants;
using WIS.Exceptions;
using WIS.Security;
using WIS.TrafficOfficer;

namespace WIS.Domain.Picking.Logic
{
    public class TraspasoEmpresaLogic
    {
        protected IIdentityService _identity { get; set; }
        protected ITrafficOfficerService _concurrencyControl;
        protected readonly IBarcodeService _barcodeService;

        public TraspasoEmpresaLogic(IIdentityService identity, ITrafficOfficerService concurrencyControl, IBarcodeService barcodeService)
        {
            _identity = identity;
            _concurrencyControl = concurrencyControl;
            _barcodeService = barcodeService;
        }
        
        public virtual TraspasoBulkOperationContext ProcesarTraspasoEntreEmpresa(IUnitOfWork uow, long nuTransaccion, long nroTraspaso, int nroPrepacion, string tipoTraspaso, int empresaOrigen, int empresaDestino, TrafficOfficerTransaction transactionTO, out List<Agente> agentes, out List<Pedido> pedidosNew)
        {
            var preparacion = uow.PreparacionRepository.GetPreparacionPorNumero(nroPrepacion);
            bool isEmpresaDocumentalEmpresaOrigen = uow.EmpresaRepository.IsEmpresaDocumental(empresaOrigen);
            bool isEmpresaDocumentalDestino = uow.EmpresaRepository.IsEmpresaDocumental(empresaDestino);
            var traspaso = uow.TraspasoEmpresasRepository.GetTraspaso(nroTraspaso);
            bool isTraspasoEmpresaDejarPreparado = (tipoTraspaso == TipoTraspasoDb.TraspasoPreparacionOrigen);
            bool isTraspasoEmpresaStockConReserva = (tipoTraspaso == TipoTraspasoDb.TraspasoPreparacionOrigen || tipoTraspaso == TipoTraspasoDb.TraspasoPreparacionPendiente);
            bool isTraspasoEmpresaPreparacionPendiente = (tipoTraspaso == TipoTraspasoDb.TraspasoPreparacionPendiente);

            var bulkContext = GetBulkContextTraspasoStock(uow, traspaso, nroPrepacion, preparacion.Predio, empresaOrigen, empresaDestino, isEmpresaDocumentalEmpresaOrigen, isEmpresaDocumentalDestino, isTraspasoEmpresaDejarPreparado, isTraspasoEmpresaPreparacionPendiente, traspaso.PropagarLPN, nuTransaccion);

            uow.TraspasoEmpresasRepository.PuedeTraspasarEmpresa(uow, bulkContext, traspaso, isEmpresaDocumentalEmpresaOrigen, isTraspasoEmpresaDejarPreparado, isTraspasoEmpresaPreparacionPendiente, traspaso.PropagarLPN, empresaDestino, nuTransaccion, out List<TraspasoEmpresasDetalleTemp> detalleStockTraspaso, out pedidosNew);

            AgregarBloqueos(detalleStockTraspaso, bulkContext.DetallePreparacionLpn, transactionTO);

            GetBulkContextAjusteStockTraspaso(uow, bulkContext, detalleStockTraspaso, traspaso.PropagarLPN, nroTraspaso, preparacion.Predio, nuTransaccion);

            uow.TraspasoEmpresasRepository.ProcesarProductosFaltantes(empresaDestino, nuTransaccion, traspaso.ReplicaProductos, traspaso.ReplicaCodBarras);

            uow.TraspasoEmpresasRepository.ProcesarAgentesFaltantes(empresaDestino, nuTransaccion, traspaso.ReplicaAgentes, out agentes);

            uow.TraspasoEmpresasRepository.ProcesarAltaStockTraspaso(uow, nuTransaccion, bulkContext, isTraspasoEmpresaStockConReserva, traspaso.PropagarLPN, isTraspasoEmpresaPreparacionPendiente);

            uow.TraspasoEmpresasRepository.ProcesarBajaStockTraspaso(uow, nuTransaccion, bulkContext, isTraspasoEmpresaDejarPreparado);

            ProcesarTraspasoDocumental(uow, bulkContext.DocumentoSalida, bulkContext.DocumentoEntrada, nuTransaccion);

            if (!isTraspasoEmpresaDejarPreparado)
            {
                if (isTraspasoEmpresaPreparacionPendiente)
                {
                    uow.TraspasoEmpresasRepository.ProcesarPedidoEnDestinoLiberado(nuTransaccion, bulkContext.PreparacionDestino.Id, _identity.Application, traspaso.PropagarLPN);

                    uow.TraspasoEmpresasRepository.GenerarPreparacionEnDestino(uow, bulkContext.PreparacionDestino, bulkContext.CargaDestino, bulkContext.ContenedoresDestino, bulkContext.DocumentoEntrada, isEmpresaDocumentalDestino, nuTransaccion, traspaso.PropagarLPN);
                }

                uow.TraspasoEmpresasRepository.ProcesarDetallePreparacionOrigen(nroPrepacion, nuTransaccion);
            }
            else
            {
                var contenedores = bulkContext.DetallePreparacion
                    .GroupBy(x => new { x.NumeroPreparacion, x.NroContenedor })
                    .Select(d => new DetallePreparacion
                    {
                        NumeroPreparacion = d.Key.NumeroPreparacion,
                        NroContenedor = d.Key.NroContenedor
                    }).ToList();


                var uts = bulkContext.DetallePreparacion.Where(x =>x.Contenedor.NumeroUnidadTransporte != null).Select(x => new UnidadTransporte() {NumeroUnidadTransporte = x.Contenedor.NumeroUnidadTransporte.Value }).Distinct().ToList();

                uow.TraspasoEmpresasRepository.ProcesarTraspasoContenedor(contenedores, nuTransaccion);

                uow.TraspasoEmpresasRepository.ProcesarTraspasoDeleteUnidadTransporte(uts, nuTransaccion);

                uow.TraspasoEmpresasRepository.ProcesarPedidoEnDestinoLiberado(nuTransaccion, bulkContext.PreparacionDestino.Id, _identity.Application);

                uow.TraspasoEmpresasRepository.GenerarPreparacionEnDestinoPreparada(uow, bulkContext.PreparacionDestino, bulkContext.CargaDestino, bulkContext.ContenedoresDestino, bulkContext.DocumentoEntrada, isEmpresaDocumentalDestino, nuTransaccion);

            }

            uow.TraspasoEmpresasRepository.BorrarTablasTemporalesTraspasoEmpresa();

            return bulkContext;

        }

        public virtual void AgregarBloqueos(List<TraspasoEmpresasDetalleTemp> stocks, List<DetallePreparacionLpn> detallePreparacionLpn, TrafficOfficerTransaction transactionTO)
        {
            var detallesStock = stocks
                .GroupBy(g => new { g.Ubicacion, g.Empresa, g.Producto, g.Faixa, g.Identificador })
                .Select(s => new { key = $"{s.Key.Ubicacion}#{s.Key.Empresa}#{s.Key.Producto}#{s.Key.Faixa}#{s.Key.Identificador}" }).Select(x => x.key);

            if (detallesStock.Count() > 0)
            {
                var listLock = this._concurrencyControl.GetLockList("T_STOCK", detallesStock.ToList(), transactionTO);

                if (listLock.Count > 0)
                {
                    var keyBloqueo = listLock.FirstOrDefault().Id_Bloqueo.Split("#");
                    throw new EntityLockedException("PRD113_msg_Error_StockBloqueada", new string[] { keyBloqueo[2], keyBloqueo[4] });
                }

                this._concurrencyControl.AddLockList("T_STOCK", detallesStock.ToList(), transactionTO, true);
            }

            var lpns = detallePreparacionLpn?.GroupBy(g => new { g.NroLpn })
                .Select(s => new { key = $"{s.Key.NroLpn}" }).Select(x => x.key);
            if (lpns.Count() > 0)
            {
                var listLpns = this._concurrencyControl.GetLockList("T_STOCK", lpns.ToList(), transactionTO);

                if (listLpns.Count > 0)
                {
                    var keyBloqueo = listLpns.FirstOrDefault().Id_Bloqueo.Split("#");
                    throw new EntityLockedException("PRD113_msg_Error_LpnBloqueada", new string[] { keyBloqueo[0] });
                }

                this._concurrencyControl.AddLockList("T_LPN", lpns.ToList(), transactionTO, true);
            }
        }

        public virtual void GetBulkContextAjusteStockTraspaso(IUnitOfWork uow, TraspasoBulkOperationContext bulkContext, List<TraspasoEmpresasDetalleTemp> detalleStockTraspaso, bool propagarLpn, long nroTraspaso, string predio, long nuTransaccion)
        {
            var apliacaion = _identity.Application.Length > 30 ? _identity.Application.Substring(0, 30) : _identity.Application;
            var detallesPickingSuelto = detalleStockTraspaso
                .Where(x => x.NroLpn == null && x.IdDetallePickingLpn == null)
                .GroupBy(x => new { x.Ubicacion, x.Empresa, x.Producto, x.Faixa, x.Identificador, x.Pedido })
                .Select(x => new
                {
                    x.Key.Ubicacion,
                    x.Key.Empresa,
                    x.Key.Producto,
                    x.Key.Faixa,
                    x.Key.Identificador,
                    x.Key.Pedido,
                    Cantidad = x.Sum(d => d.Cantidad),
                    Vencimiento = x.Min(d => d.Vencimiento)
                }).ToList();

            var idsAjustes = uow.AjusteRepository.GetNewIdsAjusteStock(detallesPickingSuelto.Count());

            foreach (var det in detallesPickingSuelto)
            {
                var id = idsAjustes.FirstOrDefault();
                AjusteStock ajuste = new AjusteStock
                {
                    NuAjusteStock = id,
                    Aplicacion = apliacaion,
                    Ubicacion = det.Ubicacion,
                    Empresa = det.Empresa,
                    Producto = det.Producto,
                    Faixa = det.Faixa,
                    Identificador = det.Identificador,
                    FechaVencimiento = det.Vencimiento,
                    QtMovimiento = det.Cantidad * -1,
                    TipoAjuste = TipoAjusteDb.TraspasoEntreEmpresa,
                    CdMotivoAjuste = MotivoAjusteDb.TraspasoEntreEmpresa,
                    FechaMotivo = DateTime.Now,
                    Funcionario = _identity.UserId,
                    IdProcesar = "N",
                    IdProcesado = "N",
                    FechaRealizado = DateTime.Now,
                    NuDocumento = nroTraspaso.ToString(),
                    Atributos = null,
                    Predio = predio,
                    NuTransaccion = nuTransaccion,
                    DescMotivo = det.Pedido
                };

                idsAjustes.Remove(id);

                bulkContext.AjustesStock.Add(ajuste);
            }

            idsAjustes = uow.AjusteRepository.GetNewIdsAjusteStock(bulkContext.DetallePreparacionLpn.Count());
            foreach (var det in bulkContext.DetallePreparacionLpn)
            {
                var id = idsAjustes.FirstOrDefault();

                AjusteStock ajuste = new AjusteStock
                {
                    NuAjusteStock = id,
                    Aplicacion = apliacaion,
                    Ubicacion = det.Ubicacion,
                    Empresa = det.Empresa ?? -1,
                    Producto = det.Producto,
                    Faixa = det.Faixa ?? 1,
                    Identificador = det.Lote,
                    FechaVencimiento = det.Vencimiento,
                    QtMovimiento = det.Cantidad * -1,
                    TipoAjuste = TipoAjusteDb.TraspasoEntreEmpresa,
                    CdMotivoAjuste = MotivoAjusteDb.TraspasoEntreEmpresa,
                    FechaMotivo = DateTime.Now,
                    Funcionario = _identity.UserId,
                    IdProcesar = "N",
                    IdProcesado = "N",
                    FechaRealizado = DateTime.Now,
                    NuDocumento = nroTraspaso.ToString(),
                    Atributos = det.Atributos,
                    Predio = predio,
                    NuTransaccion = nuTransaccion,
                    DescMotivo = det.Pedido
                };

                idsAjustes.Remove(id);

                bulkContext.AjustesStock.Add(ajuste);
            }

            var detallesPickingSueltoDestino = detalleStockTraspaso
                .Where(x => propagarLpn ? (x.NroLpn == null && x.IdDetallePickingLpn == null) : true)
                .GroupBy(x => new { x.Ubicacion, x.EmpresaDest, x.ProductoDest, x.Faixa, x.IdentificadorDest, x.Pedido })
                .Select(x => new
                {
                    x.Key.Ubicacion,
                    Empresa = x.Key.EmpresaDest,
                    Producto = x.Key.ProductoDest,
                    x.Key.Faixa,
                    x.Key.Pedido,
                    Identificador = x.Key.IdentificadorDest,
                    Cantidad = x.Sum(d => d.CantidadDestino),
                    Vencimiento = x.Min(d => d.VencimientoDest)
                });

            idsAjustes = uow.AjusteRepository.GetNewIdsAjusteStock(detallesPickingSueltoDestino.Count());

            foreach (var det in detallesPickingSueltoDestino)
            {
                var id = idsAjustes.FirstOrDefault();

                AjusteStock ajuste = new AjusteStock
                {
                    NuAjusteStock = id,
                    Aplicacion = apliacaion,
                    Ubicacion = det.Ubicacion,
                    Empresa = det.Empresa,
                    Producto = det.Producto,
                    Faixa = det.Faixa,
                    Identificador = det.Identificador,
                    FechaVencimiento = det.Vencimiento,
                    QtMovimiento = det.Cantidad,
                    TipoAjuste = TipoAjusteDb.TraspasoEntreEmpresa,
                    CdMotivoAjuste = MotivoAjusteDb.TraspasoEntreEmpresa,
                    FechaMotivo = DateTime.Now,
                    Funcionario = _identity.UserId,
                    IdProcesar = "N",
                    IdProcesado = "N",
                    FechaRealizado = DateTime.Now,
                    NuDocumento = nroTraspaso.ToString(),
                    Atributos = null,
                    Predio = predio,
                    NuTransaccion = nuTransaccion,
                    DescMotivo = det.Pedido
                };

                idsAjustes.Remove(id);

                bulkContext.AjustesStock.Add(ajuste);
            }

            if (propagarLpn)
            {
                idsAjustes = uow.AjusteRepository.GetNewIdsAjusteStock(bulkContext.DetallePreparacionLpn.Count());

                foreach (var det in bulkContext.DetallePreparacionLpn)
                {
                    var atributos = det.Atributos.Replace($"\"NroLPN\":{det.NroLpn}", $"\"NroLPN\":{det.NumeroLPNDestino}");
                    var id = idsAjustes.FirstOrDefault();

                    AjusteStock ajuste = new AjusteStock
                    {
                        NuAjusteStock = id,
                        Aplicacion = apliacaion,
                        Ubicacion = det.Ubicacion,
                        Empresa = det.EmpresaDestino,
                        Producto = det.CodigoProductoDestino,
                        Faixa = det.Faixa ?? 1,
                        Identificador = det.IdentificadorDestino,
                        FechaVencimiento = det.VencimientoDestino,
                        QtMovimiento = det.Cantidad,
                        TipoAjuste = TipoAjusteDb.TraspasoEntreEmpresa,
                        CdMotivoAjuste = MotivoAjusteDb.TraspasoEntreEmpresa,
                        FechaMotivo = DateTime.Now,
                        Funcionario = _identity.UserId,
                        IdProcesar = "N",
                        IdProcesado = "N",
                        FechaRealizado = DateTime.Now,
                        NuDocumento = nroTraspaso.ToString(),
                        Atributos = atributos,
                        Predio = predio,
                        NuTransaccion = nuTransaccion,
                        DescMotivo = det.Pedido
                    };

                    idsAjustes.Remove(id);

                    bulkContext.AjustesStock.Add(ajuste);
                }
            }
        }

        public virtual TraspasoBulkOperationContext GetBulkContextTraspasoStock(IUnitOfWork uow, TraspasoEmpresas traspaso, int nroPreparacion, string predio, int empresaOrigen, int empresaDestino, bool isEmpresaDocumentalOrigen, bool isEmpresaDocumentalEmpresaDestino, bool isTraspasoEmpresaDejarPreparado, bool isTraspasoEmpresaPreparacionPendiente, bool propagarLpn, long nuTransaccion)
        {
            TraspasoBulkOperationContext context = new TraspasoBulkOperationContext();

            if (!isTraspasoEmpresaDejarPreparado)
            {
                ProcesarDetallePreparacionDestino(uow, traspaso, nroPreparacion, context, isTraspasoEmpresaPreparacionPendiente);
            }
            else
                ProcesarDetallePreparacionDestinoDejarPreparado(uow, traspaso, nroPreparacion, context);

            var confirguracionEmpresa = uow.TraspasoEmpresasRepository.GetConfiguracionTraspasoByEmpresa(empresaOrigen);

            ControlDocumental(empresaOrigen, isEmpresaDocumentalOrigen, isEmpresaDocumentalEmpresaDestino, confirguracionEmpresa);

            ProcesarDocumentoTraspasoEmpresa(uow, nroPreparacion, predio, empresaOrigen, empresaDestino, isEmpresaDocumentalOrigen, isEmpresaDocumentalEmpresaDestino, nuTransaccion, context, confirguracionEmpresa, traspaso.TipoTraspaso);

            return context;
        }

        public virtual void ProcesarDetallePreparacionDestino(IUnitOfWork uow, TraspasoEmpresas traspaso, int nroPreparacion, TraspasoBulkOperationContext context, bool isTraspasoEmpresaPreparacionPendiente)
        {
            context.DetallePreparacion = uow.PreparacionRepository.GetDetallesPreparacion(x => x.NU_PREPARACION == nroPreparacion && x.ND_ESTADO == EstadoDetallePreparacion.ESTADO_PREP_PENDIENTE);
            context.DetallePreparacionLpn = uow.PreparacionRepository.GetDetallesPreparacionPendienteLpn(nroPreparacion);
            if (isTraspasoEmpresaPreparacionPendiente)
            {
                ProcesarPreparacionDestino(uow, traspaso, nroPreparacion, context);

                if (traspaso.PropagarLPN)
                {
                    var idsLpnsPicking = context.DetallePreparacionLpn.Select(x => x.IdDetallePickingLpn).Distinct();
                    var newidsLpnsPicking = uow.PreparacionRepository.GetNewIdDetallePickingLpn(idsLpnsPicking.Count());

                    foreach (var idLpnPicking in idsLpnsPicking)
                    {
                        var newIdLpn = newidsLpnsPicking.FirstOrDefault();

                        foreach (var detalle in context.DetallePreparacionLpn.Where(x => x.IdDetallePickingLpn == idLpnPicking))
                        {
                            detalle.IdDetallePickingLpnDest = newIdLpn;
                        }
                        newidsLpnsPicking.Remove(newIdLpn);
                    }
                }
            }

            foreach (var detalleLpn in context.DetallePreparacionLpn)
            {
                detalleLpn.Atributos = uow.TraspasoEmpresasRepository.GetSerializadoAtributos(uow, detalleLpn);
            }
        }

        public virtual void ProcesarDocumentoTraspasoEmpresa(IUnitOfWork uow, int nroPreparacion, string predio, int empresaOrigen, int empresaDestino, bool isEmpresaDocumentalOrigen, bool isEmpresaDocumentalEmpresaDestino, long nuTransaccion, TraspasoBulkOperationContext context, TraspasoEmpresasConfiguracion confirguracionEmpresa, string tipoTraspaso)
        {
            if (isEmpresaDocumentalOrigen)
            {
                var tipoDocumentoSalida = confirguracionEmpresa.TipoDocumentoEgreso;
                var estadoInicial = uow.DocumentoTipoRepository.GetEstadoInicial(tipoDocumentoSalida);
                var nroDocumento = uow.DocumentoRepository.GetNumeroDocumento(tipoDocumentoSalida);
                DocumentoEgreso documentoEgreso = new DocumentoEgreso();
                documentoEgreso.Numero = nroDocumento;
                documentoEgreso.Tipo = tipoDocumentoSalida;
                documentoEgreso.Empresa = empresaOrigen;
                documentoEgreso.Usuario = _identity.UserId;
                documentoEgreso.ValorArbitraje = 1;
                documentoEgreso.Situacion = SituacionDb.Activo;
                documentoEgreso.FechaAlta = DateTime.Now;
                documentoEgreso.Estado = estadoInicial;
                documentoEgreso.AgendarAutomaticamente = false;
                documentoEgreso.Validado = false;
                documentoEgreso.Auditoria = nuTransaccion.ToString();
                documentoEgreso.Predio = predio;
                context.DocumentoSalida = documentoEgreso;

                var detallesReservaDocumento = uow.DocumentoRepository.GetPreparacionReservasDocumental(nroPreparacion);
                decimal qtDesafectada;
                int secuencia = 1;
                foreach (var det in context.DetallePreparacion
                    .GroupBy(x => new { x.Producto, x.Lote, x.Faixa, x.Empresa })
                    .Select(x => new DetallePreparacion()
                    {
                        Producto = x.Key.Producto,
                        Lote = x.Key.Lote,
                        Faixa = x.Key.Faixa,
                        Empresa = x.Key.Empresa,
                        Cantidad = x.Sum(d => d.Cantidad)
                    }))
                {
                    decimal saldoADesafectar = det.Cantidad;

                    foreach (var detalleReservaDocumento in detallesReservaDocumento
                        .Where(x => x.Producto == det.Producto &&
                            x.Identificador == det.Lote &&
                            x.Faixa == det.Faixa &&
                            x.Empresa == det.Empresa))
                    {

                        if (saldoADesafectar == 0)
                            break;

                        if (tipoTraspaso == TipoTraspasoDb.TraspasoPreparacionOrigen)
                        {
                            if (saldoADesafectar >= detalleReservaDocumento.CantidadPreparada)
                            {
                                qtDesafectada = detalleReservaDocumento.CantidadPreparada ?? 0;
                                saldoADesafectar -= detalleReservaDocumento.CantidadPreparada ?? 0;
                            }
                            else
                            {
                                qtDesafectada = saldoADesafectar;
                                saldoADesafectar = 0;
                            }
                        }
                        else
                        {
                            if (saldoADesafectar >= detalleReservaDocumento.CantidadProducto)
                            {
                                qtDesafectada = detalleReservaDocumento.CantidadProducto ?? 0;
                                saldoADesafectar -= detalleReservaDocumento.CantidadProducto ?? 0;
                            }
                            else
                            {
                                qtDesafectada = saldoADesafectar;
                                saldoADesafectar = 0;
                            }
                        }

                        detalleReservaDocumento.CantidadProducto = qtDesafectada;
                        detalleReservaDocumento.Secuencia = secuencia;
                        detalleReservaDocumento.Auditoria = nuTransaccion.ToString();
                        context.DetallePreparacionReservaDocumental.Add(detalleReservaDocumento);
                        secuencia = secuencia + 1;
                    }

                    if (saldoADesafectar > 0)
                    {
                        throw new ValidationFailedException("PRE052_Sec0_Error_09", new string[] { det.Producto, det.Empresa.ToString(), det.Lote });
                    }

                }
            }

            if (isEmpresaDocumentalEmpresaDestino)
            {
                var tipoDocumentoIngreso = confirguracionEmpresa.TipoDocumentoIngreso;
                var estadoInicial = uow.DocumentoTipoRepository.GetEstadoInicial(tipoDocumentoIngreso);
                var nroDocumento = uow.DocumentoRepository.GetNumeroDocumento(tipoDocumentoIngreso);
                DocumentoIngreso documentoIngreso = new DocumentoIngreso();
                documentoIngreso.Numero = nroDocumento;
                documentoIngreso.Tipo = tipoDocumentoIngreso;
                documentoIngreso.Usuario = _identity.UserId;
                documentoIngreso.ValorArbitraje = 1;
                documentoIngreso.Situacion = SituacionDb.Activo;
                documentoIngreso.FechaAlta = DateTime.Now;
                documentoIngreso.Empresa = empresaDestino;
                documentoIngreso.Estado = estadoInicial;
                documentoIngreso.AgendarAutomaticamente = false;
                documentoIngreso.Validado = false;
                documentoIngreso.Auditoria = nuTransaccion.ToString();
                documentoIngreso.Predio = predio;
                context.DocumentoEntrada = documentoIngreso;

            }
        }

        public virtual void ControlDocumental(int empresaOrigen, bool isEmpresaDocumentalOrigen, bool isEmpresaDocumentalEmpresaDestino, TraspasoEmpresasConfiguracion confirguracionEmpresa)
        {
            if ((isEmpresaDocumentalOrigen || isEmpresaDocumentalEmpresaDestino) && (string.IsNullOrEmpty(confirguracionEmpresa.TipoDocumentoEgreso) || string.IsNullOrEmpty(confirguracionEmpresa.TipoDocumentoIngreso)))
            {
                throw new ValidationFailedException("PRE052_Sec0_Error_11", new string[] { empresaOrigen.ToString() });
            }
        }

        public virtual void ProcesarDetallePreparacionDestinoDejarPreparado(IUnitOfWork uow, TraspasoEmpresas traspaso, int nroPreparacion, TraspasoBulkOperationContext context)
        {
            context.DetallePreparacion = uow.PreparacionRepository.GetDetallesPreparacion(x => x.NU_PREPARACION == nroPreparacion && x.ND_ESTADO == EstadoDetallePreparacion.ESTADO_PREPARADO);
            context.DetallePreparacionLpn = uow.PreparacionRepository.GetDetallesLpnContenedor(nroPreparacion);

            foreach (var detalleLpn in context.DetallePreparacionLpn)
            {
                detalleLpn.Atributos = uow.TraspasoEmpresasRepository.GetSerializadoAtributos(uow, detalleLpn);
            }

            var nrosContenedores = context.DetallePreparacion.Select(x => x.NroContenedor).Distinct();
            var idContenedores = uow.ContenedorRepository.GetNewContenedores(nrosContenedores.Count());

            ProcesarPreparacionDestino(uow, traspaso, nroPreparacion, context);

            List<Contenedor> contenedoresDestino = new List<Contenedor>();

            foreach (var nroContenedor in nrosContenedores)
            {
                var idContenedor = idContenedores.FirstOrDefault();

                foreach (var detalle in context.DetallePreparacion.Where(x => x.NroContenedor == nroContenedor))
                {

                    detalle.NroContenedorDestino = idContenedor;

                    if (!contenedoresDestino.Any(x => x.Numero == idContenedor))
                    {
                        if (detalle.Contenedor.NroLpn == null || (detalle.Contenedor.NroLpn != null && traspaso.PropagarLPN))
                        {
                            var contenedorDestino = new Contenedor
                            {
                                NumeroPreparacion = context.PreparacionDestino.Id,
                                Numero = idContenedor,
                                TipoContenedor = detalle.Contenedor.TipoContenedor,
                                EstadoId = SituacionDb.ContenedorEnPreparacion,
                                Ubicacion = detalle.Contenedor.Ubicacion,
                                CodigoSubClase = detalle.Contenedor.CodigoSubClase,
                                FechaAgregado = DateTime.Now,
                                IdContenedorEmpaque = "N",
                                NumeroTransaccion = uow.GetTransactionNumber(),
                                IdExterno = detalle.Contenedor.IdExterno,
                                CodigoBarras = detalle.Contenedor.CodigoBarras,
                                NroLpn = detalle.Contenedor.NroLpn
                            };

                            contenedoresDestino.Add(contenedorDestino);
                        }
                        else
                        {
                            var tipoContenedor = BarcodeDb.TIPO_CONTENEDOR_W;
                            string idExterno;
                            string codigoBarras;

                            do
                            {
                                idExterno = uow.ContenedorRepository.GetUltimaSecuenciaTipoContenedor(tipoContenedor).ToString();
                                codigoBarras = _barcodeService.GenerateBarcode(idExterno, tipoContenedor);
                            }
                            while (uow.ContenedorRepository.ExisteContenedorActivoByCodigoBarras(codigoBarras));

                            var contenedorDestino = new Contenedor
                            {
                                NumeroPreparacion = context.PreparacionDestino.Id,
                                Numero = idContenedor,
                                TipoContenedor = tipoContenedor,
                                EstadoId = SituacionDb.ContenedorEnPreparacion,
                                Ubicacion = detalle.Contenedor.Ubicacion,
                                CodigoSubClase = detalle.Contenedor.CodigoSubClase,
                                FechaAgregado = DateTime.Now,
                                IdContenedorEmpaque = "N",
                                NumeroTransaccion = uow.GetTransactionNumber(),
                                IdExterno = idExterno,
                                CodigoBarras = codigoBarras,
                            };
                            contenedoresDestino.Add(contenedorDestino);
                        }
                    }
                }
                idContenedores.Remove(idContenedor);
            }
            context.ContenedoresDestino = contenedoresDestino;
        }

        public virtual void ProcesarPreparacionDestino(IUnitOfWork uow, TraspasoEmpresas traspaso, int nroPreparacion, TraspasoBulkOperationContext context)
        {
            Preparacion preparacionOrigen = uow.PreparacionRepository.GetPreparacionPorNumero(nroPreparacion);
            var newNroPreparacion = uow.PreparacionRepository.GetNextNumeroPreparacion();
            Carga cargaOrigen = uow.CargaRepository.GetCarga(nroPreparacion);

            var descripcion = $"Generada por la preparación traspaso: {traspaso.Id.ToString()}";
            context.PreparacionDestino = new Preparacion()
            {
                Id = newNroPreparacion,
                Descripcion = descripcion,
                FechaInicio = DateTime.Now,
                Usuario = _identity.UserId,
                Tipo = preparacionOrigen.Tipo,
                Situacion = SituacionDb.HabilitadoParaPickear,
                FlAceptaMercaderiaAveriada = preparacionOrigen.FlAceptaMercaderiaAveriada,
                Onda = preparacionOrigen.Onda,
                Predio = preparacionOrigen.Predio,
                Empresa = traspaso.EmpresaDestino,
                CantidadRechazo = 0,
                Agrupacion = preparacionOrigen.Agrupacion,
                FlPermitePickVencido = preparacionOrigen.FlPermitePickVencido,
                CodigoContenedorValidado = preparacionOrigen.CodigoContenedorValidado,
            };

            var nroCarga = uow.PreparacionRepository.GetNextNumeroCarga();
            context.CargaDestino = new Carga()
            {
                Id = nroCarga,
                Descripcion = descripcion,
                Preparacion = newNroPreparacion,
                FechaAlta = DateTime.Now,
                Ruta = cargaOrigen.Ruta
            };
        }

        public virtual void ProcesarTraspasoDocumental(IUnitOfWork uow, DocumentoEgreso documentoSalida, DocumentoIngreso documentoIngreso, long nuTransaccion)
        {
            if (documentoSalida != null)
            {
                uow.TraspasoEmpresasRepository.GenerarDocumentoSalidaTraspaso(documentoSalida, nuTransaccion);
            }

            if (documentoIngreso != null)
            {

                uow.TraspasoEmpresasRepository.GenerarDocumentoEntradaTraspaso(documentoIngreso, nuTransaccion);
            }
        }
    }
}
