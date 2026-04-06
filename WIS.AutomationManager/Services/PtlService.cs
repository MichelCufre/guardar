using Azure;
using MigraDoc.DocumentObjectModel.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.AutomationManager.Interfaces;
using WIS.Domain.Automatismo;
using WIS.Domain.Automatismo.Constants;
using WIS.Domain.Automatismo.Interfaces;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.Domain.Validation;
using WIS.Security;

namespace WIS.AutomationManager.Services
{
    public abstract class PtlService : IPtlService
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IUnitOfWorkInMemory _uowMemory;
        protected readonly IPtl _ptl;
        protected readonly IPtlInterpreterClientService _interpretService;
        protected readonly IAutomatismoWmsApiClientService _wmsApiClientService;
		protected readonly IIdentityService _identity;

		public PtlService(IUnitOfWorkFactory uowFactory,
            IUnitOfWorkInMemoryFactory uowMemoryFactory,
            IPtlInterpreterClientService interpretService,
            IAutomatismoWmsApiClientService wmsApiClientService,
            IPtl ptl,
			IIdentityService identity)
        {
            _ptl = ptl;
            _uowFactory = uowFactory;
            _uowMemory = uowMemoryFactory.GetUnitOfWork();
            _interpretService = interpretService;
            _wmsApiClientService = wmsApiClientService;
			_identity = identity;
		}

		protected virtual ValidationsResult InvokeTurnOnLigth(List<PtlPosicionEnUso> accion, ValidationsResult result)
        {
            var validation = _interpretService.TurnLigthOnOrOff(_ptl, accion, true);

            if (validation.HasError())
            {
                result.Errors.AddRange(validation.Errors);
            }

            return validation;
        }

        protected virtual ValidationsResult InvokeTurnOffLigth(List<PtlPosicionEnUso> accion, ValidationsResult result)
        {
            var validation = _interpretService.TurnLigthOnOrOff(_ptl, accion, false);

            if (validation.HasError())
                result.Errors.AddRange(validation.Errors);

            return result;
        }

        public virtual ValidationsResult StartOfOperation()
        {
            var result = _interpretService.StartOfOperation(_ptl);

            if (result.IsValid())
            {
                _uowMemory.PosicionRepository.RemoveUbicacionesPrendidasByPtl(_ptl.Numero);

                result.SuccessMessage = "OK";
            }

            return result;
        }

        public virtual ValidationsResult ResetOfOperation()
        {
            var result = _interpretService.ResetOfOperation(_ptl);

            if (result.IsValid())
            {
                _uowMemory.ColorRepository.RemoveColorsByPtl(_ptl.Numero);
                _uowMemory.PosicionRepository.RemoveUbicacionesPrendidasByPtl(_ptl.Numero);

                result.SuccessMessage = "OK";
            }

            return result;
        }

        protected virtual List<AutomatismoPosicion> ReservarPosicionesByComparteAgrupaciones(List<string> comparteAgrupaciones, int tipoAgrupacion)
        {
            var posiciones = new List<AutomatismoPosicion>();
            var lucesPrendidas = _uowMemory.PosicionRepository.GetUbicacionesPrendidasByPtl(_ptl.Numero);

            using (var uow = _uowFactory.GetUnitOfWork())
            {
                foreach (var comparteAgrupacion in comparteAgrupaciones)
                {
                    var ubicacionesDisponiblePorAgrupacion = uow.AutomatismoPosicionRepository.GetUbicacionesDisponibleByAgrupacion(_ptl.Numero, tipoAgrupacion, comparteAgrupacion);
                    var ubicacion = ubicacionesDisponiblePorAgrupacion
                        .FirstOrDefault(w => w.ComparteAgrupacion == comparteAgrupacion && !lucesPrendidas
                        .Any(a => a.Ubicacion == w.Id && a.Estado == PtlEstadoPosicion.Cerrando)
                    );

                    if (ubicacion == null)
                    {
                        ubicacion = ubicacionesDisponiblePorAgrupacion.OrderBy(s => s.Orden).FirstOrDefault(w => w.ComparteAgrupacion == null);
                    }

                    if (ubicacion != null)
                    {
                        if (ubicacion.ComparteAgrupacion == null)
                        {
                            ubicacion.ComparteAgrupacion = comparteAgrupacion;
                            ubicacion.Transaccion = uow.GetTransactionNumber();
                            uow.AutomatismoPosicionRepository.Update(ubicacion);
                            uow.SaveChanges();
                        }

                        posiciones.Add(ubicacion);
                    }
                }
            }

            return posiciones;
        }

