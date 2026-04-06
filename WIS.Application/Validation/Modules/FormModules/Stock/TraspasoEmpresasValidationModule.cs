using DocumentFormat.OpenXml.InkML;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.General;
using WIS.Application.Validation.Rules.Stock;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.Domain.StockEntities.Constants;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.FormComponent.Validation;
using WIS.Persistence.Database;
using WIS.Security;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules.Stock
{
    public class TraspasoEmpresasValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;

        public TraspasoEmpresasValidationModule(IUnitOfWork uow, IIdentityService identity, ISecurityService security)
        {
            this.Schema = new FormValidationSchema
            {
                ["idExterno"] = this.ValidateIdExterno,
                ["descripcion"] = this.ValidateDescripcion,
                ["cdEmpresaOrigen"] = this.ValidateEmpresaOrigen,
                ["cdEmpresaDestino"] = this.ValidateEmpresaDestino,
                ["tpTraspaso"] = this.ValidateTipoTraspaso,
                ["flFinalizarConPreparacion"] = this.ValidateFinalizarConPreparacion,
                ["flPropagarLPN"] = this.ValidatePropagarLPN,
            };

            this._uow = uow;
            this._identity = identity;
            this._security = security;
        }

        public virtual FormValidationGroup ValidateIdExterno(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.ReadOnly)
                return null;

            return new FormValidationGroup
            {
                Rules = new List<IValidationRule>
                {
                    new StringMaxLengthValidationRule(field.Value, 50)
                }
            };
        }

        public virtual FormValidationGroup ValidateDescripcion(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.ReadOnly)
                return null;

            return new FormValidationGroup
            {
                Rules = new List<IValidationRule>
                {
                    new StringMaxLengthValidationRule(field.Value, 100)
                }
            };
        }

        public virtual FormValidationGroup ValidateEmpresaOrigen(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.ReadOnly)
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
                   new EmpresaNoTieneConfiguracionTraspasoValidationRule(this._uow, field.Value),
                },
                OnSuccess = this.OnSuccessValidateEmpresaOrigen
            };
        }

        public virtual void OnSuccessValidateEmpresaOrigen(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (int.TryParse(field.Value, out int cdEmpresa))
            {
                var config = this._uow.TraspasoEmpresasRepository.GetConfiguracionTraspasoByEmpresa(cdEmpresa);

                if (config != null)
                {
                    var selectEmpresaDestino = form.GetField("cdEmpresaDestino");
                    var selectTipoTraspaso = form.GetField("tpTraspaso");

                    selectEmpresaDestino.Disabled = false;
                    selectEmpresaDestino.ReadOnly = false;
                    selectTipoTraspaso.Disabled = false;

                    selectTipoTraspaso.Options = new List<SelectOption>();

                    if (config.TodoTipoTraspaso)
                    {
                        _uow.TraspasoEmpresasRepository.GetTiposTraspaso().ForEach(w =>
                        {
                            selectTipoTraspaso.Options.Add(new SelectOption(w.Key, w.Value));
                        });
                    }
                    else
                    {
                        _uow.TraspasoEmpresasRepository.GetTiposTraspaso(config.Id).ForEach(w =>
                        {
                            selectTipoTraspaso.Options.Add(new SelectOption(w.Key, w.Value));
                        });
                    }

                    form.GetField("flReplicaProductos").Value = config.ReplicaProductos.ToString();
                    form.GetField("flReplicaCB").Value = config.ReplicaCodBarras.ToString();
                    form.GetField("flCtrlCaractIguales").Value = config.ControlaCaractIguales.ToString();
                    form.GetField("flReplicaAgentes").Value = config.ReplicaAgentes.ToString();

                    var fieldEmpresa = form.GetField("cdEmpresaDestino");

                    if (config.TodoTipoTraspaso)
                    {
                        var empresas = _uow.EmpresaRepository.GetEmpresasByUsuario(_identity.UserId);

                        if (empresas != null && empresas.Count == 1)
                        {
                            fieldEmpresa.Options = new List<SelectOption>() { new SelectOption(empresas.FirstOrDefault().Id.ToString(), $"{empresas.FirstOrDefault().Id} - {empresas.FirstOrDefault().Nombre}" )};

                            fieldEmpresa.Value = empresas.FirstOrDefault().ToString();
                        }
                    }
                    else
                    {
                        List<Empresa> empresas = _uow.TraspasoEmpresasRepository.GetEmpresasByConfiguracionEmpresa(config.Id);

                        if (empresas != null && empresas.Count == 1)
                        {
                            fieldEmpresa.Options = new List<SelectOption>() { new SelectOption(empresas.FirstOrDefault().Id.ToString(), $"{empresas.FirstOrDefault().Id} - {empresas.FirstOrDefault().Nombre}") }; ;

                            fieldEmpresa.Value = empresas.FirstOrDefault().Id.ToString();
                        }
                    }

                }
            }
        }

        public virtual FormValidationGroup ValidateEmpresaDestino(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.ReadOnly)
                return null;

            int.TryParse(form.GetField("cdEmpresaOrigen").Value, out int empresa);
            int.TryParse(form.GetField("cdEmpresaDestino").Value, out int empresaDestino);

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
                   new TraspasoEmpresasDestinoDocumentalValidationRule(this._uow,empresa,empresaDestino)
                }
            };
        }

        public virtual FormValidationGroup ValidateTipoTraspaso(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.ReadOnly)
                return null;

            var rules = new List<IValidationRule>
            {
                new NonNullValidationRule(field.Value),
                new NoExisteTipoTraspasoValidationRule(this._uow, field.Value),
            };

            if (int.TryParse(form.GetField("cdEmpresaOrigen").Value, out int empresa))
            {
                var config = this._uow.TraspasoEmpresasRepository.GetConfiguracionTraspasoByEmpresa(empresa);
                if (config != null && !config.TodoTipoTraspaso)
                    rules.Add(new TipoTraspasoAsociadoValidationRule(this._uow, field.Value, config.Id));
            }

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
                OnSuccess = this.OnSuccessValidateTipoTraspaso
            };
        }

        public virtual void OnSuccessValidateTipoTraspaso(FormField field, Form form, List<ComponentParameter> list)
        {
            var flFinalizarConPreparacion = form.GetField("flFinalizarConPreparacion");
            if (field.Value != TipoTraspasoDb.TraspasoPda)
            {
                flFinalizarConPreparacion.Disabled = true;
                flFinalizarConPreparacion.Value = "false";
            }
            else
            {
                flFinalizarConPreparacion.Disabled = false;
            }
        }

        public virtual FormValidationGroup ValidateFinalizarConPreparacion(FormField field, Form form, List<ComponentParameter> parameters)
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

        public virtual FormValidationGroup ValidatePropagarLPN(FormField field, Form form, List<ComponentParameter> parameters)
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
