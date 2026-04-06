using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Documento;
using WIS.Domain.General;
using WIS.Domain.ManejoStock.Constants;
using WIS.Domain.Picking;
using WIS.Exceptions;
using WIS.Security;

namespace WIS.Domain.Recepcion.RecepcionAgendamiento
{
    public class TransferirEtiquetasAgenda
    {
        public Logger _logger;
        public IIdentityService _identity;

        public TransferirEtiquetasAgenda(Logger logger, IIdentityService identity)
        {
            _logger = logger;
            _identity = identity;
        }

        public virtual void ProcesarTransferenciaEtiquetas(IUnitOfWork uow, Agenda agenda)
        {
            var etiquetas = uow.CrossDockingRepository.GetListaDeEtiquetasAConvertir(agenda.Id);

            if (etiquetas == null || etiquetas.Count() == 0)
                return;

            ProcesarTransferenciaEtiquetas(uow, etiquetas, agenda);
        }

        public virtual void ProcesarTransferenciaEtiquetas(IUnitOfWork uow, List<EtiquetaPreSep> etiquetas, Agenda agenda)
        {
            _logger.Trace($"Convertir etiquetas a contenedor Agenda: {agenda.Id}");

            CrossDockingAgenda crossDockingAgenda = new CrossDockingAgenda(uow, agenda);

            var detallesPicking = new List<DetallePreparacion>();
            foreach (var etiqueta in etiquetas)
            {
                _logger.Trace($"Nro Etiqueta Lote: {etiqueta.NU_ETIQUETA_LOTE}");
                _logger.Trace($"Nro Etiqueta externo: {etiqueta.NU_EXTERNO_ETIQUETA}");
                _logger.Trace($"Cliente: {etiqueta.CD_CLIENTE}");
                _logger.Trace($"Empresa: {etiqueta.CD_EMPRESA}");
                _logger.Trace($"Ubicación: {etiqueta.CD_ENDERECO}");
                _logger.Trace($"Cod. Situación: {etiqueta.CD_SITUACAO}");
                _logger.Trace($"Qt. Producto: {etiqueta.QT_PRODUTO}");

                var nuevasLineasPicking = TransferirEtiqueta(uow, etiqueta.NU_ETIQUETA_LOTE, etiqueta.NU_EXTERNO_ETIQUETA, etiqueta.CD_CLIENTE, agenda.Id,
                       agenda.IdEmpresa, etiqueta.CD_ENDERECO, this._identity.UserId, this._identity.Application);

                detallesPicking.AddRange(nuevasLineasPicking);
            }

            uow.SaveChanges();
            _logger.Trace($"Post procesar etiquetas");

            var nuPreparacion = uow.CrossDockingRepository.GetCrossDockingActivoByAgenda(agenda.Id).Preparacion;

            crossDockingAgenda.DesatenderPedidosCrossDocking(true);
            _logger.Trace($"Post DesatenderPedidosCrossDocking");
            uow.SaveChanges();

            var pedidos = uow.PedidoRepository.GetPedidosPreparacionProgramada(nuPreparacion);
            var crossDock = new CrossDocking();
            crossDock.RemovePedidos(uow, pedidos);
            uow.SaveChanges();
            _logger.Trace($"Post DesbloquearLiberacion de pedidos");

            ProcesarIntegracionDocumental(uow, agenda.Id, agenda.IdEmpresa, detallesPicking);

            uow.SaveChanges();

            _logger.Trace($"Post SaveChanges");
        }

