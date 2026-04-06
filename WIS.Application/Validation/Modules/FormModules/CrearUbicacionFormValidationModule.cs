using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.General;
using WIS.Application.Validation.Rules.Registro;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.Domain.General.Configuracion;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.FormComponent.Validation;
using WIS.Security;
using WIS.Validation;
using WIS.Domain;

namespace WIS.Application.Validation.Modules.FormModules
{
    class CrearUbicacionFormValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IIdentityService _securityService;
        protected readonly UbicacionConfiguracion _ubicacionConfiguracion;

        public CrearUbicacionFormValidationModule(IUnitOfWork uow, IIdentityService securityService)
        {
            this._uow = uow;
            this._securityService = securityService;
            this._ubicacionConfiguracion = this._uow.UbicacionRepository.GetUbicacionConfiguracion();

            this.Schema = new FormValidationSchema
            {
                ["idEmpresa"] = this.ValidateCodigoEmpresa,
                ["idArea"] = this.ValidateidArea,
                ["idTipoUbicacion"] = this.ValidateIdUbicacionTipo,
                ["idProductoClase"] = this.ValidateIdClase,
                ["idProductoFamilia"] = this.ValidateIdFamiliaProducto,
                ["idProductoRotatividad"] = this.ValidateIdRotatividad,
                ["numeroPredio"] = this.ValidateNumeroPredio,

                ["codigoBloque"] = this.ValidateCodigoBloque,
                ["codigoCalle"] = this.ValidateCodigoCalle,

                ["columnaDesde"] = this.ValidateColumnaDesde,
                ["columnaHasta"] = this.ValidateColumnaHasta,
                ["columnaSalto"] = this.ValidateColumnaSalto,

                ["alturaDesde"] = this.ValidateAlturaDesde,
                ["alturaHasta"] = this.ValidateAlturaHasta,
                ["alturaSalto"] = this.ValidateAlturaSalto,
                ["idZonaUbicacion"] = this.ValidateidZonaUbicacion,

                ["controlAcceso"] = this.ValidateControlAcceso,
            };
        }

