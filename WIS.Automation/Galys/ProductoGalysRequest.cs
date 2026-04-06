namespace WIS.Automation.Galys
{
    /// <summary>
    /// Objeto request del alta de producto estandar para un automatismo
    /// </summary>
    public class ProductoGalysRequest 
    {
        /// <summary>
        /// Predio
        /// </summary>
        public string codAlmacen { get; set; }

        /// <summary>
        /// Código de producto
        /// </summary>
        public string codArticulo { get; set; }

        /// <summary>
        /// Descripción del producto
        /// </summary>
        public string denomArticulo { get; set; }

        /// <summary>
        /// Flag de manejo de lote
        /// </summary>
        public bool gestionLoteEntrada { get; set; }

        /// <summary>
        /// Flag de mano de vencimiento
        /// </summary>
        public bool gestionCaducidadEntrada { get; set; }

        /// <summary>
        /// Flag para lectura código de barras en la salida
        /// </summary>
        public bool leerCdBSalidas { get; set; }

        /// <summary>
        /// Peso
        /// </summary>
        public decimal? peso { get; set; }

        /// <summary>
        /// Ancho
        /// </summary>
        public int? dimensionXEnvase { get; set; }

        /// <summary>
        /// Altura
        /// </summary>
        public int? dimensionYEnvase { get; set; }

        /// <summary>
        /// Profundidad
        /// </summary>
        public int? dimensionZEnvase { get; set; }

        /// <summary>
        /// UDC
        /// </summary>
        public string udc { get; set; } //Tipo de subdivisión de la caja AutoStore donde se almacena. ej. CAJA1/8

        /// <summary>
        /// Unidades por UDC
        /// </summary>
        public int? udsUdc { get; set; } //Unidades que caben en su correspondinte subdivisión        

        /// <summary>
        /// Unidad bulto producto
        /// </summary>
        public decimal? UdsUde { get; set; }

        /// <summary>
        /// Acción a tomar, A = Alta o B = Borrar
        /// </summary>
        public string estado { get; set; }
    }
}
