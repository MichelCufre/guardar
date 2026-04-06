using WIS.Exceptions;

namespace WIS.GridComponent.Excel
{
    public class GridExcelImporterException : ExpectedException
    {
        public GridExcelImporterException(string texto, string[] strArguments = null) : base(texto, strArguments)
        {
        }

        public string Payload { get; }
        
        public GridExcelImporterException(string texto, string payload, string[] strArguments = null) : base(texto, strArguments)
        {
            Payload = payload;
        }
    }
}
