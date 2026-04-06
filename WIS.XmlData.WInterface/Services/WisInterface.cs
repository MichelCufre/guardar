using WIS.XmlData.WInterface.Models;
using WIS.XmlData.WInterface.Services.Interfaces;

namespace WIS.XmlData.WInterface.Services
{
    public class WisInterface : IWisInterface
    {
        protected readonly IXmlDataExternalManager _manager;

        public WisInterface(IXmlDataExternalManager manager)
        {
            _manager = manager;
        }

        #region Sesion

        public async Task<RESPUESTA_INTERFAZ> Sesion(INTERFAZ_SESSION INTERFAZ)
        {
           return await _manager.Sesion(INTERFAZ);
        }

        #endregion

        #region Entrada

        public async Task<RESPUESTA_INTERFAZ> Inciar_Ejecucion(INTERFAZ INTERFAZ)
        {
            return await _manager.Inciar_Ejecucion(INTERFAZ);
        }

        public async Task<RESPUESTA_INTERFAZ> Enviar_Datos(INTERFAZ INTERFAZ)
        {
            return await _manager.Enviar_Datos(INTERFAZ);
        }

        public async Task<RESPUESTA_INTERFAZ> Consultar_Estado(INTERFAZ INTERFAZ)
        {
            return await _manager.Consultar_Estado(INTERFAZ);
        }

        #endregion

        #region Salida

        public async Task<RESPUESTA_INTERFAZ> Ejecuciones_Pendientes(INTERFAZ INTERFAZ)
        {
            return await _manager.Ejecuciones_Pendientes(INTERFAZ);
        }

        public async Task<RESPUESTA_INTERFAZ> Consultar_Ejecucion(INTERFAZ INTERFAZ)
        {
            return await _manager.Consultar_Ejecucion(INTERFAZ);
        }

        public async Task<RESPUESTA_INTERFAZ> Ejecucion_Leida(INTERFAZ INTERFAZ)
        {
            return await _manager.Ejecucion_Leida(INTERFAZ);
        }

        #endregion

        #region Consulta

        public async Task<RESPUESTA_INTERFAZ> Consultar_Datos(INTERFAZ INTERFAZ)
        {
            return await _manager.Consultar_Datos(INTERFAZ);
        }

        #endregion

        #region Mensaje

        public async Task<RESPUESTA_INTERFAZ> Notificacion(INTERFAZ INTERFAZ)
        {
            return await _manager.Notificacion(INTERFAZ);
        }

        #endregion
    }
}
