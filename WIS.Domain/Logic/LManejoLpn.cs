using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.Helpers;
using WIS.Domain.ManejoStock.Constants;
using WIS.Domain.Parametrizacion;
using WIS.Domain.Recepcion;
using WIS.Domain.Recepcion.Enums;
using WIS.Domain.StockEntities;
using WIS.GridComponent;
using WIS.Security;

namespace WIS.Domain.Logic
{
    public class LManejoLpn
    {
        public IIdentityService _identity;
        public readonly Logger _logger;
        public LManejoLpn(IIdentityService identity, Logger logger)
        {
            this._identity = identity;
            this._logger = logger;
        }

        public virtual void AprobarAuditoriaLpn(IUnitOfWork uow, List<string> keys, AuditoriaLpn detAditoria, Lpn lpn, LpnTipo lpnTipoLpn, List<LpnBarras> lpnBarras, short? areaUbicacion)
        {
            EtiquetaLote etiquetaActiva = null;

            foreach (var barra in lpnBarras)
            {
                etiquetaActiva = uow.EtiquetaLoteRepository.GetEtiquetaLoteActiva(lpnTipoLpn.EtiquetaRecepcion, barra.CodigoBarras, false);
                if (etiquetaActiva != null)
                    break;
            }

            ValidarEdicionAtributo(uow, lpn, detAditoria.Empresa, detAditoria.Producto, detAditoria.Identificador, detAditoria.Faixa);
            uow.ManejoLpnRepository.ValidarCantidadesAuditadasContraReservas(lpn, detAditoria.AuditoriaAgrupador, detAditoria.Empresa, detAditoria.Producto, detAditoria.Identificador, detAditoria.Faixa, detAditoria.Nivel);

            Agenda agenda = null;

            if (etiquetaActiva != null)
                agenda = uow.AgendaRepository.GetAgendaSinDetalles(etiquetaActiva.NumeroAgenda);

            var atributosDetalleLpn = uow.ManejoLpnRepository.GetAtributosAuditados(detAditoria.Auditoria);

            AprobarAuditoria(uow, detAditoria, lpn, lpnTipoLpn, lpnBarras, areaUbicacion, etiquetaActiva, agenda, atributosDetalleLpn, out string keyAjusteStock);

            if (!string.IsNullOrEmpty(keyAjusteStock))
                keys.Add(keyAjusteStock);

            if (!uow.ManejoLpnRepository.AnyDetalleLpnConStock(lpn.NumeroLPN))
            {
                lpn.FechaModificacion = DateTime.Now;
                lpn.FechaFin = DateTime.Now;
                lpn.Estado = EstadosLPN.Finalizado;
                lpn.NumeroTransaccion = uow.GetTransactionNumber();
                uow.ManejoLpnRepository.UpdateLpn(lpn);
            }
        }

