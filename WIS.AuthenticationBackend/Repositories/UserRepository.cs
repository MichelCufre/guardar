using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using WIS.AuthenticationBackend.Model;
using WIS.AuthenticationBackend.Services;

namespace WIS.AuthenticationBackend.Repositories
{
    public class UserRepository
    {
        private readonly IDapper _dapper;

        public UserRepository(IDapper dapper)
        {
            this._dapper = dapper;
        }

        public async Task<User> Get(int id)
        {
            var values = new {            
                userId = id.ToString()
            };

            var parameters = new DynamicParameters(values);

            return await Task.FromResult(_dapper.Get<User>(
                @"Select 
                    us.USERID as UserId,
                    us.LOGINNAME as Username,
                    us.FULLNAME as Name,
                    us.EMAIL as Email,
                    us.ISENABLED as IsEnabled,
                    ud.PASSWORD as Password,
                    ud.SALT as Salt,
                    ud.ISLOCKED as IsLocked
                from USERS us
                inner join USERDATA ud on us.USERID = ud.USERID
                where us.USERID = :userId",
                parameters,
                commandType: CommandType.Text));
        }

        public async Task<User> GetByUsername(string username)
        {
            var parameters = new DynamicParameters(new { username = username});

            return await Task.FromResult(_dapper.Get<User>(
                @"Select 
                    us.USERID as UserId,
                    us.LOGINNAME as Username,
                    us.FULLNAME as Name,
                    us.EMAIL as Email,
                    us.ISENABLED as IsEnabled,
                    ud.PASSWORD as Password,
                    ud.PASSWORDSALT as Salt,
                    ud.ISLOCKED as IsLocked
                from USERS us
                inner join USERDATA ud on us.USERID = ud.USERID
                where us.LOGINNAME = :username",
                parameters,
                commandType: CommandType.Text));
        }
    }
}
