using System;
using System.Collections.Generic;
using System.Text;
using WIS.Common.API.Attributes;
using WIS.Domain.General.Filters;
using WIS.Domain.Validation;

namespace WIS.Domain.General.API.Dtos.Entrada
{
    public class EgresoRequest : IApiEntradaRequest
    {
        /// <summary>
        /// Código de empresa de la ejecución
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RequiredValidation]
        [ExisteEmpresaValidation()]
        public int Empresa { get; set; }

        /// <summary>
        /// Sirve para generar una referencia o un campo de búsqueda en el panel de ejecuciones de interfaces de WIS. Mediante este campo es posible identificar o buscar la traza de la ejecución de la interfaz. Ante un incidente o un procesamiento no esperado se puede reportar el problema haciendo referencia al valor de este campo.
        /// </summary>
        /// <example>Creación de Agenda</example>
        [ApiDtoExample("Creación de Egresos")]
        [RequiredValidation]
        [StringLengthValidation(200, MinimumLength = 1)]
        public string DsReferencia { get; set; }

        /// <summary>
        /// Generalmente se utiliza en sistemas externos que generan archivos y un middleware que oficia de intermediario entre el archivo y el API. 
        /// En este campo se puede guardar el nombre del archivo original en el cual vinieron los datos. En caso de que la implantación no utilice archivos se puede utilizar con otros fines.
        /// </summary>
        /// <example>Archivo</example>
        [StringLengthValidation(100)]
        [ApiDtoExample("Archivo")]
        public string Archivo { get; set; }

		/// <summary>
		/// Sirve para controlar la unicidad de las ejecuciones
		/// </summary>
		/// <example>123</example>
		[ApiDtoExample("123")]
		[StringLengthValidation(50, MinimumLength = 0)]
		public string IdRequest { get; set; }

		/// <summary>
		/// Lista de camiones
		/// </summary>
		[RequiredListValidation]
        public List<CamionRequest> Camiones { get; set; }

        public EgresoRequest()
        {
            Camiones = new List<CamionRequest>();
        }