        public virtual void AprobarAuditoria(IUnitOfWork uow, AuditoriaLpn auditoria, Lpn lpn, LpnTipo lpnTipoLpn, List<LpnBarras> lpnBarra, short? areaUbicacion, EtiquetaLote etiquetaActiva, Agenda agenda, List<LpnAuditoriaAtributo> atributosDetalleLpn, out string keyAjusteStock)
        {
            keyAjusteStock = string.Empty;
            var detalle = uow.ManejoLpnRepository.GetDetalleEtiquetaLpn(lpn.NumeroLPN, auditoria.LpnDet ?? -1, auditoria.Producto, auditoria.Empresa, auditoria.Identificador, auditoria.Faixa);


            if (detalle != null)
            {
                var serializadoAtributos = GetSerializadoAtributos(uow, lpn, detalle, atributosDetalleLpn);

                if (auditoria.Diferencia > 0)
                    AceptarProductoDeMasLpn(this._identity.UserId, uow, detalle, agenda, etiquetaActiva, lpnTipoLpn, auditoria, lpn, serializadoAtributos, out keyAjusteStock);
                else if (auditoria.Diferencia < 0)
                    AceptarProductoDeMenosLpn(this._identity.UserId, uow, detalle, agenda, etiquetaActiva, auditoria, lpn, serializadoAtributos, out keyAjusteStock);
            }
            else
            {
                detalle = uow.ManejoLpnRepository.AgregarDetalleLpnRecepcion(agenda, lpn.NumeroLPN, lpn.Tipo, auditoria.Producto, auditoria.Empresa, auditoria.Identificador, auditoria.Faixa, 0, auditoria.Vencimiento, this._identity, uow.GetTransactionNumber());
                uow.SaveChanges();

                var serializadoAtributos = GetSerializadoAtributos(uow, lpn, detalle, atributosDetalleLpn);
                AceptarProductoDeMasLpn(this._identity.UserId, uow, detalle, agenda, etiquetaActiva, lpnTipoLpn, auditoria, lpn, serializadoAtributos, out keyAjusteStock);
            }

            if (atributosDetalleLpn != null)
            {
                var detallelpn = uow.ManejoLpnRepository.GetDetalleLpnByAtributos(detalle.NumeroLPN, auditoria.Producto, auditoria.Empresa, auditoria.Identificador, auditoria.Faixa, atributosDetalleLpn);
                if (detallelpn == null || (detallelpn != null && detallelpn.Id == detalle.Id))
                {
                    foreach (var extradata in atributosDetalleLpn)
                    {
                        var lpnDetAtributo = uow.ManejoLpnRepository.GetLpnDetalleAtributo(detalle.Id, detalle.NumeroLPN, extradata.IdAtributo, lpnTipoLpn.Tipo, detalle.CodigoProducto, detalle.Empresa, detalle.Lote);
                        lpnDetAtributo.ValorAtributo = extradata.ValorAtributo;
                        lpnDetAtributo.Estado = EstadoLpnAtributo.Ingresado;
                        lpnDetAtributo.NumeroTransaccion = uow.GetTransactionNumber();
                        uow.ManejoLpnRepository.UpdateLpnDetalleAtributo(lpnDetAtributo);
                    }
                    uow.SaveChanges();
                }
                else
                {
                    foreach (var extradata in atributosDetalleLpn)
                    {
                        int idAtributo = extradata.IdAtributo;
                        LpnDetalleAtributo lpnDetAtributo = uow.ManejoLpnRepository.GetLpnDetalleAtributo(detalle.Id, detalle.NumeroLPN, idAtributo, lpnTipoLpn.Tipo, detalle.CodigoProducto, detalle.Empresa, detalle.Lote);
                        uow.ManejoLpnRepository.RemoveAtributoDetalle(lpnDetAtributo);
                        uow.SaveChanges();
                    }

                    LpnDetalle detalleLpn = uow.ManejoLpnRepository.GetDetalleEtiquetaLpn(detalle.NumeroLPN, detallelpn.Id, detalle.CodigoProducto, detalle.Empresa, detalle.Lote, detalle.Faixa);
                    detalleLpn.Cantidad = detalle.Cantidad + detalleLpn.Cantidad;
                    detalleLpn.CantidadRecibida = detalle.CantidadRecibida + detalleLpn.CantidadRecibida;
                    detalleLpn.NumeroTransaccion = uow.GetTransactionNumber();
                    detalleLpn.Vencimiento = detalleLpn.Vencimiento > detalle.Vencimiento ? detalle.Vencimiento : detalleLpn.Vencimiento;

                    uow.ManejoLpnRepository.UpdateDetalleLpn(detalleLpn);

                    uow.ManejoLpnRepository.RemoveDetalleLpn(detalle);
                    uow.SaveChanges();
                }
            }
        }