        public virtual List<DetallePreparacion> TransferirEtiqueta(IUnitOfWork uow, int nroEtiqueta, string nroExternoEtiqueta, string cdCliente, int nroAgenda, int empresa, string ubicacion, int idUsuario, string aplicacion)
        {
            _logger.Trace("Comienzo función transferirEtiqueta");

            var detallesPreparacion = new List<DetallePreparacion>();
            var nuTransaccion = uow.GetTransactionNumber();

            ICrossDocking crossDock = uow.CrossDockingRepository.GetCrossDockingActivoByAgendaTipo(nroAgenda, TipoCrossDockingDb.UnaFase);

            decimal cantDisponible = 0;

            var etiquetaLote = uow.EtiquetaLoteRepository.GetEtiquetaLote(nroEtiqueta);
            List<EtiquetaLoteDetalle> detalles = uow.CrossDockingRepository.GetDetalleEtiqueta(nroEtiqueta);

            var contenedor = uow.ContenedorRepository.GetContenedorByCodigoBarras(etiquetaLote.CodigoBarras);

            DateTime fechaActual = DateTime.Now;
            foreach (var lineaEtiqueta in detalles)
            {
                cantDisponible = lineaEtiqueta.Cantidad ?? 0;

                _logger.Trace($"Nro Etiqueta Lote: {lineaEtiqueta.IdEtiquetaLote}");
                _logger.Trace($"Cod. Producto: {lineaEtiqueta.CodigoProducto}");
                _logger.Trace($"Identificador: {lineaEtiqueta.Identificador}");
                _logger.Trace($"Empresa: {lineaEtiqueta.IdEmpresa}");
                _logger.Trace($"Cantidad producto: {cantDisponible}");
                _logger.Trace($"Cantidad producto recibido: {lineaEtiqueta.CantidadRecibida}");

                int countVueltas = 0;
                while (cantDisponible > 0)
                {
                    _logger.Trace($"While vuelta : {countVueltas}");
                    _logger.Trace($"cantDisponible : {cantDisponible}");

                    long? nroCarga;
                    string nroPedido;
                    string especificaId;
                    decimal? cantPedido;
                    this.RetornarSiguientePedidoCargaPrep(uow, nroAgenda, empresa, cdCliente, lineaEtiqueta.CodigoProducto, lineaEtiqueta.Faixa,
                                  lineaEtiqueta.Identificador, crossDock.Preparacion, cantDisponible, out nroCarga, out nroPedido, out especificaId, out cantPedido);

                    _logger.Trace($"cantPedido : {cantPedido}");
                    _logger.Trace($"cantDisponible : {cantDisponible}");

                    if (cantPedido > cantDisponible)
                        cantPedido = cantDisponible;


                    if (uow.CrossDockingRepository.CargaFacturada(cdCliente, empresa, nroCarga) || uow.CrossDockingRepository.PermiteEditarCargaDocumental(cdCliente, empresa, nroCarga))
                    {
                        Pedido ped = uow.PedidoRepository.GetPedido(empresa, cdCliente, nroPedido);

                        if (ped.NuCarga != null)
                            nroCarga = ped.NuCarga;
                        else
                        {
                            _logger.Trace($"Pedido : {ped.Id}");
                            _logger.Trace($"Cliente : {ped.Cliente}");
                            _logger.Trace($"Empresa : {ped.Empresa}");
                            var carga = new Carga
                            {
                                Descripcion = "Carga creada para cross-docking",
                                Preparacion = crossDock.Preparacion,
                                Ruta = (short)ped.Ruta,
                                FechaAlta = DateTime.Now
                            };
                            uow.CargaRepository.AddCarga(carga);
                            _logger.Trace($"Agrego carga : {carga.Id}");

                            Modificar_Cross_Dock(uow, nroAgenda, cdCliente, nroPedido, crossDock.Preparacion, nroCarga, carga.Id, lineaEtiqueta.CodigoProducto, lineaEtiqueta.Faixa, empresa, lineaEtiqueta.Identificador, especificaId, cantPedido, idUsuario);
                            nroCarga = carga.Id;
                        }
                    }

                    _logger.Trace($"Pre create DetallePreparacion");
                    DetallePreparacion nuevaLineaPicking = new DetallePreparacion();
                    nuevaLineaPicking.NumeroPreparacion = crossDock.Preparacion;
                    nuevaLineaPicking.NumeroSecuencia = -1;
                    nuevaLineaPicking.Empresa = empresa;
                    nuevaLineaPicking.Producto = lineaEtiqueta.CodigoProducto;
                    nuevaLineaPicking.Faixa = lineaEtiqueta.Faixa;
                    nuevaLineaPicking.Lote = lineaEtiqueta.Identificador;
                    nuevaLineaPicking.Contenedor = contenedor;
                    nuevaLineaPicking.Carga = nroCarga;
                    nuevaLineaPicking.Ubicacion = ubicacion;
                    nuevaLineaPicking.Cliente = cdCliente;
                    nuevaLineaPicking.Pedido = nroPedido;
                    nuevaLineaPicking.Usuario = idUsuario;
                    nuevaLineaPicking.Cantidad = cantPedido ?? 0;
                    nuevaLineaPicking.CantidadPreparada = cantPedido;
                    nuevaLineaPicking.EspecificaLote = especificaId;
                    nuevaLineaPicking.Agrupacion = Agrupacion.Pedido;
                    nuevaLineaPicking.FechaAlta = DateTime.Now;
                    nuevaLineaPicking.FechaPickeo = lineaEtiqueta.Modificacion ?? fechaActual;
                    nuevaLineaPicking.CantidadPickeo = cantPedido ?? 0;
                    nuevaLineaPicking.NumeroContenedorPickeo = contenedor.Numero;
                    nuevaLineaPicking.Estado = EstadoDetallePreparacion.ESTADO_PREPARADO;
                    nuevaLineaPicking.Transaccion = nuTransaccion;
                    nuevaLineaPicking.UsuarioPickeo = idUsuario;
                    nuevaLineaPicking.VencimientoPickeo = lineaEtiqueta.Vencimiento;

                    detallesPreparacion.Add(nuevaLineaPicking);
                    uow.PreparacionRepository.AddDetallePreparacion(nuevaLineaPicking);
                    uow.SaveChanges();

                    _logger.Trace($"Post create DetallePreparacion");

                    _logger.Trace($"***NuevaLineaPicking***");
                    _logger.Trace($"NumeroPreparacion: {nuevaLineaPicking.NumeroPreparacion}");
                    _logger.Trace($"Producto: {nuevaLineaPicking.Producto}");
                    _logger.Trace($"Lote: {nuevaLineaPicking.Lote}");
                    _logger.Trace($"Carga: {nuevaLineaPicking.Carga}");
                    _logger.Trace($"Cantidad: {nuevaLineaPicking.Cantidad}");
                    _logger.Trace($"CantidadPreparada: {nuevaLineaPicking.CantidadPreparada}");
                    _logger.Trace($"CantidadPickeo: {nuevaLineaPicking.CantidadPickeo}");
                    _logger.Trace($"Transaccion: {nuevaLineaPicking.Transaccion}");

                    LineaCrossDocking lineaCrossDocking = uow.CrossDockingRepository.GetLineaCrossDocking(
                            crossDock.Agenda,
                            nuevaLineaPicking.NumeroPreparacion,
                            nuevaLineaPicking.Cliente,
                            nuevaLineaPicking.Producto,
                            nuevaLineaPicking.Pedido,
                            nuevaLineaPicking.Faixa,
                            nuevaLineaPicking.Lote,
                            nuevaLineaPicking.Empresa,
                            nuevaLineaPicking.NumeroPreparacion
                        );

                    if (lineaCrossDocking != null)
                    {
                        _logger.Trace($"lineaCrossDocking != null");
                        lineaCrossDocking.CantidadPreparada = lineaCrossDocking.CantidadPreparada + (nuevaLineaPicking.CantidadPreparada ?? 0);
                        _logger.Trace($"CantidadPreparada: {lineaCrossDocking.CantidadPreparada}");
                        lineaCrossDocking.NroTransaccion = nuTransaccion;

                        uow.CrossDockingRepository.UpdateDetalleCrossDocking(lineaCrossDocking);
                    }

                    cantDisponible = cantDisponible - (cantPedido ?? 0);
                    _logger.Trace($"cantDisponible: {cantDisponible}");

                    var etiquetaDisminuir = uow.CrossDockingRepository.GetDetalleEtiqueta(nroEtiqueta, lineaEtiqueta.CodigoProducto, lineaEtiqueta.Faixa, empresa, lineaEtiqueta.Identificador);
                    etiquetaDisminuir.Cantidad = cantDisponible;
                    etiquetaDisminuir.NumeroTransaccion = nuTransaccion;
                    etiquetaDisminuir.Modificacion = DateTime.Now;
                    etiquetaDisminuir.CantidadMovilizado = (etiquetaDisminuir.CantidadMovilizado ?? 0) + (cantPedido ?? 0);

                    uow.EtiquetaLoteRepository.UpdateEtiquetaLoteDetalle(etiquetaDisminuir);
                    uow.SaveChanges();

                    var etiqueta = new LogEtiqueta()
                    {
                        Agenda = nroAgenda,
                        NumeroEtiqueta = nroEtiqueta,
                        CodigoProducto = lineaEtiqueta.CodigoProducto,
                        Faixa = lineaEtiqueta.Faixa,
                        Empresa = empresa,
                        Identificador = lineaEtiqueta.Identificador,
                        Cantidad = -1 * (lineaEtiqueta.Cantidad - cantDisponible),
                        Ubicacion = ubicacion,
                        FechaOperacion = DateTime.Now,
                        NroTransaccion = nuTransaccion,
                        Vencimiento = lineaEtiqueta.Vencimiento,
                        TipoMovimiento = TiposMovimiento.TransferirEtiqueta,
                        Aplicacion = aplicacion,
                        Funcionario = idUsuario,
                    };
                    uow.EtiquetaLoteRepository.AddLogEtiqueta(etiqueta);

                    uow.SaveChanges();

                    if (cantDisponible == 0)
                    {
                        List<EtiquetaLote> listaEtiquetas = uow.CrossDockingRepository.GetListaDeEtiquetas(nroEtiqueta);
                        foreach (var det in listaEtiquetas)
                        {
                            det.Estado = SituacionDb.TransferidoAContenedores;
                            det.NumeroTransaccion = nuTransaccion;
                            uow.EtiquetaLoteRepository.UpdateEtiquetaLote(det);
                        }
                        uow.SaveChanges();
                    }

                    countVueltas++;
                }
            }

            contenedor.NumeroTransaccion = nuTransaccion;
            contenedor.FechaModificado = fechaActual;
            contenedor.Estado = General.Enums.EstadoContenedor.EnPreparacion;

            uow.ContenedorRepository.UpdateContenedor(contenedor);

            return detallesPreparacion;
        }

