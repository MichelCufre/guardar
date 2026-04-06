using MigraDoc.DocumentObjectModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Report
{
    public class ReportCell
    {
        public string Value { get; set; }
        public Color BackgroundColor { get; set; }
        public ParagraphFormat Format { get; set; }
        public int MergeRight { get; set; }
        public ParagraphAlignment HorizontalAlign { get; set; }

        public ReportCell()
        {
        }

        public ReportCell(string value, Color backgroundColor, ParagraphFormat format = null, int colspan = 0)
        {
            this.Value = value;
            this.BackgroundColor = backgroundColor;
            this.Format = format ?? new ParagraphFormat();
            this.MergeRight = colspan > 1 ? colspan - 1 : 0;
        }
        public ReportCell(string value, Color backgroundColor, ParagraphAlignment horizontalAlign = ParagraphAlignment.Left)
        {
            this.Value = value;
            this.BackgroundColor = backgroundColor;
            this.HorizontalAlign = horizontalAlign;
        }
    }
}
