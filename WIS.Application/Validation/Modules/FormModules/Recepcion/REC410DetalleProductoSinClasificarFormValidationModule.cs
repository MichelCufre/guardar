using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation.Rules.Recepcion;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules.Recepcion
{
    public class REC410DetalleProductoSinClasificarFormValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;

        public REC410DetalleProductoSinClasificarFormValidationModule(IUnitOfWork uow)
        {
            this._uow = uow;

            this.Schema = new FormValidationSchema
            {
                ["etiqueta"] = this.ValidateEtiqueta,
            };
        }

        public virtual FormValidationGroup ValidateEtiqueta(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var estacion = int.Parse(parameters.First(p => p.Id == "estacion").Value);
            var nuExterno = field.Value;
            var tpEtiqueta = form.GetField("tipoEtiqueta").Value;

            if (string.IsNullOrEmpty(nuExterno))
                return null;

            var etiqueta = this._uow.EtiquetaLoteRepository.GetEtiquetaLoteActiva(tpEtiqueta, nuExterno);
            var agenda = this._uow.AgendaRepository.GetAgenda(etiqueta.NumeroAgenda);

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new EtiquetaLoteEnEstacionClasificacionValidationRule(this._uow, etiqueta.Numero, estacion),
                },
            };
        }
    }
}
