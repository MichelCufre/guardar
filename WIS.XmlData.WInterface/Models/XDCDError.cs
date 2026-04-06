using System.ComponentModel;

namespace WIS.XmlData.WInterface.Models
{
    public enum XDCDError
    {
        [Description("WIE-000::Los datos de Entrada son null")]
        ENTIDAD_NULL,

        [Description("WIE-001::Error Genérico, msg: {0}")]
        GENERICO_AL_VALIDAR,

        [Description("WIE-002::No se pudo iniciar sesión, Usuario o contraseña Inválido")]
        NO_LOGIN,

        [Description("WIE-003::Token Inválido")]
        TOKEN_INVALIDO,

        [Description("WIE-004::Código Interfaz Externa Inválida")]
        CD_INTERFAZ_EXTERNA_INVALIDA,

        [Description("WIE-005::Empresa Inválida")]
        EMPRESA,

        [Description("WIE-006::La Empresa no está asignada al usario")]
        EMPRESA_USUARIO,

        [Description("WIE-007::El nodo DATA no contiene Información")]
        DATA_VACIO,

        [Description("WIE-008::Número de Interfaz Inválido")]
        NU_INTERFAZ_EJECUCION_INVALIDO,

        [Description("WIE-009::El nodo Finaliza ejecución inválido, valor esperado TRUE | FALSE")]
        FINALIZA_EJECUCION_INVALIDO,

        [Description("WIE-010::Número de Interfaz tiene Errores, no se puede modificar")]
        FL_ERROR_CARGA_S,

        [Description("WIE-011::Ya existe este número de paquete")]
        NU_PAQUETE_EXISTENTE,

        [Description("WIE-012::Número de paquete inválido")]
        NU_PAQUETE_INVALIDO,

        [Description("WIE-013::Número total de paquetes inválido")]
        TOTAL_PAQUETES_INVALIDO,

        [Description("WIE-014::Faltan paquetes para generar el XML, {0}")]
        FALTAN_PAQUETES,

        [Description("WIE-015::No se encntró paquete número: {0} al generar el XML")]
        NO_SE_ENCONTRO_PAQUETE_ALGENERAR_XML,

        [Description("WIE-016::No se pudo convertir Base64 a XML")]
        ERROR_BASE64_TO_STRINGXML,

        [Description("WIE-017::Los datos recibidos por interfaz no corresponden con la estructura XML esperada")]
        XML_EXTERNO_RECIBIDO_INVALIDO,

        [Description("WIE-018::No se encontró información sobre este paquete")]
        PAQUETE_SIN_INFORMACION,

        [Description("WIE-019::Nodo SESION distinto de I | F")]
        SESION_NODO_ERROR,

        [Description("WIE-020::El Número de Interfaz tiene un estado que no se puede modificar")]
        NU_INTERFAZ_EJECUCION_ESTADO_INVALIDO,

        [Description("WIE-021::Token Inválido, el valor del mismo no puede ser NULL")]
        TOKEN_INVALIDO_NULL,

        [Description("WIE-022::El usuario está bloqueado, contactar a un administrador")]
        USER_BLOQUEADO,

        [Description("WIE-023::Número de Interfaz Null")]
        NU_INTERFAZ_EJECUCION_NULL,

        [Description("WIE-024::Código Interfaz Externa Null")]
        CD_INTERFAZ_EXTERNA_NULL,

        [Description("WIE-025::Empresa  NULL")]
        CD_EMPRESA_NULL,

        [Description("WIE-026::El nodo DS_REFERENCIA debe tener un valor")]
        DS_REFERENCIA_NULL,

        [Description("WIE-027::La Empresa no esta habilitada para utilizar interfaz")]
        EMPRESA_INTERFAZ,

        [Description("WIE-028::La estructura xml no es correcta(Fallos en etiquetas)")]
        ERROR_ESTRUCTURA_XML,

        [Description("WIE-029::El valor DS_REFERENCIA duplicado, NU_INTERFAZ_EJECUCION: {0}.")]
        DS_REFERENCIA_REPETIDA,

        [Description("WIE-030::No se encuentra Párametro en la Base de Datos, Nombre:{0}.")]
        FALTA_PARAMETRO_EN_DB,
    }
}
