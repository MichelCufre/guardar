using System.Collections.Generic;
using WIS.Common.API.Attributes;
using WIS.Domain.Validation;

namespace WIS.Domain.General.API.Dtos.Entrada
{
    public class AgentesRequest : IApiEntradaRequest
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
        /// <example>Creación de agentes</example>
        [ApiDtoExample("Creación de agentes")]
        [RequiredValidation]
        [StringLengthValidation(200, MinimumLength = 1)]
        public string DsReferencia { get; set; }

        /// <summary>
        /// Generalmente se utiliza en sistemas externos que generan archivos y un middleware que oficia de intermediario entre el archivo y el API. 
        /// En este campo se puede guardar el nombre del archivo original en el cual vinieron los datos. En caso de que la implantación no utilice archivos se puede utilizar con otros fines.
        /// </summary>
        /// <example>Archivo</example>
        [ApiDtoExample("Archivo")]
        [StringLengthValidation(100)]
        public string Archivo { get; set; }

		/// <summary>
		/// Sirve para controlar la unicidad de las ejecuciones
		/// </summary>
		/// <example>123</example>
		[ApiDtoExample("123")]
		[StringLengthValidation(50, MinimumLength = 0)]
		public string IdRequest { get; set; }

		/// <summary>
		/// Lista de agentes
		/// </summary>
		[RequiredListValidation]
        public List<AgenteRequest> Agentes { get; set; }

        public AgentesRequest()
        {
            Agentes = new List<AgenteRequest>();
        }