        protected virtual void LiberarPosicionesNoUtilizadasEnAccion(List<AutomatismoPosicion> posicionesDisponibles, PtlPosicionEnUso accion)
        {
            var lucesPrendidas = _uowMemory.PosicionRepository.GetUbicacionesPrendidasByPtlAndColor(_ptl.Numero, accion.Color);

            foreach (var pos in posicionesDisponibles.Where(w => !lucesPrendidas.Any(a => a.Ubicacion == w.Id)))
            {
                LiberarPosicion(pos.Id);
            }
        }

        public virtual PtlColor GetColor(int userId)
        {
            var coloresActivos = _ptl.GetColoresActivos();
            var color = _uowMemory.ColorRepository.GetPtlColor(_ptl.Numero, userId);

            if (color == null)
            {
                var coloresUsados = _uowMemory.ColorRepository.GetPtlColoresByPtl(_ptl.Numero);
                var colorDisponible = coloresActivos.FirstOrDefault(s => !coloresUsados.Select(a => a.Color).Contains(s.Code));
                if (colorDisponible == null)
                    throw new Exception("No hay mas colores disponibles para asignar");

                _uowMemory.ColorRepository.AddPtlColor(new PtlColorEnUso
                {
                    Color = colorDisponible.Code,
                    UserId = userId,
                    Ptl = _ptl.Numero
                });

                return colorDisponible;
            }
            else
            {
                return coloresActivos.FirstOrDefault(w => w.Code == color.Color);
            }
        }

        public virtual List<PtlColor> GetColores()
        {
            var coloresUsados = _uowMemory.ColorRepository.GetPtlColoresEnUsoByPtl(_ptl.Numero);
            return coloresUsados;
        }

        public virtual void ClearColor(int userId)
        {
            var color = _uowMemory.ColorRepository.GetPtlColor(_ptl.Numero, userId);

            if (color != null)
            {
                this.FinishOperation(color.UserId, color.Color);
            }
        }

        public virtual void FinishOperation(int userId, string nuColor)
        {
            var posicionesPrendidas = _uowMemory.PosicionRepository.GetUbicacionesPrendidasByPosicionColor(_ptl.Numero, nuColor);

            if (posicionesPrendidas != null)
            {
                var result = new ValidationsResult();

                if (!_ptl.ManejaApagadoLuz() || _ptl.ManejaApagadoLuz() && this.InvokeTurnOffLigth(posicionesPrendidas, result).IsValid())
                {
                    foreach (var luz in posicionesPrendidas)
                    {
                        result = this.DescartarLuz(luz.Ubicacion, luz.Color);
                    }
                }
            }

            var color = _uowMemory.ColorRepository.GetPtlColor(_ptl.Numero, userId);

            if (color != null)
            {
                _uowMemory.ColorRepository.RemovePtlColor(_ptl.Numero, color.Color);
            }
        }

