using Newtonsoft.Json;
using System.Collections.Generic;

namespace WIS.Domain.General
{
    public class ValidationsError
    {
        public ValidationsError()
        {
        }

        public ValidationsError(int itemId, bool isProcedural, List<string> messages)
        {
            ItemId = itemId;
            IsProcedural = isProcedural;
            Messages = messages;
        }

        public int ItemId { get; set; } //nu_registro

        public List<string> Messages { get; set; } //lista de errores


        [JsonIgnore]
        public bool IsProcedural { get; set; } //error de procedimiento
    }
}