        public virtual void RetornarSiguientePedidoCargaPrep(IUnitOfWork context, int nroAgenda, int cdEmpresa, string cdCliente, string cdProducto, decimal cdFaixa, string nroIdentificador, int nroPreparacion, decimal cantDisponibleEtiq, out long? nroCarga, out string nroPedido, out string especificaId, out decimal? cantPedido)
        {
            _logger.Trace($"Entro función retornarSiguientePedidoCargaPrep");
            List<LineaCrossDocking> listaLineaCrossDock = context.CrossDockingRepository.GetDetalleCrossDock(cdProducto, cdFaixa, nroIdentificador, nroAgenda, cdCliente);

            if (listaLineaCrossDock == null)
                throw new OperationNotAllowedException("No se encontraron lineas de cross-dock.");

            nroCarga = null;
            nroPedido = null;
            especificaId = null;
            cantPedido = null;
            foreach (LineaCrossDocking lineaCrossDock in listaLineaCrossDock)
            {
                nroCarga = lineaCrossDock.Carga;
                nroPedido = lineaCrossDock.Pedido;
                especificaId = lineaCrossDock.EspecificaIdentificador == true ? "S" : "N";

                decimal? cantPreparada = context.CrossDockingRepository.GetCantidadPreparada(cdProducto, cdFaixa, nroIdentificador, cdEmpresa, cdCliente, lineaCrossDock.Pedido, lineaCrossDock.Carga, nroPreparacion);

                cantPreparada = (cantPreparada ?? 0);

                _logger.Trace($"Pedido : {lineaCrossDock.Pedido}");
                _logger.Trace($"Carga : {lineaCrossDock.Carga}");
                _logger.Trace($"cantPreparada : {cantPreparada}");

                if (cantPreparada < lineaCrossDock.Cantidad)
                {
                    _logger.Trace($"cantPreparada < lineaCrossDock.Cantidad");
                    _logger.Trace($"cantPreparada : {cantPreparada}");
                    _logger.Trace($"lineaCrossDock.Cantidad : {lineaCrossDock.Cantidad}");

                    //Todavía puede seguir metiendo mercadería en ese pedido
                    cantPedido = (lineaCrossDock.Cantidad) - cantPreparada; //Lo que me queda para meter en ese pedido
                    _logger.Trace($"cantPedido : {cantPedido}");

                    //Ahora Pregunto, si lo que me queda para meter en ese pedido es mas de lo que tengo en la etiqueta
                    if (cantPedido > cantDisponibleEtiq)
                    {
                        _logger.Trace($"cantPedido > cantDisponibleEtiq");
                        _logger.Trace($"cantPedido : {cantPedido}");
                        _logger.Trace($"cantDisponibleEtiq : {cantDisponibleEtiq}");
                        //Lo que me queda del pedido es mayor a lo que tengo en la etiqueta
                        cantPedido = cantDisponibleEtiq;
                    }

                    break;
                }
                else
                {
                    //Sino queda nada, se lo asigno todo a ese pedido
                    //Al final el último pedido se va a quedar con el eventual exceso que hubiera
                    _logger.Trace($"Else: cantPedido = cantDisponibleEtiq");
                    _logger.Trace($"cantPedido : {cantPedido}");
                    _logger.Trace($"cantDisponibleEtiq : {cantDisponibleEtiq}");
                    cantPedido = cantDisponibleEtiq;
                }
            }
        }

