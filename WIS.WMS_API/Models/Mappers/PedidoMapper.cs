using System.Collections.Generic;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Domain.Picking;
using WIS.Domain.StockEntities;
using WIS.Extension;
using WIS.WMS_API.Models.Mappers.Interfaces;

namespace WIS.WMS_API.Models.Mappers
{
    public class PedidoMapper : Mapper, IPedidoMapper
    {
        public PedidoMapper()
        {
        }

        #region Create

        public virtual List<Pedido> Map(PedidosRequest request)
        {
            var models = new List<Pedido>();

            var manual = (!string.IsNullOrEmpty(request.Archivo) && request.Archivo == "INT050") ? "S" : "N";

            foreach (var p in request.Pedidos)
            {
                var model = new Pedido(manual, p.TipoExpedicion, "N", p.CodigoAgente, p.TipoAgente);
                model.Agrupacion = p.Agrupacion;
                model.Estado = SituacionDb.PedidoAbierto;
                model.Anexo = p.Anexo1;
                model.Anexo2 = p.Anexo2;
                model.Anexo3 = p.Anexo3;
                model.Anexo4 = p.Anexo4;
                model.CodigoTransportadora = p.CodigoTransportadora;
                model.ComparteContenedorEntrega = p.ComparteContenedorEntrega;
                model.ComparteContenedorPicking = p.ComparteContenedorPicking;
                model.CondicionLiberacion = p.CondicionLiberacion;
                model.DireccionEntrega = p.Direccion;
                model.Empresa = request.Empresa;
                model.FechaEmision = p.FechaEmision;
                model.FechaEntrega = p.FechaEntrega;
                model.FechaGenerica_1 = p.FechaGenerica;
                model.FechaLiberarDesde = p.FechaLiberarDesde;
                model.FechaLiberarHasta = p.FechaLiberarHasta;
                model.Id = p.NroPedido.Trim();
                model.Lineas = MapLineas(p, request.Empresa);
                model.Memo = p.Memo;
                model.Memo1 = p.Memo1;
                model.NuGenerico_1 = p.NuGenerico;
                model.OrdenEntrega = p.OrdenEntrega;
                model.Origen = (manual == "S" ? "INT050" : "API");
                model.Predio = p.Predio;
                model.PuntoEntrega = p.PuntoEntrega;
                model.Ruta = p.Ruta;
                model.Tipo = p.TipoPedido;
                model.VlGenerico_1 = p.DsGenerico;
                model.VlSerealizado_1 = p.Serializado;
                model.Zona = p.Zona;
                model.Telefono = p.Telefono;
                model.TelefonoSecundario = p.TelefonoSecundario;
                model.Latitud = p.Latitud;
                model.Longitud = p.Longitud;
                model.Actividad = EstadoPedidoDb.Activo;
                model.Lpns = MapLineasLpn(p, request.Empresa);

                models.Add(model);
            }

            return models;
        }

        public virtual List<Lpn> MapLineasLpn(PedidoRequest request, int empresa)
        {
            var models = new List<Lpn>();

            foreach (var d in request.Lpns)
            {
                models.Add(MapLpn(d, empresa));
            }

            return models;
        }

        public virtual Lpn MapLpn(PedidoLpnRequest request, int empresa)
        {
            return new Lpn()
            {
                IdExterno = request.IdExterno,
                Tipo = request.Tipo,
                Empresa = empresa,
            };
        }

        public virtual List<DetallePedido> MapLineas(PedidoRequest request, int empresa)
        {
            var models = new List<DetallePedido>();

            foreach (var d in request.Detalles)
            {
                models.Add(MapDetalle(d, empresa, request.Agrupacion, request.NroPedido));
            }

            return models;
        }

        public virtual DetallePedido MapDetalle(DetallePedidoRequest request, int empresa, string agrupacion, string pedido)
        {
            var identificador = request.Identificador?.Trim()?.ToUpper() ?? string.Empty;
            var especificaIdentificador = true;

            if (string.IsNullOrEmpty(identificador) || identificador == ManejoIdentificadorDb.IdentificadorAuto)
                especificaIdentificador = false;

            var model = new DetallePedido(MapBooleanToString(especificaIdentificador));

            model.Agrupacion = agrupacion;
            model.Cantidad = request.Cantidad;
            model.CantidadAnulada = 0;
            model.CantidadLiberada = 0;
            model.CantidadOriginal = request.Cantidad;
            model.DatosSerializados = request.Serializado;
            model.Empresa = empresa;
            model.Faixa = 1;
            model.FechaGenerica_1 = request.FechaGenerica;
            model.Id = pedido.Trim();
            model.Identificador = identificador?.Trim()?.ToUpper();
            model.Memo = request.Memo;
            model.NuGenerico_1 = request.NuGenerico;
            model.Producto = request.CodigoProducto;
            model.VlGenerico_1 = request.DsGenerico;
            model.Duplicados = MapDuplicados(request, empresa, agrupacion, pedido);
            model.DetallesLpn = MapDetallesLpn(request, empresa, pedido);
            model.Atributos = MapAtributos(request, pedido, empresa, identificador, especificaIdentificador);

            return model;
        }

