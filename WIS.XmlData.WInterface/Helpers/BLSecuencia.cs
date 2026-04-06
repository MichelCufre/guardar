using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.XmlData.WInterface.Helpers
{
    public class BLSecuencia
    {
        public static long GetNextValSecIntfc(WISDB context, IDapper dapper)
        {
            var connection = context.Database.GetDbConnection();
			var sequenceName = "S_INTERFAZ_EJECUCION";
            var transaction = context.Database.CurrentTransaction?.GetDbTransaction();

			return dapper.GetNextSequenceValue<long>(connection, sequenceName, transaction);
        }

        public static int GetNextValS_NOTIFICACIONES(WISDB context, IDapper dapper)
        {
			var connection = context.Database.GetDbConnection();
			var sequenceName = "S_NOTIFICACIONES";
			var transaction = context.Database.CurrentTransaction?.GetDbTransaction();

			return dapper.GetNextSequenceValue<int>(connection, sequenceName, transaction);
		}
    }
}
