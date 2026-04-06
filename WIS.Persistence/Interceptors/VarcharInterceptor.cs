using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WIS.Persistence.Interceptors
{
    public class VarcharInterceptor : DbCommandInterceptor
    {
        public override InterceptionResult<DbDataReader> ReaderExecuting(
            DbCommand command,
            CommandEventData eventData,
            InterceptionResult<DbDataReader> result)
        {
            FixParams(command);
            return base.ReaderExecuting(command, eventData, result);
        }

        public override InterceptionResult<int> NonQueryExecuting(
            DbCommand command,
            CommandEventData eventData,
            InterceptionResult<int> result)
        {
            FixParams(command);
            return base.NonQueryExecuting(command, eventData, result);
        }

        public override InterceptionResult<object> ScalarExecuting(
            DbCommand command,
            CommandEventData eventData,
            InterceptionResult<object> result)
        {
            FixParams(command);
            return base.ScalarExecuting(command, eventData, result);
        }

        public void FixParams(DbCommand command)
        {
            string pattern = @"('(?:[^']|'')*')|(""[^""]*"")|(--.*?$)|(/\*[\s\S]*?\*/)|\bN+\s*(?=')";

            command.CommandText = Regex.Replace(
                command.CommandText,
                pattern,
                m =>
                {
                    // Si coincide con una cadena simple, doble comilla o comentario, la dejamos intacta
                    if (m.Groups[1].Success || m.Groups[2].Success || m.Groups[3].Success || m.Groups[4].Success)
                        return m.Value;
                    // Si es la N de NVARCHAR, la eliminamos
                    return "";
                },
                RegexOptions.Multiline
            );

            var parameters = command.Parameters.Cast<DbParameter>().Where(p => p.DbType == DbType.String);
            foreach (DbParameter param in parameters)
            {
                param.DbType = DbType.AnsiString;
            }
        }
    }
}