        public virtual List<DetallePedidoAtributos> MapAtributos(DetallePedidoRequest request, string pedido, int empresa, string identificador, bool especificaIdentificador)
        {
            var models = new List<DetallePedidoAtributos>();

            if (request.Atributos != null)
            {
                foreach (var a in request.Atributos)
                {
                    models.Add(new DetallePedidoAtributos()
                    {
                        Pedido = pedido,
                        Empresa = empresa,
                        Producto = request.CodigoProducto,
                        Faixa = 1,
                        Identificador = identificador,
                        IdEspecificaIdentificador = MapBooleanToString(especificaIdentificador),
                        CantidadPedida = a.Cantidad,
                        CantidadLiberada = 0,
                        CantidadAnulada = 0,
                        Atributos = MapAtributos(a.Atributos),
                    });
                }
            }

            return models;
        }

        public virtual List<DetallePedidoAtributosLpn> MapAtributos(ModificarDetallePedidoLpnRequest request, string pedido, int empresa, string producto, string identificador, bool especificaIdentificador)
        {
            var models = new List<DetallePedidoAtributosLpn>();

            if (request.Atributos != null)
            {
                foreach (var a in request.Atributos)
                {
                    models.Add(new DetallePedidoAtributosLpn()
                    {
                        Pedido = pedido,
                        Empresa = empresa,
                        Producto = producto,
                        Faixa = 1,
                        Identificador = identificador,
                        IdEspecificaIdentificador = MapBooleanToString(especificaIdentificador),
                        CantidadPedida = a.Cantidad,
                        IdLpnExterno = request.IdLpnExterno,
                        Tipo = request.TipoLpn,
                        Atributos = MapAtributos(a.Atributos),
                    });
                }
            }

            return models;
        }

        public virtual List<DetallePedidoAtributosLpn> MapAtributos(DetallePedidoLpnRequest request, string pedido, int empresa, string producto, string identificador, bool especificaIdentificador)
        {
            var models = new List<DetallePedidoAtributosLpn>();

            if (request.Atributos != null)
            {
                foreach (var a in request.Atributos)
                {
                    models.Add(new DetallePedidoAtributosLpn()
                    {
                        Pedido = pedido,
                        Empresa = empresa,
                        Producto = producto,
                        Faixa = 1,
                        Identificador = identificador,
                        IdEspecificaIdentificador = MapBooleanToString(especificaIdentificador),
                        IdLpnExterno = request.IdLpnExterno,
                        Tipo = request.TipoLpn,
                        CantidadPedida = a.Cantidad,
                        Atributos = MapAtributos(a.Atributos),
                    });
                }
            }

            return models;
        }

        public virtual List<DetallePedidoConfigAtributo> MapAtributos(List<DetallePedidoAtributoRequest> request)
        {
            var model = new List<DetallePedidoConfigAtributo>();

            if (request != null)
            {
                foreach (var a in request)
                {
                    model.Add(new DetallePedidoConfigAtributo
                    {
                        IdCabezal = MapBooleanToString(a.Tipo?.ToUpper() == TipoAtributoDb.CABEZAL),
                        Nombre = a.Nombre,
                        Valor = a.Valor,
                        Tipo = a.Tipo?.ToUpper(),
                    });
                }
            }

            return model;
        }

        public virtual List<DetallePedidoConfigAtributo> MapAtributos(List<ModificarDetallePedidoLpnAtributoRequest> request)
        {
            var model = new List<DetallePedidoConfigAtributo>();

            if (request != null)
            {
                foreach (var a in request)
                {
                    model.Add(new DetallePedidoConfigAtributo
                    {
                        IdCabezal = MapBooleanToString(false),
                        Nombre = a.Nombre,
                        Valor = a.Valor,
                        Tipo = TipoAtributoDb.DETALLE,
                    });
                }
            }

            return model;
        }

