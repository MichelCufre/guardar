using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Recepcion;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Impresiones;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules
{
    public class ImpresionLpnValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly int _usuario;
        protected readonly string _predio;

        public ImpresionLpnValidationModule(IUnitOfWork uow, int usuarioLogueado, string predioLogueado)
        {
            this._uow = uow;
            this._usuario = usuarioLogueado;
            this._predio = predioLogueado;

            this.Schema = new FormValidationSchema
            {
                ["impresora"] = this.ValidateImpresora,
                ["predio"] = this.ValidatePredio,
                ["lenguaje"] = this.ValidateLenguaje,
                ["numCopias"] = this.ValidateCopias
            };
        }

        #region Predio

        public virtual FormValidationGroup ValidatePredio(FormField field, Form form, List<ComponentParameter> parameters)
        {

            List<IValidationRule> Rules = new List<IValidationRule> {
                new NonNullValidationRule(field.Value),
                new ExisteImpresoraPredioValidationRule(_uow, field.Value),
            };

            if (!this._predio.Equals(GeneralDb.PredioSinDefinir))
                Rules.Add(new ExistePredioValidationRule(_uow, this._usuario, this._predio, field.Value));


            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = Rules,
                OnSuccess = this.ValidatePredio_OnSucess,
                OnFailure = this.ValidatePredio_OnFailure
            };
        }

        public virtual void ValidatePredio_OnSucess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            FormField selectorImpresora = form.GetField("impresora");
            selectorImpresora.Options = new List<SelectOption>();

            List<Impresora> ListaImpresoras = _uow.ImpresoraRepository.GetListaImpresorasPredio(field.Value);

            foreach (var impresora in ListaImpresoras)
            {
                selectorImpresora.Options.Add(new SelectOption(impresora.Id, $"{impresora.Id} - {impresora.Descripcion}"));
            };
            form.GetField("impresora").ReadOnly = false;

            if (!parameters.Any(s => s.Id == "isSubmit"))
            {
                //Le sugiere la ultima impresora utilizada para ese predio si es posible
                Impresion impresion = _uow.ImpresionRepository.ObtenerImpresoraUltimaImpresion(this._usuario, field.Value);

                if (impresion != null && !string.IsNullOrEmpty(impresion.CodigoImpresora))
                {
                    if (_uow.ImpresoraRepository.ExisteImpresora(impresion.CodigoImpresora, impresion.Predio))//Por si se cambia el codigo de impresora
                    {
                        Impresora impresora = _uow.ImpresoraRepository.GetImpresora(impresion.CodigoImpresora, impresion.Predio);
                        var lenguajeImpresion = _uow.ImpresionRepository.GetLenguajeImpresion(impresora?.CodigoLenguajeImpresion);

                        form.GetField("impresora").Value = impresion.CodigoImpresora;
                        form.GetField("lenguaje").Value = lenguajeImpresion?.Id;
                        form.GetField("descripcionLenguaje").Value = lenguajeImpresion?.Descripcion;
                    }
                    form.GetField("impresora").ReadOnly = false;
                }
                else
                {
                    form.GetField("lenguaje").Value = string.Empty;
                    form.GetField("descripcionLenguaje").Value = string.Empty;
                }
            }
        }
        public virtual void ValidatePredio_OnFailure(FormField field, Form form, List<ComponentParameter> parameters)
        {
            form.GetField("impresora").ReadOnly = true;
            form.GetField("impresora").Value = string.Empty;
            form.GetField("lenguaje").Value = string.Empty;
            form.GetField("descripcionLenguaje").Value = string.Empty;
        }

        #endregion

        #region Impresora

        public virtual FormValidationGroup ValidateImpresora(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(form.GetField("predio").Value))
            {
                form.GetField("predio").SetError(new ComponentError("General_Sec0_Error_Error25", new List<string>()));
                return null;
            }
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new ImpresoraPertenecePredioValidatonRule(_uow,field.Value, this._predio)
                },
                OnSuccess = this.ValidateImpresora_OnSucess,
                OnFailure = this.ValidateImpresora_OnFailure,
            };
        }

        public virtual void ValidateImpresora_OnSucess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            string impresoraLenguaje = _uow.ImpresoraRepository.GetImpresora(field.Value, form.GetField("predio").Value).CodigoLenguajeImpresion;

            LenguajeImpresion lenguaje = _uow.ImpresionRepository.GetLenguajeImpresion(impresoraLenguaje);

            form.GetField("lenguaje").Value = lenguaje.Id;
            form.GetField("descripcionLenguaje").Value = lenguaje.Descripcion;

        }
        public virtual void ValidateImpresora_OnFailure(FormField field, Form form, List<ComponentParameter> parameters)
        {
            form.GetField("lenguaje").Value = string.Empty;
            form.GetField("descripcionLenguaje").Value = string.Empty;
        }

        #endregion

        #region Lenguaje 

        public virtual FormValidationGroup ValidateLenguaje(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                }
            };
        }

        #endregion

        #region Numero Copias
        
        public virtual FormValidationGroup ValidateCopias(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new PositiveIntMayorACeroValidationRule(field.Value)
                }
            };
        }
        
        #endregion

    }
}
