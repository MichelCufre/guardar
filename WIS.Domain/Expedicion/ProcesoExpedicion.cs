using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.Picking;
using WIS.Domain.Reportes;
using WIS.Domain.Services.Interfaces;
using WIS.Exceptions;
using WIS.Security;

namespace WIS.Domain.Expedicion
{
    public class ProcesoExpedicion
    {
        protected readonly IDapper _dapper;
        protected readonly IParameterService _parameterService;
        protected readonly IIdentityService _identity;
        protected readonly IFactoryService _factoryService;
        protected readonly IReportKeyService _reporteKeyService;
        protected readonly IBarcodeService _barcodeService;
        protected readonly ITaskQueueService _taskQueue;

        public ProcesoExpedicion(IDapper dapper, IParameterService parameterService, IIdentityService identity, IFactoryService factoryService, IReportKeyService reporteKeyService, IBarcodeService barcodeService, ITaskQueueService taskQueue)
        {
            this._dapper = dapper;
            this._identity = identity;
            this._taskQueue = taskQueue;
            this._factoryService = factoryService;
            this._barcodeService = barcodeService;
            this._parameterService = parameterService;
            this._reporteKeyService = reporteKeyService;
        }

        public virtual void ExpedirCamion(IUnitOfWork uow, List<ContenedorExpedir> contenedoresAExpedir, out List<string> keysCamiones, out List<string> keysReportes)
        {
            keysCamiones = new List<string>();
            keysReportes = new List<string>();

            List<Camion> list = new List<Camion>();
            ContenedorLogic contL = new ContenedorLogic(_identity.UserId, _identity.Predio);
            var camionesCreadosDesdeExpedir = new HashSet<int>();

            foreach (var contenedor in contenedoresAExpedir)
            {
                Camion camion;
                CargaCamion cargarCam = uow.CargaCamionRepository.GetCamionCarga(contenedor.NumeroCarga, contenedor.CodigoEmpresa, contenedor.CodigoCliente);

                if (cargarCam != null)
                {
                    camion = uow.CamionRepository.GetCamion(cargarCam.Camion);
                }
                else
                {
                    var nroPuerta = uow.PuertaEmbarqueRepository.GetFirstPuertaByPredio(_identity.Predio);
                    if (nroPuerta == null)
                        throw new ValidationFailedException("EXP330_form1_Error_SinPuertaEmbarque");

                    var cdCamion = FacturacionLegacy.CrearCamionFactura(uow, contenedor.CodigoEmpresa, nroPuerta, _identity.Predio, "Creado en Pedidos de Mostrador - opción Expedir");

                    var cargaCamion = new CargaCamion
                    {
                        Camion = cdCamion,
                        Carga = contenedor.NumeroCarga,
                        Cliente = contenedor.CodigoCliente,
                        Empresa = contenedor.CodigoEmpresa,
                        FechaAlta = DateTime.Now
                    };

                    uow.CargaCamionRepository.AddCargaCamion(cargaCamion);

                    camionesCreadosDesdeExpedir.Add(cdCamion);

                    uow.SaveChanges();
                    camion = uow.CamionRepository.GetCamion(cdCamion);
                }

                if (uow.ContenedorRepository.ExisteContenedorEnPreparacion(contenedor.NumeroContenedor, contenedor.NumeroPreparacion))
                {
                    list.Add(camion);

                    List<CargaCamion> listCargaCamion = uow.CargaCamionRepository.GetsCargasCamion(camion);
                    foreach (CargaCamion carga in listCargaCamion)
                    {
                        List<Contenedor> listCont = uow.ContenedorRepository.GetContenedoresCargaFacturado(carga);
                        foreach (Contenedor cont in listCont)
                        {
                            ContenedorExpedir contEx = new ContenedorExpedir();
                            contEx.CodigoEmpresa = carga.Empresa;
                            contEx.CodigoCliente = carga.Cliente;
                            contEx.NumeroCarga = carga.Carga;
                            contEx.NumeroContenedor = cont.Numero;
                            contEx.NumeroPreparacion = cont.NumeroPreparacion;
                            contL.CargarContenedoresCamion(uow, camion, contEx);
                        }
                    }
                }

                uow.SaveChanges();
            }

            foreach (var camion in list)
            {
                var creadoDesdeExpedir = camionesCreadosDesdeExpedir.Contains(camion.Id);
                CerrarCamion(uow, camion, creadoDesdeExpedir);

                var cierre = new CierreEgreso(uow, camion, _dapper, _parameterService, _identity, _factoryService, _reporteKeyService, _barcodeService, _taskQueue);
                var reportes = cierre.GenerarReportes(uow, camion);

                if (camion.NumeroInterfazEjecucionCierre == -1)
                {
                    keysCamiones.Add(camion.Id.ToString());
                    keysReportes.AddRange(reportes.Select(x => x.ToString()).ToList());
                }
            }
        }

