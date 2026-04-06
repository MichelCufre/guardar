using Dapper;
using System.Data;

namespace WIS.Domain.DataModel.Handlers
{
    public class BoolFromStringTypeHandler : SqlMapper.TypeHandler<bool>
    {
        public override bool Parse(object value)
        {
            return value != null && value.ToString().ToUpper() == "S";
        }

        public override void SetValue(IDbDataParameter parameter, bool value)
        {
            parameter.Value = value ? "S" : "N";
        }
    }
}
