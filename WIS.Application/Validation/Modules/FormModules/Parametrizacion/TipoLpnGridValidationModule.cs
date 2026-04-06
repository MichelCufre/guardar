using System;
using System.Collections.Generic;
using WIS.Application.Validation.Rules;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Security;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules
{
    public class TipoLpnGridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IFormatProvider _culture;

        public TipoLpnGridValidationModule(IUnitOfWork uow, IIdentityService identity, ISecurityService security)
        {
            this._uow = uow;
            this._identity = identity;
            this._security = security;
            this._culture = identity.GetFormatProvider();
            this.Schema = new GridValidationSchema
            {
                ["TP_LPN_TIPO"] = this.ValidateTipoLpn,
                ["NM_LPN_TIPO"] = this.ValidateNombreTipoLpn,
                ["DS_LPN_TIPO"] = this.ValidateDescripcionTipoLpn,
                ["NU_COMPONENTE"] = this.ValidateComponente,
                ["NU_TEMPLATE_ETIQUETA"] = this.ValidateTemplate,
                ["NU_SEQ_LPN"] = this.ValidateNumeroInicioSecuencia,
                ["FL_PERMITE_GENERAR"] = this.ValidatePermiteGenerar,
                ["VL_PREFIJO"] = this.ValidatePrefijo,
                ["FL_CONTENEDOR_LPN"] = this.ValidateContenedorLPN,
                ["FL_PERMITE_AGREGAR_LINEAS"] = this.ValidatePermiteAgregarLineas,
            };
        }

        public virtual GridValidationGroup ValidateTipoLpn(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(cell.Value),
                   new StringMaxLengthValidationRule(cell.Value, 10),
                }
            };
        }

        public virtual GridValidationGroup ValidateNombreTipoLpn(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(cell.Value),
                   new StringMaxLengthValidationRule(cell.Value, 30),
                }
            };
        }

        public virtual GridValidationGroup ValidateDescripcionTipoLpn(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(cell.Value),
                   new StringMaxLengthValidationRule(cell.Value, 400),
                }
            };
        }

        public virtual GridValidationGroup ValidateComponente(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(cell.Value))
                return null;

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(cell.Value),
                   new StringMaxLengthValidationRule(cell.Value, 10),
                }
            };
        }

        public virtual GridValidationGroup ValidateTemplate(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(cell.Value),
                   new StringMaxLengthValidationRule(cell.Value, 15),
                }
            };
        }

        public virtual GridValidationGroup ValidateNumeroInicioSecuencia(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var permiteGenerar = row.GetCell("FL_PERMITE_GENERAR").Value;

            if (permiteGenerar == "N")
                return null;

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(cell.Value),                                           
                   new StringMaxLengthValidationRule(cell.Value,15),
                   new PositiveLongValidationRule(cell.Value)
                }
            };
        }

        public virtual GridValidationGroup ValidatePermiteGenerar(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (cell.Value == "N")
            {
                row.GetCell("NU_SEQ_LPN").Editable = false;
                row.GetCell("NU_SEQ_LPN").Value = "";
            }
            else
                row.GetCell("NU_SEQ_LPN").Editable = true;

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                }
            };
        }

        public virtual GridValidationGroup ValidatePrefijo(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var rules = new List<IValidationRule>
            {
                new NonNullValidationRule(cell.Value),
                new StringMaxLengthValidationRule(cell.Value, 3),
                new StringMinLengthValidationRule(cell.Value, 3),
            };

            if (cell.Old != cell.Value)
                rules.Add(new TipoEtiquetaValidationRule(cell.Value, _uow));

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules
            };
        }

        public virtual GridValidationGroup ValidateContenedorLPN(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(cell.Value),
                },
                OnSuccess = ValidateContenedorLPN_OnSucess
            };
        }

        public virtual void ValidateContenedorLPN_OnSucess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var permiteAgregarLineas = row.GetCell("FL_PERMITE_AGREGAR_LINEAS");

            if (cell.Value == "S")
            {
                permiteAgregarLineas.Value = "S";
            }
        }

        public virtual GridValidationGroup ValidatePermiteAgregarLineas(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(cell.Value),
                },  
            };
        }
    }
}
