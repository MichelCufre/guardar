using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Mappers.Produccion;
using WIS.Domain.Produccion;
using WIS.Domain.Services.Interfaces;
using WIS.Exceptions;
using WIS.Extension;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.GridComponent.Build;
using WIS.GridComponent.Excel;
using WIS.PageComponent.Execution;
using WIS.Security;
using WIS.Session;

namespace WIS.Application.Controllers.PRD
{
    public class PRD175 : AppController
    {
        protected readonly ISessionAccessor _session;
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IDocumentoService _documentoService;
        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();

        protected List<string> GridKeys { get; }

        public PRD175(ISessionAccessor session,
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IGridExcelService excelService,
            IFormValidationService formValidationService,
            IGridValidationService gridValidationService,
            IDocumentoService documentoService)
        {
            this._session = session;
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._excelService = excelService;
            this._formValidationService = formValidationService;
            this._gridValidationService = gridValidationService;
            this._documentoService = documentoService;
        }

        public override PageContext PageLoad(PageContext context)
        {
            try
            {
                using (var uow = this._uowFactory.GetUnitOfWork())
                {
                    string nroIngreso = context.GetParameter("nuPrdcIngreso");
                    IIngresoPanel ingreso = uow.ProduccionRepository.GetIngresoPanel(nroIngreso);

                    if (ingreso != null)
                    {
                        if (ingreso.Linea != null)
                        {
                            context.AddParameter("CD_LINEA", ingreso.Linea.Id);
                            context.AddParameter("CD_ENDERECO_ENTRADA", ingreso.Linea.UbicacionEntrada);
                            context.AddParameter("TP_LINEA", ingreso.Linea.Tipo.ToString());
                            context.AddParameter("DS_LINEA", ingreso.Linea.Descripcion);
                            context.AddParameter("CD_ENDERECO_SALIDA", ingreso.Linea.UbicacionSalida);
                        }

                        ProduccionMapper mapper = new ProduccionMapper(new LineaMapper(), new FormulaMapper(new FormulaAccionMapper()));
                        var predio = uow.PredioRepository.GetPredio(ingreso.Predio);

                        context.AddParameter("NU_PRDC_INGRESO", ingreso.Id);
                        context.AddParameter("CD_SITUACION", string.Format("{0} - {1}", ingreso.Situacion, mapper.GetDescripcionSituacion(ingreso.Situacion)));
                        context.AddParameter("NU_PRDC_DEFNICION", string.Format("{0} - {1}", ingreso.Formula.Id, ingreso.Formula.Descripcion));
                        context.AddParameter("NU_PREDIO", string.Format("{0} - {1}", ingreso.Predio, predio.Descripcion));
                        context.AddParameter("NU_DOCUMENTO_INGRESO", "");
                        context.AddParameter("DT_ADDROW", ingreso.FechaAlta.ToIsoString());
                        context.AddParameter("QT_FORMULA", ingreso.CantidadIteracionesFormula.ToString());
                        context.AddParameter("CD_FUNCIONARIO", string.Format("{0} - {1}", ingreso.Funcionario, uow.SecurityRepository.GetUserFullname((int)ingreso.Funcionario)));
                        context.AddParameter("NU_DOCUMENTO_EGRESO", "");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                context.AddErrorNotification("General_Sec0_Error_Error45");
            }

            return context;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                string nroIngreso = context.GetParameter("nuPrdcIngreso");
                var ingreso = uow.ProduccionRepository.GetIngresoWhiteBox(nroIngreso);

                if (ingreso.Situacion == SituacionDb.PRODUCCION_FINALIZADA)
                {
                    form.Buttons.FirstOrDefault(b => b.Id == "btnCerrarProduccionWhiteBox").Disabled = true;
                    form.Buttons.FirstOrDefault(b => b.Id == "btnConfirmarProduccionWhiteBox").Disabled = true;
                    form.Buttons.FirstOrDefault(b => b.Id == "btnDescartarPasadasProduccionWhiteBox").Disabled = true;
                }
                else
                {
                    IngresoWhiteBox ingreso1 = uow.ProduccionRepository.GetIngresoWhiteBox(nroIngreso);
                    if (!uow.StockRepository.AnyStockUbicacion(ingreso1.Linea.UbicacionSalida))
                    {
                        form.Buttons.FirstOrDefault(b => b.Id == "btnCerrarProduccionWhiteBox").Disabled = true;
                    }
                }
            }

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            switch (context.ButtonId)
            {
                case "btnCerrarProduccionWhiteBox":
                    this.CerrarProduccion(form, context);
                    break;
                case "btnConfirmarProduccionWhiteBox":
                    this.ConfirmarProduccion(form, context);
                    break;
                case "btnDesreservarProduccionWhiteBox":
                    this.DesreservarStockProduccion(form, context);
                    break;
                case "btnDescartarPasadasProduccionWhiteBox":
                    //this.DescartarPasadasProduccion(form, context);
                    break;
            }

            return form;
        }

        #region Metodos Auxiliares

        public virtual void CerrarProduccion(Form form, FormSubmitContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                try
                {
                    uow.CreateTransactionNumber("PRD175 CerrarProduccion");

                    var nroIngreso = context.GetParameter("nuPrdcIngreso");

                    this.ValidarCierreDeProduccion(form, context, uow, nroIngreso);

                    var ingreso = uow.ProduccionRepository.GetIngresoWhiteBox(nroIngreso);
                    var nuTransaccion = uow.GetTransactionNumber();
                    var cierreProduccion = new CierreProduccionWhiteBox(uow, ingreso, this._identity.UserId, nuTransaccion);

                    #region INTEGRACION_DOCUMENTAL

                    var manejoDocumental = uow.ParametroRepository.GetParameter(ParamManager.MANEJO_DOCUMENTAL, new Dictionary<string, string> { [ParamManager.PARAM_EMPR] = $"{ParamManager.PARAM_EMPR}_{ingreso.Empresa}" });

                    if (manejoDocumental == "S")
                    {
                        var response = this._documentoService.CrearDocumentos(uow, ingreso);

                        if (!response.Success)
                        {
                            throw new ValidationFailedException(response.ErrorMsg);
                        }
                        else
                        {
                            //Registrar documentos de produccion
                            ingreso.Documento = new Domain.Produccion.Documento()
                            {
                                NumeroEgreso = response.NroDocumentoEgreso,
                                TipoEgreso = response.TipoDocumentoEgreso,
                                NumeroIngreso = response.NroDocumentoIngreso,
                                TipoIngreso = response.TipoDocumentoIngreso
                            };

                            uow.ProduccionRepository.AddDocumentosIngreso(ingreso, ingreso.Documento);
                        }
                    }

                    #endregion

                    cierreProduccion.CerrarProduccion();
                    cierreProduccion.GenerarHistorico();

                    uow.SaveChanges();

                    context.AddSuccessNotification("PRD175_Sec0_Success_ProduccionCerrada");

                    form.Buttons.FirstOrDefault(b => b.Id == "btnCerrarProduccionWhiteBox").Disabled = true;
                    form.Buttons.FirstOrDefault(b => b.Id == "btnConfirmarProduccionWhiteBox").Disabled = true;
                    form.Buttons.FirstOrDefault(b => b.Id == "btnDescartarPasadasProduccionWhiteBox").Disabled = true;
                }
                catch (Exception ex)
                {
                    this._logger.Error(ex, ex.Message);
                    throw ex;
                }
            }
        }

