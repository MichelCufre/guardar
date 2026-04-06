using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Registro;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.FormComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules
{
    public class MantenimientoEmpresaFormValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider _culture;


        Func<Form, FormSelectSearchContext, List<SelectOption>> _searchPais;
        Func<Form, FormSelectSearchContext, List<SelectOption>> _searchPaisSubdivision;

        public MantenimientoEmpresaFormValidationModule(IUnitOfWork uow, IFormatProvider culture)
        {
            this._uow = uow;
            this._culture = culture;

            this.Schema = new FormValidationSchema
            {
                ["codigoEmpresa"] = this.ValidateCodigoEmpresa,
                ["nombreEmpresa"] = this.ValidateNombreEmpresa,
                ["codigoAlmacenaje"] = this.ValidateCodigoAlmacenaje,
                ["minimoStock"] = this.ValidateMinimoStock,
                ["tipoFiscal"] = this.ValidateTipoFiscal,
                ["numeroFiscal"] = this.ValidateNumeroFiscal,
                ["telefono"] = this.ValidateTelefono,
                ["direccion"] = this.ValidateDireccion,
                ["pais"] = this.ValidatePais,
                ["paisSubdivision"] = this.ValidatePaisSubdivision,
                ["localidad"] = this.ValidateLocalidad,
                ["codigoPostal"] = this.ValidateCodigoPostal,
                ["anexo1"] = this.ValidateAnexo1,
                ["anexo2"] = this.ValidateAnexo2,
                ["anexo3"] = this.ValidateAnexo3,
                ["anexo4"] = this.ValidateAnexo4,
                ["tipoNotificacion"] = this.ValidateTipoNotificacion,
                ["payloadUrl"] = this.ValidatePayloadUrl,
                ["secret"] = this.ValidateSecret,
                ["resecret"] = this.ValidateReSecret,
                ["habilitadoCambioSecret"] = this.ValidateCambioSecret
            };
        }
        public MantenimientoEmpresaFormValidationModule(IUnitOfWork uow, Func<Form, FormSelectSearchContext, List<SelectOption>> searchPais, Func<Form, FormSelectSearchContext, List<SelectOption>> searchPaisSubdivision, IFormatProvider culture) : this(uow, culture)
        {
            _searchPais = searchPais;
            _searchPaisSubdivision = searchPaisSubdivision;
        }

        public virtual FormValidationGroup ValidateCodigoEmpresa(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.ReadOnly) return null;

            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new PositiveNumberMaxLengthValidationRule(field.Value,10),
                    new IdEmpresaNoExistenteGeneralValidationRule(_uow, field.Value)
                },
            };
        }
        public virtual FormValidationGroup ValidateNombreEmpresa(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 55),
                },
            };
        }
        public virtual FormValidationGroup ValidateCodigoAlmacenaje(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new PositiveShortNumberMaxLengthValidationRule(field.Value,3),
                    new TipoAlmacenajeSeguroExistenteValidationRule(_uow,short.Parse( field.Value))
                },
            };
        }
        public virtual FormValidationGroup ValidateMinimoStock(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                    {
                        new DecimalLengthWithPresicionValidationRule(field.Value,17,3,this._culture)
                    }
            };

        }
        public virtual FormValidationGroup ValidateTipoFiscal(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.ReadOnly) return null;

            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                    {
                        new ExisteTipoFiscalValidationRule(_uow, field.Value)
                    },
            };
        }

        public virtual FormValidationGroup ValidateTipoNotificacion(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.ReadOnly) return null;

            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                    {
                        new ExisteTipoNotificacionValidationRule(_uow, field.Value)
                    },
                OnSuccess = ValidateTipoNotificacion_OnSuccess
            };
        }

        public virtual void ValidateTipoNotificacion_OnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var parametro = parameters.Find(x => x.Id == "mostrarCamposWebhook");
            parameters.Remove(parametro);
            parameters.Add(new ComponentParameter("mostrarCamposWebhook", field.Value == CodigoDominioDb.TipoNotificacionWebhook ? "true" : "false"));
        }

        public virtual FormValidationGroup ValidatePayloadUrl(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var tipoNotificacion = form.GetField("tipoNotificacion").Value;

            if (string.IsNullOrEmpty(tipoNotificacion) || tipoNotificacion != CodigoDominioDb.TipoNotificacionWebhook)
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new UrlValidationRule(field.Value),
                },
            };
        }

        public virtual FormValidationGroup ValidateSecret(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var mostrarCamposSecret = parameters.Find(x => x.Id == "mostrarCamposSecret");

            if (mostrarCamposSecret == null || mostrarCamposSecret.Value == "false")
                return null;

            var tipoNotificacion = form.GetField("tipoNotificacion").Value;

            if (string.IsNullOrEmpty(tipoNotificacion) || tipoNotificacion != CodigoDominioDb.TipoNotificacionWebhook)
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 50),
                    new ValidacionSecretValidationRule(field.Value),

                },
            };
        }

        public virtual FormValidationGroup ValidateReSecret(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var mostrarCamposSecret = parameters.Find(x => x.Id == "mostrarCamposSecret");

            if (mostrarCamposSecret == null || mostrarCamposSecret.Value == "false")
                return null;

            var tipoNotificacion = form.GetField("tipoNotificacion").Value;

            if (string.IsNullOrEmpty(tipoNotificacion) || tipoNotificacion != CodigoDominioDb.TipoNotificacionWebhook)
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new ConfirmarSecretValidationRule(field.Value, form.GetField("secret").Value),

                },
                Dependencies = { "secret" }
            };
        }

        public virtual FormValidationGroup ValidateNumeroFiscal(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.ReadOnly) return null;

            if (string.IsNullOrEmpty(field.Value))
                return null;

            string tipoFiscal = form.GetField("tipoFiscal").Value;

            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                    {
                       new StringMaxLengthValidationRule(field.Value, 30),
                       new NumeroFiscalRule(_uow, tipoFiscal,  field.Value),
                    },
                Dependencies = { "tipoFiscal" },
                OnSuccess = ValidateNumeroFiscal_OnSuccess
            };

        }
        public virtual void ValidateNumeroFiscal_OnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(form.GetField("tipoFiscal").Value) && !string.IsNullOrEmpty(field.Value))
            {
                form.GetField("tipoFiscal").Value = CodigoDominioDb.TipoFiscalLibre;
            }
        }
        public virtual FormValidationGroup ValidateTelefono(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                    {
                        new StringMaxLengthValidationRule(field.Value,30)
                    }
            };

        }
        public virtual FormValidationGroup ValidateDireccion(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                    {
                        new StringMaxLengthValidationRule(field.Value,100)
                    }
            };

        }
        public virtual FormValidationGroup ValidatePais(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                    {
                        new StringMaxLengthValidationRule(field.Value, 2),
                        new ExistePaisValidationRule(_uow,field.Value)
                    },
                OnSuccess = this.ValidatePais_OnSuccess
            };
        }
        public virtual void ValidatePais_OnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (parameters.FirstOrDefault(s => s.Id == "isSubmit") == null || (parameters.FirstOrDefault(s => s.Id == "isSubmit") != null && !bool.Parse(parameters.FirstOrDefault(s => s.Id == "isSubmit").Value)))
            {
                form.GetField("paisSubdivision").Value = string.Empty;
                form.GetField("localidad").Value = string.Empty;
            }
        }
        public virtual FormValidationGroup ValidatePaisSubdivision(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            var pais = form.GetField("pais").Value;

            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                    {
                        new StringMaxLengthValidationRule(field.Value, 20),
                        new ExistePaisSubdivisionValidationRule(_uow,  field.Value, pais)
                    },
                OnSuccess = this.ValidatePaisSubdivision_OnSuccess
            };

        }
        public virtual void ValidatePaisSubdivision_OnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (parameters.FirstOrDefault(s => s.Id == "isSubmit") == null || (parameters.FirstOrDefault(s => s.Id == "isSubmit") != null && !bool.Parse(parameters.FirstOrDefault(s => s.Id == "isSubmit").Value)))
            {
                form.GetField("localidad").Value = string.Empty;
            }

            var fieldPais = form.GetField("pais");

            if (string.IsNullOrEmpty(fieldPais.Value))
            {
                PaisSubdivision subdivision = _uow.PaisSubdivisionRepository.GetPaisSubdivision(field.Value);

                fieldPais.Options = _searchPais(form, new FormSelectSearchContext()
                {
                    SearchValue = subdivision.Pais.Id
                });

                form.GetField("pais").Value = subdivision.Pais.Id;
            }
        }
        public virtual FormValidationGroup ValidateLocalidad(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            var pais = form.GetField("pais").Value;
            var paisSubdivision = form.GetField("paisSubdivision").Value;

            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                    {
                        new PositiveLongValidationRule(field.Value),
                        new ExisteLocalidadValidationRule(_uow, long.Parse( field.Value),pais,paisSubdivision)
                    },
                OnSuccess = this.ValidateCodigoLocalidad_OnSuccess
            };

        }
        public virtual void ValidateCodigoLocalidad_OnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var fieldPais = form.GetField("pais");
            var fieldPaisSubdivision = form.GetField("paisSubdivision");

            PaisSubdivisionLocalidad subdivisionLocalidad = null;

            if (string.IsNullOrEmpty(fieldPais.Value))
            {
                subdivisionLocalidad = _uow.PaisSubdivisionLocalidadRepository.GetLocalidad(long.Parse(field.Value));

                fieldPais.Options = _searchPais(form, new FormSelectSearchContext()
                {
                    SearchValue = subdivisionLocalidad.Subdivision.Pais.Id
                });

                fieldPais.Value = subdivisionLocalidad.Subdivision.Pais.Id;
            }

            if (string.IsNullOrEmpty(fieldPaisSubdivision.Value))
            {
                if (subdivisionLocalidad == null)
                    subdivisionLocalidad = _uow.PaisSubdivisionLocalidadRepository.GetLocalidad(long.Parse(field.Value));

                fieldPaisSubdivision.Options = _searchPaisSubdivision(form, new FormSelectSearchContext()
                {
                    SearchValue = subdivisionLocalidad.Subdivision.Id
                });

                fieldPaisSubdivision.Value = subdivisionLocalidad.Subdivision.Id;
            }

        }
        public virtual FormValidationGroup ValidateCodigoPostal(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;
            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                    {
                        new StringMaxLengthValidationRule(field.Value,15)
                    }
            };
        }
        public virtual FormValidationGroup ValidateAnexo1(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                    {
                        new StringMaxLengthValidationRule(field.Value,200)
                    }
            };
        }
        public virtual FormValidationGroup ValidateAnexo2(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                    {
                        new StringMaxLengthValidationRule(field.Value,200)
                    }
            };
        }
        public virtual FormValidationGroup ValidateAnexo3(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                    {
                        new StringMaxLengthValidationRule(field.Value,200)
                    }
            };
        }
        public virtual FormValidationGroup ValidateAnexo4(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                    {
                        new StringMaxLengthValidationRule(field.Value,200)
                    }
            };

        }

        public virtual FormValidationGroup ValidateCambioSecret(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (parameters.Count > 0 && parameters.Find(x => x.Id == "mostrarCamposSecret") == null)
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value)
                },
            };
        }
    }
}