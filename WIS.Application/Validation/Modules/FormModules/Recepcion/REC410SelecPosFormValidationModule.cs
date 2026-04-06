using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Recepcion;
using WIS.Application.Validation.Rules.General;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.Services.Interfaces;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules.Recepcion
{
    public class REC410SelecPosFormValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IBarcodeService _barcodeService;
        protected readonly IFormatProvider _culture;
        protected readonly decimal _cantidadPendiente;
        
        public REC410SelecPosFormValidationModule(IUnitOfWork uow,
            IBarcodeService barcodeService,
            IFormatProvider culture,
            decimal cantidadPendiente)
        {
            this._uow = uow;
            this._barcodeService = barcodeService;
            this._cantidadPendiente = cantidadPendiente;
            this._culture = culture;
        
            this.Schema = new FormValidationSchema
            {
                ["cantidadSeparar"] = this.ValidateCantidadSeparar,
                ["posicionEquipo"] = this.ValidatePosicionEquipo,
            };
        }

        public virtual FormValidationGroup ValidateCantidadSeparar(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (parameters.Find(x => x.Id == "Focus").Value != field.Id)
                return null;

            var nuExterno = parameters.First(p => p.Id == "etiqueta").Value;
            var tpEtiqueta = parameters.First(p => p.Id == "tipoEtiqueta").Value;
            var codigoProducto = form.GetField("codigoProducto").Value;
            var isReabastecer = bool.Parse(parameters.FirstOrDefault(p => p.Id == "isReabastecer").Value);
            var cdEstacion = int.Parse(form.GetField("estacion").Value);
            var estacion = _uow.MesaDeClasificacionRepository.GetEstacionDeClasificacion(cdEstacion);
            var cdEmpresa = int.Parse(parameters.FirstOrDefault(p => p.Id == "cdEmpresa").Value);
            var cdFaixa = decimal.Parse(parameters.FirstOrDefault(p => p.Id == "faixa").Value, _culture);
            var maximoReabastecible = _uow.AlmacenamientoRepository.GetMaximoReabastecible(estacion.Predio, cdEmpresa, codigoProducto, cdFaixa);

            if (string.IsNullOrEmpty(nuExterno))
                return null;

            var etiqueta = this._uow.EtiquetaLoteRepository.GetEtiquetaLoteActiva(tpEtiqueta, nuExterno);
            var agenda = this._uow.AgendaRepository.GetAgenda(etiqueta.NumeroAgenda);
            var producto = this._uow.ProductoRepository.GetProducto(agenda.IdEmpresa, codigoProducto);

            var rules = new List<IValidationRule>
            {
                new NonNullValidationRule(field.Value),
                new PositiveDecimalValidationRule(this._culture, field.Value, false),
                new DecimalLengthWithPresicionValidationRule(field.Value, 12, 3, this._culture, producto.AceptaDecimales),
                new DecimalLowerThanValidationRule(this._culture, field.Value, _cantidadPendiente),
                new EtiquetaLoteEnEstacionClasificacionValidationRule(this._uow, etiqueta.Numero, cdEstacion),
                new ManejoIdentificadorSerieCantidadValidationRule(field.Value, producto.ManejoIdentificador, _culture)
            };

            if (isReabastecer && maximoReabastecible > 0)
            {
                rules.Add(new DecimalLowerThanValidationRule(this._culture, field.Value, maximoReabastecible));
            }

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
            };
        }

        public virtual FormValidationGroup ValidatePosicionEquipo(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (parameters.Find(x => x.Id == "Focus").Value != field.Id || string.IsNullOrEmpty(field.Value))
                return null;

            var estacion = int.Parse(form.GetField("estacion").Value);
            var nuExterno = parameters.First(p => p.Id == "etiqueta").Value;
            var tpEtiqueta = parameters.First(p => p.Id == "tipoEtiqueta").Value;
            var etiqueta = this._uow.EtiquetaLoteRepository.GetEtiquetaLoteActiva(tpEtiqueta, nuExterno);
            var agenda = this._uow.AgendaRepository.GetAgenda(etiqueta.NumeroAgenda);

            var destino = form.GetField("destino").Value;
            var zona = form.GetField("codigoZona").Value;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new EtiquetaPosicionEquipoValidationRule(_uow, _barcodeService, field.Value, destino, zona, estacion),
                    new EtiquetaLoteEnEstacionClasificacionValidationRule(this._uow, etiqueta.Numero, estacion),
                },
            };
        }
    }
}
