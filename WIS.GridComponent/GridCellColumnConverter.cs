using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.GridComponent
{
    public class GridCellColumnConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            string valueToWrite = string.Empty;

            if (value != null)
            {
                IGridColumn column = (GridColumn)value;
                valueToWrite = column.Id;
            }

            writer.WriteValue(valueToWrite);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                //TOOD: Esto va a fallar
                IGridColumn target = new GridColumn
                {
                    Id = (string)reader.Value
                };

                return target;
            }

            return serializer.Deserialize(reader, objectType);
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime);
        }
    }
}
