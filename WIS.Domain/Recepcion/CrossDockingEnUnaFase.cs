using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Recepcion;
using WIS.Domain.Documento;
using WIS.Domain.Picking;
using WIS.Exceptions;
using WIS.Filtering;
using WIS.GridComponent.Execution.Configuration;

namespace WIS.Domain.Recepcion
{
    public class CrossDockingEnUnaFase : CrossDocking
    {
        public CrossDockingEnUnaFase() : base()
        {
            this.Tipo = TipoCrossDockingDb.UnaFase;
        }

        public override bool PuedeFinalizarCrossDock()
        {
            return false;
        }

        #region Agregar y quitar pedidos

        public override IEnumerable<Pedido> GetPedidosToAdd(IUnitOfWork uow, Agenda agenda, CrossDockingSeleccionTipo tipoSeleccion, GridMenuItemActionContext context, List<string> gridKeys, IFilterInterpreter filterInterpreter)
        {
            var selection = context.Selection.GetSelection(gridKeys, keys =>
            {
                return new Pedido
                {
                    Id = keys["NU_PEDIDO"],
                    Cliente = keys["CD_CLIENTE"],
                    Empresa = int.Parse(keys["CD_EMPRESA"])
                };
            });

            if (context.Selection.AllSelected)
            {
                var dbQuery = new PedidosCrossDockQuery(agenda, tipoSeleccion);

                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(filterInterpreter, context.Filters);

                selection = dbQuery.GetPedidos().Except(selection).ToList();
            }

            return uow.PedidoRepository.GetPedidosContext(selection);
        }

        public override void AddPedidos(IUnitOfWork uow, IEnumerable<Pedido> pedidos)
        {
            base.AddPedidos(uow, pedidos);

            var agenda = uow.AgendaRepository.GetAgenda(this.Agenda);

            var crossDockingAgenda = new CrossDockingAgenda(uow, agenda);

            agenda.Detalles = agenda.Detalles.OrderBy(w => w.Identificador).ToList();

            crossDockingAgenda.AtenderPedidoCrossDocking(pedidos);

            uow.SaveChanges();
        }

        public override void RemovePedidos(IUnitOfWork uow, List<Pedido> pedidos)
        {
            Agenda agenda = uow.AgendaRepository.GetAgenda(this.Agenda);

            var crossDockingAgenda = new CrossDockingAgenda(uow, agenda);

            foreach (var pedido in pedidos)
            {
                crossDockingAgenda.DesatenderPedidoCrossDocking(this, pedido, false, false);
            }

            base.RemovePedidos(uow, pedidos);

            uow.SaveChanges();
        }

        #endregion

        #region Iniciar CrossDocking

        public override void Iniciar(IUnitOfWork uow, Agenda agenda, bool consumirOtrosDocumentos)
        {
            Liberar(uow, agenda, new List<Carga>());

            uow.CrossDockingRepository.ProcesarInicioCrossDockingUF(_operationContext);
        }