        public virtual void Modificar_Cross_Dock(IUnitOfWork uow, int pNroAgenda, string pCdCliente, string nroPedido, int nU_PREPARACION, long? nroCarga, long newcarga, string cD_PRODUTO, decimal cD_FAIXA, int pCdEmpresa, string nU_IDENTIFICADOR, string especificaId, decimal? cantPedido, int fun)
        {
            _logger.Trace($"Entro función Modificar_Cross_Dock");

            CrossDocking new_reg = new CrossDocking();
            new_reg.Agenda = pNroAgenda;
            new_reg.Preparacion = nU_PREPARACION;
            new_reg.Usuario = fun;
            new_reg.FechaAlta = DateTime.Now;

            _logger.Trace($"Agenda: {new_reg.Agenda}");
            _logger.Trace($"Preparación: {new_reg.Preparacion}");

            List<LineaCrossDocking> lista = uow.CrossDockingRepository.GetDetalleCrossDock(pNroAgenda, nU_PREPARACION, nroPedido, pCdCliente, pCdEmpresa, nroCarga, cD_PRODUTO, cD_FAIXA, nU_IDENTIFICADOR, especificaId, cantPedido);
            foreach (var det in lista)
            {
                det.Carga = newcarga;

                _logger.Trace($"Actualizo detalle crossdocking");
                _logger.Trace($"Pedido: {det.Pedido}");
                _logger.Trace($"Cliente: {det.Cliente}");
                _logger.Trace($"Empresa: {det.Empresa}");
                _logger.Trace($"Producto: {det.Producto}");
                _logger.Trace($"Identificador: {det.Identificador}");
                _logger.Trace($"Carga: {det.Carga}");
                _logger.Trace($"Cantidad: {det.Cantidad}");
                _logger.Trace($"CantidadPreparada: {det.CantidadPreparada}");

                uow.CrossDockingRepository.UpdateDetalleCrossDocking(det);
            }
        }

