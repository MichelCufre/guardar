using System;
using System.Collections.Generic;
using System.Linq;

namespace WIS.Domain.DataModel.Mappers.Constants
{
    public class ModalidadIngresoLoteDb
    {
        public const string Normal = "PMIDNOR";
        public const string Vencimiento = "PMIDVEN";
        public const string Agenda = "PMIDAGE";

        public const string Documento = "PMIDDOC";
        public const string VencimientoYYYYMM = "PMIDIDVE";

        public static List<string> GetConstantNames()
        {
            Type type = typeof(ModalidadIngresoLoteDb);
            
            return type.GetFields().Select(x => x.GetValue(null).ToString()).ToList();        
        }
    }
}
