using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Produccion;
using WIS.Application.Validation.Rules.Registro;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Produccion.Constants;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Security;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules.Produccion
{
    public class PRD110FormValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IIdentityService _securityService;

        public PRD110FormValidationModule(IUnitOfWork uow, IIdentityService securityService)
        {
            _uow = uow;
            _securityService = securityService;

            Schema = new FormValidationSchema
            {
                ["cdFormula"] = this.ValidateCodigoFormula,
                ["tpIngreso"] = this.ValidateTipoIngreso,
                ["qtFormula"] = this.ValidateCantidad,
                ["predio"] = this.ValidatePredio,
                ["ingresoConFormula"] = this.ValidateIngresoConFormula,
                ["generarPedido"] = this.ValidateGenerarPedido,
                ["empresa"] = this.ValidateEmpresa,
                ["idExterno"] = this.ValidateIdExterno

            };
        }

        public virtual FormValidationGroup ValidateCodigoFormula(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var isConFormula = form.GetField("ingresoConFormula").Value;

            if (string.IsNullOrEmpty(field.Value) || isConFormula == "false")
            {
                return null;
            }

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 10),
                    new FormulaNoExisteValidationRule(this._uow,field.Value),
                    new FormulaIsActiveValidationRule(this._uow,field.Value)
                },
                OnSuccess = this.ValidateCodigoFormulaOnSuccess
            };
        }

        public virtual void ValidateCodigoFormulaOnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var formula = this._uow.FormulaRepository.GetFormula(field.Value);
            var nombreEmpresa = this._uow.EmpresaRepository.GetNombre(formula.Empresa);

            var inputDescripcion = form.GetField("dsFormula");
            inputDescripcion.Value = formula.Descripcion;

            var inputEmpresa = form.GetField("empresa");

            inputEmpresa.Options = new List<SelectOption>();
            inputEmpresa.ReadOnly = true;
            inputEmpresa.Options.Add(new SelectOption(formula.Empresa.ToString(), $"{formula.Empresa.ToString()} - {nombreEmpresa}"));
            inputEmpresa.Value = formula.Empresa.ToString();
        }

        public virtual FormValidationGroup ValidateTipoIngreso(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new TipoIngresoValidationRule(this._uow,field.Value)
                },
                OnSuccess = ValidateTipoIngresoOnSuccess
            };
        }

        public virtual void ValidateTipoIngresoOnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!parameters.Any(s => s.Id == "isSubmit"))
            {
                AgregarParametroValidation(parameters, "tipoIngreso", field.Value);

                var fieldPedido = form.GetField("generarPedido");
                var ingresoConFormula = form.GetField("ingresoConFormula");
                var generaPreparacion = form.GetField("generaPreparacion");

                HabilitarCampos(field, parameters, fieldPedido, ingresoConFormula, generaPreparacion);
            }
        }

        public virtual void HabilitarCampos(FormField field, List<ComponentParameter> parameters, FormField fieldPedido, FormField ingresoConFormula, FormField generaPreparacion)
        {
            if (field.Value == TipoIngresoProduccion.Colector)
            {
                AgregarParametroValidation(parameters, "ingresoConFormula", ingresoConFormula.Value);

                fieldPedido.Disabled = true;
                fieldPedido.Value = "true";
                generaPreparacion.Disabled = false;
            }
            else
            {
                AgregarParametroValidation(parameters, "ingresoConFormula", ingresoConFormula.Value);

                fieldPedido.Disabled = false;
                generaPreparacion.Disabled = true;

                generaPreparacion.Disabled = fieldPedido.Value != "true";
                if (fieldPedido.Value == "false")
                {
                    generaPreparacion.Value = fieldPedido.Value;
                }
            }
        }

        public virtual FormValidationGroup ValidateCantidad(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var isConFormula = parameters.Find(x => x.Id == "isConFormula")?.Value;

            if (isConFormula == null || isConFormula == "false")
            {
                return null;
            }

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new PositiveIntValidationRule(field.Value),
                    new IntGreaterOrEqualThanValidationRule(field.Value,1)
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
                    new ExistePredioValidationRule(this._uow,this._securityService.UserId,this._securityService.Predio,field.Value)
                },
                OnSuccess = ValidatePredioOnSuccess
            };
        }

        public virtual void ValidateEmpresaOnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            HabilitarTiposProduccion(form, parameters);
        }

        public virtual void ValidatePredioOnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            HabilitarTiposProduccion(form, parameters);
        }

        public virtual void HabilitarTiposProduccion(Form form, List<ComponentParameter> parameters)
        {
            var predio = form.GetField("predio").Value;
            var empresa = form.GetField("empresa").Value;
            var selectTipoIngreso = form.GetField("tpIngreso");

            if (string.IsNullOrEmpty(predio) || string.IsNullOrEmpty(empresa))
            {
                selectTipoIngreso.Options.Clear();
            }
            else if (!parameters.Any(s => s.Id == "isSubmit"))
            {
                selectTipoIngreso.Options = new List<SelectOption>();

                var tiposIngreso = _uow.DominioRepository.GetDominios(CodigoDominioDb.TipoIngresoProduccion);
                var tipoIngreso = tiposIngreso.LastOrDefault().Id;
                var detaultTipoIngreso = _uow.ParametroRepository.GetParameter("PRD110_DEFAULT_TIPO_INGRESO");
                var anyEspacioPredio = _uow.EspacioProduccionRepository.AnyEspacioProduccionPredio(predio);

                if (detaultTipoIngreso != null)
                {
                    tipoIngreso = detaultTipoIngreso;
                }

                foreach (var tipo in tiposIngreso)
                {
                    if ((tipo.Id == TipoIngresoProduccion.BlackBox && !_uow.EmpresaRepository.IsEmpresaDocumental(int.Parse(empresa)) && anyEspacioPredio) || tipo.Id == TipoIngresoProduccion.Colector)
                    {
                        selectTipoIngreso.Options.Add(new SelectOption(tipo.Id, tipo.Descripcion));
                    }
                }

                if (selectTipoIngreso.Options.Count == 1)
                    selectTipoIngreso.Value = selectTipoIngreso.Options.FirstOrDefault().Value;
                else if (!string.IsNullOrEmpty(tipoIngreso))
                    selectTipoIngreso.Value = tipoIngreso;


                var fieldPedido = form.GetField("generarPedido");
                var ingresoConFormula = form.GetField("ingresoConFormula");
                var generaPreparacion = form.GetField("generaPreparacion");

                HabilitarCampos(selectTipoIngreso, parameters, fieldPedido, ingresoConFormula, generaPreparacion);
            }
        }

        public virtual FormValidationGroup ValidateIngresoConFormula(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new BooleanStringValidationRule(field.Value)
                },
                OnSuccess = ValidateIngresoConFormulaOnSuccess
            };
        }

        public virtual void ValidateIngresoConFormulaOnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!parameters.Any(s => s.Id == "isSubmit"))
            {
                AgregarParametroValidation(parameters, "ingresoConFormula", field.Value);
            }
        }

        public virtual FormValidationGroup ValidateGenerarPedido(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new BooleanStringValidationRule(field.Value)
                },
                OnSuccess = this.ValidateGenerarPedido_OnSuccess
            };
        }

        public virtual void ValidateGenerarPedido_OnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!parameters.Any(s => s.Id == "isSubmit"))
            {

                var generaPreparacion = form.GetField("generaPreparacion");

                if (field.Value == "true")
                {
                    generaPreparacion.Value = "true";
                    generaPreparacion.Disabled = false;
                }
                else
                {
                    generaPreparacion.Value = "false";
                    generaPreparacion.Disabled = true;
                }
            }
        }

        public virtual FormValidationGroup ValidateEmpresa(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new EmpresaExistenteValidationRule(this._uow, field.Value)
                },
                OnSuccess = ValidateEmpresaOnSuccess
            };
        }

        public virtual FormValidationGroup ValidateIdExterno(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new StringMaxLengthValidationRule(field.Value, 100),
                    new EmpresaRequeridaValidationRule(_uow, form.GetField("empresa").Value ),
                    new ExisteIdExternoIngresoValidationRule(_uow, field.Value, form.GetField("empresa").Value)
                }
            };
        }

        public virtual void AgregarParametroValidation(List<ComponentParameter> parameters, string Id, string value)
        {
            var genericParam = new ComponentParameter()
            {
                Id = Id,
                Value = value
            };

            if (parameters == null)
            {
                parameters = new List<ComponentParameter>();
                parameters.Add(genericParam);
            }
            else
            {
                if (parameters.FirstOrDefault(p => p.Id == Id) != null)
                    parameters.FirstOrDefault(p => p.Id == Id).Value = genericParam.Value;
                else
                    parameters.Add(genericParam);
            }
        }
    }
}