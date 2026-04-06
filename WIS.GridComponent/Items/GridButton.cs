using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using WIS.Components.Common;

namespace WIS.GridComponent.Items
{
    public class GridButton : GridItem, IGridItem
    {
        [JsonConverter(typeof(ConfirmMessageConverter))]
        public IConfirmMessage ConfirmMessage { get; set; }

        public GridButton()
        {
            this.ItemType = GridItemType.Button;
        }
        public GridButton(string id, string label)
        {
            this.Id = id;
            this.Label = label;
            this.ItemType = GridItemType.Button;
        }
        public GridButton(string id, string label, string cssClass)
        {
            this.Id = id;
            this.Label = label;
            this.CssClass = cssClass;
            this.ItemType = GridItemType.Button;
        }

        public GridButton(string id, string label, string cssClass, IConfirmMessage confirmMessage)
        {
            this.Id = id;
            this.Label = label;
            this.CssClass = cssClass;
            this.ItemType = GridItemType.Button;
            this.ConfirmMessage = confirmMessage;
        }
    }
}
