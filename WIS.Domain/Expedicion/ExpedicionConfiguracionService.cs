using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Picking;
using WIS.Domain.Services.Interfaces;

namespace WIS.Domain.Expedicion
{
    public class ExpedicionConfiguracionService
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IParameterService _parameterService;
        protected readonly ParametroMapper _mapper;

        public ExpedicionConfiguracionService(IUnitOfWork uow,
            IParameterService parameterService,
            ParametroMapper mapper)
        {
            this._uow = uow;
            this._parameterService = parameterService;
            this._mapper = mapper;
        }

        public virtual ConfiguracionExpedicion GetConfiguracion()
        {
            Dictionary<string, string> parameters = this._uow.ParametroRepository.GetParameters(new List<string> {
                ParamManager.WEXP040_CONTROLAR_FACT_CIERRE,
                ParamManager.MANEJO_DOCUMENTAL,
                ParamManager.VL_DIF_CD_GRUPO_EXPEDICION_PED
            });

            return new ConfiguracionExpedicion
            {
                IsControlFacturacionRequerido = this._mapper.MapStringToBoolean(parameters[ParamManager.WEXP040_CONTROLAR_FACT_CIERRE]),
                PermiteAsociarDisitntosGruposExpedicion = this._mapper.MapStringToBoolean(parameters[ParamManager.VL_DIF_CD_GRUPO_EXPEDICION_PED])
            };
        }

        public virtual ConfiguracionPedido GetConfiguracionPedido()
        {
            Dictionary<string, string> parameters = this._uow.ParametroRepository.GetParameters(new List<string> {
                ParamManager.PEDIDO_NUMERICO,
                ParamManager.WPRE100_VALIDAR_HORAS_ENTRE_DT,
                ParamManager.FILTRAR_TP_AGENTE_CLI,
            });

            return new ConfiguracionPedido
            {
                PedidoDebeSerNumerico = this._mapper.MapStringToBoolean(parameters[ParamManager.PEDIDO_NUMERICO]),
                ValidarHorasEntreEmisionEntrega = this._mapper.MapStringToBoolean(parameters[ParamManager.WPRE100_VALIDAR_HORAS_ENTRE_DT]),
                PermitePedidosAProveedores = !this._mapper.MapStringToBoolean(parameters[ParamManager.FILTRAR_TP_AGENTE_CLI])
            };
        }
        public virtual bool IsCierreParcialHabilitado(int empresa)
        {
            var value = this._parameterService.GetValueByEmpresa(ParamManager.WEXP040_CAMION_CIERRE_PARCIAL, empresa);

            return this._mapper.MapStringToBoolean(value);
        }
        public virtual bool IsManejoDocumentalHabilitado(int empresa)
        {
            var value = this._parameterService.GetValueByEmpresa(ParamManager.MANEJO_DOCUMENTAL, empresa);

            return this._mapper.MapStringToBoolean(value);
        }
        public virtual bool RespetaOrdenCargarDefault(int empresa)
        {
            var value = this._parameterService.GetValueByEmpresa(ParamManager.RESPETA_ORDEN_CARGA_DEFAULT, empresa);

            return this._mapper.MapStringToBoolean(value);
        }
        public virtual bool RuteoHabilitadoDefault(int empresa)
        {
            var value = this._parameterService.GetValueByEmpresa(ParamManager.RUTEO_HABILITADO_DEFAULT, empresa);

            return this._mapper.MapStringToBoolean(value);
        }
        public virtual int TransportistaDefault(int empresa)
        {
            int cdTransportista = -1;
            var value = this._parameterService.GetValueByEmpresa(ParamManager.CD_TRANSPORTADORA_DEFAULT, empresa);

            if (int.TryParse(value, out int parsedValue))
                cdTransportista = parsedValue;

            if (!this._uow.TransportistaRepository.AnyTransportista(cdTransportista))
                cdTransportista = _uow.TransportistaRepository.GetFirstTransportista().Id;

            return cdTransportista;
        }
    }
}
