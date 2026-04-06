using System;
using System.Collections.Generic;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Facturacion;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules.Eventos
{
    
    public class RegistroCodigoFacturacionValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        public RegistroCodigoFacturacionValidationModule(IUnitOfWork uow)
        {
            this._uow = uow;

            this.Schema = new GridValidationSchema
            {
                ["CD_FACTURACION"] = this.ValidateCodigoFacturacion,
                ["DS_FACTURACION"] = this.ValidateDescripcion,
                ["TP_CALCULO"] = this.ValidateTipo,

            };
        }

        public virtual GridValidationGroup ValidateCodigoFacturacion(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {

            ExisteCodigoFacturacion ExisteCodigoFacuracion = new ExisteCodigoFacturacion(this._uow, cell.Value);
            var Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new StringSoloUpperValidationRule(cell.Value),
                    new StringMaxLengthValidationRule(cell.Value, 20),
                    

                };

            
            if (row.IsNew)
            {
                
                Rules.Add(ExisteCodigoFacuracion);
            }
            else
            {
                Rules.Remove(ExisteCodigoFacuracion);
            }


            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = Rules
                
            };

        }

        public virtual GridValidationGroup ValidateDescripcion(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new StringMaxLengthValidationRule(cell.Value, 100),
                    new StringDescripcionSoloUpperValidationRule(cell.Value)
                }
            };
        }

        public virtual GridValidationGroup ValidateTipo(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new StringMaxLengthValidationRule(cell.Value, 1),
                    new ExisteTipoCalculoValidationRule(this._uow, cell.Value),
                },
                
            };
        }

     
    }
}
