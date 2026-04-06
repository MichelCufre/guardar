using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Expedicion;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.Expedicion.EXP110EmpaquetadoPicking.Dto;
using WIS.Domain.Services.Interfaces;
using WIS.Extension;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Security;
using WIS.TrafficOfficer;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules.Expedicion
{
    public class EXP110FormValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IIdentityService _securityService;
        protected readonly ConfiguracionInicial _confInicial;
        protected readonly bool _isSubmitting;
        protected readonly string _buttonId;
        protected readonly ContenedorDestinoData _contenedorDestinoData;
        protected readonly ITrafficOfficerService _concurrencyControl;
        protected readonly IParameterService _parameterService;
        protected readonly IPrintingService _printingService;
        protected readonly ITrackingService _trackingService;
        protected readonly IBarcodeService _barcodeService;
        protected readonly ICodigoMultidatoService _codigoMultidatoService;

        public EXP110FormValidationModule(IUnitOfWork uow,
            IIdentityService securityService,
            ConfiguracionInicial confInicial,
            ContenedorDestinoData contenedorDestinoData,
            bool isSubmitting,
            string buttonId,
            ITrafficOfficerService concurrencyControl,
            IParameterService parameterService,
            IPrintingService printingService,
            ITrackingService trackingService,
            IBarcodeService barcodeService,
            ICodigoMultidatoService codigoMultidatoService)
        {
            _uow = uow;
            _securityService = securityService;
            _confInicial = confInicial;
            _contenedorDestinoData = contenedorDestinoData;
            _isSubmitting = isSubmitting;
            _buttonId = buttonId;
            _concurrencyControl = concurrencyControl;
            _parameterService = parameterService;
            _printingService = printingService;
            _trackingService = trackingService;
            _barcodeService = barcodeService;
            _codigoMultidatoService = codigoMultidatoService;

            Schema = new FormValidationSchema
            {
                ["contenedorDestino"] = this.ValidateContenedorDestino,
                ["contenedorOrigen"] = this.ValidateContenedorOrigen,
                ["codigoBarraProducto"] = this.ValidateCodigoBarraProducto
            };
        }

        public virtual FormValidationGroup ValidateCodigoBarraProducto(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.Id != _buttonId && _isSubmitting)
                return new FormValidationGroup { };

            var isSummit = parameters.FirstOrDefault(x => x.Id == "CONT_ORIGEN_SUMMIT") == null ? false : true;

            if (!isSummit || string.IsNullOrEmpty(field.Value))
                return new FormValidationGroup { };

            var contenedorOrigen = !string.IsNullOrEmpty(parameters.FirstOrDefault(x => x.Id == "CONT_ORIGEN_DATA")?.Value) ?
                                    JsonConvert.DeserializeObject<DatosClientePedidoOriginal>(parameters.FirstOrDefault(x => x.Id == "CONT_ORIGEN_DATA")?.Value) : null;

            var nuContenedor = parameters.FirstOrDefault(x => x.Id == "AUX_CONT_ORIGEN_NU_CONTENEDOR")?.Value;
            var nuPreparacion = parameters.FirstOrDefault(x => x.Id == "AUX_CONT_ORIGEN_NU_PREPARACION")?.Value;

            var preparacion = _uow.PreparacionRepository.GetPreparacionPorNumero(nuPreparacion.ToNumber<int>());
            var empresa = contenedorOrigen != null ? contenedorOrigen.Empresa : preparacion.Empresa ?? -1;

            var fechaEntrega = form.GetField("fechaEntrega").Value;
            var pesoEmpaque = form.GetField("pesoEmpaque").Value;

            if (!string.IsNullOrEmpty(fechaEntrega))
                fechaEntrega = DateTime.Parse(fechaEntrega, _securityService.GetFormatProvider()).ToString(CultureInfo.InvariantCulture);

            if (!string.IsNullOrEmpty(pesoEmpaque))
                pesoEmpaque = decimal.Parse(pesoEmpaque, _securityService.GetFormatProvider()).ToString(CultureInfo.InvariantCulture);

            var codigoProducto = field.Value;
            var ais = _codigoMultidatoService.GetAIs(this._uow, "EXP110", codigoProducto, new Dictionary<string, string>
            {
                ["USERID"] = _securityService.UserId.ToString(),
                ["NU_PREDIO"] = _securityService.Predio,
                ["NU_PREPARACION"] = nuPreparacion,
                ["contenedorDestino"] = form.GetField("contenedorDestino").Value,
                ["pesoEmpaque"] = pesoEmpaque,
                ["descripcionEntrega"] = form.GetField("descripcionEntrega").Value,
                ["descripcionAnexo4"] = form.GetField("descripcionAnexo4").Value,
                ["fechaEntrega"] = fechaEntrega,
                ["numeroPedido"] = form.GetField("numeroPedido").Value,
                ["codigoCliente"] = form.GetField("codigoCliente").Value,
                ["codigoRuta"] = form.GetField("codigoRuta").Value,
                ["contenedorOrigen"] = form.GetField("contenedorOrigen").Value,
                ["tipoPedido"] = form.GetField("tipoPedido").Value,
                ["tipoExpedicion"] = form.GetField("tipoExpedicion").Value,
                ["CD_CAMPO"] = "codigoBarraProducto",
            }, empresa).GetAwaiter().GetResult()?.AIs;

            if (ais != null && ais.ContainsKey("codigoBarraProducto"))
            {
                codigoProducto = ais["codigoBarraProducto"].ToString();
            }

            codigoProducto = codigoProducto.ToUpper();

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new EXP110FormCodigoBarrasProducto(this._uow, codigoProducto,empresa,_contenedorDestinoData,nuContenedor.ToNumber<int>(),nuPreparacion.ToNumber<int>())
                }
            };
        }

        public virtual FormValidationGroup ValidateContenedorOrigen(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.Id != _buttonId && _isSubmitting)
                return new FormValidationGroup { };

            var isSummit = parameters.FirstOrDefault(x => x.Id == "CONT_ORIGEN_SUMMIT") == null ? false : true;

            if (isSummit || string.IsNullOrEmpty(field.Value))
                return new FormValidationGroup { };

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new StringMaxLengthValidationRule(field.Value, 16),
                   new EXP110FormContenedorOrigen(this._uow, field.Value, _securityService, this._contenedorDestinoData, field, _concurrencyControl, _parameterService, _printingService,_trackingService, _barcodeService )
                }
            };
        }

        public virtual FormValidationGroup ValidateContenedorDestino(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.Id != _buttonId && _isSubmitting)
                return new FormValidationGroup { };

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new StringMaxLengthValidationRule(field.Value,16),
                   new EXP110FormContenedorDestino(this._uow, field.Value, _securityService, _confInicial, _concurrencyControl, _parameterService, _barcodeService)
                }
            };
        }
    }
}
