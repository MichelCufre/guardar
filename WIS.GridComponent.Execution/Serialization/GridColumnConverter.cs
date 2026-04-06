using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.GridComponent.Columns;

namespace WIS.GridComponent.Execution.Serialization
{
    public class GridColumnConverter : Newtonsoft.Json.JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.Name == "IGridColumn";
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jsonObject = JObject.Load(reader);

            var token = jsonObject.SelectToken("Type");

            var value = token == null ? GridColumnType.Unknown : (GridColumnType)token.Value<int>();

            switch (value)
            {
                case GridColumnType.Button: return jsonObject.ToObject<GridColumnButton>(serializer);
                case GridColumnType.ItemList: return jsonObject.ToObject<GridColumnItemList>(serializer);
                case GridColumnType.Progress: return jsonObject.ToObject<GridColumnProgress>(serializer);
                case GridColumnType.Select: return jsonObject.ToObject<GridColumnSelect>(serializer);
                case GridColumnType.SelectAsync: return jsonObject.ToObject<GridColumnSelectAsync>(serializer);
                case GridColumnType.Toggle: return jsonObject.ToObject<GridColumnToggle>(serializer);
                default: return jsonObject.ToObject<GridColumnText>(serializer);
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
