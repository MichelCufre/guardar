using NLog;
using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Recepcion.Enums;
using WIS.Exceptions;

namespace WIS.Domain.Recepcion
{
    public class ModificarAgendaDetalle
    {
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();

        protected readonly IUnitOfWork _uow;
        protected string _aplicacion;
        protected int _usuario;
        protected int _numeroAgenda;
        protected int _idEmpresa;
        protected string _codigoProducto;
        protected decimal _faixa;
        protected string _identificador;

        /// <summary>
        /// Constructor para modificar el detalle de agenda
        /// Métodos: 
        /// - ModificarDetalle
        /// </summary>
        /// <param name="uow"></param>
        /// <param name="usuario"></param>
        /// <param name="aplicacion"></param>
        /// <param name="numeroAgenda"></param>
        /// <param name="idEmpresa"></param>
        /// <param name="codigoProducto"></param>
        /// <param name="faixa"></param>
        /// <param name="identificador"></param>
        public ModificarAgendaDetalle(IUnitOfWork uow, int usuario, string aplicacion, int numeroAgenda, int idEmpresa, string codigoProducto, decimal faixa, string identificador)
        {
            this._uow = uow;
            this._usuario = usuario;
            this._aplicacion = aplicacion;
            this._numeroAgenda = numeroAgenda;
            this._idEmpresa = idEmpresa;
            this._codigoProducto = codigoProducto;
            this._faixa = faixa;
            this._identificador = identificador;

        }

        /// <summary>
        ///  /// Métodos: 
        /// - ActualizarSituacionAgendaDetalle
        /// </summary>
        /// <param name="uow"></param>
        /// <param name="usuario"></param>
        /// <param name="aplicacion"></param>
        public ModificarAgendaDetalle(IUnitOfWork uow, int usuario, string aplicacion)
        {
            this._uow = uow;
            this._usuario = usuario;
            this._aplicacion = aplicacion;
        }

        /// <summary>
        /// Modifica las cantidades del detalle de agenda
        /// Comprueba y actualiza los problemas de recepcion
        /// </summary>
        /// <param name="cantidadAjuste"></param>
        public virtual void ModificarDetalle(decimal cantidadAjuste)
        {
            var detalleAgenda = this._uow.AgendaRepository.GetAgendaDetalle(_numeroAgenda, _idEmpresa, _codigoProducto, _faixa, _identificador);
            var nuTransaccion = _uow.GetTransactionNumber();

            if (detalleAgenda == null)
                throw new ValidationFailedException("General_Sec0_Error_DetalleAgendaNoEncontrado", new string[] { _numeroAgenda.ToString(), _idEmpresa.ToString(), _codigoProducto, _identificador });

            logger.Info("ModificarDetalleAgenda: Update Cantida Recibida");

            detalleAgenda.CantidadRecibida = (detalleAgenda.CantidadRecibida + cantidadAjuste);
            detalleAgenda.NumeroTransaccion = nuTransaccion;

            AgendaDetalle lineaAuto = null;

            if (detalleAgenda.CantidadAgendada >= detalleAgenda.CantidadRecibida && detalleAgenda.CantidadAgendadaOriginal == 0)
            {
                lineaAuto = this._uow.AgendaRepository.GetAgendaDetalle(_numeroAgenda, _idEmpresa, _codigoProducto, _faixa, ManejoIdentificadorDb.IdentificadorAuto);

                if (lineaAuto != null)
                {
                    decimal qtRestante = detalleAgenda.CantidadAgendada - detalleAgenda.CantidadRecibida;

                    detalleAgenda.CantidadAgendada += qtRestante * -1;

                    lineaAuto.NumeroTransaccion = nuTransaccion;

                    List<AgendaDetalle> lineasSecundarias = this._uow.AgendaRepository.GetAgendaDetalleSecundarios(_numeroAgenda, _idEmpresa, _codigoProducto, _faixa, _identificador);

                    //P1 (AUTO) 1 0 6  (10)  SI LO RECIBIDO ES MENOR O IGUAL AL ORIGINAL DEL AUTO, SE IGUALA 

                    //P1  L1   2 2 0   (9)
                    //P1  L2   1 1 0   (9)
                    //P1  L3   2 2 0   (9)

                    //ESTO ES PARA AJUSTAR LOS DEMAS DETALLES, PARA NO DEJAR RESTANTE EN EL AUTO SIN SER NECESARIO
                    if (lineasSecundarias != null && lineasSecundarias.Count > 0)
                    {
                        foreach (var item in lineasSecundarias)
                        {
                            decimal qtRestanteSecundario = item.CantidadRecibida - item.CantidadAgendada;

                            item.NumeroTransaccion = nuTransaccion;

                            if (qtRestanteSecundario < qtRestante)
                            {
                                qtRestante -= qtRestanteSecundario;
                                item.CantidadAgendada += qtRestanteSecundario;

                                new ModificarAgendaDetalleProblema(_uow, _usuario, _aplicacion, item, lineaAuto).ActualizarProblemasEnDetalle();

                                _uow.AgendaRepository.UpdateAgendaDetalle(item);
                                _uow.SaveChanges();
                            }
                            else
                            {
                                item.CantidadAgendada += qtRestante;
                                qtRestante -= qtRestante;

                                new ModificarAgendaDetalleProblema(_uow, _usuario, _aplicacion, item, lineaAuto).ActualizarProblemasEnDetalle();

                                _uow.AgendaRepository.UpdateAgendaDetalle(item);
                                _uow.SaveChanges();

                                break;
                            }
                        }
                    }

                    if (qtRestante > 0)
                    {
                        lineaAuto.CantidadAgendada += qtRestante;
                        _uow.AgendaRepository.UpdateAgendaDetalle(lineaAuto);
                    }
                }
            }

            _uow.SaveChanges();

            new ModificarAgendaDetalleProblema(_uow, _usuario, _aplicacion, detalleAgenda, lineaAuto).ActualizarProblemasEnDetalle();
            new ModificarAgendaDetalleProblema(_uow, _usuario, _aplicacion, detalleAgenda, lineaAuto).ActualizarProblemasEnDetalleLocal();

            _uow.AgendaRepository.UpdateAgendaDetalle(detalleAgenda);
            _uow.SaveChanges();

            logger.Info("ModificarDetalleAgenda: Commit Update Cantida Recibida");
        }

        /// <summary>
        /// Actualiza el estado del detalle dependiendo si hay problemas (Con / sin diferencia), No atacha la entidad en UOW
        /// </summary>
        /// <param name="detalle"></param>
        /// <returns></returns>
        public virtual bool ActualizarSituacionAgendaDetalle(AgendaDetalle detalle)
        {
            var conProblema = _uow.AgendaRepository.AnyProblemaAgendaDetalle(detalle.IdAgenda, detalle.CodigoProducto, detalle.Faixa, detalle.Identificador);

            detalle.Estado = (conProblema ? EstadoAgendaDetalle.ConferidaConDiferencias : EstadoAgendaDetalle.ConferidaSinDiferencias);

            return conProblema;
        }

        public virtual bool ActualizarSituacionAgendaDetalleLocal(AgendaDetalle detalle)
        {
            var conProblema = _uow.AgendaRepository.AnyProblemaAgendaDetalleLocal(detalle.IdAgenda, detalle.CodigoProducto, detalle.Faixa, detalle.Identificador);

            detalle.Estado = (conProblema ? EstadoAgendaDetalle.ConferidaConDiferencias : EstadoAgendaDetalle.ConferidaSinDiferencias);

            return conProblema;
        }

    }
}
