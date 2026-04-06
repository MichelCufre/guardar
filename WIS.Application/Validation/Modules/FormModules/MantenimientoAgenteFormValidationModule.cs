using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Registro;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.FormComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules
{
    public class MantenimientoAgenteFormValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly int _idUsuario;
        protected readonly IFormatProvider _culture;

        Func<Form, FormSelectSearchContext, List<SelectOption>> _searchPais;
        Func<Form, FormSelectSearchContext, List<SelectOption>> _searchPaisSubdivision;

        public MantenimientoAgenteFormValidationModule(IUnitOfWork uow, int idUsuario, IFormatProvider culture)
        {
            this._uow = uow;
            this._idUsuario = idUsuario;
            this._culture = culture;

            this.Schema = new FormValidationSchema
            {
                ["empresa"] = this.ValidateCodigoEmpresa,
                ["tipoAgente"] = this.ValidateTipoAgente,
                ["codigo"] = this.ValidateCodigoAgente,
                ["descripcion"] = this.ValidateDescripcionAgente,
                ["tipoFiscal"] = this.ValidateTipoFiscal,
                ["valorManejaVidaUtil"] = this.ValidateValorManejaVidaUtil,
                ["numeroFiscal"] = this.ValidateNumeroFiscal,
                ["otroDatoFiscal"] = this.ValidateOtroDatoFiscal,
                ["locacionGlobal"] = this.ValidateLocacionGlobal,
                ["ruta"] = this.ValidateRuta,
                ["ordenCarga"] = this.ValidateOrdenCarga,
                ["pais"] = this.ValidatePais,
                ["paisSubdivision"] = this.ValidatePaisSubdivision,
                ["localidad"] = this.ValidateLocalidad,
                ["direccion"] = this.ValidateDireccion,
                ["barrio"] = this.ValidateBarrio,
                ["codigoPostal"] = this.ValidateCodigoPostal,
                ["telefonoPrincipal"] = this.ValidateTelefonoPrincipal,
                ["telefonoSecundario"] = this.ValidateTelefonoSecundario,
                ["anexo1"] = this.ValidateAnexo1,
                ["anexo2"] = this.ValidateAnexo2,
                ["anexo3"] = this.ValidateAnexo3,
                ["anexo4"] = this.ValidateAnexo4,
                ["email"] = this.ValidateEmail,
            };
        }
        public MantenimientoAgenteFormValidationModule(IUnitOfWork uow, Func<Form, FormSelectSearchContext, List<SelectOption>> searchPais, Func<Form, FormSelectSearchContext, List<SelectOption>> searchPaisSubdivision, int idUsuario, IFormatProvider culture) : this(uow, idUsuario, culture)
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
                    new ExisteEmpresaValidationRule(_uow, field.Value)
                },

            };
        }

        public virtual FormValidationGroup ValidateTipoAgente(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.ReadOnly) return null;

            //  if (!string.IsNullOrEmpty(field.Value))
            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value,10),
                    new ExisteTipoAgenteValidationRule(_uow, field.Value)
                },
            };

        }

        public virtual FormValidationGroup ValidateCodigoAgente(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.ReadOnly) return null;

            string empresa = form.GetField("empresa").Value;
            string tipoAgente = form.GetField("tipoAgente").Value;

            field.Value = field.Value.Trim();

            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new StringMaxLengthValidationRule(field.Value, 10),
                   new CodigoAgenteRule(_uow, empresa, tipoAgente,  field.Value),
                },
                Dependencies = { "empresa", "tipoAgente" }
            };
        }

        public virtual FormValidationGroup ValidateDescripcionAgente(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 100),
                },
            };
        }

        public virtual FormValidationGroup ValidateTipoFiscal(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.ReadOnly) return null;

            if (!string.IsNullOrEmpty(field.Value))
            {
                return new FormValidationGroup
                {
                    BreakValidationChain = true,
                    Rules = new List<IValidationRule>
                    {
                        new ExisteTipoFiscalValidationRule(_uow, field.Value)
                    },

                };
            }
            else
            {
                return null;
            }

        }

        public virtual FormValidationGroup ValidateValorManejaVidaUtil(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.ReadOnly) return null;

            if (!string.IsNullOrEmpty(field.Value))
            {
                return new FormValidationGroup
                {
                    BreakValidationChain = true,
                    Rules = new List<IValidationRule>
                    {
                        new DecimalGreaterThanValidationRule(this._culture,field.Value, 0)
                    },

                };
            }
            else
            {
                return null;
            }

        }

        public virtual FormValidationGroup ValidateNumeroFiscal(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.ReadOnly) return null;

            if (!string.IsNullOrEmpty(field.Value))
            {

                string tipoFiscal = form.GetField("tipoFiscal").Value;

                return new FormValidationGroup
                {
                    BreakValidationChain = false,
                    Rules = new List<IValidationRule>
                    {
                       new StringMaxLengthValidationRule(field.Value, 30),
                       new NumeroFiscalRule(_uow, tipoFiscal,  field.Value),
                    },
                    Dependencies = { "tipoFiscal" }
                };
            }
            else
            {
                return null;
            }
        }

        public virtual FormValidationGroup ValidateOtroDatoFiscal(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!string.IsNullOrEmpty(field.Value))
            {
                return new FormValidationGroup
                {
                    BreakValidationChain = false,
                    Rules = new List<IValidationRule>
                    {
                        new StringMaxLengthValidationRule(field.Value, 30),
                    },
                };
            }
            else
                return null;
        }

        public virtual FormValidationGroup ValidateLocacionGlobal(FormField field, Form form, List<ComponentParameter> parameters)
        {

            if (!string.IsNullOrEmpty(field.Value))
            {
                return new FormValidationGroup
                {
                    BreakValidationChain = false,
                    Rules = new List<IValidationRule>
                    {
                        new PositiveLongValidationRule(field.Value),
                        new LocacionGlobalValidationRule(_uow, field.Value)
                    }
                };
            }
            else
                return null;
        }

        public virtual FormValidationGroup ValidatePais(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!string.IsNullOrEmpty(field.Value))
            {
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
            else
                return null;
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
            if (!string.IsNullOrEmpty(field.Value))
            {
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
            else
                return null;
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
            if (!string.IsNullOrEmpty(field.Value))
            {
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
            else
                return null;
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

        public virtual FormValidationGroup ValidateDireccion(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!string.IsNullOrEmpty(field.Value))
                return new FormValidationGroup
                {
                    BreakValidationChain = false,
                    Rules = new List<IValidationRule>
                    {
                        new StringMaxLengthValidationRule(field.Value,100)
                    }
                };
            else
                return null;
        }

        public virtual FormValidationGroup ValidateBarrio(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!string.IsNullOrEmpty(field.Value))
                return new FormValidationGroup
                {
                    BreakValidationChain = false,
                    Rules = new List<IValidationRule>
                    {
                        new StringMaxLengthValidationRule(field.Value,50)
                    }
                };
            else
                return null;
        }

        public virtual FormValidationGroup ValidateTelefonoPrincipal(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!string.IsNullOrEmpty(field.Value))
                return new FormValidationGroup
                {
                    BreakValidationChain = false,
                    Rules = new List<IValidationRule>
                    {
                        new StringMaxLengthValidationRule(field.Value,30)
                    }
                };
            else
                return null;
        }

        public virtual FormValidationGroup ValidateTelefonoSecundario(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!string.IsNullOrEmpty(field.Value))
                return new FormValidationGroup
                {
                    BreakValidationChain = false,
                    Rules = new List<IValidationRule>
                    {
                        new StringMaxLengthValidationRule(field.Value,30)
                    }
                };
            else
                return null;
        }

        public virtual FormValidationGroup ValidateCodigoPostal(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!string.IsNullOrEmpty(field.Value))
                return new FormValidationGroup
                {
                    BreakValidationChain = false,
                    Rules = new List<IValidationRule>
                    {
                        new StringMaxLengthValidationRule(field.Value,15)
                    }
                };
            else
                return null;
        }

        public virtual FormValidationGroup ValidateRuta(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new PositiveShortNumberMaxLengthValidationRule(field.Value,3),
                    new ExisteRutaValidationRule(_uow, field.Value, this._idUsuario)
                },
            };
        }

        public virtual FormValidationGroup ValidateOrdenCarga(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!string.IsNullOrEmpty(field.Value))
            {

                return new FormValidationGroup
                {
                    BreakValidationChain = false,
                    Rules = new List<IValidationRule>
                    {
                       new PositiveShortNumberMaxLengthValidationRule(field.Value,3),
                    },
                    OnSuccess = this.ValidateOrdenCarga_OnSuccess,
                    Dependencies = { "empresa", "codigo", "ruta" }
                };
            }
            else
                return null;
        }
        public virtual void ValidateOrdenCarga_OnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            // Advertencia Orden de carga
            var ordenCarga = form.GetField("ordenCarga").Value;
            var ruta = form.GetField("ruta").Value;
            string codigoAgente = form.GetField("codigo").Value;
            string tipoAgente = form.GetField("tipoAgente").Value;
            string codigoEmpresa = form.GetField("empresa").Value;

            if (!string.IsNullOrEmpty(ordenCarga) && !string.IsNullOrEmpty(ruta) && !string.IsNullOrEmpty(codigoAgente) && !string.IsNullOrEmpty(codigoEmpresa) && !string.IsNullOrEmpty(tipoAgente))
            {

                if (_uow.AgenteRepository.AnyAgentePorRuta(short.Parse(ordenCarga), short.Parse(ruta), tipoAgente, codigoAgente, int.Parse(codigoEmpresa), out Agente agente))
                {
                    parameters.Add(new ComponentParameter() { Id = "warning", Value = "General_Sec0_Warn_CargaClienteExistente" });
                    parameters.Add(new ComponentParameter() { Id = "tipoAgente", Value = agente.Tipo.ToString() });
                    parameters.Add(new ComponentParameter() { Id = "codigoAgente", Value = agente.Codigo });
                    parameters.Add(new ComponentParameter() { Id = "descripcionAgente", Value = agente.Descripcion });
                    parameters.Add(new ComponentParameter() { Id = "empresaAgente", Value = agente.Empresa.ToString() });
                }
            }

        }

        public virtual FormValidationGroup ValidateAnexo1(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!string.IsNullOrEmpty(field.Value))
                return new FormValidationGroup
                {
                    BreakValidationChain = false,
                    Rules = new List<IValidationRule>
                    {
                        new StringMaxLengthValidationRule(field.Value,200)
                    }
                };
            else
                return null;
        }

        public virtual FormValidationGroup ValidateAnexo2(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!string.IsNullOrEmpty(field.Value))
                return new FormValidationGroup
                {
                    BreakValidationChain = false,
                    Rules = new List<IValidationRule>
                    {
                        new StringMaxLengthValidationRule(field.Value,200)
                    }
                };
            else
                return null;
        }

        public virtual FormValidationGroup ValidateAnexo3(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!string.IsNullOrEmpty(field.Value))
                return new FormValidationGroup
                {
                    BreakValidationChain = false,
                    Rules = new List<IValidationRule>
                    {
                        new StringMaxLengthValidationRule(field.Value,200)
                    }
                };
            else
                return null;
        }

        public virtual FormValidationGroup ValidateAnexo4(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!string.IsNullOrEmpty(field.Value))
                return new FormValidationGroup
                {
                    BreakValidationChain = false,
                    Rules = new List<IValidationRule>
                    {
                        new StringMaxLengthValidationRule(field.Value,200)
                    }
                };
            else
                return null;
        }

        public virtual FormValidationGroup ValidateEmail(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!string.IsNullOrEmpty(field.Value))
                return new FormValidationGroup
                {
                    BreakValidationChain = false,
                    Rules = new List<IValidationRule>
                    {
                        new StringMaxLengthValidationRule(field.Value,100),
                        new EmailValidationRule(_uow, field.Value, validacionUsuario:true, allowNull: true, validarMailEnUso: false)
                    }
                };
            else
                return null;
        }
    }
}