        public virtual List<DetallePedidoConfigAtributo> MapAtributos(List<DetallePedidoLpnAtributoRequest> request)
        {
            var model = new List<DetallePedidoConfigAtributo>();

            if (request != null)
            {
                foreach (var a in request)
                {
                    model.Add(new DetallePedidoConfigAtributo
                    {
                        IdCabezal = MapBooleanToString(false),
                        Nombre = a.Nombre,
                        Valor = a.Valor,
                        Tipo = TipoAtributoDb.DETALLE,
                    });
                }
            }

            return model;
        }

        public virtual List<DetallePedidoLpn> MapDetallesLpn(DetallePedidoRequest request, int empresa, string pedido)
        {
            var identificador = request.Identificador?.Trim()?.ToUpper() ?? string.Empty;
            var especificaIdentificador = true;

            if (string.IsNullOrEmpty(identificador) || identificador == ManejoIdentificadorDb.IdentificadorAuto)
                especificaIdentificador = false;

            var models = new List<DetallePedidoLpn>();

            if (request.DetallesLpn != null)
            {
                foreach (var d in request.DetallesLpn)
                {
                    models.Add(new DetallePedidoLpn()
                    {
                        CantidadPedida = d.Cantidad,
                        Empresa = empresa,
                        Faixa = 1,
                        Identificador = identificador,
                        IdEspecificaIdentificador = MapBooleanToString(especificaIdentificador),
                        Pedido = pedido.Trim(),
                        Producto = request.CodigoProducto,
                        Tipo = d.TipoLpn ?? string.Empty,
                        IdLpnExterno = d.IdLpnExterno,
                        CantidadLiberada = 0,
                        CantidadAnulada = 0,
                        Atributos = MapAtributos(d, pedido, empresa, request.CodigoProducto, identificador, especificaIdentificador)
                    });
                }
            }

            return models;
        }

        public virtual List<DetallePedidoDuplicado> MapDuplicados(DetallePedidoRequest request, int empresa, string agrupacion, string nroPedido)
        {
            var models = new List<DetallePedidoDuplicado>();
            var identificador = request.Identificador?.Trim()?.ToUpper() ?? string.Empty;

            if (request.Duplicados != null)
            {
                foreach (var d in request.Duplicados)
                {
                    models.Add(new DetallePedidoDuplicado()
                    {
                        CantidadPedida = d.Cantidad,
                        Empresa = empresa,
                        IdEspecificaIdentificador = MapBooleanToString(identificador != ManejoIdentificadorDb.IdentificadorAuto),
                        Faixa = 1,
                        Identificador = identificador,
                        IdLineaSistemaExterno = d.IdLineaSistemaExterno ?? string.Empty,
                        Pedido = nroPedido.Trim(),
                        Producto = request.CodigoProducto,
                        TipoLinea = d.TipoLinea ?? string.Empty,
                        DatosSerializados = d.Serializado ?? string.Empty
                    });
                }
            }

            return models;
        }

        #endregion

        #region Update

        public virtual List<Pedido> Map(ModificarPedidosRequest request)
        {
            var models = new List<Pedido>();

            foreach (var p in request.Pedidos)
            {
                var model = new Pedido("N", p.TipoExpedicion, "N", p.CodigoAgente, p.TipoAgente);
                model.Agrupacion = p.Agrupacion;
                model.Estado = SituacionDb.PedidoAbierto;
                model.Anexo = p.Anexo1;
                model.Anexo2 = p.Anexo2;
                model.Anexo3 = p.Anexo3;
                model.Anexo4 = p.Anexo4;
                model.CodigoTransportadora = p.CodigoTransportadora;
                model.ComparteContenedorEntrega = p.ComparteContenedorEntrega;
                model.ComparteContenedorPicking = p.ComparteContenedorPicking;
                model.CondicionLiberacion = p.CondicionLiberacion;
                model.DireccionEntrega = p.Direccion;
                model.Empresa = request.Empresa;
                model.FechaEmision = p.FechaEmision;
                model.FechaEntrega = p.FechaEntrega;
                model.FechaGenerica_1 = p.FechaGenerica;
                model.FechaLiberarDesde = p.FechaLiberarDesde;
                model.FechaLiberarHasta = p.FechaLiberarHasta;
                model.Id = p.NroPedido.Trim();
                model.Lineas = MapLineas(p, request.Empresa);
                model.Memo = p.Memo;
                model.Memo1 = p.Memo1;
                model.NuGenerico_1 = p.NuGenerico;
                model.OrdenEntrega = p.OrdenEntrega;
                model.Origen = "API";
                model.Predio = p.Predio;
                model.PuntoEntrega = p.PuntoEntrega;
                model.Ruta = p.Ruta;
                model.Tipo = p.TipoPedido;
                model.VlGenerico_1 = p.DsGenerico;
                model.VlSerealizado_1 = p.Serializado;
                model.Zona = p.Zona;
                model.Telefono = p.Telefono;
                model.TelefonoSecundario = p.TelefonoSecundario;
                model.Longitud = p.Longitud;
                model.Latitud = p.Latitud;
                model.Lpns = MapLineasLpn(p, request.Empresa);
                models.Add(model);
            }

            return models;
        }

