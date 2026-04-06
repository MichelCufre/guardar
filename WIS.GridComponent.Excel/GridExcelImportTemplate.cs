using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WIS.Translation;

namespace WIS.GridComponent.Excel
{
    public class GridExcelImportTemplate
    {
        private readonly string _fileName;
        private readonly Dictionary<string, List<IGridColumn>> _sheetColums;
        private readonly ITranslator _translator;
        private readonly int? _interfazExterna;
        private byte[] _data;

        public GridExcelImportTemplate(string fileName, List<IGridColumn> columns, ITranslator translator, int? interfazExterna = null)
        {
            this._fileName = fileName;
            this._sheetColums = new Dictionary<string, List<IGridColumn>>();
            this._translator = translator;
            _interfazExterna = interfazExterna;
            this._sheetColums["IExcel_Template_lbl_SheetData"] = columns;
        }

        public GridExcelImportTemplate(string fileName, Dictionary<string, List<IGridColumn>> sheetColumns, ITranslator translator, int? interfazExterna = null)
        {
            this._fileName = fileName;
            this._sheetColums = sheetColumns;
            this._translator = translator;
            this._interfazExterna = interfazExterna;
        }

        public void Generate()
        {
            using (XLWorkbook workbook = new XLWorkbook())
            {
                Dictionary<string, string> translations = this.GetTranslatedValues();

                foreach (var key in this._sheetColums.Keys)
                {
                    var columns = this._sheetColums[key];

                    var traduccion = translations[key] ?? string.Empty;

                    if (traduccion == key)
                        traduccion = GetValueDefault(key);

                    if (!string.IsNullOrEmpty(traduccion) && traduccion.Length > 30)
                        traduccion = traduccion.Substring(0, 30);

                    var worksheet = workbook.Worksheets.Add(traduccion);

                    int index = 1;

                    foreach (var column in columns.Where(d => d.Insertable))
                    {
                        if (!column.AllowsFiltering)
                            continue;

                        worksheet.Cell(1, index).Value = column.Id;

                        worksheet.Cell(1, index).Style.Fill.PatternType = XLFillPatternValues.Solid;
                        worksheet.Cell(1, index).Style.Fill.BackgroundColor = XLColor.LightGray;
                        worksheet.Cell(1, index).Style.Font.Bold = true;

                        worksheet.Cell(2, index).Value = translations[column.Name];
                        worksheet.Cell(2, index).Style.Fill.PatternType = XLFillPatternValues.Solid;
                        worksheet.Cell(2, index).Style.Fill.BackgroundColor = XLColor.LightGray;
                        worksheet.Cell(2, index).Style.Font.Bold = true;

                        worksheet.Column(index).Style.NumberFormat.Format = "@";

                        index++;
                    }

                    worksheet.Row(1).Hide();
                    FreezePanes(worksheet, 2, 0);
                    worksheet.Columns().AdjustToContents();
                }

                this.AddReferenceSheet(workbook, translations);

                SetByteArray(workbook);
            }
        }

        public virtual void FreezePanes(IXLWorksheet worksheet, int rows, int columnns)
        {
            worksheet.SheetView.Freeze(rows, columnns);
        }

        private void AddReferenceSheet(XLWorkbook workbook, Dictionary<string, string> translations)
        {
            string sheet = translations["IExcel_Template_lbl_SheetReferences"];
            var worksheet = workbook.Worksheets.Add(sheet);

            worksheet.Cell(2, 2).Value = translations["IExcel_Template_lbl_Referencias"];
            worksheet.Cell(2, 2).Style.Font.Bold = true;

            worksheet.Cell(3, 2).Value = "Interfaz Externa:";
            worksheet.Cell(3, 2).Style.Font.Bold = true;
            worksheet.Cell(3, 3).Value = _interfazExterna;

            worksheet.Cell(5, 2).Value = translations["IExcel_Template_lbl_Notas"];
            worksheet.Cell(5, 2).Style.Font.Bold = true;

            worksheet.Cell(6, 2).Value = translations["IExcel_Template_lbl_NoAlterarPrimerasFilas"];
            worksheet.Cell(7, 2).Value = translations["IExcel_Template_lbl_NoCambiarPosicionHoja"];
            worksheet.Cell(8, 2).Value = translations["IExcel_Template_lbl_FormatoFechas"];
            worksheet.Cell(9, 2).Value = translations["IExcel_Template_lbl_FormatoDecimales"];

            worksheet.Cell(11, 2).Style.Fill.PatternType = XLFillPatternValues.Solid;
            worksheet.Cell(11, 2).Style.Fill.BackgroundColor = XLColor.Gray;
            worksheet.Cell(11, 3).Value = translations["IExcel_Template_lbl_NormalValues"];

            worksheet.Cell(12, 2).Style.Fill.PatternType = XLFillPatternValues.Solid;
            worksheet.Cell(12, 2).Style.Fill.BackgroundColor = XLColor.Brown;
            worksheet.Cell(12, 3).Value = translations["IExcel_Template_lbl_ModifiedValues"];

            worksheet.Cell(13, 2).Style.Fill.PatternType = XLFillPatternValues.Solid;
            worksheet.Cell(13, 2).Style.Fill.BackgroundColor = XLColor.Red;
            worksheet.Cell(13, 3).Value = translations["IExcel_Template_lbl_ErrorValues"];
        }

