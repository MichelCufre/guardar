using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Tracking.Models;

namespace WIS.Domain.Tracking.Config
{
    public class TrackingConfigProvider : ITrackingConfigProvider
    {
        protected readonly IUnitOfWorkFactory _uowFactory;

        public TrackingConfigProvider(IUnitOfWorkFactory uowFactory)
        {
            this._uowFactory = uowFactory;
        }

        public virtual Dictionary<string, string> GetConfig()
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                Dictionary<string, string> parameters = uow.ParametroRepository.GetParameters(new List<string>
                {
                    ParamManager.TRACKING_API,
                    ParamManager.TRACKING_SCOPE,
                    ParamManager.TRACKING_TENANT,
                    ParamManager.TRACKING_ACTIVO,
                    ParamManager.TRACKING_API_WMS,
                    ParamManager.TRACKING_API_USERS,
                    ParamManager.TRACKING_CLIENT_ID,
                    ParamManager.TRACKING_GRANT_TYPE,
                    ParamManager.TRACKING_CLIENT_SECRET,
                    ParamManager.TRACKING_AGRUPACION_CD,
                    ParamManager.TRACKING_TP_CONT_DEFAULT,
                    ParamManager.TRACKING_TAREA_CANT_DIAS,
                    ParamManager.TRACKING_ACCESS_TOKEN_URL,
                    ParamManager.TRACKING_TP_CONT_FICTICIO,
                    ParamManager.TRACKING_JOB_FECHA_INICIAL,
                });

                Dictionary<string, string> config = new Dictionary<string, string>()
                {
                    { "url", parameters[ParamManager.TRACKING_API] },
                    { "scope", parameters[ParamManager.TRACKING_SCOPE] },
                    { "tenant", parameters[ParamManager.TRACKING_TENANT] },
                    { "urlApiWMS", parameters[ParamManager.TRACKING_API_WMS] },
                    { "clientId", parameters[ParamManager.TRACKING_CLIENT_ID] },
                    { "granType", parameters[ParamManager.TRACKING_GRANT_TYPE] },
                    { "urlApiUsers", parameters[ParamManager.TRACKING_API_USERS] },
                    { "estadoTracking", parameters[ParamManager.TRACKING_ACTIVO] },
                    { "tpCont", parameters[ParamManager.TRACKING_TP_CONT_DEFAULT] },
                    { "agrupacionCD", parameters[ParamManager.TRACKING_AGRUPACION_CD] },
                    { "clientSecret", parameters[ParamManager.TRACKING_CLIENT_SECRET] },
                    { "cantDiasTarea", parameters[ParamManager.TRACKING_TAREA_CANT_DIAS] },
                    { "urlAccesToken", parameters[ParamManager.TRACKING_ACCESS_TOKEN_URL] },
                    { "fechaInicial", parameters[ParamManager.TRACKING_JOB_FECHA_INICIAL] },
                    { "tpContFicticio", parameters[ParamManager.TRACKING_TP_CONT_FICTICIO] },
                };

                return config;
            }
        }

        public virtual bool TrackingHabilitado()
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                return uow.ParametroRepository.GetParameter(ParamManager.TRACKING_ACTIVO) == CEstadoTracking.Activo ? true : false;
            }
        }
    }
}
