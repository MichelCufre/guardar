using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace WIS.GridComponent.Excel
{
    public class LoadFromCollection<T>
    {
        private IList<T> collection;
        private LoadFromCollectionParams parameters;
        private IXLWorksheet worksheet;

        public LoadFromCollection(IXLWorksheet worksheet, IList<T> collection, LoadFromCollectionParams parameters)
        {
            this.collection = collection;
            this.parameters = parameters;
            this.worksheet = worksheet;
        }

        public virtual IXLRange Load()
        {
            var range = worksheet.Range(1, 1, collection.Count() + 1, parameters.Members.Count());
            range.Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium); //Generamos las lineas exteriores
            range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left; //Alineamos horizontalmente
            #region Header

            for (int j = 0; j < parameters.Members.Count(); j++)
            {
                range.Cell(1, j + 1).Value = parameters.Members[j].Name;
                range.Cell(1, j + 1).Style.Font.Bold = true;
                range.Cell(1, j + 1).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
            }

            #endregion

            #region Body

            for (int i = 0; i < collection.Count(); i++)
            {
                var item = collection[i];

                for (int j = 0; j < parameters.Members.Count(); j++)
                {
                    var property = (PropertyInfo)(parameters.Members[j] is PropertyInfo ? parameters.Members[j] : null);
                    object obj = property?.GetValue(item);
                    if (obj != null)
                    {
                        range.Cell(i + 2, j + 1).Value = GetCellValue(obj);
                    }
                    else
                        range.Cell(i + 2, j + 1).Value = "";
                }
            }

            #endregion
            range.RangeUsed().SetAutoFilter();

            return range;
        }

        public virtual XLCellValue GetCellValue(object obj)
        {
            var type = obj.GetType();

            if (type.Equals(typeof(DateTime)))
                return (DateTime)obj;
            else if (type.Equals(typeof(bool)))
                return (bool)obj;
            else if (type.Equals(typeof(long)))
                return (long)obj;
            else if (type.Equals(typeof(float)))
                return (float)obj;
            else if (type.Equals(typeof(decimal)))
                return (decimal)obj;
            else if (type.Equals(typeof(double)))
                return (double)obj;
            else if (type.Equals(typeof(short)))
                return (short)obj;
            else if (type.Equals(typeof(int)))
                return (int)obj;
            else
                return obj.ToString();
        }
    }
}