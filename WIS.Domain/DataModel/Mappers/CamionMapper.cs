using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Expedicion;
using WIS.Domain.Expedicion.Enums;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class CamionMapper : Mapper
    {
        public CamionMapper()
        {

        }

        public virtual Camion MapToObject(T_CAMION entity)
        {
            if (entity == null) return null;

            return new Camion
            {
                Id = entity.CD_CAMION,
                Matricula = entity.CD_PLACA_CARRO,
                Ruta = entity.CD_ROTA,
                Puerta = entity.CD_PORTA,
                RespetaOrdenCarga = this.MapStringToBoolean(entity.ID_RESPETA_ORD_CARGA),
                Transportista = entity.CD_TRANSPORTADORA,
                FechaCreacion = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UPDROW,
                Empresa = entity.CD_EMPRESA,
                FechaProgramado = entity.DT_PROGRAMADO,
                FechaArriboCamion = entity.DT_ARRIBO_CAMION,
                Descripcion = entity.DS_CAMION,
                Vehiculo = entity.CD_VEICULO,
                FechaFacturacion = entity.DT_FACTURACION,
                FuncionarioCierre = entity.CD_FUNC_CIERRE,
                FechaCierre = entity.DT_CIERRE,
                NumeroInterfazEjecucionCierre = entity.NU_INTERFAZ_EJECUCION,
                NumeroInterfazEjecucionFactura = entity.NU_INTERFAZ_EJECUCION_FACT,
                IsTrackingHabilitado = this.MapStringToBoolean(entity.FL_TRACKING),
                IsRuteoHabilitado = this.MapStringToBoolean(entity.FL_RUTEO),
                NumeroOrtOrden = entity.NU_ORT_ORDEN,
                Predio = entity.NU_PREDIO,
                IsSincronizacionRealizada = this.MapStringToBoolean(entity.FL_SYNC_REALIZADA),
                ArmadoEgreso = entity.ND_ARMADO_EGRESO,
                IsCierreParcialHabilitado = this.MapStringToBoolean(entity.FL_CIERRE_PARCIAL),
                NumeroTransaccion = entity.NU_TRANSACCION,
                IsCierreHabilitado = this.MapStringToBoolean(entity.FL_HABILITADO_CIERRE),
                ArmadoHabilitado = this.MapStringToBoolean(entity.FL_HABILITADO_ARMADO),
                IsCargaHabilitada = this.MapStringToBoolean(entity.FL_HABILITADO_CARGA),
                Documento = entity.DS_DOCUMENTO,
                TipoArmadoEgreso = entity.TP_ARMADO_EGRESO,
                Estado = this.MapEstado(entity.CD_SITUACAO),
                IsCierreAutomaticoHabilitado = this.MapStringToBoolean(entity.FL_CIERRE_AUTO),
                IsCargaAutomaticaHabilitada = this.MapStringToBoolean(entity.FL_AUTO_CARGA),
                IsControlContenedoresHabilitado = this.MapStringToBoolean(entity.FL_CTRL_CONTENEDORES),
                ConfirmacionViajeRealizada = this.MapStringToBoolean(entity.FL_CONF_VIAJE_REALIZADA),
                IdExterno = entity.ID_EXTERNO,
                EmpresaExterna = entity.CD_EMPRESA_EXTERNA
            };
        }

        public virtual T_CAMION MapToEntity(Camion obj)
        {
            return new T_CAMION
            {
                CD_CAMION = obj.Id,
                CD_PLACA_CARRO = obj.Matricula,
                CD_ROTA = obj.Ruta,
                CD_SITUACAO = this.MapEstado(obj.Estado),
                CD_PORTA = obj.Puerta,
                ID_RESPETA_ORD_CARGA = this.MapBooleanToString(obj.RespetaOrdenCarga),
                CD_TRANSPORTADORA = obj.Transportista,
                DT_ADDROW = obj.FechaCreacion,
                DT_UPDROW = obj.FechaModificacion,
                CD_EMPRESA = obj.Empresa,
                DT_PROGRAMADO = obj.FechaProgramado,
                DT_ARRIBO_CAMION = obj.FechaArriboCamion,
                DS_CAMION = obj.Descripcion,
                CD_VEICULO = obj.Vehiculo,
                DT_FACTURACION = obj.FechaFacturacion,
                CD_FUNC_CIERRE = obj.FuncionarioCierre,
                DT_CIERRE = obj.FechaCierre,
                NU_INTERFAZ_EJECUCION = obj.NumeroInterfazEjecucionCierre,
                NU_INTERFAZ_EJECUCION_FACT = obj.NumeroInterfazEjecucionFactura,
                FL_TRACKING = this.MapBooleanToString(obj.IsTrackingHabilitado),
                FL_RUTEO = this.MapBooleanToString(obj.IsRuteoHabilitado),
                NU_ORT_ORDEN = obj.NumeroOrtOrden,
                NU_PREDIO = obj.Predio,
                FL_SYNC_REALIZADA = this.MapBooleanToString(obj.IsSincronizacionRealizada),
                ND_ARMADO_EGRESO = obj.ArmadoEgreso,
                NU_TRANSACCION = obj.NumeroTransaccion,
                DS_DOCUMENTO = obj.Documento,
                FL_CIERRE_PARCIAL = this.MapBooleanToString(obj.IsCierreParcialHabilitado),
                FL_HABILITADO_CARGA = this.MapBooleanToString(obj.IsCargaHabilitada),
                FL_HABILITADO_ARMADO = this.MapBooleanToString(obj.ArmadoHabilitado),
                FL_HABILITADO_CIERRE = this.MapBooleanToString(obj.IsCierreHabilitado),
                TP_ARMADO_EGRESO = obj.TipoArmadoEgreso,
                FL_CIERRE_AUTO = this.MapBooleanToString(obj.IsCierreAutomaticoHabilitado),
                FL_AUTO_CARGA = this.MapBooleanToString(obj.IsCargaAutomaticaHabilitada),
                FL_CTRL_CONTENEDORES = this.MapBooleanToString(obj.IsControlContenedoresHabilitado),
                FL_CONF_VIAJE_REALIZADA = this.MapBooleanToString(obj.ConfirmacionViajeRealizada),
                ID_EXTERNO = obj.IdExterno,
                CD_EMPRESA_EXTERNA = obj.EmpresaExterna
            };
        }

        public virtual CamionDescripcion MapToObject(V_CAMION_EXP050 entity)
        {
            if (entity == null) return null;

            return new CamionDescripcion()
            {
                Id = entity.CD_CAMION,
                Matricula = entity.CD_PLACA_CARRO,
                Empresa = entity.CD_EMPRESA,
                DescEmpresa = entity.NM_EMPRESA,
                Puerta = entity.CD_PORTA,
                DescPuerta = entity.DS_PORTA,
                Estado = this.MapEstado(entity.CD_SITUACAO),
                DescSituacion = entity.DS_SITUACAO,
                Ruta = entity.CD_ROTA,
                DescRuta = entity.DS_ROTA,
                FechaCreacion = entity.DT_ADDROW
            };
        }

        public virtual CamionEstado MapEstado(short estado)
        {
            switch (estado)
            {
                case SituacionDb.CamionAguardandoCarga: return CamionEstado.AguardandoCarga;
                case SituacionDb.CamionCargando: return CamionEstado.Cargando;
                case SituacionDb.CamionCerrado: return CamionEstado.Cerrado;
                case SituacionDb.CamionIniciandoCierre: return CamionEstado.IniciandoCierre;
                case SituacionDb.CamionSinOrdenDeTrabajo: return CamionEstado.SinOrdenDeTrabajo;
            }

            return CamionEstado.Unknown;
        }

        public virtual short MapEstado(CamionEstado estado)
        {
            switch (estado)
            {
                case CamionEstado.AguardandoCarga: return SituacionDb.CamionAguardandoCarga;
                case CamionEstado.Cargando: return SituacionDb.CamionCargando;
                case CamionEstado.Cerrado: return SituacionDb.CamionCerrado;
                case CamionEstado.IniciandoCierre: return SituacionDb.CamionIniciandoCierre;
                case CamionEstado.SinOrdenDeTrabajo: return SituacionDb.CamionSinOrdenDeTrabajo;
            }

            return -1;
        }
    }
}