        public virtual void AceptarProductoDeMenosLpn(int userId, IUnitOfWork uow, LpnDetalle detalle, Agenda agenda, EtiquetaLote etiquetaActiva, AuditoriaLpn auditoria, Lpn lpn, string serializadoAtributos, out string keyAjusteStock)
        {
            keyAjusteStock = "";
            var dsMotivo = "Auditoria Producto de menos en el LPN";
            var cdMotivo = MotivoAjusteDb.SinRegistrar;

            if (etiquetaActiva != null)
            {
                var detEtiqueta = uow.EtiquetaLoteRepository.GetEtiquetaLoteDetalle(etiquetaActiva.Numero, auditoria.Producto, auditoria.Empresa, auditoria.Faixa, auditoria.Identificador);

                new AjusteEtiquetaLote(uow
                      , this._identity.UserId
                      , this._identity.Application
                      , agenda.Id
                      , etiquetaActiva.IdUbicacion
                      , etiquetaActiva.IdUbicacionSugerida
                      , auditoria.Producto
                      , auditoria.Faixa
                      , auditoria.Empresa
                      , auditoria.Identificador
                      , etiquetaActiva.Numero
                      , etiquetaActiva.TipoEtiqueta
                      , etiquetaActiva.NumeroExterno
                      , (short)etiquetaActiva.Estado
                      , EstadoAgendaDetalleDb.ConferidaConDiferencias
                      , (detEtiqueta.Cantidad ?? 0) + (auditoria.Diferencia ?? 0)
                      , (detEtiqueta.Cantidad ?? 0))
                        .AjustarEtiqueta(true);

                if (agenda == null || ((agenda.Estado == EstadoAgenda.Cerrada || agenda.Estado == EstadoAgenda.Cancelada)))
                {
                    var ubicacion = uow.UbicacionRepository.GetUbicacion(etiquetaActiva.IdUbicacion);
                    var ajuste = new AjusteStock
                    {
                        IdAreaAveria = "N",
                        Aplicacion = this._identity.Application,
                        Funcionario = this._identity.UserId,
                        NuAjusteStock = uow.AjusteRepository.GetNextNuAjuste(),
                        Producto = auditoria.Producto,
                        Faixa = auditoria.Faixa,
                        Identificador = auditoria.Identificador,
                        Empresa = auditoria.Empresa,
                        FechaRealizado = DateTime.Now,
                        TipoAjuste = TipoAjusteDb.Stock,
                        QtMovimiento = auditoria.Diferencia,
                        DescMotivo = dsMotivo,
                        CdMotivoAjuste = cdMotivo,
                        Ubicacion = ubicacion.Id,
                        Serializado = "",
                        FechaVencimiento = auditoria.Vencimiento,
                        NuTransaccion = uow.GetTransactionNumber(),
                        Predio = ubicacion.NumeroPredio,
                        Atributos = serializadoAtributos,
                    };

                    int nuAjuste = uow.AjusteRepository.Add(ajuste);

                    keyAjusteStock = nuAjuste.ToString();
                }
            }
            else
            {
                keyAjusteStock = ConsumoInterno(uow, lpn.Ubicacion, auditoria.Empresa, auditoria.Producto, auditoria.Faixa, auditoria.Identificador, auditoria.Vencimiento, auditoria.Diferencia ?? 0, cdMotivo, dsMotivo, serializadoAtributos);
            }

            if (agenda != null && (agenda.Estado != EstadoAgenda.Cerrada && agenda.Estado != EstadoAgenda.Cancelada))
            {
                detalle.CantidadRecibida = detalle.CantidadRecibida + (auditoria.Diferencia ?? 0);
            }

            detalle.Cantidad = detalle.Cantidad + (auditoria.Diferencia ?? 0);
            detalle.NumeroTransaccion = uow.GetTransactionNumber();
            uow.ManejoLpnRepository.UpdateDetalleLpn(detalle);

            uow.SaveChanges();

            if (etiquetaActiva == null)
            {
                var Lpn = uow.ManejoLpnRepository.GetLpn(detalle.NumeroLPN);
                var stock = uow.StockRepository.GetStock(auditoria.Empresa, auditoria.Producto, auditoria.Faixa, Lpn.Ubicacion, auditoria.Identificador);

                if ((stock.Cantidad - stock.ReservaSalida) < detalle.Cantidad)
                {
                    throw new Exception("STO730_Sec0_Error_LaReservaNoPuedeSerMenorAStock");
                }
            }

        }

