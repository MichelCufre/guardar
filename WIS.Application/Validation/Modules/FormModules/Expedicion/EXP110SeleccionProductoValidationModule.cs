using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Expedicion;
using WIS.Application.Validation.Rules.General;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.Extension;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules.Expedicion
{
    public class EXP110SeleccionProductoValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly int _usuario;
        protected readonly string _predio;
        protected readonly IFormatProvider _formatProvider;

        public EXP110SeleccionProductoValidationModule(IUnitOfWork uow, IFormatProvider formatProvider, int usuarioLogueado, string predioLogueado)
        {
            this._uow = uow;
            this._usuario = usuarioLogueado;
            this._predio = predioLogueado;
            this._formatProvider = formatProvider;
            this.Schema = new FormValidationSchema
            {
                ["cantidadProducto"] = this.ValidateCantidadProducto
            };
        }

        public virtual FormValidationGroup ValidateCantidadProducto(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var dataClientePedido = parameters.FirstOrDefault(x => x.Id == "CONT_ORIGEN_DATA")?.Value;
            var contenedorDestinoDataJSON = parameters.FirstOrDefault(x => x.Id == "CONT_DESTINO_DATA")?.Value;
            var nuContenedorOrigen = parameters.FirstOrDefault(x => x.Id == "AUX_CONT_ORIGEN_NU_CONTENEDOR").Value.ToNumber<int>();
            var nuPreparacionOrigen = parameters.FirstOrDefault(x => x.Id == "AUX_CONT_ORIGEN_NU_PREPARACION").Value.ToNumber<int>();

            var prodJson = parameters.FirstOrDefault(x => x.Id == "AUX_PROD_LEIDO").Value;
            var prod = JsonConvert.DeserializeObject<Producto>(prodJson);
            var rowSelectedPedProdCont = parameters.FirstOrDefault(x => x.Id == "AUX_ROW_SELECTED_PEDPRODCONT").Value;
            var rowSelectedPedProdLote = parameters.FirstOrDefault(x => x.Id == "AUX_ROW_SELECTED_PEDPRODLOTE").Value;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new PositiveDecimalValidationRule(this._formatProvider, field.Value),
                    new DecimalLengthWithPresicionValidationRule(field.Value, 12, 3, this._formatProvider,prod.AceptaDecimales),
                    new EXP110SeleccionProductoCantidadProducto(_uow,_formatProvider,field.Value, contenedorDestinoDataJSON, nuContenedorOrigen, nuPreparacionOrigen, prod, rowSelectedPedProdCont, rowSelectedPedProdLote),
                    new ManejoIdentificadorSerieCantidadValidationRule(field.Value, prod.ManejoIdentificador, _formatProvider)
                }
            };
        }
    }
}