        public virtual List<Lpn> MapLineasLpn(ModificarPedidoRequest request, int empresa)
        {
            var models = new List<Lpn>();

            foreach (var d in request.Lpns)
            {
                models.Add(MapLpn(d, empresa));
            }

            return models;
        }

        public virtual Lpn MapLpn(ModificarPedidoLpnRequest request, int empresa)
        {
            return new Lpn()
            {
                IdExterno = request.IdExterno,
                Tipo = request.Tipo,
                Empresa = empresa,
            };
        }

        public virtual List<DetallePedido> MapLineas(ModificarPedidoRequest request, int empresa)
        {
            var models = new List<DetallePedido>();

            foreach (var d in request.Detalles)
            {
                models.Add(MapDetalle(d, empresa, request.Agrupacion, request.NroPedido));
            }

            return models;
        }

        public virtual DetallePedido MapDetalle(ModificarDetallePedidoRequest request, int empresa, string agrupacion, string pedido)
        {
            var identificador = request.Identificador?.Trim()?.ToUpper() ?? string.Empty;
            var especificaIdentificador = true;

            if (string.IsNullOrEmpty(identificador) || identificador == ManejoIdentificadorDb.IdentificadorAuto)
                especificaIdentificador = false;

            var model = new DetallePedido(MapBooleanToString(especificaIdentificador));

            model.Agrupacion = agrupacion;
            model.Cantidad = request.Cantidad;
            model.CantidadAnulada = 0;
            model.CantidadLiberada = 0;
            model.CantidadOriginal = request.Cantidad;
            model.DatosSerializados = request.Serializado;
            model.Empresa = empresa;
            model.Faixa = 1;
            model.FechaGenerica_1 = request.FechaGenerica;
            model.Id = pedido.Trim();
            model.Identificador = identificador?.Trim()?.ToUpper();
            model.Memo = request.Memo;
            model.NuGenerico_1 = request.NuGenerico;
            model.Producto = request.CodigoProducto;
            model.VlGenerico_1 = request.DsGenerico;
            model.Duplicados = MapDuplicados(request, empresa, agrupacion, pedido);
            model.DetallesLpn = MapDetallesLpn(request, empresa, pedido);
            model.Atributos = MapAtributos(request, pedido, empresa, identificador, especificaIdentificador);

            return model;
        }

        public virtual List<DetallePedidoAtributos> MapAtributos(ModificarDetallePedidoRequest request, string pedido, int empresa, string identificador, bool especificaIdentificador)
        {
            var models = new List<DetallePedidoAtributos>();

            if (request.Atributos != null)
            {
                foreach (var a in request.Atributos)
                {
                    models.Add(new DetallePedidoAtributos()
                    {
                        Pedido = pedido,
                        Empresa = empresa,
                        Producto = request.CodigoProducto,
                        Faixa = 1,
                        Identificador = identificador,
                        IdEspecificaIdentificador = MapBooleanToString(especificaIdentificador),
                        CantidadPedida = a.Cantidad,
                        Atributos = MapAtributos(a.Atributos),
                    });
                }
            }

            return models;
        }

        public virtual List<DetallePedidoConfigAtributo> MapAtributos(List<ModificarDetallePedidoAtributoRequest> request)
        {
            var model = new List<DetallePedidoConfigAtributo>();

            if (request != null)
            {
                foreach (var a in request)
                {
                    model.Add(new DetallePedidoConfigAtributo
                    {
                        IdCabezal = MapBooleanToString(a.Tipo?.ToString() == TipoAtributoDb.CABEZAL),
                        Nombre = a.Nombre,
                        Valor = a.Valor,
                        Tipo = a.Tipo?.ToUpper(),
                    });
                }
            }

            return model;
        }

