using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using WIS.BackendService.Configuration;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Columns;

namespace WIS.BackendService.Services
{
    public class GridConfigProvider : IGridConfigProvider
    {
        private readonly IUnitOfWorkFactory _uowFactory;
        private readonly string _maxSelectAsyncResultsParam;

        public GridConfigProvider(IUnitOfWorkFactory uowFactory, IOptions<SelectSettings> settings)
        {
            this._uowFactory = uowFactory;
            this._maxSelectAsyncResultsParam = settings.Value.MaxSelectAsyncResults;
        }

        public List<IGridColumn> GetColumns(string application, int userId, string gridId)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            var columnFactory = new GridColumnFactory();
            return uow.GridConfigRepository.GetColumns(gridId, columnFactory);
        }

        public Dictionary<string, List<IGridColumn>> GetApiColumns(int interfazExterna, out string filename)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            return uow.GridConfigRepository.GetApiColumns(interfazExterna, out filename);
        }

        public List<IGridColumn> GetColumnsFromEntity<T>(List<IGridColumn> columnsSource)
        {
            List<IGridColumn> columns = new List<IGridColumn>();

            var type = typeof(T);

            var properties = type.GetProperties();

            var mapper = new GridConfigMapper();

            foreach (var prop in properties)
            {
                if (columnsSource.Any(d => d.Id == prop.Name))
                    continue;

                columns.Add(mapper.MapPropertyToColum(prop));
            }

            return columns;
        }

        public GridFilterData GetFilterData(string application, int userId, string gridId)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return uow.GridConfigRepository.GetDefaultFilter(gridId);
        }

        public void SaveColumns(Grid grid, string application, int userId)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (grid.Columns.Count == 0)
                return;

            var currentOrder = grid.Columns.Max(d => d.Order);

            foreach (var column in grid.Columns.Where(d => d.IsNew))
            {
                column.Order = currentOrder++;

                uow.GridConfigRepository.AddDefaultConfig(grid.Id, column);
            }

            uow.SaveChanges();
        }

        public int GetSelectResultLimit()
        {
            if (string.IsNullOrEmpty(this._maxSelectAsyncResultsParam) || !int.TryParse(this._maxSelectAsyncResultsParam, out int result))
                return 20;

            return result;
        }

        public bool IsModoConsulta(int userid)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            Dictionary<string, string> colParams = new Dictionary<string, string>();
            colParams[ParamManager.PARAM_USER] = string.Format("{0}_{1}", ParamManager.PARAM_USER, userid);
            bool modoLectura = (uow.ParametroRepository.GetParameter(ParamManager.MODO_CONSULTA, colParams) ?? "N")== "S" ? true: false; 
            return modoLectura;
        }

        public bool IsPantallaModoConsulta(int userid,string pagina)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            Dictionary<string, string> colParams = new Dictionary<string, string>();
            colParams[ParamManager.PARAM_USER] = string.Format("{0}_{1}", ParamManager.PARAM_USER, userid);
            List<string> paginaModoConsulta = (uow.ParametroRepository.GetParameter(ParamManager.PANTALLAS_MODO_CONSULTA, colParams) ?? "N").Split(GeneralDb.SeparadorGuion).ToList();
            bool modoLectura = paginaModoConsulta.Any(x =>x == pagina);
            return modoLectura;
        }
    }
}
