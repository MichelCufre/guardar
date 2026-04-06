using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Components.Common.Select;
using WIS.Configuration;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.Impresiones;
using WIS.Domain.Services.Interfaces;
using WIS.Exceptions;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;

namespace WIS.Application.Controllers.IMP
{
    public class IMP110ReimpresionGeneralContenedores : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IOptions<DatabaseSettings> _databaseSettings;
        protected readonly IPrintingService _printingService;
        protected readonly IBarcodeService _barcodeService;

        public IMP110ReimpresionGeneralContenedores(IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IFormValidationService formValidationService,
            IOptions<DatabaseSettings> databaseSettings,
            IPrintingService printingService,
            IBarcodeService barcodeService)
        {
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._formValidationService = formValidationService;
            this._databaseSettings = databaseSettings;
            this._printingService = printingService;
            this._barcodeService = barcodeService;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (context.Parameters.Count > 1)
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "contenedor")?.Value, out int idContenedor))
                    throw new MissingParameterException("General_Sec0_Error_Sin_ParametrosURI");

                if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "preparacion")?.Value, out int idPreparacion))
                    throw new MissingParameterException("General_Sec0_Error_Sin_ParametrosURI");

                Contenedor contenedor = uow.ContenedorRepository.GetContenedor(idPreparacion, idContenedor);

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
                this.InicializarSelect(uow, form, contenedor);
            }

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (context.Parameters.Count > 1)
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "contenedor")?.Value, out int idContenedor))
                    throw new MissingParameterException("General_Sec0_Error_Sin_ParametrosURI");

                if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "preparacion")?.Value, out int idPreparacion))
                    throw new MissingParameterException("General_Sec0_Error_Sin_ParametrosURI");

                var contenedor = uow.ContenedorRepository.GetContenedor(idPreparacion, idContenedor);

                var predio = form.GetField("predio").Value;
                var estilo = form.GetField("estilo").Value;
                var tipoContenedor = form.GetField("tipoContenedor").Value;
                var lenguaje = form.GetField("lenguaje").Value;
                var impresora = form.GetField("impresora").Value;
                var cantidadCopias = int.Parse(form.GetField("numCopias").Value);

                uow.CreateTransactionNumber("Reimpresion de contenedores");
                var transaccion = uow.GetTransactionNumber();

                IEstiloTemplate estiloTemplate = new EstiloTemplate(uow, estilo);
                IImpresionDetalleBuildingStrategy strategy = new ContenedorImpresionStrategy(estiloTemplate, uow, _printingService, contenedor, _databaseSettings, _barcodeService);
                var builder = new ImpresionBuilder(uow.ImpresoraRepository.GetImpresora(impresora, predio), strategy, _printingService);

                var impresion = builder.GenerarCabezal(this._identity.UserId, predio)
                    .GenerarDetalle()
                    .Build();

                if (string.IsNullOrEmpty(impresion.Referencia))
                    impresion.Referencia = string.Format("{0} / TP: {1}", contenedor.IdExterno, contenedor.TipoContenedor);

                uow.ContenedorRepository.MarcarImpresionContenedor(contenedor, predio, transaccion, true);

                uow.BeginTransaction();

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

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new ImpresionesGeneralesContenedoresValidationModule(uow, this._identity), form, context);
        }

        public virtual void InicializarSelect(IUnitOfWork uow, Form form, Contenedor contenedor = null)
        {
            FormField selectorEstilo = form.GetField("estilo");
            FormField selectorImpresora = form.GetField("impresora");
            FormField selectorPredios = form.GetField("predio");
            FormField selectorTipoContenedor = form.GetField("tipoContenedor");

            selectorEstilo.Options = new List<SelectOption>();
            selectorImpresora.Options = new List<SelectOption>();
            selectorTipoContenedor.Options = new List<SelectOption>();

            form.GetField("lenguaje").ReadOnly = true;
            form.GetField("descripcionLenguaje").ReadOnly = true;
            form.GetField("estilo").ReadOnly = true;


            List<EtiquetaEstiloLenguaje> listaEstilos = new List<EtiquetaEstiloLenguaje>();

            if (contenedor != null)
                listaEstilos = uow.ImpresionRepository.GetEstiloByTipoContenedor("CON", contenedor.TipoContenedor);
            else
                listaEstilos = uow.ImpresionRepository.GetEstiloByTipo("CON");

            foreach (var estilo in listaEstilos)
            {
                selectorEstilo.Options.Add(new SelectOption(estilo.CodigoLabel, $"{estilo.CodigoLabel} - {estilo.Descripcion}"));
            }

            //Tipo Contenedor
            TipoContenedor tipoContenedor = uow.ContenedorRepository.GetTipoContenedor(contenedor.TipoContenedor);

            selectorTipoContenedor.Options.Add(new SelectOption(tipoContenedor.Id, $"{tipoContenedor.Id} - {tipoContenedor.Descripcion}"));
            selectorTipoContenedor.ReadOnly = true;

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

            if (contenedor != null)
            {
                form.GetField("tipoContenedor").Value = contenedor.TipoContenedor;
                form.GetField("estilo").ReadOnly = false;
            }
        }
    }
}
