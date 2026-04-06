namespace WIS.XmlData.WInterface.Models
{
    public class XDInterfazException : Exception
    {
        public XDInterfazException(XDCDError cd_error, string extradesc = null)
            : base("")
        {
            _errores = new List<XDErrorCodigo>();
            XDErrorCodigo error = new XDErrorCodigo(cd_error, extradesc);
            _errores.Add(error);
        }

        public void AddError(XDCDError cd_error, string extradesc = null)
        {
            if (_errores == null) _errores = new List<XDErrorCodigo>();
            XDErrorCodigo error = new XDErrorCodigo(cd_error, extradesc);
            _errores.Add(error);

        }

        List<XDErrorCodigo> _errores;
        public List<XDErrorCodigo> Errores { get { return _errores; } }
    }
}
