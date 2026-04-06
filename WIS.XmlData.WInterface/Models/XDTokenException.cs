namespace WIS.XmlData.WInterface.Models
{
    public class XDTokenException : Exception
    {
        List<XDErrorCodigo> _errores;
        public List<XDErrorCodigo> Errores { get { return _errores; } }

        public XDTokenException(XDCDError cd_error, string extradesc = null)
            : base("")
        {
            _errores = new List<XDErrorCodigo>();
            XDErrorCodigo error = new XDErrorCodigo(cd_error, extradesc);
            _errores.Add(error);
        }

    }
}