        public virtual string ConsumoInterno(IUnitOfWork uow, string endereco, int empresa, string codigoProducto, decimal faixa, string lote, DateTime? vencimiento, decimal diferencia, string cdMotivo, string dsMotivo, string serializadoAtributos)
        {
            string keyAjusteStock = string.Empty;
            string idAreaAveria = null;

            var ubicacion = uow.UbicacionRepository.GetUbicacion(endereco);
            var tipoArea = uow.UbicacionAreaRepository.GetTipoAreaByUbicacion(ubicacion.Id);
            if (tipoArea != null)
                idAreaAveria = tipoArea.EsAreaAveria ? "S" : "N";

            var stock = uow.StockRepository.GetStock(empresa, codigoProducto, faixa, ubicacion.Id, lote);
            if (stock != null)
            {
                if (vencimiento < stock.Vencimiento)
                    stock.Vencimiento = vencimiento;

                stock.Cantidad += diferencia;
                stock.FechaModificacion = DateTime.Now;
                stock.NumeroTransaccion = uow.GetTransactionNumber();

                uow.StockRepository.UpdateStock(stock);

                var ajuste = new AjusteStock
                {
                    IdAreaAveria = idAreaAveria,
                    Aplicacion = this._identity.Application,
                    Funcionario = this._identity.UserId,
                    NuAjusteStock = uow.AjusteRepository.GetNextNuAjuste(),
                    Producto = codigoProducto,
                    Faixa = faixa,
                    Identificador = lote,
                    Empresa = empresa,
                    FechaRealizado = DateTime.Now,
                    TipoAjuste = TipoAjusteDb.Stock,
                    QtMovimiento = diferencia,
                    DescMotivo = dsMotivo,
                    CdMotivoAjuste = cdMotivo,
                    Ubicacion = ubicacion.Id,
                    Serializado = "",
                    FechaVencimiento = (stock == null ? vencimiento : stock.Vencimiento),
                    NuTransaccion = uow.GetTransactionNumber(),
                    Predio = ubicacion.NumeroPredio,
                    Atributos = serializadoAtributos,
                };

                int nuAjuste = uow.AjusteRepository.Add(ajuste);
                keyAjusteStock = nuAjuste.ToString();
            }
            else
            {
                throw new Exception("General_Sec0_Error_COL152");
            }

            return keyAjusteStock;
        }

        public virtual void AceptarProductoDeMasLpn(int userId, IUnitOfWork uow, LpnDetalle detalle, Agenda agenda, EtiquetaLote etiquetaActiva, LpnTipo lpnTipoLpn, AuditoriaLpn auditoria, Lpn lpn, string serializadoAtributos, out string keyAjusteStock)
        {
            string dsMotivo = "Auditoria Producto de más en el LPN";
            if (etiquetaActiva != null)
            {
                ConfirmarOperacionEtiquetaRecepcion(userId, uow, agenda, auditoria, etiquetaActiva, lpnTipoLpn.EtiquetaRecepcion, dsMotivo, serializadoAtributos, out keyAjusteStock);
            }
            else
            {
                //AjusteEtiqueta
                string cdMotivo = MotivoAjusteDb.SinRegistrar;

                Producir(uow, lpn.Ubicacion, detalle.CodigoProducto, detalle.Empresa, detalle.Faixa, detalle.Lote, auditoria.Diferencia ?? 0, cdMotivo, dsMotivo, TipoAjusteDb.Stock, this._identity.Application, userId, detalle.Vencimiento, serializadoAtributos, out keyAjusteStock);
            }

            if (agenda != null && agenda.Estado != EstadoAgenda.Cerrada && agenda.Estado != EstadoAgenda.Cancelada)
            {
                detalle.CantidadRecibida = detalle.CantidadRecibida + (auditoria.Diferencia ?? 0);
            }

            detalle.Cantidad = detalle.Cantidad + (auditoria.Diferencia ?? 0);
            detalle.NumeroTransaccion = uow.GetTransactionNumber();

            uow.ManejoLpnRepository.UpdateDetalleLpn(detalle);
            uow.SaveChanges();
        }

