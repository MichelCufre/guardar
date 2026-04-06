using System;
using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Security.Enums;
using WIS.Security;
using WIS.Security.Models;

namespace WIS.Application.Security
{
    public class SecurityService : ISecurityService
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly SecurityMapper _mapper;

        public SecurityService(IUnitOfWorkFactory uowFactory, IIdentityService identity)
        {
            this._uowFactory = uowFactory;
            this._identity = identity;
            this._mapper = new SecurityMapper();
        }

        public virtual bool CanUserAccessApplication()
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (this._identity.Application == "Default")
                return true;

            return uow.SecurityRepository.IsUserAllowed(this._identity.UserId, $"{this._identity.Application}_Page_Access_Allow"); //TODO: ver de sacar a otro lado
        }

        public virtual bool IsUserAllowed(string resource)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return uow.SecurityRepository.IsUserAllowed(this._identity.UserId, resource);
        }

        public virtual bool IsEmpresaAllowed(int cdEmpresa)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return uow.SecurityRepository.AnyEmpresaUsuario(this._identity.UserId, cdEmpresa);
        }

        public virtual Dictionary<string, bool> CheckPermissions(List<string> resources)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return uow.SecurityRepository.CheckPermissions(this._identity.UserId, resources);
        }

        public virtual void UpdateUserLanguage(int userid, string language)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                uow.SecurityRepository.UpdateLanguage(userid, language);
                uow.SaveChanges();
            }
        }

        public virtual Usuario GetUser(SecurityRequest userInfo)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var user = this._mapper.MapToUsuario(GetOrCreateUser(uow, userInfo));

                foreach (var p in uow.SecurityRepository.GetUserPermissions(user.UserId))
                {
                    user.Permisos.Add(new UsuarioPermiso()
                    {
                        Id = p.Id,
                        UniqueName = p.UniqueName
                    });
                }

                foreach (var p in uow.SecurityRepository.GetPrediosByUserLogin(user.UserId, GeneralDb.PredioSinDefinir))
                {
                    user.Predios.Add(new UsuarioPredio()
                    {
                        Numero = p.Numero,
                        Descripcion = p.Descripcion
                    });
                }

                user.Predio = user.Predios.Count == 1 ? user.Predios[0].Numero : GeneralDb.PredioSinDefinir;

                return user;
            }
        }

        public virtual bool IsValidPassword(string password, string hashedPassword, string salt)
        {
            var encoderPwd = new WIS.Domain.Security.SeguridadEncodePassword();
            return this.SlowEquals(Convert.FromBase64String(hashedPassword), Convert.FromBase64String(encoderPwd.EncodePASSWORD(password, FormatoContrasenia.Hashed, salt)[0]));
        }

        public virtual bool SlowEquals(byte[] a, byte[] b) // Prevención para timing attack
        {
            uint diff = (uint)a.Length ^ (uint)b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++)
                diff |= (uint)(a[i] ^ b[i]);
            return diff == 0;
        }

        public virtual Domain.Security.Usuario GetOrCreateUser(IUnitOfWork uow, SecurityRequest userInfo)
        {
            var userService = new Domain.Security.UserService();
            var user = uow.SecurityRepository.GetUser(userInfo.Username);

            if (user == null)
            {
                uow.CreateTransactionNumber("GetOrCreateUser", "WebPanel", -1);

                user = new Domain.Security.Usuario()
                {
                    Username = userInfo.Username,
                    Name = !string.IsNullOrEmpty(userInfo.Name) ? userInfo.Name : userInfo.Username,
                    Email = userInfo.Email,
                    Language = !string.IsNullOrEmpty(userInfo.Language) ? userInfo.Language : "es",
                    TipoUsuario = userInfo.Role == "admin" ? 3 : 1,
                    IsEnabled = true,
                    SincronizacionRealizada = false,
                };

                userService.AgregarUsuario(uow, user);
            }
            else
            {
                var newName = userInfo.Name;
                var newLanguage = userInfo.Language;
                var newTipoUsuario = userInfo.Role == "admin" ? 3 : 1;

                if (newName != user.Name || newLanguage != user.Language || newTipoUsuario != user.TipoUsuario)
                {
                    uow.CreateTransactionNumber("GetOrCreateUser", "WebPanel", user.UserId);

                    user.Name = newName;
                    user.Language = newLanguage;
                    user.TipoUsuario = newTipoUsuario;

                    userService.ActualizarUsuario(uow, user);
                }
            }

            return user;
        }

    }
}
