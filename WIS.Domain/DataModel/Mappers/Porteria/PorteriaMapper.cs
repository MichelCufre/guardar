using System.Collections.Generic;
using WIS.Domain.Porteria;
using WIS.Domain.Recepcion;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class PorteriaMapper : Mapper
    {
        protected readonly AgenteMapper _agenteMapper;

        public PorteriaMapper(AgenteMapper agenteMapper)
        {
            this._agenteMapper = agenteMapper;
        }

        #region >> PorteriaRegistroPersona

        public virtual PorteriaRegistroPersona Map(T_PORTERIA_REGISTRO_PERSONA entity)
        {
            if (entity == null) return null;

            return new PorteriaRegistroPersona
            {
                NU_PORTERIA_REGISTRO_PERSONA = entity.NU_PORTERIA_REGISTRO_PERSONA,
                ND_TP_POTERIA_REGISTRO = entity.ND_TP_POTERIA_REGISTRO,
                ND_POTERIA_MOTIVO = entity.ND_POTERIA_MOTIVO,
                CD_FUNCIONARIO = entity.CD_FUNCIONARIO,
                CD_AGENTE = entity.CD_AGENTE,
                NU_POTERIA_PERSONA = entity.NU_POTERIA_PERSONA,
                DT_PERSONA_ENTRADA = entity.DT_PERSONA_ENTRADA,
                DT_PERSONA_SALIDA = entity.DT_PERSONA_SALIDA,
                NU_PORTERIA_VEHICULO_ENTRADA = entity.NU_PORTERIA_VEHICULO_ENTRADA,
                NU_PORTERIA_VEHICULO_SALIDA = entity.NU_PORTERIA_VEHICULO_SALIDA,
                ND_SECTOR = entity.ND_SECTOR,
                DS_NOTA = entity.DS_NOTA,
                ND_ESTADO = entity.ND_ESTADO,
                DT_ADDROW = entity.DT_ADDROW,
                DT_UPDROW = entity.DT_UPDROW,
                CD_EMPRESA = entity.CD_EMPRESA,
                CD_SECTOR = entity.CD_SECTOR,
                TP_AGENTE = entity.TP_AGENTE,
                NU_PREDIO = entity.NU_PREDIO,
            };

        }

        public virtual T_PORTERIA_REGISTRO_PERSONA Map(PorteriaRegistroPersona obj)
        {
            if (obj == null) return null;

            return new T_PORTERIA_REGISTRO_PERSONA
            {
                NU_PORTERIA_REGISTRO_PERSONA = obj.NU_PORTERIA_REGISTRO_PERSONA,
                ND_TP_POTERIA_REGISTRO = obj.ND_TP_POTERIA_REGISTRO,
                ND_POTERIA_MOTIVO = obj.ND_POTERIA_MOTIVO,
                CD_FUNCIONARIO = obj.CD_FUNCIONARIO,
                CD_AGENTE = obj.CD_AGENTE,
                NU_POTERIA_PERSONA = obj.NU_POTERIA_PERSONA,
                DT_PERSONA_ENTRADA = obj.DT_PERSONA_ENTRADA,
                DT_PERSONA_SALIDA = obj.DT_PERSONA_SALIDA,
                NU_PORTERIA_VEHICULO_ENTRADA = obj.NU_PORTERIA_VEHICULO_ENTRADA,
                NU_PORTERIA_VEHICULO_SALIDA = obj.NU_PORTERIA_VEHICULO_SALIDA,
                ND_SECTOR = obj.ND_SECTOR,
                DS_NOTA = obj.DS_NOTA,
                ND_ESTADO = obj.ND_ESTADO,
                DT_ADDROW = obj.DT_ADDROW,
                DT_UPDROW = obj.DT_UPDROW,
                CD_EMPRESA = obj.CD_EMPRESA,
                CD_SECTOR = obj.CD_SECTOR,
                TP_AGENTE = obj.TP_AGENTE,
                NU_PREDIO = obj.NU_PREDIO,

            };

        }

        #endregion << PorteriaRegistroPersona

        #region >> PorteriaRegistroVehiculo

        public virtual PorteriaRegistroVehiculo Map(T_PORTERIA_REGISTRO_VEHICULO entity, List<T_PORTERIA_REGISTRO_PERSONA> T_PORTERIA_REGISTRO_PERSONA = null)
        {
            if (entity == null) return null;

            List<PorteriaRegistroPersona> lineas = new List<PorteriaRegistroPersona>();

            if (T_PORTERIA_REGISTRO_PERSONA != null)
            {
                foreach (var liena in T_PORTERIA_REGISTRO_PERSONA)
                {
                    lineas.Add(this.Map(liena));
                }
            }

            return new PorteriaRegistroVehiculo
            {
                NU_PORTERIA_VEHICULO = entity.NU_PORTERIA_VEHICULO,
                ND_TRANSPORTE = entity.ND_TRANSPORTE,
                VL_MATRICULA_1 = entity.VL_MATRICULA_1,
                VL_MATRICULA_2 = entity.VL_MATRICULA_2,
                VL_PESO_ENTRADA = entity.VL_PESO_ENTRADA,
                VL_PESO_SALIDA = entity.VL_PESO_SALIDA,
                CD_EMPRESA = entity.CD_EMPRESA,
                DT_PORTERIA_ENTRADA = entity.DT_PORTERIA_ENTRADA,
                DT_PORTERIA_SALIDA = entity.DT_PORTERIA_SALIDA,
                DT_ADDROW = entity.DT_ADDROW,
                DT_UPDROW = entity.DT_UPDROW,
                FL_SOLO_BALANZA = entity.FL_SOLO_BALANZA,
                NU_EJECUCION_ENTRADA = entity.NU_EJECUCION_ENTRADA,
                NU_EJECUCION_SALIDA = entity.NU_EJECUCION_SALIDA,
                FL_SALIDA_HABILITADA = entity.FL_SALIDA_HABILITADA,
                FL_CONTROL_SALIDA = entity.FL_CONTROL_SALIDA,
                ND_TP_FACTURACION = entity.ND_TP_FACTURACION,
                Personas = lineas,
                CD_AGENTE = entity.CD_AGENTE,
                NU_PREDIO = entity.NU_PREDIO,
                CD_SECTOR = entity.CD_SECTOR,
                ND_POTERIA_MOTIVO = entity.ND_POTERIA_MOTIVO,
                ND_SECTOR = entity.ND_SECTOR,
                TP_AGENTE = entity.TP_AGENTE,
            };

        }

        public virtual T_PORTERIA_REGISTRO_VEHICULO Map(PorteriaRegistroVehiculo obj)
        {
            if (obj == null) return null;

            return new T_PORTERIA_REGISTRO_VEHICULO
            {
                NU_PORTERIA_VEHICULO = obj.NU_PORTERIA_VEHICULO,
                ND_TRANSPORTE = obj.ND_TRANSPORTE,
                VL_MATRICULA_1 = obj.VL_MATRICULA_1,
                VL_MATRICULA_2 = obj.VL_MATRICULA_2,
                VL_PESO_ENTRADA = obj.VL_PESO_ENTRADA,
                VL_PESO_SALIDA = obj.VL_PESO_SALIDA,
                CD_EMPRESA = obj.CD_EMPRESA,
                DT_PORTERIA_ENTRADA = obj.DT_PORTERIA_ENTRADA,
                DT_PORTERIA_SALIDA = obj.DT_PORTERIA_SALIDA,
                DT_ADDROW = obj.DT_ADDROW,
                DT_UPDROW = obj.DT_UPDROW,
                FL_SOLO_BALANZA = obj.FL_SOLO_BALANZA,
                NU_EJECUCION_ENTRADA = obj.NU_EJECUCION_ENTRADA,
                NU_EJECUCION_SALIDA = obj.NU_EJECUCION_SALIDA,
                FL_SALIDA_HABILITADA = obj.FL_SALIDA_HABILITADA,
                FL_CONTROL_SALIDA = obj.FL_CONTROL_SALIDA,
                ND_TP_FACTURACION = obj.ND_TP_FACTURACION,
                CD_AGENTE = obj.CD_AGENTE,
                NU_PREDIO = obj.NU_PREDIO,
                CD_SECTOR = obj.CD_SECTOR,
                ND_POTERIA_MOTIVO = obj.ND_POTERIA_MOTIVO,
                ND_SECTOR = obj.ND_SECTOR,
                TP_AGENTE = obj.TP_AGENTE,

            };

        }

        #endregion << PorteriaRegistroVehiculo

        #region >> PorteriaPersona

        public virtual PorteriaPersona Map(T_PORTERIA_PERSONA entity)
        {
            if (entity == null) return null;

            return new PorteriaPersona
            {
                NU_POTERIA_PERSONA = entity.NU_POTERIA_PERSONA,
                NU_DOCUMENTO = entity.NU_DOCUMENTO,
                ND_TP_DOCUMENTO = entity.ND_TP_DOCUMENTO,
                CD_PAIS_EMISOR = entity.CD_PAIS_EMISOR,
                NM_PERSONA = entity.NM_PERSONA,
                AP_PERSONA = entity.AP_PERSONA,
                NU_CELULAR = entity.NU_CELULAR,
                ND_TP_PERSONA = entity.ND_TP_PERSONA,
                ND_PUESTO_FUNCIONARIO = entity.ND_PUESTO_FUNCIONARIO,
                DT_ADDROW = entity.DT_ADDROW,
                DT_UPDROW = entity.DT_UPDROW,
                CD_EMPRESA = entity.CD_EMPRESA,
                CD_PORTERIA_EMPRESA = entity.CD_PORTERIA_EMPRESA,
            };

        }

        public virtual T_PORTERIA_PERSONA Map(PorteriaPersona entity)
        {
            if (entity == null) return null;

            return new T_PORTERIA_PERSONA
            {
                NU_POTERIA_PERSONA = entity.NU_POTERIA_PERSONA,
                NU_DOCUMENTO = entity.NU_DOCUMENTO,
                ND_TP_DOCUMENTO = entity.ND_TP_DOCUMENTO,
                CD_PAIS_EMISOR = entity.CD_PAIS_EMISOR,
                NM_PERSONA = entity.NM_PERSONA,
                AP_PERSONA = entity.AP_PERSONA,
                NU_CELULAR = entity.NU_CELULAR,
                ND_TP_PERSONA = entity.ND_TP_PERSONA,
                ND_PUESTO_FUNCIONARIO = entity.ND_PUESTO_FUNCIONARIO,
                DT_ADDROW = entity.DT_ADDROW,
                DT_UPDROW = entity.DT_UPDROW,
                CD_EMPRESA = entity.CD_EMPRESA,
                CD_PORTERIA_EMPRESA = entity.CD_PORTERIA_EMPRESA,
            };

        }

        #endregion << PorteriaPersona

        #region >> PorteriaSector

        public virtual T_PORTERIA_SECTOR Map(PorteriaSector entity)
        {
            if (entity == null) return null;

            return new T_PORTERIA_SECTOR
            {

                CD_SECTOR = entity.CD_SECTOR,
                DS_SECTOR = entity.DS_SECTOR,
                CD_PORTA = entity.CD_PORTA,
                DT_ADDROW = entity.DT_ADDROW,
                DT_UDPROW = entity.DT_UDPROW,
                NU_PREDIO = entity.NU_PREDIO,
            };
        }

        public virtual PorteriaSector Map(T_PORTERIA_SECTOR entity)
        {
            if (entity == null) return null;

            return new PorteriaSector
            {

                CD_SECTOR = entity.CD_SECTOR,
                DS_SECTOR = entity.DS_SECTOR,
                CD_PORTA = entity.CD_PORTA,
                DT_ADDROW = entity.DT_ADDROW,
                DT_UDPROW = entity.DT_UDPROW,
                NU_PREDIO = entity.NU_PREDIO,
            };
        }

        #endregion << PorteriaSector

        #region >> PorteriaVehiculoAgenda

        public virtual PorteriaVehiculoAgenda Map(T_PORTERIA_VEHICULO_AGENDA entity)
        {
            if (entity == null) return null;

            return new PorteriaVehiculoAgenda
            {
                NU_PORTERIA_VEHICULO_AGENDA = entity.NU_PORTERIA_VEHICULO_AGENDA,
                NU_PORTERIA_VEHICULO = entity.NU_PORTERIA_VEHICULO,
                NU_AGENDA = entity.NU_AGENDA,

            };
        }

        public virtual T_PORTERIA_VEHICULO_AGENDA Map(PorteriaVehiculoAgenda obj)
        {
            if (obj == null) return null;

            return new T_PORTERIA_VEHICULO_AGENDA
            {
                NU_PORTERIA_VEHICULO_AGENDA = obj.NU_PORTERIA_VEHICULO_AGENDA,
                NU_PORTERIA_VEHICULO = obj.NU_PORTERIA_VEHICULO,
                NU_AGENDA = obj.NU_AGENDA,

            };
        }


        #endregion << PorteriaVehiculoAgenda

        #region >> PorteriaVehiculoCamion

        public virtual PorteriaVehiculoCamion Map(T_PORTERIA_VEHICULO_CAMION entity)
        {
            if (entity == null) return null;

            return new PorteriaVehiculoCamion
            {
                NU_PORTERIA_VEHICULO_CAMION = entity.NU_PORTERIA_VEHICULO_CAMION,
                NU_PORTERIA_VEHICULO = entity.NU_PORTERIA_VEHICULO,
                CD_CAMION = entity.CD_CAMION,

            };
        }

        public virtual T_PORTERIA_VEHICULO_CAMION Map(PorteriaVehiculoCamion obj)
        {
            if (obj == null) return null;

            return new T_PORTERIA_VEHICULO_CAMION
            {
                NU_PORTERIA_VEHICULO_CAMION = obj.NU_PORTERIA_VEHICULO_CAMION,
                NU_PORTERIA_VEHICULO = obj.NU_PORTERIA_VEHICULO,
                CD_CAMION = obj.CD_CAMION,

            };
        }


        #endregion << PorteriaVehiculoCamion

        #region >> PorteriaVehiculoObjeto

        public virtual PorteriaVehiculoObjeto Map(T_PORTERIA_VEHICULO_OBJETO entity)
        {
            if (entity == null) return null;

            return new PorteriaVehiculoObjeto
            {
                NU_PORTERIA_VEHICULO_OBJETO = entity.NU_PORTERIA_VEHICULO_OBJETO,
                NU_PORTERIA_VEHICULO = entity.NU_PORTERIA_VEHICULO,
                CD_OBJETO = entity.CD_OBJETO,
                TP_OBJETO = entity.TP_OBJETO,

            };
        }

        public virtual T_PORTERIA_VEHICULO_OBJETO Map(PorteriaVehiculoObjeto obj)
        {
            if (obj == null) return null;

            return new T_PORTERIA_VEHICULO_OBJETO
            {
                NU_PORTERIA_VEHICULO_OBJETO = obj.NU_PORTERIA_VEHICULO_OBJETO,
                NU_PORTERIA_VEHICULO = obj.NU_PORTERIA_VEHICULO,
                CD_OBJETO = obj.CD_OBJETO,
                TP_OBJETO = obj.TP_OBJETO,
            };
        }


        #endregion << PorteriaVehiculoObjeto

        public virtual Container Map(T_CONTAINER entity)
        {
            if (entity == null) return null;

            return new Container
            {
                Numero = entity.NU_CONTAINER,
                Id = entity.NU_SEQ_CONTAINER,
                TP_CONTAINER = entity.TP_CONTAINER,
                Empresa = entity.CD_EMPRESA,
                DT_ADDROW = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UPDROW,
                DT_VEN_RET_TERMINAL = entity.DT_VEN_RET_TERMINAL,
                DT_FIN_RET_TERMINAL = entity.DT_FIN_RET_TERMINAL,
                DT_MAX_DEVOLUCION = entity.DT_MAX_DEVOLUCION,
                DT_INI_DEVOLUCION = entity.DT_INI_DEVOLUCION,
                DT_FIN_DEVOLUCION = entity.DT_FIN_DEVOLUCION,
                DT_FIN_APERTURA = entity.DT_FIN_APERTURA,
                CD_SITUACAO = entity.CD_SITUACAO,
                PS_TARA = entity.PS_TARA,
                ID_CONSOLIDADO = entity.ID_CONSOLIDADO,
                CD_ENDERECO_DEPOSITO = entity.CD_ENDERECO_DEPOSITO,
                CD_FUNC_ENCARGADO = entity.CD_FUNC_ENCARGADO,
                CD_FUNC_VERIFICADOR = entity.CD_FUNC_VERIFICADOR,
                DS_MEMO = entity.DS_MEMO,
                DT_ENTREGA_DOCUMENTO = entity.DT_ENTREGA_DOCUMENTO,
                CD_TERMINAL_ENTREGA = entity.CD_TERMINAL_ENTREGA,
                CD_TERMINAL_DEVOLUCION = entity.CD_TERMINAL_DEVOLUCION,
                CD_TRANSPORTISTA_RETIRO = entity.CD_TRANSPORTISTA_RETIRO,
                CD_TRANSPORTISTA_DEVOLUCION = entity.CD_TRANSPORTISTA_DEVOLUCION,
                VL_PRECINTO_1 = entity.VL_PRECINTO_1,
                VL_PRECINTO_2 = entity.VL_PRECINTO_2,
                DS_OBSERVACIONES_1 = entity.DS_OBSERVACIONES_1,
                DS_OBSERVACIONES_2 = entity.DS_OBSERVACIONES_2,
                DS_BOOKING = entity.DS_BOOKING,
                DT_POSICIONAMIENTO = entity.DT_POSICIONAMIENTO,
                DT_APERTURA = entity.DT_APERTURA,
                FL_PALETIZADO = entity.FL_PALETIZADO,
                CD_FUNCIONARIO_APERTURA = entity.CD_FUNCIONARIO_APERTURA,
                CD_FUNCIONARIO_CIERRE = entity.CD_FUNCIONARIO_CIERRE,
                FechaEntrada = entity.DT_PORTERIA_ENTRADA,
                FechaSalida = entity.DT_PORTERIA_SALIDA,
                NumeroTransaccion = entity.NU_TRANSACCION,
                NumeroTransaccionDelete = entity.NU_TRANSACCION_DELETE
            };
        }
        public virtual T_CONTAINER Map(Container obj)
        {
            if (obj == null) return null;

            return new T_CONTAINER
            {
                NU_CONTAINER = obj.Numero,
                NU_SEQ_CONTAINER = obj.Id,
                TP_CONTAINER = obj.TP_CONTAINER,
                CD_EMPRESA = obj.Empresa,
                DT_ADDROW = obj.DT_ADDROW,
                DT_UPDROW = obj.FechaModificacion,
                DT_VEN_RET_TERMINAL = obj.DT_VEN_RET_TERMINAL,
                DT_FIN_RET_TERMINAL = obj.DT_FIN_RET_TERMINAL,
                DT_MAX_DEVOLUCION = obj.DT_MAX_DEVOLUCION,
                DT_INI_DEVOLUCION = obj.DT_INI_DEVOLUCION,
                DT_FIN_DEVOLUCION = obj.DT_FIN_DEVOLUCION,
                DT_FIN_APERTURA = obj.DT_FIN_APERTURA,
                CD_SITUACAO = obj.CD_SITUACAO,
                PS_TARA = obj.PS_TARA,
                ID_CONSOLIDADO = obj.ID_CONSOLIDADO,
                CD_ENDERECO_DEPOSITO = obj.CD_ENDERECO_DEPOSITO,
                CD_FUNC_ENCARGADO = obj.CD_FUNC_ENCARGADO,
                CD_FUNC_VERIFICADOR = obj.CD_FUNC_VERIFICADOR,
                DS_MEMO = obj.DS_MEMO,
                DT_ENTREGA_DOCUMENTO = obj.DT_ENTREGA_DOCUMENTO,
                CD_TERMINAL_ENTREGA = obj.CD_TERMINAL_ENTREGA,
                CD_TERMINAL_DEVOLUCION = obj.CD_TERMINAL_DEVOLUCION,
                CD_TRANSPORTISTA_RETIRO = obj.CD_TRANSPORTISTA_RETIRO,
                CD_TRANSPORTISTA_DEVOLUCION = obj.CD_TRANSPORTISTA_DEVOLUCION,
                VL_PRECINTO_1 = obj.VL_PRECINTO_1,
                VL_PRECINTO_2 = obj.VL_PRECINTO_2,
                DS_OBSERVACIONES_1 = obj.DS_OBSERVACIONES_1,
                DS_OBSERVACIONES_2 = obj.DS_OBSERVACIONES_2,
                DS_BOOKING = obj.DS_BOOKING,
                DT_POSICIONAMIENTO = obj.DT_POSICIONAMIENTO,
                DT_APERTURA = obj.DT_APERTURA,
                FL_PALETIZADO = obj.FL_PALETIZADO,
                CD_FUNCIONARIO_APERTURA = obj.CD_FUNCIONARIO_APERTURA,
                CD_FUNCIONARIO_CIERRE = obj.CD_FUNCIONARIO_CIERRE,
                DT_PORTERIA_ENTRADA = obj.FechaEntrada,
                DT_PORTERIA_SALIDA = obj.FechaSalida,
                NU_TRANSACCION = obj.NumeroTransaccion,
                NU_TRANSACCION_DELETE = obj.NumeroTransaccionDelete,
            };
        }
    }
}
