using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.General;
using WIS.Application.Validation.Rules.OrdenTarea;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.Domain.OrdenTarea;
using WIS.Domain.Security;
using WIS.Domain.Services.Interfaces;
using WIS.Exceptions;
using WIS.Extension;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Security;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules
{
    class ORT090ValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly int _userId;
        protected readonly string _aplicacion;
        protected readonly IIdentityService _identity;
        protected readonly IAuthorizationService _authService;

        public ORT090ValidationModule(IUnitOfWork uow, IIdentityService identity, IAuthorizationService authService, int userId, string aplicacion)
        {
            this._uow = uow;
            this._userId = userId;
            this._aplicacion = aplicacion;
            this._identity = identity;
            this._authService = authService;
            this.Schema = new FormValidationSchema
            {
                ["usuario"] = this.ValidateUsuario,
                ["nombre"] = this.ValidateNombre,
                ["password"] = this.ValidatePassword,
                ["codigoEmpresa"] = this.ValidateCodigoEmpresa,
                ["descripcionEmpresa"] = this.ValidateDescipcionEmpresa,
                ["codigoTarea"] = this.ValidateCodigoTarea,
                ["descripcionTarea"] = this.ValidateDescrpcionTarea,
                ["numeroOrden"] = this.ValidateNumeroOrden,
                ["descripcionOrden"] = this.ValidateDescrpcionOrden,
                ["fechaInicio"] = this.ValidateFechaInicio,
                ["fechaFin"] = this.ValidateFechaFin,
            };
        }

        public virtual FormValidationGroup ValidateUsuario(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var rules = new List<IValidationRule>
            {
                new NonNullValidationRule(field.Value),
                new StringMaxLengthValidationRule(field.Value, 50)
            };

            if (form.GetField("usuario").Value != null)
            {
                rules.Add(new ExisteUsuarioVerificacionValidationRule(this._uow, field.Value));

                var userId = _uow.SecurityRepository.GetUserIdByLoginName(form.GetField("usuario").Value);

                if (userId != null)
                {
                    rules.Add(new ExisteSesionFuncionarioActiva(this._uow, userId.Value));

                    var ordenTareaFuncAmigable = _uow.TareaRepository.GetOrdenTareaFuncionarioAmigableByUserId(userId.Value);
                    OrdenTareaObjeto ordenTarea = ordenTareaFuncAmigable != null ? _uow.TareaRepository.GetOrdenTarea(ordenTareaFuncAmigable.NumeroOrdenTarea) : null;

                    var nuOrden = parameters.FirstOrDefault(x => x.Id == "nuOrden")?.Value;
                    Orden orden = !string.IsNullOrEmpty(nuOrden) ? _uow.OrdenRepository.GetOrden(int.Parse(nuOrden)) : null;

                    if (ordenTareaFuncAmigable != null && orden != null)
                    {
                        rules.Add(new OrdenesDistintasTareaAmigableValidationRule(this._uow, orden.Id, ordenTarea.NuOrden));
                    }
                }
            }

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
                OnSuccess = this.OnSuccessFormValidateUsuario
            };
        }

        public virtual void OnSuccessFormValidateUsuario(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (form.GetField("usuario").Value != null)
            {
                Usuario usuario = this._uow.SecurityRepository.GetUsuario(field.Value);
                form.GetField("nombre").Value = usuario.Name;
                form.GetField("password").Disabled = false;
            }
        }

        public virtual FormValidationGroup ValidateNombre(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.ReadOnly == true)
                return null;

            return new FormValidationGroup
            {
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 100)
                }
            };
        }

        public virtual FormValidationGroup ValidatePassword(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var rules = new List<IValidationRule>
            {
                new NonNullValidationRule(field.Value),
            };

            if (!string.IsNullOrEmpty(form.GetField("usuario").Value) && !string.IsNullOrEmpty(form.GetField("password").Value))
                rules.Add(new ValidarPasswordValidationRule(_authService, form.GetField("usuario").Value, field.Value));

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
                OnSuccess = this.OnSuccessFormValidatePassword
            };
        }

        public virtual void OnSuccessFormValidatePassword(FormField field, Form form, List<ComponentParameter> parameters)
        {
            string envioDeFormulario = parameters.Any(s => s.Id == "isSubmit") ? parameters.FirstOrDefault(s => s.Id == "isSubmit").Value : null;
            if (envioDeFormulario == null)
            {
                var userId = _uow.SecurityRepository.GetUserIdByLoginName(form.GetField("usuario").Value);
                var ordenTareaFuncAmigable = _uow.TareaRepository.GetOrdenTareaFuncionarioAmigableByUserId(userId.Value);

                field.Disabled = true;
                form.GetField("usuario").Disabled = true;
                form.GetButton("Cancelar").Disabled = false;

                var selectorCodigoEmpresa = form.GetField("codigoEmpresa");

                var nuOrden = parameters.FirstOrDefault(x => x.Id == "nuOrden")?.Value;
                Orden orden = !string.IsNullOrEmpty(nuOrden) ? _uow.OrdenRepository.GetOrden(int.Parse(nuOrden)) : null;

                if (ordenTareaFuncAmigable == null)
                {
                    if (orden == null)
                    {
                        form.GetField("numeroOrden").ReadOnly = false;
                        form.GetField("numeroOrden").Value = "";
                        form.GetField("descripcionOrden").Value = "";
                    }

                    form.GetField("codigoTarea").ReadOnly = false;
                    form.GetField("codigoTarea").Value = "";
                    form.GetField("descripcionTarea").Value = "";

                    form.GetField("fechaInicio").ReadOnly = false;
                    form.GetField("fechaInicio").Value = DateTime.Now.ToIsoString();

                    form.GetField("fechaFin").ReadOnly = false;
                    form.GetField("fechaFin").Value = "";

                    form.GetButton("Confirmar").Disabled = false;
                    form.GetButton("Terminar").Disabled = true;

                    selectorCodigoEmpresa.ReadOnly = false;
                    selectorCodigoEmpresa.Value = "";
                    form.GetField("descripcionEmpresa").Value = "";

                    selectorCodigoEmpresa.Options = new List<SelectOption>();

                    List<Empresa> empresas = _uow.EmpresaRepository.GetEmpresasParaUsuario(userId.Value);
                    foreach (var empresa in empresas)
                    {
                        selectorCodigoEmpresa.Options.Add(new SelectOption(empresa.Id.ToString(), $"{empresa.Id} - {empresa.Nombre}"));
                    }

                    if (empresas.Count == 1)
                    {
                        selectorCodigoEmpresa.Value = empresas[0].Id.ToString();
                        form.GetField("descripcionEmpresa").Value = empresas[0].Nombre;
                        selectorCodigoEmpresa.ReadOnly = true;
                    }
                }
                else
                {
                    var ordenTarea = _uow.TareaRepository.GetOrdenTarea(ordenTareaFuncAmigable.NumeroOrdenTarea);
                    var empresa = _uow.EmpresaRepository.GetEmpresa(ordenTarea.Empresa);

                    //Cargar campos
                    form.GetField("descripcionEmpresa").Value = empresa.Nombre;
                    form.GetField("descripcionTarea").Value = _uow.TareaRepository.GetDescripcionTarea(ordenTarea.CdTarea);
                    form.GetField("fechaInicio").Value = ordenTareaFuncAmigable.FechaDesde.ToIsoString();
                    form.GetField("fechaInicio").ReadOnly = true;
                    form.GetField("fechaFin").ReadOnly = false;
                    form.GetField("fechaFin").Value = DateTime.Now.ToIsoString();
                    form.GetField("codigoTarea").ReadOnly = true;
                    form.GetField("codigoTarea").Value = ordenTarea.CdTarea;
                    form.GetButton("Confirmar").Disabled = true;
                    form.GetButton("Terminar").Disabled = false;

                    if (orden == null)
                    {
                        var fieldNuOrden = form.GetField("numeroOrden");
                        fieldNuOrden.ReadOnly = true;
                        fieldNuOrden.Value = ordenTarea.NuOrden.ToString();
                        form.GetField("descripcionOrden").Value = _uow.OrdenRepository.GetDescripcionOrden(int.Parse(fieldNuOrden.Value));
                    }

                    selectorCodigoEmpresa.Options = new List<SelectOption>()
                    {
                        new SelectOption(empresa.Id.ToString(), $"{empresa.Id} - {empresa.Nombre}")
                    };

                    selectorCodigoEmpresa.ReadOnly = true;
                    selectorCodigoEmpresa.Value = empresa.Id.ToString();
                }
            }
        }

        public virtual FormValidationGroup ValidateNumeroOrden(FormField field, Form form, List<ComponentParameter> list)
        {
            if (field.ReadOnly == true)
                return null;

            var existeOrdenActivaValidationRule = new ExisteOrdenActivaValidationRule(this._uow, field.Value);
            var Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value)
                };

            if (form.GetField("numeroOrden").Value != "")
                Rules.Add(existeOrdenActivaValidationRule);
            else
                Rules.Remove(existeOrdenActivaValidationRule);

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = Rules,
                OnSuccess = this.OnSuccessFormValidateOrden
            };
        }

        public virtual void OnSuccessFormValidateOrden(FormField field, Form form, List<ComponentParameter> list)
        {
            form.GetField("descripcionOrden").Value = this._uow.OrdenRepository.GetDescripcionOrden(int.Parse(field.Value));
        }

        public virtual FormValidationGroup ValidateDescrpcionOrden(FormField field, Form form, List<ComponentParameter> list)
        {
            return new FormValidationGroup
            {
                Rules = new List<IValidationRule>
                {
                    new StringMaxLengthValidationRule(field.Value, 60)
                }
            };
        }

        public virtual FormValidationGroup ValidateCodigoEmpresa(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.ReadOnly == true)
                return null;

            ExisteEmpresaValidationRule existeEmpresaValidationRule = new ExisteEmpresaValidationRule(this._uow, field.Value);
            var Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 20)

                };

            if (form.GetField("codigoEmpresa").Value != "")
                Rules.Add(existeEmpresaValidationRule);
            else
                Rules.Remove(existeEmpresaValidationRule);

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = Rules,
                OnSuccess = this.OnSuccessFormValidateEmpresa
            };
        }

        public virtual void OnSuccessFormValidateEmpresa(FormField field, Form form, List<ComponentParameter> parameters)
        {

            if (form.GetField("codigoEmpresa").Value != "")
            {
                Empresa empresa = this._uow.EmpresaRepository.GetEmpresa(int.Parse(field.Value));
                form.GetField("descripcionEmpresa").Value = empresa.Nombre;
            }

        }

        public virtual FormValidationGroup ValidateDescipcionEmpresa(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.ReadOnly == true)
                return null;

            return new FormValidationGroup
            {
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 55)
                }
            };
        }

        public virtual FormValidationGroup ValidateCodigoTarea(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.ReadOnly == true)
                return null;

            var existeCodigoDeTarea = new ExisteCodigoDeTarea(this._uow, field.Value);

            var Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 100)
                };

            if (form.GetField("codigoTarea").Value != "")
                Rules.Add(existeCodigoDeTarea);
            else
                Rules.Remove(existeCodigoDeTarea);

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = Rules,
                OnSuccess = this.OnSuccessFormValidateCodigoTarea
            };
        }

        public virtual void OnSuccessFormValidateCodigoTarea(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (form.GetField("codigoTarea").Value != "")
            {
                Tarea tarea = this._uow.TareaRepository.GetTarea(field.Value);
                form.GetField("descripcionTarea").Value = tarea.Descripcion;
            }
        }

        public virtual FormValidationGroup ValidateDescrpcionTarea(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.ReadOnly == true)
                return null;

            return new FormValidationGroup
            {
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 60)
                }
            };
        }

        public virtual FormValidationGroup ValidateFechaInicio(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.Disabled == true)
                return null;

            var userId = this._uow.SecurityRepository.GetUserIdByLoginName(form.GetField("usuario").Value);
            var ordenTareaFuncAmigable = _uow.TareaRepository.GetOrdenTareaFuncionarioAmigableByUserId(userId.Value);

            if (ordenTareaFuncAmigable != null)
                return null;


            var rules = new List<IValidationRule>
            {
                new NonNullValidationRule(field.Value),
                new DateTimeValidationRule(field.Value)
            };

            var numeroOrdenStr = form.GetField("numeroOrden").Value;

            if (!string.IsNullOrEmpty(numeroOrdenStr) && !string.IsNullOrEmpty(field.Value))
            {
                var nuOrden = int.Parse(numeroOrdenStr);
                var orden = this._uow.OrdenRepository.GetOrden(nuOrden);

                if (orden != null)
                {
                    var validarFecha = new FechaDesdeOrdenTareaFuncionarioValidationRule(
                        this._uow,
                        orden.FechaInicio.ToIsoString(),
                        field.Value,
                        userId.ToString(),
                        this._identity.GetFormatProvider());

                    rules.Add(validarFecha);
                }
            }

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
            };

        }

        public virtual FormValidationGroup ValidateFechaFin(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.ReadOnly == true)
                return null;

            var userId = _uow.SecurityRepository.GetUserIdByLoginName(form.GetField("usuario").Value);
            var ordenTareaFuncAmigable = _uow.TareaRepository.GetOrdenTareaFuncionarioAmigableByUserId(userId.Value);

            if (ordenTareaFuncAmigable == null && string.IsNullOrEmpty(field.Value))
                return null;

            var rules = new List<IValidationRule>
            {
                new NonNullValidationRule(field.Value),
                new DateTimeValidationRule(field.Value)
            };

            var numeroOrdenStr = form.GetField("numeroOrden").Value;
            var userLoginName = form.GetField("usuario").Value;

            if (!string.IsNullOrEmpty(numeroOrdenStr) && !string.IsNullOrEmpty(field.Value))
            {
                var nuOrden = int.Parse(numeroOrdenStr);
                var orden = this._uow.OrdenRepository.GetOrden(nuOrden);

                if (orden != null)
                {
                    var validarFecha = new FechaHastaOrdenTareaFuncionarioValidationRule(
                        this._uow,
                        form.GetField("fechaInicio").Value,
                        field.Value,
                        userId.ToString(),
                        this._identity.GetFormatProvider(),
                        ordenTareaFuncAmigable?.NuOrtOrdenTareaFuncionario.ToString()
                    );

                    rules.Add(validarFecha);
                }
            }

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
            };

        }
    }
}
