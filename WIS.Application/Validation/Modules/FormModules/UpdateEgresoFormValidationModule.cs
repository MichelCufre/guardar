using System.Collections.Generic;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Expedicion;
using WIS.Application.Validation.Rules.General;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.Expedicion;
using WIS.Domain.General;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Security;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules
{
    public class UpdateEgresoFormValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly Camion _camion;

        public UpdateEgresoFormValidationModule(IUnitOfWork uow, IIdentityService identity, ISecurityService security, Camion camion)
        {
            this._uow = uow;
            this._identity = identity;
            this._security = security;
            this._camion = camion;

            this.Schema = new FormValidationSchema
            {
                ["descripcion"] = this.ValidateDescripcion,
                ["predio"] = this.ValidatePredio,
                ["codigoRuta"] = this.ValidateRuta,
                ["codigoEmpresa"] = this.ValidateEmpresa,
                ["respetaOrdenCarga"] = this.ValidateRespetaOrdenCarga,
                ["habilitarTracking"] = this.ValidateHabilitarTracking,
                ["habilitarRuteo"] = this.ValidateHabilitarRuteo,
                ["codigoPuerta"] = this.ValidatePuerta,
                ["codigoVehiculo"] = this.ValidateVehiculo,
                ["matricula"] = this.ValidateMatricula,
                ["codigoTransportista"] = this.ValidateTransportista
            };
        }

        public virtual FormValidationGroup ValidateDescripcion(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new StringMaxLengthValidationRule(field.Value, 15)
                }
            };
        }

        public virtual FormValidationGroup ValidatePredio(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new StringMaxLengthValidationRule(field.Value, 15),
                   new ExistePredioValidationRule(this._uow, this._identity.UserId, this._identity.Predio, field.Value)
                },
                OnFailure = this.ValidatePredioOnFailure,
                OnSuccess = this.ValidatePredioOnSuccess
            };
        }

        public virtual FormValidationGroup ValidateRuta(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            string predio = form.GetField("predio").Value;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new PositiveShortValidationRule(field.Value),
                   new RutaExistsValidationRule(this._uow, field.Value, predio)
                },
                Dependencies = { "predio" },
                OnSuccess = this.ValidateRutaOnSuccess
            };
        }

        public virtual FormValidationGroup ValidateEmpresa(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (this._uow.ParametroRepository.GetParameter("WEXP040_CD_EMPRESA_NULL").Equals("S") && string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new StringMaxLengthValidationRule(field.Value, 10),
                   new PositiveIntValidationRule(field.Value),
                   new ExisteEmpresaValidationRule(this._uow, field.Value),
                   new UserCanWorkWithEmpresaValidationRule(this._security, field.Value),
                   new CargasEmpresaCamionValidationRule(this._uow, this._camion.Id, field.Value)
                }
            };
        }

        public virtual FormValidationGroup ValidateRespetaOrdenCarga(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new BooleanStringValidationRule(field.Value)
                }
            };
        }

        public virtual FormValidationGroup ValidateHabilitarTracking(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new BooleanStringValidationRule(field.Value)
                }
            };
        }

        public virtual FormValidationGroup ValidateHabilitarRuteo(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new BooleanStringValidationRule(field.Value)
                }
            };
        }

        public virtual FormValidationGroup ValidatePuerta(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            string predio = form.GetField("predio").Value;

            var rules = new List<IValidationRule>
            {
                new StringMaxLengthValidationRule(field.Value, 3),
                new PositiveShortValidationRule(field.Value),
                new PuertaEmbarqueExistsValidationRule(this._uow, field.Value)
            };

            if (!string.IsNullOrEmpty(predio))
                rules.Add(new PuertaEmbarquePertenecePredioValidationRule(this._uow, field.Value, predio));

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Dependencies = { "predio" },
                Rules = rules
            };
        }

        public virtual FormValidationGroup ValidateVehiculo(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new StringMaxLengthValidationRule(field.Value, 3),
                    new PositiveIntValidationRule(field.Value),
                    new VehiculoExistsValidationRule(this._uow, field.Value)
                }
            };
        }

        public virtual FormValidationGroup ValidateMatricula(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new StringMaxLengthValidationRule(field.Value, 15)
                }
            };
        }

        public virtual FormValidationGroup ValidateTransportista(FormField field, Form form, List<ComponentParameter> parameters)
        {
            string vehiculo = form.GetField("codigoVehiculo").Value;

            if (string.IsNullOrEmpty(vehiculo) && string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 10),
                    new PositiveIntValidationRule(field.Value),
                    new ExisteTransportadoraValidationRule(this._uow, field.Value)
                }
            };
        }

        public virtual void ValidatePredioOnFailure(FormField field, Form form, List<ComponentParameter> parameters)
        {
            form.GetField("codigoRuta").ReadOnly = true;
            form.GetField("codigoPuerta").ReadOnly = true;
        }

        public virtual void ValidatePredioOnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            form.GetField("codigoRuta").ReadOnly = false;
            form.GetField("codigoPuerta").ReadOnly = false;
        }

        public virtual void ValidateRutaOnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            Ruta ruta = this._uow.RutaRepository.GetRuta(short.Parse(field.Value));

            if (this._uow.CamionRepository.AnyCamionConRutaAsociada(ruta.Id, this._camion.Id))
                parameters.Add(new ComponentParameter("rutaYaAsociada", "S"));
        }
    }
}