using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Impresion;
using WIS.Domain.Impresiones;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.StockEntities;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;

namespace WIS.Application.Controllers.IMP
{
    public class IMP200ImpresionUT : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IBarcodeService _barcodeService;
        protected readonly IPrintingService _printingService;
        protected readonly IFormValidationService _formValidationService;

        public IMP200ImpresionUT(IIdentityService identity,
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

            var impresion = uow.ImpresionRepository.ObtenerImpresoraUltimaImpresion(this._identity.UserId, this._identity.Predio);

            if (impresion != null && !string.IsNullOrEmpty(impresion.CodigoImpresora))
            {
                if (uow.ImpresoraRepository.ExisteImpresora(impresion.CodigoImpresora, impresion.Predio))//Por si se cambia el codigo de impresora
                {
                    var impresora = uow.ImpresoraRepository.GetImpresora(impresion.CodigoImpresora, impresion.Predio);
                    var lenguajeImpresion = uow.ImpresionRepository.GetLenguajeImpresion(impresora?.CodigoLenguajeImpresion);

                    form.GetField("impresora").Value = impresion.CodigoImpresora;
                    form.GetField("lenguaje").Value = lenguajeImpresion?.Id;
                    form.GetField("descripcionLenguaje").Value = lenguajeImpresion?.Descripcion;
                }
                form.GetField("impresora").ReadOnly = false;
                form.GetField("predio").Value = impresion.Predio;
            }

            InicializarSelect(uow, form);
            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var predio = form.GetField("predio").Value;
            var estilo = form.GetField("estilo").Value;
            var lenguaje = form.GetField("lenguaje").Value;
            var impresora = form.GetField("impresora").Value;

            int.TryParse(form.GetField("numCopias").Value, out int cantidadCopias);

            var impresion = new Impresion
            {
                Estilo = EstiloEtiquetaDb.UnidadTransporte
            };

            var uts = new List<UnidadTransporte>();
            var reimprimir = bool.Parse(context.GetParameter("reimprimirEtiquetas") ?? "false");

            uow.CreateTransactionNumber(this._identity.Application);

            if (reimprimir)
            {
                var nroUTs = JsonConvert.DeserializeObject<List<int>>(context.GetParameter("selectedKeys"));

                var referenciaImpresion = string.Empty;

                foreach (var nroUT in nroUTs)
                {
                    var ut = uow.UnidadTransporteRepository.GetUnidadTransporte(nroUT);
                    uts.Add(ut);

                    if (nroUT == nroUTs.Last())
                        referenciaImpresion += ut.NumeroExternoUnidad;
                    else
                        referenciaImpresion += ut.NumeroExternoUnidad + " - ";
                }

                IEstiloTemplate estiloTemplate = new EstiloTemplate(uow, estilo);
                IImpresionDetalleBuildingStrategy strategy = new UnidadTransporteImpresionStrategy(estiloTemplate, uts, uow, _printingService, _barcodeService);
                ImpresionBuilder builder = new ImpresionBuilder(uow.ImpresoraRepository.GetImpresora(impresora, predio), strategy, _printingService);

                impresion = builder.GenerarCabezal(this._identity.UserId, predio)
                .GenerarDetalle()
                .Build();

                if (uts.Count == 1)
                    impresion.Referencia = string.Format("Impresión de etiqueta de {0} número externo {1}", EstiloEtiquetaDb.UnidadTransporte, uts.First().NumeroExternoUnidad);
                else
                    impresion.Referencia = string.Format("Impresión de {0} etiquetas, números externos: {1}", uts.Count.ToString(), referenciaImpresion);

            }
            else
            {
                //genera nuevos
                cantidadCopias = 1;
                var cantidadGenerar = int.Parse(form.GetField("cantGenerar").Value);

                uts = new List<UnidadTransporte> { new UnidadTransporte() };

                IEstiloTemplate estiloTemplate = new EstiloTemplate(uow, estilo);
                IImpresionDetalleBuildingStrategy strategy = new UnidadTransporteImpresionStrategy(estiloTemplate, uts, uow, _printingService, _barcodeService, cantidadGenerar);
                ImpresionBuilder builder = new ImpresionBuilder(uow.ImpresoraRepository.GetImpresora(impresora, predio), strategy, _printingService);

                impresion = builder.GenerarCabezal(this._identity.UserId, predio)
                .GenerarDetalle()
                .Build();

                impresion.Referencia = string.Format("Impresión de {0} etiquetas de {1}", cantidadGenerar, EstiloEtiquetaDb.UnidadTransporte);
            }

            uow.EndTransaction();
            uow.BeginTransaction();

            int numImpresion = uow.ImpresionRepository.Add(impresion);
            uow.SaveChanges();

            int iterador = 1;
            int cantidadRegistros = 1;

            DetalleImpresion detalleImpresionInsercion = new DetalleImpresion()
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

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var reimprimir = bool.Parse(context.GetParameter("reimprimirEtiquetas") ?? "false");
            return this._formValidationService.Validate(new ImpresionUTValidationModule(uow, this._identity, reimprimir), form, context);
        }

        public virtual void InicializarSelect(IUnitOfWork uow, Form form)
        {
            var selectorEstilo = form.GetField("estilo");
            var selectorImpresora = form.GetField("impresora");
            var selectorPredios = form.GetField("predio");

            selectorEstilo.Options = new List<SelectOption>();
            selectorImpresora.Options = new List<SelectOption>();


            //Estilo

            if (selectorImpresora.Value != "")
            {
                var lenguaje = form.GetField("lenguaje").Value;

                List<EtiquetaEstiloLenguaje> listaEstilos = uow.ImpresionRepository.GetEstiloByLenguaje(lenguaje, EstiloEtiquetaDb.UnidadTransporte);

                foreach (var estilo in listaEstilos)
                {
                    selectorEstilo.Options.Add(new SelectOption(estilo.CodigoLabel, $"{estilo.CodigoLabel} - {estilo.Descripcion}"));
                }
            }
            else
            {
                var dbQuery = new EstilosEtiquetaQuery();
                uow.HandleQuery(dbQuery);

                var estilos = uow.EstiloEtiquetaRepository.GetEtiquetaEstilos(EstiloEtiquetaDb.UnidadTransporte);
                foreach (var estilo in estilos)
                {
                    selectorEstilo.Options.Add(new SelectOption(estilo.Id, $"{estilo.Id} - {estilo.Descripcion}"));
                }

                if (estilos.Count == 1)
                    form.GetField("estilo").Value = estilos.FirstOrDefault().Id;
            }


            //Predio
            var userPredios = uow.PredioRepository.GetPrediosUsuario(_identity.UserId);
            foreach (var predio in userPredios)
            {
                selectorPredios.Options.Add(new SelectOption(predio.Numero, $"{predio.Numero} - {predio.Descripcion}"));
            }

            if (this._identity.Predio != GeneralDb.PredioSinDefinir && string.IsNullOrEmpty(form.GetField("predio").Value))
                form.GetField("predio").Value = this._identity.Predio;

            //Impresora
            List<Impresora> impresoras;

            if (this._identity.Predio.Equals(GeneralDb.PredioSinDefinir))
                impresoras = uow.ImpresoraRepository.GetListaImpresorasPredio(form.GetField("predio").Value);
            else
            {
                impresoras = uow.ImpresoraRepository.GetListaImpresorasPredio(this._identity.Predio);
                form.GetField("impresora").ReadOnly = false;
            }

            foreach (var impresora in impresoras)
            {
                selectorImpresora.Options.Add(new SelectOption(impresora.Id, $"{impresora.Id} - {impresora.Descripcion}"));
            }
        }
    }
}