        public virtual void ConfirmarProduccion(Form form, FormSubmitContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                try
                {
                    uow.CreateTransactionNumber("PRD175 ConfirmarProduccion");

                    var nroIngreso = context.GetParameter("nuPrdcIngreso");

                    //Validar
                    this.ValidarConfirmarProduccion(form, context, uow, nroIngreso);

                    var nuTransaccion = uow.GetTransactionNumber();
                    var ingreso = uow.ProduccionRepository.GetIngresoWhiteBox(nroIngreso);
                    var confirmarProduccion = new ConfirmacionProduccion(uow, ingreso, nuTransaccion, this._identity.UserId);

                    ////Confirmar lineas consumo                    
                    //var lineasConsumoAgrupadas = ingreso.Consumidos.GroupBy(c => new { c.Empresa, c.Faixa, c.Producto, c.Identificador }).ToList();

                    //foreach (var grupoConsumo in lineasConsumoAgrupadas)
                    //{
                    //    string producto = grupoConsumo.FirstOrDefault().Producto;
                    //    string identificador = grupoConsumo.FirstOrDefault().Identificador;
                    //    decimal faixa = grupoConsumo.FirstOrDefault().Faixa;
                    //    int empresa = grupoConsumo.FirstOrDefault().Empresa;
                    //    decimal cantidad = grupoConsumo.Sum(c => c.Cantidad);

                    //    confirmarProduccion.ConfirmarConsumoProduccion(producto, faixa, empresa, identificador, ingreso.Linea.UbicacionEntrada, cantidad);
                    //}

                    //Confirmar lineas producido
                    //              var lineasProducidoAgrupadas = ingreso.Producidos.GroupBy(p => new { p.Empresa, p.Faixa, p.Producto, p.Identificador }).ToList();

                    //              foreach (var grupoProducido in lineasProducidoAgrupadas)
                    //              {
                    //int empresaInt = (int)grupoProducido.FirstOrDefault().Empresa;

                    //string producto = grupoProducido.FirstOrDefault().Producto;
                    //                  string identificador = grupoProducido.FirstOrDefault().Identificador;
                    //                  decimal faixa = grupoProducido.FirstOrDefault().Faixa;
                    //                  int empresa = empresaInt;
                    //                  decimal cantidad = grupoProducido.Sum(c => c.Cantidad);
                    //                  DateTime? vencimiento = grupoProducido.Min(P => P.Vencimiento);

                    //                  confirmarProduccion.ConfirmarProducidoProduccion(producto, faixa, empresa, identificador, vencimiento, ingreso.Linea.UbicacionSalida, cantidad);
                    //              }

                    uow.SaveChanges();

                    context.AddSuccessNotification("PRD175_Sec0_Success_ProduccionConfirmada");

                    form.Buttons.FirstOrDefault(b => b.Id == "btnCerrarProduccionWhiteBox").Disabled = false;
                }
                catch (Exception ex)
                {
                    this._logger.Error(ex, ex.Message);
                    throw ex;
                }
            }
        }

