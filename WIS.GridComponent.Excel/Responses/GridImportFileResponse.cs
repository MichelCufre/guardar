using System;
using System.Collections.Generic;
using System.Text;
using WIS.Components.Common;

namespace WIS.GridComponent.Excel.Responses
{
    public class GridImportFileResponse : ComponentContext
    {
        public string FileName { get; set; }
        public string FileContent { get; set; }

        public void SetExcelContent(byte[] content)
        {
            this.FileContent = Convert.ToBase64String(content);
        }

        public byte[] GetExcelContent()
        {
            return Convert.FromBase64String(this.FileContent);
        }
    }
}
