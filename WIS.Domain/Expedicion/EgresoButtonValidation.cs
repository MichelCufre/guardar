using System;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Services.Interfaces;
using WIS.Extension;

namespace WIS.Domain.Expedicion
{
    public class EgresoButtonValidation
    {
        protected readonly IParameterService _parameterService;
        protected readonly IUnitOfWork _uow;
        protected readonly CamionMapper _mapper;

        protected Camion _camion;
        protected bool _tienePedidosPendientes;
        protected bool _manejoDocumentalActivo;
        protected bool _egresoDocumentalEditable;

        public EgresoButtonValidation(IParameterService parameterService, IUnitOfWork uow)
        {
            this._parameterService = parameterService;
            this._uow = uow;
            this._mapper = new CamionMapper();
        }

        public virtual void LoadCamion(int codigoCamion, string situacion, string interfazEjecucionFactura, string isTrackingEnabled, string isSincronizacionRealizada, string confirmacionViajeRealizada, string documento, string fechaFacturacion, string pedidosPendientes, string codigoEmpresa, string tpArmadoEgreso)
        {
            this._camion = new Camion
            {
                Id = codigoCamion,
                Estado = this._mapper.MapEstado(Convert.ToInt16(situacion)),
                NumeroInterfazEjecucionFactura = string.IsNullOrEmpty(interfazEjecucionFactura) ? null : (long?)Convert.ToInt64(interfazEjecucionFactura),
                IsTrackingHabilitado = this._mapper.MapStringToBoolean(isTrackingEnabled),
                IsSincronizacionRealizada = this._mapper.MapStringToBoolean(isSincronizacionRealizada),
                ConfirmacionViajeRealizada = this._mapper.MapStringToBoolean(confirmacionViajeRealizada),
                FechaFacturacion = string.IsNullOrEmpty(fechaFacturacion) ? null : DateTimeExtension.ParseFromIso(fechaFacturacion),
                Documento = documento,
                Empresa = string.IsNullOrEmpty(codigoEmpresa) ? null : (int?)int.Parse(codigoEmpresa),
                TipoArmadoEgreso = tpArmadoEgreso,
            };

            this._tienePedidosPendientes = this._mapper.MapStringToBoolean(pedidosPendientes);
            this._manejoDocumentalActivo = this._camion.Empresa.HasValue && this._parameterService.GetValueByEmpresa(ParamManager.MANEJO_DOCUMENTAL, this._camion.Empresa.Value) == "S";

            if (this._manejoDocumentalActivo)
            {
                var documentoEgreso = this._uow.DocumentoRepository.GetEgresoPorCamion(this._camion.Id);

                if (documentoEgreso == null)
                    this._egresoDocumentalEditable = true;
                else
                    this._egresoDocumentalEditable = this._uow.DocumentoTipoRepository.PermiteEditarCamion(documentoEgreso.Tipo, documentoEgreso.Estado);
            }
        }

        public virtual bool IsCerrado()
        {
            if (this._camion == null)
                throw new InvalidOperationException("EXP040_Sec0_Error_EgresonoNoCargado");

            return this._camion.IsCerrado();
        }

        public virtual bool PuedeFacturarse()
        {
            if (this._camion == null)
                throw new InvalidOperationException("EXP040_Sec0_Error_EgresonoNoCargado");

            return (this._camion.PuedeFacturarse() && _uow.CamionRepository.RequiereFacturacion(_camion.Id));
        }

        public virtual bool PuedeGenerarseEgresoDocumental()
        {
            if (this._camion == null)
                throw new InvalidOperationException("EXP040_Sec0_Error_EgresonoNoCargado");

            return this._camion.PuedeGenerarseEgresoDocumental(this._manejoDocumentalActivo, this._egresoDocumentalEditable);
        }

        public virtual bool PuedeArmarse(bool armadoPorContenedor = false)
        {
            if (this._camion == null)
                throw new InvalidOperationException("EXP040_Sec0_Error_EgresonoNoCargado");

            return this._camion.PuedeArmarse(this._manejoDocumentalActivo, this._egresoDocumentalEditable, armadoPorContenedor);
        }

        public virtual bool PuedeArmarsePorCarga()
        {
            if (this._camion == null)
                throw new InvalidOperationException("EXP040_Sec0_Error_EgresonoNoCargado");

            return this._camion.PuedeArmarsePorCarga(this._manejoDocumentalActivo, this._egresoDocumentalEditable);
        }

        public virtual bool PuedeSincronizarTracking()
        {
            return this._camion.PuedeSincronizarTracking();
        }

        public virtual bool PuedeReSincronizarTracking()
        {
            return this._camion.PuedeReSincronizarTracking();
        }

        public virtual bool TienePedidosPendientes()
        {
            return this._tienePedidosPendientes;
        }

        public virtual bool IsPlanificacion()
        {
            if (this._camion == null)
                throw new InvalidOperationException("EXP040_Sec0_Error_EgresonoNoCargado");

            return _camion.TipoArmadoEgreso == TipoArmadoEgreso.Planificacion;
        }
    }
}
