using System;
using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;

namespace WIS.Domain.Recorridos
{
    public class Recorrido
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public bool EsDefault { get; set; }
        public string Predio { get; set; }
        public long? Transaccion { get; set; }
        public long? TransaccionDelete { get; set; }
        public bool EsHabilitado { get; set; }
        public DateTime? FechaAlta { get; set; }
        public DateTime? FechaModificacion { get; set; }

        public static bool PermiteModificarse(string predio, bool esDefault, IUnitOfWork uow)
        {
            return !esDefault;
        }
    }
}