        public override void Liberar(IUnitOfWork uow, Agenda agenda, List<Carga> cargas)
        {
            var nuTransaccion = uow.GetTransactionNumber();

            this.Estado = EstadoCrossDockingDb.Iniciado;

            uow.CrossDockingRepository.UpdateCrossDocking(this);

            var detallesTemporales = uow.CrossDockingRepository.GetCrossDockTempWrec220(agenda.IdEmpresa, agenda.Id);

            if (detallesTemporales == null || detallesTemporales.Count == 0)
                throw new ValidationFailedException("General_Sec0_Error_Error64_NoHayDatosParaLiberar");

            IDocumentoIngreso docIngreso = null;
            var lineasAModificar = new List<DocumentoLinea>();
            var nuevasReservas = new List<DocumentoPreparacionReserva>();

            var manejaDocumental = uow.ParametroRepository.GetParameter(ParamManager.MANEJO_DOCUMENTAL, new Dictionary<string, string> { [ParamManager.PARAM_EMPR] = $"{ParamManager.PARAM_EMPR}_{agenda.IdEmpresa}" }) == "S";

            if (manejaDocumental)
                docIngreso = uow.DocumentoRepository.GetIngresoPorAgenda(agenda.Id);

            detallesTemporales.ForEach(delegate (CrossDockingTemp detalleTemp)
            {
                var pedido = uow.PedidoRepository.GetPedido(detalleTemp.CD_EMPRESA, detalleTemp.CD_CLIENTE, detalleTemp.NU_PEDIDO);

                var detallePedidoAux = new DetallePedido
                {
                    Producto = detalleTemp.CD_PRODUTO,
                    Identificador = detalleTemp.NU_IDENTIFICADOR,
                    EspecificaIdentificador = detalleTemp.ID_ESPECIFICA_IDENTIFICADOR,
                    Faixa = detalleTemp.CD_FAIXA
                };

                AddLinea(uow, cargas, pedido, detallePedidoAux, detalleTemp.QT_PRODUTO, nuTransaccion);

                if (docIngreso != null)
                {
                    var lineaIngreso = lineasAModificar.FirstOrDefault(l => l.Producto == detalleTemp.CD_PRODUTO
                                        && l.Faixa == detalleTemp.CD_FAIXA
                                        && l.Identificador == detalleTemp.NU_IDENTIFICADOR);

                    if (lineaIngreso == null)
                    {
                        lineaIngreso = docIngreso.Lineas.FirstOrDefault(d => d.Producto == detalleTemp.CD_PRODUTO
                                        && d.Faixa == detalleTemp.CD_FAIXA
                                        && d.Identificador == detalleTemp.NU_IDENTIFICADOR);

                        lineasAModificar.Add(lineaIngreso);
                    }

                    if (lineaIngreso != null)
                    {
                        lineaIngreso.CantidadReservada = (lineaIngreso.CantidadReservada ?? 0) + detalleTemp.QT_PRODUTO;
                        lineaIngreso.FechaModificacion = DateTime.Now;
                    }

                    var reserva = nuevasReservas
                        .FirstOrDefault(r => r.NroDocumento == docIngreso.Numero
                            && r.TipoDocumento == docIngreso.Tipo
                            && r.Preparacion == this.Preparacion
                            && r.Empresa == detalleTemp.CD_EMPRESA
                            && r.Producto == detalleTemp.CD_PRODUTO
                            && r.Faixa == detalleTemp.CD_FAIXA
                            && r.NroIdentificadorPicking == detalleTemp.NU_IDENTIFICADOR);

                    if (reserva != null)
                    {
                        reserva.CantidadProducto = (reserva.CantidadProducto ?? 0) + detalleTemp.QT_PRODUTO;
                    }
                    else
                    {
                        reserva = new DocumentoPreparacionReserva()
                        {
                            NroDocumento = docIngreso.Numero,
                            TipoDocumento = docIngreso.Tipo,
                            Preparacion = this.Preparacion,
                            Empresa = detalleTemp.CD_EMPRESA,
                            Producto = detalleTemp.CD_PRODUTO,
                            Faixa = detalleTemp.CD_FAIXA,
                            Identificador = detalleTemp.NU_IDENTIFICADOR,
                            NroIdentificadorPicking = detalleTemp.NU_IDENTIFICADOR,
                            EspecificaIdentificador = detalleTemp.ID_ESPECIFICA_IDENTIFICADOR,
                            CantidadProducto = detalleTemp.QT_PRODUTO,
                            CantidadPreparada = null,
                            CantidadAnular = null,
                            FechaAlta = DateTime.Now
                        };

                        nuevasReservas.Add(reserva);
                    }
                }

                _operationContext.RemoveDetalleCrossDockingTemporal.Add(detalleTemp);
            });

            if (docIngreso != null)
            {
                _operationContext.UpdateDetallesDocumento = (docIngreso.Numero, docIngreso.Tipo, lineasAModificar);

                _operationContext.NewDocumentoPreparacionReserva.AddRange(nuevasReservas);
            }

            var pedidos = uow.PedidoRepository.GetPedidosPreparacionProgramada(this.Preparacion);

            var pedidosQuitar = pedidos.Where(w => !detallesTemporales.Any(a => a.NU_PEDIDO == w.Id && a.CD_CLIENTE == w.Cliente && a.CD_EMPRESA == w.Empresa)).ToList();
            
            RemovePedidos(uow, pedidosQuitar);
        }

        #endregion
    }
}
