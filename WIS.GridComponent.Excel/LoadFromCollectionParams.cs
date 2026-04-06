using ClosedXML.Excel;
using System.Reflection;

namespace WIS.GridComponent.Excel
{
    public class LoadFromCollectionParams
    {
        public bool PrintHeaders { get; set; }
        public IXLStyle TableStyle { get; set; }
        public BindingFlags BindingFlags { get; set; }
        public MemberInfo[] Members { get; set; }
    }
}