        private Dictionary<string, string> GetTranslatedValues()
        {
            List<string> valuesToTranslate = new List<string>();

            foreach (var key in this._sheetColums.Keys)
            {
                var columns = this._sheetColums[key];
                valuesToTranslate.AddRange(columns.Select(d => d.Name));
            }

            valuesToTranslate.AddRange(new List<string> {
                GridExcelSheetNames.SheetDataName,
                "IExcel_Template_lbl_SheetReferences",
                "IExcel_Template_lbl_Referencias",
                "IExcel_Template_lbl_Notas",
                "IExcel_Template_lbl_NoAlterarPrimerasFilas",
                "IExcel_Template_lbl_NoCambiarPosicionHoja",
                "IExcel_Template_lbl_FormatoFechas",
                "IExcel_Template_lbl_FormatoDecimales",
                "IExcel_Template_lbl_NormalValues",
                "IExcel_Template_lbl_ModifiedValues",
                "IExcel_Template_lbl_ErrorValues",
                GridExcelSheetNames.SheetDetailName,
                GridExcelSheetNames.SheetDuplicateName,
                GridExcelSheetNames.SheetAtributosName,
                GridExcelSheetNames.SheetAtributosDetailName,
                GridExcelSheetNames.SheetBarrasName,
                GridExcelSheetNames.SheetLpnName,
                GridExcelSheetNames.SheetDetalleLpnName,
                GridExcelSheetNames.SheetAtributosLpnName,
                GridExcelSheetNames.SheetAtributosLpnDetailName,
                GridExcelSheetNames.SheetProduccion,
                GridExcelSheetNames.SheetProduccionInsumo,
                GridExcelSheetNames.SheetProduccionProducto,
                GridExcelSheetNames.SheetControles,
                GridExcelSheetNames.SheetCriterios
            });

            return this._translator.Translate(valuesToTranslate);
        }

        public virtual void SetByteArray(XLWorkbook workbook)
        {
            using (var ms = new MemoryStream())
            {
                workbook.SaveAs(ms);
                this._data = ms.ToArray();
            }
        }
        public byte[] GetAsByteArray()
        {
            return this._data;
        }

        public static string GetValueDefault(string key)
        {
            var value = key;

            switch (key)
            {
                case GridExcelSheetNames.SheetDataName:
                    value = "Datos";
                    break;
                case "IExcel_Template_lbl_SheetReferences":
                    value = "Referencia";
                    break;
                case "IExcel_Template_lbl_Referencias":
                    value = "Referencias:";
                    break;
                case "IExcel_Template_lbl_Notas":
                    value = "NOTAS:";
                    break;
                case "IExcel_Template_lbl_NoAlterarPrimerasFilas":
                    value = "No alterar las primeras dos filas del documento.";
                    break;
                case "IExcel_Template_lbl_NoCambiarPosicionHoja":
                    value = "No cambiar de posición la hoja de datos.";
                    break;
                case "IExcel_Template_lbl_FormatoFechas":
                    value = "Los formatos de fechas aceptados son: [yyyy-MM-dd], [yyyy/MM/dd], [MM/dd/yyyy] y [MM-dd-yyyy]";
                    break;
                case "IExcel_Template_lbl_FormatoDecimales":
                    value = "Los decimales se deben enviar con punto";
                    break;
                case "IExcel_Template_lbl_NormalValues":
                    value = "Representa los valores autocompletados a modo de información";
                    break;
                case "IExcel_Template_lbl_ModifiedValues":
                    value = "Representa los valores que fueron modificados debido a la validación de los datos";
                    break;
                case "IExcel_Template_lbl_ErrorValues":
                    value = "Representa los valores en los que se encontraron errores, a estos se les agrega un comentario, describiendo el error";
                    break;
                case GridExcelSheetNames.SheetDetailName:
                    value = "Detalles";
                    break;
                case GridExcelSheetNames.SheetDuplicateName:
                    value = "Duplicados";
                    break;
                case GridExcelSheetNames.SheetAtributosName:
                    value = "Atributos";
                    break;
                case GridExcelSheetNames.SheetAtributosDetailName:
                    value = "Atributos Detalle";
                    break;
                case GridExcelSheetNames.SheetBarrasName:
                    value = "Barras";
                    break;
                case GridExcelSheetNames.SheetLpnName:
                    value = "LPN";
                    break;
                case GridExcelSheetNames.SheetDetalleLpnName:
                    value = "Detalle LPN";
                    break;
                case GridExcelSheetNames.SheetAtributosLpnName:
                    value = "Atributos LPN";
                    break;
                case GridExcelSheetNames.SheetAtributosLpnDetailName:
                    value = "Atributos LPN Detalle";
                    break;
                case GridExcelSheetNames.SheetProduccion:
                    value = "Produccion";
                    break;
                case GridExcelSheetNames.SheetProduccionInsumo:
                    value = "Insumos";
                    break;
                case GridExcelSheetNames.SheetProduccionProducto:
                    value = "Productos";
                    break;
                case GridExcelSheetNames.SheetControles:
                    value = "Controles";
                    break;
                case GridExcelSheetNames.SheetCriterios:
                    value = "Criterios";
                    break;
            }
            return value;
        }
    }
}
