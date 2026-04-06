using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Expedicion;
using WIS.Application.Validation.Rules.General;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Security;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules
{
    public class CreateEgresoFormValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;

        public CreateEgresoFormValidationModule(IUnitOfWork uow, IIdentityService identity, ISecurityService security)
        {
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
                ["codigoTransportista"] = this.ValidateTransportista,
                ["controlContenedores"] = this.ValidateControlContenedores

            };

            this._uow = uow;
            this._identity = identity;
            this._security = security;
        }

        public virtual FormValidationGroup ValidateDescripcion(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new StringMaxLengthValidationRule(field.Value, 50)
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
        public virtual void ValidatePredioOnFailure(FormField field, Form form, List<ComponentParameter> parameters)
        {
            form.GetField("codigoRuta").ReadOnly = true;
            form.GetField("codigoPuerta").ReadOnly = true;
        }
        public virtual void ValidatePredioOnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            form.GetField("codigoRuta").ReadOnly = false;
            form.GetField("codigoPuerta").ReadOnly = false;

            if (!parameters.Any(s => s.Id == "isSubmit"))
            {
                form.GetField("codigoRuta").Value = string.Empty;
                form.GetField("codigoRuta").Options.Clear();
                form.GetField("codigoPuerta").Value = string.Empty;
                form.GetField("codigoPuerta").Options.Clear();
                parameters.Add(new ComponentParameter("rutaYaAsociada", "N"));

                form.GetField("codigoVehiculo").Value = string.Empty;
                form.GetField("codigoVehiculo").Options.Clear();

                form.GetField("matricula").ReadOnly = false;
                form.GetField("matricula").Value = string.Empty;

                form.GetField("codigoTransportista").ReadOnly = false;
                form.GetField("codigoTransportista").Value = string.Empty;
            }
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
                //Dependencies = { "predio" },
                OnSuccess = this.ValidateRutaOnSuccess
            };
        }
        public virtual void ValidateRutaOnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            Ruta ruta = this._uow.RutaRepository.GetRuta(short.Parse(field.Value));

            if (this._uow.CamionRepository.AnyCamionConRutaAsociada(ruta.Id))
                parameters.Add(new ComponentParameter("rutaYaAsociada", "S"));
            else
                parameters.Add(new ComponentParameter("rutaYaAsociada", "N"));
        }

        public virtual FormValidationGroup ValidateEmpresa(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (this._uow.ParametroRepository.GetParameter("WEXP040_CD_EMPRESA_NULL") == "S" && string.IsNullOrEmpty(field.Value))
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
                   new UserCanWorkWithEmpresaValidationRule(this._security, field.Value)
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
                //Dependencies = { "predio" },
                Rules = rules
            };
        }

        public virtual FormValidationGroup ValidateVehiculo(FormField field, Form form, List<ComponentParameter> parameters)
        {
            form.GetField("matricula").ReadOnly = false;
            form.GetField("codigoTransportista").ReadOnly = false;

            var flTracking = form.GetField("habilitarTracking").Value == "false" ? false : true;

            var rules = new List<IValidationRule>()
            {
                new StringMaxLengthValidationRule(field.Value, 3),
                new PositiveIntValidationRule(field.Value),
                new VehiculoExistsValidationRule(this._uow, field.Value)
            };

            if (flTracking)
                rules.Add(new NonNullValidationRule(field.Value));
            else if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
                OnSuccess = ValidateVehiculo_OnSucess,
                OnFailure = ValidateVehiculo_OnFailure,
            };
        }
        public virtual void ValidateVehiculo_OnSucess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var vehiculo = _uow.VehiculoRepository.GetVehiculo(int.Parse(field.Value));

            if (!string.IsNullOrEmpty(vehiculo.Matricula))
            {
                form.GetField("matricula").Value = vehiculo.Matricula;
                form.GetField("matricula").ReadOnly = true;
            }

            if (vehiculo.Transportista != null)
            {
                var fieldTransportista = form.GetField("codigoTransportista");
                var transportista = _uow.TransportistaRepository.GetTransportista((int)vehiculo.Transportista);

                fieldTransportista.Options.Add(new SelectOption(transportista.Id.ToString(), $"{transportista.Id} - {transportista.Descripcion}"));
                fieldTransportista.Value = vehiculo.Transportista.ToString();

                form.GetField("codigoTransportista").ReadOnly = true;
            }

        }
        public virtual void ValidateVehiculo_OnFailure(FormField field, Form form, List<ComponentParameter> parameters)
        {
            form.GetField("matricula").Value = string.Empty;
        }
        public virtual FormValidationGroup ValidateMatricula(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var rules = new List<IValidationRule>()
            {
                new NonNullValidationRule(field.Value),
                new StringMaxLengthValidationRule(field.Value, 15)
            };

            var flTracking = form.GetField("habilitarTracking").Value == "false" ? false : true;

            string cdVehiculo = form.GetField("codigoVehiculo").Value;
            if (string.IsNullOrEmpty(cdVehiculo) && flTracking) //Este if es solo para que muestre primero el mensaje de requerido en el vehiculo
                rules = new List<IValidationRule>() { };
            else if (!string.IsNullOrEmpty(cdVehiculo))
            {
                var vehiculo = _uow.VehiculoRepository.GetVehiculo(int.Parse(cdVehiculo));
                if (!string.IsNullOrEmpty(vehiculo.Matricula))
                    rules = new List<IValidationRule>() { };
            }

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules
            };
        }

        public virtual FormValidationGroup ValidateTransportista(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var rules = new List<IValidationRule>()
            {
                new NonNullValidationRule(field.Value),
                new StringMaxLengthValidationRule(field.Value, 10),
                new PositiveIntValidationRule(field.Value),
                new ExisteTransportadoraValidationRule(this._uow, field.Value)
            };

            string cdVehiculo = form.GetField("codigoVehiculo").Value;

            if (!string.IsNullOrEmpty(cdVehiculo))
            {
                var vehiculo = _uow.VehiculoRepository.GetVehiculo(int.Parse(cdVehiculo));
                if (vehiculo.Transportista != null)
                    rules = new List<IValidationRule>() { };
            }

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules
            };
        }

        public virtual FormValidationGroup ValidateControlContenedores(FormField field, Form form, List<ComponentParameter> parameters)
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

    }
}