using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.GridComponent;
using WIS.GridComponent.Columns;

namespace WIS.Persistence.Database
{
    public abstract class GridColumnConfigPartial
    {
        public virtual string DS_COLUMNA { get; set; }
        public virtual string NM_DATAFIELD { get; set; }
        public virtual short? VL_POSICION_FIJADO { get; set; }
        public virtual string VL_TYPE { get; set; }
        public virtual string VL_ALINEACION { get; set; }
        public virtual short? NU_ORDEN_VISUAL { get; set; }
        public virtual decimal? VL_WIDTH { get; set; }
        public virtual string FL_VISIBLE { get; set; }
        
        public GridFixPosition GetFixPosition()
        {
            return (GridFixPosition)(this.VL_POSICION_FIJADO ?? 1);
        }
        public GridColumnType GetColumnType()
        {
            if (this.VL_TYPE == null)
                throw new Exception("Tipo de columna no definido");

            switch (this.VL_TYPE)
            {
                case "ST":
                    return GridColumnType.Text;
                case "CK":
                    return GridColumnType.Checkbox;
                case "DT":
                    return GridColumnType.DateTime;
                case "DO":
                    return GridColumnType.Date;
                case "BA":
                    return GridColumnType.Button;
                case "BL":
                    return GridColumnType.ItemList;
                case "PG":
                    return GridColumnType.Progress;
            }

            return GridColumnType.Text;
        }
        public GridTextAlign GetTextAlign()
        {
            if (this.VL_ALINEACION == null)
                return GridTextAlign.Left;

            switch (this.VL_ALINEACION)
            {
                case "D":
                    return GridTextAlign.Right;
                case "I":
                    return GridTextAlign.Left;
                case "C":
                    return GridTextAlign.Center;
            }

            return GridTextAlign.Left;
        }
        public short GetOrder()
        {
            return this.NU_ORDEN_VISUAL ?? 0;
        }
        public decimal GetWidth()
        {
            return this.VL_WIDTH ?? 100;
        }

        public void SetFixPosition(GridFixPosition position)
        {
            this.VL_POSICION_FIJADO = (short)position;
        }
        public void SetColumnType(GridColumnType type)
        {
            string dbType = "ST";

            switch (type)
            {
                case GridColumnType.Text:
                    dbType = "ST";
                    break;
                case GridColumnType.DateTime:
                    dbType = "DT";
                    break;
                case GridColumnType.Date:
                    dbType = "DO";
                    break;
                case GridColumnType.Checkbox:
                    dbType = "CK";
                    break;
                case GridColumnType.Button:
                    dbType = "BA";
                    break;
                case GridColumnType.ItemList:
                    dbType = "BL";
                    break;
                case GridColumnType.Progress:
                    dbType = "PG";
                    break;
            }

            this.VL_TYPE = dbType;
        }
        public void SetTextAlign(GridTextAlign textAlign)
        {
            string dbAlign = null;

            switch (textAlign)
            {
                case GridTextAlign.Right:
                    dbAlign = "D";
                    break;
                case GridTextAlign.Left:
                    dbAlign = "I";
                    break;
                case GridTextAlign.Center:
                    dbAlign = "C";
                    break;
            }

            this.VL_ALINEACION = dbAlign;
        }
        public void SetVisible(bool visible)
        {
            this.FL_VISIBLE = visible ? "S" : "N";
        }

        public bool IsVisible()
        {
            return FL_VISIBLE == "S";
        }
    }
}
