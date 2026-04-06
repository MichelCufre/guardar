using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.Impresiones;
using WIS.Domain.Recepcion;
using WIS.Domain.Services.Interfaces;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;

namespace WIS.Application.Controllers.IMP
{
    public class IMP080ConsultaEtiquetas : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IPrintingService _printingService;
        protected readonly IBarcodeService _barcodeService;

        public IMP080ConsultaEtiquetas(IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IFormValidationService formValidationService,
            IPrintingService printingService,
            IBarcodeService barcodeService)
        {
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._formValidationService = formValidationService;
            this._printingService = printingService;
            this._barcodeService = barcodeService;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            form.GetField("lenguaje").ReadOnly = true;
            form.GetField("descripcionLenguaje").ReadOnly = true;

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
            string lenguaje = form.GetField("lenguaje").Value;
            string impresora = form.GetField("impresora").Value;

            int cantidadGenerar = int.Parse(form.GetField("cantGenerar").Value);

            var etiquetas = new List<EtiquetaLote> { new EtiquetaLote() { TipoEtiqueta = EstiloEtiquetaDb.Recepcion } };

            uow.CreateTransactionNumber(this._identity.Application);

            uow.CreateTransactionNumber(this._identity.Application);

            IEstiloTemplate estiloTemplate = new EstiloTemplate(uow, estilo);
            IImpresionDetalleBuildingStrategy strategy = new EtiquetaRecepcionImpresionStrategy(estiloTemplate, etiquetas, uow, _printingService, _barcodeService, cantidadGenerar);
            ImpresionBuilder builder = new ImpresionBuilder(uow.ImpresoraRepository.GetImpresora(impresora, predio), strategy, _printingService);

            Impresion impresion = builder.GenerarCabezal(this._identity.UserId, predio)
                .GenerarDetalle()
                .Build();

            impresion.Referencia = string.Format("Impresión de {0} etiquetas de {1}", cantidadGenerar, etiquetas.First().TipoEtiqueta);

            uow.BeginTransaction();

            int numImpresion = uow.ImpresionRepository.Add(impresion);
            uow.SaveChanges();

            int cantidadRegistros = 1;

            DetalleImpresion detalleImpresionInsercion = new DetalleImpresion()
            {
                NumeroImpresion = numImpresion,
                FechaProcesado = DateTime.Now,
                Estado = _printingService.GetEstadoInicial(),
            };

            foreach (var detalle in impresion.Detalles.OrderBy(s => s.NumeroImpresion))
            {
                detalleImpresionInsercion.Contenido += detalle.Contenido + "\n";

                //TODO: Ver donde agregar ese 2000 "MAX_LENGHT_STRING_PRINT"
                if (detalleImpresionInsercion.Contenido.Length > 2000)
                {
                    detalleImpresionInsercion.Registro = cantidadRegistros;

                    uow.ImpresionRepository.AddDetalleImpresion(detalleImpresionInsercion);

                    cantidadRegistros++;
                    detalleImpresionInsercion.Contenido = string.Empty;
                }
                else
                {
                    detalleImpresionInsercion.Registro = cantidadRegistros;

                    uow.ImpresionRepository.AddDetalleImpresion(detalleImpresionInsercion);

                    cantidadRegistros++;
                    detalleImpresionInsercion.Contenido = string.Empty;
                }
            }

            uow.ImpresionRepository.ActualizarImpresion(numImpresion, cantidadRegistros - 1);

            uow.SaveChanges();
            uow.Commit();

            _printingService.SendToPrint(numImpresion);

            context.AddSuccessNotification("IMP050_Sec0_error_ImpresoCorrectamete");

            return form;
        }
        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new ImpresionConsultaEtiquetasValidationModule(uow, this._identity), form, context);
        }

        public virtual void InicializarSelect(IUnitOfWork uow, Form form)
        {
            FormField selectorEstilo = form.GetField("estilo");
            FormField selectorImpresora = form.GetField("impresora");
            FormField selectorPredios = form.GetField("predio");

            selectorEstilo.Options = new List<SelectOption>();
            selectorImpresora.Options = new List<SelectOption>();

            //Estilo

            if (selectorImpresora.Value != "")
            {
                var lenguaje = form.GetField("lenguaje").Value;

                List<EtiquetaEstiloLenguaje> listaEstilos = uow.ImpresionRepository.GetEstiloByLenguaje(lenguaje, EstiloEtiquetaDb.Recepcion);

                foreach (var estilo in listaEstilos)
                {
                    selectorEstilo.Options.Add(new SelectOption(estilo.CodigoLabel, $"{estilo.CodigoLabel} - {estilo.Descripcion}"));
                }
            }
            else
            {
                List<EtiquetaEstiloLenguaje> listaEstilos = uow.ImpresionRepository.GetEstiloByTipo(EstiloEtiquetaDb.Recepcion);
                foreach (var estilo in listaEstilos)
                {
                    selectorEstilo.Options.Add(new SelectOption(estilo.CodigoLabel, $"{estilo.CodigoLabel} - {estilo.Descripcion}"));
                }
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
    }
}
