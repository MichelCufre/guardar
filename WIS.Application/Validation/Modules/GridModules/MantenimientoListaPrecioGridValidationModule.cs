using System;
using System.Collections.Generic;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Facturacion;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules
{
    public class MantenimientoListaPrecioGridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        public MantenimientoListaPrecioGridValidationModule(IUnitOfWork uow)
        {
            this._uow = uow;

            this.Schema = new GridValidationSchema
            {
                ["CD_LISTA_PRECIO"] = this.ValidateIdListaPrecio,
                ["DS_LISTA_PRECIO"] = this.ValidateDescipcionListaPrecio,
                ["CD_MONEDA"] = this.ValidateIdMoneda,
            };
        }

        public virtual GridValidationGroup ValidateIdListaPrecio(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {

            var rules = new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new PositiveIntValidationRule(cell.Value)
                }
            };

            if (row.IsNew)
            {
                rules.Rules.Add(new ExisteListaPrecioRegistradaValidationRule(this._uow, cell.Value));
            }

            return rules;
        }

        public virtual GridValidationGroup ValidateDescipcionListaPrecio(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new StringMaxLengthValidationRule(cell.Value, 50),
                    new StringSoloUpperValidationRule(cell.Value)
                }
            };
        }

        public virtual GridValidationGroup ValidateIdMoneda(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new StringMaxLengthValidationRule(cell.Value, 15),
                    new ExisteMonedaValidationRule(this._uow, cell.Value)
                },
                OnSuccess = this.OnSuccessGridValidateIdMoneda
            };
        }

        public virtual void OnSuccessGridValidateIdMoneda(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            Moneda moneda = this._uow.MonedaRepository.GetMoneda(cell.Value);

            row.GetCell("DS_MONEDA").Value = moneda?.Descripcion;
            row.GetCell("DS_SIMBOLO").Value = moneda?.Simbolo;
        }
    }
}