        public virtual List<DetallePedidoLpn> MapDetallesLpn(ModificarDetallePedidoRequest request, int empresa, string pedido)
        {
            var identificador = request.Identificador?.Trim()?.ToUpper() ?? string.Empty;
            var especificaIdentificador = true;

            if (string.IsNullOrEmpty(identificador) || identificador == ManejoIdentificadorDb.IdentificadorAuto)
                especificaIdentificador = false;

            var models = new List<DetallePedidoLpn>();

            if (request.DetallesLpn != null)
            {
                foreach (var d in request.DetallesLpn)
                {
                    models.Add(new DetallePedidoLpn()
                    {
                        CantidadPedida = d.Cantidad,
                        Empresa = empresa,
                        Faixa = 1,
                        Identificador = identificador,
                        IdEspecificaIdentificador = MapBooleanToString(especificaIdentificador),
                        Pedido = pedido.Trim(),
                        Producto = request.CodigoProducto,
                        Tipo = d.TipoLpn ?? string.Empty,
                        IdLpnExterno = d.IdLpnExterno,
                        CantidadLiberada = 0,
                        CantidadAnulada = 0,
                        Atributos = MapAtributos(d, pedido, empresa, request.CodigoProducto, identificador, especificaIdentificador)
                    });
                }
            }

            return models;
        }

        public virtual List<DetallePedidoDuplicado> MapDuplicados(ModificarDetallePedidoRequest request, int empresa, string agrupacion, string nroPedido)
        {
            var models = new List<DetallePedidoDuplicado>();
            var identificador = request.Identificador?.Trim()?.ToUpper() ?? string.Empty;
            var especificaIdentificador = true;

            if (string.IsNullOrEmpty(identificador) || identificador == ManejoIdentificadorDb.IdentificadorAuto)
                especificaIdentificador = false;

            foreach (var d in request.Duplicados)
            {
                models.Add(new DetallePedidoDuplicado()
                {
                    CantidadPedida = d.Cantidad,
                    Empresa = empresa,
                    IdEspecificaIdentificador = MapBooleanToString(especificaIdentificador),
                    Faixa = 1,
                    Identificador = identificador,
                    IdLineaSistemaExterno = d.IdLineaSistemaExterno ?? string.Empty,
                    Pedido = nroPedido.Trim(),
                    Producto = request.CodigoProducto,
                    TipoLinea = d.TipoLinea ?? string.Empty,
                    DatosSerializados = d.Serializado ?? string.Empty
                });
            }

            return models;
        }

        #endregion

        #region GetPedido

        public virtual PedidoResponse MapToResponse(Pedido pedido, string tipoAgente, string codigoAgente)
        {
            var pedResponde = new PedidoResponse()
            {
                NroPedido = pedido.Id,
                Empresa = pedido.Empresa,
                TipoAgente = tipoAgente,
                CodigoAgente = codigoAgente,
                CondicionLiberacion = pedido.CondicionLiberacion,
                FuncionarioResponsable = pedido.FuncionarioResponsable,
                Origen = pedido.Origen,
                PuntoEntrega = pedido.PuntoEntrega,
                Ruta = pedido.Ruta,
                Situacion = pedido.Estado,
                CodigoTransportadora = pedido.CodigoTransportadora,
                CodigoUF = pedido.CodigoUF,
                Zona = pedido.Zona,
                Anexo = pedido.Anexo,
                Anexo2 = pedido.Anexo2,
                Anexo3 = pedido.Anexo3,
                Anexo4 = pedido.Anexo4,
                DireccionEntrega = pedido.DireccionEntrega,
                Memo = pedido.Memo,
                Memo1 = pedido.Memo1,
                FechaAlta = pedido.FechaAlta.ToString(CDateFormats.DATE_ONLY),
                FechaEmision = pedido.FechaEmision.ToString(CDateFormats.DATE_ONLY),
                FechaEntrega = pedido.FechaEntrega.ToString(CDateFormats.DATE_ONLY),
                FechaFuncionarioResponsable = pedido.FechaFuncionarioResponsable.ToString(CDateFormats.DATE_ONLY),
                FechaGenerica = pedido.FechaGenerica_1.ToString(CDateFormats.DATE_ONLY),
                FechaLiberarDesde = pedido.FechaLiberarDesde.ToString(CDateFormats.DATE_ONLY),
                FechaLiberarHasta = pedido.FechaLiberarHasta.ToString(CDateFormats.DATE_ONLY),
                FechaUltimaPreparacion = pedido.FechaUltimaPreparacion.ToString(CDateFormats.DATE_ONLY),
                FechaModificacion = pedido.FechaModificacion.ToString(CDateFormats.DATE_ONLY),
                SincronizacionRealizada = MapBooleanToString(pedido.IsSincronizacionRealizada),
                Agrupacion = pedido.Agrupacion,
                IdManual = MapBooleanToString(pedido.IsManual),
                Actividad = pedido.Actividad,
                NuGenerico = pedido.NuGenerico_1,
                NroIntzFacturacion = pedido.NroIntzFacturacion,
                OrdenEntrega = pedido.OrdenEntrega,
                NumeroOrdenLiberacion = pedido.NumeroOrdenLiberacion,
                IngresoProduccion = pedido.IngresoProduccion,
                Predio = pedido.Predio,
                NroPrepManual = pedido.NroPrepManual,
                PreparacionProgramada = pedido.PreparacionProgramada,
                Transaccion = pedido.Transaccion,
                NumeroUltimaPreparacion = pedido.NumeroUltimaPreparacion,
                TipoExpedicion = pedido.ConfiguracionExpedicion.Tipo,
                TipoPedido = pedido.Tipo,
                ComparteContenedorEntrega = pedido.ComparteContenedorEntrega,
                ComparteContenedorPicking = pedido.ComparteContenedorPicking,
                DsGenerico = pedido.VlGenerico_1,
                Serealizado = pedido.VlSerealizado_1,
                Telefono = pedido.Telefono,
                TelefonoSecundario = pedido.TelefonoSecundario,
                Latitud = pedido.Latitud,
                Longitud = pedido.Longitud,
            };

            foreach (var detalle in pedido.Lineas)
            {
                pedResponde.Detalles.Add(MapDetalleToResponse(detalle));
            }

            return pedResponde;
        }

