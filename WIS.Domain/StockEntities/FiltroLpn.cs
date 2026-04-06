using System.Collections.Generic;

namespace WIS.Domain.StockEntities
{
    public class FiltroLpn
    {
        public string TipoLpn { get; set; }
        public List<IdAtributoValor> AtributosCabezal { get; set; }
        public List<IdAtributoValor> AtributosDetalle { get; set; }
    }

    public class IdAtributoValor
    {
        public string Id { get; set; }
        public string Value { get; set; }
    }
}
