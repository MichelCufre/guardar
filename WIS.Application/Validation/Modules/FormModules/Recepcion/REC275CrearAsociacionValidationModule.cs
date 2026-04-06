using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Documento;
using WIS.Application.Validation.Rules.Registro;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Documento.Constants;
using WIS.Domain.General;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Persistence.Database;
using WIS.Security;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules.Documento
{
    public class REC275CrearAsociacionValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IIdentityService _identity;
        protected readonly IFormatProvider _culture;

        public REC275CrearAsociacionValidationModule(
            IUnitOfWork uow,
            IIdentityService identity)
        {
            this._uow = uow;
            this._identity = identity;
            this._culture = identity.GetFormatProvider();

            Schema = new FormValidationSchema
            {
                ["estrategia"] = this.ValidateEstrategia,
                ["predio"] = this.ValidatePredio,
                ["asociacion"] = this.ValidateAsociacion,
                ["clase"] = this.ValidateClase,
                ["grupo"] = this.ValidateGrupo,
                ["empresa"] = this.ValidateEmpresa,
                ["producto"] = this.ValidateProducto,
            };
        }

        public virtual FormValidationGroup ValidateEstrategia(FormField field, Form form, List<ComponentParameter> parameters)
        {

            var Rules = new List<IValidationRule> {
                    new NonNullValidationRule(field.Value),
            };

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = Rules,
            };

        }

        public virtual FormValidationGroup ValidatePredio(FormField field, Form form, List<ComponentParameter> parameters)
        {

            var Rules = new List<IValidationRule> {
                    new NonNullValidationRule(field.Value),
            };

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = Rules,
            };

        }

        public virtual FormValidationGroup ValidateAsociacion(FormField field, Form form, List<ComponentParameter> parameters)
        {

            switch (field.Value)
            {
                case AlmacenamientoDb.TIPO_ENTIDAD_CLASE:

                    return new FormValidationGroup
                    {
                        Rules = new List<IValidationRule>
                            {
                                new NonNullValidationRule(field.Value)
                            },
                        OnSuccess = this.ValidateAsociacionOnSucces
                    };

                case AlmacenamientoDb.TIPO_ENTIDAD_GRUPO:

                    return new FormValidationGroup
                    {
                        Rules = new List<IValidationRule>
                            {
                                new NonNullValidationRule(field.Value)
                            },
                        OnSuccess = this.ValidateAsociacionOnSucces
                    };

                case AlmacenamientoDb.TIPO_ENTIDAD_PRODUCTO:

                    return new FormValidationGroup
                    {
                        Rules = new List<IValidationRule>
                            {
                                new NonNullValidationRule(field.Value)
                            },
                        OnSuccess = this.ValidateAsociacionOnSucces
                    };

                default:
                    return null;
            }
        }

        public virtual void ValidateAsociacionOnSucces(FormField field, Form form, List<ComponentParameter> parameters)
        {
            
            if (field.Value == AlmacenamientoDb.TIPO_ENTIDAD_CLASE)
                this.AgregarParametroValidation(parameters, "Clase", "true");

            if (field.Value == AlmacenamientoDb.TIPO_ENTIDAD_GRUPO)
                this.AgregarParametroValidation(parameters, "Grupo", "true");

            if (field.Value == AlmacenamientoDb.TIPO_ENTIDAD_PRODUCTO)
                this.AgregarParametroValidation(parameters, "Producto", "true");

        }

        public virtual FormValidationGroup ValidateClase(FormField field, Form form, List<ComponentParameter> parameters)
        {
            string asociacion = form.GetField("asociacion").Value;
            NonNullValidationRule validacionNoNulla = new NonNullValidationRule(field.Value);

            var Rules = new List<IValidationRule>();

            if (asociacion == AlmacenamientoDb.TIPO_ENTIDAD_CLASE)
                Rules.Add(validacionNoNulla);
            else
                Rules.Remove(validacionNoNulla);

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = Rules,
            };

        }

        public virtual FormValidationGroup ValidateGrupo(FormField field, Form form, List<ComponentParameter> parameters)
        {
            string asociacion = form.GetField("asociacion").Value;
            NonNullValidationRule validacionNoNulla = new NonNullValidationRule(field.Value);

            var Rules = new List<IValidationRule>();

            if (asociacion == AlmacenamientoDb.TIPO_ENTIDAD_GRUPO)
                Rules.Add(validacionNoNulla);
            else
                Rules.Remove(validacionNoNulla);

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = Rules,
            };

        }
        public virtual FormValidationGroup ValidateEmpresa(FormField field, Form form, List<ComponentParameter> parameters)
        {
            string asociacion = form.GetField("asociacion").Value;
            NonNullValidationRule validacionNoNulla = new NonNullValidationRule(field.Value);

            var Rules = new List<IValidationRule>();

            if (asociacion == AlmacenamientoDb.TIPO_ENTIDAD_PRODUCTO)
                Rules.Add(validacionNoNulla);
            else
                Rules.Remove(validacionNoNulla);

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = Rules
            };

        }


        public virtual FormValidationGroup ValidateProducto(FormField field, Form form, List<ComponentParameter> parameters)
        {
            string asociacion = form.GetField("asociacion").Value;
            string empresa = form.GetField("empresa").Value;

            NonNullValidationRule validacionNoNulla = new NonNullValidationRule(field.Value);
            ProductoExisteEmpresaValidationRule existeProductoEmpresa = new ProductoExisteEmpresaValidationRule(_uow, empresa, field.Value);
            
            var Rules = new List<IValidationRule>();

            if (asociacion == AlmacenamientoDb.TIPO_ENTIDAD_PRODUCTO)
                Rules.Add(validacionNoNulla);
            else
                Rules.Remove(validacionNoNulla);


            if(!string.IsNullOrEmpty(empresa))
                Rules.Add(existeProductoEmpresa);
            else
                Rules.Remove(existeProductoEmpresa);

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = Rules,
            };

        }
        public virtual void AgregarParametroValidation(List<ComponentParameter> parameters, string Id, string value)
        {
            ComponentParameter genericParam = new ComponentParameter()
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
                {
                    parameters.FirstOrDefault(p => p.Id == Id).Value = genericParam.Value;
                }
                else
                {
                    parameters.Add(genericParam);
                }
            }
        }
    }
}
