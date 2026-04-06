using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.Helpers;
using WIS.Domain.Inventario.Factories;
using WIS.Domain.Parametrizacion;
using WIS.Domain.StockEntities;
using WIS.Exceptions;
using WIS.Security;
using WIS.TrafficOfficer;

namespace WIS.Domain.Inventario
{
    public class InventarioLogic
    {
        protected IIdentityService _identity;
        protected ITrafficOfficerService _concurrencyControl;

        public InventarioLogic(IIdentityService identity, ITrafficOfficerService concurrencyControl)
        {
            this._identity = identity;
            this._concurrencyControl = concurrencyControl;
        }

        #region Agregar registros

        public virtual bool AgregarRegistros(IUnitOfWork uow, IInventario inventario, InventarioSelectRegistroLpn registro, out bool enOtroIventario)
        {
            enOtroIventario = false;
            try
            {
                var nuTransaccion = uow.GetTransactionNumber();
                InventarioUbicacion inventarioUbicacion = null;

                if (!uow.InventarioRepository.IsUbicacionInInventario(registro.Ubicacion, inventario.NumeroInventario))
                {
                    inventarioUbicacion = new InventarioUbicacion();
                    inventarioUbicacion.Id = uow.InventarioRepository.GetNextNuInventarioEndereco();
                    inventarioUbicacion.Ubicacion = registro.Ubicacion;
                    inventarioUbicacion.IdInventario = inventario.NumeroInventario;
                    inventarioUbicacion.Estado = EstadoInventarioUbicacionDb.ND_ESTADO_ENDERECO_PENDIENTE;
                    inventarioUbicacion.NumeroTransaccion = nuTransaccion;

                    uow.InventarioRepository.AddInventarioEndereco(inventarioUbicacion);
                    inventario.Ubicaciones.Add(inventarioUbicacion);
                }
                else
                {
                    inventarioUbicacion = uow.InventarioRepository.GetInventarioEnderecoByInvAndUbicacion(inventario.NumeroInventario, registro.Ubicacion);
                }

                registro.NuInventario = inventario.NumeroInventario;
                var registroSuelto = (registro.NroLpn == "-" || string.IsNullOrEmpty(registro.NroLpn));

                if (!registroSuelto)
                {
                    registro.NroLpnReal = long.Parse(registro.NroLpn);

                    if (!string.IsNullOrEmpty(registro.IdDetalleLpn))
                        registro.IdDetalleLpnReal = int.Parse(registro.IdDetalleLpn);
                }

                if (uow.InventarioRepository.RegistroEnOtroInventario(inventario, registro.Ubicacion, registro.Producto, registro.Empresa, registro.NroLpnReal, registro.IdDetalleLpnReal)
                    || (!registroSuelto && uow.ManejoLpnRepository.AnyAuditoriaEnCurso(registro.NroLpnReal.Value)))
                {
                    enOtroIventario = true;
                }
                else if (inventario.TipoInventario == TipoInventario.Registro)
                {
                    var detallesFinales = uow.InventarioRepository.GetDetallesFinalesPorRegistro(inventario, registro);

                    foreach (var det in detallesFinales)
                    {
                        if (det.NroLpn != "-" && !string.IsNullOrEmpty(det.NroLpn))
                        {
                            det.NroLpnReal = long.Parse(det.NroLpn);
                            det.IdDetalleLpnReal = int.Parse(det.IdDetalleLpn);
                        }

                        det.NuInventarioUbicacion = inventarioUbicacion.Id;
                        AgregarDetalleInventario(uow, det);
                    }
                }
                else if (inventario.TipoInventario == TipoInventario.Lpn)
                {
                    if (registroSuelto)
                    {
                        registro.NuInventarioUbicacion = inventarioUbicacion.Id;
                        AgregarDetalleInventario(uow, registro);
                    }
                    else
                    {
                        var detallesFinales = uow.InventarioRepository.GetDetallesFinalesPorLpn(registro);

                        foreach (var det in detallesFinales)
                        {
                            det.NuInventarioUbicacion = inventarioUbicacion.Id;
                            det.NroLpnReal = long.Parse(det.NroLpn);
                            det.IdDetalleLpnReal = int.Parse(det.IdDetalleLpn);

                            AgregarDetalleInventario(uow, det);
                        }
                    }
                }
                else if (inventario.TipoInventario == TipoInventario.DetalleLpn)
                {
                    registro.NuInventarioUbicacion = inventarioUbicacion.Id;
                    AgregarDetalleInventario(uow, registro);
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public virtual bool AgregarUbicacion(IUnitOfWork uow, IInventario inventario, string cdEndereco)
        {
            try
            {
                var newInventarioUbicacion = new InventarioUbicacion
                {
                    Id = uow.InventarioRepository.GetNextNuInventarioEndereco(),
                    IdInventario = inventario.NumeroInventario,
                    Ubicacion = cdEndereco,
                    Estado = EstadoInventarioUbicacionDb.ND_ESTADO_ENDERECO_PENDIENTE,
                    NumeroTransaccion = uow.GetTransactionNumber()
                };

                uow.InventarioRepository.AddInventarioEndereco(newInventarioUbicacion);
                inventario.Ubicaciones.Add(newInventarioUbicacion);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #endregion

        #region Actualizar inventario

        public virtual void AceptarConteos(IUnitOfWork uow, IEnumerable<InventarioUbicacionDetalle> detalles, IEnumerable<InventarioOperacion> inventariosOperacion, out List<string> keys, out Dictionary<decimal, List<Lpn>> inventarioLpns)
        {
            keys = new List<string>();
            inventarioLpns = new Dictionary<decimal, List<Lpn>>();

            var restablecerLpn = false;

            uow.CreateTransactionNumber("AceptarConteos");
            var nuTransaccion = uow.GetTransactionNumber();

            var keysStock = from d in detalles.Where(d => !string.IsNullOrEmpty(d.Producto))
                            join o in inventariosOperacion on d.Id equals o.NumeroInventarioUbicacionDetalle
                            group d by new { o.Ubicacion, d.Empresa, d.Producto, d.Faixa, d.Identificador } into i
                            select new InventarioSelectRegistroLpn
                            {
                                IdOperacion = nuTransaccion,
                                Ubicacion = i.Key.Ubicacion,
                                Empresa = i.Key.Empresa.Value,
                                Producto = i.Key.Producto,
                                Faixa = i.Key.Faixa.Value,
                                Identificador = i.Key.Identificador
                            };

            var keysDetallesLpn = detalles
                .Where(c => c.NumeroLPN.HasValue && c.IdDetalleLPN.HasValue)
                .Select(c => new LpnDetalle()
                {
                    NumeroLPN = c.NumeroLPN.Value,
                    Id = c.IdDetalleLPN.Value,
                });

            var keysLpns = detalles
                .Where(d => d.NumeroLPN.HasValue)
                .Select(l => l.NumeroLPN)
                .Distinct()
                .Select(l => new Lpn() { NumeroLPN = l.Value });

            var keysProductos = detalles
                .Where(d => !string.IsNullOrEmpty(d.Producto))
                .GroupBy(d => new { d.Producto, Empresa = d.Empresa.Value })
                .Select(l => new Producto() { Codigo = l.Key.Producto, CodigoEmpresa = l.Key.Empresa });
            var lpnsReestablecidos = new Dictionary<long?, Lpn>();

            var lpns = uow.ManejoLpnRepository.GetLpnsByIds(keysLpns).ToList();
            var detallesLpns = uow.ManejoLpnRepository.GetDetallesLpn(keysDetallesLpn);
            var stocks = uow.InventarioRepository.GetStockByDetallesInventario(keysStock).ToList();
            var productos = uow.ProductoRepository.GetProductos(keysProductos);
            var empresasFuncionario = uow.EmpresaRepository.GetEmpresasParaUsuario(_identity.UserId);

            var resolverVencimiento = ((uow.ParametroRepository.GetParameter(ParamManager.INV_RESOLVER_VENCIMIENTO) ?? "N") == "S");
            var paramsAceptarControlesPendientes = uow.ParametroRepository.GetParams(ParamManager.INV_ACEPTAR_CTRL_CALIDAD_PEND);

            foreach (var detalleConteo in detalles)
            {
                uow.BeginTransaction();

                try
                {
                    var rechazarConteo = false;

                    var inventarioOperacion = inventariosOperacion.FirstOrDefault(i => i.NumeroInventarioUbicacionDetalle == detalleConteo.Id);

                    var aceptarControlesPendientes = GetParametroAceptarControles(uow, paramsAceptarControlesPendientes, inventarioOperacion.Predio);

                    if (!inventarioLpns.ContainsKey(inventarioOperacion.NumeroInventario))
                        inventarioLpns[inventarioOperacion.NumeroInventario] = new List<Lpn>();

                    if (detalleConteo.NumeroLPN != null && !inventarioLpns[inventarioOperacion.NumeroInventario].Any(x => x.NumeroLPN == detalleConteo.NumeroLPN))
                    {
                        inventarioLpns[inventarioOperacion.NumeroInventario].Add(lpns.FirstOrDefault(x => x.NumeroLPN == detalleConteo.NumeroLPN.Value));
                    }

                    detalleConteo.InventarioUbicacion = new InventarioUbicacion() { Ubicacion = inventarioOperacion.Ubicacion };

                    if (detalleConteo.Estado != EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_FINALIZADO_DIF)
                        continue;

                    var empresaAsignada = (empresasFuncionario.FirstOrDefault(e => e.Id == detalleConteo.Empresa.Value) != null);
                    if (!empresaAsignada)
                        throw new ValidationFailedException("General_Sec0_Error_UsuarioSinPermisosParaEmpresa", [detalleConteo.Empresa.Value.ToString()]);

                    ValidarProductoSerie(uow, productos, detalleConteo);

                    var stockActual = stocks.FirstOrDefault(x => x.Empresa == detalleConteo.Empresa
                        && x.Ubicacion == detalleConteo.InventarioUbicacion.Ubicacion
                        && x.Producto == detalleConteo.Producto
                        && x.Faixa == detalleConteo.Faixa
                        && x.Identificador == detalleConteo.Identificador);

                    var lpn = lpns.FirstOrDefault(l => l.NumeroLPN == detalleConteo.NumeroLPN);

                    if (detalleConteo.NumeroLPN.HasValue)
                    {
                        if (lpn == null)
                            throw new ValidationFailedException("INV410_Sec0_Error_LpnNoExiste", new string[] { detalleConteo.NumeroLPN.Value.ToString() });
                        else if (!EstadosLPN.GetEstadosPermitidos(TipoOperacionDb.Inventario).Contains(lpn.Estado))
                            throw new ValidationFailedException("INV410_Sec0_Error_LpnNoEstaActivo", new string[] { lpn.NumeroLPN.ToString() });
                        else if (lpn.Estado == EstadosLPN.Activo && lpn.Ubicacion != detalleConteo.InventarioUbicacion.Ubicacion)
                            throw new ValidationFailedException("INV410_Sec0_Error_LpnDistintaUbicacion", new string[] { lpn.NumeroLPN.ToString(), detalleConteo.InventarioUbicacion.Ubicacion });
                        else if (lpn.Estado != EstadosLPN.Activo)
                        {
                            var lpnPosteriormenteUtilizado = uow.ManejoLpnRepository.GetLpn(lpn.IdExterno, lpn.Tipo, lpn.Empresa);

                            if (lpn.Estado == EstadosLPN.Finalizado)
                            {
                                if (lpnPosteriormenteUtilizado != null && lpnPosteriormenteUtilizado.Estado == EstadosLPN.Activo)
                                    rechazarConteo = true;
                                else if (inventarioOperacion.RestablecerLpnFinalizado != "S")
                                    throw new ValidationFailedException("INV410_Sec0_Error_LpnNoEstaActivo", new string[] { detalleConteo.NumeroLPN.Value.ToString() });
                                else
                                    restablecerLpn = true;
                            }
                            else if (lpnPosteriormenteUtilizado != null && lpnPosteriormenteUtilizado.Estado == EstadosLPN.Generado && !string.IsNullOrEmpty(lpnPosteriormenteUtilizado.Ubicacion))
                                rechazarConteo = true;
                        }
                    }

                    var detalleLpn = detallesLpns.FirstOrDefault(d => d.NumeroLPN == detalleConteo.NumeroLPN && d.Id == detalleConteo.IdDetalleLPN);

                    stocks.Remove(stockActual);

                    ProcesarStock(uow, detalleConteo, ref stockActual, detalleLpn, nuTransaccion, rechazarConteo, restablecerLpn, resolverVencimiento, aceptarControlesPendientes);

                    stocks.Add(stockActual);

                    var nroLpnFinal = detalleConteo.NumeroLPN;
                    var idDetalleLpnFinal = detalleConteo.IdDetalleLPN;

                    ProcesarLpn(uow, detalleConteo, ref detalleLpn, lpnsReestablecidos, nuTransaccion, rechazarConteo, restablecerLpn, aceptarControlesPendientes, ref lpn, ref nroLpnFinal, ref idDetalleLpnFinal);

                    if (!rechazarConteo)
                    {
                        var serializadoAtributos = GetSerializadoAtributos(uow, lpn, detalleLpn);
                        GenerarAjusteStock(uow, inventarioOperacion.NumeroInventario, inventarioOperacion.Predio, detalleConteo, stockActual.Vencimiento, nuTransaccion, serializadoAtributos, ref keys);
                    }

                    detalleConteo.Estado = rechazarConteo ? EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_FINALIZADO_REC
                        : EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_ACTUALIZADO;

                    detalleConteo.UserId = _identity.UserId;
                    detalleConteo.NumeroTransaccion = nuTransaccion;

                    detalleConteo.NumeroLPN = nroLpnFinal;
                    detalleConteo.IdDetalleLPN = idDetalleLpnFinal;

                    uow.InventarioRepository.UpdateInventarioEnderecoDetalle(detalleConteo);

                    uow.SaveChanges();
                    uow.Commit();
                    uow.EndTransaction();
                }
                catch (ValidationFailedException ex)
                {
                    uow.Rollback();
                    throw;
                }
                catch (Exception ex)
                {
                    uow.Rollback();
                    throw;
                }
            }
        }

        public virtual void RechazarConteos(IUnitOfWork uow, IEnumerable<InventarioUbicacionDetalle> detalles, IEnumerable<InventarioOperacion> inventariosOperacion, out int cantNoRegenerados, out Dictionary<decimal, List<Lpn>> inventarioLpns, bool regenerarConteo = false)
        {
            inventarioLpns = new Dictionary<decimal, List<Lpn>>();
            cantNoRegenerados = 0;
            uow.CreateTransactionNumber("RechazarConteos");
            var nuTransaccion = uow.GetTransactionNumber();

            var keysStock = from d in detalles.Where(d => !string.IsNullOrEmpty(d.Producto))
                            join o in inventariosOperacion on d.Id equals o.NumeroInventarioUbicacionDetalle
                            group d by new { o.Ubicacion, d.Empresa, d.Producto, d.Faixa, d.Identificador } into i
                            select new InventarioSelectRegistroLpn
                            {
                                IdOperacion = nuTransaccion,
                                Ubicacion = i.Key.Ubicacion,
                                Empresa = i.Key.Empresa.Value,
                                Producto = i.Key.Producto,
                                Faixa = i.Key.Faixa.Value,
                                Identificador = i.Key.Identificador
                            };

            var keysDetallesLpn = detalles
                .Where(c => c.NumeroLPN.HasValue && c.IdDetalleLPN.HasValue)
                .Select(c => new LpnDetalle()
                {
                    NumeroLPN = c.NumeroLPN.Value,
                    Id = c.IdDetalleLPN.Value,
                });

            var keysLpns = detalles
                .Where(d => d.NumeroLPN.HasValue)
                .Select(l => l.NumeroLPN)
                .Distinct()
                .Select(l => new Lpn() { NumeroLPN = l.Value });

            var lpns = uow.ManejoLpnRepository.GetLpnsByIds(keysLpns).ToList();
            var detallesLpns = uow.ManejoLpnRepository.GetDetallesLpn(keysDetallesLpn);
            var stocks = uow.InventarioRepository.GetStockByDetallesInventario(keysStock).ToList();
            var stocksSueltos = uow.InventarioRepository.GetStockSueltoByDetallesInventario(keysStock).ToList();

            foreach (var detalleConteo in detalles)
            {
                uow.BeginTransaction();

                try
                {
                    var inventarioOperacion = inventariosOperacion.FirstOrDefault(i => i.NumeroInventarioUbicacionDetalle == detalleConteo.Id);

                    if (detalleConteo.Estado == EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_ACTUALIZADO)
                        throw new ValidationFailedException("General_Sec0_Error_RegistroYaReplicado", new string[] { inventarioOperacion.NumeroInventario.ToString(_identity.GetFormatProvider()) });

                    if (!inventarioLpns.ContainsKey(inventarioOperacion.NumeroInventario))
                        inventarioLpns[inventarioOperacion.NumeroInventario] = new List<Lpn>();

                    if (detalleConteo.NumeroLPN != null && !inventarioLpns[inventarioOperacion.NumeroInventario].Any(x => x.NumeroLPN == detalleConteo.NumeroLPN))
                    {
                        inventarioLpns[inventarioOperacion.NumeroInventario].Add(lpns.FirstOrDefault(x => x.NumeroLPN == detalleConteo.NumeroLPN.Value));
                    }

                    detalleConteo.InventarioUbicacion = new InventarioUbicacion() { Ubicacion = inventarioOperacion.Ubicacion };

                    detalleConteo.Estado = EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_FINALIZADO_REC;
                    detalleConteo.NumeroTransaccion = nuTransaccion;
                    detalleConteo.UserId = _identity.UserId;

                    if (inventarioOperacion.ModificarStockEnDiferencia == "S")
                    {
                        var stockActual = stocks.FirstOrDefault(x => x.Empresa == detalleConteo.Empresa
                            && x.Ubicacion == detalleConteo.InventarioUbicacion.Ubicacion
                            && x.Producto == detalleConteo.Producto
                            && x.Faixa == detalleConteo.Faixa
                            && x.Identificador == detalleConteo.Identificador);

                        if (stockActual != null)
                        {
                            stockActual.Inventario = "R";
                            stockActual.FechaInventario = DateTime.Now;
                            stockActual.NumeroTransaccion = nuTransaccion;

                            uow.StockRepository.UpdateStock(stockActual);
                        }

                        if (!regenerarConteo && detalleConteo.ConteoEsperado == "S" && detalleConteo.IdDetalleLPN.HasValue)
                        {
                            var detalleLpn = detallesLpns.FirstOrDefault(d => d.NumeroLPN == detalleConteo.NumeroLPN && d.Id == detalleConteo.IdDetalleLPN);
                            if (detalleLpn != null)
                            {
                                detalleLpn.IdInventario = "R";
                                detalleLpn.NumeroTransaccion = nuTransaccion;

                                uow.ManejoLpnRepository.UpdateDetalleLpn(detalleLpn);
                            }
                        }
                    }

                    uow.InventarioRepository.UpdateInventarioEnderecoDetalle(detalleConteo);

                    uow.SaveChanges();

                    if (regenerarConteo)
                    {
                        decimal qtInventariar = 0;
                        var confirmarNuevoDetalle = true;

                        var inventario = InventarioFactory.Create(inventarioOperacion.TipoInventario); //Solo para reutilizar metodo RegistroEnOtroInventario()
                        inventario.NumeroInventario = inventarioOperacion.NumeroInventario;

                        if (uow.InventarioRepository.RegistroEnOtroInventario(inventario, inventarioOperacion.Ubicacion, detalleConteo.Producto, detalleConteo.Empresa.Value, detalleConteo.NumeroLPN, detalleConteo.IdDetalleLPN))
                            confirmarNuevoDetalle = false;
                        else if (detalleConteo.NumeroLPN.HasValue)
                        {
                            var lpn = lpns.FirstOrDefault(l => l.NumeroLPN == detalleConteo.NumeroLPN.Value);

                            if (lpn == null || lpn.Estado != EstadosLPN.Activo || lpn.Ubicacion != inventarioOperacion.Ubicacion ||
                                uow.ManejoLpnRepository.AnyAuditoriaEnCurso(lpn.NumeroLPN))
                            {
                                confirmarNuevoDetalle = false;
                            }
                        }

                        var stockSuelto = stocksSueltos.FirstOrDefault(x => x.Empresa == detalleConteo.Empresa
                            && x.Ubicacion == detalleConteo.InventarioUbicacion.Ubicacion
                            && x.Producto == detalleConteo.Producto
                            && x.Faixa == detalleConteo.Faixa
                            && x.Identificador == detalleConteo.Identificador);

                        if (!detalleConteo.NumeroLPN.HasValue)
                            qtInventariar = (stockSuelto?.CantidadSuelta ?? 0);
                        else
                        {
                            var detalleLpn = detallesLpns.FirstOrDefault(d => d.NumeroLPN == detalleConteo.NumeroLPN && d.Id == detalleConteo.IdDetalleLPN);

                            if (detalleLpn != null)
                                qtInventariar = detalleLpn.Cantidad;
                            else
                            {
                                //No se generará un nuevo detalle cuando el conteo es no esperado con LPN, debe volver a contar con sus atributos correspondientes
                                confirmarNuevoDetalle = false;
                            }
                        }

                        if (confirmarNuevoDetalle)
                        {
                            var idDetalle = uow.InventarioRepository.GetNextNuInventarioEnderecoDet();
                            uow.InventarioRepository.AddInventarioEnderecoDetalle(new InventarioUbicacionDetalle
                            {
                                Id = idDetalle,
                                IdInventarioUbicacion = detalleConteo.IdInventarioUbicacion,
                                NuConteoDetalle = (detalleConteo.NuConteoDetalle ?? 0) + 1,
                                Empresa = detalleConteo.Empresa,
                                Producto = detalleConteo.Producto,
                                Faixa = detalleConteo.Faixa,
                                Identificador = detalleConteo.Identificador,
                                Vencimiento = detalleConteo.Vencimiento,
                                CantidadInventario = qtInventariar,
                                CantidadDiferencia = 0,
                                TiempoInsumido = 0,
                                Estado = EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_RECONTAR,
                                UserId = _identity.UserId,
                                FechaAlta = DateTime.Now,
                                NumeroTransaccion = nuTransaccion,
                                NumeroLPN = detalleConteo.NumeroLPN,
                                IdDetalleLPN = detalleConteo.IdDetalleLPN,
                                ConteoEsperado = !string.IsNullOrEmpty(detalleConteo.Producto) ? "S" : "N",
                                NuInstanciaConteo = uow.InventarioRepository.GetInstanciaConteo(detalleConteo.InventarioUbicacion.Ubicacion, inventarioOperacion.NumeroInventario) ?? uow.InventarioRepository.GetNextNuInstanciaConteo()
                            });

                            var detalleAtributoOrigen = uow.InventarioRepository.GetAtributosDetalle(detalleConteo.Id);
                            foreach (var det in detalleAtributoOrigen)
                            {
                                var inventareoDetalleAtributoLpn = new InventarioUbicacionDetalleAtributo();
                                inventareoDetalleAtributoLpn.Id = idDetalle;
                                inventareoDetalleAtributoLpn.Valor = det.Valor;
                                inventareoDetalleAtributoLpn.IdAtributo = det.IdAtributo;
                                uow.InventarioRepository.AddInventarioEnderecoDetalleAtr(inventareoDetalleAtributoLpn);
                            }

                        }
                        else
                            cantNoRegenerados++;
                    }

                    uow.SaveChanges();
                    uow.Commit();
                    uow.EndTransaction();
                }
                catch (ValidationFailedException ex)
                {
                    uow.Rollback();
                    throw;
                }
                catch (Exception ex)
                {
                    uow.Rollback();
                    throw;
                }
            }
        }

        #endregion

        #region Habilitar inventario

        public virtual void HabilitarInventario(IUnitOfWork uow, IInventario inventario, out Validation.Error info)
        {
            info = null;
            var transactionTO = this._concurrencyControl.CreateTransaccion();

            try
            {
                var nuTransaccion = uow.GetTransactionNumber();

                if (!inventario.IsEditable())
                    throw new ValidationFailedException("INV410_Sec0_Error_NohabilitarPorEstado");

                if (inventario.Ubicaciones.Count == 0)
                    throw new ValidationFailedException("General_Sec0_Error_InvSinUbicaciones");

                if (this._concurrencyControl.IsLocked("T_INVENTARIO", inventario.NumeroInventario.ToString(_identity.GetFormatProvider()), global: true))
                    throw new EntityLockedException("INV410_msg_Error_InventarioBloqueado", [inventario.NumeroInventario.ToString(_identity.GetFormatProvider())]);

                this._concurrencyControl.AddLock("T_INVENTARIO", inventario.NumeroInventario.ToString(_identity.GetFormatProvider()), transactionTO, isGlobal: true);

                var cantidadRegEnOtroInv = 0;
                var cancelarInventario = false;

                if (inventario.TipoInventario == TipoInventario.Ubicacion)
                    HabilitarInventarioUbicacion(uow, inventario, out cantidadRegEnOtroInv, out cancelarInventario);
                else
                    HabilitarInventarioRegistros(uow, inventario, out cantidadRegEnOtroInv, out cancelarInventario);

                inventario.NumeroTransaccion = nuTransaccion;
                inventario.Estado = cancelarInventario ? EstadoInventario.Cancelado : EstadoInventario.EnProceso;

                uow.InventarioRepository.UpdateInventario(inventario);
                uow.SaveChanges();

                if (cancelarInventario)
                {
                    info = new Validation.Error("General_Sec0_Error_ProductosOtroInv");
                }
                else if (cantidadRegEnOtroInv > 0)
                {
                    info = new Validation.Error("General_Sec0_Error_XProductosOtroInventario", cantidadRegEnOtroInv);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                this._concurrencyControl.DeleteTransaccion(transactionTO);
            }
        }

        public virtual void HabilitarInventarioRegistros(IUnitOfWork uow, IInventario inventario, out int cantidadRegEnOtroInv, out bool cancelarInventario)
        {
            cantidadRegEnOtroInv = 0;
            cancelarInventario = false;

            var cantidadRegInv = 0;

            var detallesInventario = uow.InventarioRepository.GetsInvEnderecoDetByNuInv(inventario.NumeroInventario);
            var nuTransaccion = uow.GetTransactionNumber();

            var keysLpns = detallesInventario
                .Where(d => d.NumeroLPN.HasValue)
                .Select(l => l.NumeroLPN)
                .Distinct()
                .Select(l => new Lpn() { NumeroLPN = l.Value });

            var lpns = uow.ManejoLpnRepository.GetLpnsByIds(keysLpns);
            var keysAtributos = new List<InventarioSelectRegistroLpn>();

            long? nuInstanciaConteo = uow.InventarioRepository.GetNextNuInstanciaConteo();

            foreach (var det in detallesInventario)
            {
                cantidadRegInv++;
                var eliminarDetalle = false;
                var ubicacion = det.InventarioUbicacion.Ubicacion;

                det.NuInstanciaConteo = nuInstanciaConteo;

                if (uow.InventarioRepository.RegistroEnOtroInventario(inventario, ubicacion, det.Producto, det.Empresa.Value, det.NumeroLPN, det.IdDetalleLPN))
                    eliminarDetalle = true;
                else if (det.NumeroLPN.HasValue)
                {
                    var lpn = lpns.FirstOrDefault(l => l.NumeroLPN == det.NumeroLPN.Value);

                    if (lpn == null || lpn.Estado != EstadosLPN.Activo || lpn.Ubicacion != ubicacion ||
                        uow.ManejoLpnRepository.AnyAuditoriaEnCurso(lpn.NumeroLPN))
                    {
                        eliminarDetalle = true;
                    }
                }

                if (eliminarDetalle)
                {
                    det.NumeroTransaccion = nuTransaccion;
                    det.NumeroTransaccionDelete = nuTransaccion;
                    det.UserId = _identity.UserId;

                    uow.InventarioRepository.UpdateInventarioEnderecoDetalle(det);
                    uow.SaveChanges();

                    uow.InventarioRepository.DeleteInventarioEnderecoDetalle(det);
                    uow.SaveChanges();

                    cantidadRegEnOtroInv++;
                }
                else
                {
                    det.NumeroTransaccion = nuTransaccion;
                    det.UserId = _identity.UserId;

                    uow.InventarioRepository.UpdateInventarioEnderecoDetalle(det);
                    uow.SaveChanges();

                    if (det.IdDetalleLPN.HasValue)
                    {
                        keysAtributos.Add(new InventarioSelectRegistroLpn()
                        {
                            NuInventarioUbicacionDetalle = det.Id,
                            IdDetalleLpnReal = det.IdDetalleLPN,
                            IdOperacion = nuTransaccion,
                            NroLpnReal = det.NumeroLPN
                        });
                    }

                    if (inventario.ModificarStockEnDiferencia && det.IdDetalleLPN.HasValue)
                    {
                        var detalleLpn = uow.ManejoLpnRepository.GetDetalleLpnByIdDetalle(det.NumeroLPN ?? -1, det.IdDetalleLPN.Value);

                        detalleLpn.IdInventario = "D";
                        detalleLpn.NumeroTransaccion = nuTransaccion;

                        uow.ManejoLpnRepository.UpdateDetalleLpn(detalleLpn);
                        uow.SaveChanges();
                    }


                    if (inventario.GenerarPrimerConteo)
                        GenerarPrimerConteo(uow, det, keysAtributos, nuInstanciaConteo);
                }
            }

            if (keysAtributos.Any())
                uow.InventarioRepository.SetearAtributosDetalleInventario(keysAtributos);

            cancelarInventario = (cantidadRegInv == cantidadRegEnOtroInv);
        }

        public virtual void HabilitarInventarioUbicacion(IUnitOfWork uow, IInventario inventario, out int cantidadRegEnOtroInv, out bool cancelarInventario)
        {
            cantidadRegEnOtroInv = 0;
            cancelarInventario = false;

            var cantidadRegInv = 0;
            var ubicaciones = inventario.Ubicaciones.Select(s => new Stock { Ubicacion = s.Ubicacion }).ToList();

            var detallesFinales = GetDetallesFinalesUbicaciones(uow, inventario);

            var keysLpns = detallesFinales
                .Where(d => d.NroLpn != "-" && !string.IsNullOrEmpty(d.NroLpn))
                .Select(l => l.NroLpn)
                .Distinct()
                .Select(l => new Lpn()
                {
                    NumeroLPN = long.Parse(l)
                });

            var lpns = uow.ManejoLpnRepository.GetLpnsByIds(keysLpns);

            var keysAtributos = new List<InventarioSelectRegistroLpn>();

            long? nuInstanciaConteo = uow.InventarioRepository.GetNextNuInstanciaConteo();

            foreach (var i in inventario.Ubicaciones)
            {
                var detalles = detallesFinales
                    .Where(s => s.Ubicacion == i.Ubicacion)
                    .OrderBy(r => r.Producto)
                    .ThenBy(r => r.Identificador)
                    .ToList();

                if (detalles != null && detalles.Count > 0)
                {
                    foreach (var det in detalles)
                    {
                        cantidadRegInv++;

                        if (uow.InventarioRepository.RegistroEnOtroInventario(inventario, det.Ubicacion, det.Producto, det.Empresa))
                        {
                            cantidadRegEnOtroInv++;
                            continue;
                        }

                        long? nroLpn = null;
                        int? idDetalleLpn = null;

                        if (det.NroLpn != "-" && !string.IsNullOrEmpty(det.NroLpn))
                            nroLpn = long.Parse(det.NroLpn);

                        if (nroLpn.HasValue)
                        {
                            var lpn = lpns.FirstOrDefault(l => l.NumeroLPN == nroLpn.Value);

                            if (lpn == null || lpn.Estado != EstadosLPN.Activo || lpn.Ubicacion != i.Ubicacion ||
                                uow.ManejoLpnRepository.AnyAuditoriaEnCurso(lpn.NumeroLPN))
                            {
                                cantidadRegEnOtroInv++;
                                continue;
                            }
                        }

                        if (det.IdDetalleLpn != "-" && !string.IsNullOrEmpty(det.IdDetalleLpn))
                            idDetalleLpn = int.Parse(det.IdDetalleLpn);

                        det.NuInventarioUbicacion = i.Id;
                        det.NroLpnReal = nroLpn;
                        det.IdDetalleLpnReal = idDetalleLpn;

                        var newDetalleInventario = AgregarDetalleInventario(uow, det);
                        uow.SaveChanges();

                        if (newDetalleInventario.IdDetalleLPN.HasValue)
                        {
                            keysAtributos.Add(new InventarioSelectRegistroLpn()
                            {
                                NuInventarioUbicacionDetalle = newDetalleInventario.Id,
                                IdDetalleLpnReal = newDetalleInventario.IdDetalleLPN,
                                IdOperacion = uow.GetTransactionNumber(),
                                NroLpnReal = newDetalleInventario.NumeroLPN
                            });
                        }

                        if (inventario.ModificarStockEnDiferencia && idDetalleLpn.HasValue)
                        {
                            var detalleLpn = uow.ManejoLpnRepository.GetDetalleLpnByIdDetalle(newDetalleInventario.NumeroLPN ?? -1, idDetalleLpn.Value);

                            detalleLpn.IdInventario = "D";
                            detalleLpn.NumeroTransaccion = uow.GetTransactionNumber();

                            uow.ManejoLpnRepository.UpdateDetalleLpn(detalleLpn);
                            uow.SaveChanges();
                        }

                        if (inventario.GenerarPrimerConteo)
                            GenerarPrimerConteo(uow, newDetalleInventario, keysAtributos, nuInstanciaConteo);
                    }
                }

                cantidadRegInv++;

                if (!uow.InventarioRepository.RegistroEnOtroInventario(inventario, i.Ubicacion, null, -1))
                    AddEmptyInventarioEnderecoDetalle(uow, inventario, i, nuInstanciaConteo);
                else
                    cantidadRegEnOtroInv++;

            }

            if (keysAtributos.Any())
                uow.InventarioRepository.SetearAtributosDetalleInventario(keysAtributos);

            cancelarInventario = (cantidadRegInv == cantidadRegEnOtroInv);
        }

        public virtual void AddEmptyInventarioEnderecoDetalle(IUnitOfWork uow, IInventario inventario, InventarioUbicacion invUbicacion, long? nuInstanciaConteo)
        {
            uow.InventarioRepository.AddInventarioEnderecoDetalle(new InventarioUbicacionDetalle
            {
                Id = uow.InventarioRepository.GetNextNuInventarioEnderecoDet(),
                IdInventarioUbicacion = invUbicacion.Id,
                NuConteoDetalle = 0,
                CantidadInventario = 0,
                CantidadDiferencia = 0,
                TiempoInsumido = 0,
                Estado = EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_RECONTAR,
                UserId = _identity.UserId,
                Empresa = inventario.Empresa,
                FechaAlta = DateTime.Now,
                ConteoEsperado = "N",
                NuInstanciaConteo = nuInstanciaConteo
            });
        }

        public virtual List<InventarioSelectRegistroLpn> GetDetallesFinalesUbicaciones(IUnitOfWork uow, IInventario inventario)
        {
            return uow.InventarioRepository.GetDetallesFinalesUbicaciones(inventario.Ubicaciones)
                .Where(i => (inventario.Empresa.HasValue ? i.Empresa == inventario.Empresa.Value : true)
                    && (inventario.ExcluirSueltos ? (i.NroLpn != "-" && !string.IsNullOrEmpty(i.NroLpn)) : true)
                    && (inventario.ExcluirLpns ? (i.NroLpn == "-" || string.IsNullOrEmpty(i.NroLpn)) : true)
                    && i.Cantidad > 0)
                .OrderBy(r => r.Producto)
                .ThenBy(r => r.Identificador)
                .ToList();
        }

        #endregion

        #region Cierre de Inventario

        public virtual void CierreParcial(IUnitOfWork uow, IInventario inventario, out List<string> keys)
        {
            if (!inventario.EnProceso())
                throw new ValidationFailedException("INV410_Sec0_Error_NoCerrarPorEstado");

            if (!uow.InventarioRepository.TieneConteoFinalizado(inventario.NumeroInventario))
                throw new ValidationFailedException("General_Sec0_Error_NoPuedeCerrarParcial");

            if (!uow.InventarioRepository.TieneConteoPendiente(inventario.NumeroInventario))
                throw new ValidationFailedException("General_Sec0_Error_NoPuedeCerrarParcial2");

            var transactionTO = this._concurrencyControl.CreateTransaccion();

            try
            {
                if (this._concurrencyControl.IsLocked("T_INVENTARIO", inventario.NumeroInventario.ToString(_identity.GetFormatProvider()), global: true))
                    throw new EntityLockedException("INV410_msg_Error_InventarioBloqueado", [inventario.NumeroInventario.ToString(_identity.GetFormatProvider())]);

                this._concurrencyControl.AddLock("T_INVENTARIO", inventario.NumeroInventario.ToString(_identity.GetFormatProvider()), transactionTO, isGlobal: true);

                var newInventario = InventarioFactory.Create(inventario.TipoInventario);
                newInventario.NumeroInventario = uow.InventarioRepository.GetNextNuInventario();
                newInventario.Descripcion = $"Creado a partir de inventario Nro. {inventario.NumeroInventario}";
                newInventario.Estado = EstadoInventario.EnProceso;
                newInventario.Empresa = inventario.Empresa;
                newInventario.Predio = inventario.Predio;
                newInventario.NumeroConteo = 1;
                newInventario.IsCreacionWeb = true;
                newInventario.FechaAlta = DateTime.Now;
                newInventario.CierreConteo = inventario.CierreConteo;
                newInventario.NumeroTransaccion = uow.GetTransactionNumber();
                newInventario.ExcluirLpns = inventario.ExcluirLpns;
                newInventario.ExcluirSueltos = inventario.ExcluirSueltos;
                newInventario.ActualizarConteoFin = inventario.ActualizarConteoFin;
                newInventario.ControlarVencimiento = inventario.ControlarVencimiento;
                newInventario.PermiteIngresarMotivo = inventario.PermiteIngresarMotivo;
                newInventario.RestablecerLpnFinalizado = inventario.RestablecerLpnFinalizado;
                newInventario.ModificarStockEnDiferencia = inventario.ModificarStockEnDiferencia;
                newInventario.BloquearConteoConsecutivoUsuario = inventario.BloquearConteoConsecutivoUsuario;
                newInventario.GenerarPrimerConteo = inventario.GenerarPrimerConteo;

                uow.InventarioRepository.AddInventario(newInventario);
                uow.SaveChanges();

                long? nuInstanciaConteo = inventario.GenerarPrimerConteo ? uow.InventarioRepository.GetNextNuInstanciaConteo() : null;
                var keysAtributos = new List<InventarioSelectRegistroLpn>();

                var keysStock = inventario.Ubicaciones
                    .SelectMany(x => x.Detalles
                    .Where(d => !string.IsNullOrEmpty(d.Producto) &&
                        (d.Estado == EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_ACTUALIZADO ||
                        d.Estado == EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_FINALIZADO_DIF ||
                        d.Estado == EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_FINALIZADO_OK)),
                    (ubicacion, detalle) => new InventarioSelectRegistroLpn
                    {
                        IdOperacion = uow.GetTransactionNumber(),
                        Ubicacion = ubicacion.Ubicacion,
                        Empresa = detalle.Empresa.Value,
                        Producto = detalle.Producto,
                        Faixa = detalle.Faixa.Value,
                        Identificador = detalle.Identificador
                    })
                    .ToList();

                var stocks = uow.InventarioRepository.GetStockByDetallesInventario(keysStock).ToList();

                var ubicaciones = inventario.Ubicaciones
                    .Where(w => w.Estado == EstadoInventarioUbicacionDb.ND_ESTADO_ENDERECO_PENDIENTE)
                    .ToList();

                foreach (var invUbic in ubicaciones)
                {
                    var conteosPendientes = invUbic.Detalles
                        .Where(x => x.Estado == EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_RECONTAR)
                        .ToList();

                    if (conteosPendientes != null && conteosPendientes.Count > 0)
                    {
                        var newInvUbicacion = new InventarioUbicacion
                        {
                            IdInventario = newInventario.NumeroInventario,
                            Id = uow.InventarioRepository.GetNextNuInventarioEndereco(),
                            Ubicacion = invUbic.Ubicacion,
                            NumeroTransaccion = newInventario.NumeroTransaccion,
                            Estado = EstadoInventarioUbicacionDb.ND_ESTADO_ENDERECO_PENDIENTE,
                        };

                        uow.InventarioRepository.AddInventarioEndereco(newInvUbicacion);
                        uow.SaveChanges();

                        foreach (var detalle in conteosPendientes)
                        {
                            detalle.Estado = EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_CANCELADO;
                            detalle.NumeroTransaccion = newInventario.NumeroTransaccion;
                            detalle.UserId = _identity.UserId;

                            uow.InventarioRepository.UpdateInventarioEnderecoDetalle(detalle);
                            uow.SaveChanges();

                            var newDetalleInventario = new InventarioUbicacionDetalle
                            {
                                Id = uow.InventarioRepository.GetNextNuInventarioEnderecoDet(),
                                IdInventarioUbicacion = newInvUbicacion.Id,
                                Estado = EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_RECONTAR,
                                NuConteoDetalle = 0,
                                Empresa = detalle.Empresa,
                                Producto = detalle.Producto,
                                Identificador = detalle.Identificador,
                                Faixa = detalle.Faixa,
                                Vencimiento = detalle.Vencimiento,
                                CantidadInventario = detalle.CantidadInventario,
                                CantidadDiferencia = 0,
                                TiempoInsumido = 0,
                                UserId = _identity.UserId,
                                FechaAlta = DateTime.Now,
                                NumeroTransaccion = newInventario.NumeroTransaccion,
                                NumeroLPN = detalle.NumeroLPN,
                                IdDetalleLPN = detalle.IdDetalleLPN,
                                ConteoEsperado = !string.IsNullOrEmpty(detalle.Producto) ? "S" : "N",
                                NuInstanciaConteo = nuInstanciaConteo
                            };

                            uow.InventarioRepository.AddInventarioEnderecoDetalle(newDetalleInventario);
                            uow.SaveChanges();

                            if (newDetalleInventario.IdDetalleLPN.HasValue)
                            {
                                keysAtributos.Add(new InventarioSelectRegistroLpn()
                                {
                                    NuInventarioUbicacionDetalle = newDetalleInventario.Id,
                                    IdDetalleLpnReal = newDetalleInventario.IdDetalleLPN,
                                    NroLpnReal = newDetalleInventario.NumeroLPN,
                                    IdOperacion = uow.GetTransactionNumber(),
                                });
                            }

                            if (inventario.ModificarStockEnDiferencia && detalle.IdDetalleLPN.HasValue)
                            {
                                var detalleLpn = uow.ManejoLpnRepository.GetDetalleLpnByIdDetalle(newDetalleInventario.NumeroLPN ?? -1, detalle.IdDetalleLPN.Value);

                                detalleLpn.IdInventario = "D";
                                detalleLpn.NumeroTransaccion = newInventario.NumeroTransaccion;

                                uow.ManejoLpnRepository.UpdateDetalleLpn(detalleLpn);
                            }

                            if (inventario.GenerarPrimerConteo && !string.IsNullOrEmpty(newDetalleInventario.Producto))
                                GenerarPrimerConteo(uow, newDetalleInventario, keysAtributos, nuInstanciaConteo, newInvUbicacion.Ubicacion, stocks);
                        }
                    }
                }

                if (keysAtributos.Any())
                    uow.InventarioRepository.SetearAtributosDetalleInventario(keysAtributos);

            }
            finally
            {
                this._concurrencyControl.DeleteTransaccion(transactionTO);
            }

            CerrarInventario(uow, inventario, out keys, controlarConteosPendientes: false);

            uow.SaveChanges();
        }

        public virtual void CerrarInventario(IUnitOfWork uow, IInventario inventario, out List<string> keys, bool controlarConteosPendientes = true)
        {
            keys = new List<string>();
            var restablecerLpn = false;

            if (!inventario.EnProceso())
                throw new ValidationFailedException("INV410_Sec0_Error_NoCerrarPorEstado");

            if (controlarConteosPendientes)
            {
                if (uow.InventarioRepository.TieneConteoPendiente(inventario.NumeroInventario))
                    throw new ValidationFailedException("General_Sec0_Error_NoPuedeCerrarConteosPendientes");
            }

            var transactionTO = this._concurrencyControl.CreateTransaccion();

            try
            {
                if (this._concurrencyControl.IsLocked("T_INVENTARIO", inventario.NumeroInventario.ToString(_identity.GetFormatProvider()), global: true))
                    throw new EntityLockedException("INV410_msg_Error_InventarioBloqueado", [inventario.NumeroInventario.ToString(_identity.GetFormatProvider())]);

                this._concurrencyControl.AddLock("T_INVENTARIO", inventario.NumeroInventario.ToString(_identity.GetFormatProvider()), transactionTO, isGlobal: true);


                var nuTransaccion = uow.GetTransactionNumber();
                var conteosFinalizados = uow.InventarioRepository.GetsInvEnderecoDetallesFinalizados(inventario.NumeroInventario);
                List<Lpn> lpns = null;

                if (conteosFinalizados != null && conteosFinalizados.Count > 0)
                {

                    var registros = conteosFinalizados
                        .GroupBy(c => new { c.InventarioUbicacion.Ubicacion, Empresa = c.Empresa.Value, c.Producto, Faixa = c.Faixa.Value, c.Identificador })
                        .Select(c => new InventarioSelectRegistroLpn()
                        {
                            Ubicacion = c.Key.Ubicacion,
                            Empresa = c.Key.Empresa,
                            Producto = c.Key.Producto,
                            Faixa = c.Key.Faixa,
                            Identificador = c.Key.Identificador,
                            IdOperacion = nuTransaccion
                        });

                    var keysDetallesLpn = conteosFinalizados
                        .Where(c => c.NumeroLPN.HasValue && c.IdDetalleLPN.HasValue)
                        .Select(c => new LpnDetalle()
                        {
                            NumeroLPN = c.NumeroLPN.Value,
                            Id = c.IdDetalleLPN.Value,
                        });

                    var keysLpns = conteosFinalizados
                        .Where(d => d.NumeroLPN.HasValue)
                        .Select(l => l.NumeroLPN)
                        .Distinct()
                        .Select(l => new Lpn() { NumeroLPN = l.Value });

                    var keysProductos = conteosFinalizados
                        .Where(d => !string.IsNullOrEmpty(d.Producto))
                        .GroupBy(d => new { d.Producto, Empresa = d.Empresa.Value })
                        .Select(l => new Producto() { Codigo = l.Key.Producto, CodigoEmpresa = l.Key.Empresa });

                    var lpnsReestablecidos = new Dictionary<long?, Lpn>();

                    lpns = uow.ManejoLpnRepository.GetLpnsByIds(keysLpns).ToList();
                    var detallesLpns = uow.ManejoLpnRepository.GetDetallesLpn(keysDetallesLpn);
                    var stocks = uow.InventarioRepository.GetStockByDetallesInventario(registros).ToList();
                    var productos = uow.ProductoRepository.GetProductos(keysProductos);
                    var empresasFuncionario = uow.EmpresaRepository.GetEmpresasParaUsuario(_identity.UserId);

                    var resolverVencimiento = ((uow.ParametroRepository.GetParameter(ParamManager.INV_RESOLVER_VENCIMIENTO) ?? "N") == "S");
                    var aceptarControlesPendientes = (uow.ParametroRepository.GetParameter(ParamManager.INV_ACEPTAR_CTRL_CALIDAD_PEND, new Dictionary<string, string>
                    {
                        [ParamManager.PARAM_PRED] = $"{ParamManager.PARAM_PRED}_{inventario.Predio}"
                    }) ?? "N") == "S";

                    foreach (var detalleConteo in conteosFinalizados)
                    {
                        var rechazarConteo = false;

                        if (detalleConteo.Estado != EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_FINALIZADO_DIF)
                            continue;

                        var empresaAsignada = (empresasFuncionario.FirstOrDefault(e => e.Id == detalleConteo.Empresa.Value) != null);
                        if (!empresaAsignada)
                            throw new ValidationFailedException("General_Sec0_Error_UsuarioSinPermisosParaEmpresa", [detalleConteo.Empresa.Value.ToString()]);

                        ValidarProductoSerie(uow, productos, detalleConteo);

                        var stockActual = stocks.FirstOrDefault(x => x.Empresa == detalleConteo.Empresa
                            && x.Ubicacion == detalleConteo.InventarioUbicacion.Ubicacion
                            && x.Producto == detalleConteo.Producto
                            && x.Faixa == detalleConteo.Faixa
                            && x.Identificador == detalleConteo.Identificador);

                        var lpn = lpns.FirstOrDefault(l => l.NumeroLPN == detalleConteo.NumeroLPN);

                        if (detalleConteo.NumeroLPN.HasValue)
                        {
                            if (lpn == null)
                                throw new ValidationFailedException("INV410_Sec0_Error_LpnNoExiste", new string[] { detalleConteo.NumeroLPN.Value.ToString() });
                            else if (!EstadosLPN.GetEstadosPermitidos(TipoOperacionDb.Inventario).Contains(lpn.Estado))
                                throw new ValidationFailedException("INV410_Sec0_Error_LpnNoEstaActivo", new string[] { lpn.NumeroLPN.ToString() });
                            else if (lpn.Estado == EstadosLPN.Activo && lpn.Ubicacion != detalleConteo.InventarioUbicacion.Ubicacion)
                                throw new ValidationFailedException("INV410_Sec0_Error_LpnDistintaUbicacion", new string[] { lpn.NumeroLPN.ToString(), detalleConteo.InventarioUbicacion.Ubicacion });
                            else if (lpn.Estado != EstadosLPN.Activo)
                            {
                                var lpnPosteriormenteUtilizado = uow.ManejoLpnRepository.GetLpn(lpn.IdExterno, lpn.Tipo, lpn.Empresa);

                                if (lpn.Estado == EstadosLPN.Finalizado)
                                {
                                    if (lpnPosteriormenteUtilizado != null && lpnPosteriormenteUtilizado.Estado == EstadosLPN.Activo && lpn.NumeroLPN != lpnPosteriormenteUtilizado.NumeroLPN)
                                        rechazarConteo = true;
                                    else if (!inventario.RestablecerLpnFinalizado)
                                        throw new ValidationFailedException("INV410_Sec0_Error_LpnNoEstaActivo", new string[] { detalleConteo.NumeroLPN.Value.ToString() });
                                    else
                                        restablecerLpn = true;
                                }
                                else if (lpnPosteriormenteUtilizado != null && lpnPosteriormenteUtilizado.Estado == EstadosLPN.Generado &&
                                    !string.IsNullOrEmpty(lpnPosteriormenteUtilizado.Ubicacion) && lpn.NumeroLPN != lpnPosteriormenteUtilizado.NumeroLPN)
                                {
                                    rechazarConteo = true;
                                }
                                else if (lpnPosteriormenteUtilizado.Estado == EstadosLPN.Contenedor)
                                {
                                    rechazarConteo = true;
                                }
                            }
                        }

                        var detalleLpn = detallesLpns.FirstOrDefault(d => d.NumeroLPN == detalleConteo.NumeroLPN && d.Id == detalleConteo.IdDetalleLPN);

                        stocks.Remove(stockActual);

                        ProcesarStock(uow, detalleConteo, ref stockActual, detalleLpn, nuTransaccion, rechazarConteo, restablecerLpn, resolverVencimiento, aceptarControlesPendientes);

                        stocks.Add(stockActual);

                        var nroLpnFinal = detalleConteo.NumeroLPN;
                        var idDetalleLpnFinal = detalleConteo.IdDetalleLPN;

                        ProcesarLpn(uow, detalleConteo, ref detalleLpn, lpnsReestablecidos, nuTransaccion, rechazarConteo, restablecerLpn, aceptarControlesPendientes, ref lpn, ref nroLpnFinal, ref idDetalleLpnFinal);

                        if (!rechazarConteo)
                        {
                            var serializadoAtributos = GetSerializadoAtributos(uow, lpn, detalleLpn);
                            GenerarAjusteStock(uow, inventario.NumeroInventario, inventario.Predio, detalleConteo, stockActual.Vencimiento, nuTransaccion, serializadoAtributos, ref keys);
                        }

                        detalleConteo.Estado = rechazarConteo ? EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_FINALIZADO_REC
                            : EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_ACTUALIZADO;

                        detalleConteo.UserId = _identity.UserId;
                        detalleConteo.NumeroTransaccion = nuTransaccion;

                        detalleConteo.NumeroLPN = nroLpnFinal;
                        detalleConteo.IdDetalleLPN = idDetalleLpnFinal;

                        uow.InventarioRepository.UpdateInventarioEnderecoDetalle(detalleConteo);
                        uow.SaveChanges();
                    }
                }

                inventario.Estado = EstadoInventario.Cerrado;
                inventario.NumeroTransaccion = nuTransaccion;

                uow.InventarioRepository.UpdateInventario(inventario);
                uow.InventarioRepository.FinalizarUbicacionesInventario(inventario.NumeroInventario, nuTransaccion);

                if (lpns != null && lpns.Count() > 0)
                    uow.InventarioRepository.FinalizarLpnSinStock(inventario.NumeroInventario, nuTransaccion, lpns.ToList());

                uow.SaveChanges();

            }
            finally
            {
                this._concurrencyControl.DeleteTransaccion(transactionTO);
            }
        }

        #endregion

        public virtual void CancelarInventario(IUnitOfWork uow, IInventario inventario)
        {
            if (!inventario.PermiteCancelar())
                throw new ValidationFailedException("INV410_Sec0_Error_NoPermiteCanelar");

            var transaccion = uow.GetTransactionNumber();

            inventario.Estado = EstadoInventario.Cancelado;
            inventario.NumeroTransaccion = transaccion;

            uow.InventarioRepository.UpdateInventario(inventario);

            uow.InventarioRepository.FinalizarConteosPendientes(inventario.NumeroInventario, transaccion);
            uow.InventarioRepository.FinalizarUbicacionesInventario(inventario.NumeroInventario, transaccion);
            uow.InventarioRepository.DesmarcarDiferenciaStock(inventario.NumeroInventario, transaccion);
            uow.InventarioRepository.DesmarcarDiferenciasLpn(inventario.NumeroInventario, transaccion);
            uow.InventarioRepository.FinalizarLpnSinStock(inventario.NumeroInventario, transaccion, null);

            uow.SaveChanges();
        }

        public virtual void RegenerarInventario(IUnitOfWork uow, IInventario inventario)
        {
            var insertDetalleInventario = false;

            if (!inventario.PermiteRegenerar())
                throw new ValidationFailedException("INV410_Sec0_Error_NoPermiteRegenerar");

            if (!uow.InventarioRepository.HayConteosFinalizadosOActualizados(inventario.NumeroInventario))
                throw new ValidationFailedException("INV410_Sec0_Error_NohayConteosFin");

            var newInventario = InventarioFactory.Create(inventario.TipoInventario);
            newInventario.NumeroInventario = uow.InventarioRepository.GetNextNuInventario();
            newInventario.Descripcion = $"Regeneración de inventario Nro. {inventario.NumeroInventario}";
            newInventario.Estado = EstadoInventario.EnProceso;
            newInventario.Empresa = inventario.Empresa;
            newInventario.Predio = inventario.Predio;
            newInventario.NumeroConteo = 1;
            newInventario.IsCreacionWeb = true;
            newInventario.FechaAlta = DateTime.Now;
            newInventario.CierreConteo = inventario.CierreConteo;
            newInventario.NumeroTransaccion = uow.GetTransactionNumber();
            newInventario.ExcluirLpns = inventario.ExcluirLpns;
            newInventario.ExcluirSueltos = inventario.ExcluirSueltos;
            newInventario.ActualizarConteoFin = inventario.ActualizarConteoFin;
            newInventario.ControlarVencimiento = inventario.ControlarVencimiento;
            newInventario.PermiteIngresarMotivo = inventario.PermiteIngresarMotivo;
            newInventario.RestablecerLpnFinalizado = inventario.RestablecerLpnFinalizado;
            newInventario.ModificarStockEnDiferencia = inventario.ModificarStockEnDiferencia;
            newInventario.BloquearConteoConsecutivoUsuario = inventario.BloquearConteoConsecutivoUsuario;
            newInventario.GenerarPrimerConteo = inventario.GenerarPrimerConteo;

            uow.InventarioRepository.AddInventario(newInventario);
            uow.SaveChanges();

            var keysAtributos = new List<InventarioSelectRegistroLpn>();
            var detallesAGenerarConteo = new List<InventarioUbicacionDetalle>();

            long? nuInstanciaConteo = inventario.GenerarPrimerConteo ? uow.InventarioRepository.GetNextNuInstanciaConteo() : null;

            var keysStock = inventario.Ubicaciones
                .SelectMany(x => x.Detalles
                .Where(d => !string.IsNullOrEmpty(d.Producto) &&
                    (d.Estado == EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_ACTUALIZADO ||
                    d.Estado == EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_FINALIZADO_DIF ||
                    d.Estado == EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_FINALIZADO_OK)),
                (ubicacion, detalle) => new InventarioSelectRegistroLpn
                {
                    IdOperacion = uow.GetTransactionNumber(),
                    Ubicacion = ubicacion.Ubicacion,
                    Empresa = detalle.Empresa.Value,
                    Producto = detalle.Producto,
                    Faixa = detalle.Faixa.Value,
                    Identificador = detalle.Identificador
                })
                .ToList();

            var stocks = uow.InventarioRepository.GetStockByDetallesInventario(keysStock).ToList();

            foreach (var invUbicacion in inventario.Ubicaciones)
            {
                var conteosFinalizados = invUbicacion.Detalles
                    .Where(x => x.Estado == EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_ACTUALIZADO ||
                        x.Estado == EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_FINALIZADO_DIF ||
                        x.Estado == EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_FINALIZADO_OK)
                    .ToList();

                if (conteosFinalizados != null && conteosFinalizados.Count > 0)
                {
                    var newInvUbicacion = new InventarioUbicacion
                    {
                        Id = uow.InventarioRepository.GetNextNuInventarioEndereco(),
                        IdInventario = newInventario.NumeroInventario,
                        Ubicacion = invUbicacion.Ubicacion,
                        NumeroTransaccion = newInventario.NumeroTransaccion,
                        Estado = EstadoInventarioUbicacionDb.ND_ESTADO_ENDERECO_PENDIENTE
                    };

                    var insertInventarioUbicacion = false;

                    foreach (var detalle in conteosFinalizados.Where(d => !string.IsNullOrEmpty(d.Producto)))
                    {
                        if (!uow.InventarioRepository.RegistroEnOtroInventario(inventario, invUbicacion.Ubicacion, detalle.Producto, detalle.Empresa.Value, detalle.NumeroLPN, detalle.IdDetalleLPN) &&
                            (!detalle.NumeroLPN.HasValue || (detalle.NumeroLPN.HasValue && !uow.ManejoLpnRepository.AnyAuditoriaEnCurso(detalle.NumeroLPN.Value))))
                        {
                            var newDetalleInventario = new InventarioUbicacionDetalle
                            {
                                Id = uow.InventarioRepository.GetNextNuInventarioEnderecoDet(),
                                IdInventarioUbicacion = newInvUbicacion.Id,
                                Estado = EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_RECONTAR,
                                Empresa = detalle.Empresa,
                                Producto = detalle.Producto,
                                Identificador = detalle.Identificador,
                                NuConteoDetalle = 0,
                                Faixa = detalle.Faixa,
                                Vencimiento = detalle.Vencimiento,
                                CantidadInventario = detalle.CantidadInventario,
                                CantidadDiferencia = 0,
                                TiempoInsumido = 0,
                                UserId = _identity.UserId,
                                FechaAlta = DateTime.Now,
                                NumeroTransaccion = newInventario.NumeroTransaccion,
                                NumeroLPN = detalle.NumeroLPN,
                                IdDetalleLPN = detalle.IdDetalleLPN,
                                ConteoEsperado = !string.IsNullOrEmpty(detalle.Producto) ? "S" : "N",
                            };

                            uow.InventarioRepository.AddInventarioEnderecoDetalle(newDetalleInventario);

                            if (newDetalleInventario.IdDetalleLPN.HasValue)
                            {
                                keysAtributos.Add(new InventarioSelectRegistroLpn()
                                {
                                    NuInventarioUbicacionDetalle = newDetalleInventario.Id,
                                    IdDetalleLpnReal = newDetalleInventario.IdDetalleLPN,
                                    NroLpnReal = newDetalleInventario.NumeroLPN,
                                    IdOperacion = uow.GetTransactionNumber(),
                                });
                            }

                            if (inventario.ModificarStockEnDiferencia && detalle.IdDetalleLPN.HasValue)
                            {
                                var detalleLpn = uow.ManejoLpnRepository.GetDetalleLpnByIdDetalle(newDetalleInventario.NumeroLPN ?? -1, detalle.IdDetalleLPN.Value);

                                detalleLpn.IdInventario = "D";
                                detalleLpn.NumeroTransaccion = newInventario.NumeroTransaccion;

                                uow.ManejoLpnRepository.UpdateDetalleLpn(detalleLpn);
                            }

                            detallesAGenerarConteo.Add(newDetalleInventario);
                            insertInventarioUbicacion = true;
                            insertDetalleInventario = true;
                        }
                    }

                    if (inventario.TipoInventario == TipoInventario.Ubicacion &&
                        !uow.InventarioRepository.RegistroEnOtroInventario(inventario, invUbicacion.Ubicacion, null, -1))
                    {
                        AddEmptyInventarioEnderecoDetalle(uow, inventario, newInvUbicacion, nuInstanciaConteo);
                        insertDetalleInventario = true;
                        insertInventarioUbicacion = true;
                    }

                    if (insertInventarioUbicacion)
                    {
                        uow.InventarioRepository.AddInventarioEndereco(newInvUbicacion);
                        uow.SaveChanges();

                        if (inventario.GenerarPrimerConteo & detallesAGenerarConteo.Count > 1)
                        {
                            foreach (var det in detallesAGenerarConteo)
                            {
                                GenerarPrimerConteo(uow, det, keysAtributos, nuInstanciaConteo, newInvUbicacion.Ubicacion, stocks);
                            }
                        }
                    }
                }

                if (!insertDetalleInventario)
                    throw new ValidationFailedException("General_Sec0_Error_InvNoGeneradoConteosPendientes");

                if (keysAtributos.Any())
                    uow.InventarioRepository.SetearAtributosDetalleInventario(keysAtributos);

                uow.SaveChanges();
            }
        }

        #region Metodos Auxiliares

        public virtual void ProcesarStock(IUnitOfWork uow, InventarioUbicacionDetalle detalleConteo, ref Stock stockActual, LpnDetalle detalleLpn, long nuTransaccion, bool rechazarConteo, bool restablecerLpn, bool resolverVencimiento, bool aceptarControlesPendientes)
        {
            if (stockActual != null)
            {
                if (!rechazarConteo)
                {
                    var nuevoSaldo = (stockActual.Cantidad ?? 0) + (detalleConteo.CantidadDiferencia ?? 0);

                    if (nuevoSaldo < 0)
                        throw new ValidationFailedException("General_Sec0_Error_SinSaldoDisponible");

                    if (nuevoSaldo < stockActual.ReservaSalida)
                        throw new ValidationFailedException("General_Sec0_Error_SaldoMenorReserva");

                    if ((nuevoSaldo == 0) && stockActual.ControlCalidad == EstadoControlCalidad.Pendiente && !aceptarControlesPendientes)
                        throw new ValidationFailedException("INV410_msg_Error_CtrlCalidaddPendiente", [stockActual.Ubicacion, stockActual.Producto, stockActual.Empresa.ToString(), stockActual.Identificador]);

                    if (!restablecerLpn && detalleConteo.ConteoEsperado == "S" && detalleConteo.IdDetalleLPN.HasValue)
                    {
                        detalleLpn.Ubicacion = detalleConteo.InventarioUbicacion.Ubicacion;
                        var reservaDetalleLpn = uow.ManejoLpnRepository.GetReservaDetalleLpn(detalleLpn);

                        var nuevoSaldoDetLpn = detalleLpn.Cantidad + (detalleConteo.CantidadDiferencia ?? 0);

                        if (nuevoSaldoDetLpn < reservaDetalleLpn)
                            throw new ValidationFailedException("INV410_Sec0_Error_SaldoMenorReservaLpn", [detalleLpn.Id.ToString()]);

                        if ((nuevoSaldoDetLpn == 0) && detalleLpn.IdCtrlCalidad == EstadoControlCalidad.Pendiente && !aceptarControlesPendientes)
                            throw new ValidationFailedException("INV410_msg_Error_CtrlCalidaddPendienteLPN", [detalleLpn.Id.ToString(), detalleLpn.CodigoProducto, stockActual.Empresa.ToString(), stockActual.Identificador, detalleLpn.Ubicacion]);
                    }

                    stockActual.Cantidad = nuevoSaldo;
                    stockActual.Vencimiento = resolverVencimiento ? ResolverVencimiento(stockActual.Vencimiento, detalleConteo.Vencimiento) : detalleConteo.Vencimiento;

                    AceptarControlesDeCalidadPendientes(uow, stock: stockActual);
                }

                stockActual.Inventario = "R";
                stockActual.FechaInventario = DateTime.Now;
                stockActual.FechaModificacion = DateTime.Now;
                stockActual.NumeroTransaccion = nuTransaccion;

                uow.StockRepository.UpdateStock(stockActual);
            }
            else if (!rechazarConteo)
            {
                if (detalleConteo.CantidadDiferencia < 0)
                    throw new ValidationFailedException("INV410_Sec0_Error_SinStockDiferenciaPositiva");

                stockActual = new Stock()
                {
                    Ubicacion = detalleConteo.InventarioUbicacion.Ubicacion,
                    Empresa = detalleConteo.Empresa.Value,
                    Producto = detalleConteo.Producto,
                    Faixa = detalleConteo.Faixa.Value,
                    Identificador = detalleConteo.Identificador,
                    Vencimiento = detalleConteo.Vencimiento,
                    Cantidad = detalleConteo.CantidadDiferencia,
                    ReservaSalida = 0,
                    CantidadTransitoEntrada = 0,
                    Averia = "N",
                    Inventario = "R",
                    ControlCalidad = EstadoControlCalidad.Controlado,
                    FechaInventario = DateTime.Now,
                    FechaModificacion = DateTime.Now,
                    NumeroTransaccion = nuTransaccion
                };

                uow.StockRepository.AddStock(stockActual);
            }

            uow.SaveChanges();
        }

        public virtual void GenerarAjusteStock(IUnitOfWork uow, decimal nuInventario, string predio, InventarioUbicacionDetalle detalleConteo, DateTime? vencimiento, long nuTransaccion, string serializadoAtributos, ref List<string> keys)
        {
            var ajuste = new AjusteStock
            {
                NuAjusteStock = uow.AjusteRepository.GetNextNuAjuste(),
                Ubicacion = detalleConteo.InventarioUbicacion.Ubicacion,
                Empresa = detalleConteo.Empresa.Value,
                Producto = detalleConteo.Producto,
                Faixa = detalleConteo.Faixa.Value,
                Identificador = detalleConteo.Identificador,
                QtMovimiento = detalleConteo.CantidadDiferencia,
                FechaVencimiento = vencimiento,
                FechaRealizado = DateTime.Now,
                TipoAjuste = TipoAjusteDb.Inventario,
                CdMotivoAjuste = !string.IsNullOrEmpty(detalleConteo.MotivoAjuste) ? detalleConteo.MotivoAjuste : MotivoAjusteDb.SinRegistrar,
                DescMotivo = $"Inventario Nro. {nuInventario.ToString(_identity.GetFormatProvider())}",
                NuTransaccion = nuTransaccion,
                Predio = predio,
                IdAreaAveria = "N",
                FechaMotivo = DateTime.Now,
                NuInventarioEnderecoDet = detalleConteo.Id,
                Funcionario = _identity.UserId,
                Aplicacion = _identity.Application,
                Atributos = serializadoAtributos
            };

            uow.AjusteRepository.Add(ajuste);

            keys.Add(ajuste.NuAjusteStock.ToString());

            uow.SaveChanges();
        }

        public virtual void ProcesarLpn(IUnitOfWork uow, InventarioUbicacionDetalle detalleConteo, ref LpnDetalle detalleLpn, Dictionary<long?, Lpn> lpnsReestablecidos, long nuTransaccion, bool rechazarConteo, bool restablecerLpn, bool aceptarControlesPendientes, ref Lpn lpn, ref long? nroLpnFinal, ref int? idDetalleLpnFinal)
        {
            if (detalleConteo.NumeroLPN.HasValue)
            {
                if (!rechazarConteo)
                {
                    if (restablecerLpn)
                    {
                        if (!lpnsReestablecidos.ContainsKey(detalleConteo.NumeroLPN))
                        {
                            lpn.Ubicacion = detalleConteo.InventarioUbicacion.Ubicacion;
                            lpn = RestablecerLpnInventario(uow, lpn, nuTransaccion);

                            lpnsReestablecidos.Add(detalleConteo.NumeroLPN, lpn);

                            uow.SaveChanges();
                        }
                        else
                            lpn = lpnsReestablecidos[detalleConteo.NumeroLPN];
                    }
                    else if (lpn.Estado == EstadosLPN.Generado)
                    {
                        lpn.Estado = EstadosLPN.Activo;
                        lpn.Ubicacion = detalleConteo.InventarioUbicacion.Ubicacion;
                        lpn.FechaActivacion = DateTime.Now;
                        lpn.FechaModificacion = DateTime.Now;
                        lpn.NumeroTransaccion = nuTransaccion;

                        uow.ManejoLpnRepository.UpdateLpn(lpn);
                        uow.SaveChanges();
                    }

                    nroLpnFinal = lpn.NumeroLPN;

                    if (detalleConteo.ConteoEsperado == "N" && !detalleConteo.IdDetalleLPN.HasValue)
                    {
                        idDetalleLpnFinal = uow.ManejoLpnRepository.GetNextIdDetalleLpn();

                        detalleLpn = new LpnDetalle()
                        {
                            Id = idDetalleLpnFinal.Value,
                            NumeroLPN = nroLpnFinal.Value,
                            IdLineaSistemaExterno = detalleConteo.Id.ToString(_identity.GetFormatProvider()),
                            Empresa = detalleConteo.Empresa.Value,
                            CodigoProducto = detalleConteo.Producto,
                            Faixa = detalleConteo.Faixa.Value,
                            Lote = detalleConteo.Identificador,
                            Vencimiento = detalleConteo.Vencimiento,
                            Cantidad = (detalleConteo.CantidadDiferencia ?? 0),
                            CantidadReserva = 0,
                            CantidadDeclarada = 0,
                            CantidadRecibida = 0,
                            CantidadExpedida = 0,
                            IdAveria = "N",
                            IdInventario = "R",
                            IdCtrlCalidad = EstadoControlCalidad.Controlado,
                            NumeroTransaccion = nuTransaccion,
                        };

                        uow.ManejoLpnRepository.AddDetalleLpn(detalleLpn);
                        uow.SaveChanges();

                        var atributosInventario = uow.InventarioRepository.GetAtributosDetalle(detalleConteo.Id);

                        foreach (var at in atributosInventario)
                        {
                            uow.ManejoLpnRepository.AddAtributoDetalle(new LpnDetalleAtributo()
                            {
                                IdLpnDetalle = detalleLpn.Id,
                                NumeroLpn = detalleLpn.NumeroLPN,
                                Tipo = lpn.Tipo,
                                IdAtributo = at.IdAtributo,
                                ValorAtributo = at.Valor,
                                Empresa = detalleLpn.Empresa,
                                Producto = detalleLpn.CodigoProducto,
                                Faixa = detalleLpn.Faixa,
                                Lote = detalleLpn.Lote,
                                Estado = EstadoLpnAtributo.Ingresado,
                                NumeroTransaccion = nuTransaccion
                            });
                        }
                    }
                    else if (detalleLpn != null)
                    {
                        detalleLpn.Cantidad += (detalleConteo.CantidadDiferencia ?? 0);
                        detalleLpn.IdInventario = "R";
                        detalleLpn.NumeroTransaccion = nuTransaccion;
                        detalleLpn.Vencimiento = detalleConteo.Vencimiento;

                        var atributosDetalle = uow.ManejoLpnRepository.GetAllLpnDetalleAtributo(detalleLpn.NumeroLPN, detalleLpn.Id, detalleLpn.Empresa, detalleLpn.CodigoProducto, detalleLpn.Lote, detalleLpn.Faixa);//Mantener previo al if

                        if (restablecerLpn)
                        {
                            idDetalleLpnFinal = uow.ManejoLpnRepository.GetNextIdDetalleLpn();

                            detalleLpn.NumeroLPN = nroLpnFinal.Value;
                            detalleLpn.Id = idDetalleLpnFinal.Value;

                            uow.ManejoLpnRepository.AddDetalleLpn(detalleLpn);
                            uow.SaveChanges();

                            foreach (var at in atributosDetalle)
                            {
                                at.NumeroLpn = nroLpnFinal.Value;
                                at.IdLpnDetalle = idDetalleLpnFinal.Value;
                                at.NumeroTransaccion = nuTransaccion;

                                uow.ManejoLpnRepository.AddAtributoDetalle(at);
                            }
                        }
                        else
                        {
                            AceptarControlesDeCalidadPendientes(uow, detalleLpn: detalleLpn);

                            uow.ManejoLpnRepository.UpdateDetalleLpn(detalleLpn);
                        }
                    }

                    uow.SaveChanges();
                }
                else
                {
                    detalleLpn.IdInventario = "R";
                    detalleLpn.NumeroTransaccion = nuTransaccion;

                    uow.ManejoLpnRepository.UpdateDetalleLpn(detalleLpn);
                    uow.SaveChanges();
                }

                uow.SaveChanges();
            }
        }

        public static string GetSerializadoAtributos(IUnitOfWork uow, Lpn lpn, LpnDetalle detalle)
        {
            if (lpn == null || detalle == null)
                return null;

            var formaterDateTime = uow.ParametroRepository.GetParameter(ParamManager.DATETIME_FORMAT_DATE_SECONDS);
            var separador = uow.ParametroRepository.GetParameter(ParamManager.NUMBER_DECIMAL_SEPARATOR);

            var atributos = uow.AtributoRepository.GetAtributos();

            var serializado = new DatosAjusteStockLpnAtributos(lpn.NumeroLPN, lpn.IdExterno, lpn.Tipo);

            var atributosCabezal = uow.ManejoLpnRepository.GetAllLpnAtributo(lpn.NumeroLPN)
                .Select(a => new AtributoValor()
                {
                    Nombre = a.Nombre,
                    Valor = AtributoHelper.GetValorDisplayByIdTipo(a.Valor, atributos.FirstOrDefault(at => at.Id == a.Id), separador, formaterDateTime, true)

                })
                .ToList();

            serializado.AtributosCabezal = atributosCabezal;

            var atributosDetalle = new List<AtributoValor>();
            if (detalle != null)
            {
                atributosDetalle = uow.ManejoLpnRepository.GetAllLpnDetalleAtributo(lpn.NumeroLPN, detalle.Id, detalle.Empresa, detalle.CodigoProducto, detalle.Lote, detalle.Faixa)

                .Select(a => new AtributoValor()
                {
                    Nombre = a.NombreAtributo,
                    Valor = AtributoHelper.GetValorDisplayByIdTipo(a.ValorAtributo, atributos.FirstOrDefault(at => at.Id == a.IdAtributo), separador, formaterDateTime, true)
                })
                .ToList();
            }

            serializado.AtributosDetalle = atributosDetalle;

            return JsonConvert.SerializeObject(serializado, new JsonSerializerSettings
            {
                StringEscapeHandling = StringEscapeHandling.EscapeHtml
            });
        }

        public static DateTime? ResolverVencimiento(DateTime? vencimientoStock, DateTime? vencimientoEntrante)
        {
            if (vencimientoEntrante != null)
            {
                if (vencimientoStock == null)
                    return vencimientoEntrante;
                else if (vencimientoStock >= vencimientoEntrante)
                    return vencimientoEntrante;
                else
                    return vencimientoStock;
            }

            return vencimientoStock;
        }

        public static Lpn RestablecerLpnInventario(IUnitOfWork uow, Lpn lpn, long nuTransaccion)
        {
            if (lpn != null)
            {
                var nuLpn = lpn.NumeroLPN;
                if (lpn.Estado == EstadosLPN.Finalizado)
                {
                    lpn = new Lpn
                    {
                        NumeroLPN = uow.ManejoLpnRepository.GetNextNuLpn(),
                        IdExterno = lpn.IdExterno,
                        Tipo = lpn.Tipo,
                        Estado = EstadosLPN.Activo,
                        Ubicacion = lpn.Ubicacion,
                        FechaAdicion = DateTime.Now,
                        FechaActivacion = DateTime.Now,
                        Empresa = lpn.Empresa,
                        IdPacking = lpn.IdPacking,
                        NumeroTransaccion = nuTransaccion
                    };
                    uow.ManejoLpnRepository.AddLPN(lpn);

                    var atributosCabezal = uow.ManejoLpnRepository.GetAllLpnAtributo(nuLpn);

                    foreach (var at in atributosCabezal)
                    {
                        at.NumeroLpn = lpn.NumeroLPN;
                        at.NumeroTransaccion = nuTransaccion;
                        at.Estado = EstadoLpnAtributo.Ingresado;

                        uow.ManejoLpnRepository.AddAtributoAsociado(at);
                    }

                    var barras = uow.ManejoLpnRepository.GetCodigoDeBarras(nuLpn);

                    foreach (var cb in barras)
                    {
                        cb.IdLpnBarras = uow.ManejoLpnRepository.GetNextLpnBarras();
                        cb.NumeroLpn = lpn.NumeroLPN;

                        uow.ManejoLpnRepository.AddLPNBarras(cb);
                    }
                }
            }
            return lpn;
        }

        public static InventarioFiltros GetFiltros(IUnitOfWork uow, ComponentContext context, IIdentityService identity)
        {
            var filtros = new InventarioFiltros();

            var nuInventario = decimal.Parse(context.GetParameter("nuInventario"), identity.GetFormatProvider());
            var jsonFiltro = context.GetParameter("filtro");

            var inventario = uow.InventarioRepository.GetInventario(nuInventario);

            filtros.NuInventario = inventario.NumeroInventario;
            filtros.Empresa = inventario.Empresa;
            filtros.Predio = inventario.Predio;
            filtros.ExcluirSueltos = inventario.ExcluirSueltos;
            filtros.ExcluirLpns = inventario.ExcluirLpns;
            filtros.PermiteUbicacionesDeOtrosInventarios = inventario.PermiteUbicacionesDeOtrosInventarios;

            if (!string.IsNullOrEmpty(jsonFiltro))
            {
                var filtro = JsonConvert.DeserializeObject<FiltroLpn>(jsonFiltro);

                if (filtro != null)
                {
                    foreach (var atributo in filtro.AtributosCabezal)
                    {
                        filtros.AtributosCabezal[int.Parse(atributo.Id)] = atributo.Value;
                    }

                    foreach (var atributo in filtro.AtributosDetalle)
                    {
                        filtros.AtributosDetalle[int.Parse(atributo.Id)] = atributo.Value;
                    }
                }
            }

            return filtros;
        }

        public virtual void ValidarProductoSerie(IUnitOfWork uow, IEnumerable<Producto> productos, InventarioUbicacionDetalle detalleConteo)
        {
            var producto = productos.FirstOrDefault(p => p.Codigo == detalleConteo.Producto && p.CodigoEmpresa == detalleConteo.Empresa.Value);

            var manejaSerie = (producto.ManejoIdentificador != General.Enums.ManejoIdentificador.Unknown) ? producto.IsIdentifiedBySerie()
                : (producto.ManejoIdentificadorId == "S");

            if (detalleConteo.CantidadDiferencia > 0 && manejaSerie)
            {
                if (uow.StockRepository.ExisteSerie(detalleConteo.Producto, detalleConteo.Empresa.Value, detalleConteo.Identificador))
                    throw new ValidationFailedException("General_Sec0_Error_SerieYaExiste", new string[] { detalleConteo.Identificador, detalleConteo.Producto, detalleConteo.Empresa.ToString() });
            }
        }

        public virtual InventarioUbicacionDetalle AgregarDetalleInventario(IUnitOfWork uow, InventarioSelectRegistroLpn registro)
        {
            var newDetalleInventario = new InventarioUbicacionDetalle
            {
                Id = uow.InventarioRepository.GetNextNuInventarioEnderecoDet(),
                IdInventarioUbicacion = registro.NuInventarioUbicacion,
                Empresa = registro.Empresa,
                Producto = registro.Producto,
                Faixa = registro.Faixa,
                Identificador = registro.Identificador,
                CantidadInventario = registro.Cantidad,
                TiempoInsumido = 0,
                NuConteoDetalle = registro.NuConteo ?? 0,
                CantidadDiferencia = 0,
                Estado = EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_RECONTAR,
                UserId = _identity.UserId,
                FechaAlta = DateTime.Now,
                NumeroTransaccion = uow.GetTransactionNumber(),
                NumeroLPN = registro.NroLpnReal,
                IdDetalleLPN = registro.IdDetalleLpnReal,
                Vencimiento = registro.Vencimiento,
                ConteoEsperado = "S",
                NuInstanciaConteo = registro.NuInstanciaConteo ?? uow.InventarioRepository.GetNextNuInstanciaConteo()
            };

            uow.InventarioRepository.AddInventarioEnderecoDetalle(newDetalleInventario);

            return newDetalleInventario;
        }

        public virtual void GenerarPrimerConteo(IUnitOfWork uow, InventarioUbicacionDetalle detalleInventario, List<InventarioSelectRegistroLpn> keysAtributos, long? nuInstanciaConteo, string ubicacion = null, List<Stock> stocks = null)
        {
            var regenerarInventario = !string.IsNullOrEmpty(ubicacion);

            detalleInventario.Estado = EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_CONTADO;
            detalleInventario.NumeroTransaccion = uow.GetTransactionNumber();
            detalleInventario.MotivoAjuste = MotivoAjusteDb.SinRegistrar;
            detalleInventario.NuInstanciaConteo = nuInstanciaConteo;

            if (regenerarInventario)
            {
                var stock = stocks?.FirstOrDefault(s => s.Ubicacion == ubicacion
                        && s.Empresa == detalleInventario.Empresa
                        && s.Producto == detalleInventario.Producto
                        && s.Faixa == detalleInventario.Faixa
                        && s.Identificador == detalleInventario.Identificador);

                if (stock != null)
                {
                    detalleInventario.Vencimiento = stock.Vencimiento;
                    detalleInventario.CantidadDiferencia = (stock.Cantidad - detalleInventario.CantidadInventario);
                }
                else
                    detalleInventario.CantidadDiferencia = (0 - detalleInventario.CantidadInventario);
            }

            uow.InventarioRepository.UpdateInventarioEnderecoDetalle(detalleInventario);
            uow.SaveChanges();

            var registro = new InventarioSelectRegistroLpn()
            {
                NuInventarioUbicacion = detalleInventario.IdInventarioUbicacion,
                Empresa = detalleInventario.Empresa.Value,
                Producto = detalleInventario.Producto,
                Faixa = detalleInventario.Faixa.Value,
                Identificador = detalleInventario.Identificador,
                Cantidad = 0,
                NroLpnReal = detalleInventario.NumeroLPN,
                IdDetalleLpnReal = detalleInventario.IdDetalleLPN,
                Vencimiento = detalleInventario.Vencimiento,

                NuConteo = (detalleInventario.NuConteoDetalle + 1),
                NuInstanciaConteo = nuInstanciaConteo
            };

            var newDetallePendiente = AgregarDetalleInventario(uow, registro);
            uow.SaveChanges();

            if (newDetallePendiente.IdDetalleLPN.HasValue)
            {
                keysAtributos.Add(new InventarioSelectRegistroLpn()
                {
                    NuInventarioUbicacionDetalle = newDetallePendiente.Id,
                    IdDetalleLpnReal = newDetallePendiente.IdDetalleLPN,
                    IdOperacion = uow.GetTransactionNumber(),
                    NroLpnReal = newDetallePendiente.NumeroLPN
                });
            }
        }

        public virtual bool GetParametroAceptarControles(IUnitOfWork uow, List<ParametroConfiguracion> paramsAceptarControlesPendientes, string predio)
        {
            var aceptarControles = false;
            var claveEntidad = $"{ParamManager.PARAM_PRED}_{predio}";

            var paramValue = paramsAceptarControlesPendientes.FirstOrDefault(p => p.Clave == claveEntidad)?.Valor;

            if (!string.IsNullOrEmpty(paramValue))
                aceptarControles = paramValue == "S";
            else
                aceptarControles = (paramsAceptarControlesPendientes.FirstOrDefault(p => p.Clave == ParamManager.PARAM_GRAL)?.Valor ?? "N") == "S";

            return aceptarControles;
        }

        public virtual void AceptarControlesDeCalidadPendientes(IUnitOfWork uow, Stock stock = null, LpnDetalle detalleLpn = null)
        {
            if (stock == null && detalleLpn == null)
                return;

            ControlDeCalidadPendiente controlDeCalidad = null;

            if (stock != null && stock.Cantidad == 0)
            {
                controlDeCalidad = uow.ControlDeCalidadRepository.GetControlDeCalidadPendiente(stock.Ubicacion, stock.Empresa, stock.Producto, stock.Faixa, stock.Identificador, estado: "N");

                stock.SetControlado();
            }

            if (detalleLpn != null && detalleLpn.Cantidad == 0)
            {
                controlDeCalidad = uow.ControlDeCalidadRepository.GetControlDeCalidadPendiente(detalleLpn.Ubicacion, detalleLpn.Empresa, detalleLpn.CodigoProducto, detalleLpn.Faixa, detalleLpn.Lote, detalleLpn.NumeroLPN, detalleLpn.Id, "N");

                detalleLpn.IdCtrlCalidad = EstadoControlCalidad.Controlado;
            }

            if (controlDeCalidad != null)
            {
                controlDeCalidad.Aceptar(_identity.UserId);
                uow.ControlDeCalidadRepository.UpdateControlPendiente(controlDeCalidad);
            }
        }

        #endregion
    }
}



