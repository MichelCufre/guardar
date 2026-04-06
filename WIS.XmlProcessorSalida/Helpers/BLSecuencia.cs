using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.XmlProcessorSalida.Helpers
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
    }
}
