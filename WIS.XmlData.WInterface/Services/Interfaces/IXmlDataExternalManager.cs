using WIS.XmlData.WInterface.Models;

namespace WIS.XmlData.WInterface.Services.Interfaces
{
    public interface IXmlDataExternalManager
    {
        Task<RESPUESTA_INTERFAZ> Sesion(INTERFAZ_SESSION interfaz);
        Task<RESPUESTA_INTERFAZ> Inciar_Ejecucion(INTERFAZ interfaz);
        Task<RESPUESTA_INTERFAZ> Enviar_Datos(INTERFAZ interfaz);
        Task<RESPUESTA_INTERFAZ> Consultar_Estado(INTERFAZ interfaz);
        Task<RESPUESTA_INTERFAZ> Ejecuciones_Pendientes(INTERFAZ interfaz);
        Task<RESPUESTA_INTERFAZ> Consultar_Ejecucion(INTERFAZ interfaz);
        Task<RESPUESTA_INTERFAZ> Ejecucion_Leida(INTERFAZ interfaz);
        Task<RESPUESTA_INTERFAZ> Consultar_Datos(INTERFAZ interfaz);
        Task<RESPUESTA_INTERFAZ> Notificacion(INTERFAZ interfaz);
    }
}