        public virtual ValidationsResult DescartarLuz(int posicion, string color)
        {
            var validationResult = new ValidationsResult();
            var luz = _uowMemory.PosicionRepository.GetPtlUbicacionPrendida(posicion, color);

            try
            {
                _uowMemory.PosicionRepository.RemovePtlUbicacionPrendida(posicion, color);

                if (luz.Estado != PtlEstadoPosicion.Error && luz.Estado != PtlEstadoPosicion.Cerrando && luz.Estado != PtlEstadoPosicion.Llena && !_uowMemory.PosicionRepository.AnyUbicacionesPrendidasByUbicacion(luz.Ubicacion))
                {
                    string ubicacion = _ptl.GetPosicion(posicion).IdUbicacion;

                    using (var uow = _uowFactory.GetUnitOfWork())
                    {
                        if (!uow.PtlRepository.AnyMercaderiaEnUbicacion(ubicacion))
                        {
                            this.LiberarPosicion(luz.Ubicacion);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.NotificarLuzErrorNoControlado(validationResult, luz, ex);
            }

            return validationResult;
        }

        protected virtual void LiberarPosicion(int nuPosicion)
        {
            using (var uow = _uowFactory.GetUnitOfWork())
            {

                var pos = uow.AutomatismoPosicionRepository.GetAutomatismoPosicionById(nuPosicion);
                if (!uow.PtlRepository.AnyMercaderiaEnUbicacion(pos.IdUbicacion))
                {
                    if (uow.GetTransactionNumber() == 0) uow.CreateTransactionNumber("LiberarPosicion");

                    pos.LimpiarComparteAgrupacion();
                    pos.Transaccion = uow.GetTransactionNumber();

                    uow.AutomatismoPosicionRepository.Update(pos);
                    uow.SaveChanges();
                }
            }
        }

        protected virtual bool PrenderLuz(List<PtlPosicionEnUso> luces, ValidationsResult result, bool colorReservado = true, long? orden = null)
        {
            if (luces.Count() > 0)
            {
                if (InvokeTurnOnLigth(luces, result).IsValid())
                {
                    foreach (PtlPosicionEnUso luz in luces)
                    {
                        _uowMemory.PosicionRepository.AddPtlUbicacionPrendida(luz, colorReservado, orden);
                    }

                    return true;
                }
            }

            return false;
        }

        public virtual void ApagarLuz(List<PtlPosicionEnUso> luces, ValidationsResult result, bool colorReservado = true)
        {
            if (_ptl.ManejaApagadoLuz() && InvokeTurnOffLigth(luces, result).IsValid())
            {
                foreach (PtlPosicionEnUso luz in luces)
                {
                    _uowMemory.PosicionRepository.RemovePtlUbicacionPrendida(luz.Ubicacion, luz.Color, colorReservado);
                }
            }
        }

        protected virtual void EnviarAlPrincipioDeLaCola(PtlPosicionEnUso luz, ValidationsResult result, bool colorReservado = true)
        {
            List<PtlPosicionEnUso> colPosicion = new List<PtlPosicionEnUso>();
            colPosicion.Add(luz);

            if (_ptl.ManejaApagadoLuz())
            {
                var cola = _uowMemory.PosicionRepository.GetUbicacionesPrendidasByPosicion(luz.Ubicacion);
                long? orden = null;

                if (cola.Any())
                {
                    orden = cola.Min(s => s.Orden) - 1;


                    this.InvokeTurnOffLigth(cola, result);
                    orden = cola.Min(s => s.Orden) - 1;
                }

                PrenderLuz(colPosicion, result, colorReservado, orden);

                if (cola.Any())
                {
                    this.InvokeTurnOnLigth(cola.OrderBy(s => s.Orden).ToList(), result);
                }
            }
            else
            {
                PrenderLuz(colPosicion, result, colorReservado);
            }
        }

        public virtual ValidationsResult ValidarOperacion(string color, int empresa, string producto)
        {
            var ubicPrendidas = _uowMemory.PosicionRepository.GetUbicacionesPrendidasByPtl(_ptl.Numero);
            var result = new ValidationsResult();
			var validaciones = new List<Error>();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                if (ubicPrendidas.Any(s => s.Color == color))
                {
                    validaciones.Add(new Error("WMSAPI_msg_Error_PtlFinalizarOperativa", ubicPrendidas.Where(s => s.Color == color).Count(), ubicPrendidas.FirstOrDefault().Producto));
                    var messages = Translator.Translate(uow, validaciones, _identity.UserId);
					result.Errors.Add(new ValidationsError(0, true, messages));
				}
			}

			return result;
        }

        public virtual ValidationsResult ValidarColor(string color, int userId)
        {
            ValidationsResult result = new ValidationsResult();
			var validaciones = new List<Error>();

			var colorEnUso = _uowMemory.ColorRepository.GetColorEnUso(_ptl.Numero, color);

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                if (colorEnUso == null)
                {
                    validaciones.Add(new Error("WMSAPI_msg_Error_PtlColorEnUso"));
                    var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                    result.Errors.Add(new ValidationsError(0, true, messages));
                }
                else if (colorEnUso.UserId != userId)
                {
                    validaciones.Add(new Error("WMSAPI_msg_Error_PtlColorOtroUsuario"));
                    var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                    result.Errors.Add(new ValidationsError(0, true, messages));
                }
            }

            return result;
        }

        public virtual ValidationsResult CerrarUbicacion(PtlPosicionEnUso accion)
        {
            var result = new ValidationsResult();
			var validaciones = new List<Error>();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                if (_uowMemory.PosicionRepository.GetUbicacionesPrendidasByPosicion(accion.Ubicacion).Any(w => w.Estado == PtlEstadoPosicion.Cerrando))
                {
					validaciones.Add(new Error("WMSAPI_msg_Error_PtlUbicacionCerrandose"));
					var messages = Translator.Translate(uow, validaciones, _identity.UserId);
					result.Errors.Add(new ValidationsError(0, true, messages));
                }
                else
                {
                    accion.Estado = PtlEstadoPosicion.Cerrando;
                    accion.Color = _ptl.GetColorCerrado();

                    _uowMemory.BeginTransaction();

                    var ubicLlena = _uowMemory.PosicionRepository.GetUbicacionesPrendidasByPtl(accion.Ptl).Where(s => s.Ubicacion == accion.Ubicacion && s.Estado == PtlEstadoPosicion.Llena).ToList();

                    if (ubicLlena != null)
                    {
                        this.ApagarLuz(ubicLlena, result, false);
                    }

                    if (_ptl.RequiereConfirmacionCierre())
                    {
                        EnviarAlPrincipioDeLaCola(accion, result);
                    }
                    else
                    {
                        _uowMemory.PosicionRepository.AddPtlUbicacionPrendida(accion, false);

                        result = ConfirmarCerrarUbicacion(accion);
                    }

                    if (result.IsValid())
                        _uowMemory.Commit();
                    else
                        _uowMemory.Rollback();
                }
            }

            return result;
        }

