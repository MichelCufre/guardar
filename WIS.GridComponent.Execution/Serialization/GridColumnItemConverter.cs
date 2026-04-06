using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using WIS.GridComponent.Columns;
using WIS.GridComponent.Items;

namespace WIS.GridComponent.Execution.Serialization
{
    public class GridColumnItemConverter : Newtonsoft.Json.JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.Name == "IGridItem";
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jsonObject = JObject.Load(reader);

            var token = jsonObject.SelectToken("ItemType");

            var value = token == null ? GridItemType.Unknown : (GridItemType)token.Value<int>();

            switch (value)
            {
                case GridItemType.Button: return jsonObject.ToObject<GridButton>();
                case GridItemType.Divider: return jsonObject.ToObject<GridItemDivider>();
                case GridItemType.Header: return jsonObject.ToObject<GridItemHeader>();
            }

            throw new NotSupportedException("Tipo de item no soportado");
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