        public virtual PedidoDetalleResponse MapDetalleToResponse(DetallePedido det)
        {
            if (det == null)
                return null;

            var detalle = new PedidoDetalleResponse
            {
                Faixa = det.Faixa,
                Producto = det.Producto,
                Identificador = det.Identificador?.Trim()?.ToUpper(),
                Cantidad = det.Cantidad,
                Memo = det.Memo,
                FechaAlta = det.FechaAlta.ToString(CDateFormats.DATE_ONLY),
                FechaGenerica = det.FechaGenerica_1.ToString(CDateFormats.DATE_ONLY),
                FechaModificacion = det.FechaModificacion.ToString(CDateFormats.DATE_ONLY),
                Agrupacion = det.Agrupacion,
                EspecificaIdentificador = MapBooleanToString(det.EspecificaIdentificador),
                NuGenerico = det.NuGenerico_1,
                Transaccion = det.Transaccion,
                CantidadAbastecida = det.CantidadAbastecida,
                CantidadAnulada = det.CantidadAnulada,
                CantidadAnuladaFactura = det.CantidadAnuladaFactura,
                CantidadCargada = det.CantidadCargada,
                CantidadControlada = det.CantidadControlada,
                CantidadCrossDocking = det.CantidadCrossDocking,
                CantidadExpedida = det.CantidadExpedida,
                CantidadFacturada = det.CantidadFacturada,
                CantidadLiberada = det.CantidadLiberada,
                CantidadOriginal = det.CantidadOriginal,
                CantidadPreparada = det.CantidadPreparada,
                CantidadTransferida = det.CantidadTransferida,
                CantUndAsociadoCamion = det.CantUndAsociadoCamion,
                DsGenerico = det.VlGenerico_1,
                PorcentajeTolerancia = det.PorcentajeTolerancia,
                Serializado = det.DatosSerializados
            };

            foreach (var duplicado in det.Duplicados)
            {
                detalle.Duplicados.Add(MapDuplicadoToResponse(duplicado));
            }

            return detalle;
        }

        public virtual PedidoDetalleDuplicadoResponse MapDuplicadoToResponse(DetallePedidoDuplicado dup)
        {
            return new PedidoDetalleDuplicadoResponse()
            {
                CantidadAnulada = dup.CantidadAnulada,
                CantidadExpedida = dup.CantidadExpedida,
                CantidadFacturada = dup.CantidadFacturada,
                CantidadPedida = dup.CantidadPedida,
                IdLineaSistemaExterno = dup.IdLineaSistemaExterno,
                Serializado = dup.DatosSerializados,
                TipoLinea = dup.TipoLinea,
            };
        }

        #endregion
    }
}
