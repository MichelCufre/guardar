using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules.Recepcion;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.Impresiones;
using WIS.Domain.Recepcion.Enums;
using WIS.Domain.Services.Interfaces;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;

namespace WIS.Application.Controllers.REC
{
    public class REC171ImprimirEtiquetas : AppController
    {
        protected readonly IIdentityService _identity;
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly ILogger<REC171ImprimirEtiquetas> _logger;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IPrintingService _printingService;

        public REC171ImprimirEtiquetas(
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            ILogger<REC171ImprimirEtiquetas> logger,
            IPrintingService printingService,
            IFormValidationService formValidationService)
        {
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._logger = logger;
            this._formValidationService = formValidationService;
            this._printingService = printingService;
        }

        public override Form FormInitialize(Form form, FormInitializeContext query)
        {
            using var uow = _uowFactory.GetUnitOfWork();

            var impresion = uow.ImpresionRepository.ObtenerImpresoraUltimaImpresion(_identity.UserId, _identity.Predio);

            if (impresion != null && !string.IsNullOrEmpty(impresion.CodigoImpresora))
            {
                if (uow.ImpresoraRepository.ExisteImpresora(impresion.CodigoImpresora, impresion.Predio))
                {
                    var impresora = uow.ImpresoraRepository.GetImpresora(impresion.CodigoImpresora, impresion.Predio);
                    var lenguajeImpresion = uow.ImpresionRepository.GetLenguajeImpresion(impresora?.CodigoLenguajeImpresion);

                    form.GetField("impresora").Value = impresion.CodigoImpresora;
                    form.GetField("lenguaje").Value = lenguajeImpresion.Id;
                    form.GetField("desc_lenguaje").Value = lenguajeImpresion.Descripcion;
                }

                form.GetField("impresora").ReadOnly = false;
            }

            this.InicializarSelect(uow, form, query);

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = _uowFactory.GetUnitOfWork();

            Enum.TryParse(form.GetField("modalidad").Value, out ModalidadImpresion modalidad);

            int cantPrintGeneral = JsonConvert.DeserializeObject<int>(context.GetParameter("CANT_PRINT"));

            if (cantPrintGeneral < 1)
            {
                context.AddErrorNotification("REC171_frm1_Error_NoImprimirCant0");
                return form;
            }

            List<string> selectedKeys = JsonConvert.DeserializeObject<List<string>>(context.GetParameter("SELECTED_KEYS"));

            string predio = _identity.Predio;
            string estilo = form.GetField("estilo").Value;
            string lenguaje = form.GetField("lenguaje").Value;
            string impresora = form.GetField("impresora").Value;
            string tipoBarras = form.GetField("tipo_barras").Value;
            Dictionary<Producto, int> toPrint = new Dictionary<Producto, int>();

            foreach (string key in selectedKeys)
            {
                string[] decomposedKey = key.Split('$');
                var (nuAgenda, cdFaixa, nuIdentificador, cdProduto, cdEmpresa) = this.ParseKey(decomposedKey);

                Producto prod = uow.ProductoRepository.GetProducto(cdEmpresa, cdProduto);
                int cant = 0;

                switch (modalidad)
                {
                    case ModalidadImpresion.Unidades:
                        cant = Convert.ToInt32(decimal.Parse(decomposedKey[6], _identity.GetFormatProvider())); // QT_AGENDADO
                        break;
                    case ModalidadImpresion.Embalajes:
                        cant = Convert.ToInt32(decimal.Parse(decomposedKey[7], _identity.GetFormatProvider())); // QT_UNIDADES_BULTO
                        break;
                    case ModalidadImpresion.CampoVirtual:
                        cant = Convert.ToInt32(decimal.Parse(decomposedKey[0], _identity.GetFormatProvider()));
                        break;
                }

                toPrint.Add(prod, cant);
            }

            IEstiloTemplate estiloTemplate = new EstiloTemplate(uow, estilo);

            IImpresionDetalleBuildingStrategy strategy = new ProductoImpresionAgendaStrategy(
                estiloTemplate,
                tipoBarras,
                toPrint,
                uow,
                _printingService);

            ImpresionBuilder builder = new ImpresionBuilder(
                uow.ImpresoraRepository.GetImpresora(impresora, predio),
                strategy,
                _printingService);

            Impresion impresion = builder
                .GenerarCabezal(this._identity.UserId, predio)
                .GenerarDetalle()
                .Build();

            impresion.Referencia = $"REC171: Impresion de {cantPrintGeneral} etiquetas";

            try
            {
                uow.BeginTransaction();

                int numImpresion = uow.ImpresionRepository.Add(impresion);
                uow.SaveChanges();

                DetalleImpresion detalleImpresion = new DetalleImpresion()
                {
                    NumeroImpresion = numImpresion,
                    FechaProcesado = DateTime.Now,
                    Estado = _printingService.GetEstadoInicial(),
                };

                int cantidadRegistros = 1;

                foreach (DetalleImpresion detalle in impresion.Detalles.OrderBy(x => x.NumeroImpresion))
                {
                    detalleImpresion.Contenido = detalle.Contenido + "\n";
                    detalleImpresion.Registro = cantidadRegistros;

                    uow.ImpresionRepository.AddDetalleImpresion(detalleImpresion);

                    cantidadRegistros++;
                    detalleImpresion.Contenido = string.Empty;
                }

                uow.SaveChanges();
                uow.Commit();

                _printingService.SendToPrint(numImpresion);

                context.AddSuccessNotification("REC171_frm1_Success_ImpresionCorrecta");
            }
            catch (Exception e)
            {
                uow.Rollback();
                context.AddErrorNotification("REC171_frm1_Error_ImpresionNoCorrecta");
                throw;
            }

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return _formValidationService.Validate(new REC171ImpresionEtiquetasValidationModule(uow, _identity), form, context);
        }

