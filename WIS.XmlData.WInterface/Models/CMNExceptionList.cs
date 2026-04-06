using DocumentFormat.OpenXml.Spreadsheet;

namespace WIS.XmlData.WInterface.Models
{
    public class CMNExceptionList : Exception
    {
        public List<CMNException> Items { set; get; }

        public CMNExceptionList()
        {
            Items = new List<CMNException>();
        }

        public void Add(CMNException ex, string property = "")
        {
            ex.Nivel = 0;
            ex.Property = property;
            Items.Add(ex);
        }

        public void Add(string message, string property = "")
        {
            CMNException ex = new CMNException(message);
            ex.Nivel = 0;
            ex.Property = property;
            Items.Add(ex);
        }

        public void Order()
        {
            Items = Items.OrderBy(x => x.Nivel).ToList();
        }

        public bool HasErrors()
        {
            if (this.Items != null && this.Items.Count > 0)
                return true;

            return false;
        }
    }
}
