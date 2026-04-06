using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WIS.Filtering;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Columns;
using WIS.Persistence.Database;
using WIS.Sorting;

namespace WIS.Domain.DataModel.Mappers
{
    public class GridConfigMapper : Mapper
    {
        private readonly List<GridColumnType> TypeNotDsColumn = new List<GridColumnType> { GridColumnType.Button, GridColumnType.ItemList };

        public virtual List<IGridColumn> MapToUserColumns(GridColumnFactory columnFactory, List<T_GRID_USER_CONFIG> userConfig)
        {
            List<IGridColumn> colList = new List<IGridColumn>();

            foreach (var col in userConfig)
            {
                GridColumnType columnType = this.MapColumnType(col.VL_TYPE);

                IGridColumn column = columnFactory.Create(columnType);

                column.Name = col.DS_COLUMNA;
                column.Id = col.NM_DATAFIELD;
                column.Hidden = !this.MapStringToBoolean(col.FL_VISIBLE);
                column.Fixed = this.MapFixedPosition(col.VL_POSICION_FIJADO);
                column.Width = col.VL_WIDTH ?? 100;
                column.Type = columnType;
                column.TextAlign = this.MapTextAlign(col.VL_ALINEACION);
                column.Order = col.NU_ORDEN_VISUAL ?? 0;
                column.Translate = this.MapStringToBoolean(col.FL_TRADUCIR);

                colList.Add(column);
            }

            return colList;
        }
        public virtual List<IGridColumn> MapToDefaultColumns(GridColumnFactory columnFactory, List<T_GRID_DEFAULT_CONFIG> defaultConfig)
        {
            List<IGridColumn> colList = new List<IGridColumn>();

            foreach (var col in defaultConfig)
            {
                GridColumnType columnType = this.MapColumnType(col.VL_TYPE);

                IGridColumn column = columnFactory.Create(columnType);

                column.Name = col.DS_COLUMNA;
                column.Id = col.NM_DATAFIELD;
                column.Hidden = !this.MapStringToBoolean(col.FL_VISIBLE);
                column.Fixed = this.MapFixedPosition(col.VL_POSICION_FIJADO);
                column.Width = col.VL_WIDTH ?? 100;
                column.Type = columnType;
                column.TextAlign = this.MapTextAlign(col.VL_ALINEACION);
                column.Order = col.NU_ORDEN_VISUAL ?? 0;
                column.Translate = this.MapStringToBoolean(col.FL_TRADUCIR);

                colList.Add(column);
            }

            return colList;
        }

        public virtual List<IGridColumn> MapApiColumns(GridColumnFactory columnFactory, List<string> properties)
        {
            List<IGridColumn> colList = new List<IGridColumn>();

            foreach (var name in properties)
            {
                IGridColumn column = columnFactory.Create(GridColumnType.Text);

                column.Name = name;
                column.Id = name;
                column.Insertable = true;
                column.Type = GridColumnType.Text;
                colList.Add(column);
            }

            return colList;
        }

        public virtual IGridColumn MapPropertyToColum(PropertyInfo prop)
        {
            return new GridColumn
            {
                Id = prop.Name,
                Name = prop.Name,
                IsNew = true,
                Type = this.MapProperyTypeToColumnType(prop.PropertyType),
            };
        }

        public virtual T_GRID_USER_CONFIG MapToUserEntity(string gridId, string application, int userId, IGridColumn column)
        {
            return new T_GRID_USER_CONFIG
            {
                NM_DATAFIELD = column.Id,
                CD_APLICACION = application,
                USERID = userId,
                CD_BLOQUE = gridId,
                DS_COLUMNA = column.Name,
                VL_WIDTH = column.Width,
                NU_ORDEN_VISUAL = column.Order,
                FL_VISIBLE = this.MapBooleanToString(!column.Hidden),
                VL_POSICION_FIJADO = (short)column.Fixed,
                VL_TYPE = this.MapColumnType(column.Type),
                VL_ALINEACION = this.MapTextAlign(column.TextAlign),
                FL_TRADUCIR = this.MapBooleanToString(column.Translate)
            };
        }
        public virtual T_GRID_DEFAULT_CONFIG MapToDefaultEntity(string gridId, string application, IGridColumn column)
        {
            return new T_GRID_DEFAULT_CONFIG
            {
                NM_DATAFIELD = column.Id,
                CD_APLICACION = application,
                CD_BLOQUE = gridId,
                DS_COLUMNA = (TypeNotDsColumn.Contains(column.Type)) ? "" : this.MapDefaultGridDescription(application, gridId, column.Name),
                VL_WIDTH = column.Width,
                NU_ORDEN_VISUAL = column.Order,
                FL_VISIBLE = this.MapBooleanToString(!column.Hidden),
                VL_POSICION_FIJADO = (short)column.Fixed,
                VL_TYPE = this.MapColumnType(column.Type),
                VL_ALINEACION = this.MapTextAlign(column.TextAlign),
                FL_TRADUCIR = this.MapBooleanToString(column.Translate)
            };
        }

