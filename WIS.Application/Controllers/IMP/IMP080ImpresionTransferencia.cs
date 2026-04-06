using Newtonsoft.Json;
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
    public class IMP080ImpresionTransferencia : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IPrintingService _printingService;
        protected readonly IBarcodeService _barcodeService;

        public IMP080ImpresionTransferencia(IIdentityService identity,
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

            int.TryParse(form.GetField("numCopias").Value, out int cantidadCopias);

            Impresion impresion = new Impresion();
            impresion.Estilo = EstiloEtiquetaDb.Transferencia;

            List<EtiquetaTransferencia> transferencias = new List<EtiquetaTransferencia>();

            uow.CreateTransactionNumber(this._identity.Application);

            //Distincion entre generar etiquetas o reimprimir etiquetas
            //TODO: MEJORAR ESTOS IF, FUNCIONAL EN ESTE MOMENTO
            if (context.Parameters.Any(x => x.Id == "reimprimirEtiquetas"))
            {
                List<string> etiquetasTransferenciaReimprimir = JsonConvert.DeserializeObject<List<string>>(context.Parameters.FirstOrDefault(x => x.Id == "ListaFilasSeleccionadas")?.Value);

                transferencias = new List<EtiquetaTransferencia>();

                string clavesReferenciaTransferencias = string.Empty;

                foreach (var transfe in etiquetasTransferenciaReimprimir)
                {
                    string[] stringTransferenciaClave = transfe.Split('$');

                    decimal numeroEtiquetaTransferencia = decimal.Parse(stringTransferenciaClave[0], _identity.GetFormatProvider());

                    transferencias.Add(uow.EtiquetaTransferenciaRepository.GetEtiquetaTransferencia(numeroEtiquetaTransferencia));

                    if (transfe == etiquetasTransferenciaReimprimir.Last())
                        clavesReferenciaTransferencias += numeroEtiquetaTransferencia.ToString();
                    else
                        clavesReferenciaTransferencias += numeroEtiquetaTransferencia.ToString() + " - ";
                }

                IEstiloTemplate estiloTemplate = new EstiloTemplate(uow, estilo);
                IImpresionDetalleBuildingStrategy strategy = new TransferenciaImpresionStrategy(estiloTemplate, transferencias, uow, _printingService, _barcodeService);
                ImpresionBuilder builder = new ImpresionBuilder(uow.ImpresoraRepository.GetImpresora(impresora, predio), strategy, _printingService);

                impresion = builder.GenerarCabezal(this._identity.UserId, predio)
                .GenerarDetalle()
                .Build();

                if (transferencias.Count == 1)
                    impresion.Referencia = string.Format("Impresión de etiqueta de {0} número {1}", EstiloEtiquetaDb.Transferencia, transferencias.First().NumeroEtiqueta);
                else
                    impresion.Referencia = string.Format("Impresión de {0} etiquetas, números: {1}", transferencias.Count.ToString(), clavesReferenciaTransferencias);
            }
            else
            {
                cantidadCopias = 1;
                //genera nuevos
                int cantidadGenerar = int.Parse(form.GetField("cantGenerar").Value);

                transferencias = new List<EtiquetaTransferencia> { new EtiquetaTransferencia() };

                IEstiloTemplate estiloTemplate = new EstiloTemplate(uow, estilo);
                IImpresionDetalleBuildingStrategy strategy = new TransferenciaImpresionStrategy(estiloTemplate, transferencias, uow, _printingService, _barcodeService, cantidadGenerar);
                ImpresionBuilder builder = new ImpresionBuilder(uow.ImpresoraRepository.GetImpresora(impresora, predio), strategy, _printingService);

                impresion = builder.GenerarCabezal(this._identity.UserId, predio)
                .GenerarDetalle()
                .Build();

                impresion.Referencia = string.Format("Impresión de {0} etiquetas de {1}", cantidadGenerar, EstiloEtiquetaDb.Transferencia);

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

            //FALTARIA MARCAR IMPRESION SI ES ENVASE, CONSULTARLE A GONZA

            uow.SaveChanges();
            uow.Commit();

            _printingService.SendToPrint(numImpresion);

            context.AddSuccessNotification("IMP050_Sec0_error_ImpresoCorrectamete");

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new ImpresionTransferenciaValidationModule(uow, this._identity), form, context);
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

                List<EtiquetaEstiloLenguaje> listaEstilos = uow.ImpresionRepository.GetEstiloByLenguaje(lenguaje, EstiloEtiquetaDb.Transferencia);

                foreach (var estilo in listaEstilos)
                {
                    selectorEstilo.Options.Add(new SelectOption(estilo.CodigoLabel, $"{estilo.CodigoLabel} - {estilo.Descripcion}"));
                }
            }
            else
            {
                List<EtiquetaEstiloLenguaje> listaEstilos = uow.ImpresionRepository.GetEstiloByTipo(EstiloEtiquetaDb.Transferencia);
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
