using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WIS.AuthenticationBackend.Repositories;
using WIS.AuthenticationBackend.Services;

namespace WIS.AuthenticationBackend.Model
{
    public class UnitOfWork : IUnitOfWork
    {
        public UserRepository UserRepository { get; }

        public UnitOfWork(IDapper dapper)
        {
            this.UserRepository = new UserRepository(dapper);
        }
    }
}
