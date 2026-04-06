using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Report
{
    public class ReportAtl
    {
        Document document;

        public ReportAtl()
        {
            this.document = new Document();
        }

        public virtual Section AddSection(PageFormat format, string leftMargin, string topMargin, string rightMargin, string bottomMargin)
        {
            var section = this.document.AddSection();

            section.PageSetup = document.DefaultPageSetup.Clone();
            section.PageSetup.PageFormat = PageFormat.A4;
            section.PageSetup.LeftMargin = leftMargin;
            section.PageSetup.RightMargin = rightMargin;
            section.PageSetup.TopMargin = topMargin;
            section.PageSetup.BottomMargin = bottomMargin;

            return section;
        }
        public void AddSpace(Section section, Unit height)
        {
            Paragraph p = section.AddParagraph();
            p.Format.LineSpacingRule = LineSpacingRule.Exactly;
            p.Format.LineSpacing = "0mm";
            p.Format.SpaceBefore = height;
        }
        public virtual Table AddSectionTable(Section section, double[] widths = null)
        {
            var table = section.AddTable();

            if (widths != null)
            {
                foreach (var width in widths)
                {
                    this.AddTableColumn(table, width);
                }
            }

            return table;
        }
        public virtual Table AddHeaderTable(HeaderFooter header, double[] widths = null)
        {
            var table = header.AddTable();

            if (widths != null)
            {
                foreach (var width in widths)
                {
                    this.AddTableColumn(table, width);
                }
            }

            return table;
        }
        public virtual Table AddNestedTable(Cell cell, double[] widths = null)
        {
            var table = cell.Elements.AddTable();

            if (widths != null)
            {
                var reference = cell.Column.Width.Centimeter;

                foreach (var width in widths)
                {
                    this.AddTableColumn(table, width, reference);
                }
            }

            return table;
        }

        public virtual void AddTableColumn(Table table, double width)
        {
            var usableWidth = table.Section.PageSetup.PageWidth.Centimeter - table.Section.PageSetup.LeftMargin.Centimeter - table.Section.PageSetup.RightMargin.Centimeter;

            table.AddColumn(Convert.ToString((usableWidth * width) / 100) + "cm");
        }
        public virtual void AddTableColumn(Table table, double width, double reference)
        {
            table.AddColumn(Convert.ToString((reference * width) / 100) + "cm");
        }
        public virtual Row AddTableRow(Table table, string label, string text1, string text2 = null, string text3 = null)
        {
            var row = table.AddRow();

            row.Shading.Color = ((table.Rows.Count % 2 != 0) ? Colors.LightGray : Colors.White);

            row.Cells[0].AddParagraph(label);
            row.Cells[1].AddParagraph(text1);

            if (!string.IsNullOrEmpty(text2))
            {
                row.Cells[2].AddParagraph(text2);

                if (!string.IsNullOrEmpty(text3))
                {
                    row.Cells[3].AddParagraph(text3);
                }
                else
                {
                    row.Cells[2].MergeRight = 1;
                }
            }
            else
            {
                row.Cells[1].MergeRight = 2;
            }

            return row;
        }
        public virtual Row AddTableRow(Table table, List<ReportCell> cellsToAdd)
        {
            var i = 0;

            var row = table.AddRow();

            foreach (var cell in cellsToAdd)
            {
                row.Cells[i].AddParagraph(cell.Value);
                row.Cells[i].Shading.Color = cell.BackgroundColor;
                row.Cells[i].Format.Alignment = cell.HorizontalAlign;

                i++;
            }

            return row;
        }
        public virtual Row AddTableHeaderRow(Table table, List<ReportCell> cellsToAdd)
        {
            var i = 0;

            var row = table.AddRow();
            row.HeadingFormat = true;

            foreach (var cell in cellsToAdd)
            {
                row.Cells[i].AddParagraph(cell.Value);
                row.Cells[i].Shading.Color = cell.BackgroundColor;
                row.Cells[i].Format.Alignment = cell.HorizontalAlign;

                i++;
            }

            return row;
        }
        public virtual Row AddTableRowPaging(Table table, string label, string separator)
        {
            var row = table.AddRow();
            row.Shading.Color = ((table.Rows.Count % 2 != 0) ? Colors.LightGray : Colors.White);

            Paragraph paragraph;

            paragraph = new Paragraph();
            paragraph.AddText(label);

            row.Cells[0].Add(paragraph);

            paragraph = new Paragraph();
            paragraph.AddPageField();
            paragraph.AddText(" " + separator + " ");
            paragraph.AddNumPagesField();

            row.Cells[1].Add(paragraph);
            row.Cells[1].MergeRight = 2;

            return row;
        }

        public virtual Row AddHeaderTableRowImage(Table table, string path, string height)
        {
            var row = table.AddRow();

            row.Cells[0].MergeRight = 2;
            row.Cells[0].Borders.Bottom.Visible = true;
            row.Cells[0].Borders.Bottom.Color = Colors.DarkGray;
            row.Cells[0].VerticalAlignment = VerticalAlignment.Center;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Left;

            Image image = row.Cells[0].AddImage(path);

            image.Height = height;
            image.LockAspectRatio = true;
            image.RelativeVertical = RelativeVertical.Line;
            image.RelativeHorizontal = RelativeHorizontal.Margin;
            image.Top = ShapePosition.Top;
            image.Left = ShapePosition.Right;
            image.WrapFormat.Style = WrapStyle.Through;

            return row;
        }

        public virtual Style GetNormalStyle()
        {
            return this.document.Styles["Normal"];
        }
        public virtual Style AddStyle(string styleName, string styleBase)
        {
            return this.document.AddStyle(styleName, styleBase);
        }

        public virtual byte[] RenderToByteArray()
        {
            byte[] content;

            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(true);

            pdfRenderer.Document = this.document;

            pdfRenderer.RenderDocument();

            using (MemoryStream stream = new MemoryStream())
            {
                pdfRenderer.Save(stream, true);
                content = stream.ToArray();
            }

            return content;
        }
    }
}