        public virtual void CerrarCamion(IUnitOfWork uow, Camion camion, bool creadoDesdeExpedir)
        {
            var controlar_fact_cierre = uow.ParametroRepository.GetParameter("WEXP040_CONTROLAR_FACT_CIERRE");

            if (camion.IsControlContenedoresHabilitado && uow.PreparacionRepository.AnyContenedorSinControl(camion, out int cantCont))
                throw new OperationNotAllowedException("General_Sec0_Error_ContenedoreSinControlar", new string[] { cantCont.ToString() });
            else if (uow.PreparacionRepository.AnyContenedorSinFinalizarControl(camion, out cantCont))
                throw new OperationNotAllowedException("General_Sec0_Error_ContenedoreSinFinalizarControl", new string[] { cantCont.ToString() });
            else
            {
                if (!creadoDesdeExpedir && controlar_fact_cierre != null && controlar_fact_cierre.Equals("S") &&
                    uow.CamionRepository.RequiereFacturacion(camion.Id)
                    && (camion.NumeroInterfazEjecucionFactura == -1 || camion.NumeroInterfazEjecucionFactura == null))
                {
                    throw new Exception("WEXP040_Sec0_Error_Er001_CamionDebeEstarFacturado");
                }
                else
                {
                    if (camion.NumeroInterfazEjecucionFactura == null)
                    {
                        ContenedorLogic contL = new ContenedorLogic();
                        contL.FacturarContenedores(uow, camion, _identity.UserId);
                        camion.NumeroTransaccion = uow.GetTransactionNumber();
                        camion.NumeroInterfazEjecucionFactura = 0;
                        uow.CamionRepository.UpdateCamion(camion);
                    }
                }

                #region  - Control Documental -

                string manejoDocumental = Convert.ToString((uow.ParametroRepository.GetParameter(ParamManager.MANEJO_DOCUMENTAL)) ?? "N");
                if (manejoDocumental == "S")
                {
                    if (!camion.IsCierreHabilitado)
                        throw new ValidationFailedException("General_Sec0_Error_EgresoDocumentalCamionSinCierreHabilitado");
                }
                else if (!camion.IsCierreParcialHabilitado)
                {
                    if (uow.PreparacionRepository.AnyProductoSinPreparar(camion.Id))
                        throw new ValidationFailedException("General_Sec0_Error_Er202_PickeosPendientes");

                    if (!uow.PreparacionRepository.AnyContenedorEmbarcado(camion.Id))
                        throw new ValidationFailedException("General_Sec0_Error_Er200_ContNoCargadosCamion");

                    if (uow.PreparacionRepository.AnyContenedorSinEmbarcar(camion.Id))
                        throw new ValidationFailedException("General_Sec0_Error_Er201_ContPendientesEnvios");
                }

                #endregion

                CamionLogic dbCamion = new CamionLogic();
                dbCamion.ProcesarCerrarCamion(uow, camion, this);
            }

        }

        public virtual void GrabarExpedicion(IUnitOfWork uow, int cdCam, string numeroPedido, string codigoCliente, int codigoEmpresa, string codigoProducto, decimal codigoFaixa, string lote, bool especificaLote, decimal? cantidadPreparada)
        {
            DetallePedidoExpedido detPedExp = uow.PedidoRepository.GetDetallePedidoExpedido(cdCam, numeroPedido, codigoCliente, codigoEmpresa, codigoProducto, codigoFaixa, lote, especificaLote);

            if (detPedExp != null)
            {
                detPedExp.Cantidad = detPedExp.Cantidad + cantidadPreparada;
                detPedExp.FechaExpedicion = DateTime.Now;
                uow.PedidoRepository.UpdateDetallePedidoExpedido(detPedExp);
            }
            else
                AgregarDetallePedidoExpedido(uow, cdCam, numeroPedido, codigoCliente, codigoEmpresa, codigoProducto, codigoFaixa, lote, especificaLote, cantidadPreparada);
        }

        public virtual void AgregarDetallePedidoExpedido(IUnitOfWork uow, int cdCam, string nuPedido, string cdCliente, int cdEmp, string cdProd, decimal cdFaixa, string iden, bool idEspecIdent, decimal? qtPrep)
        {
            DetallePedidoExpedido detPedExp = new DetallePedidoExpedido();
            detPedExp.Camion = cdCam;
            detPedExp.Pedido = nuPedido;
            detPedExp.Cliente = cdCliente;
            detPedExp.Empresa = cdEmp;
            detPedExp.Producto = cdProd;
            detPedExp.Faixa = cdFaixa;
            detPedExp.Identificador = iden;
            detPedExp.EspecificaLote = idEspecIdent;
            detPedExp.FechaExpedicion = DateTime.Now;
            detPedExp.Cantidad = qtPrep;

            uow.PedidoRepository.AddDetallePedidoExpedido(detPedExp);
        }

    }
}
