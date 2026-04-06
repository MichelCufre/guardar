using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.Impresiones;
using WIS.Domain.Services.Interfaces;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;

namespace WIS.Application.Controllers.IMP
{
    public class IMP050ImpresionUbicaciones : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IPrintingService _printingService;

        public IMP050ImpresionUbicaciones(IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IFormValidationService formValidationService,
            IPrintingService printingService)
        {
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._formValidationService = formValidationService;
            this._printingService = printingService;
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

            var ubicacionesParaImprimir = JsonConvert.DeserializeObject<List<string>>(context.Parameters.FirstOrDefault(x => x.Id == "ListaFilasSeleccionadas")?.Value);
            var configuracion = uow.UbicacionRepository.GetUbicacionConfiguracion();
            var predio = form.GetField("predio").Value;
            var estilo = form.GetField("estilo").Value;
            var lenguaje = form.GetField("lenguaje").Value;
            var impresora = form.GetField("impresora").Value;

            var ubicaciones = new List<Ubicacion>();
            var clavesUbicacionesReferencia = string.Empty;

            uow.CreateTransactionNumber(this._identity.Application);

            foreach (var ubic in ubicacionesParaImprimir)
            {
                ubicaciones.Add(uow.UbicacionRepository.GetUbicacion(ubic));

                if (ubic == ubicacionesParaImprimir.Last())
                    clavesUbicacionesReferencia += ubic;
                else
                    clavesUbicacionesReferencia += ubic + " - ";
            }

            IEstiloTemplate estiloTemplate = new EstiloTemplate(uow, estilo);
            IImpresionDetalleBuildingStrategy strategy = new UbicacionImpresionStrategy(estiloTemplate, _printingService, configuracion, ubicaciones);
            ImpresionBuilder builder = new ImpresionBuilder(uow.ImpresoraRepository.GetImpresora(impresora, predio), strategy, _printingService);

            Impresion impresion = builder.GenerarCabezal(this._identity.UserId, predio)
                .GenerarDetalle()
                .Build();

            impresion.Referencia = string.Format("Cant. Ubic: {0}", ubicaciones.Count());

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
            List<Ubicacion> ubicaciones = new List<Ubicacion>();
            List<string> ubicacionesParaImprimir = JsonConvert.DeserializeObject<List<string>>(context.Parameters.FirstOrDefault(x => x.Id == "ListaFilasSeleccionadas")?.Value);

            foreach (var ubic in ubicacionesParaImprimir)
            {
                ubicaciones.Add(uow.UbicacionRepository.GetUbicacion(ubic));
            }

            return this._formValidationService.Validate(new ImpresionesUbicacionValidationModule(uow, this._identity.UserId, this._identity.Predio, ubicaciones), form, context);
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

                List<EtiquetaEstiloLenguaje> listaEstilos = uow.ImpresionRepository.GetEstiloByLenguaje(lenguaje, EstiloEtiquetaDb.Ubicacion);

                foreach (var estilo in listaEstilos)
                {
                    selectorEstilo.Options.Add(new SelectOption(estilo.CodigoLabel, $"{estilo.CodigoLabel} - {estilo.Descripcion}"));
                }
            }
            else
            {
                List<EtiquetaEstiloLenguaje> listaEstilos = uow.ImpresionRepository.GetEstiloByTipo(EstiloEtiquetaDb.Ubicacion);
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
            List<Impresora> listaImpresoras;
            if (this._identity.Predio.Equals(GeneralDb.PredioSinDefinir))
                listaImpresoras = uow.ImpresoraRepository.GetListaImpresorasPredio(form.GetField("predio").Value);
            else
            {
                listaImpresoras = uow.ImpresoraRepository.GetListaImpresorasPredio(this._identity.Predio);
                form.GetField("impresora").ReadOnly = false;
            }

            foreach (var impresora in listaImpresoras)
            {
                selectorImpresora.Options.Add(new SelectOption(impresora.Id, $"{impresora.Id} - {impresora.Descripcion}"));
            }
        }
    }
}
