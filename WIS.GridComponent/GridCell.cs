using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using WIS.Components.Common;
using WIS.GridComponent.Columns;

namespace WIS.GridComponent
{
    public class GridCell
    {
        [JsonConverter(typeof(GridCellColumnConverter))]
        public IGridColumn Column { get; set; }

        [JsonIgnore]
        private bool IsOldSet { get; set; }
        [JsonIgnore]
        private string OldValue { get; set; }

        public string Value { get; set; }
        public bool Editable { get; set; }
        public string CssClass { get; set; }
        public GridStatus Status { get; set; }
        public ComponentError Error { get; set; }
        public bool Modified { get; set; }
        public string Old
        {
            get { return this.OldValue; }
            set
            {
                if (this.IsOldSet) throw new Exception("Valor ya seteado");

                this.IsOldSet = true;

                this.OldValue = value;
            }
        }

        [JsonIgnore]
        public bool IsValidated { get; set; }

        public GridCell()
        {
            this.Column = null;
            this.Value = string.Empty;
            this.Editable = false;
            this.CssClass = null;
            this.Status = GridStatus.Ok;
            this.Error = null;
            this.Modified = false;
            this.IsValidated = false;
        }

        public GridCell(string value)
        {
            this.Column = null;
            this.Value = value;
            this.Old = value;
            this.Editable = false;
            this.CssClass = null;
            this.Status = GridStatus.Ok;
            this.Error = null;
            this.Modified = false;
            this.IsValidated = false;
        }

        public void SetError(string message, List<string> arguments)
        {
            this.Status = GridStatus.Error;
            this.Error = new ComponentError(message, arguments);
            this.IsValidated = true;
        }
        public void SetError(ComponentError error)
        {
            this.Status = GridStatus.Error;
            this.Error = error;
            this.IsValidated = true;
        }
        public void SetOk()
        {
            this.Status = GridStatus.Ok;
            this.Error = null;
            this.IsValidated = true;
        }

        public bool ShouldValidate()
        {
            return !this.IsValidated && this.Modified;
        }

        public bool IsValid()
        {
            return this.Status == GridStatus.Ok;
        }

        public void ForceSetOldValue(string value)
        {
            this.OldValue = value;
        }
    }
}
