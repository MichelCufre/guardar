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
using WIS.Domain.General;
using WIS.Domain.Impresiones;
using WIS.Domain.Services.Interfaces;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;

namespace WIS.Application.Controllers.IMP
{
    public class IMP060ImpresionProductos : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IFormValidationService _formValidationService;
        protected readonly EstiloEtiquetaMapper _estiloEtiquetaMapper;
        protected readonly IPrintingService _printingService;

        public IMP060ImpresionProductos(IIdentityService identity,
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

            List<string> ProductosParaImprimir = JsonConvert.DeserializeObject<List<string>>(context.Parameters.FirstOrDefault(x => x.Id == "ListaFilasSeleccionadas")?.Value);

            if (ProductosParaImprimir.Count > 0)
            {
                string predio = form.GetField("predio").Value;
                string estilo = form.GetField("estilo").Value;
                string lenguaje = form.GetField("lenguaje").Value;
                string impresora = form.GetField("impresora").Value;
                int cantidadCopias = int.Parse(form.GetField("numCopias").Value);
                string tipoCodigoBarras = form.GetField("tipoCodigoBarra").Value;

                List<Producto> productos = new List<Producto>();
                string clavesProductosReferencia = string.Empty;

                uow.CreateTransactionNumber(this._identity.Application);

                foreach (var prod in ProductosParaImprimir)
                {
                    var stringProductoClave = prod.Split('$');

                    string codigoProd = stringProductoClave[1];
                    int empresa = int.Parse(stringProductoClave[0]);

                    productos.Add(uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(empresa, codigoProd));

                    clavesProductosReferencia += codigoProd + " - ";
                }

                IEstiloTemplate estiloTemplate = new EstiloTemplate(uow, estilo);
                IImpresionDetalleBuildingStrategy strategy = new ProductoImpresionStrategy(estiloTemplate, tipoCodigoBarras, productos, uow, _printingService);
                ImpresionBuilder builder = new ImpresionBuilder(uow.ImpresoraRepository.GetImpresora(impresora, predio), strategy, _printingService);

                Impresion impresion = builder.GenerarCabezal(this._identity.UserId, predio)
                    .GenerarDetalle()
                    .Build();

                if (productos.Select(x => x.CodigoEmpresa).Distinct().Count() == 1)
                {
                    impresion.Referencia = string.Format("Cant. Prod: {0}", productos.Count());

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

                            detalleImpresionInsercion.Contenido += detalle.Contenido + "\n";

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
                else
                {
                    context.AddErrorNotification("General_Sec0_Error_ImpresionMasivaMultiempresa");
                }
            }

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new ImpresionProductoValidationModule(uow, this._identity), form, context);
        }

        public virtual void InicializarSelect(IUnitOfWork uow, Form form)
        {
            FormField selectorEstilo = form.GetField("estilo");
            FormField selectorImpresora = form.GetField("impresora");
            FormField selectorPredios = form.GetField("predio");
            FormField selectorTipoCodigoBarras = form.GetField("tipoCodigoBarra");

            selectorEstilo.Options = new List<SelectOption>();
            selectorImpresora.Options = new List<SelectOption>();
            selectorTipoCodigoBarras.Options = new List<SelectOption>();

            //Estilo

            if (selectorImpresora.Value != "")
            {
                var lenguaje = form.GetField("lenguaje").Value;

                List<EtiquetaEstiloLenguaje> listaEstilos = uow.ImpresionRepository.GetEstiloByLenguaje(lenguaje, EstiloEtiquetaDb.Producto);

                foreach (var estilo in listaEstilos)
                {
                    selectorEstilo.Options.Add(new SelectOption(estilo.CodigoLabel, $"{estilo.CodigoLabel} - {estilo.Descripcion}"));
                }
            }
            else
            {
                List<EtiquetaEstiloLenguaje> listaEstilos = uow.ImpresionRepository.GetEstiloByTipo(EstiloEtiquetaDb.Producto);
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

            //TipoCodigoDeBarras
            List<ProductoCodigoBarraTipo> listaTipoCodigoBarras = uow.ProductoCodigoBarraRepository.GetTiposCodigosBarras();
            foreach (var tipo in listaTipoCodigoBarras)
            {
                selectorTipoCodigoBarras.Options.Add(new SelectOption(tipo.Id.ToString(), $"{tipo.Id.ToString()} - {tipo.Descripcion}"));
            }

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
