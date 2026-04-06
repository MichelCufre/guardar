using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Data;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.Extensions
{
    public static class WISDBExtension
    {
        public static decimal GetNextSequenceValueDecimal(this WISDB context, IDapper dapper, string sequenceName)
        {
            return Convert.ToDecimal(GetNextSequenceValue<decimal>(context, dapper, sequenceName));
        }

        public static int GetNextSequenceValueInt(this WISDB context, IDapper dapper, string sequenceName)
        {
            return Convert.ToInt32(GetNextSequenceValue<int>(context, dapper, sequenceName));
        }

        public static long GetNextSequenceValueLong(this WISDB context, IDapper dapper, string sequenceName)
        {
            return Convert.ToInt64(GetNextSequenceValue<long>(context, dapper, sequenceName));
        }
        public static short GetNextSequenceValueShort(this WISDB context, IDapper dapper, string sequenceName)
        {
            return Convert.ToInt16(GetNextSequenceValue<short>(context, dapper, sequenceName));
        }

        private static T GetNextSequenceValue<T>(this WISDB context, IDapper dapper, string sequenceName)
        {
            var connection = context.Database.GetDbConnection();
            var shouldHandleConnection = connection.State != ConnectionState.Open;

            try
            {
                if (sequenceName.IndexOf(" ") >= 0)
                    throw new InvalidOperationException("Secuencia no admite espacios");

                if (shouldHandleConnection)
                    connection.Open();

                return dapper.GetNextSequenceValue<T>(connection, sequenceName, context.Database.CurrentTransaction?.GetDbTransaction());
            }
            finally
            {
                if (shouldHandleConnection)
                    connection.Close();
            }
        }
    }
}