        public virtual T_GRID_FILTER MapToFilterEntity(GridFilterData data, string application, int userId)
        {
            var newFilter = new T_GRID_FILTER
            {
                CD_APLICACION = application,
                USERID = userId,
                CD_BLOQUE = data.GridId,
                DS_FILTRO = data.Description,
                NM_FILTRO = data.Name,
                VL_FILTRO_AVANZADO = data.ExplicitFilter,
                DT_ADDROW = DateTime.Now,
                FL_GLOBAL = this.MapBooleanToString(data.IsGlobal),
                FL_INICIAL = this.MapBooleanToString(data.IsDefault)
            };

            foreach (var filter in data.Filters)
            {
                var filterDetail = new T_GRID_FILTER_DET
                {
                    CD_COLUMNA = filter.ColumnId,
                    VL_FILTRO = filter.Value
                };

                newFilter.T_GRID_FILTER_DET.Add(filterDetail);
            }

            int index = 1;

            foreach (var sort in data.Sorts)
            {
                var detail = newFilter.T_GRID_FILTER_DET.Where(d => d.CD_COLUMNA == sort.ColumnId).FirstOrDefault();

                if (detail == null)
                {
                    detail = new T_GRID_FILTER_DET
                    {
                        CD_FILTRO = newFilter.CD_FILTRO,
                        CD_COLUMNA = sort.ColumnId,
                    };

                    newFilter.T_GRID_FILTER_DET.Add(detail);
                }

                detail.VL_ORDEN = (short)sort.Direction;
                detail.NU_ORDEN_EJECUCION = index;

                index++;
            }

            return newFilter;
        }
        public virtual GridFilterData MapToFilterObject(T_GRID_FILTER data)
        {
            var filterData = new GridFilterData
            {
                Id = data.CD_FILTRO,
                Name = data.NM_FILTRO,
                Description = data.DS_FILTRO,
                Date = data.DT_ADDROW,
                ExplicitFilter = data.VL_FILTRO_AVANZADO,
                GridId = data.CD_BLOQUE,
                IsDefault = this.MapStringToBoolean(data.FL_INICIAL),
                IsGlobal = this.MapStringToBoolean(data.FL_GLOBAL)
            };

            if (data.T_GRID_FILTER_DET != null)
            {
                foreach (var detalle in data.T_GRID_FILTER_DET.Where(d => d.VL_FILTRO != null))
                {
                    filterData.Filters.Add(new FilterCommand(detalle.CD_COLUMNA, detalle.VL_FILTRO));
                }

                foreach (var detalle in data.T_GRID_FILTER_DET.Where(d => d.VL_ORDEN != null).OrderBy(d => d.NU_ORDEN_EJECUCION))
                {
                    filterData.Sorts.Add(new SortCommand(detalle.CD_COLUMNA, (SortDirection)detalle.VL_ORDEN));
                }
            }

            return filterData;
        }

        public virtual T_GRID_USER_CONFIG MapFromDefaultConfig(T_GRID_DEFAULT_CONFIG defaultConfig)
        {
            return new T_GRID_USER_CONFIG
            {
                CD_APLICACION = defaultConfig.CD_APLICACION,
                CD_BLOQUE = defaultConfig.CD_BLOQUE,
                DS_COLUMNA = defaultConfig.DS_COLUMNA,
                DS_DATA_FORMAT_STRING = defaultConfig.DS_DATA_FORMAT_STRING,
                VL_POSICION_FIJADO = defaultConfig.VL_POSICION_FIJADO,
                FL_VISIBLE = defaultConfig.FL_VISIBLE,
                NM_DATAFIELD = defaultConfig.NM_DATAFIELD,
                NU_ORDEN_VISUAL = defaultConfig.NU_ORDEN_VISUAL,
                RESOURCEID = defaultConfig.RESOURCEID,
                VL_ALINEACION = defaultConfig.VL_ALINEACION,
                VL_LINK = defaultConfig.VL_LINK,
                VL_TYPE = defaultConfig.VL_TYPE,
                VL_WIDTH = defaultConfig.VL_WIDTH,
                FL_TRADUCIR = defaultConfig.FL_TRADUCIR
            };
        }

        public virtual string MapColumnType(GridColumnType type)
        {
            switch (type)
            {
                case GridColumnType.Text:
                    return "ST";
                case GridColumnType.DateTime:
                    return "DT";
                case GridColumnType.Date:
                    return "DO";
                case GridColumnType.Checkbox:
                    return "CK";
                case GridColumnType.Button:
                    return "BA";
                case GridColumnType.ItemList:
                    return "BL";
                case GridColumnType.Progress:
                    return "PG";
                case GridColumnType.Select:
                    return "SL";
                case GridColumnType.SelectAsync:
                    return "SA";
                case GridColumnType.Toggle:
                    return "TG";
            }

            return "ST";
        }

        public virtual GridColumnType MapProperyTypeToColumnType(Type type)
        {
            if (type.Equals(typeof(DateTime)) || type.Equals(typeof(DateTime?)))
                return GridColumnType.DateTime;

            return GridColumnType.Text;
        }

        public virtual GridColumnType MapColumnType(string columnType)
        {
            if (columnType == null)
                throw new Exception("Tipo de columna no definido");

            switch (columnType)
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
                case "SL":
                    return GridColumnType.Select;
                case "SA":
                    return GridColumnType.SelectAsync;
                case "TG":
                    return GridColumnType.Toggle;
            }

            return GridColumnType.Text;
        }
        public virtual string MapTextAlign(GridTextAlign textAlign)
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

            return dbAlign;
        }
        public virtual GridTextAlign MapTextAlign(string textAlign)
        {
            if (string.IsNullOrEmpty(textAlign))
                return GridTextAlign.Left;

            switch (textAlign)
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
        public virtual GridFixPosition MapFixedPosition(short? fixedPosition)
        {
            return (GridFixPosition)(fixedPosition ?? 1);
        }


        public virtual string MapDefaultGridDescription(string app, string gridId, string columnName)
        {
            string[] splitGridId = gridId.Split('_');
            return $"{app}_{splitGridId[1]}{splitGridId[2]}_colname_{columnName}";
        }
    }
}
