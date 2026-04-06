using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace WIS.AuthenticationBackend.Services
{
	public interface IDapper : IDisposable
    {
        DbConnection GetDbConnection();
        T Get<T>(string sql, DynamicParameters parms, CommandType commandType = CommandType.Text);
        List<T> GetAll<T>(string sql, DynamicParameters parms, CommandType commandType = CommandType.Text);
        T Insert<T>(string sql, DynamicParameters parms, CommandType commandType = CommandType.Text);
        T Update<T>(string sql, DynamicParameters parms, CommandType commandType = CommandType.Text);
    }
}
