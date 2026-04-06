using DocumentFormat.OpenXml.Math;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Domain.Interfaces;
using WIS.Domain.Security;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.Validation;
using WIS.Exceptions;

namespace WIS.Domain.Services
{
    public class EjecucionService : IEjecucionService
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IEmpresaService _empresaService;
        protected readonly IParameterService _parameterService;

        public EjecucionService(IUnitOfWorkFactory uowFactory,
            IEmpresaService empresaService,
            IParameterService parameterService)
        {
            _uowFactory = uowFactory;
            _empresaService = empresaService;
            _parameterService = parameterService;
        }

        public virtual async Task<InterfazEjecucion> AddEjecucion(InterfazEjecucion ejecucion)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var itfz = await uow.EjecucionRepository.AddEjecucion(ejecucion);

                itfz.InterfazExterna = uow.InterfazRepository.GetInterfazExterna(itfz.CdInterfazExterna.Value);

                return itfz;
            }
        }

        public virtual async Task<InterfazEjecucion> AddEjecucion(int cdIntExterna, int empresa, string dsReferencia, string data, string archivo, string loginName, string idRequest, string entidadParam = ParamManager.PARAM_EMPR, string entidad = null)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var userId = uow.SecurityRepository.GetUserIdByLoginName(loginName) ?? 0;

                await ValidateRequest(uow, idRequest, empresa, userId, dsReferencia, archivo);

                InterfazEjecucion itfz = CrearEjecucion(uow, cdIntExterna, empresa, dsReferencia, archivo, userId, idRequest, entidadParam, entidad);
                itfz = await uow.EjecucionRepository.AddEjecucion(itfz);

                InterfazData itfzData = CrearEjecucionData(itfz.Id, data);
                itfzData = await uow.EjecucionRepository.AddEjecucionData(itfzData);

                itfz.InterfazExterna = uow.InterfazRepository.GetInterfazExterna(cdIntExterna);

                return itfz;
            }
        }

        public virtual async Task<InterfazData> AddEjecucionData(InterfazData ejecucion)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                return await uow.EjecucionRepository.AddEjecucionData(ejecucion);
            }
        }

        public virtual async Task<InterfazError> AddError(InterfazEjecucion ejecucion, int nroRegistro, string error)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                int nroError = await uow.EjecucionRepository.GetUltimoError(ejecucion.Id) ?? 0;
                nroError++;
                var newError = new InterfazError
                {
                    Id = ejecucion.Id,
                    NroError = nroError,
                    Registro = nroRegistro,
                    Referencia = ejecucion.Referencia,
                    Descripcion = error.Length > 1000 ? error.Substring(0, 1000) : error
                };

                return await uow.EjecucionRepository.AddEjecucionError(newError);
            }
        }

        public virtual async Task<InterfazEjecucion> AddErrores(InterfazEjecucion ejecucion, List<ValidationsError> errores)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                int nroError = await uow.EjecucionRepository.GetUltimoError(ejecucion.Id) ?? 0;
                var iErrores = new List<InterfazError>();

                foreach (var error in errores)
                {
                    foreach (var messagge in error.Messages)
                    {
                        nroError++;
                        iErrores.Add(new InterfazError
                        {
                            Id = ejecucion.Id,
                            NroError = nroError,
                            Registro = error.ItemId,
                            Referencia = ejecucion.Referencia,
                            Descripcion = messagge.Length > 1000 ? messagge.Substring(0, 1000) : messagge
                        });
                    }
                }
                await uow.EjecucionRepository.AddEjecucionErrores(iErrores);
                return await uow.EjecucionRepository.Update(ejecucion);
            }
        }

        public virtual async Task<InterfazEjecucion> IniciarReprocesamiento(InterfazEjecucion ejecucion)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                ejecucion.Comienzo = DateTime.Now;
                ejecucion.ErrorCarga = "N";
                ejecucion.ErrorProcedimiento = "N";
                ejecucion.Situacion = SituacionDb.ProcesamientoIniciado;

                return await uow.EjecucionRepository.UpdateAndDeleteErrores(ejecucion);
            }
        }

        public virtual async Task<InterfazEjecucion> UpdateEjecucion(InterfazEjecucion ejecucion)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                return await uow.EjecucionRepository.Update(ejecucion);
            }
        }

        public virtual async Task<InterfazEjecucion> GetEjecucion(long nroEjecucion)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                return await uow.EjecucionRepository.GetEjecucion(nroEjecucion);
            }
        }

        public virtual async Task<bool> ExisteEjecucion(long nroEjecucion)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                return await uow.EjecucionRepository.ExisteEjecucion(nroEjecucion);
            }
        }

        public virtual async Task<List<InterfazEjecucion>> GetNotificacionesPendientes()
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                return await uow.EjecucionRepository.GetNotificacionesPendientes();
            }
        }

        public virtual async Task<List<InterfazEjecucion>> GetSalidasPendientes(int empresa, List<string> gruposConsulta)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                return await uow.EjecucionRepository.GetSalidasPendientes(empresa, gruposConsulta, false);
            }
        }

        public virtual async Task<List<InterfazError>> GetErrores(long nroEjecucion)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                return await uow.EjecucionRepository.GetErrores(nroEjecucion);
            }
        }

        public virtual async Task<InterfazData> GetEjecucionData(long nroEjecucion)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                return await uow.EjecucionRepository.GetEjecucionData(nroEjecucion);
            }
        }

        public virtual async Task<InterfazEstado> ConsultarEstado(long nroEjecucion, int empresa, List<string> gruposConsulta)
        {
            var ejecucion = await GetSalida(nroEjecucion, empresa, gruposConsulta, true);
            var errores = new List<InterfazError>();

            if (ejecucion.ErrorCarga == "S" || ejecucion.ErrorProcedimiento == "S")
                errores = await GetErrores(nroEjecucion);

            return new InterfazEstado(ejecucion, errores);
        }

        public virtual async Task ConfirmarLectura(long nroEjecucion, int empresa, List<string> gruposConsulta, bool ok, List<string> errores)
        {
            var ejecucion = await GetSalida(nroEjecucion, empresa, gruposConsulta);

            try
            {
                if (ok)
                {
                    ejecucion.Situacion = SituacionDb.ProcesadoOK;
                    await UpdateEjecucion(ejecucion);
                }
                else
                {
                    ejecucion.Situacion = SituacionDb.ProcesadoConError;
                    ejecucion.ErrorProcedimiento = "S";

                    await AddErrores(ejecucion, new List<ValidationsError>(){
                        new ValidationsError(1, true, errores)
                    });
                    await _empresaService.UpdateLock(empresa, true);
                }
            }
            catch (Exception ex)
            {
                ejecucion.Situacion = SituacionDb.ProcesadoConError;
                ejecucion.ErrorProcedimiento = "S";

                await AddError(ejecucion, 0, ex.Message);
                await UpdateEjecucion(ejecucion);
                await _empresaService.UpdateLock(empresa, true);

                throw ex;
            }
        }

        public virtual async Task<InterfazEjecucion> GetSalida(long nroEjecucion, int cdEmpresa, List<string> gruposConsulta, bool consultarEstado = false)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var ejecucion = await uow.EjecucionRepository.GetEjecucion(nroEjecucion);

                if (ejecucion == null ||
                    !ejecucion.Empresa.HasValue ||
                    ejecucion.Empresa.Value != cdEmpresa ||
                    !ejecucion.IsSalida ||
                    (gruposConsulta != null && !gruposConsulta.Exists(gc => gc == ejecucion.GrupoConsulta)))
                {
                    var gc = $" - {gruposConsulta}";
                    throw new EntityNotFoundException(string.Format(ValidationMessage.WMSAPI_msg_Error_InterfazNoEncontradaEmpresaGrupo, nroEjecucion, cdEmpresa, gc));
                }


                var empresa = await _empresaService.GetEmpresa(cdEmpresa);
                if (empresa.TipoNotificacion != null && empresa.TipoNotificacion.Id == CodigoDominioDb.TipoNotificacionWebhook)
                    throw new ValidationFailedException(string.Format(ValidationMessage.WMSAPI_msg_Error_InterfazProcesablePorWebhook, nroEjecucion, cdEmpresa));

                if (empresa.IsLocked)
                    throw new ValidationFailedException(string.Format(ValidationMessage.WMSAPI_msg_Error_EmpresaBloqueada, cdEmpresa));

                if (!consultarEstado)
                {
                    var interfacesOmisibles = new List<int>();
                    var isConfirmacionOmisibles = _parameterService.GetValueByEmpresa(ParamManager.IS_CONFIRMACION_OMISIBLE, cdEmpresa);

                    if (!string.IsNullOrEmpty(isConfirmacionOmisibles))
                    {
                        foreach (var interfazExterna in isConfirmacionOmisibles.Split(";", StringSplitOptions.RemoveEmptyEntries))
                        {
                            interfacesOmisibles.Add(int.Parse(interfazExterna));
                        }
                    }

                    if (!interfacesOmisibles.Contains(ejecucion.CdInterfazExterna ?? -1))
                    {
                        var primeraSalidaPendiente = await uow.EjecucionRepository.GetPrimeraSalida(cdEmpresa, SituacionDb.ProcesadoPendiente, interfacesOmisibles);
                        if (primeraSalidaPendiente.HasValue && primeraSalidaPendiente.Value != nroEjecucion)
                            throw new ValidationFailedException(string.Format(string.Format(ValidationMessage.WMSAPI_msg_Error_InterfazPendientePrevia, primeraSalidaPendiente, nroEjecucion)));
                    }
                }

                return ejecucion;
            }
        }

        public virtual List<string> GetGruposConsulta(string loginName)
        {
            var result = new List<string>();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var userId = uow.SecurityRepository.GetUserIdByLoginName(loginName);
                result.AddRange(uow.SecurityRepository.GetGruposUsuario(userId ?? -1));
            }

            return result;
        }

        #region Metodos auxiliares

        public virtual InterfazEjecucion CrearEjecucion(IUnitOfWork uow, int cdIntExterna, int empresa, string ds_deferencia, string archivo, int userId, string idRequest, string entidadParam = ParamManager.PARAM_EMPR, string entidad = null)
        {
            entidad = string.IsNullOrEmpty(entidad) ? empresa.ToString() : entidad;
            return new InterfazEjecucion
            {
                CdInterfazExterna = cdIntExterna,
                Archivo = archivo,
                Situacion = SituacionDb.ProcesamientoIniciado,
                Comienzo = DateTime.Now,
                FechaSituacion = DateTime.Now,
                ErrorCarga = "N",
                ErrorProcedimiento = "N",
                Referencia = ds_deferencia,
                Empresa = empresa,
                UserId = userId,
                GrupoConsulta = uow.ParametroRepository.GetParametro(ParamManager.GRUPO_CONSULTA, entidadParam, entidad).Result ?? "S/N",
                IdRequest = idRequest
            };
        }

        public virtual InterfazData CrearEjecucionData(long nuItfzEjecucion, string data)
        {
            return new InterfazData
            {
                Id = nuItfzEjecucion,
                Alta = DateTime.Now,
                Data = Encoding.UTF8.GetBytes(data)
            };
        }

        public virtual bool IsValidUser(UsuarioRequest usuario)
        {
            if (usuario != null)
            {
                using (var uow = this._uowFactory.GetUnitOfWork())
                {
                    return SecurityLogic.IsValidUser(uow, usuario.LoginName, usuario.Hash);
                }
            }

            return false;
        }

        public virtual async Task ValidateRequest(IUnitOfWork uow, string idRequest, int empresa, int userId, string dsReferencia, string archivo)
        {
            if (!uow.EmpresaRepository.AnyEmpresa(empresa))
            {
                var error = new Error("WMSAPI_msg_Error_ExisteEmpresaValidation", empresa);
                var mensajeError = Translator.Translate(uow, error, userId) ?? error.Mensaje;
                throw new ValidationFailedException(mensajeError);
            }
            else if (!string.IsNullOrEmpty(idRequest) && await uow.EjecucionRepository.ExisteIdRequest(idRequest, empresa))
            {
                var error = new Error("WMSAPI_msg_Error_IdEjecucionExistente", idRequest, empresa.ToString());
                var mensajeError = Translator.Translate(uow, error, userId) ?? error.Mensaje;
                throw new ValidationFailedException(mensajeError);
            }
            else if (!string.IsNullOrEmpty(idRequest) && idRequest.Length > 50)
            {
                var error = new Error("WMSAPI_msg_Error_IdEjecucionExistente", idRequest, empresa.ToString());
                var mensajeError = Translator.Translate(uow, error, userId) ?? error.Mensaje;
                throw new ValidationFailedException(mensajeError);
            }
            else if (!string.IsNullOrEmpty(dsReferencia) && dsReferencia.Length > 200)
            {
                var error = new Error("WMSAPI_msg_Error_LargoStringValidation", "DsReferencia", 0, 200);
                var mensajeError = Translator.Translate(uow, error, userId) ?? error.Mensaje;
                throw new ValidationFailedException(mensajeError);
            }
            else if (!string.IsNullOrEmpty(archivo) && archivo.Length > 100)
            {
                var error = new Error("WMSAPI_msg_Error_LargoStringValidation", "Archivo", 0, 100);
                var mensajeError = Translator.Translate(uow, error, userId) ?? error.Mensaje;
                throw new ValidationFailedException(mensajeError);
            }
        }

        #endregion
    }
}
