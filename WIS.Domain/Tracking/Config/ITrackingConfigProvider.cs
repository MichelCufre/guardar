using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Tracking.Config
{
    public interface ITrackingConfigProvider
    {
        public Dictionary<string, string> GetConfig();
        public bool TrackingHabilitado();
    }
}