        public virtual void Producir(IUnitOfWork uow, string endereco, string codigoProducto, int empresa, decimal faixa, string lote, decimal diferencia, string cdMotivo, string dsMotivo, string tipoAjuste, string application, int userId, DateTime? vencimiento, string serializadoAtributos, out string keyAjusteStock)
        {
            keyAjusteStock = string.Empty;
            string idAreaAveria = null;

            var ubicacion = uow.UbicacionRepository.GetUbicacion(endereco);
            var tipoArea = uow.UbicacionAreaRepository.GetTipoAreaByUbicacion(ubicacion.Id);
            if (tipoArea != null)
                idAreaAveria = tipoArea.EsAreaAveria ? "S" : "N";

            var stock = uow.StockRepository.GetStock(empresa, codigoProducto, faixa, ubicacion.Id, lote);
            if (stock != null)
            {
                stock.Cantidad += diferencia;
                stock.FechaModificacion = DateTime.Now;
                stock.NumeroTransaccion = uow.GetTransactionNumber();

                if (vencimiento < stock.Vencimiento)
                {
                    stock.Vencimiento = vencimiento;
                }
                uow.StockRepository.UpdateStock(stock);
            }
            else
            {
                stock = new Stock()
                {
                    Ubicacion = ubicacion.Id,
                    Empresa = empresa,
                    Producto = codigoProducto,
                    Identificador = lote,
                    Faixa = faixa,
                    Cantidad = diferencia,
                    ReservaSalida = 0,
                    CantidadTransitoEntrada = 0,
                    FechaModificacion = DateTime.Now,
                    Averia = "N",
                    NumeroTransaccion = uow.GetTransactionNumber()
                };
                uow.StockRepository.AddStock(stock);
            }

            var ajuste = new AjusteStock
            {
                IdAreaAveria = idAreaAveria,
                Aplicacion = this._identity.Application,
                Funcionario = this._identity.UserId,
                NuAjusteStock = uow.AjusteRepository.GetNextNuAjuste(),
                Producto = codigoProducto,
                Faixa = faixa,
                Identificador = lote,
                Empresa = empresa,
                FechaRealizado = DateTime.Now,
                TipoAjuste = tipoAjuste,
                QtMovimiento = diferencia,
                DescMotivo = dsMotivo,
                CdMotivoAjuste = cdMotivo,
                Ubicacion = ubicacion.Id,
                Serializado = "",
                FechaVencimiento = (stock == null ? vencimiento : stock.Vencimiento),
                NuTransaccion = uow.GetTransactionNumber(),
                Predio = ubicacion.NumeroPredio,
                Atributos = serializadoAtributos
            };

            int nuAjuste = uow.AjusteRepository.Add(ajuste);
            keyAjusteStock = nuAjuste.ToString();
        }

        public virtual void ConfirmarOperacionEtiquetaRecepcion(int userId, IUnitOfWork uow, Agenda agenda, AuditoriaLpn auditoria, EtiquetaLote etiquetaActiva, string tipoEtiquetaRecepcion, string dsMotivo, string serializadoAtributos, out string keyAjusteStock)
        {
            keyAjusteStock = "";
            LAgenda logicAgenda = new LAgenda(uow, this._identity.UserId, this._identity.Application, _logger);
            if (agenda != null && agenda.Estado != EstadoAgenda.Cerrada && agenda.Estado != EstadoAgenda.Cancelada)
                logicAgenda.ProcesarMotivos(uow, agenda, auditoria.Producto, auditoria.Faixa, auditoria.Empresa, auditoria.Identificador, auditoria.Diferencia ?? 0, userId, this._identity.Application, uow.GetTransactionNumber());

            logicAgenda.ProcesarEtiqueta(agenda, auditoria.Producto, auditoria.Empresa, auditoria.Faixa, etiquetaActiva, auditoria.Identificador, auditoria.Vencimiento, dsMotivo, auditoria.Diferencia ?? 0, TiposMovimiento.Recepcion);
            logicAgenda.ProcesarControlCalidad(uow, agenda, auditoria.Producto, auditoria.Empresa, auditoria.Faixa, auditoria.Identificador, etiquetaActiva, userId, this._identity.Application);
            logicAgenda.ProcesarAltaStock(uow, agenda, auditoria.Producto, auditoria.Empresa, auditoria.Identificador, etiquetaActiva, auditoria.Faixa, auditoria.Vencimiento, auditoria.Diferencia ?? 0);

            decimal saldoLoteAuto = uow.AgendaRepository.GetSaldoAgendaDetalle(agenda.Id, auditoria.Empresa, auditoria.Producto, auditoria.Faixa, ManejoIdentificadorDb.IdentificadorAuto);


            if (agenda != null && agenda.Estado != EstadoAgenda.Cerrada && agenda.Estado != EstadoAgenda.Cancelada)
            {
                logicAgenda.ProcesarQtAgendada(uow, agenda, auditoria.Producto, auditoria.Empresa, auditoria.Faixa, auditoria.Identificador, etiquetaActiva, auditoria.Diferencia ?? 0, auditoria.Vencimiento, saldoLoteAuto, userId, this._identity.Application, uow.GetTransactionNumber());
            }
            else
            {
                string predio = uow.UbicacionRepository.GetPredio(etiquetaActiva.IdUbicacion);
                string cdMotivo = MotivoAjusteDb.SinRegistrar;

                var ajuste = new AjusteStock
                {
                    IdAreaAveria = "N",
                    Aplicacion = this._identity.Application,
                    Funcionario = this._identity.UserId,
                    NuAjusteStock = uow.AjusteRepository.GetNextNuAjuste(),
                    Producto = auditoria.Producto,
                    Faixa = auditoria.Faixa,
                    Identificador = auditoria.Identificador,
                    Empresa = auditoria.Empresa,
                    FechaRealizado = DateTime.Now,
                    TipoAjuste = TipoAjusteDb.Stock,
                    QtMovimiento = auditoria.Diferencia,
                    DescMotivo = dsMotivo,
                    CdMotivoAjuste = cdMotivo,
                    Ubicacion = etiquetaActiva.IdUbicacion,
                    Serializado = "",
                    FechaVencimiento = auditoria.Vencimiento,
                    NuTransaccion = uow.GetTransactionNumber(),
                    Predio = predio,
                    Atributos = serializadoAtributos,
                };

                int nuAjuste = uow.AjusteRepository.Add(ajuste);

                keyAjusteStock = nuAjuste.ToString();
            }
        }