        public virtual void DesreservarStockProduccion(Form form, FormSubmitContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                try
                {
                    uow.CreateTransactionNumber("PRD175 DesreservarStockProduccion");

                    var nroIngreso = context.GetParameter("nuPrdcIngreso");

                    //this.ValidarDesreservarStock(form, context, uow, nroIngreso);

                    var nuTransaccion = uow.GetTransactionNumber();
                    var ingreso = uow.ProduccionRepository.GetIngresoWhiteBox(nroIngreso);
                    var stockConReserva = uow.StockRepository.GetStockUbicacionConReserva(ingreso.Linea.UbicacionEntrada);

                    #region INTEGRACION_DOCUMENTAL

                    var manejoDocumental = uow.ParametroRepository.GetParameter(ParamManager.MANEJO_DOCUMENTAL, new Dictionary<string, string> { [ParamManager.PARAM_EMPR] = $"{ParamManager.PARAM_EMPR}_{ingreso.Empresa}" });

                    if (manejoDocumental == "S")
                    {
                        var response = this._documentoService.DesreservarStock(uow, stockConReserva);

                        if (!response.Success)
                        {
                            throw new ValidationFailedException(response.ErrorMsg);
                        }
                    }
                    #endregion

                    foreach (var stock in stockConReserva)
                    {
                        stock.EliminarReserva();
                        stock.NumeroTransaccion = nuTransaccion;
                        uow.StockRepository.UpdateStock(stock);
                    }

                    uow.SaveChanges();

                    context.AddSuccessNotification("PRD175_Sec0_Success_DesreservaConfirmada");
                }
                catch (Exception ex)
                {
                    this._logger.Error(ex, ex.Message);
                    throw ex;
                }
            }
        }

        public virtual void ValidarConfirmarProduccion(Form form, FormSubmitContext query, IUnitOfWork uow, string nroIngreso)
        {
            IngresoWhiteBox ingreso = uow.ProduccionRepository.GetIngresoWhiteBox(nroIngreso);

            if (ingreso == null)
                throw new ValidationFailedException("General_Sec0_Error_IngresoNoExiste");

            if (ingreso.Linea == null)
                throw new ValidationFailedException("General_Sec0_Error_ProdnoExisteOFinalizada");

            if (ingreso.Situacion == SituacionDb.PRODUCCION_FINALIZADA)
                throw new ValidationFailedException("General_Sec0_Error_ProdnoExisteOFinalizada");

            if (ingreso.Pasadas.Count == 0)
                throw new ValidationFailedException("General_Sec0_Error_ProduccionNoIniciada");
        }

        public virtual void ValidarDescartarPasadasProduccion(Form form, FormSubmitContext context, IUnitOfWork uow, string nroIngreso)
        {
            IngresoWhiteBox ingreso = uow.ProduccionRepository.GetIngresoWhiteBox(nroIngreso);

            if (ingreso == null)
                throw new ValidationFailedException("General_Sec0_Error_IngresoNoExiste");

            if (ingreso.Linea == null)
                throw new ValidationFailedException("General_Sec0_Error_ProdnoExisteOFinalizada");

            if (ingreso.Situacion == SituacionDb.PRODUCCION_FINALIZADA)
                throw new ValidationFailedException("General_Sec0_Error_ProdnoExisteOFinalizada");

            //if (!ingreso.Consumidos.Any(c => !c.IsLineaConfirmada()) && !ingreso.Producidos.Any(p => !p.IsLineaConfirmada()))
            //    throw new ValidationFailedException("General_Sec0_Error_NoHayPasadasParaDescartar");
        }

        public virtual void ValidarCierreDeProduccion(Form form, FormSubmitContext context, IUnitOfWork uow, string nroIngreso)
        {
            IngresoWhiteBox ingreso = uow.ProduccionRepository.GetIngresoWhiteBox(nroIngreso);

            if (ingreso == null)
                throw new ValidationFailedException("General_Sec0_Error_IngresoNoExiste");

            if (ingreso.Linea == null)
                throw new ValidationFailedException("General_Sec0_Error_ProdnoExisteOFinalizada");

            if (ingreso.Situacion == SituacionDb.PRODUCCION_FINALIZADA)
                throw new ValidationFailedException("General_Sec0_Error_ProdnoExisteOFinalizada");

            if (!ingreso.Producidos.Any())
                throw new ValidationFailedException("General_Sec0_Error_ProdNoIniciada");

            if (ingreso.Consumidos.Any(c => c.NuTransaccion == null) || ingreso.Producidos.Any(p => p.NuTransaccion == null))
                throw new ValidationFailedException("General_Sec0_Error_ExisteProdPendienteActNoCierra");
        }

        #endregion
    }
}