        public void Add(IApiEntradaItemRequest item)
        {
            Agentes.Add(item as AgenteRequest);
        }
    }
    public class AgenteRequest : IApiEntradaItemRequest
    {
        /// <summary>
        /// Es el código del proveedor o el código del cliente. Es el código que se visualizará en todas las pantallas y reportes de WIS
        /// </summary>
        /// <example>AGE01</example>
        [ApiDtoExample("AGE01")]
        [RequiredValidation]
        [StringLengthValidation(40, MinimumLength = 1)]
        public string CodigoAgente { get; set; }

        /// <summary>
        /// Tipo de agente
        /// CLI (Cliente) - PRO (Proveedor)
        /// </summary>
        /// <example>CLI</example>
        [ApiDtoExample("CLI")]
        [RequiredValidation]
        [StringLengthValidation(3, MinimumLength = 1)]
        public string Tipo { get; set; }

        /// <summary>
        /// Descripción / nombre / razón social del proveedor o cliente.
        /// </summary>
        /// <example>Agente de prueba</example>
        [ApiDtoExample("Agente de prueba")]
        [RequiredValidation]
        [StringLengthValidation(100, MinimumLength = 1)]
        public string Descripcion { get; set; }

        /// <summary>
        /// Es la ruta que se le asigna por defecto al Cliente para el armado del viaje de entrega de mercadería. El valor depende de la instalación de WIS. (Consulte valores disponibles con el responsable operativo de WIS). Por lo general "1" es el valor más usado.
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RangeValidation(3)]
        public short? Ruta { get; set; }

        /// <summary>
        /// Situación del agente. 
        /// 15 (Activo) - 16 (Inactivo)
        /// </summary>
        /// <example>15</example>
        [ApiDtoExample("15")]
        [RangeValidation(3)]
        public short? Estado { get; set; }

        /// <summary>
        /// Información anexa
        /// </summary>
        /// <example></example>
        [StringLengthValidation(200, MinimumLength = 0)]
        public string Anexo1 { get; set; }

        /// <summary>
        /// Información anexa
        /// </summary>
        /// <example></example>
        [StringLengthValidation(200, MinimumLength = 0)]
        public string Anexo2 { get; set; }

        /// <summary>
        /// Información anexa
        /// </summary>
        /// <example></example>
        [StringLengthValidation(200, MinimumLength = 0)]
        public string Anexo3 { get; set; }

        /// <summary>
        /// Información anexa
        /// </summary>
        /// <example></example>
        [StringLengthValidation(200, MinimumLength = 0)]
        public string Anexo4 { get; set; }

        /// <summary>
        /// Barrio
        /// </summary>
        /// <example>Centro</example>
        [ApiDtoExample("Centro")]
        [StringLengthValidation(50, MinimumLength = 0)]
        public string Barrio { get; set; }

        /// <summary>
        /// Dirección de entrega que se usará para la generación de puntos de entrega y ruteo en el módulo de tracking.
        /// El formato recomendable para mayor precisión al geolocalizar es:
        ///     Calle + Número, Código Postal + Localidad, Departamento, País
        /// </summary>
        /// <example> Arturo Santana 811, 20100 Punta del Este, Departamento de Maldonado,Uruguay</example>
        [ApiDtoExample("Arturo Santana 811, 20100 Punta del Este, Departamento de Maldonado,Uruguay")]
        [StringLengthValidation(100, MinimumLength = 0)]
        public string Direccion { get; set; }

        /// <summary>
        /// Acepta devolución 
        /// S (Sí) - N (No)
        /// </summary>
        /// <example>S</example>
        [ApiDtoExample("S")]
        [StringLengthValidation(1, MinimumLength = 0)]
        public string AceptaDevolucion { get; set; }

        /// <summary>
        /// Teléfono
        /// </summary>
        /// <example>42255555</example>
        [ApiDtoExample("42255555")]
        [StringLengthValidation(30, MinimumLength = 0)]
        public string TelefonoPrincipal { get; set; }

        /// <summary>
        /// Teléfono secundario
        /// </summary>
        /// <example>42255554</example>
        [ApiDtoExample("42255555")]
        [StringLengthValidation(30, MinimumLength = 0)]
        public string TelefonoSecundario { get; set; }

        /// <summary>
        /// Porcentaje de vida útil
        /// </summary>
        /// <example>10</example>
        [ApiDtoExample("10")]
        [RangeValidation(15, 3, false)]
        public decimal? ValorManejoVidaUtil { get; set; }

        /// <summary>
        /// Categoría de cliente
        /// </summary>
        /// <example>Categoria</example>
        [ApiDtoExample("Categoria")]
        [StringLengthValidation(10, MinimumLength = 0)]
        public string Categoria { get; set; }

        /// <summary>
        /// Código postal
        /// </summary>
        /// <example>20000</example>
        [ApiDtoExample("20000")]
        [StringLengthValidation(15, MinimumLength = 0)]
        public string CodigoPostal { get; set; }

        /// <summary>
        /// Número fiscal
        /// </summary>
        /// <example>2341</example>
        [ApiDtoExample("2341")]
        [StringLengthValidation(30, MinimumLength = 0)]
        public string NumeroFiscal { get; set; }

        /// <summary>
        /// El GLN, número de identificación del cliente o proveedor, es una clave de acceso a la información de la base de datos. Está diseñado para mejorar eficientemente la comunicación entre socios comerciales y clientes, añadiéndole valor a sus transacciones y beneficiando a los consumidores. Mas información en: https://gs1uy.org › 17-gln-numero-global-de-identificacion
        /// </summary>
        /// <example>1500000000004</example>
        [ApiDtoExample("1500000000004")]
        [RangeValidation(16)]
        public long? NumeroLocalizacionGlobal { get; set; }

        /// <summary>
        /// Grupo de Consulta
        /// </summary>
        /// <example>S/N</example>
        [ApiDtoExample("S/N")]
        [StringLengthValidation(20, MinimumLength = 0)]
        public string GrupoConsulta { get; set; }

        /// <summary>
        /// Punto de Entrega o código único de entrega para el cliente. Puede dejarse nulo, generalmente es asignado automáticamente cuando Tracking está instalado y georreferencia la dirección de entrega, asignándole un punto de entrega único.
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [StringLengthValidation(20, MinimumLength = 0)]
        public string PuntoDeEntrega { get; set; }

        /// <summary>
        /// Identifica al tipo de cliente filial
        /// Valores posibles:
        /// C - Cliente
        /// F - Filial
        /// P - Persona física
        /// </summary>
        /// <example>C</example>
        [ApiDtoExample("C")]
        [StringLengthValidation(1, MinimumLength = 0)]
        public string IdClienteFilial { get; set; }

        /// <summary>
        /// Tipo fiscal
        /// RUT (RUT) - LIBRE (LIBRE)
        /// </summary>
        /// <example>RUT</example>
        [ApiDtoExample("RUT")]
        [StringLengthValidation(20, MinimumLength = 0)]
        public string TipoFiscal { get; set; }

        /// <summary>
        /// Característica telefónica
        /// </summary>
        /// <example>2341</example>
        [ApiDtoExample("2341")]
        [StringLengthValidation(15, MinimumLength = 0)]
        public string CaracteristicaTelefonica { get; set; }

        /// <summary>
        /// Dato Fiscal
        /// </summary>
        /// <example>Dato fiscal</example>
        [ApiDtoExample("Dato fiscal")]
        [StringLengthValidation(30, MinimumLength = 0)]
        public string OtroDatoFiscal { get; set; }

        /// <summary>
        /// En el caso que los clientes se ordenan en una ruta, este valor corresponde al orden de carga del cliente dentro de la ruta. Si no se utiliza esta funcionalidad especifique 1 o 0
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RangeValidation(3)]
        public short? OrdenDeCarga { get; set; }

        /// <summary>
        /// Código de país (ISO 3166-1)
        /// </summary>
        /// <example>UY</example>
        [ApiDtoExample("UY")]
        [StringLengthValidation(2, MinimumLength = 0)]
        public string Pais { get; set; }

        /// <summary>
        /// Subdivisión - (ISO 3166-2)
        /// </summary>
        /// <example>UY-MA</example>
        [ApiDtoExample("UY-MA")]
        [StringLengthValidation(15, MinimumLength = 0)]
        public string Subdivision { get; set; }

        /// <summary>
        /// Municipio localidad
        /// </summary>
        /// <example>PDE</example>
        [ApiDtoExample("PDE")]
        [StringLengthValidation(20, MinimumLength = 0)]
        public string Localidad { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        /// <example>jhon.doe@acme.com</example>
        [ApiDtoExample("jhon.doe@acme.com")]
        [StringLengthValidation(100, MinimumLength = 0)]
        public string Email { get; set; }
    }
}
