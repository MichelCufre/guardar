using System.ServiceModel;
using WIS.XmlData.WInterface.Models;

namespace WIS.XmlData.WInterface.Services.Interfaces
{
    [ServiceContract(Namespace = "https://www.gicla.com/wisinterface")]
    public interface IWisInterface
    {
        #region Sesion

        [OperationContract]
        Task<RESPUESTA_INTERFAZ> Sesion(INTERFAZ_SESSION INTERFAZ);

        #endregion

        #region Entrada

        [OperationContract]
        Task<RESPUESTA_INTERFAZ> Inciar_Ejecucion(INTERFAZ INTERFAZ);

        [OperationContract]
        Task<RESPUESTA_INTERFAZ> Enviar_Datos(INTERFAZ INTERFAZ);

        [OperationContract]
        Task<RESPUESTA_INTERFAZ> Consultar_Estado(INTERFAZ INTERFAZ);

        #endregion

        #region Salida

        [OperationContract]
        Task<RESPUESTA_INTERFAZ> Ejecuciones_Pendientes(INTERFAZ INTERFAZ);

        [OperationContract]
        Task<RESPUESTA_INTERFAZ> Consultar_Ejecucion(INTERFAZ INTERFAZ);

        [OperationContract]
        Task<RESPUESTA_INTERFAZ> Ejecucion_Leida(INTERFAZ INTERFAZ);

        #endregion

        #region Consulta

        [OperationContract]
        Task<RESPUESTA_INTERFAZ> Consultar_Datos(INTERFAZ INTERFAZ);

        #endregion

        #region Mensaje

        [OperationContract]
        Task<RESPUESTA_INTERFAZ> Notificacion(INTERFAZ INTERFAZ);

        #endregion
    }
}
