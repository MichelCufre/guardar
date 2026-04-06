using System;
using System.Collections.Generic;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Facturacion;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.Facturacion;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules.Eventos
{
    public class RegistroCodigoFacturacionComponenteValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        public RegistroCodigoFacturacionComponenteValidationModule(IUnitOfWork uow)
        {
            this._uow = uow;

            this.Schema = new GridValidationSchema
            {
                ["CD_FACTURACION"] = this.ValidateCodigoFacturacion,
                ["NU_COMPONENTE"] = this.ValidateComponente,
                ["DS_SIGNIFICADO"] = this.ValidateSignificadoComponente,
                ["NU_CUENTA_CONTABLE"] = this.VlidateNumeroCuentaContable,


            };
        }

        public virtual GridValidationGroup ValidateCodigoFacturacion(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {

            var Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new StringSoloUpperValidationRule(cell.Value),
                    new StringMaxLengthValidationRule(cell.Value, 20),
                    new ExisteCodigoFacturacionComponente(this._uow, cell.Value),


                };




            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = Rules

            };

        }

        public virtual GridValidationGroup ValidateComponente(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            string numeroDeFactura = row.GetCell("CD_FACTURACION").Value;

            var Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new StringSoloUpperValidationRule(cell.Value),
                    new StringMaxLengthValidationRule(cell.Value, 20),

                };

            if (row.IsNew)
            {
                Rules.Add(new FacturaTieneComponenteValidationRule(cell.Value, numeroDeFactura, this._uow));
            }

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = Rules

            };
        }
        public virtual GridValidationGroup ValidateSignificadoComponente(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {

            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new StringMaxLengthValidationRule(cell.Value, 100),
                    new StringDescripcionSoloUpperValidationRule(cell.Value),
                },

            };
        }
        public virtual GridValidationGroup VlidateNumeroCuentaContable(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {

            ExisteNumeroCuentaContable ExisteNumeroCuentaContable = new ExisteNumeroCuentaContable(this._uow, cell.Value);
            var Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new StringSoloUpperValidationRule(cell.Value),
                    new StringMaxLengthValidationRule(cell.Value, 10),


                };


            if (row.IsNew)
            {

                Rules.Add(ExisteNumeroCuentaContable);
            }
            else
            {
                Rules.Remove(ExisteNumeroCuentaContable);
            }
            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = Rules,
                OnSuccess = this.OnSuccessGridValidateCuentaContable
            };

        }

        public virtual void OnSuccessGridValidateCuentaContable(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            CuentaContable cuentaContable = this._uow.CuentaContableRepository.GetCuentaContable(cell.Value);
            var cellDescripcion = row.GetCell("DS_CUENTA_CONTABLE");

            if (cellDescripcion != null)
                cellDescripcion.Value = cuentaContable?.Descripcion;

        }
    }
}