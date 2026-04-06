using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Domain.Security;
using WIS.Exceptions;
using WIS.Security;

namespace WIS.Application.Security
{
    public class UserProvider : IUserProvider
    {
        protected readonly IUnitOfWorkFactory _uowFactory;

        public UserProvider(IUnitOfWorkFactory uowFactory)
        {
            this._uowFactory = uowFactory;
        }

        public virtual BasicUserData GetUserData(int userId)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            Usuario user = uow.SecurityRepository.GetUsuario(userId);

            if (user == null)
                throw new InvalidUserException("Usuario no encontrado");

            return new BasicUserData
            {
                UserId = user.UserId,
                Language = user.Language
            };
        }

        public virtual BasicUserData GetUserData(string loginName)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            Usuario user = uow.SecurityRepository.GetUsuario(loginName);

            if (user == null)
                throw new InvalidUserException("Usuario no encontrado");

            return new BasicUserData
            {
                UserId = user.UserId,
                Language = user.Language
            };
        }
    }
}