        public void Add(IApiEntradaItemRequest item)
        {
            Camiones.Add(item as CamionRequest);
        }
    }

    public class CamionRequest : IApiEntradaItemRequest
    {
        /// <summary>
        /// Identificador externo del egreso enviado.
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [StringLengthValidation(50, MinimumLength = 0)]
        public string IdExterno { get; set; }

        /// <summary>
        /// Descripción del camión.
        /// Texto que describe el egreso, el ingresar un valor identificativo sería de ayuda al funcionario en la operativa.
        /// </summary>
        /// <example>TEXTO LIBRE</example>
        [ApiDtoExample("TEXTO LIBRE")]
        [RequiredValidation]
        [StringLengthValidation(50, MinimumLength = 1)]
        public string Descripcion { get; set; }

        /// <summary>
        /// Descripción sobre el documento del egreso.
        /// Sumamente informativo ya que se sobreescribirá al momento de generar el egreso documental.
        /// </summary>
        /// <example>DUA1234</example>
        [ApiDtoExample("DUA1234")]
        [StringLengthValidation(50, MinimumLength = 0)]
        public string Documento { get; set; }

        /// <summary>
        /// Número de Predio.
        /// Especifica el predio del depósito al que corresponde el egreso.
        /// No es requerido siempre y cuando el PredioExterno venga especificado.
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        //[RequiredValidation]
        [StringLengthValidation(10, MinimumLength = 0)]
        public string Predio { get; set; }

        /// <summary>
        /// Identificar externo del Predio.
        /// Especifica el predio del depósito al que corresponde el egreso.
        /// </summary>
        /// <example>1</example>   
        [ApiDtoExample("1")]
        [StringLengthValidation(50, MinimumLength = 0)]
        public string PredioExterno { get; set; }

        /// <summary>
        /// Código del vehículo del egreso.
        /// Requerido siempre y cuando el egreso maneje tracking.
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RangeValidation(10)]
        public int? CodigoVehiculo { get; set; }


        /// <summary>
        /// Matrícula de vehículo.
        /// Requerida cuando no se envíe un vehículo, de lo contrario se carga con la del vehículo enviado.
        /// </summary>
        /// <example>B-ABC123</example>
        [ApiDtoExample("B-ABC123")]
        [StringLengthValidation(15, MinimumLength = 0)]
        public string Matricula { get; set; }

        /// <summary>
        /// Código de la empresa transportista relacionada al egreso.
        /// Requerido cuando no se envíe un vehículo, de lo contrario se carga con él del vehículo enviado.
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RangeValidation(10)]
        public int? Transportista { get; set; }

        /// <summary>
        /// Indica el código de ruta del egreso, detallando a cuál recorrido pertenece la entrega.
        /// En caso de no enviar este dato se considerará  un egreso multi-ruta.
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RangeValidation(3)]
        public short? Ruta { get; set; }

        /// <summary>
        /// Código de empresa del egreso.
        /// En caso de no enviar este dato se considerará  un egreso multi-empresa.
        /// </summary>
        /// <example>1</example>      
        [ApiDtoExample("1")]
        [RangeValidation(10)]
        public int? Empresa { get; set; }

        /// <summary>
        /// Código de Puerta en la que sale el egreso.
        /// </summary>
        /// <example>10</example>
        [ApiDtoExample("10")]
        [RangeValidation(3)]
        public short? Puerta { get; set; }

        /// <summary>
        /// Fecha programada del egreso.
        /// </summary>
        /// <example>2022-02-15</example>
        [ApiDtoExample("2022-02-15")]
        public DateTime? ProgramacionFecha { get; set; }

        /// <summary>    
        /// De momento este dato no se persiste, está reservado para una funcionalidad futura.
        /// </summary>
        /// <example>10:00</example>
        [SwaggerIgnore]
        [ApiDtoExample("10:00")]
        [StringLengthValidation(5, MinimumLength = 0)]
        public string ProgramacionHoraInicio { get; set; }

        /// <summary>
        /// De momento este dato no se persiste, está reservado para una funcionalidad futura.
        /// </summary>
        /// <example>18:00</example>
        [SwaggerIgnore]
        [ApiDtoExample("18:00")]
        [StringLengthValidation(5, MinimumLength = 0)]
        public string ProgramacionHoraFin { get; set; }

        /// <summary>
        /// Necesidades del egreso.
        /// Ejemplo: Carga lateral, carga trasera, vehìculo chico, etc.
        /// De momento este dato no se persiste, está reservado para una funcionalidad futura.
        /// </summary>
        /// <example>TEXTO LIBRE</example>
        [SwaggerIgnore]
        [ApiDtoExample("TEXTO LIBRE")]
        [StringLengthValidation(15, MinimumLength = 0)]
        public string Necesidades { get; set; }

        /// <summary>
        /// Determina si se respeta el orden de carga.
        /// True por defecto.
        /// </summary>
        /// <example>false</example>
        [ApiDtoExample("false")]
        public bool? RespetaOrdenCarga { get; set; }

        /// <summary>
        /// Determina si el egreso maneja tracking.
        /// False por defecto.      
        /// </summary>
        /// <example>false</example>
        [ApiDtoExample("false")]
        public bool? Tracking { get; set; }

        /// <summary>
        /// Determina si el viaje se rutea con Tracking.
        /// True por defecto.
        /// </summary>
        /// <example>false</example>
        [ApiDtoExample("false")]
        public bool? Ruteo { get; set; }

        /// <summary>
        /// Determina si al cerrar o facturar el egreso se requiere que todos los contenedores asociados tengan un control finalizado.
        /// False por defecto.
        /// </summary>
        /// <example>false</example>
        [ApiDtoExample("false")]
        public bool? RequiereControlContenedores { get; set; }

        /// <summary>
        /// Determina si existe un viaje teorico en tracking para el egreso.
        /// False por defecto.
        /// </summary>
        /// <example>false</example>
        [ApiDtoExample("false")]
        [SwaggerIgnore]
        public bool? TrackingSincronizado { get; set; }

        /// <summary>
        /// Determina si está habilitado el cierre del egreso.
        /// True por defecto.
        /// </summary>
        /// <example>true</example>
        [ApiDtoExample("true")]
        public bool? CierreHabilitado { get; set; }

        /// <summary>
        /// Determina si está habilitada la posibilidad de cargar el egreso.
        /// True por defecto.
        /// </summary>
        /// <example>true</example>
        [ApiDtoExample("true")]
        public bool? CargaHabilitada { get; set; }

        /// <summary>
        /// Determina si está habilitado el cierre parcial del egreso.
        /// True por defecto.
        /// </summary>
        /// <example>true</example>
        [ApiDtoExample("true")]
        public bool? CierreParcial { get; set; }

        /// <summary>
        /// Habilita el cierre automático del egreso al completar la carga.
        /// False por defecto.
        /// </summary>
        /// <example>false</example>
        [ApiDtoExample("true")]
        public bool? CierreAutomatico { get; set; }

        /// <summary>
        /// Habilita al operador de depósito a agregar cargas mediante el colector aunque no estén en la definición.
        /// False por defecto.
        /// </summary>
        /// <example>false</example>
        [ApiDtoExample("false")]
        public bool? CargaLibre { get; set; }

        /// <summary>
        /// Habilita el armado de egreso manualmente.
        /// True por defecto.
        /// </summary>
        /// <example>true</example>
        [ApiDtoExample("false")]
        public bool? ArmadoHabilitado { get; set; }

        /// <summary>
        /// Habilita el uso de cargas que ya estén en otro egreso.
        /// Esto moverá las cargas del egreso existente al nuevo.
        /// False por defecto.
        /// </summary>
        /// <example>false</example>
        [ApiDtoExample("false")]
        public bool? HabilitarUsoCargaAsignada { get; set; }

        [ApiDtoExample("false")]
        public bool IgnorarCargasInexistentes { get; set; }

        public DetalleCamionRequest Detalles { get; set; }

        public CamionRequest()
        {
            Detalles = new DetalleCamionRequest();
        }
    }

    public class DetalleCamionRequest
    {
        public List<PedidoCamionRequest> Pedidos { get; set; }
        public List<CargaCamionRequest> Cargas { get; set; }
        public List<ContenedorCamionRequest> Contenedores { get; set; }

        public DetalleCamionRequest()
        {
            Pedidos = new List<PedidoCamionRequest>();
            Cargas = new List<CargaCamionRequest>();
            Contenedores = new List<ContenedorCamionRequest>();
        }
    }

    public class PedidoCamionRequest
    {
        /// <summary>
        /// Valor alfanumérico que representa un pedido en el sistema, conforma un identificador único junto al código de agente, tipo de agente y la empresa a la que pertenece.
        /// </summary>
        /// <example>PED100</example>
        [ApiDtoExample("PED100")]
        [RequiredValidation]
        [StringLengthValidation(40, MinimumLength = 1)]
        public string NroPedido { get; set; }

        /// <summary>
        /// Código del agente correspondiente al pedido.
        /// </summary>
        /// <example>AGE01</example>
        [ApiDtoExample("AGE01")]
        [RequiredValidation]
        [StringLengthValidation(10, MinimumLength = 1)]
        public string CodigoAgente { get; set; }

        /// <summary>
        /// Tipo de agente .
        /// PRO (Proveedor) - CLI (Cliente).
        /// </summary>
        /// <example>PRO</example>
        [ApiDtoExample("PRO")]
        [RequiredValidation]
        [StringLengthValidation(3, MinimumLength = 1)]
        public string TipoAgente { get; set; }

        /// <summary>
        /// Código de empresa del pedido.
        /// En caso que el egreso sea mono-empresa deben coincidir.
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RequiredValidation]
        [RangeValidation(10)]
        public int Empresa { get; set; }
    }
    public class CargaCamionRequest
    {
        /// <summary>
        /// Código del agente correspondiente a la carga que se le asignara al egreso.
        /// </summary>
        /// <example>AGE01</example>
        [ApiDtoExample("AGE01")]
        [RequiredValidation]
        [StringLengthValidation(10, MinimumLength = 1)]
        public string CodigoAgente { get; set; }

        /// <summary>
        /// Tipo de agente .
        /// PRO (Proveedor) - CLI (Cliente).
        /// </summary>
        /// <example>PRO</example>
        [ApiDtoExample("PRO")]
        [RequiredValidation]
        [StringLengthValidation(3, MinimumLength = 1)]
        public string TipoAgente { get; set; }

        /// <summary>
        /// Código de empresa.
        /// En caso que el egreso sea mono-empresa deben coincidir.
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RequiredValidation]
        [RangeValidation(10)]
        public int Empresa { get; set; }

        /// <summary>
        /// Número de carga que se asignará al egreso.
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RequiredValidation]
        [RangeValidation(15)]
        public long Carga { get; set; }
    }
    public class ContenedorCamionRequest
    {
        /// <summary>
        /// Identificador del contenedor definido por el cliente. 
        /// En caso de que el cliente no haya asignado un identificador propio, corresponde al número de contenedor asignado por WIS.
        /// </summary>
        /// <example>SSCC9000</example>
        [ApiDtoExample("SSCC900")]
        [RequiredValidation]
        [StringLengthValidation(50, MinimumLength = 1)]
        public string IdExternoContenedor { get; set; }

        /// <summary>
        /// Tipo de contenedor
        /// </summary>
        /// <example>W</example>
        [ApiDtoExample("W")]
        [StringLengthValidation(10, MinimumLength = 0)]
        public string TipoContenedor { get; set; }

        /// <summary>
        /// Empresa bajo la cual se identifica el contenedor mediante la prepación asocaida al mismo.
        /// </summary>
        /// <example>1</example>        
        [ApiDtoExample("1")]
        [RequiredValidation]
        [RangeValidation(10)]
        public int Empresa { get; set; }
    }
}
