using Dapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Extensions;
using WIS.Domain.General;
using WIS.Domain.Security;
using WIS.Domain.Services.Interfaces;
using WIS.Exceptions;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class SecurityRepository
    {
        protected readonly WISDB _context;
        protected readonly string _cdAplicacion;
        protected readonly int _userId;
        protected readonly SecurityMapper _mapper;
        protected readonly DominioMapper _mapperDominio;
        protected readonly PredioMapper _mapperPredio;
        protected readonly IDapper _dapper;

        public SecurityRepository(WISDB context, string cdAplicacion, int userId, IDapper dapper)
        {
            this._dapper = dapper;
            this._context = context;
            this._cdAplicacion = cdAplicacion;
            this._userId = userId;
            this._mapper = new SecurityMapper();
            this._mapperDominio = new DominioMapper();
            this._mapperPredio = new PredioMapper();
        }

        #region Any

        public virtual bool AnyEmpresaUsuario(int userId, int cdEmpresa)
        {
            return this._context.T_EMPRESA_FUNCIONARIO
                .AsNoTracking()
                .Any(d => d.USERID == userId && d.CD_EMPRESA == cdEmpresa);
        }

        public virtual bool IsUserAllowed(int userId, string resource)
        {
            return this._context.USERPERMISSIONS
                .Include("RESOURCES")
                .Include("PROFILES")
                .Include("PROFILES.PROFILERESOURCES")
                .Any(up => up.USERID == userId
                      && ((up.RESOURCES.UNIQUENAME == resource && up.RESOURCES.FL_ACTIVO == "S")
                           || up.PROFILES.PROFILERESOURCES.Any(pr => pr.RESOURCES.UNIQUENAME == resource && pr.RESOURCES.FL_ACTIVO == "S")));
        }

        public virtual bool AnyPerfil(int id)
        {
            return this._context.PROFILES.Any(x => x.PROFILEID == id);
        }

        public virtual bool AnyConfiguracionUsuario(int id)
        {
            return this._context.T_USUARIO_CONFIGURACION.AsNoTracking().Any(x => x.USERID == id);
        }

        public virtual bool AnyUserMail(string email, int? user = null)
        {
            if (user == null)
                return this._context.USERS.AsNoTracking().Any(x => x.EMAIL == email);
            else
                return this._context.USERS.AsNoTracking().Any(x => x.EMAIL == email && x.USERID != user);
        }

        public virtual bool AnyUsuario(string loginname)
        {
            return this._context.USERS.AsNoTracking().Any(x => x.LOGINNAME.ToUpper() == loginname.ToUpper());
        }

        public virtual bool AnyUsuario(int idUsuario)
        {
            return this._context.USERS.AsNoTracking().Any(x => x.USERID == idUsuario);
        }

        public virtual bool AnyContactoUsuario(int usuario)
        {
            return this._context.T_CONTACTO.AsNoTracking().Any(x => x.USERID == usuario);
        }

        public virtual bool AnyFuncionario(int usuario)
        {
            return this._context.T_FUNCIONARIO.AsNoTracking().Any(x => x.CD_FUNCIONARIO == usuario);
        }

        #endregion

        #region Get

        public virtual List<UsuarioPermiso> GetUserPermissions(int userId)
        {
            return this._context.V_RECURSOS_USUARIO
                .AsNoTracking()
                .Where(r => r.USERID == userId && r.FL_ACTIVO == "S")
                .Select(r => new UsuarioPermiso(r.RESOURCEID, r.UNIQUENAME))
                .ToList();
        }

        public virtual Dictionary<string, bool> CheckPermissions(int userId, List<string> resources)
        {
            var values = this._context.V_RECURSOS_USUARIO
                .AsNoTracking()
                .Where(d => d.USERID == userId && resources.Contains(d.UNIQUENAME) && d.FL_ACTIVO == "S")
                .Select(d => d.UNIQUENAME)
                .ToList();

            Dictionary<string, bool> result = new Dictionary<string, bool>();

            foreach (var resource in resources)
            {
                result.Add(resource, values.Any(d => d == resource));
            }

            return result;
        }

        public virtual Usuario GetUsuario(int userId)
        {
            return this._mapper.Map(this._context.USERS
                .FirstOrDefault(d => d.USERID == userId));
        }

        public virtual UsuarioConfiguracion GetConfiguracionUsuario(int userId)
        {
            return _mapper.MapUsuarioConfiguracionToObject(_context.T_USUARIO_CONFIGURACION
                .FirstOrDefault(i => i.USERID == userId));
        }

        public virtual Usuario GetUsuario(string username)
        {
            var count = this._context.USERS
                .Where(u => u.LOGINNAME.Trim().ToLower() == username.Trim().ToLower())
                .Count();

            if (count == 0)
                throw new InvalidUserException("Usuario no encontrado");

            if (count > 1)
                throw new InvalidUserException("Múltiples usuarios para el mismo login name");

            return this._mapper.Map(this._context.USERS
                .Include("T_GRUPO_CONSULTA_FUNCIONARIO.T_GRUPO_CONSULTA")
                .Include("T_EMPRESA_FUNCIONARIO.T_EMPRESA")
                .AsNoTracking()
                .Where(u => u.LOGINNAME.Trim().ToLower() == username.Trim().ToLower())
                .FirstOrDefault(), true);
        }

        public virtual List<string> GetPrediosUsuario(int userId)
        {
            return _context.T_PREDIO_USUARIO
                .AsNoTracking()
                .Where(w => w.USERID == userId)
                .Select(w => w.NU_PREDIO)
                .ToList();
        }

        public virtual string GetUserFullname(int userId)
        {
            return this._context.USERS
                .AsNoTracking()
                .Where(d => d.USERID == userId).Select(d => d.FULLNAME)
                .FirstOrDefault();
        }

        public virtual int? GetUserIdByLoginName(string loginName)
        {
            return this._context.USERS
                .AsNoTracking()
                .FirstOrDefault(d => d.LOGINNAME == loginName)?.USERID;
        }

        public virtual string GetUserLanguage(int userId)
        {
            return this._context.USERS
                .AsNoTracking()
                .Where(d => d.USERID == userId)
                .Select(d => d.LANGUAGE)
                .FirstOrDefault();
        }

        public virtual string GetUserLanguage(string loginName)
        {
            return this._context.USERS
                .AsNoTracking()
                .Where(d => d.LOGINNAME == loginName)
                .Select(d => d.LANGUAGE)
                .FirstOrDefault();
        }

        public virtual List<UsuarioTipo> GetUsuarioTipos()
        {
            List<UsuarioTipo> retorno = new List<UsuarioTipo>();
            foreach (var tipo in this._context.USERTYPES.ToList().Distinct())
                retorno.Add(this._mapper.MapUsuarioTipoToObject(tipo));

            return retorno;
        }

        public virtual UsuarioTipo GetUsuarioTipoPorId(int id)
        {
            return this._mapper.MapUsuarioTipoToObject(this._context.USERTYPES
                .AsNoTracking()
                .FirstOrDefault(x => x.USERTYPEID == id));
        }

        public virtual UsuarioData GetUserData(int id)
        {
            return this._mapper.MapUsuarioDataToObject(this._context.USERDATA.FirstOrDefault(s => s.USERID == id));
        }

        public virtual List<UsuarioContraseniaHistorica> GetUserPassHistory(int userId, int cantRegistros)
        {
            List<UsuarioContraseniaHistorica> uph = new List<UsuarioContraseniaHistorica>();
            List<USERDATA_PASS_HISTORY> listPassHistory = this._context.USERDATA_PASS_HISTORY
                .AsNoTracking()
                .Where(d => d.USERID == userId)
                .OrderByDescending(u => u.NU_PASS_USERID)
                .Take(cantRegistros)
                .ToList();

            if (listPassHistory == null || listPassHistory.Count == 0)
                return null;

            foreach (var item in listPassHistory)
            {
                uph.Add(this._mapper.MapUsuarioContraHistoToObject(item));
            }
            return uph;
        }

        public virtual List<int> GetEmpresasUsuario(int userId)
        {
            return this._context.T_EMPRESA_FUNCIONARIO
                .AsNoTracking()
                .Where(d => d.USERID == userId)
                .Select(d => d.CD_EMPRESA)
                .ToList();
        }

        public virtual List<Usuario> GetUsuarios()
        {
            var entities = this._context.USERS.AsNoTracking().ToList();

            List<Usuario> usuarios = new List<Usuario>();

            foreach (var entity in entities)
            {
                usuarios.Add(this._mapper.Map(entity));
            }

            return usuarios;
        }

        public virtual List<Usuario> GetUsuariosEmpresa(int idEmpresa)
        {
            var entities = this._context.T_EMPRESA_FUNCIONARIO
                .Include("USERS")
                .AsNoTracking()
                .Where(d => d.CD_EMPRESA == idEmpresa)
                .Select(d => d.USERS);

            List<Usuario> usuarios = new List<Usuario>();

            foreach (var entity in entities)
            {
                usuarios.Add(this._mapper.Map(entity));
            }

            return usuarios;
        }

        public virtual List<Usuario> GetUsuariosByDescripcionOrUserNamePartial(string searchValue, int idEmpresa)
        {
            var entities = this._context.T_EMPRESA_FUNCIONARIO
                .Include("USERS")
                .AsNoTracking()
                .Where(d => d.CD_EMPRESA == idEmpresa
                    && (d.USERS.FULLNAME.ToLower().Contains(searchValue.ToLower()) || d.USERS.LOGINNAME == searchValue))
                .Select(d => d.USERS)
                .ToList();

            List<Usuario> usuarios = new List<Usuario>();

            foreach (var entity in entities)
            {
                usuarios.Add(this._mapper.Map(entity));
            }

            return usuarios;
        }

        public virtual List<Usuario> GetUsuariosById(int searchValue, int idEmpresa)
        {
            var entities = this._context.T_EMPRESA_FUNCIONARIO
                .Include("USERS")
                .AsNoTracking()
                .Where(d => d.CD_EMPRESA == idEmpresa
                    && (d.USERS.USERID == searchValue))
                .Select(d => d.USERS)
                .ToList();

            List<Usuario> usuarios = new List<Usuario>();

            foreach (var entity in entities)
            {
                usuarios.Add(this._mapper.Map(entity));
            }

            return usuarios;

        }

        public virtual List<Usuario> GetUsuariosByPredio(string predio)
        {
            List<Usuario> usuarios = new List<Usuario>();

            var usuariosDB = this._context.USERS.AsNoTracking().ToList();

            foreach (var usuario in usuariosDB)
            {
                if (this._context.V_PREDIO_USUARIO.Any(a => a.USERID == usuario.USERID && a.NU_PREDIO == predio))
                    usuarios.Add(this._mapper.Map(usuario));
            }

            return usuarios;
        }

        public virtual List<string> GetGruposUsuario(int userId)
        {
            var user = this._context.USERS
                .Include("T_GRUPO_CONSULTA_FUNCIONARIO")
                .AsNoTracking()
                .Where(d => d.USERID == userId)
                .FirstOrDefault();

            if (user == null)
                return new List<string>();

            return user.T_GRUPO_CONSULTA_FUNCIONARIO
                .Select(e => e.CD_GRUPO_CONSULTA)
                .ToList();
        }

        public virtual List<Usuario> GetUsuariosByNombreOrCodePartial(string searchValue)
        {
            return this._context.USERS
                .AsNoTracking()
                .Where(d => d.LOGINNAME.ToLower().Contains(searchValue.ToLower())
                    || d.USERID.ToString().Contains(searchValue.ToLower()))
                .ToList()
                .Select(x => _mapper.Map(x))
                .ToList();
        }

        public virtual List<int> GetUsuariosConPermisoATodasLasEmpresas()
        {
            return this._context.T_USUARIO_CONFIGURACION
                .AsNoTracking()
                .Where(d => d.FL_ASIG_AUTO_NUEVA_EMPRESA == "S")
                .Select(d => d.USERID)
                .ToList();
        }

        public virtual Perfil GetPerfil(int id)
        {
            return this._mapper.MapPerfilToObject(this._context.PROFILES
                .AsNoTracking()
                .FirstOrDefault(x => x.PROFILEID == id));
        }

        public virtual List<Perfil> GetPerfiles()
        {
            var retornoList = new List<Perfil>();
            var perfiles = this._context.PROFILES.AsNoTracking().ToList();

            foreach (var entity in perfiles)
            {
                retornoList.Add(_mapper.MapPerfilToObject(entity));
            }

            return retornoList;
        }

        public virtual List<Perfil> GetPerfilesAsociados(int usuario)
        {
            var perfilesAsociados = new List<Perfil>();
            var list = this._context.USERPERMISSIONS
                .Where(x => x.USERID == usuario && x.PROFILEID != null)
                .Select(x => x.PROFILEID)
                .Distinct()
                .ToList();

            foreach (var perfil in list)
            {
                perfilesAsociados.Add(this.GetPerfil(perfil ?? 0));
            }

            return perfilesAsociados;
        }

        public virtual List<Predio> GetPrediosByUserLogin(int userId, string predioLogueado)
        {
            var prediosUsuario = _context.T_PREDIO_USUARIO
                .AsNoTracking()
                .Where(a => a.USERID == userId)
                .Select(w => w.NU_PREDIO)
                .ToList();

            return _context.T_PREDIO
                .AsNoTracking()
                .Where(w => w.NU_PREDIO != "MT"
                    && (predioLogueado == GeneralDb.PredioSinDefinir || w.NU_PREDIO == predioLogueado)
                    && prediosUsuario.Contains(w.NU_PREDIO))
                .ToList()
                .Select(w => _mapperPredio.MapPredioToObject(w))
                .ToList();
        }

        public virtual Usuario GetUser(string username)
        {
            var users = _context.USERS
                .Include("T_GRUPO_CONSULTA_FUNCIONARIO.T_GRUPO_CONSULTA")
                .Include("T_EMPRESA_FUNCIONARIO.T_EMPRESA")
                .AsNoTracking()
                .Where(u => u.LOGINNAME.Trim().ToLower() == username.Trim().ToLower());

            var count = this._context.USERS
                .Where(u => u.LOGINNAME.Trim().ToLower() == username.Trim().ToLower())
                .Count();

            if (users.Count() > 1)
                throw new InvalidUserException("Múltiples usuarios para el mismo login name");

            return this._mapper.Map(users?.FirstOrDefault(), true);
        }

        #endregion

        #region Add

        public virtual void AsignarEmpresaAUsuario(int idUsuario, int idEmpresa)
        {
            if (!this.AnyEmpresaUsuario(idUsuario, idEmpresa))
            {
                this._context.T_EMPRESA_FUNCIONARIO.Add(new T_EMPRESA_FUNCIONARIO()
                {
                    USERID = idUsuario,
                    CD_EMPRESA = idEmpresa
                });
            }
        }

        public virtual void AsignarEmpresaAUsuariosConPermiso(int idUsuario, int idEmpresa)
        {
            List<int> usuarios = this.GetUsuariosConPermisoATodasLasEmpresas();

            foreach (var id in usuarios)
            {
                if (idUsuario != id)
                    this.AsignarEmpresaAUsuario(id, idEmpresa);
            }
        }

        public virtual int AddPerfil(Perfil perf)
        {
            perf.Id = this._context.GetNextSequenceValueInt(_dapper, "S_PROFILE");

            this._context.PROFILES.Add(this._mapper.MapPerfilToEntity(perf));

            return perf.Id;
        }

        public virtual void AddUsuario(Usuario usuario)
        {
            usuario.UserId = this._context.GetNextSequenceValueInt(_dapper, "S_USER");

            var entity = this._mapper.MapUsuarioToObject(usuario);

            entity.T_GRUPO_CONSULTA_FUNCIONARIO.Add(new T_GRUPO_CONSULTA_FUNCIONARIO
            {
                USERS = entity,
                T_GRUPO_CONSULTA = this._context.T_GRUPO_CONSULTA.FirstOrDefault(x => x.CD_GRUPO_CONSULTA.Equals("S/N"))
            });

            this._context.USERS.Add(entity);
        }

        public virtual void AddUsuarioConfiguracion(UsuarioConfiguracion configuracionUsuario)
        {

            var entity = this._mapper.MapUsuarioConfiguracionToEntity(configuracionUsuario);

            this._context.T_USUARIO_CONFIGURACION.Add(entity);
        }

        public virtual void AgregarRecursoPerfil(List<int> recursos, int perfil)
        {
            foreach (var recu in recursos)
            {
                this._context.PROFILERESOURCES.Add(new PROFILERESOURCES
                {
                    PROFILEID = perfil,
                    PROFILERESOURCEID = this._context.GetNextSequenceValueInt(_dapper, "S_PROFILERESOURCE"),
                    RESOURCEID = recu
                });
            }
        }

        public virtual void AgregarRecursoUsuario(List<int> recursos, int usuario)
        {
            foreach (var recu in recursos)
            {
                if (!this._context.USERPERMISSIONS.Any(x => x.RESOURCEID == recu && x.USERID == usuario))
                    this._context.USERPERMISSIONS.Add(new USERPERMISSIONS
                    {
                        USERPERMISSIONID = this._context.GetNextSequenceValueInt(_dapper, Secuencias.S_USERPERMISSION),
                        USERID = usuario,
                        RESOURCEID = recu
                    });
            }
        }

        public virtual void AgregarPerfilUsuario(List<int> perfiles, int usuario)
        {
            foreach (var perfil in perfiles)
            {
                if (!this._context.USERPERMISSIONS.Any(x => x.PROFILEID == perfil && x.USERID == usuario))
                    this._context.USERPERMISSIONS.Add(new USERPERMISSIONS
                    {
                        USERPERMISSIONID = this._context.GetNextSequenceValueInt(_dapper, Secuencias.S_USERPERMISSION),
                        USERID = usuario,
                        PROFILEID = perfil
                    });
            }
        }

        public virtual void AgregarUsuarioFuncionario(int usuario, string fullName)
        {
            int maxNmAbrev = 15;
            int currLength = fullName.IndexOf(" ") > 0 ? fullName.IndexOf(" ") : fullName.Length;
            if (currLength < 15)
                maxNmAbrev = currLength;

            var funcionario = new T_FUNCIONARIO
            {
                CD_FUNCIONARIO = usuario,
                CD_ATIVIDADE = 1,
                NM_FUNCIONARIO = fullName.Length > 30 ? fullName.Substring(0, 30) : fullName,
                NM_ABREV_FUNC = fullName.Substring(0, maxNmAbrev),
                DS_FUNCAO = "TODO",
                QT_CARGA_HORARIA = 0,
                DT_ADDROW = DateTime.Now,
                DT_UPDROW = DateTime.Now,
                CD_OPID = "WIS",
                CD_USER_UNIX = "WIS",
                CD_USER_ORCLE = "WIS",
                CD_IDIOMA = "ESP",

            };

            this._context.T_FUNCIONARIO.Add(funcionario);
        }

        public virtual void AgregarContacto(int usuario, long nuTransaccion, int empresa)
        {
            var user = this._context.USERS.FirstOrDefault(x => x.USERID == usuario);

            this._context.T_CONTACTO.Add(new T_CONTACTO
            {
                NM_CONTACTO = user.LOGINNAME,
                DS_CONTACTO = user.FULLNAME,
                USERID = user.USERID,
                DS_EMAIL = user.EMAIL,
                NU_TRANSACCION = nuTransaccion,
                CD_EMPRESA = empresa,
                NU_TELEFONO = null,
                DT_ADDROW = DateTime.Now,
                DT_UPDROW = DateTime.Now,
                NU_CONTACTO = this._context.GetNextSequenceValueInt(_dapper, Secuencias.S_CONTACTO)
            });
        }

        #endregion

        #region Update

        public virtual void UpdateContacto(int usuario, long nuTransaccion)
        {
            var user = this._context.USERS.FirstOrDefault(x => x.USERID == usuario);

            T_CONTACTO entity = this._context.T_CONTACTO.FirstOrDefault(x => x.USERID == usuario);

            if (entity.NM_CONTACTO != user.LOGINNAME || entity.DS_CONTACTO != user.FULLNAME || entity.DS_EMAIL != user.EMAIL)
            {
                entity.NM_CONTACTO = user.LOGINNAME;
                entity.DS_CONTACTO = user.FULLNAME;
                entity.DS_EMAIL = user.EMAIL;
                entity.DT_UPDROW = DateTime.Now;
                entity.NU_TRANSACCION = nuTransaccion;

                T_CONTACTO attachedEntity = _context.T_CONTACTO.Local.FirstOrDefault(x => x.USERID == entity.USERID);

                if (attachedEntity != null)
                {
                    var attachedEntry = _context.Entry(attachedEntity);
                    attachedEntry.State = EntityState.Modified;
                    attachedEntry.CurrentValues.SetValues(entity);
                }
                else
                {
                    _context.T_CONTACTO.Attach(entity);
                    _context.Entry<T_CONTACTO>(entity).State = EntityState.Modified;
                }
            }
        }

        public virtual void UpdateLanguage(int userId, string language)
        {
            var user = this._context.USERS.FirstOrDefault(u => u.USERID == userId);

            if (user == null)
                throw new InvalidUserException("Usuario no encontrado");

            user.LANGUAGE = language;
        }

        public virtual void UpdateResource(Resource resource)
        {
            RESOURCES entity = this._mapper.MapToResourceEntity(resource);
            RESOURCES attachedEntity = _context.RESOURCES.Local
                .FirstOrDefault(r => r.RESOURCEID == entity.RESOURCEID);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.RESOURCES.Attach(entity);
                this._context.Entry(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdatePerfil(Perfil perfil)
        {
            PROFILES entity = this._mapper.MapPerfilToEntity(perfil);
            PROFILES attachedEntity = _context.PROFILES.Local
                .FirstOrDefault(x => x.PROFILEID == entity.PROFILEID);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.State = EntityState.Modified;
                attachedEntry.CurrentValues.SetValues(entity);
            }
            else
            {
                _context.PROFILES.Attach(entity);
                _context.Entry<PROFILES>(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdatePerfilesAsociadosUsuario(List<int> perfilesQuitar, List<int> perfilesAgregar, int usuario)
        {
            this.QuitarPerfilesUsuario(perfilesQuitar, usuario);
            this.AgregarPerfilUsuario(perfilesAgregar, usuario);
        }

        public virtual void UpdateConfiguracionUsuario(UsuarioConfiguracion configuracionUsuario)
        {
            T_USUARIO_CONFIGURACION entity = this._mapper.MapUsuarioConfiguracionToEntity(configuracionUsuario);
            T_USUARIO_CONFIGURACION attachedEntity = _context.T_USUARIO_CONFIGURACION.Local
                .FirstOrDefault(x => x.USERID == entity.USERID);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.State = EntityState.Modified;
                attachedEntry.CurrentValues.SetValues(entity);
            }
            else
            {
                _context.T_USUARIO_CONFIGURACION.Attach(entity);
                _context.Entry<T_USUARIO_CONFIGURACION>(entity).State = EntityState.Modified;
            }
        }

        public virtual int UpdateUsuario(Usuario usuario)
        {
            USERS entity = this._mapper.MapUsuarioToObject(usuario);
            USERS attachedEntity = _context.USERS.Local
                .FirstOrDefault(x => x.USERID == entity.USERID);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.State = EntityState.Modified;
                attachedEntry.CurrentValues.SetValues(entity);
            }
            else
            {
                _context.USERS.Attach(entity);
                _context.Entry<USERS>(entity).State = EntityState.Modified;
            }

            return usuario.UserId;
        }

        public virtual void UpdateUsuarioFuncionario(int userId, string fullName)
        {
            int maxNmAbrev = 15;

            int currLength = fullName.IndexOf(" ") > 0 ? fullName.IndexOf(" ") : fullName.Length;
            if (currLength < 15)
                maxNmAbrev = currLength;

            var func = this._context.T_FUNCIONARIO.FirstOrDefault(s => s.CD_FUNCIONARIO == userId);

            var funcEntity = _context.T_FUNCIONARIO.Local.FirstOrDefault(x => x.CD_FUNCIONARIO == func.CD_FUNCIONARIO);

            func.NM_FUNCIONARIO = fullName.Length > 30 ? fullName.Substring(0, 30) : fullName;
            func.NM_ABREV_FUNC = fullName.Substring(0, maxNmAbrev);

            //if (password != null)
            //    func.CD_OPID = password;

            if (funcEntity != null)
            {
                var attachedEntry = _context.Entry(funcEntity);
                attachedEntry.State = EntityState.Modified;
                attachedEntry.CurrentValues.SetValues(func);
            }
            else
            {
                _context.T_FUNCIONARIO.Attach(func);
                _context.Entry<T_FUNCIONARIO>(func).State = EntityState.Modified;
            }
        }

        public virtual void CambiarEstadoUsuario(int usuario)
        {
            USERS entity = _context.USERS.FirstOrDefault(x => x.USERID == usuario);

            USERS attachedEntity = _context.USERS.Local.FirstOrDefault(x => x.USERID == entity.USERID);

            if (entity.ISENABLED == 1)
                entity.ISENABLED = 0;
            else
                entity.ISENABLED = 1;

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.State = EntityState.Modified;
                attachedEntry.CurrentValues.SetValues(entity);
            }
            else
            {
                _context.USERS.Attach(entity);
                _context.Entry<USERS>(entity).State = EntityState.Modified;
            }
        }

        #endregion

        #region Remove

        public virtual void QuitarRecursosPerfil(List<int> recursos, int perfil)
        {
            foreach (var recurso in recursos)
            {
                var entities = this._context.PROFILERESOURCES
                    .Where(x => x.PROFILEID == perfil && x.RESOURCEID == recurso);

                foreach (var entity in entities)
                {
                    var attachedEntity = this._context.PROFILERESOURCES.Local
                        .FirstOrDefault(x => x.PROFILERESOURCEID == entity.PROFILERESOURCEID);

                    if (attachedEntity != null)
                    {
                        this._context.PROFILERESOURCES.Remove(attachedEntity);
                    }
                    else
                    {
                        this._context.PROFILERESOURCES.Remove(entity);
                    }
                }
            }
        }

        public virtual void QuitarRecursosUsuario(List<int> recursos, int usuario)
        {
            foreach (var recurso in recursos)
            {
                var entities = this._context.USERPERMISSIONS
                    .Where(x => x.USERID == usuario && x.RESOURCEID == recurso);

                foreach (var entity in entities)
                {
                    var attachedEntity = this._context.USERPERMISSIONS.Local
                        .FirstOrDefault(x => x.USERPERMISSIONID == entity.USERPERMISSIONID);

                    if (attachedEntity != null)
                    {
                        this._context.USERPERMISSIONS.Remove(attachedEntity);
                    }
                    else
                    {
                        this._context.USERPERMISSIONS.Remove(entity);
                    }
                }
            }
        }

        public virtual void QuitarPerfilesUsuario(List<int> perfiles, int usuario)
        {
            foreach (var perfil in perfiles)
            {
                var entities = this._context.USERPERMISSIONS
                    .Where(x => x.USERID == usuario && x.PROFILEID == perfil);

                foreach (var entity in entities)
                {
                    var attachedEntity = this._context.USERPERMISSIONS.Local
                        .FirstOrDefault(x => x.USERPERMISSIONID == entity.USERPERMISSIONID);

                    if (attachedEntity != null)
                    {
                        this._context.USERPERMISSIONS.Remove(attachedEntity);
                    }
                    else
                    {
                        this._context.USERPERMISSIONS.Remove(entity);
                    }
                }
            }
        }

        #endregion

        #region Dapper

        public virtual async Task<bool> CheckPermission(int userId, string recurso)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync();

                var template = new
                {
                    recurso = recurso,
                    userId = userId
                };

                var param = new DynamicParameters(template);

                var sql =
                @"SELECT * FROM V_RECURSOS_USUARIO 
                WHERE USERID= :userId
                    AND   UNIQUENAME= :recurso
                    AND   FL_ACTIVO ='S'";

                var query = connection.Query<string>(sql, param: param, commandType: CommandType.Text);

                return query.FirstOrDefault() != null;
            }
        }

        #endregion
    }
}
