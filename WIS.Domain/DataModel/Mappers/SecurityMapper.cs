using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.General.Enums;
using WIS.Domain.Security;
using WIS.Domain.Security.Enums;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class SecurityMapper : Mapper
    {
        public virtual UsuarioEmpresa MapToFuncionario(T_EMPRESA_FUNCIONARIO stockEntity)
        {
            if (stockEntity == null)
                return null;

            return new UsuarioEmpresa(stockEntity.CD_EMPRESA, stockEntity.T_EMPRESA?.NM_EMPRESA);
        }

        public virtual UsuarioGrupo MapToGrupoConsulta(T_GRUPO_CONSULTA stockEntity)
        {
            if (stockEntity == null)
                return null;

            return new UsuarioGrupo(stockEntity.CD_GRUPO_CONSULTA, stockEntity.DS_GRUPO_CONSULTA);
        }

        public virtual RESOURCES MapToResourceEntity(Resource resource)
        {
            return new RESOURCES
            {
                RESOURCEID = resource.Id,
                UNIQUENAME = resource.UniqueName,
                NAME = resource.Name,
                DESCRIPTION = resource.Description,
                FL_ACTIVO = this.MapBooleanToString(resource.Enabled),
                USERTYPEID = resource.UserType
            };
        }

        public virtual Resource MapToResourceObject(RESOURCES reso)
        {
            return new Resource
            {
                Id = reso.RESOURCEID,
                UniqueName = reso.UNIQUENAME,
                Name = reso.NAME,
                Description = reso.DESCRIPTION,
                Enabled = this.MapStringToBoolean(reso.FL_ACTIVO),
                UserType = reso.USERTYPEID ?? 0
            };
        }

        public virtual Usuario Map(USERS entity)
        {
            return Map(entity, false);
        }

        public virtual Usuario Map(USERS entity, bool includeRelations)
        {
            if (entity == null) return null;

            var usuario = new Usuario
            {
                UserId = entity.USERID,
                Username = entity.LOGINNAME,
                Language = entity.LANGUAGE,
                IsEnabled = this.MapShortToBoolean(entity.ISENABLED),
                Email = entity.EMAIL,
                Name = entity.FULLNAME,
                SessionToken = entity.SESSIONTOKEN,
                SessionTokenWeb = entity.SESSIONTOKENWEB,
                SincronizacionRealizada = this.MapStringToBoolean(entity.FL_SYNC_REALIZADA),
                DomainName = entity.DOMAINNAME,
                TipoUsuario = entity.USERTYPEID
            };

            if (includeRelations)
            {
                foreach (var ef in entity.T_EMPRESA_FUNCIONARIO)
                {
                    usuario.Empresas.Add(new UsuarioEmpresa(ef.T_EMPRESA.CD_EMPRESA, ef.T_EMPRESA.NM_EMPRESA));
                }

                foreach (var gcf in entity.T_GRUPO_CONSULTA_FUNCIONARIO)
                {
                    usuario.Grupos.Add(new UsuarioGrupo(gcf.T_GRUPO_CONSULTA.CD_GRUPO_CONSULTA, gcf.T_GRUPO_CONSULTA.DS_GRUPO_CONSULTA));
                }
            }

            return usuario;
        }

        public virtual USERS MapUsuarioToObject(Usuario usu)
        {
            return new USERS
            {
                USERID = usu.UserId,
                LOGINNAME = usu.Username.ToLower(),
                LANGUAGE = usu.Language,
                ISENABLED = this.MapBooleanToShort(usu.IsEnabled),
                EMAIL = usu.Email,
                FULLNAME = usu.Name,
                SESSIONTOKEN = usu.SessionToken,
                SESSIONTOKENWEB = usu.SessionTokenWeb,
                FL_SYNC_REALIZADA = this.MapBooleanToString(usu.SincronizacionRealizada),
                DOMAINNAME = usu.DomainName,
                USERTYPEID = usu.TipoUsuario,
            };
        }

        public virtual Perfil MapPerfilToObject(PROFILES perfilEntity)
        {
            return new Perfil
            {
                Id = perfilEntity.PROFILEID,
                Descripcion = perfilEntity.DESCRIPTION,
                Tipo = perfilEntity.USERTYPEID

            };
        }

        public virtual PROFILES MapPerfilToEntity(Perfil perfil)
        {
            return new PROFILES
            {
                PROFILEID = perfil.Id,
                DESCRIPTION = perfil.Descripcion,
                USERTYPEID = perfil.Tipo
            };
        }

        public virtual USERTYPES MapUsuarioTipoToEntity(UsuarioTipo usuTipo)
        {
            return new USERTYPES
            {
                USERTYPEID = usuTipo.Id,
                NAME = usuTipo.Name
            };
        }

        public virtual UsuarioTipo MapUsuarioTipoToObject(USERTYPES useType)
        {
            return new UsuarioTipo
            {
                Id = useType.USERTYPEID,
                Name = useType.NAME
            };
        }
        public virtual UsuarioConfiguracion MapUsuarioConfiguracionToObject(T_USUARIO_CONFIGURACION entity)
        {
            if (entity == null)
                return null;

            return new UsuarioConfiguracion
            {
                IdUsuario = entity.USERID,
                AsignarAutoNuevasEmpresas = entity.FL_ASIG_AUTO_NUEVA_EMPRESA,
                FechaAlta = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UPDROW
            };
        }
        public virtual UsuarioData MapUsuarioDataToObject(USERDATA data)
        {
            if (data == null)
                return null;

            return new UsuarioData
            {
                UserId = data.USERID,
                Password = data.PASSWORD,
                IdTenant = data.ID_TENANT,
                CausaBloqueo = data.LOCKCAUSE,
                PasswordSalt = data.PASSWORDSALT,
                FechaUltimoLogin = data.LASTLOGINDATETIME,
                FechaUltimoBloqueo = data.LASTLOCKOUTDATETIME,
                IsLocked = this.MapShortToBoolean(data.ISLOCKED ?? 0),
                FechaUltimoCambioPassword = data.LASTPASSWORDCHANGEDATETIME,
                NroIntentosContraseñaErronea = data.FAILEDPASSWORDATTEMPTCOUNT,
                FormatoPassword = this.MapToFormatoContrasenia(data.PASSWORDFORMAT),
                FechaUltimoIntentoIncorrectoLogin = data.FAILEDPASSWORDATTEMPTWINSTART,
            };
        }

        public virtual USERDATA MapUsuarioDataToEntity(UsuarioData userData)
        {
            return new USERDATA
            {
                USERID = userData.UserId,
                PASSWORD = userData.Password,
                ID_TENANT = userData.IdTenant,
                LOCKCAUSE = userData.CausaBloqueo,
                PASSWORDSALT = userData.PasswordSalt,
                LASTLOGINDATETIME = userData.FechaUltimoLogin,
                LASTLOCKOUTDATETIME = userData.FechaUltimoBloqueo,
                ISLOCKED = this.MapBooleanToShort(userData.IsLocked),
                LASTPASSWORDCHANGEDATETIME = userData.FechaUltimoCambioPassword,
                FAILEDPASSWORDATTEMPTCOUNT = userData.NroIntentosContraseñaErronea,
                PASSWORDFORMAT = this.MapToFormatoContraseniaDb(userData.FormatoPassword),
                FAILEDPASSWORDATTEMPTWINSTART = userData.FechaUltimoIntentoIncorrectoLogin,
            };
        }

        public virtual T_USUARIO_CONFIGURACION MapUsuarioConfiguracionToEntity(UsuarioConfiguracion userData)
        {
            return new T_USUARIO_CONFIGURACION
            {
                USERID = userData.IdUsuario,
                FL_ASIG_AUTO_NUEVA_EMPRESA = userData.AsignarAutoNuevasEmpresas,
                DT_ADDROW = userData.FechaAlta,
                DT_UPDROW = userData.FechaModificacion,
            };
        }

        public virtual UsuarioContraseniaHistorica MapUsuarioContraHistoToObject(USERDATA_PASS_HISTORY data)
        {
            return new UsuarioContraseniaHistorica
            {
                Id = data.NU_PASS_HISTORY,
                UserId = data.USERID,
                PasswordUserId = data.NU_PASS_USERID,
                Password = data.PASSWORD,
                FormatoPassword = this.MapToFormatoContrasenia(data.PASSWORDFORMAT),
                PasswordSalt = data.PASSWORDSALT,
                FechaModificacion = data.DT_ADDROW,
                Anexo = data.VL_ANEXO
            };
        }

        public virtual TipoPerfil MapToTipoPerfil(int estado)
        {
            switch (estado)
            {
                case 1: return TipoPerfil.Interno;
            }

            return TipoPerfil.Unknown;
        }

        public virtual int MapToTipoPerfilDb(TipoPerfil estado)
        {
            switch (estado)
            {
                case TipoPerfil.Interno: return TipoPerfilDb.Interno;

            }

            return 0;
        }

        public virtual FormatoContrasenia MapToFormatoContrasenia(decimal? estado)
        {
            switch (estado)
            {
                case 1: return FormatoContrasenia.Hashed;
                case 2: return FormatoContrasenia.Encrypted;
            }

            return FormatoContrasenia.Clear;
        }

        public virtual decimal? MapToFormatoContraseniaDb(FormatoContrasenia estado)
        {
            switch (estado)
            {
                case FormatoContrasenia.Hashed: return FormatoContraseniaDb.Hashed;
                case FormatoContrasenia.Encrypted: return FormatoContraseniaDb.Encrypted;

            }

            return FormatoContraseniaDb.Clear;
        }

        public virtual WIS.Security.Models.Usuario MapToUsuario(Usuario model)
        {
            WIS.Security.Models.Usuario usuario = new WIS.Security.Models.Usuario();

            usuario.Email = model.Email;
            usuario.IsEnabled = model.IsEnabled;
            usuario.Language = model.Language;
            usuario.Name = model.Name;
            usuario.SessionToken = model.SessionToken;
            usuario.SessionTokenWeb = model.SessionTokenWeb;
            usuario.UserId = model.UserId;
            usuario.Username = model.Username;

            foreach (var ue in model.Empresas)
            {
                usuario.Empresas.Add(MapToUsuarioEmpresa(ue));
            }

            foreach (var ug in model.Grupos)
            {
                usuario.Grupos.Add(MapToUsuarioGrupo(ug));
            }

            return usuario;
        }

        public virtual WIS.Security.Models.UsuarioPermiso MapToUsuarioPermiso(UsuarioPermiso model)
        {
            WIS.Security.Models.UsuarioPermiso permiso = new WIS.Security.Models.UsuarioPermiso();

            permiso.Id = model.Id;
            permiso.UniqueName = model.UniqueName;

            return permiso;
        }

        public virtual WIS.Security.Models.UsuarioGrupo MapToUsuarioGrupo(UsuarioGrupo model)
        {
            WIS.Security.Models.UsuarioGrupo grupo = new WIS.Security.Models.UsuarioGrupo();

            grupo.Codigo = model.Codigo;
            grupo.Descripcion = model.Descripcion;

            return grupo;
        }

        public virtual WIS.Security.Models.UsuarioEmpresa MapToUsuarioEmpresa(UsuarioEmpresa model)
        {
            WIS.Security.Models.UsuarioEmpresa empresa = new WIS.Security.Models.UsuarioEmpresa();

            empresa.Id = model.Id;
            empresa.Nombre = model.Nombre;

            return empresa;
        }

        public virtual WIS.Security.Models.UsuarioPredio MapToUsuarioPredio(Predio model)
        {
            WIS.Security.Models.UsuarioPredio predio = new WIS.Security.Models.UsuarioPredio();

            predio.Numero = model.Numero;
            predio.Descripcion = model.Descripcion;

            return predio;
        }
    }
}
