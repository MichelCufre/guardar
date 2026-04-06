using System.Collections.Generic;
using WIS.Application.Validation.Rules;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.GridComponent;
using WIS.GridComponent.Validation;

namespace WIS.Application.Validation.Modules.GridModules
{
    public class STO150GridValidationModule : GridValidationModule
    {
        protected IUnitOfWork _uow;

        public STO150GridValidationModule(IUnitOfWork uow)
        {
            this._uow = uow;
            this.Schema = new GridValidationSchema
            {
                ["DT_VENCIMIENTO"] = this.ValidateVencimiento,
                ["ID_AVERIA"] = this.ValidateAveria,
                ["NU_DOMINIO"] = this.ValidateDominio
            };
        }

        public virtual GridValidationGroup ValidateDominio(GridCell cell, GridRow row, List<ComponentParameter> list)
        {
            string averiado = row.GetCell("ID_AVERIA").Value;
            if (averiado == "N")
                return null ;

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = {
                     new NonNullValidationRule(cell.Value),
                }
            };
        }

        public virtual GridValidationGroup ValidateVencimiento(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (cell.Value == null)
                return null;

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = {
                    new DateTimeValidationRule(cell.Value)
                }
            };
        }
        
        public virtual GridValidationGroup ValidateAveria(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            string ubicacion = row.GetCell("CD_ENDERECO").Value;
            string producto= row.GetCell("CD_PRODUTO").Value;
            string identificador = row.GetCell("NU_IDENTIFICADOR").Value;
            int empresa = int.Parse(row.GetCell("CD_EMPRESA").Value);
            decimal faixa = decimal.Parse(row.GetCell("CD_FAIXA").Value);

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = {
                    new NonNullValidationRule(cell.Value),
                    new AveriaStockValidationRule(cell.Value),
                },OnSuccess = this.ValidateColumnaAveria
            };
        }

        public virtual void ValidateColumnaAveria(GridCell cell, GridRow row, List<ComponentParameter> list)
        {
            if (cell.Value =="S")
            {
                row.GetCell("NU_DOMINIO").Editable = true;
            }
            else
            {
                row.GetCell("NU_DOMINIO").Editable = false;
                row.GetCell("NU_DOMINIO").Value= string.Empty;
            }
        }
    }
}
