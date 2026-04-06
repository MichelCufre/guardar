using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WIS.CheckboxListComponent;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Security.Enums;
using WIS.Exceptions;
using WIS.Security.Models;
using WIS.Validation;

namespace WIS.Domain.Security
{
    /// <summary>
    /// La clase tiene por finalidad:
    /// + manejar validacion y encode de la contraseña del usuario
    /// + Crear usuario
    /// + Actualizar usuario
    /// </summary>
    public class UserService
    {
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();
        protected readonly SecurityMapper _mapper;

        public UserService()
        {
            this._mapper = new SecurityMapper();
        }

        // Publicas
        public virtual void AgregarUsuario(IUnitOfWork uow, Usuario usuario, List<int> listaPerfiles = null)
        {
            uow.SecurityRepository.AddUsuario(usuario);
            uow.SaveChanges();

            var perfilDefault = uow.ParametroRepository.GetParameter(ParamManager.USUARIO_PERFIL_DEFAULT);
            if (int.TryParse(perfilDefault, out int perfil)
                && uow.SecurityRepository.AnyPerfil(perfil))
            {
                var perfiles = new List<int>() { perfil };
                uow.SecurityRepository.AgregarPerfilUsuario(perfiles, usuario.UserId);
            }

            uow.SecurityRepository.AgregarUsuarioFuncionario(usuario.UserId, usuario.Name);
            uow.DominioRepository.AddDetalleDominioUsuario(usuario.UserId);

            var predioDefault = uow.ParametroRepository.GetParameter(ParamManager.USUARIO_PREDIO_DEFAULT);
            if (!string.IsNullOrEmpty(predioDefault)
                && uow.PredioRepository.AnyPredio(predioDefault))
            {
                uow.PredioRepository.AddPredioUsuario(usuario.UserId, predioDefault);
            }

            var empresa = 0;
            var empresaDefault = uow.ParametroRepository.GetParameter(ParamManager.USUARIO_EMPRESA_DEFAULT);
            if (int.TryParse(empresaDefault, out empresa)
                && uow.EmpresaRepository.AnyEmpresa(empresa))
            {
                uow.EmpresaRepository.AsignarEmpresasUsuario(usuario.UserId, new List<int>() { empresa });
            }

            uow.SecurityRepository.AgregarContacto(usuario.UserId, uow.GetTransactionNumber(), empresa);

            uow.SaveChanges();
        }

        public virtual void ActualizarUsuario(IUnitOfWork uow, Usuario usuario, List<int> perfilesAgregar, List<int> perfilesQuitar, string asignarAutoEmpresas)
        {
            uow.SecurityRepository.UpdateUsuario(usuario);

            uow.SaveChanges();

            UpdatePerfilesUsuario(uow, perfilesAgregar, perfilesQuitar, usuario.UserId);

            UpdateUsuarioFuncionario(uow, usuario.UserId, usuario.Name);

            UpdateContactoUsuario(uow, usuario.UserId);

            ActualizarConfiguracionUsuario(uow, usuario.UserId, asignarAutoEmpresas);

            uow.SaveChanges();
        }

        public virtual void ActualizarUsuario(IUnitOfWork uow, Usuario usuario)
        {
            uow.SecurityRepository.UpdateUsuario(usuario);
            uow.SaveChanges();

            UpdateUsuarioFuncionario(uow, usuario.UserId, usuario.Name);
            UpdateContactoUsuario(uow, usuario.UserId);
            uow.SaveChanges();
        }

        public virtual void AgregarRecursosUsuario(IUnitOfWork uow, List<int> listaRecursos, int usuarioId)
        {
            uow.SecurityRepository.AgregarRecursoUsuario(listaRecursos, usuarioId);
        }
        public virtual void QuitarRecursosUsuario(IUnitOfWork uow, List<int> listaRecursos, int usuarioId)
        {
            uow.SecurityRepository.QuitarRecursosUsuario(listaRecursos, usuarioId);
        }

        public virtual void UpdatePerfilesUsuario(IUnitOfWork uow, List<int> perfilesAgregar, List<int> perfilesQuitar, int usuarioId)
        {
            uow.SecurityRepository.UpdatePerfilesAsociadosUsuario(perfilesQuitar, perfilesAgregar, usuarioId);
        }

        public virtual void UpdateUsuarioFuncionario(IUnitOfWork uow, int userId, string fullname)
        {
            if (uow.SecurityRepository.AnyFuncionario(userId))
                uow.SecurityRepository.UpdateUsuarioFuncionario(userId, fullname);
            else
                uow.SecurityRepository.AgregarUsuarioFuncionario(userId, fullname);

        }

        public virtual void UpdateContactoUsuario(IUnitOfWork uow, int usuarioId)
        {
            if (uow.SecurityRepository.AnyContactoUsuario(usuarioId))
                uow.SecurityRepository.UpdateContacto(usuarioId, uow.GetTransactionNumber());
            else
            {
                var empresa = 0;
                var empresaDefault = uow.ParametroRepository.GetParameter(ParamManager.USUARIO_EMPRESA_DEFAULT);

                if (int.TryParse(empresaDefault, out int parsedValue) && uow.EmpresaRepository.AnyEmpresa(parsedValue))
                    empresa = parsedValue;

                uow.SecurityRepository.AgregarContacto(usuarioId, uow.GetTransactionNumber(), empresa);
            }
        }

        public virtual void AgregarDetalleDominio(IUnitOfWork uow, int usuarioId)
        {
        }

        public virtual void AgregarConfiguracionUsuario(IUnitOfWork uow, int userId, string asignarNuevaEmpresa)
        {
            UsuarioConfiguracion usuarioConfiguracion = new UsuarioConfiguracion();

            usuarioConfiguracion.IdUsuario = userId;
            usuarioConfiguracion.FechaAlta = DateTime.Now;

            if (asignarNuevaEmpresa == "S")
                usuarioConfiguracion.AsignarAutoNuevasEmpresas = "S";
            else
                usuarioConfiguracion.AsignarAutoNuevasEmpresas = "N";

            uow.SecurityRepository.AddUsuarioConfiguracion(usuarioConfiguracion);
        }
        public virtual void ActualizarConfiguracionUsuario(IUnitOfWork uow, int usuarioId, string asignarAutoEmpresas)
        {
            var configuracionUsuario = uow.SecurityRepository.GetConfiguracionUsuario(usuarioId);

            if (configuracionUsuario != null)
            {
                configuracionUsuario.AsignarAutoNuevasEmpresas = asignarAutoEmpresas;
                configuracionUsuario.FechaModificacion = DateTime.Now;

                uow.SecurityRepository.UpdateConfiguracionUsuario(configuracionUsuario);
            }
            else
            {
                configuracionUsuario = new UsuarioConfiguracion()
                {
                    IdUsuario = usuarioId,
                    AsignarAutoNuevasEmpresas = asignarAutoEmpresas,
                    FechaAlta = DateTime.Now,
                };

                uow.SecurityRepository.AddUsuarioConfiguracion(configuracionUsuario);
            }
        }
    }
}
