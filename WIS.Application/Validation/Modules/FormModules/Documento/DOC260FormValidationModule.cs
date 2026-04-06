using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Documento;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.Documento.Constants;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Security;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules.Documento
{
    public class DOC260FormValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IIdentityService _identity;
        protected readonly IFormatProvider _culture;

        public DOC260FormValidationModule(
            IUnitOfWork uow,
            IIdentityService identity)
        {
            this._uow = uow;
            this._identity = identity;
            this._culture = identity.GetFormatProvider();

            Schema = new FormValidationSchema
            {
                ["nuevoEstadoDocumento"] = this.ValidateID_ESTADO,
                ["tpDua"] = this.ValidateTP_DUA,
                ["nroDua"] = this.ValidateNU_DUA,
                ["fechVerificadoDua"] = this.ValidateDT_VERIFICADO,
                ["nroDTI"] = this.ValidateNU_DTI,
                ["fechDTI"] = this.ValidateDT_DTI,
                ["nroFactura"] = this.ValidateNU_FACTURA,
                ["nroAgenda"] = this.ValidateNU_AGENDA,
                ["tpRefExterna"] = this.ValidateTP_REF_EXTERNA,
                ["nroRefExterna"] = this.ValidateNU_REF_EXTERNA,
                ["fechRefExterna"] = this.ValidateDT_REF_EXTERNA,
            };
        }

        public virtual FormValidationGroup ValidateID_ESTADO(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var nuDocumento = parameters.FirstOrDefault(p => p.Id == "NU_DOCUMENTO").Value;
            var tpDocumento = parameters.FirstOrDefault(p => p.Id == "TP_DOCUMENTO").Value;
            var tipoDocumento = this._uow.DocumentoTipoRepository.GetTipoDocumento(tpDocumento);

            ValidateParameters(field.Value, parameters);

            switch (tipoDocumento.TipoOperacion)
            {
                case TipoDocumentoOperacion.INGRESO:

                    return new FormValidationGroup
                    {
                        Rules = new List<IValidationRule>
                        {
                            new PuedeIngresoCambiarEstadoValidationRule(this._uow, nuDocumento, tpDocumento, field.Value, this._identity.UserId)
                        }
                    };

                case TipoDocumentoOperacion.EGRESO:

                    return new FormValidationGroup
                    {
                        Rules = new List<IValidationRule>
                        {
                            new PuedeEgresoCambiarEstadoValidationRule(this._uow, nuDocumento, tpDocumento, field.Value, this._identity.UserId)
                        }
                    };

                case TipoDocumentoOperacion.MODIFICACION:

                    return new FormValidationGroup
                    {
                        Rules = new List<IValidationRule>
                        {
                            new PuedeActaCambiarEstadoValidationRule(this._uow, nuDocumento, tpDocumento, field.Value, this._identity.UserId)
                        }
                    };

                default:
                    return null;
            }
        }

        public virtual FormValidationGroup ValidateTP_DUA(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var tpDocumento = parameters.FirstOrDefault(p => p.Id == "TP_DOCUMENTO").Value;
            var estadoDestino = form.GetField("nuevoEstadoDocumento").Value;

            ValidateParameters(estadoDestino, parameters);

            if (this._uow.DocumentoTipoRepository.RequiereDUA(tpDocumento, estadoDestino))
                return new FormValidationGroup
                {
                    BreakValidationChain = true,
                    Rules = new List<IValidationRule>
                    {
                        new NonNullValidationRule(field.Value),
                        new StringMaxLengthValidationRule(field.Value,6),
                        new ExisteTipoDuaValidationRule(this._uow, tpDocumento, field.Value)
                    }
                };
            else
                return null;
        }

        public virtual FormValidationGroup ValidateNU_DUA(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var tpDocumento = parameters.FirstOrDefault(p => p.Id == "TP_DOCUMENTO").Value;
            var estadoDestino = form.GetField("nuevoEstadoDocumento").Value;

            ValidateParameters(estadoDestino, parameters);

            if (this._uow.DocumentoTipoRepository.RequiereDUA(tpDocumento, estadoDestino))
                return new FormValidationGroup
                {
                    BreakValidationChain = true,
                    Rules = new List<IValidationRule>
                    {
                        new NonNullValidationRule(field.Value),
                        new StringMaxLengthValidationRule(field.Value,15)
                    }
                };
            else
                return null;
        }

        public virtual FormValidationGroup ValidateTP_REF_EXTERNA(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var tpDocumento = parameters.FirstOrDefault(p => p.Id == "TP_DOCUMENTO").Value;
            var estadoDestino = form.GetField("nuevoEstadoDocumento").Value;

            ValidateParameters(estadoDestino, parameters);

            if (this._uow.DocumentoTipoRepository.RequiereReferenciaExterna(tpDocumento, estadoDestino))
                return new FormValidationGroup
                {
                    BreakValidationChain = true,
                    Rules = new List<IValidationRule>
                    {
                        new NonNullValidationRule(field.Value),
                        new StringMaxLengthValidationRule(field.Value,6),
                        new ExisteTipoReferenciaExternaValidationRule(this._uow, tpDocumento, field.Value)
                    }
                };
            else
                return null;
        }

        public virtual FormValidationGroup ValidateNU_REF_EXTERNA(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var tpDocumento = parameters.FirstOrDefault(p => p.Id == "TP_DOCUMENTO").Value;
            var estadoDestino = form.GetField("nuevoEstadoDocumento").Value;

            ValidateParameters(estadoDestino, parameters);

            if (this._uow.DocumentoTipoRepository.RequiereReferenciaExterna(tpDocumento, estadoDestino))
                return new FormValidationGroup
                {
                    BreakValidationChain = true,
                    Rules = new List<IValidationRule>
                    {
                        new NonNullValidationRule(field.Value),
                        new StringMaxLengthValidationRule(field.Value,30)
                    }
                };
            else
                return null;
        }

        public virtual FormValidationGroup ValidateNU_DTI(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var tpDocumento = parameters.FirstOrDefault(p => p.Id == "TP_DOCUMENTO").Value;
            var estadoDestino = form.GetField("nuevoEstadoDocumento").Value;

            ValidateParameters(estadoDestino, parameters);

            if (this._uow.DocumentoTipoRepository.RequiereDTI(tpDocumento, estadoDestino))
                return new FormValidationGroup
                {
                    BreakValidationChain = true,
                    Rules = new List<IValidationRule>
                    {
                        new NonNullValidationRule(field.Value),
                        new StringMaxLengthValidationRule(field.Value,30)
                    }
                };
            else
                return null;
        }

        public virtual FormValidationGroup ValidateNU_FACTURA(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var tpDocumento = parameters.FirstOrDefault(p => p.Id == "TP_DOCUMENTO").Value;
            var estadoDestino = form.GetField("nuevoEstadoDocumento").Value;

            ValidateParameters(estadoDestino, parameters);

            if (this._uow.DocumentoTipoRepository.RequiereFactura(tpDocumento, estadoDestino))
                return new FormValidationGroup
                {
                    BreakValidationChain = true,
                    Rules = new List<IValidationRule>
                    {
                       new NonNullValidationRule(field.Value),
                       new StringMaxLengthValidationRule(field.Value,200)
                    }
                };
            else
                return null;
        }

        public virtual FormValidationGroup ValidateNU_AGENDA(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var nuDocumento = parameters.FirstOrDefault(p => p.Id == "NU_DOCUMENTO").Value;
            var tpDocumento = parameters.FirstOrDefault(p => p.Id == "TP_DOCUMENTO").Value;
            var documento = this._uow.DocumentoRepository.GetDocumento(nuDocumento, tpDocumento);
            var estadoDestino = form.GetField("nuevoEstadoDocumento").Value;

            ValidateParameters(estadoDestino, parameters);

            if (this._uow.DocumentoTipoRepository.RequiereAgenda(tpDocumento, estadoDestino)
                && !this._uow.DocumentoTipoRepository.IsAutoAgendable(tpDocumento))
                return new FormValidationGroup
                {
                    BreakValidationChain = true,
                    Rules = new List<IValidationRule>
                    {
                       new NonNullValidationRule(field.Value),
                       new PositiveIntValidationRule(field.Value),
                       new AgendaDocumentableValidationRule(this._uow, documento.Empresa, field.Value)
                    }
                };
            else
                return null;
        }

        public virtual FormValidationGroup ValidateDT_VERIFICADO(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var tpDocumento = parameters.FirstOrDefault(p => p.Id == "TP_DOCUMENTO").Value;
            var estadoDestino = form.GetField("nuevoEstadoDocumento").Value;

            ValidateParameters(estadoDestino, parameters);

            if (this._uow.DocumentoTipoRepository.RequiereDUA(tpDocumento, estadoDestino))
                return new FormValidationGroup
                {
                    BreakValidationChain = true,
                    Rules = new List<IValidationRule>
                    {
                        new NonNullValidationRule(field.Value),
                        new StringIsoDateToDateTimeValidationRule(field.Value)
                    }
                };
            else
                return null;
        }

        public virtual FormValidationGroup ValidateDT_DTI(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var tpDocumento = parameters.FirstOrDefault(p => p.Id == "TP_DOCUMENTO").Value;
            var estadoDestino = form.GetField("nuevoEstadoDocumento").Value;

            ValidateParameters(estadoDestino, parameters);

            if (this._uow.DocumentoTipoRepository.RequiereDTI(tpDocumento, estadoDestino))
                return new FormValidationGroup
                {
                    BreakValidationChain = true,
                    Rules = new List<IValidationRule>
                    {
                        new NonNullValidationRule(field.Value),
                        new StringIsoDateToDateTimeValidationRule(field.Value)
                    }
                };
            else
                return null;
        }

        public virtual FormValidationGroup ValidateDT_REF_EXTERNA(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var tpDocumento = parameters.FirstOrDefault(p => p.Id == "TP_DOCUMENTO").Value;
            var estadoDestino = form.GetField("nuevoEstadoDocumento").Value;

            ValidateParameters(estadoDestino, parameters);

            if (this._uow.DocumentoTipoRepository.RequiereReferenciaExterna(tpDocumento, estadoDestino))
                return new FormValidationGroup
                {
                    BreakValidationChain = true,
                    Rules = new List<IValidationRule>
                    {
                        new NonNullValidationRule(field.Value),
                        new StringIsoDateToDateTimeValidationRule(field.Value)
                    }
                };
            else
                return null;
        }

        public virtual void ValidateParameters(string estadoDestino, List<ComponentParameter> parameters)
        {
            var tpDocumento = parameters.FirstOrDefault(p => p.Id == "TP_DOCUMENTO").Value;

            if (this._uow.DocumentoTipoRepository.RequiereReferenciaExterna(tpDocumento, estadoDestino))
                this.AgregarParametroValidation(parameters, "RefExternaRequired", "true");

            if (this._uow.DocumentoTipoRepository.RequiereDTI(tpDocumento, estadoDestino))
                this.AgregarParametroValidation(parameters, "DTIRequired", "true");

            if (this._uow.DocumentoTipoRepository.RequiereDUA(tpDocumento, estadoDestino))
                this.AgregarParametroValidation(parameters, "DUARequired", "true");

            if (this._uow.DocumentoTipoRepository.RequiereFactura(tpDocumento, estadoDestino))
                this.AgregarParametroValidation(parameters, "facturaRequiered", "true");

            if (this._uow.DocumentoTipoRepository.RequiereAgenda(tpDocumento, estadoDestino)
                && !this._uow.DocumentoTipoRepository.IsAutoAgendable(tpDocumento))
                this.AgregarParametroValidation(parameters, "agendaRequiered", "true");
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
