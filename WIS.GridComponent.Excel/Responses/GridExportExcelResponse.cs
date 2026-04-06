using System;
using System.Collections.Generic;
using System.Text;
using WIS.Components.Common;

namespace WIS.GridComponent.Excel.Responses
{
    public class GridExportExcelResponse : ComponentContext
    {
        public string FileName { get; set; }
        public string ExcelContent { get; set; }

        public void SetExcelContent(byte[] content)
        {
            this.ExcelContent = Convert.ToBase64String(content);
        }

        public byte[] GetExcelContent()
        {
            return Convert.FromBase64String(this.ExcelContent);
        }
    }
}