        #region Metodos Auxiliares
        public virtual (int nuAgenda, decimal cdFaixa, string nuIdentificador, string cdProduto, int cdEmpresa) ParseKey(string[] parts)
        {
            int nuAgenda = Convert.ToInt32(parts[1]);
            decimal cdFaixa = decimal.Parse(parts[2], _identity.GetFormatProvider());
            string nuIdentificador = parts[3];
            string cdProduto = parts[4];
            int cdEmpresa = Convert.ToInt32(parts[5]);

            return (nuAgenda, cdFaixa, nuIdentificador, cdProduto, cdEmpresa);
        }

        public virtual void InicializarSelect(IUnitOfWork uow, Form form, FormInitializeContext query)
        {
            InicializarSelectImpresora(uow, form, query);
            InicializarSelectEstilo(uow, form, query);
            InicializarSelectBarras(uow, form, query);
            InicializarSelectModalidad(uow, form, query);
        }

        public virtual void InicializarSelectImpresora(IUnitOfWork uow, Form form, FormInitializeContext query)
        {
            var selectorImpresora = form.GetField("impresora");
            selectorImpresora.Options = new List<SelectOption>();

            if (!_identity.Predio.Equals(GeneralDb.PredioSinDefinir))
            {
                foreach (var impresora in uow.ImpresoraRepository.GetListaImpresorasPredio(_identity.Predio))
                {
                    selectorImpresora.Options.Add(new SelectOption(impresora.Id, $"{impresora.Id} - {impresora.Descripcion}"));
                }
            }
            else
                query.AddWarningNotification("REC171_frm1_Error_PredioNoSeleccionado");
        }

        public virtual void InicializarSelectEstilo(IUnitOfWork uow, Form form, FormInitializeContext query)
        {
            var selectorEstilo = form.GetField("estilo");
            selectorEstilo.Options = new List<SelectOption>();

            string impresora;
            if (string.IsNullOrEmpty(form.GetField("impresora").Value) || _identity.Predio == GeneralDb.PredioSinDefinir)
                return;
            else
                impresora = form.GetField("impresora").Value;

            Impresora imp = uow.ImpresoraRepository.GetImpresora(impresora, _identity.Predio);

            var listaEstilos = uow.EstiloEtiquetaRepository.GetEtiquetaEstilos(EstiloEtiquetaDb.Producto, imp.CodigoLenguajeImpresion);
            foreach (var estilo in listaEstilos)
            {
                selectorEstilo.Options.Add(new SelectOption(estilo.Id, $"{estilo.Id} - {estilo.Descripcion}"));
            }

        }

        public virtual void InicializarSelectBarras(IUnitOfWork uow, Form form, FormInitializeContext query)
        {
            var selectorBarras = form.GetField("tipo_barras");
            selectorBarras.Options = new List<SelectOption>();

            foreach (var tipoCdBarra in uow.ProductoCodigoBarraRepository.GetTiposCodigosBarras())
            {
                selectorBarras.Options.Add(new SelectOption(tipoCdBarra.Id.ToString(), $"{tipoCdBarra.Id} - {tipoCdBarra.Descripcion}"));
            }
        }

        public virtual void InicializarSelectModalidad(IUnitOfWork uow, Form form, FormInitializeContext query)
        {
            var selectorModalidad = form.GetField("modalidad");
            selectorModalidad.Options = new List<SelectOption>();

            foreach (var modalidad in ModalidadImpresionExtensions.StringValues())
            {
                selectorModalidad.Options.Add(new SelectOption(modalidad.Key, modalidad.Value));
            }
        }

        #endregion
    }
}
