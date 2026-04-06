using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WIS.AuthenticationBackend.Repositories;

namespace WIS.AuthenticationBackend.Model
{
    public interface IUnitOfWork
    {
        UserRepository UserRepository { get; }
    }
}