        public static string GetValorByIdTipo(Atributo atributo, string valor, string separador, string formaterDateTime)
        {
            string valorReturn = "";
            switch (atributo.IdTipo.ToString())
            {
                case TipoAtributoDb.NUMERICO:

                    if (!string.IsNullOrEmpty(atributo.Separador))
                        valorReturn = valor.Replace(atributo.Separador, separador);
                    else
                        valorReturn = valor;

                    break;
                case TipoAtributoDb.FECHA:

                    if (!string.IsNullOrEmpty(valor))
                        valorReturn = DateTime.ParseExact(valor, atributo.MascaraIngreso, CultureInfo.InvariantCulture).ToString(formaterDateTime);

                    break;
                case TipoAtributoDb.HORA:
                    valorReturn = DateTime.ParseExact(valor, atributo.MascaraIngreso, CultureInfo.InvariantCulture).ToString(CDateFormats.HORA_MINUTOS);
                    break;
                case TipoAtributoDb.TEXTO:
                    valorReturn = valor;
                    break;
                case TipoAtributoDb.DOMINIO:
                    valorReturn = valor;
                    break;
                case TipoAtributoDb.SISTEMA:
                    valorReturn = "";
                    break;
            }
            return valorReturn;
        }

        public static bool ValidarEdicionAtributo(IUnitOfWork uow, GridRow row, IIdentityService identity)
        {
            var tipoLpn = row.GetCell("TP_LPN_TIPO").Value;
            var ubicacion = row.GetCell("CD_ENDERECO").Value;
            var nroLpn = long.Parse(row.GetCell("NU_LPN").Value);
            var tpAtributoAsociado = row.GetCell("TP_ATRIBUTO_ASOCIADO").Value;
            var empresa = int.Parse(row.GetCell("CD_EMPRESA").Value);

            var isDetail = false;
            var atributosCabezal = uow.ManejoLpnRepository
                .GetAllLpnAtributo(nroLpn)
                 .Select(a => new { a.Id, a.Valor })
                .OrderBy(a => a.Id);

            var serializadoCabezal = "";
            foreach (var atributoCabezal in atributosCabezal)
            {
                if (string.IsNullOrEmpty(serializadoCabezal))
                    serializadoCabezal += "{\"" + atributoCabezal.Id + "\":\"" + atributoCabezal.Valor + "\"";
                else
                    serializadoCabezal += ",\"" + atributoCabezal.Id + "\":\"" + atributoCabezal.Valor + "\"";
            }
            serializadoCabezal = serializadoCabezal + ",\"Detalles\":";
            var serializado = serializadoCabezal;
            if (tpAtributoAsociado == "D")
            {
                bool ExisteReserva = false;
                isDetail = true;
                var producto = row.GetCell("CD_PRODUTO").Value;
                var idDetalleLpn = int.Parse(row.GetCell("ID_LPN_DET").Value);
                var identificador = row.GetCell("NU_IDENTIFICADOR").Value;
                var faixa = decimal.Parse(row.GetCell("CD_FAIXA").Value, identity.GetFormatProvider());

                var Detalles = uow.ManejoLpnRepository
                    .GetAllLpnDetalleAtributo(nroLpn, idDetalleLpn, empresa, producto, identificador, faixa)
                     .GroupBy(a => new { a.IdLpnDetalle })
                    .Select(g => new
                    {
                        g.Key.IdLpnDetalle,
                        Atributos = g.Select(a => new { a.IdAtributo, a.ValorAtributo }).OrderBy(a => a.IdAtributo)
                    });

                foreach (var detalle in Detalles)
                {
                    serializado = serializadoCabezal;
                    var serializadoDetalle = "";
                    foreach (var atributoDetalle in detalle.Atributos)
                    {

                        if (string.IsNullOrEmpty(serializadoDetalle))
                            serializadoDetalle += "{\"" + atributoDetalle.IdAtributo + "\":\"" + atributoDetalle.ValorAtributo + "\"";
                        else
                            serializadoDetalle += ",\"" + atributoDetalle.IdAtributo + "\":\"" + atributoDetalle.ValorAtributo + "\"";
                    }
                    serializado += serializadoDetalle + "}}";
                    if (uow.ManejoLpnRepository.ExisteReservaAtributo(tipoLpn, ubicacion, serializado, isDetail, empresa, producto, identificador, faixa))
                    {
                        ExisteReserva = true;
                    }
                }
                return ExisteReserva;
            }
            else
            {
                return uow.ManejoLpnRepository.ExisteReservaAtributo(tipoLpn, ubicacion, serializado, isDetail, empresa);
            }
        }

