using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Components.Common.Select;
using WIS.Configuration;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Preparacion;
using WIS.Domain.General;
using WIS.Domain.Impresiones;
using WIS.Domain.Services.Interfaces;
using WIS.Exceptions;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;

namespace WIS.Application.Controllers.IMP
{
    public class IMP110ImpresionGeneralContenedores : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IOptions<DatabaseSettings> _databaseSettings;
        protected readonly ILogger<IMP110ImpresionGeneralContenedores> _logger;
        protected readonly IPrintingService _printingService;
        protected readonly IBarcodeService _barcodeService;

        public IMP110ImpresionGeneralContenedores(IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IFormValidationService formValidationService,
            IOptions<DatabaseSettings> databaseSettings,
            ILogger<IMP110ImpresionGeneralContenedores> logger,
            IPrintingService printingService,
            IBarcodeService barcodeService)
        {
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._formValidationService = formValidationService;
            this._databaseSettings = databaseSettings;
            this._logger = logger;
            this._printingService = printingService;
            _barcodeService = barcodeService;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            form.GetField("lenguaje").ReadOnly = true;
            form.GetField("descripcionLenguaje").ReadOnly = true;
            form.GetField("estilo").ReadOnly = true;

            if (context.Parameters.Count > 1)
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "contenedor")?.Value, out int idContenedor))
                    throw new MissingParameterException("General_Sec0_Error_Sin_ParametrosURI");

                if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "preparacion")?.Value, out int idPreparacion))
                    throw new MissingParameterException("General_Sec0_Error_Sin_ParametrosURI");

                Contenedor contenedor = uow.ContenedorRepository.GetContenedor(idPreparacion, idContenedor);

                form.GetField("tipoContenedor").Value = contenedor.TipoContenedor;

                FormField selectEstilos = form.GetField("estilo");
                selectEstilos.Options = new List<SelectOption>();

                List<EtiquetaEstiloLenguaje> listaEstilos = new List<EtiquetaEstiloLenguaje>();
                listaEstilos = uow.ImpresionRepository.GetEstiloByTipoContenedor("CON", contenedor.TipoContenedor);

                foreach (var estilo in listaEstilos)
                {
                    selectEstilos.Options.Add(new SelectOption(estilo.CodigoLabel, $"{estilo.CodigoLabel} - {estilo.Descripcion}"));
                }

                form.GetField("estilo").ReadOnly = false;
                form.GetField("tipoContenedor").ReadOnly = false;

            }

            Impresion impresion = uow.ImpresionRepository.ObtenerImpresoraUltimaImpresion(this._identity.UserId, this._identity.Predio);

            if (impresion != null && !string.IsNullOrEmpty(impresion.CodigoImpresora))
            {
                if (uow.ImpresoraRepository.ExisteImpresora(impresion.CodigoImpresora, impresion.Predio))//Por si se cambia el codigo de impresora
                {
                    Impresora impresora = uow.ImpresoraRepository.GetImpresora(impresion.CodigoImpresora, impresion.Predio);
                    var lenguajeImpresion = uow.ImpresionRepository.GetLenguajeImpresion(impresora?.CodigoLenguajeImpresion);

                    form.GetField("impresora").Value = impresion.CodigoImpresora;
                    form.GetField("lenguaje").Value = lenguajeImpresion?.Id;
                    form.GetField("descripcionLenguaje").Value = lenguajeImpresion?.Descripcion;
                }
                form.GetField("impresora").ReadOnly = false;
                form.GetField("predio").Value = impresion.Predio;
            }

            this.InicializarSelect(uow, form);
            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            string predio = form.GetField("predio").Value;
            string estilo = form.GetField("estilo").Value;
            int.TryParse(form.GetField("contenedor")?.Value, out int numContenedor);
            string tipoContenedor = form.GetField("tipoContenedor").Value;
            string impresora = form.GetField("impresora").Value;

            int.TryParse(form.GetField("cantGenerar").Value, out int cantidadGenerar);
            int.TryParse(form.GetField("numCopias").Value, out int cantidadCopias);

            bool retry = CrearImpresion(form, context, predio, estilo, numContenedor, tipoContenedor, impresora, cantidadGenerar, cantidadCopias);

            int maxRetries = 10;
            int retrySleep = 1000;
            for (int i = 0; retry && i < maxRetries; i++)
            {
                Thread.Sleep(retrySleep);
                context.Notifications.Clear();
                retry = CrearImpresion(form, context, predio, estilo, numContenedor, tipoContenedor, impresora, cantidadGenerar, cantidadCopias);
            }

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new ImpresionesYGeneradorContenedoresValidationModule(uow, this._identity), form, context);
        }

        public override List<SelectOption> FormSelectSearch(Form form, FormSelectSearchContext query)
        {
            switch (query.FieldId)
            {
                case "contenedor": return this.SearchContenedor(form, query);
                default: return new List<SelectOption>();
            }
        }

        #region Metodos Auxiliares
        public virtual List<SelectOption> SearchContenedor(Form form, FormSelectSearchContext FormQuery)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            ConsultaGeneralContenedoresQuery dbQuery = new ConsultaGeneralContenedoresQuery();

            uow.HandleQuery(dbQuery);

            var informacionContenedor = dbQuery.GetInfoContenedoresSearch(FormQuery.SearchValue);


            foreach (var info in informacionContenedor)
            {
                opciones.Add(new SelectOption(info.Numero.ToString(), $"{info.TipoContenedor} - {info.IdExterno} - {info.Numero} - {info.DescripcionContenedor}"));
            }

            return opciones;
        }

        public virtual void InicializarSelect(IUnitOfWork uow, Form form)
        {
            FormField selectorEstilo = form.GetField("estilo");
            FormField selectorImpresora = form.GetField("impresora");
            FormField selectorPredios = form.GetField("predio");
            FormField selectorTipoContenedor = form.GetField("tipoContenedor");

            selectorEstilo.Options = new List<SelectOption>();
            selectorImpresora.Options = new List<SelectOption>();
            selectorTipoContenedor.Options = new List<SelectOption>();

            if (selectorImpresora.Value != "")
            {
                var lenguaje = form.GetField("lenguaje").Value;

                List<EtiquetaEstiloLenguaje> listaEstilos = uow.ImpresionRepository.GetEstiloByLenguaje(lenguaje, EstiloEtiquetaDb.Contenedor);

                foreach (var estilo in listaEstilos)
                {
                    selectorEstilo.Options.Add(new SelectOption(estilo.CodigoLabel, $"{estilo.CodigoLabel} - {estilo.Descripcion}"));
                }
            }
            else
            {
                List<EtiquetaEstiloLenguaje> listaEstilos = uow.ImpresionRepository.GetEstiloByTipo(EstiloEtiquetaDb.Contenedor);
                foreach (var estilo in listaEstilos)
                {
                    selectorEstilo.Options.Add(new SelectOption(estilo.CodigoLabel, $"{estilo.CodigoLabel} - {estilo.Descripcion}"));
                }
            }

            //Tipo Contenedor
            var listaTipoContenedores = uow.ImpresionRepository.GetTiposContenedoresImpresion();
            foreach (var tipo in listaTipoContenedores)
            {
                selectorTipoContenedor.Options.Add(new SelectOption(tipo.Id, $"{tipo.Id} - {tipo.Descripcion}"));
            }

            //Predio
            List<Predio> userPredios = uow.PredioRepository.GetPrediosUsuario(_identity.UserId);

            foreach (var predio in userPredios)
            {
                selectorPredios.Options.Add(new SelectOption(predio.Numero, $"{predio.Numero} - {predio.Descripcion}"));
            }
            if (this._identity.Predio != GeneralDb.PredioSinDefinir && string.IsNullOrEmpty(form.GetField("predio").Value))
                form.GetField("predio").Value = this._identity.Predio;

            //Impresora
            List<Impresora> ListaImpresoras;
            if (this._identity.Predio.Equals(GeneralDb.PredioSinDefinir))
                ListaImpresoras = uow.ImpresoraRepository.GetListaImpresorasPredio(form.GetField("predio").Value);
            else
            {
                ListaImpresoras = uow.ImpresoraRepository.GetListaImpresorasPredio(this._identity.Predio);
                form.GetField("impresora").ReadOnly = false;
            }

            foreach (var impresora in ListaImpresoras)
            {
                selectorImpresora.Options.Add(new SelectOption(impresora.Id, $"{impresora.Id} - {impresora.Descripcion}"));
            }
        }

        public virtual bool CrearImpresion(Form form, FormSubmitContext context, string predio, string estilo, int numContenedor, string tipoContenedor, string impresora, int cantidadGenerar, int cantidadCopias)
        {
            var retry = false;
            var referenciaImpresion = string.Empty;
            var isolationLevel = System.Data.IsolationLevel.ReadCommitted;

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var contenedorUtilizar = uow.ContenedorRepository.GetContenedor(numContenedor);

                if (!string.IsNullOrEmpty(form.GetField("cantGenerar").Value))
                {
                    cantidadCopias = 1;
                    isolationLevel = uow.GetSnapshotIsolationLevel();

                    contenedorUtilizar = new Contenedor()
                    {
                        NumeroPreparacion = -1,
                        TipoContenedor = tipoContenedor,
                    };

                    referenciaImpresion = string.Format("Cant. Etiquetas generadas {0} / TP: {1}", cantidadGenerar.ToString(), tipoContenedor);
                }
                else if (!string.IsNullOrEmpty(form.GetField("numCopias").Value))
                {
                    referenciaImpresion = string.Format("{0} / TP: {1}", contenedorUtilizar.IdExterno, contenedorUtilizar.TipoContenedor);
                }

                IEstiloTemplate estiloTemplate = new EstiloTemplate(uow, estilo);
                IImpresionDetalleBuildingStrategy strategy = new ContenedorImpresionStrategy(estiloTemplate, uow, _printingService, contenedorUtilizar, _databaseSettings, _barcodeService, cantidadGenerar);
                var builder = new ImpresionBuilder(uow.ImpresoraRepository.GetImpresora(impresora, predio), strategy, _printingService);

                try
                {
                    uow.CreateTransactionNumber(this._identity.Application);
                    uow.BeginTransaction(isolationLevel);

                    var impresion = builder.GenerarCabezal(this._identity.UserId, predio)
                        .GenerarDetalle()
                        .Build();

                    impresion.Referencia = referenciaImpresion;

                    var numImpresion = uow.ImpresionRepository.Add(impresion);

                    uow.SaveChanges();

                    var iterador = 1;
                    var cantidadRegistros = 1;

                    var detalleImpresionInsercion = new DetalleImpresion()
                    {
                        NumeroImpresion = numImpresion,
                        FechaProcesado = DateTime.Now,
                        Estado = _printingService.GetEstadoInicial(),
                    };

                    foreach (var detalle in impresion.Detalles.OrderBy(s => s.NumeroImpresion))
                    {
                        for (int i = 0; i < cantidadCopias; i++)
                        {
                            detalleImpresionInsercion.Contenido += detalle.Contenido + "\n" + "\n";

                            //TODO: Ver donde agregar ese 2000 "MAX_LENGHT_STRING_PRINT"
                            if (detalleImpresionInsercion.Contenido.Length > 2000)
                            {
                                detalleImpresionInsercion.Registro = cantidadRegistros;

                                uow.ImpresionRepository.AddDetalleImpresion(detalleImpresionInsercion);

                                cantidadRegistros++;
                                detalleImpresionInsercion.Contenido = string.Empty;
                            }
                            else if (cantidadCopias == iterador)
                            {
                                detalleImpresionInsercion.Registro = cantidadRegistros;

                                uow.ImpresionRepository.AddDetalleImpresion(detalleImpresionInsercion);

                                cantidadRegistros++;
                                detalleImpresionInsercion.Contenido = string.Empty;
                            }

                            iterador++;
                        }

                        iterador = 1;
                    }

                    uow.ImpresionRepository.ActualizarImpresion(numImpresion, cantidadRegistros - 1);

                    uow.SaveChanges();
                    uow.Commit();

                    _printingService.SendToPrint(numImpresion);

                    context.AddSuccessNotification("IMP050_Sec0_error_ImpresoCorrectamete");
                }
                catch (ExpectedException ex)
                {
                    uow.Rollback();
                    context.AddErrorNotification(ex.Message);
                }
                catch (Exception ex)
                {
                    if (ex is Microsoft.EntityFrameworkCore.DbUpdateException
                        && ex.InnerException != null
                        && uow.IsSnapshotException(ex.InnerException))
                    {
                        retry = true;
                    }

                    uow.Rollback();
                    this._logger.LogError(ex, "IMP110FormSubmit");
                    context.AddErrorNotification("General_Sec0_Error_Operacion_Reintente");
                }
                finally
                {
                    uow.EndTransaction();
                }
            }

            return retry;
        }

        #endregion
    }
}
