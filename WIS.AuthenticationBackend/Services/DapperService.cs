using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace WIS.AuthenticationBackend.Services
{
    public class DapperService : IDapper
    {
        private readonly IDatabaseFactory _dbFactory;

        public DapperService(IDatabaseFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public void Dispose()
        {

        }

        public T Get<T>(string sql, DynamicParameters parms, CommandType commandType = CommandType.Text)
        {
            using (IDbConnection db = GetDbConnection())
            {
                db.Open();
                sql = _dbFactory.TranslateDapperQuery(sql);
                return db.Query<T>(sql, parms, commandType: commandType).FirstOrDefault();
            }
        }

        public List<T> GetAll<T>(string sql, DynamicParameters parms, CommandType commandType = CommandType.Text)
        {
            using (IDbConnection db = GetDbConnection())
            {
                db.Open();
                sql = _dbFactory.TranslateDapperQuery(sql);
                return db.Query<T>(sql, parms, commandType: commandType).ToList();
            }
        }

        public DbConnection GetDbConnection()
        {
            return _dbFactory.GetDbConnection();
        }

        public T Insert<T>(string sql, DynamicParameters parms, CommandType commandType = CommandType.Text)
        {
            T result;

            using (IDbConnection db = GetDbConnection())
            {
                db.Open();

                using (var tran = db.BeginTransaction())
                {
                    try
                    {
                        sql = _dbFactory.TranslateDapperQuery(sql);
                        result = db.Query<T>(sql, parms, commandType: commandType, transaction: tran).FirstOrDefault();
                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        throw ex;
                    }
                }
            }

            return result;
        }

        public T Update<T>(string sql, DynamicParameters parms, CommandType commandType = CommandType.Text)
        {
            T result;

            using (IDbConnection db = GetDbConnection())
            {
                db.Open();

                using (var tran = db.BeginTransaction())
                {
                    try
                    {
                        sql = _dbFactory.TranslateDapperQuery(sql);
                        result = db.Query<T>(sql, parms, commandType: commandType, transaction: tran).FirstOrDefault();
                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        throw ex;
                    }
                }
            }

            return result;
        }
    }
}
