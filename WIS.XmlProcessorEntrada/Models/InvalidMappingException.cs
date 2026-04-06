namespace WIS.XmlProcessorEntrada.Models
{
    public class InvalidMappingException : Exception
    {
        public InvalidMappingException() { }

        public InvalidMappingException(string fieldName) : base($"No es posible procesar el valor especificado para el campo {fieldName}") { }
    }
}
