using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Expedicion;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.Domain.General.Auxiliares;
using WIS.Domain.Services.Interfaces;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Security;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules.Expedicion
{
    public class EXP110ImpresionBultosValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IIdentityService _identity;
        protected readonly IBarcodeService _barcodeService;

        public EXP110ImpresionBultosValidationModule(IUnitOfWork uow, IIdentityService identity, IBarcodeService barcodeService)
        {
            _uow = uow;
            _identity = identity;
            _barcodeService = barcodeService;

            Schema = new FormValidationSchema
            {
                ["nuContenedorBulto"] = this.ValidateNuContenedorBulto,
                ["cdEstiloBulto"] = this.ValidateCdEstiloBulto,
                ["cantidadBulto"] = this.ValidateCantidadBulto,
                ["comentarios"] = this.ValidateComentarios,
                ["status"] = this.ValidateStatus
            };
        }

        public virtual FormValidationGroup ValidateNuContenedorBulto(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return new FormValidationGroup { BreakValidationChain = true };

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new StringMaxLengthValidationRule(field.Value, 400),
                    new EXP110ImpresionBulto(_uow,field.Value,_identity, _barcodeService)
                },
                OnSuccess = this.ValidateNuContenedorBulto_OnSucess,
            };
        }
        public virtual void ValidateNuContenedorBulto_OnSucess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (parameters.FirstOrDefault(w => w.Id == "isSubmit")?.Value != "true")
            {
                _barcodeService.ValidarEtiquetaContenedor(field.Value, _identity.UserId, out AuxContenedor datosContenedor, out int cantidadEmpresa);

                var datosContenedorBulto = _uow.EmpaquetadoPickingRepository.GetDatosContenedorBulto(datosContenedor.NuPreparacion, datosContenedor.NuContenedor);
                form.GetField("nuPreparacionBulto").Value = datosContenedor.NuPreparacion.ToString();
                form.GetField("cantidadBulto").Value = datosContenedorBulto.CantidadBultos.ToString();
                form.GetField("comentarios").Value = datosContenedorBulto?.DescripcionMemo?.ToString();

                parameters.Add(new ComponentParameter
                {
                    Id = "AUX_DATOS_CONT_BULTO",
                    Value = JsonConvert.SerializeObject(datosContenedorBulto)
                });
            }
        }
        public virtual FormValidationGroup ValidateCdEstiloBulto(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new StringMaxLengthValidationRule(field.Value,400),
                }
            };
        }
        public virtual FormValidationGroup ValidateCantidadBulto(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NumeroEnteroValidationRule(field.Value),
                    new PositiveIntValidationRule(field.Value)
                }
            };
        }
        public virtual FormValidationGroup ValidateComentarios(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new StringMaxLengthValidationRule(field.Value,400),
                }
            };
        }
        public virtual FormValidationGroup ValidateStatus(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new StringMaxLengthValidationRule(field.Value,400),
                }
            };
        }

    }
}
