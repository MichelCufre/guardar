using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.GridComponent.Validation
{
    public abstract class GridValidationModule : IGridValidationModule
    {
        public GridValidator Validator { get; set; }
        public GridValidationSchema Schema { get; set; }

        public GridValidationModule()
        {

        }

        public virtual void Validate(GridRow row)
        {
            if (row == null)
                throw new ArgumentNullException("No se especifico fila para validar");

            if (this.Validator == null)
                throw new InvalidOperationException("No se especifico validator");

            if (this.Schema == null)
                throw new InvalidOperationException("No se especifico esquema de validacion");

            this.Validator.Schema = this.Schema;

            this.Validator.Validate(row);
        }
    }
}
