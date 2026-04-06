using iText.IO.Font.Constants;
using iText.Kernel.Events;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace WIS.Report
{
	public class PageEventHandler : IEventHandler
    {
		protected PdfFormXObject placeholder;
		protected float side = 20;
		protected float x = 300;
		protected float y = 25;
		protected float space = 4.5f;
		protected float descent = 3;

		public PageEventHandler()
		{
			placeholder = new PdfFormXObject(new Rectangle(0, 0, side, side));
		}

		public void HandleEvent(Event currentEvent)
		{
			PdfDocumentEvent docEvent = (PdfDocumentEvent)currentEvent;
			PdfDocument pdfDoc = docEvent.GetDocument();

			int pageNum = pdfDoc.GetPageNumber(docEvent.GetPage());
			Rectangle pageSize = docEvent.GetPage().GetPageSize();
			PdfCanvas pdfCanvas = new PdfCanvas(docEvent.GetPage());

			Canvas canvas = new Canvas(pdfCanvas, pageSize);
			canvas.SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).SetFontSize(10);
			Paragraph p = new Paragraph().Add("Página ").Add(pageNum.ToString()).Add(" de");
			canvas.ShowTextAligned(p, x, y, TextAlignment.RIGHT);
			canvas.Close();
			pdfCanvas.AddXObjectAt(placeholder, x + space, y - descent);
			pdfCanvas.Release();
		}

		public void WriteTotalPages(PdfDocument pdfDoc)
		{
			Canvas canvas = new Canvas(placeholder, pdfDoc);
			canvas.SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).SetFontSize(10);
			canvas.ShowTextAligned(pdfDoc.GetNumberOfPages().ToString(), 0, descent, TextAlignment.LEFT);
			canvas.Close();
		}
	}
}
