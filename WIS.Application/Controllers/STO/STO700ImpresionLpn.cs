using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Impresiones;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.StockEntities;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;

namespace WIS.Application.Controllers.STO
{
    public class STO700ImpresionLpn : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IFormValidationService _formValidationService;
        protected readonly EstiloEtiquetaMapper _estiloEtiquetaMapper;
        protected readonly IPrintingService _printingService;
        protected readonly List<string> _prediosNoEspecificados = new List<string> { GeneralDb.PredioSinDefinir, GeneralDb.SinEspecificar, GeneralDb.PredioSinPredio };

        public STO700ImpresionLpn(IIdentityService identity, 
            IUnitOfWorkFactory uowFactory, 
            IFormValidationService formValidationService,
            IPrintingService printingService)
        {
            this._identity = identity;
            this._estiloEtiquetaMapper = new EstiloEtiquetaMapper();
            this._uowFactory = uowFactory;
            this._formValidationService = formValidationService;
            this._printingService = printingService;
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

            if (!_prediosNoEspecificados.Contains(this._identity.Predio))
            {
                form.GetField("predio").Disabled = true;
            }

            InicializarSelect(uow, form);

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            List<string> lpnsParaImprimir = JsonConvert.DeserializeObject<List<string>>(context.Parameters.FirstOrDefault(x => x.Id == "ListaFilasSeleccionadas")?.Value);

            if (lpnsParaImprimir.Count > 0)
            {
                string predio = form.GetField("predio").Value;
                string lenguaje = form.GetField("lenguaje").Value;
                string impresora = form.GetField("impresora").Value;
                int cantidadCopias = int.Parse(form.GetField("numCopias").Value);
                string estilo;
                string clavesLpnsReferencia = string.Empty;
                List<Lpn> lpns = new List<Lpn>();

                uow.CreateTransactionNumber(this._identity.Application);

                foreach (var nuLpns in lpnsParaImprimir)
                {
                    lpns.Add(uow.ManejoLpnRepository.GetLpn(long.Parse(nuLpns)));
                    clavesLpnsReferencia += nuLpns + " - ";
                }

                estilo = uow.ManejoLpnRepository.GetCodigoEstiloEtiquetaLPN(lpns[0].Tipo);

                IEstiloTemplate estiloTemplate = new EstiloTemplate(uow, estilo);
                IImpresionDetalleBuildingStrategy strategy = new LpnImpresionStrategy(estiloTemplate, lpns, uow, _printingService);
                ImpresionBuilder builder = new ImpresionBuilder(uow.ImpresoraRepository.GetImpresora(impresora, predio), strategy, _printingService);

                Impresion impresion = builder
                    .GenerarCabezal(this._identity.UserId, predio)
                    .GenerarDetalle()
                    .Build();

                impresion.Referencia = string.Format("Cant. Lpns: {0}", lpns.Count());

                uow.BeginTransaction();

                int numImpresion = uow.ImpresionRepository.Add(impresion);
                int iterador = 1;
                int cantidadRegistros = 1;

                uow.SaveChanges();

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
                        detalleImpresionInsercion.Contenido += detalle.Contenido + "\n";

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

            return this._formValidationService.Validate(new ImpresionLpnValidationModule(uow, this._identity.UserId, this._identity.Predio), form, context);
        }

        public virtual void InicializarSelect(IUnitOfWork uow, Form form)
        {
            var selectorImpresora = form.GetField("impresora");
            var selectorPredios = form.GetField("predio");

            selectorImpresora.Options = new List<SelectOption>();
            selectorPredios.Options = new List<SelectOption>();

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