        public virtual FormValidationGroup ValidateControlAcceso(FormField field, Form form, List<ComponentParameter> parameteres)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule> {
                    new ExisteControlAccesoValidationRule(_uow,field.Value),
                },
            };
        }

        public virtual FormValidationGroup ValidateCodigoEmpresa(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.ReadOnly) return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new PositiveNumberMaxLengthValidationRule(field.Value,10),
                    new ExisteEmpresaValidationRule(_uow, field.Value)
                },

            };
        }

        public virtual FormValidationGroup ValidateidArea(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(field.Value),
                    new PositiveShortNumberMaxLengthValidationRule(field.Value,2),
                    new IdUbicacionAreaNoExistenteValidationRule(_uow,  field.Value),
                    new IdUbicacionAreaNoMantenibleValidationRule(_uow,field.Value),
                },
            };
        }

        public virtual FormValidationGroup ValidateidZonaUbicacion(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule> {
                    new IdZonaUbicacionValidationRule(_uow,  field.Value),
                },
            };
        }

        public virtual FormValidationGroup ValidateIdUbicacionTipo(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(field.Value),
                    new PositiveShortNumberMaxLengthValidationRule(field.Value,2),
                    new IdUbicacionTipoNoExistenteValidationRule(_uow, field.Value),

                },
            };
        }

        public virtual FormValidationGroup ValidateIdClase(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(field.Value),
                    new ClaseNoExistenteValidationRule(_uow, field.Value)
                },
            };
        }

        public virtual FormValidationGroup ValidateIdFamiliaProducto(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(field.Value),
                    new PositiveIntValidationRule(field.Value),
                    new IdProductoFamiliaNoExistenteValidationRule(_uow,  field.Value),
                },
            };
        }

        public virtual FormValidationGroup ValidateIdRotatividad(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(field.Value),
                    new PositiveShortNumberMaxLengthValidationRule(field.Value,2),
                    new IdProductoRotatividadNoExistenteValidationRule(_uow,  field.Value),
                },
            };
        }

        public virtual FormValidationGroup ValidateNumeroPredio(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = [
                    new NonNullValidationRule(field.Value),
                    new PredioTieneImportacionUbicacionesDeshabilitadoValidationRule(this._uow, field.Value),
                    new PredioUsuarioExistenteValidationRule(this._uow, this._securityService.UserId ,field.Value)
                ],
            };
        }

        public virtual FormValidationGroup ValidateCodigoBloque(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var rules = new List<IValidationRule>()
            {
                new NonNullValidationRule(field.Value),
                new StringMaxLengthValidationRule(field.Value, _ubicacionConfiguracion.BloqueLargo),
            };

            var validateGroup = new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
            };

            if (_ubicacionConfiguracion.BloqueNumerico)
            {
                rules.Add(new PositiveNumberMaxLengthValidationRule(field.Value, _ubicacionConfiguracion.BloqueLargo));
            }
            else
            {
                rules.Add(new StringSoloLetrasValidationRule(field.Value.ToUpper()));
                validateGroup.OnSuccess = this.ValidateCodigoBloque_OnSuccess;
            }
            return validateGroup;

        }
        public virtual void ValidateCodigoBloque_OnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            field.Value = field.Value.ToUpper();
        }

        public virtual FormValidationGroup ValidateCodigoCalle(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var rules = new List<IValidationRule>()
            {
                new NonNullValidationRule(field.Value),
                new StringMaxLengthValidationRule(field.Value, _ubicacionConfiguracion.CalleLargo),
            };

            var validateGroup = new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
            };

            if (_ubicacionConfiguracion.CalleNumerico)
            {
                rules.Add(new PositiveNumberMaxLengthValidationRule(field.Value, _ubicacionConfiguracion.CalleLargo));
            }
            else
            {
                rules.Add(new StringSoloLetrasValidationRule(field.Value.ToUpper()));
                validateGroup.OnSuccess = ValidateCodigoCalle_OnSuccess;
            }

            return validateGroup;
        }
        public virtual void ValidateCodigoCalle_OnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            field.Value = field.Value.ToUpper();
        }

        public virtual FormValidationGroup ValidateColumnaDesde(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var rules = new List<IValidationRule>()
            {
                new NonNullValidationRule(field.Value),
                new PositiveNumberMaxLengthValidationRule(field.Value,_ubicacionConfiguracion.ColumnaLargo)
            };

            var validationGroup = new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
                OnSuccess = this.ValidateColumnaDesde_OnSuccess
            };

            return validationGroup;
        }
        public virtual void ValidateColumnaDesde_OnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            //if (parameters.FirstOrDefault(s => s.Id == "isSubmit") == null || (parameters.FirstOrDefault(s => s.Id == "isSubmit") != null && !bool.Parse(parameters.FirstOrDefault(s => s.Id == "isSubmit").Value)))
            //{
            //    //  form.GetField("columnaHasta").Value = string.Empty;
            //    //  form.GetField("columnaSalto").Value = string.Empty;
            //}
        }

        public virtual FormValidationGroup ValidateColumnaHasta(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var rules = new List<IValidationRule>()
            {
                new NonNullValidationRule(field.Value),
                new PositiveNumberMaxLengthValidationRule(field.Value,_ubicacionConfiguracion.ColumnaLargo),
                new ColumnaHastaUbicacionValidationRule(field.Value,form.GetField("columnaDesde").Value),
            };

            var validationGroup = new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
                OnSuccess = this.ValidateColumnaHasta_OnSuccess
            };

            validationGroup.Dependencies = new List<string>() { "columnaDesde" };

            return validationGroup;
        }
        public virtual void ValidateColumnaHasta_OnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (parameters.FirstOrDefault(s => s.Id == "isSubmit") == null || (parameters.FirstOrDefault(s => s.Id == "isSubmit") != null && !bool.Parse(parameters.FirstOrDefault(s => s.Id == "isSubmit").Value)))
            {

                //  form.GetField("columnaSalto").Value = string.Empty;

                if (form.GetField("columnaHasta").Value.Equals(form.GetField("columnaDesde").Value))
                {
                    form.GetField("columnaSalto").Value = "1";
                }
            }
        }

        public virtual FormValidationGroup ValidateColumnaSalto(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var rules = new List<IValidationRule>()
            {
                new NonNullValidationRule(field.Value),
                new PositiveIntValidationRule(field.Value),
                new ColumnaSaltoUbicacionValidationRule(field.Value, form.GetField("columnaDesde").Value, form.GetField("columnaHasta").Value),
            };

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
                Dependencies = { "columnaDesde", "columnaHasta" },
                OnSuccess = this.ValidateColumnaSalto_OnSuccess
            };
        }
        public virtual void ValidateColumnaSalto_OnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!string.IsNullOrEmpty(form.GetField("columnaHasta").Value) && !string.IsNullOrEmpty(form.GetField("columnaDesde").Value))
            {
                if (int.TryParse(form.GetField("columnaHasta").Value, out int hasta) && int.TryParse(form.GetField("columnaDesde").Value, out int desde))
                {
                    if ((hasta - desde) >= 0)
                    {
                        int cantidadColumnas = ((hasta - desde) / (field.Value == "0" ? 1 : int.Parse(field.Value)) + 1);

                        parameters.Add(new ComponentParameter() { Id = "infoColumnas", Value = "REG040_Sec0_Info_CantidadColumnas" });
                        parameters.Add(new ComponentParameter() { Id = "cantidadColumnas", Value = cantidadColumnas.ToString() });
                    }
                }
            }

        }

        public virtual FormValidationGroup ValidateAlturaDesde(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var rules = new List<IValidationRule>()
            {
                new NonNullValidationRule(field.Value),
                new PositiveNumberMaxLengthValidationRule(field.Value,_ubicacionConfiguracion.AlturaLargo),
            };

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
                OnSuccess = this.ValidateAlturaDesde_OnSuccess
            };
        }
        public virtual void ValidateAlturaDesde_OnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            //if (parameters.FirstOrDefault(s => s.Id == "isSubmit") == null || (parameters.FirstOrDefault(s => s.Id == "isSubmit") != null && !bool.Parse(parameters.FirstOrDefault(s => s.Id == "isSubmit").Value)))
            //{
            //    form.GetField("alturaDesde").Value = string.Empty;
            //    form.GetField("alturaSalto").Value = string.Empty;
            //}
        }

        public virtual FormValidationGroup ValidateAlturaHasta(FormField field, Form form, List<ComponentParameter> parameters)
        {

            var rules = new List<IValidationRule>()
            {
                new NonNullValidationRule(field.Value),
                new PositiveNumberMaxLengthValidationRule(field.Value,_ubicacionConfiguracion.AlturaLargo),
                new AlturaHastaUbicacionValidationRule( field.Value,form.GetField("alturaDesde").Value),
            };

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
                Dependencies = { "alturaDesde" },
                OnSuccess = this.ValidateAlturaHasta_OnSuccess
            };

        }
        public virtual void ValidateAlturaHasta_OnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (parameters.FirstOrDefault(s => s.Id == "isSubmit") == null || (parameters.FirstOrDefault(s => s.Id == "isSubmit") != null && !bool.Parse(parameters.FirstOrDefault(s => s.Id == "isSubmit").Value)))
            {
                // form.GetField("alturaSalto").Value = string.Empty;

                if (form.GetField("alturaHasta").Value.Equals(form.GetField("alturaDesde").Value))
                {
                    form.GetField("alturaSalto").Value = "1";
                }
            }
        }

        public virtual FormValidationGroup ValidateAlturaSalto(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var rules = new List<IValidationRule>()
            {
                new NonNullValidationRule(field.Value),
                new PositiveIntValidationRule(field.Value),
                new AlturaSaltoUbicacionValidationRule(field.Value, form.GetField("alturaDesde").Value, form.GetField("alturaHasta").Value),
            };

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
                Dependencies = { "alturaDesde", "alturaHasta" },
                OnSuccess = this.ValidateAlturaSalto_OnSuccess
            };
        }
        public virtual void ValidateAlturaSalto_OnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!string.IsNullOrEmpty(form.GetField("alturaHasta").Value) && !string.IsNullOrEmpty(form.GetField("alturaDesde").Value))
            {
                if (int.TryParse(form.GetField("alturaHasta").Value, out int hasta) && int.TryParse(form.GetField("alturaDesde").Value, out int desde))
                {
                    if ((hasta - desde) >= 0)
                    {
                        int cantidadAltura = ((hasta - desde) / (field.Value == "0" ? 1 : int.Parse(field.Value)) + 1);

                        parameters.Add(new ComponentParameter() { Id = "infoAlturas", Value = "REG040_Sec0_Info_CantidadAlturas" });
                        parameters.Add(new ComponentParameter() { Id = "cantidadAlturas", Value = cantidadAltura.ToString() });
                    }

                }

            }
        }

    }
}
