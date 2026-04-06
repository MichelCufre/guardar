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

namespace WIS.Application.Validation.Modules.GridModules
{
    public class MantenimientoPrecioEmpresaGridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        public MantenimientoPrecioEmpresaGridValidationModule(IUnitOfWork uow)
        {
            this._uow = uow;

            this.Schema = new GridValidationSchema
            {
                ["CD_LISTA_PRECIO"] = this.ValidateListaPrecio,
            };
        }
        
        public virtual GridValidationGroup ValidateListaPrecio(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new ExisteListaPrecioEmpresaValidationRule(this._uow, int.Parse(row.GetCell("CD_EMPRESA").Value), int.Parse(cell.Value))
                },
                OnSuccess = this.ValidateListaPrecioOnSuccess
            };
        }

        public virtual void ValidateListaPrecioOnSuccess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            ListaPrecio listaPrecio = this._uow.ListaPrecioRepository.GetListaPrecio(int.Parse(cell.Value));

            row.GetCell("DS_LISTA_PRECIO").Value = listaPrecio.Descripcion;
            row.GetCell("CD_MONEDA").Value = listaPrecio.IdMoneda;
        }
    }
}
