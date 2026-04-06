namespace WIS.Automation.Galys
{
    /// <summary>
    /// Objeto request del alta de producto estandar para un automatismo
    /// </summary>
    public class CodigoBarraGalysRequest 
    {
        /// <summary>
        /// Predio
        /// </summary>
        public string codAlmacen { get; set; }
        
        /// <summary>
        /// Código de producto
        /// </summary>
        public string codArticulo { get; set; }

        public string codBarras { get; set; }
       
        /// <summary>
        /// Acción a tomar, A = Alta, M = Modificar o B = Borrar
        /// </summary>
        public string accion { get; set; }
    }
}
