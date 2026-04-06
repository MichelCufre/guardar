using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using WIS.Application.Validation.Rules;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Impresiones;
using WIS.Domain.Recepcion.Enums;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Security;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules.Recepcion
{
    public class REC171ImpresionEtiquetasValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IIdentityService _identity;

        public REC171ImpresionEtiquetasValidationModule(IUnitOfWork uow, IIdentityService identity)
        {
            _uow = uow;
            _identity = identity;

            Schema = new FormValidationSchema
            {
                ["impresora"] = this.ValidateImpresora,
                ["estilo"] = this.ValidateCampo,
                ["tipo_barras"] = this.ValidateCampo,
                ["modalidad"] = this.ValidateModalidad,
            };
        }

        public virtual FormValidationGroup ValidateImpresora(FormField field, Form form, List<ComponentParameter> parameters)
        {
            List<IValidationRule> rules = new List<IValidationRule>
            {
                new NonNullValidationRule (field.Value),
                new ImpresoraPertenecePredioValidatonRule (_uow, field.Value, _identity.Predio),
            };

            if (_identity.Predio == GeneralDb.PredioSinDefinir)
                rules.Add(new ExistePredioValidationRule(_uow, _identity.UserId, null, _identity.Predio));

            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = rules,
                OnSuccess = this.ValidateImpresora_OnSucess,
                OnFailure = this.ValidateImpresora_OnFailure,
            };
        }
        public virtual void ValidateImpresora_OnSucess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            string impresoraLenguaje = _uow.ImpresoraRepository.GetImpresora(field.Value, _identity.Predio)?.CodigoLenguajeImpresion;

            LenguajeImpresion lenguaje = _uow.ImpresionRepository.GetLenguajeImpresion(impresoraLenguaje);

            form.GetField("lenguaje").Value = lenguaje?.Id;
            form.GetField("desc_lenguaje").Value = lenguaje?.Descripcion;

            var campo = form.GetField("estilo");
            campo.Options = new List<SelectOption>();

            List<EtiquetaEstilo> listaEstilos = _uow.EstiloEtiquetaRepository.GetEtiquetaEstilos(EstiloEtiquetaDb.Producto, lenguaje?.Id);

            foreach (var estilo in listaEstilos)
            {
                campo.Options.Add(new SelectOption(estilo.Id, $"{estilo.Id} - {estilo.Descripcion}"));
            }
        }
        public virtual void ValidateImpresora_OnFailure(FormField field, Form form, List<ComponentParameter> parameters)
        {
            form.GetField("lenguaje").Value = string.Empty;
            form.GetField("desc_lenguaje").Value = string.Empty;
        }

        public virtual FormValidationGroup ValidateModalidad(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                },
                OnSuccess = this.ValidateModalidad_OnSucess,
            };
        }
        public virtual void ValidateModalidad_OnSucess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            Enum.TryParse(form.GetField("modalidad").Value, out ModalidadImpresion modalidad);

            List<string> keys = JsonConvert.DeserializeObject<List<string>>(parameters.Find(x => x.Id == "SELECTED_KEYS").Value);

            int cantidad = 0;

            foreach (string key in keys)
            {
                string[] decomposedKey = key.Split('$');
                int nuAgenda = Convert.ToInt32(decomposedKey[1]);
                decimal cdFaixa = decimal.Parse(decomposedKey[2], _identity.GetFormatProvider());
                string nuIdentificador = decomposedKey[3];
                string cdProduto = decomposedKey[4];
                int cdEmpresa = Convert.ToInt32(decomposedKey[5]);

                switch (modalidad)
                {
                    case ModalidadImpresion.Unidades:
                        cantidad += Convert.ToInt32(decimal.Parse(decomposedKey[6], _identity.GetFormatProvider()));
                        break;
                    case ModalidadImpresion.Embalajes:
                        cantidad += Convert.ToInt32(decimal.Parse(decomposedKey[7], _identity.GetFormatProvider()));
                        break;
                    case ModalidadImpresion.CampoVirtual:
                        cantidad += int.Parse(decomposedKey[0]);
                        break;
                }
            }

            parameters.Add(new ComponentParameter() { Id = "MENSAJE_IMPRESION", Value = "REC171_Sec0_Info_CantidadImpresiones" });
            parameters.Add(new ComponentParameter() { Id = "CANTIDAD_IMPRESION", Value = cantidad.ToString() });
        }

        public virtual FormValidationGroup ValidateCampo(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                },
            };
        }
    }
}