        protected virtual void NotificarLuzError(ValidationsResult validationResult, PtlPosicionEnUso luz)
        {
            EnviarAlPrincipioDeLaCola(luz, validationResult);

            PtlPosicionEnUso luzError = new PtlPosicionEnUso();

            luzError.Color = _ptl.GetColorError();
            luzError.Estado = PtlEstadoPosicion.Error;
            luzError.Display = _ptl.GetCodigoError();
            luzError.Ptl = luz.Ptl;
            luzError.Ubicacion = luz.Ubicacion;
            luzError.Orden = luz.Orden;
            luzError.Detalle = luz.Detalle;

            EnviarAlPrincipioDeLaCola(luzError, validationResult, false);
        }

        protected virtual void NotificarLuzErrorNoControlado(ValidationsResult validationResult, PtlPosicionEnUso luz, Exception ex)
        {
			var validaciones = new List<Error>();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
				validaciones.Add(new Error("WMSAPI_msg_Error_PtlErrorNoControlado"));
				var mensaje = Translator.Translate(uow, validaciones, _identity.UserId);
                validationResult.AddError(mensaje);
			}
			
            if (luz != null)
                NotificarLuzError(validationResult, luz);
        }

        public virtual ValidationsResult ProcesarConfirmacion(PtlPosicionEnUso accion)
        {
            var validationResult = new ValidationsResult();
			var validaciones = new List<Error>();
			var luz = _uowMemory.PosicionRepository.GetPtlUbicacionPrendida(accion.Ubicacion, accion.Color);

            if (int.TryParse(accion.Display, out int aux))
                accion.Display = aux.ToString();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                if (luz == null)
                {
					validaciones.Add(new Error("WMSAPI_msg_Error_PtlErrorLuzApagada"));
					var mensaje = Translator.Translate(uow, validaciones, _identity.UserId);
					validationResult.AddError(mensaje);
                    return validationResult;
                }
                else if (luz.Estado == PtlEstadoPosicion.Llena || accion.Display == "0" || accion.DisplayFn == _ptl.GetCodigoCancelacion())
                {
                    return this.DescartarLuz(accion.Ubicacion, accion.Color);
                }
                else if (luz.Estado == PtlEstadoPosicion.Error)
                {
                    return this.DescartarLuz(accion.Ubicacion, accion.Color);
                }
                else if (luz.Estado == PtlEstadoPosicion.Cerrando)
                {
                    return ConfirmarCerrarUbicacion(accion);
                }
            }

            validationResult = ConfirmarLuzConCantidad(accion);

            if (validationResult.IsValid())
            {
                if (!_uowMemory.PosicionRepository.AnyUbicacionesPrendidasByUbicacion(accion.Ubicacion))
                {
                    validationResult = ProcesarPosicionCompletada(accion, validationResult);
                }
            }

            return validationResult;
        }

        public abstract ValidationsResult PrenderLuces(PtlPosicionEnUso accion);

        protected abstract ValidationsResult ConfirmarLuzConCantidad(PtlPosicionEnUso accion);

        protected virtual ValidationsResult ProcesarPosicionCompletada(PtlPosicionEnUso accion, ValidationsResult validationResult)
        {
            if (IsPosicionLlena(accion))
            {
                List<PtlPosicionEnUso> luces = new List<PtlPosicionEnUso>();

                luces.Add(new PtlPosicionEnUso
                {
                    Estado = PtlEstadoPosicion.Llena,
                    Display = _ptl.GetCodigoLleno(),
                    Color = _ptl.GetColorCerrado(),
                    Ptl = accion.Ptl,
                    Ubicacion = accion.Ubicacion,
                });

                PrenderLuz(luces, validationResult);
            }

            return validationResult;
        }

        protected abstract bool IsPosicionLlena(PtlPosicionEnUso posicion);

        public abstract ValidationsResult ConfirmarCerrarUbicacion(PtlPosicionEnUso accion);

        public virtual List<PtlPosicionEnUso> GetLightsOn()
        {
            return _uowMemory.PosicionRepository.GetUbicacionesPrendidasByPtl(_ptl.Numero).ToList().OrderBy(w => w.Color).ThenBy(w => w.Orden).ToList();
        }

        public virtual List<AutomatismoPtl> GetPtlByTipo()
        {
            throw new NotImplementedException();
        }

        public virtual ValidationsResult UpdateLuzByPtlColor(PtlColorActivoRequest colorActivo)
        {
            throw new NotImplementedException();
        }

        public virtual ValidationsResult ValidatePtlReferencia(string referencia)
        {
            throw new NotImplementedException();
        }
    }
}