        public virtual string GetSerializadoAtributos(IUnitOfWork uow, Lpn lpn, LpnDetalle detalle, List<LpnAuditoriaAtributo> atributosDetalleLpn)
        {
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
            if (atributosDetalleLpn != null && atributosDetalleLpn.Count > 0)
            {
                atributosDetalle = atributosDetalleLpn
               .Select(a => new AtributoValor()
               {
                   Nombre = atributos.FirstOrDefault(at => at.Id == a.IdAtributo)?.Nombre,
                   Valor = AtributoHelper.GetValorDisplayByIdTipo(a.ValorAtributo, atributos.FirstOrDefault(at => at.Id == a.IdAtributo), separador, formaterDateTime, true)
               })
               .ToList();
            }
            else if (detalle != null)
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

        public virtual void ValidarEdicionAtributo(IUnitOfWork uow, Lpn lpn, int empresa, string producto, string identificador, decimal faixa)
        {
            var atributosCabezal = uow.ManejoLpnRepository.GetAllLpnAtributo(lpn.NumeroLPN)
                .Select(a => new { a.Id, a.Valor })
                .OrderBy(a => a.Id);

            var serializadoCabezal = string.Empty;

            foreach (var atributoCabezal in atributosCabezal)
            {
                if (string.IsNullOrEmpty(serializadoCabezal))
                    serializadoCabezal += "{\"" + atributoCabezal.Id + "\":\"" + atributoCabezal.Valor + "\"";
                else
                    serializadoCabezal += ",\"" + atributoCabezal.Id + "\":\"" + atributoCabezal.Valor + "\"";
            }

            serializadoCabezal = serializadoCabezal + ",\"Detalles\":";

            var detalles = uow.ManejoLpnRepository.GetAllLpnDetalleAtributo(lpn.NumeroLPN, lpn.Empresa, producto, identificador, faixa)
                .GroupBy(a => new { a.IdLpnDetalle })
                .Select(g => new
                {
                    g.Key.IdLpnDetalle,
                    Atributos = g.Select(a => new { a.IdAtributo, a.ValorAtributo }).OrderBy(a => a.ValorAtributo)
                });

            var serializado = string.Empty;
            foreach (var detalle in detalles)
            {
                serializado = serializadoCabezal;
                var serializadoDetalle = string.Empty;

                foreach (var atributoDetalle in detalle.Atributos)
                {

                    if (string.IsNullOrEmpty(serializadoDetalle))
                        serializadoDetalle += "{\"" + atributoDetalle.IdAtributo + "\":\"" + atributoDetalle.ValorAtributo + "\"";
                    else
                        serializadoDetalle += ",\"" + atributoDetalle.IdAtributo + "\":\"" + atributoDetalle.ValorAtributo + "\"";
                }

                serializado += serializadoDetalle + "}}";

                if (uow.ManejoLpnRepository.ExisteReservaAtributo(lpn.Tipo, lpn.Ubicacion, empresa, producto, identificador, faixa, serializado))
                    throw new Exception("STO700_Sec0_Error_AtributoConReserva");
            }
        }

    }
}
