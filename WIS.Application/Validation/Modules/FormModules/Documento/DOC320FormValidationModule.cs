using System;
using System.Collections.Generic;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Documento;
using WIS.Application.Validation.Rules.Registro;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Security;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules.Documento
{
    public class DOC320FormValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IIdentityService _identity;
        protected readonly IFormatProvider _culture;
        protected readonly Form _form;

        public DOC320FormValidationModule(
            Form form,
            IUnitOfWork uow,
            IIdentityService identity)
        {
            this._form = form;
            this._uow = uow;
            this._identity = identity;
            this._culture = identity.GetFormatProvider();

            if (this._form.Id == "DOC320_form_1")
            {
                Schema = new FormValidationSchema
                {
                    ["tpAgrupador"] = this.ValidateTP_AGRUPADOR,
                    ["nroLacre"] = this.ValidateNU_LACRE,
                    ["qtVolumen"] = this.ValidateQT_VOLUMEN,
                    ["qtPeso"] = this.ValidateQT_PESO,
                    ["vlTotal"] = this.ValidateVL_TOTAL,
                    ["qtPesoLiquido"] = this.ValidateQT_PESOLIQUIDO,
                    ["cdTransportadora"] = this.ValidateCD_TRANSPORTADORA,
                    ["tipoVehiculo"] = this.ValidateCD_TIPO_VEICULO,
                    ["motorista"] = this.ValidateDS_MOTORISTA,
                    ["placa"] = this.ValidateDS_PLACA,
                    ["cdEmpresa"] = this.ValidateCD_EMPRESA,
                    ["fechaSalida"] = this.ValidateDT_SAIDA,
                    ["anexo1"] = this.ValidateANEXO1,
                    ["anexo2"] = this.ValidateANEXO2,
                    ["anexo3"] = this.ValidateANEXO3,
                    ["anexo4"] = this.ValidateANEXO4,
                    ["nuPredio"] = this.ValidateNU_PREDIO
                };
            }
            else
            {
                Schema = new FormValidationSchema
                {
                    ["motivoCancelacion"] = this.ValidateMotivo
                };
            }
        }

        public virtual FormValidationGroup ValidateTP_AGRUPADOR(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new TipoDocumentoAgrupadorHabilitadoValidationRule(field.Value, this._uow)
                },
                OnSuccess = this.ValidateTP_AGRUPADOR_OnSuccess
            };
        }

        public virtual FormValidationGroup ValidateNU_LACRE(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value,20)
                }
            };
        }

        public virtual FormValidationGroup ValidateQT_VOLUMEN(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new PositiveNumberMaxLengthValidationRule(field.Value, 999999999)
                }
            };
        }

        public virtual FormValidationGroup ValidateQT_PESO(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new DecimalLengthWithPresicionValidationRule(field.Value,12,3, this._culture),
                    new DecimalGreaterThanValidationRule(this._culture, field.Value, (decimal)0.1)
                }
            };
        }
        public virtual FormValidationGroup ValidateQT_PESOLIQUIDO(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new DecimalLengthWithPresicionValidationRule(field.Value, 12, 3, this._culture),
                    new DecimalGreaterThanValidationRule(this._culture, field.Value, (decimal)0.1)
                }
            };
        }

        public virtual FormValidationGroup ValidateVL_TOTAL(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new DecimalLengthWithPresicionValidationRule(field.Value, 12, 3, this._culture),
                    new DecimalGreaterThanValidationRule(this._culture, field.Value, (decimal)0.1)
                }
            };
        }

        public virtual FormValidationGroup ValidateCD_TRANSPORTADORA(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new ExisteTransportadoraValidationRule(this._uow,field.Value)
                }
            };
        }

        public virtual FormValidationGroup ValidateCD_TIPO_VEICULO(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new ExisteTipoVehiculoValidationRule(this._uow,field.Value)
                }
            };
        }

        public virtual FormValidationGroup ValidateDS_MOTORISTA(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value,100)
                }
            };
        }

        public virtual FormValidationGroup ValidateDS_PLACA(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value,20)
                }
            };
        }

        public virtual FormValidationGroup ValidateCD_EMPRESA(FormField field, Form form, List<ComponentParameter> parameters)
        {

            if (!string.IsNullOrEmpty(field.Value))
            {
                return new FormValidationGroup
                {
                    BreakValidationChain = true,
                    Rules = new List<IValidationRule>
                    {
                        new PositiveNumberMaxLengthValidationRule(field.Value,10),
                        new ExisteEmpresaValidationRule(this._uow, field.Value)
                    }
                };
            }
            else
            {
                return null;
            }
        }

        public virtual FormValidationGroup ValidateDT_SAIDA(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringIsoDateToDateTimeValidationRule(field.Value)
                }
            };
        }

        public virtual FormValidationGroup ValidateANEXO1(FormField field, Form form, List<ComponentParameter> parameters)
        {

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value,200)
                }
            };

        }

        public virtual FormValidationGroup ValidateANEXO2(FormField field, Form form, List<ComponentParameter> parameters)
        {

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value,200)
                }
            };
        }

        public virtual FormValidationGroup ValidateANEXO3(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!string.IsNullOrEmpty(field.Value))
            {
                return new FormValidationGroup
                {
                    BreakValidationChain = true,
                    Rules = new List<IValidationRule>
                    {
                        new NonNullValidationRule(field.Value),
                        new StringMaxLengthValidationRule(field.Value,200)
                    }
                };
            }
            else
            {
                return null;
            }
        }

        public virtual FormValidationGroup ValidateANEXO4(FormField field, Form form, List<ComponentParameter> parameters)
        {

            if (!string.IsNullOrEmpty(field.Value))
            {
                return new FormValidationGroup
                {
                    BreakValidationChain = true,
                    Rules = new List<IValidationRule>
                    {
                        new NonNullValidationRule(field.Value),
                        new StringMaxLengthValidationRule(field.Value,200)
                    }
                };
            }
            else
            {
                return null;
            }
        }

        public virtual FormValidationGroup ValidateNU_PREDIO(FormField field, Form form, List<ComponentParameter> parameters)
        {
            string TP_AGRUPADOR = form.GetField("tpAgrupador").Value;

            if (string.IsNullOrEmpty(TP_AGRUPADOR))
                return null;

            if (this.ValidarRequierePredio(TP_AGRUPADOR, this._uow))
            {
                return new FormValidationGroup
                {
                    BreakValidationChain = true,
                    Rules = new List<IValidationRule>
                    {
                        new NonNullValidationRule(field.Value),
                        new ExistePredioValidationRule(this._uow, this._identity.UserId, this._identity.Predio, field.Value)
                    }
                };
            }
            else
            {
                return null;
            }
        }

        public virtual FormValidationGroup ValidateMotivo(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value,512)
                }
            };
        }

        public virtual void ValidateTP_AGRUPADOR_OnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var agrupadorTipo = this._uow.DocumentoRepository.GetDocumentoAgrupadorTipo(field.Value);

            if (agrupadorTipo.ManejaPredio)
            {
                parameters.Add(new ComponentParameter() { Id = "ManejaPredio", Value = "true" });
            }
            else
            {
                parameters.Add(new ComponentParameter() { Id = "ManejaPredio", Value = "false" });
            }
        }

        public virtual bool ValidarRequierePredio(string tpAgrupador, IUnitOfWork uow)
        {
            var agrupadorTipo = uow.DocumentoRepository.GetDocumentoAgrupadorTipo(tpAgrupador);

            return agrupadorTipo.ManejaPredio;
        }
    }
}
