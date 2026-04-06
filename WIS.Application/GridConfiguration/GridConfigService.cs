using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Exceptions;
using WIS.GridComponent.Build;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Execution.Serialization;

namespace WIS.Application.GridConfiguration
{
    public class GridConfigService : IGridConfigService
    {
        protected readonly IUnitOfWorkFactory _uowFactory;

        public GridConfigService(IUnitOfWorkFactory uowFactory)
        {
            this._uowFactory = uowFactory;
        }

        public virtual void UpdateGridConfig(GridWrapper data)
        {
            var configData = data.GetData<GridUpdateConfigContext>();

            using var uow = this._uowFactory.GetUnitOfWork();

            uow.GridConfigRepository.Update(configData.GridId, configData.Columns);

            uow.SaveChanges();
        }

        public virtual void SaveFilter(GridWrapper data)
        {
            var filterData = data.GetData<GridFilterData>();

            if (string.IsNullOrEmpty(filterData.Name))
                throw new MissingParameterException("General_Sec0_Error_NombreFiltroRequerido");

            using var uow = this._uowFactory.GetUnitOfWork();

            if (filterData.IsDefault)
            {
                var defaultFilter = uow.GridConfigRepository.GetDefaultFilter(data.GridId);

                if (defaultFilter != null)
                    throw new ValidationFailedException("General_Sec0_lbl_Error_DUPLICATE");
            }

            if (uow.GridConfigRepository.AnyFilter(filterData.Name, data.User))
                throw new ValidationFailedException("General_Sec0_Error_NombreFiltroExistente");

            uow.GridConfigRepository.SaveFilter(filterData);

            uow.SaveChanges();
        }

        public virtual void RemoveFilter(GridWrapper data)
        {
            var request = data.GetData<GridFilterRemoveContext>();

            using var uow = this._uowFactory.GetUnitOfWork();

            //uow.GridConfigRepository.RemoveFilter(request.FilterId);
            uow.GridConfigRepository.RemoveFilter(request.FilterId, data.User);

            uow.SaveChanges();
        }

        public virtual List<GridFilterData> GetFilterList(GridWrapper data)
        {
            var filterData = data.GetData<GridGetFilterListContext>();

            using var uow = this._uowFactory.GetUnitOfWork();

            return uow.GridConfigRepository.GetFilterList(filterData.GridId);
        }
    }
}
