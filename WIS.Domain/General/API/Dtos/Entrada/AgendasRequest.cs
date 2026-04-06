using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WIS.Common.API.Attributes;
using WIS.Domain.Validation;

namespace WIS.Domain.General.API.Dtos.Entrada
{
    public class AgendasRequest : IApiEntradaRequest
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
        [ApiDtoExample("Creación de Agenda")]
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
		/// Lista de agendas
		/// </summary>
		[RequiredListValidation]
        public List<AgendaRequest> Agendas { get; set; }

        public AgendasRequest()
        {
            Agendas = new List<AgendaRequest>();
        }

        public void Add(IApiEntradaItemRequest item)
        {
            Agendas.Add(item as AgendaRequest);
        }
    }

    public class AgendaRequest : IApiEntradaItemRequest
    {
        /// <summary>
        /// Código de agente
        /// </summary>
        /// <example>AGE01</example>
        [ApiDtoExample("AGE01")]
        [RequiredValidation]
        [StringLengthValidation(40, MinimumLength = 1)]
        public string CodigoAgente { get; set; }

        /// <summary>
        /// El tipo de Agente aceptado depende del tipo de recepción.
        /// Tipos de recepción de devolución, tipo de agente CLI (Cliente), tipos de recepción normales PRO (Proveedor)
        /// </summary>
        /// <example>PRO</example>
        [ApiDtoExample("PRO")]
        [RequiredValidation]
        [StringLengthValidation(3, MinimumLength = 1)]
        public string TipoAgente { get; set; }

        /// <summary>
        /// Tipos de recepción permitidos:
        /// -SOC: Mono OC
        /// -REFREC: Mono RR
        /// -MOC: Múltiple OC
        /// -SOD: Múltiple(OD)
        /// -DIGLIB: Digitación Libre
        /// -DEVOD: Devolución mono(OD)
        /// -DDIGLI: Devolución Digitada Libre
        /// </summary>
        /// <example>SOC</example>
        [ApiDtoExample("SOC")]
        [RequiredValidation]
        [StringLengthValidation(6, MinimumLength = 1)]
        public string TipoRecepcion { get; set; }

        /// <summary>
        /// Referencia
        /// En caso de que el tipo de recepción sea libre este campo no es requerido, 
        /// de lo contrario la referencia deberá existir y ser compatible con el tipo.
        /// </summary>
        /// <example>238</example>
        [ApiDtoExample("238")]
        [StringLengthValidation(30, MinimumLength = 0)]
        public string Referencia { get; set; }

        /// <summary>
        /// Tipos de referencia permitidos:
        /// -OC	Orden de compra
        /// -OD Orden de devolución
        /// -RR Referencia recepción
        /// </summary>
        /// <example>OC</example>
        [ApiDtoExample("OC")]
        [StringLengthValidation(6, MinimumLength = 0)]
        public string TipoReferencia { get; set; }

        /// <summary>
        /// Número de Predio
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RequiredValidation]
        [StringLengthValidation(10, MinimumLength = 1)]
        public string Predio { get; set; }

        /// <summary>
        /// Configurar liberacion automatica de la agenda
        /// </summary>
        /// <example>true</example>
        public bool? LiberarAgenda { get; set; }

        /// <summary>
        /// Puerta asignada a la agenda
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
		public short? PuertaDesembarco { get; set; }

        /// <summary>
        /// Información anexa
        /// </summary>
        /// <example>Anexo 1</example>
        [ApiDtoExample("Anexo 1")]
        [StringLengthValidation(200, MinimumLength = 0)]
        public string Anexo1 { get; set; }

        /// <summary>
        /// Información anexa
        /// </summary>
        /// <example>Anexo 2</example>
        [ApiDtoExample("Anexo 2")]
        [StringLengthValidation(200, MinimumLength = 0)]
        public string Anexo2 { get; set; }

        /// <summary>
        /// Información anexa
        /// </summary>
        /// <example></example>
        [ApiDtoExample("Anexo 3")]
        [StringLengthValidation(200, MinimumLength = 0)]
        public string Anexo3 { get; set; }

        /// <summary>
        /// Información anexa
        /// </summary>
        /// <example></example>
        [ApiDtoExample("Anexo 4")]
        [StringLengthValidation(200, MinimumLength = 0)]
        public string Anexo4 { get; set; }

        /// <summary>
        /// Placa de vehículo
        /// </summary>
        /// <example>B-ABC123</example>
        [ApiDtoExample("B-ABC123")]
        [RequiredValidation]
        [StringLengthValidation(30, MinimumLength = 0)]
        public string PlacaVehiculo { get; set; }

        /// <summary>
        /// Fecha de entrega
        /// La misma no puede ser menor a la fecha actual
        /// </summary>
        /// <example>2025-10-15</example>
        [ApiDtoExample("2025-10-15")]
        public DateTime? FechaEntrega { get; set; }

    }
}
