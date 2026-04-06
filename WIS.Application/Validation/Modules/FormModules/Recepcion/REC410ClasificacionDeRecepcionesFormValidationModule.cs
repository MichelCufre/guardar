using System;
using System.Collections.Generic;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Recepcion;
using WIS.Application.Validation.Rules.General;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.General.Enums;
using WIS.Domain.Services.Interfaces;
using WIS.Extension;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Security;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules.Recepcion
{
    public class REC410ClasificacionDeRecepcionesFormValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IBarcodeService _barcodeService;
        protected readonly IParameterService _parameterService;
        protected readonly IIdentityService _identity;
        protected readonly ICodigoMultidatoService _codigoMultidatoService;

        public REC410ClasificacionDeRecepcionesFormValidationModule(
            IUnitOfWork uow,
            IBarcodeService barcodeService,
            IParameterService parameterService,
            IIdentityService identity,
            ICodigoMultidatoService codigoMultidatoService)
        {
            this._uow = uow;
            this._barcodeService = barcodeService;
            this._parameterService = parameterService;
            this._identity = identity;
            this._codigoMultidatoService = codigoMultidatoService;

            this.Schema = new FormValidationSchema
            {
                ["estacion"] = this.ValidateEstacion,
                ["etiqueta"] = this.ValidateEtiqueta,
                ["codigoProducto"] = this.ValidateCodigoProducto,
                ["lote"] = this.ValidateLote,
                ["vencimiento"] = this.ValidateVencimiento,
                ["cantidad"] = this.ValidateCantidadProducto,
            };
        }


        public virtual FormValidationGroup ValidateEstacion(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (parameters.Find(x => x.Id == "Focus").Value != field.Id)
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new ExisteEstacionClasificacionValidationRule(this._uow, field.Value),
                },
            };
        }

        public virtual FormValidationGroup ValidateEtiqueta(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (parameters.Find(x => x.Id == "Focus").Value != field.Id || string.IsNullOrEmpty(field.Value))
                return null;

            var estacion = int.Parse(form.GetField("estacion").Value);

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new EtiquetaLoteEnUsoValidationRule(this._uow, this._barcodeService, field.Value),
                    new EtiquetaLoteClasificacionValidationRule(this._uow, this._barcodeService, field.Value, estacion),
                },
            };
        }

        public virtual FormValidationGroup ValidateCodigoProducto(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (parameters.Find(x => x.Id == "Focus").Value != field.Id || string.IsNullOrEmpty(field.Value))
                return null;

            var estacion = int.Parse(form.GetField("estacion").Value);
            var nuExterno = parameters.Find(x => x.Id == "etiqueta").Value;
            var tpEtiqueta = parameters.Find(x => x.Id == "tipoEtiqueta").Value;

            if (string.IsNullOrEmpty(nuExterno))
                return null;

            var etiqueta = this._uow.EtiquetaLoteRepository.GetEtiquetaLoteActiva(tpEtiqueta, nuExterno);
            var agenda = this._uow.AgendaRepository.GetAgenda(etiqueta.NumeroAgenda);
            var cdBarrasProducto = field.Value = field.Value.ToUpper();
            var ais = _codigoMultidatoService.GetAIs(this._uow, "REC410", cdBarrasProducto, new Dictionary<string, string>
            {
                ["USERID"] = _identity.UserId.ToString(),
                ["NU_PREDIO"] = _identity.Predio,
                ["estacion"] = form.GetField("estacion").Value,
                ["reabastecer"] = form.GetField("reabastecer").Value,
                ["ignorarStock"] = form.GetField("ignorarStock").Value,
                ["etiqueta"] = form.GetField("etiqueta").Value,
                ["tipoEtiqueta"] = form.GetField("tipoEtiqueta").Value,
                ["CD_CAMPO"] = "codigoProducto"
            }, agenda.IdEmpresa).GetAwaiter().GetResult()?.AIs;

            if (ais != null && ais.ContainsKey("codigoProducto"))
            {
                cdBarrasProducto = ais["codigoProducto"].ToString();
                field.IsMultidataCodeReading = true;
            }

            cdBarrasProducto = cdBarrasProducto.ToUpper();

            var rules = new List<IValidationRule>
            {
                new ProductoExisteCodigoValidationRule(this._uow, agenda.IdEmpresa, cdBarrasProducto),
                new ProductoAuditadoClasificacionValidationRule(this._uow, this._parameterService, etiqueta.Numero, etiqueta.NumeroAgenda, cdBarrasProducto),
                new EtiquetaLoteEnEstacionClasificacionValidationRule(this._uow, etiqueta.Numero, estacion),
            };

            if (ais != null)
            {
                var cdProducto = this._uow.CodigoBarrasRepository.GetCodigoBarras(cdBarrasProducto, agenda.IdEmpresa)?.Producto;
                if (cdProducto != null)
                {
                    var producto = _uow.ProductoRepository.GetProducto(agenda.IdEmpresa, cdProducto);

                    if (ais.ContainsKey("lote") && !producto.IsIdentifiedByProducto())
                    {
                        var lote = ais["lote"].ToString();
                        var loteValidation = ValidateLote(cdProducto, lote, form, parameters);
                        rules.AddRange(loteValidation.Rules);
                    }

                    if (ais.ContainsKey("vencimiento") && producto.ManejaFechaVencimiento())
                    {
                        var vencimiento = DateTimeExtension.ToIsoString((DateTime)ais["vencimiento"]);
                        var vencimientoValidation = ValidateVencimiento(cdProducto, vencimiento, form, parameters);
                        rules.AddRange(vencimientoValidation.Rules);
                    }
                }
            }

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
            };
        }

        public virtual FormValidationGroup ValidateLote(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (parameters.Find(x => x.Id == "Focus").Value != field.Id || string.IsNullOrEmpty(field.Value))
                return null;

            var codigoProducto = form.GetField("codigoProducto").Value;
            field.Value = field.Value.ToUpper();
            return ValidateLote(codigoProducto, field.Value, form, parameters);
        }

        public virtual FormValidationGroup ValidateLote(string codigoProducto, string lote, Form form, List<ComponentParameter> parameters)
        {
            var estacion = int.Parse(form.GetField("estacion").Value);
            var nuExterno = parameters.Find(x => x.Id == "etiqueta").Value;
            var tpEtiqueta = parameters.Find(x => x.Id == "tipoEtiqueta").Value;

            if (string.IsNullOrEmpty(nuExterno))
                return null;

            var etiqueta = this._uow.EtiquetaLoteRepository.GetEtiquetaLoteActiva(tpEtiqueta, nuExterno);
            var agenda = this._uow.AgendaRepository.GetAgenda(etiqueta.NumeroAgenda);

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new LoteAuditadoClasificacionValidationRule(this._uow, this._parameterService, etiqueta.Numero, etiqueta.NumeroAgenda, codigoProducto, lote.ToUpper()),
                    new EtiquetaLoteEnEstacionClasificacionValidationRule(this._uow, etiqueta.Numero, estacion),
                },
            };
        }

        public virtual FormValidationGroup ValidateVencimiento(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (parameters.Find(x => x.Id == "Focus").Value != field.Id || string.IsNullOrEmpty(field.Value))
                return null;

            var codigoProducto = form.GetField("codigoProducto").Value;

            return ValidateVencimiento(codigoProducto, field.Value, form, parameters);
        }

        public virtual FormValidationGroup ValidateVencimiento(string codigoProducto, string vencimiento, Form form, List<ComponentParameter> parameters)
        {
            DateTimeExtension.TryParseFromIso(vencimiento, out DateTime? parsedDate);

            var estacion = int.Parse(form.GetField("estacion").Value);
            var nuExterno = parameters.Find(x => x.Id == "etiqueta").Value;
            var tpEtiqueta = parameters.Find(x => x.Id == "tipoEtiqueta").Value;
            var etiqueta = this._uow.EtiquetaLoteRepository.GetEtiquetaLoteActiva(tpEtiqueta, nuExterno);

            var agenda = this._uow.AgendaRepository.GetAgendaSinDetalles(etiqueta.NumeroAgenda);
            var tipoRecepcion = this._uow.RecepcionTipoRepository.GetRecepcionTipo(agenda.TipoRecepcionInterno);
            var producto = this._uow.ProductoRepository.GetProducto(agenda.IdEmpresa, codigoProducto);

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new DateTimeValidationRule(vencimiento),
                    new DateTimeAgendaValidationRule(parsedDate, tipoRecepcion.ControlVencimiento, producto.DiasValidez),
                    new EtiquetaLoteEnEstacionClasificacionValidationRule(this._uow, etiqueta.Numero, estacion),
                },
            };
        }

        public virtual FormValidationGroup ValidateCantidadProducto(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (parameters.Find(x => x.Id == "Focus").Value != field.Id || string.IsNullOrEmpty(field.Value))
                return null;

            var estacion = int.Parse(form.GetField("estacion").Value);
            var nuExterno = parameters.Find(x => x.Id == "etiqueta").Value;
            var tpEtiqueta = parameters.Find(x => x.Id == "tipoEtiqueta").Value;
            var codigoProducto = form.GetField("codigoProducto").Value;
            var lote = form.GetField("lote").Value;

            if (string.IsNullOrEmpty(nuExterno))
                return null;

            var etiqueta = this._uow.EtiquetaLoteRepository.GetEtiquetaLoteActiva(tpEtiqueta, nuExterno);
            var agenda = this._uow.AgendaRepository.GetAgenda(etiqueta.NumeroAgenda);
            var producto = this._uow.ProductoRepository.GetProducto(agenda.IdEmpresa, codigoProducto);

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new PositiveDecimalValidationRule(this._identity.GetFormatProvider(), field.Value, false),
                    new DecimalLengthWithPresicionValidationRule(field.Value, 12, 3, this._identity.GetFormatProvider(), producto.AceptaDecimales),
                    new CantidadAuditadaClasificacionValidationRule(this._uow, this._parameterService, this._identity.GetFormatProvider(), field.Value, etiqueta.Numero, etiqueta.NumeroAgenda, codigoProducto, lote),
                    new EtiquetaLoteEnEstacionClasificacionValidationRule(this._uow, etiqueta.Numero, estacion),
                    new ManejoIdentificadorSerieCantidadValidationRule(field.Value, producto.ManejoIdentificador, _identity.GetFormatProvider())
                },
            };
        }
    }
}