        public virtual void ProcesarIntegracionDocumental(IUnitOfWork uow, int nroAgenda, int empresa, List<DetallePreparacion> pickings)
        {
            var nuTransaccion = uow.GetTransactionNumber();
            var manejaDocumental = uow.ParametroRepository.GetParameter(ParamManager.MANEJO_DOCUMENTAL, new Dictionary<string, string> { [ParamManager.PARAM_EMPR] = $"{ParamManager.PARAM_EMPR}_{empresa}" }) == "S";
            var reservasAPreparar = new List<DocumentoPreparacionReserva>();

            if (manejaDocumental)
            {
                var docIngreso = uow.DocumentoRepository.GetIngresoPorAgenda(nroAgenda);
                foreach (var picking in pickings)
                {
                    var docPrepReserva = reservasAPreparar.FirstOrDefault(l => l.NroDocumento == docIngreso.Numero
                        && l.TipoDocumento == docIngreso.Tipo
                        && l.Preparacion == picking.NumeroPreparacion
                        && l.Empresa == picking.Empresa
                        && l.Producto == picking.Producto
                        && l.Faixa == picking.Faixa
                        && l.NroIdentificadorPicking == picking.Lote);

                    if (docPrepReserva == null)
                    {
                        docPrepReserva = uow.DocumentoRepository.GetPreparacionReserva(docIngreso.Numero, docIngreso.Tipo, picking.NumeroPreparacion, picking.Empresa, picking.Producto, picking.Faixa, picking.Lote);
                        reservasAPreparar.Add(docPrepReserva);
                    }

                    if (docPrepReserva != null)
                    {
                        docPrepReserva.CantidadPreparada = (docPrepReserva.CantidadPreparada ?? 0) + picking.CantidadPreparada;
                        docPrepReserva.FechaModificacion = DateTime.Now;
                    }
                }

                foreach (var reserva in reservasAPreparar)
                {
                    reserva.NumeroTransaccion = nuTransaccion;
                    reserva.NumeroTransaccionDelete = null;
                    uow.DocumentoRepository.UpdateDocumentoPreparacionReserva(reserva);
                }
            }
        }
    }
}
